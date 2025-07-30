namespace MCPHub.CommandLineApp.Services;

/// <summary>
/// Interface for offline mode operations and fallback support
/// </summary>
public interface IOfflineModeService {
    /// <summary>
    /// Checks if the application is currently operating in offline mode
    /// </summary>
    /// <returns>True if offline mode is active</returns>
    bool IsOfflineMode { get; }

    /// <summary>
    /// Tests connectivity to the API and updates offline mode status
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if API is accessible</returns>
    Task<bool> TestConnectivityAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Enables offline mode manually
    /// </summary>
    void EnableOfflineMode();

    /// <summary>
    /// Disables offline mode manually
    /// </summary>
    void DisableOfflineMode();

    /// <summary>
    /// Gets offline mode status and information
    /// </summary>
    /// <returns>Offline mode status information</returns>
    OfflineModeStatus GetOfflineModeStatus();

    /// <summary>
    /// Attempts to execute an operation with automatic fallback to cached data
    /// </summary>
    /// <typeparam name="T">Type of the result</typeparam>
    /// <param name="onlineOperation">Operation to execute when online</param>
    /// <param name="offlineOperation">Fallback operation using cached data</param>
    /// <param name="cacheKey">Cache key for storing/retrieving data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result from online or offline operation</returns>
    Task<T?> ExecuteWithFallbackAsync<T>(
        Func<CancellationToken, Task<T>> onlineOperation,
        Func<CancellationToken, Task<T?>> offlineOperation,
        string cacheKey,
        CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Synchronizes local cache with remote data when connectivity is restored
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Synchronization result</returns>
    Task<SynchronizationResult> SynchronizeAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets offline capabilities for different operations
    /// </summary>
    /// <returns>Dictionary of operation capabilities</returns>
    Dictionary<string, OfflineCapability> GetOfflineCapabilities();

    /// <summary>
    /// Validates if an operation can be performed offline
    /// </summary>
    /// <param name="operationType">Type of operation to validate</param>
    /// <returns>Validation result with capability information</returns>
    OfflineOperationValidation ValidateOfflineOperation(string operationType);

    /// <summary>
    /// Prepares the application for offline use by caching essential data
    /// </summary>
    /// <param name="essentialPackages">List of essential packages to cache</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Preparation result</returns>
    Task<OfflinePreparationResult> PrepareForOfflineAsync(IEnumerable<string>? essentialPackages = null, CancellationToken cancellationToken = default);
}

/// <summary>
/// Information about offline mode status
/// </summary>
public class OfflineModeStatus {
    /// <summary>
    /// Whether offline mode is currently active
    /// </summary>
    public bool IsOffline { get; set; }

    /// <summary>
    /// Whether offline mode was manually enabled
    /// </summary>
    public bool IsManuallyEnabled { get; set; }

    /// <summary>
    /// When offline mode was last activated
    /// </summary>
    public DateTimeOffset? OfflineSince { get; set; }

    /// <summary>
    /// Last successful connectivity test
    /// </summary>
    public DateTimeOffset? LastConnectivityTest { get; set; }

    /// <summary>
    /// Result of the last connectivity test
    /// </summary>
    public bool? LastConnectivityResult { get; set; }

    /// <summary>
    /// Reason for offline mode activation
    /// </summary>
    public string? OfflineReason { get; set; }

    /// <summary>
    /// Available cached data summary
    /// </summary>
    public CachedDataSummary CachedData { get; set; } = new();

    /// <summary>
    /// Estimated duration offline (if known)
    /// </summary>
    public TimeSpan? EstimatedOfflineDuration => OfflineSince.HasValue
        ? DateTimeOffset.UtcNow - OfflineSince.Value
        : null;
}

/// <summary>
/// Summary of available cached data for offline use
/// </summary>
public class CachedDataSummary {
    /// <summary>
    /// Number of packages with cached information
    /// </summary>
    public int CachedPackages { get; set; }

    /// <summary>
    /// Number of cached search results
    /// </summary>
    public int CachedSearchResults { get; set; }

    /// <summary>
    /// Total size of cached data
    /// </summary>
    public long TotalCacheSize { get; set; }

    /// <summary>
    /// Human-readable cache size
    /// </summary>
    public string TotalCacheSizeFormatted => FormatBytes(TotalCacheSize);

    /// <summary>
    /// Oldest cached entry date
    /// </summary>
    public DateTimeOffset? OldestEntry { get; set; }

    /// <summary>
    /// Newest cached entry date
    /// </summary>
    public DateTimeOffset? NewestEntry { get; set; }

    private static string FormatBytes(long bytes) {
        string[] sizes = ["B", "KB", "MB", "GB", "TB"];
        var order = 0;
        double size = bytes;

        while (size >= 1024 && order < sizes.Length - 1) {
            order++;
            size /= 1024;
        }

        return $"{size:0.##} {sizes[order]}";
    }
}

/// <summary>
/// Offline capability information for an operation
/// </summary>
public class OfflineCapability {
    /// <summary>
    /// Operation name
    /// </summary>
    public string OperationName { get; set; } = string.Empty;

    /// <summary>
    /// Whether the operation can be performed offline
    /// </summary>
    public bool IsAvailableOffline { get; set; }

    /// <summary>
    /// Confidence level of offline data (0-100)
    /// </summary>
    public int DataFreshnessConfidence { get; set; }

    /// <summary>
    /// Limitations when operating offline
    /// </summary>
    public List<string> Limitations { get; set; } = [];

    /// <summary>
    /// Required cached data for offline operation
    /// </summary>
    public List<string> RequiredCachedData { get; set; } = [];

    /// <summary>
    /// Fallback behavior description
    /// </summary>
    public string? FallbackBehavior { get; set; }
}

/// <summary>
/// Result of offline operation validation
/// </summary>
public class OfflineOperationValidation {
    /// <summary>
    /// Whether the operation can proceed offline
    /// </summary>
    public bool CanProceedOffline { get; set; }

    /// <summary>
    /// Operation capability information
    /// </summary>
    public OfflineCapability? Capability { get; set; }

    /// <summary>
    /// Validation warnings
    /// </summary>
    public List<string> Warnings { get; set; } = [];

    /// <summary>
    /// Validation errors (if operation cannot proceed)
    /// </summary>
    public List<string> Errors { get; set; } = [];

    /// <summary>
    /// Suggested actions for improving offline capability
    /// </summary>
    public List<string> SuggestedActions { get; set; } = [];
}

/// <summary>
/// Result of synchronization operations
/// </summary>
public class SynchronizationResult {
    /// <summary>
    /// Whether synchronization was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Number of items synchronized
    /// </summary>
    public int ItemsSynchronized { get; set; }

    /// <summary>
    /// Number of items updated
    /// </summary>
    public int ItemsUpdated { get; set; }

    /// <summary>
    /// Number of new items added to cache
    /// </summary>
    public int ItemsAdded { get; set; }

    /// <summary>
    /// Number of items removed from cache
    /// </summary>
    public int ItemsRemoved { get; set; }

    /// <summary>
    /// Time taken for synchronization
    /// </summary>
    public TimeSpan SynchronizationTime { get; set; }

    /// <summary>
    /// Errors encountered during synchronization
    /// </summary>
    public List<string> Errors { get; set; } = [];

    /// <summary>
    /// Warnings during synchronization
    /// </summary>
    public List<string> Warnings { get; set; } = [];

    /// <summary>
    /// Data transfer statistics
    /// </summary>
    public DataTransferStatistics TransferStats { get; set; } = new();
}

/// <summary>
/// Statistics about data transfer during synchronization
/// </summary>
public class DataTransferStatistics {
    /// <summary>
    /// Bytes downloaded during sync
    /// </summary>
    public long BytesDownloaded { get; set; }

    /// <summary>
    /// Bytes uploaded during sync
    /// </summary>
    public long BytesUploaded { get; set; }

    /// <summary>
    /// Total bytes transferred
    /// </summary>
    public long TotalBytesTransferred => BytesDownloaded + BytesUploaded;

    /// <summary>
    /// Number of network requests made
    /// </summary>
    public int NetworkRequests { get; set; }

    /// <summary>
    /// Average response time
    /// </summary>
    public TimeSpan AverageResponseTime { get; set; }
}

/// <summary>
/// Result of offline preparation operations
/// </summary>
public class OfflinePreparationResult {
    /// <summary>
    /// Whether preparation was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Number of packages cached for offline use
    /// </summary>
    public int PackagesCached { get; set; }

    /// <summary>
    /// Number of search results cached
    /// </summary>
    public int SearchResultsCached { get; set; }

    /// <summary>
    /// Total data downloaded for offline use
    /// </summary>
    public long DataDownloaded { get; set; }

    /// <summary>
    /// Human-readable size of data downloaded
    /// </summary>
    public string DataDownloadedFormatted => FormatBytes(DataDownloaded);

    /// <summary>
    /// Time taken for preparation
    /// </summary>
    public TimeSpan PreparationTime { get; set; }

    /// <summary>
    /// Errors encountered during preparation
    /// </summary>
    public List<string> Errors { get; set; } = [];

    /// <summary>
    /// Warnings during preparation
    /// </summary>
    public List<string> Warnings { get; set; } = [];

    /// <summary>
    /// Packages that failed to cache
    /// </summary>
    public List<string> FailedPackages { get; set; } = [];

    private static string FormatBytes(long bytes) {
        string[] sizes = ["B", "KB", "MB", "GB", "TB"];
        var order = 0;
        double size = bytes;

        while (size >= 1024 && order < sizes.Length - 1) {
            order++;
            size /= 1024;
        }

        return $"{size:0.##} {sizes[order]}";
    }
}