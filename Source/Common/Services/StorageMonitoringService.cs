using System.Collections.Concurrent;
using System.Diagnostics;

using Common.Models;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Common.Services;

/// <summary>
/// Service for monitoring storage operations and collecting metrics
/// </summary>
public interface IStorageMonitoringService {
    /// <summary>
    /// Records a storage operation metric
    /// </summary>
    Task RecordOperationAsync(StorageOperationMetric metric, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets storage metrics for a time period
    /// </summary>
    Task<StorageMetrics> GetMetricsAsync(TimeSpan period, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets storage health status
    /// </summary>
    Task<StorageHealthStatus> GetHealthStatusAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets storage usage statistics
    /// </summary>
    Task<StorageUsageStatistics> GetUsageStatisticsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Starts monitoring a storage operation
    /// </summary>
    IStorageOperationMonitor StartOperationMonitoring(string operationName, string containerName, string? fileName = null);

    /// <summary>
    /// Checks if storage thresholds are exceeded
    /// </summary>
    Task<List<StorageThresholdAlert>> CheckThresholdsAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Interface for monitoring individual storage operations
/// </summary>
public interface IStorageOperationMonitor : IDisposable {
    /// <summary>
    /// Records operation success
    /// </summary>
    void RecordSuccess(long? bytesTransferred = null);

    /// <summary>
    /// Records operation failure
    /// </summary>
    void RecordFailure(Exception exception);

    /// <summary>
    /// Records operation completion
    /// </summary>
    void RecordCompletion(bool success, long? bytesTransferred = null, Exception? exception = null);
}

/// <summary>
/// Background service for periodic storage monitoring
/// </summary>
public class StorageMonitoringBackgroundService(
    IServiceProvider serviceProvider,
    IOptions<StorageConfiguration> configuration,
    ILogger<StorageMonitoringBackgroundService> logger) : BackgroundService {
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly StorageConfiguration _configuration = configuration.Value;
    private readonly ILogger<StorageMonitoringBackgroundService> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        while (!stoppingToken.IsCancellationRequested) {
            try {
                using var scope = _serviceProvider.CreateScope();
                var monitoringService = scope.ServiceProvider.GetRequiredService<IStorageMonitoringService>();

                // Check thresholds and send alerts
                var alerts = await monitoringService.CheckThresholdsAsync(stoppingToken);

                foreach (var alert in alerts) {
                    _logger.LogWarning("Storage threshold exceeded: {AlertType} - {Message}", alert.AlertType, alert.Message);

                    // In production, this would send alerts via email, SMS, etc.
                    await SendAlertAsync(alert);
                }
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error in storage monitoring background service");
            }

            await Task.Delay(_configuration.Monitoring.MetricsInterval, stoppingToken);
        }
    }

    private async Task SendAlertAsync(StorageThresholdAlert alert)
        // Implementation would send alerts to configured recipients
        => _logger.LogInformation("Alert sent: {AlertType} - {Message}", alert.AlertType, alert.Message);
}

/// <summary>
/// Implementation of storage monitoring service
/// </summary>
public class StorageMonitoringService(
    IStorageService storageService,
    ICacheService cacheService,
    IOptions<StorageConfiguration> configuration,
    ILogger<StorageMonitoringService> logger) : IStorageMonitoringService {
    private readonly IStorageService _storageService = storageService;
    private readonly ICacheService _cacheService = cacheService;
    private readonly StorageConfiguration _configuration = configuration.Value;
    private readonly ILogger<StorageMonitoringService> _logger = logger;
    private readonly ConcurrentDictionary<string, StorageOperationMetric> _recentMetrics = new();

    public async Task RecordOperationAsync(StorageOperationMetric metric, CancellationToken cancellationToken = default) {
        try {
            if (!_configuration.Monitoring.EnableMetrics) {
                return;
            }

            // Store metric in cache for recent access
            var key = $"storage_metric:{metric.OperationName}:{metric.Timestamp.Ticks}";
            await _cacheService.SetAsync(key, metric, TimeSpan.FromHours(24), cancellationToken);

            // Also store in memory for quick access
            _recentMetrics.TryAdd(key, metric);

            // Clean up old metrics
            var cutoffTime = DateTimeOffset.UtcNow.AddHours(-24);
            var keysToRemove = _recentMetrics
                .Where(kvp => kvp.Value.Timestamp < cutoffTime)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var keyToRemove in keysToRemove) {
                _recentMetrics.TryRemove(keyToRemove, out _);
            }
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error recording storage operation metric");
        }
    }

    public async Task<StorageMetrics> GetMetricsAsync(TimeSpan period, CancellationToken cancellationToken = default) {
        try {
            var cutoffTime = DateTimeOffset.UtcNow.Subtract(period);
            var relevantMetrics = _recentMetrics.Values
                .Where(m => m.Timestamp >= cutoffTime)
                .ToList();

            var metrics = new StorageMetrics {
                Period = period,
                GeneratedAt = DateTimeOffset.UtcNow,
                TotalOperations = relevantMetrics.Count,
                SuccessfulOperations = relevantMetrics.Count(m => m.IsSuccess),
                FailedOperations = relevantMetrics.Count(m => !m.IsSuccess),
                TotalBytesTransferred = relevantMetrics.Sum(m => m.BytesTransferred),
                AverageResponseTime = relevantMetrics.Any()
                    ? TimeSpan.FromMilliseconds(relevantMetrics.Average(m => m.ResponseTime.TotalMilliseconds))
                    : TimeSpan.Zero,
                OperationsByType = relevantMetrics
                    .GroupBy(m => m.OperationType)
                    .ToDictionary(g => g.Key, g => g.Count()),
                ErrorsByType = relevantMetrics
                    .Where(m => !m.IsSuccess)
                    .GroupBy(m => m.ErrorType ?? "Unknown")
                    .ToDictionary(g => g.Key, g => g.Count())
            };

            return metrics;
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error getting storage metrics");
            return new StorageMetrics { Period = period, GeneratedAt = DateTimeOffset.UtcNow };
        }
    }

    public async Task<StorageHealthStatus> GetHealthStatusAsync(CancellationToken cancellationToken = default) {
        try {
            var healthStatus = new StorageHealthStatus {
                CheckedAt = DateTimeOffset.UtcNow,
                OverallStatus = HealthStatus.Healthy
            };

            // Check recent operation success rate
            var recentMetrics = _recentMetrics.Values
                .Where(m => m.Timestamp >= DateTimeOffset.UtcNow.AddMinutes(-5))
                .ToList();

            if (recentMetrics.Any()) {
                var successRate = (double)recentMetrics.Count(m => m.IsSuccess) / recentMetrics.Count;
                healthStatus.SuccessRate = successRate;

                if (successRate < 0.95) // Less than 95% success rate
                {
                    healthStatus.OverallStatus = HealthStatus.Degraded;
                }

                if (successRate < 0.8) // Less than 80% success rate
                {
                    healthStatus.OverallStatus = HealthStatus.Unhealthy;
                }
            }

            // Check average response time
            if (recentMetrics.Any()) {
                var avgResponseTime = TimeSpan.FromMilliseconds(recentMetrics.Average(m => m.ResponseTime.TotalMilliseconds));
                healthStatus.AverageResponseTime = avgResponseTime;

                if (avgResponseTime > _configuration.Monitoring.Thresholds.MaxResponseTime) {
                    healthStatus.OverallStatus = HealthStatus.Degraded;
                }
            }

            // Check error rate
            var errorRate = recentMetrics.Any()
                ? (double)recentMetrics.Count(m => !m.IsSuccess) / recentMetrics.Count
                : 0;

            healthStatus.ErrorRate = errorRate;

            return healthStatus;
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error getting storage health status");
            return new StorageHealthStatus {
                CheckedAt = DateTimeOffset.UtcNow,
                OverallStatus = HealthStatus.Unhealthy
            };
        }
    }

    public async Task<StorageUsageStatistics> GetUsageStatisticsAsync(CancellationToken cancellationToken = default) {
        try {
            // This would typically aggregate usage across all containers
            // For now, we'll return a sample implementation

            var stats = new StorageUsageStatistics {
                GeneratedAt = DateTimeOffset.UtcNow,
                TotalStorageUsed = 0,
                TotalFileCount = 0,
                ContainerStatistics = []
            };

            // In a real implementation, this would enumerate containers and get usage
            // For now, we'll simulate some data

            return stats;
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error getting storage usage statistics");
            return new StorageUsageStatistics { GeneratedAt = DateTimeOffset.UtcNow };
        }
    }

    public IStorageOperationMonitor StartOperationMonitoring(string operationName, string containerName, string? fileName = null) => new StorageOperationMonitor(this, operationName, containerName, fileName);

    public async Task<List<StorageThresholdAlert>> CheckThresholdsAsync(CancellationToken cancellationToken = default) {
        var alerts = new List<StorageThresholdAlert>();

        try {
            var thresholds = _configuration.Monitoring.Thresholds;
            var healthStatus = await GetHealthStatusAsync(cancellationToken);

            // Check success rate threshold
            if (healthStatus.SuccessRate < 0.95) {
                alerts.Add(new StorageThresholdAlert {
                    AlertType = StorageAlertType.LowSuccessRate,
                    Message = $"Storage success rate is {healthStatus.SuccessRate:P2}, below 95% threshold",
                    Severity = AlertSeverity.Warning,
                    Timestamp = DateTimeOffset.UtcNow,
                    Value = healthStatus.SuccessRate,
                    Threshold = 0.95
                });
            }

            // Check response time threshold
            if (healthStatus.AverageResponseTime > thresholds.MaxResponseTime) {
                alerts.Add(new StorageThresholdAlert {
                    AlertType = StorageAlertType.HighResponseTime,
                    Message = $"Storage response time is {healthStatus.AverageResponseTime.TotalSeconds:F2}s, above {thresholds.MaxResponseTime.TotalSeconds:F2}s threshold",
                    Severity = AlertSeverity.Warning,
                    Timestamp = DateTimeOffset.UtcNow,
                    Value = healthStatus.AverageResponseTime.TotalSeconds,
                    Threshold = thresholds.MaxResponseTime.TotalSeconds
                });
            }

            // Check error rate threshold
            if (healthStatus.ErrorRate > 0.05) // 5% error rate
            {
                alerts.Add(new StorageThresholdAlert {
                    AlertType = StorageAlertType.HighErrorRate,
                    Message = $"Storage error rate is {healthStatus.ErrorRate:P2}, above 5% threshold",
                    Severity = AlertSeverity.Critical,
                    Timestamp = DateTimeOffset.UtcNow,
                    Value = healthStatus.ErrorRate,
                    Threshold = 0.05
                });
            }

            return alerts;
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error checking storage thresholds");
            return alerts;
        }
    }
}

/// <summary>
/// Implementation of storage operation monitor
/// </summary>
public class StorageOperationMonitor(
    IStorageMonitoringService monitoringService,
    string operationName,
    string containerName,
    string? fileName = null) : IStorageOperationMonitor {
    private readonly IStorageMonitoringService _monitoringService = monitoringService;
    private readonly string _operationName = operationName;
    private readonly string _containerName = containerName;
    private readonly string? _fileName = fileName;
    private readonly Stopwatch _stopwatch = Stopwatch.StartNew();
    private readonly DateTimeOffset _startTime = DateTimeOffset.UtcNow;
    private bool _disposed = false;

    public void RecordSuccess(long? bytesTransferred = null) => RecordCompletion(true, bytesTransferred);

    public void RecordFailure(Exception exception) => RecordCompletion(false, null, exception);

    public void RecordCompletion(bool success, long? bytesTransferred = null, Exception? exception = null) {
        if (_disposed)
            return;

        _stopwatch.Stop();

        var metric = new StorageOperationMetric {
            OperationName = _operationName,
            OperationType = ParseOperationType(_operationName),
            ContainerName = _containerName,
            FileName = _fileName,
            IsSuccess = success,
            ResponseTime = _stopwatch.Elapsed,
            BytesTransferred = bytesTransferred ?? 0,
            Timestamp = _startTime,
            ErrorType = exception?.GetType().Name,
            ErrorMessage = exception?.Message
        };

        Task.Run(async () => {
            try {
                await _monitoringService.RecordOperationAsync(metric);
            }
            catch {
                // Ignore errors in metric recording to avoid affecting main operations
            }
        });
    }

    private static StorageOperationType ParseOperationType(string operationName) => operationName.ToLowerInvariant() switch {
        "upload" => StorageOperationType.Upload,
        "download" => StorageOperationType.Download,
        "delete" => StorageOperationType.Delete,
        "list" => StorageOperationType.List,
        "getmetadata" => StorageOperationType.GetMetadata,
        "copy" => StorageOperationType.Copy,
        "exists" => StorageOperationType.Exists,
        _ => StorageOperationType.Unknown
    };

    public void Dispose() {
        if (!_disposed) {
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}

/// <summary>
/// Storage operation metric
/// </summary>
public class StorageOperationMetric {
    public string OperationName { get; set; } = string.Empty;
    public StorageOperationType OperationType { get; set; }
    public string ContainerName { get; set; } = string.Empty;
    public string? FileName { get; set; }
    public bool IsSuccess { get; set; }
    public TimeSpan ResponseTime { get; set; }
    public long BytesTransferred { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public string? ErrorType { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Storage operation types
/// </summary>
public enum StorageOperationType {
    Upload,
    Download,
    Delete,
    List,
    GetMetadata,
    Copy,
    Exists,
    Unknown
}

/// <summary>
/// Storage metrics aggregate
/// </summary>
public class StorageMetrics {
    public TimeSpan Period { get; set; }
    public DateTimeOffset GeneratedAt { get; set; }
    public int TotalOperations { get; set; }
    public int SuccessfulOperations { get; set; }
    public int FailedOperations { get; set; }
    public long TotalBytesTransferred { get; set; }
    public TimeSpan AverageResponseTime { get; set; }
    public Dictionary<StorageOperationType, int> OperationsByType { get; set; } = [];
    public Dictionary<string, int> ErrorsByType { get; set; } = [];
    public double SuccessRate => TotalOperations > 0 ? (double)SuccessfulOperations / TotalOperations : 1.0;
    public double ErrorRate => TotalOperations > 0 ? (double)FailedOperations / TotalOperations : 0.0;
}

/// <summary>
/// Storage health status
/// </summary>
public class StorageHealthStatus {
    public DateTimeOffset CheckedAt { get; set; }
    public HealthStatus OverallStatus { get; set; }
    public double SuccessRate { get; set; }
    public double ErrorRate { get; set; }
    public TimeSpan AverageResponseTime { get; set; }
}

/// <summary>
/// Storage usage statistics
/// </summary>
public class StorageUsageStatistics {
    public DateTimeOffset GeneratedAt { get; set; }
    public long TotalStorageUsed { get; set; }
    public long TotalFileCount { get; set; }
    public List<StorageContainerStatistics> ContainerStatistics { get; set; } = [];
}

/// <summary>
/// Storage container statistics
/// </summary>
public class StorageContainerStatistics {
    public string ContainerName { get; set; } = string.Empty;
    public long StorageUsed { get; set; }
    public long FileCount { get; set; }
    public DateTimeOffset LastModified { get; set; }
}

/// <summary>
/// Storage threshold alert
/// </summary>
public class StorageThresholdAlert {
    public StorageAlertType AlertType { get; set; }
    public string Message { get; set; } = string.Empty;
    public AlertSeverity Severity { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public double Value { get; set; }
    public double Threshold { get; set; }
}

/// <summary>
/// Storage alert types
/// </summary>
public enum StorageAlertType {
    HighUsage,
    LowSuccessRate,
    HighResponseTime,
    HighErrorRate,
    SecurityThreat,
    QuotaExceeded
}

/// <summary>
/// Alert severity levels
/// </summary>
public enum AlertSeverity {
    Info,
    Warning,
    Critical
}