namespace MCPHub.Common.Services;

/// <summary>
/// Enhanced caching service with enterprise features
/// Supports: In-Memory → Redis → Redis Cluster
/// </summary>
public interface IEnhancedCacheService : ICacheService
{
    /// <summary>
    /// Gets cache provider type
    /// </summary>
    CacheProviderType ProviderType { get; }
    
    /// <summary>
    /// Distributed locking for cache coordination
    /// </summary>
    Task<IDisposable> AcquireLockAsync(string key, TimeSpan expiration, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Cache invalidation with pattern matching
    /// </summary>
    Task InvalidateTagAsync(string tag, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Bulk operations for performance
    /// </summary>
    Task SetBulkAsync<T>(IDictionary<string, T> items, TimeSpan? expiration = null, CancellationToken cancellationToken = default);
    Task<IDictionary<string, T?>> GetBulkAsync<T>(IEnumerable<string> keys, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Cache statistics for monitoring
    /// </summary>
    Task<CacheStatistics> GetStatisticsAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Cache warming for performance
    /// </summary>
    Task WarmupAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Memory pressure handling
    /// </summary>
    Task CompactAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Cache provider types for technology progression
/// </summary>
public enum CacheProviderType
{
    InMemory,     // Development tier
    Redis,        // Production tier
    RedisCluster  // Enterprise tier
}

/// <summary>
/// Cache performance statistics
/// </summary>
public record CacheStatistics(
    long HitCount,
    long MissCount,
    double HitRatio,
    long ItemCount,
    long MemoryUsage,
    TimeSpan AverageAccessTime,
    DateTime Timestamp);