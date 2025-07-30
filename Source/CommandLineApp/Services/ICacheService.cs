using System.Text.Json;

namespace MCPHub.CommandLineApp.Services;

/// <summary>
/// Interface for caching operations in the CLI application
/// </summary>
public interface ICacheService : IDisposable {
    /// <summary>
    /// Stores an item in the cache with optional expiration
    /// </summary>
    /// <typeparam name="T">Type of the item to cache</typeparam>
    /// <param name="key">Cache key</param>
    /// <param name="item">Item to cache</param>
    /// <param name="expiration">Optional expiration time</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task indicating completion</returns>
    Task SetAsync<T>(string key, T item, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Retrieves an item from the cache
    /// </summary>
    /// <typeparam name="T">Type of the item to retrieve</typeparam>
    /// <param name="key">Cache key</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Cached item or null if not found or expired</returns>
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Checks if an item exists in the cache and is not expired
    /// </summary>
    /// <param name="key">Cache key</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if item exists and is valid</returns>
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes an item from the cache
    /// </summary>
    /// <param name="key">Cache key</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if item was removed</returns>
    Task<bool> RemoveAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes all items matching a pattern from the cache
    /// </summary>
    /// <param name="pattern">Pattern to match (supports wildcards)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Number of items removed</returns>
    Task<int> RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default);

    /// <summary>
    /// Clears all items from the cache
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task indicating completion</returns>
    Task ClearAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets cache statistics and information
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Cache statistics</returns>
    Task<CacheStatistics> GetStatisticsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets detailed information about a specific cache entry
    /// </summary>
    /// <param name="key">Cache key</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Cache entry information or null if not found</returns>
    Task<CacheEntryInfo?> GetEntryInfoAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists all cache keys matching an optional pattern
    /// </summary>
    /// <param name="pattern">Optional pattern to match (supports wildcards)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of matching cache keys</returns>
    Task<IEnumerable<string>> GetKeysAsync(string? pattern = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs cache maintenance operations (cleanup expired items, etc.)
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Maintenance result information</returns>
    Task<CacheMaintenanceResult> PerformMaintenanceAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Cache statistics information
/// </summary>
public class CacheStatistics {
    /// <summary>
    /// Total number of cache entries
    /// </summary>
    public int TotalEntries { get; set; }

    /// <summary>
    /// Number of expired entries
    /// </summary>
    public int ExpiredEntries { get; set; }

    /// <summary>
    /// Total cache size in bytes
    /// </summary>
    public long TotalSizeBytes { get; set; }

    /// <summary>
    /// Total cache size in a human-readable format
    /// </summary>
    public string TotalSizeFormatted => FormatBytes(TotalSizeBytes);

    /// <summary>
    /// Number of cache hits since start
    /// </summary>
    public long HitCount { get; set; }

    /// <summary>
    /// Number of cache misses since start
    /// </summary>
    public long MissCount { get; set; }

    /// <summary>
    /// Cache hit rate as a percentage
    /// </summary>
    public double HitRate => HitCount + MissCount > 0 ? (double)HitCount / (HitCount + MissCount) * 100 : 0;

    /// <summary>
    /// When cache statistics were last reset
    /// </summary>
    public DateTimeOffset StatisticsResetAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Cache entries grouped by type
    /// </summary>
    public Dictionary<string, int> EntriesByType { get; set; } = new();

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
/// Information about a specific cache entry
/// </summary>
public class CacheEntryInfo {
    /// <summary>
    /// Cache key
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Type of the cached object
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Size of the cached entry in bytes
    /// </summary>
    public long SizeBytes { get; set; }

    /// <summary>
    /// Size in a human-readable format
    /// </summary>
    public string SizeFormatted => FormatBytes(SizeBytes);

    /// <summary>
    /// When the entry was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// When the entry was last accessed
    /// </summary>
    public DateTimeOffset LastAccessedAt { get; set; }

    /// <summary>
    /// When the entry expires (if applicable)
    /// </summary>
    public DateTimeOffset? ExpiresAt { get; set; }

    /// <summary>
    /// Whether the entry is expired
    /// </summary>
    public bool IsExpired => ExpiresAt.HasValue && DateTimeOffset.UtcNow > ExpiresAt.Value;

    /// <summary>
    /// Time until expiration (if applicable)
    /// </summary>
    public TimeSpan? TimeUntilExpiration => ExpiresAt.HasValue && !IsExpired
        ? ExpiresAt.Value - DateTimeOffset.UtcNow
        : null;

    /// <summary>
    /// Number of times this entry has been accessed
    /// </summary>
    public int AccessCount { get; set; }

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
/// Result of cache maintenance operations
/// </summary>
public class CacheMaintenanceResult {
    /// <summary>
    /// Number of expired entries removed
    /// </summary>
    public int ExpiredEntriesRemoved { get; set; }

    /// <summary>
    /// Bytes of storage reclaimed
    /// </summary>
    public long BytesReclaimed { get; set; }

    /// <summary>
    /// Human-readable size of storage reclaimed
    /// </summary>
    public string BytesReclaimedFormatted => FormatBytes(BytesReclaimed);

    /// <summary>
    /// Time taken for maintenance operation
    /// </summary>
    public TimeSpan MaintenanceTime { get; set; }

    /// <summary>
    /// Any errors encountered during maintenance
    /// </summary>
    public List<string> Errors { get; set; } = new();

    /// <summary>
    /// Whether maintenance completed successfully
    /// </summary>
    public bool Success => !Errors.Any();

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