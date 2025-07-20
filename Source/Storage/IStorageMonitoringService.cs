namespace MCPHub.Storage;

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
