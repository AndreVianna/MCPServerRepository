using Common.Models;
using Common.Services;

namespace Common.Extensions;

/// <summary>
/// Extension methods for configuring storage services
/// </summary>
public static class StorageServiceExtensions {
    /// <summary>
    /// Adds storage services to the service collection
    /// </summary>
    public static IServiceCollection AddStorageServices(this IServiceCollection services, IConfiguration configuration) {
        // Configure storage settings
        services.Configure<StorageConfiguration>(configuration.GetSection(StorageConfiguration.ConfigurationKey));

        // Add storage services based on configuration
        services.AddTransient<IStorageService>(provider => {
            var storageConfig = provider.GetRequiredService<IOptions<StorageConfiguration>>().Value;

            return storageConfig.Provider switch {
                StorageProviderType.AzureBlob => provider.GetRequiredService<AzureBlobStorageService>(),
                StorageProviderType.AwsS3 => provider.GetRequiredService<AwsS3StorageService>(),
                StorageProviderType.S3Compatible => provider.GetRequiredService<S3CompatibleStorageService>(),
                _ => throw new InvalidOperationException($"Unsupported storage provider: {storageConfig.Provider}")
            };
        });

        // Register concrete implementations
        services.AddTransient<AzureBlobStorageService>();
        services.AddTransient<AwsS3StorageService>();
        services.AddTransient<S3CompatibleStorageService>();

        // Add supporting services
        services.AddTransient<IStorageSecurityService, StorageSecurityService>();
        services.AddTransient<IStorageLifecycleService, StorageLifecycleService>();
        services.AddTransient<IStorageMonitoringService, StorageMonitoringService>();
        services.AddTransient<IStorageBackupService, StorageBackupService>();

        // Add background services
        services.AddHostedService<StorageLifecycleBackgroundService>();
        services.AddHostedService<StorageMonitoringBackgroundService>();
        services.AddHostedService<StorageBackupBackgroundService>();

        // Add health checks
        services.AddStorageHealthChecks(configuration);

        return services;
    }

    /// <summary>
    /// Adds storage health checks
    /// </summary>
    public static IServiceCollection AddStorageHealthChecks(this IServiceCollection services, IConfiguration configuration) {
        var storageConfig = configuration.GetSection(StorageConfiguration.ConfigurationKey).Get<StorageConfiguration>();

        if (storageConfig?.Monitoring.EnableHealthChecks == true) {
            services.AddHealthChecks()
                .AddCheck<StorageHealthCheck>("storage", HealthStatus.Unhealthy, new[] { "storage", "ready" });
        }

        return services;
    }

    /// <summary>
    /// Adds storage-specific decorators and middleware
    /// </summary>
    public static IServiceCollection AddStorageDecorators(this IServiceCollection services) {
        // Add security decorator
        services.Decorate<IStorageService, StorageSecurityDecorator>();

        // Add monitoring decorator
        services.Decorate<IStorageService, StorageMonitoringDecorator>();

        return services;
    }
}

/// <summary>
/// Health check for storage services
/// </summary>
public class StorageHealthCheck(
    IStorageMonitoringService monitoringService,
    ILogger<StorageHealthCheck> logger) : IHealthCheck {
    private readonly IStorageMonitoringService _monitoringService = monitoringService;
    private readonly ILogger<StorageHealthCheck> _logger = logger;

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default) {
        try {
            var healthStatus = await _monitoringService.GetHealthStatusAsync(cancellationToken);

            var data = new Dictionary<string, object> {
                ["success_rate"] = healthStatus.SuccessRate,
                ["error_rate"] = healthStatus.ErrorRate,
                ["average_response_time"] = healthStatus.AverageResponseTime.TotalMilliseconds,
                ["checked_at"] = healthStatus.CheckedAt
            };

            return healthStatus.OverallStatus switch {
                HealthStatus.Healthy => HealthCheckResult.Healthy("Storage service is healthy", data),
                HealthStatus.Degraded => HealthCheckResult.Degraded("Storage service is degraded", null, data),
                HealthStatus.Unhealthy => HealthCheckResult.Unhealthy("Storage service is unhealthy", null, data),
                _ => HealthCheckResult.Unhealthy("Storage service status unknown", null, data)
            };
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error checking storage health");
            return HealthCheckResult.Unhealthy("Storage health check failed", ex, new Dictionary<string, object> {
                ["error"] = ex.Message
            });
        }
    }
}

/// <summary>
/// Security decorator for storage service
/// </summary>
public class StorageSecurityDecorator(
    IStorageService inner,
    IStorageSecurityService securityService,
    ILogger<StorageSecurityDecorator> logger) : IStorageService {
    private readonly IStorageService _inner = inner;
    private readonly IStorageSecurityService _securityService = securityService;
    private readonly ILogger<StorageSecurityDecorator> _logger = logger;

    public async Task<string> UploadAsync(
        string containerName,
        string fileName,
        Stream content,
        string contentType,
        Dictionary<string, string>? metadata = null,
        CancellationToken cancellationToken = default) {
        // Validate upload security
        var validationResult = await _securityService.ValidateFileUploadAsync(
            fileName, content, contentType, cancellationToken: cancellationToken);

        if (!validationResult.IsValid) {
            throw new UnauthorizedAccessException($"Upload security validation failed: {string.Join(", ", validationResult.Errors)}");
        }

        // Encrypt content if enabled
        var encryptedContent = await _securityService.EncryptContentAsync(content, cancellationToken);

        return await _inner.UploadAsync(containerName, fileName, encryptedContent, contentType, metadata, cancellationToken);
    }

    public async Task<Stream> DownloadAsync(string containerName, string fileName, CancellationToken cancellationToken = default) {
        // Validate download security
        var validationResult = await _securityService.ValidateFileDownloadAsync(
            containerName, fileName, cancellationToken: cancellationToken);

        if (!validationResult.IsValid) {
            throw new UnauthorizedAccessException($"Download security validation failed: {string.Join(", ", validationResult.Errors)}");
        }

        var encryptedContent = await _inner.DownloadAsync(containerName, fileName, cancellationToken);

        // Decrypt content if enabled
        return await _securityService.DecryptContentAsync(encryptedContent, cancellationToken);
    }

    // Delegate other methods to the inner service
    public Task DownloadToFileAsync(string containerName, string fileName, string destinationPath, CancellationToken cancellationToken = default)
        => _inner.DownloadToFileAsync(containerName, fileName, destinationPath, cancellationToken);

    public Task DeleteAsync(string containerName, string fileName, CancellationToken cancellationToken = default)
        => _inner.DeleteAsync(containerName, fileName, cancellationToken);

    public Task DeleteBatchAsync(string containerName, IEnumerable<string> fileNames, CancellationToken cancellationToken = default)
        => _inner.DeleteBatchAsync(containerName, fileNames, cancellationToken);

    public Task<bool> ExistsAsync(string containerName, string fileName, CancellationToken cancellationToken = default)
        => _inner.ExistsAsync(containerName, fileName, cancellationToken);

    public Task<StorageFileMetadata> GetMetadataAsync(string containerName, string fileName, CancellationToken cancellationToken = default)
        => _inner.GetMetadataAsync(containerName, fileName, cancellationToken);

    public Task<IEnumerable<StorageFileInfo>> ListFilesAsync(string containerName, string? prefix = null, CancellationToken cancellationToken = default)
        => _inner.ListFilesAsync(containerName, prefix, cancellationToken);

    public Task<string> GetPresignedUrlAsync(string containerName, string fileName, TimeSpan expiration, StoragePermissions permissions = StoragePermissions.Read, CancellationToken cancellationToken = default)
        => _inner.GetPresignedUrlAsync(containerName, fileName, expiration, permissions, cancellationToken);

    public Task CreateContainerAsync(string containerName, bool isPublic = false, CancellationToken cancellationToken = default)
        => _inner.CreateContainerAsync(containerName, isPublic, cancellationToken);

    public Task DeleteContainerAsync(string containerName, CancellationToken cancellationToken = default)
        => _inner.DeleteContainerAsync(containerName, cancellationToken);

    public Task CopyAsync(string sourceContainer, string sourceFileName, string destinationContainer, string destinationFileName, CancellationToken cancellationToken = default)
        => _inner.CopyAsync(sourceContainer, sourceFileName, destinationContainer, destinationFileName, cancellationToken);

    public Task<StorageUsageInfo> GetUsageAsync(string containerName, CancellationToken cancellationToken = default)
        => _inner.GetUsageAsync(containerName, cancellationToken);
}

/// <summary>
/// Monitoring decorator for storage service
/// </summary>
public class StorageMonitoringDecorator(
    IStorageService inner,
    IStorageMonitoringService monitoringService,
    ILogger<StorageMonitoringDecorator> logger) : IStorageService {
    private readonly IStorageService _inner = inner;
    private readonly IStorageMonitoringService _monitoringService = monitoringService;
    private readonly ILogger<StorageMonitoringDecorator> _logger = logger;

    public async Task<string> UploadAsync(
        string containerName,
        string fileName,
        Stream content,
        string contentType,
        Dictionary<string, string>? metadata = null,
        CancellationToken cancellationToken = default) {
        using var monitor = _monitoringService.StartOperationMonitoring("upload", containerName, fileName);

        try {
            var result = await _inner.UploadAsync(containerName, fileName, content, contentType, metadata, cancellationToken);
            monitor.RecordSuccess(content.Length);
            return result;
        }
        catch (Exception ex) {
            monitor.RecordFailure(ex);
            throw;
        }
    }

    public async Task<Stream> DownloadAsync(string containerName, string fileName, CancellationToken cancellationToken = default) {
        using var monitor = _monitoringService.StartOperationMonitoring("download", containerName, fileName);

        try {
            var result = await _inner.DownloadAsync(containerName, fileName, cancellationToken);
            monitor.RecordSuccess(result.Length);
            return result;
        }
        catch (Exception ex) {
            monitor.RecordFailure(ex);
            throw;
        }
    }

    public async Task DownloadToFileAsync(string containerName, string fileName, string destinationPath, CancellationToken cancellationToken = default) {
        using var monitor = _monitoringService.StartOperationMonitoring("download", containerName, fileName);

        try {
            await _inner.DownloadToFileAsync(containerName, fileName, destinationPath, cancellationToken);
            monitor.RecordSuccess();
        }
        catch (Exception ex) {
            monitor.RecordFailure(ex);
            throw;
        }
    }

    public async Task DeleteAsync(string containerName, string fileName, CancellationToken cancellationToken = default) {
        using var monitor = _monitoringService.StartOperationMonitoring("delete", containerName, fileName);

        try {
            await _inner.DeleteAsync(containerName, fileName, cancellationToken);
            monitor.RecordSuccess();
        }
        catch (Exception ex) {
            monitor.RecordFailure(ex);
            throw;
        }
    }

    public async Task DeleteBatchAsync(string containerName, IEnumerable<string> fileNames, CancellationToken cancellationToken = default) {
        using var monitor = _monitoringService.StartOperationMonitoring("delete", containerName);

        try {
            await _inner.DeleteBatchAsync(containerName, fileNames, cancellationToken);
            monitor.RecordSuccess();
        }
        catch (Exception ex) {
            monitor.RecordFailure(ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string containerName, string fileName, CancellationToken cancellationToken = default) {
        using var monitor = _monitoringService.StartOperationMonitoring("exists", containerName, fileName);

        try {
            var result = await _inner.ExistsAsync(containerName, fileName, cancellationToken);
            monitor.RecordSuccess();
            return result;
        }
        catch (Exception ex) {
            monitor.RecordFailure(ex);
            throw;
        }
    }

    public async Task<StorageFileMetadata> GetMetadataAsync(string containerName, string fileName, CancellationToken cancellationToken = default) {
        using var monitor = _monitoringService.StartOperationMonitoring("getmetadata", containerName, fileName);

        try {
            var result = await _inner.GetMetadataAsync(containerName, fileName, cancellationToken);
            monitor.RecordSuccess();
            return result;
        }
        catch (Exception ex) {
            monitor.RecordFailure(ex);
            throw;
        }
    }

    public async Task<IEnumerable<StorageFileInfo>> ListFilesAsync(string containerName, string? prefix = null, CancellationToken cancellationToken = default) {
        using var monitor = _monitoringService.StartOperationMonitoring("list", containerName);

        try {
            var result = await _inner.ListFilesAsync(containerName, prefix, cancellationToken);
            monitor.RecordSuccess();
            return result;
        }
        catch (Exception ex) {
            monitor.RecordFailure(ex);
            throw;
        }
    }

    public async Task<string> GetPresignedUrlAsync(string containerName, string fileName, TimeSpan expiration, StoragePermissions permissions = StoragePermissions.Read, CancellationToken cancellationToken = default) {
        using var monitor = _monitoringService.StartOperationMonitoring("getpresignedurl", containerName, fileName);

        try {
            var result = await _inner.GetPresignedUrlAsync(containerName, fileName, expiration, permissions, cancellationToken);
            monitor.RecordSuccess();
            return result;
        }
        catch (Exception ex) {
            monitor.RecordFailure(ex);
            throw;
        }
    }

    public async Task CreateContainerAsync(string containerName, bool isPublic = false, CancellationToken cancellationToken = default) {
        using var monitor = _monitoringService.StartOperationMonitoring("createcontainer", containerName);

        try {
            await _inner.CreateContainerAsync(containerName, isPublic, cancellationToken);
            monitor.RecordSuccess();
        }
        catch (Exception ex) {
            monitor.RecordFailure(ex);
            throw;
        }
    }

    public async Task DeleteContainerAsync(string containerName, CancellationToken cancellationToken = default) {
        using var monitor = _monitoringService.StartOperationMonitoring("deletecontainer", containerName);

        try {
            await _inner.DeleteContainerAsync(containerName, cancellationToken);
            monitor.RecordSuccess();
        }
        catch (Exception ex) {
            monitor.RecordFailure(ex);
            throw;
        }
    }

    public async Task CopyAsync(string sourceContainer, string sourceFileName, string destinationContainer, string destinationFileName, CancellationToken cancellationToken = default) {
        using var monitor = _monitoringService.StartOperationMonitoring("copy", sourceContainer, sourceFileName);

        try {
            await _inner.CopyAsync(sourceContainer, sourceFileName, destinationContainer, destinationFileName, cancellationToken);
            monitor.RecordSuccess();
        }
        catch (Exception ex) {
            monitor.RecordFailure(ex);
            throw;
        }
    }

    public async Task<StorageUsageInfo> GetUsageAsync(string containerName, CancellationToken cancellationToken = default) {
        using var monitor = _monitoringService.StartOperationMonitoring("getusage", containerName);

        try {
            var result = await _inner.GetUsageAsync(containerName, cancellationToken);
            monitor.RecordSuccess();
            return result;
        }
        catch (Exception ex) {
            monitor.RecordFailure(ex);
            throw;
        }
    }
}