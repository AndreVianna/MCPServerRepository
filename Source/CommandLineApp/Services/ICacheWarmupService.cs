namespace MCPHub.CommandLineApp.Services;

/// <summary>
/// Interface for cache warming and background maintenance operations
/// </summary>
public interface ICacheWarmupService : IDisposable {
    /// <summary>
    /// Starts background cache warming and maintenance operations
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task indicating completion</returns>
    Task StartAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Stops background operations
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task indicating completion</returns>
    Task StopAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs immediate cache warming with popular packages
    /// </summary>
    /// <param name="maxPackages">Maximum number of packages to warm</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Number of packages successfully warmed</returns>
    Task<int> WarmPopularPackagesAsync(int maxPackages = 50, CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs immediate cache warming with popular search queries
    /// </summary>
    /// <param name="maxQueries">Maximum number of queries to warm</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Number of queries successfully warmed</returns>
    Task<int> WarmPopularSearchesAsync(int maxQueries = 20, CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs comprehensive cache maintenance
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Maintenance result</returns>
    Task<CacheMaintenanceResult> PerformMaintenanceAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the current status of background operations
    /// </summary>
    /// <returns>Background operation status</returns>
    CacheWarmupStatus GetStatus();
}

/// <summary>
/// Status information for cache warmup service
/// </summary>
public class CacheWarmupStatus {
    /// <summary>
    /// Whether background operations are running
    /// </summary>
    public bool IsRunning { get; set; }

    /// <summary>
    /// When background operations were started
    /// </summary>
    public DateTimeOffset? StartedAt { get; set; }

    /// <summary>
    /// Last maintenance operation time
    /// </summary>
    public DateTimeOffset? LastMaintenanceAt { get; set; }

    /// <summary>
    /// Last package warming time
    /// </summary>
    public DateTimeOffset? LastPackageWarmupAt { get; set; }

    /// <summary>
    /// Last search warming time
    /// </summary>
    public DateTimeOffset? LastSearchWarmupAt { get; set; }

    /// <summary>
    /// Number of packages warmed in the last operation
    /// </summary>
    public int LastPackagesWarmed { get; set; }

    /// <summary>
    /// Number of searches warmed in the last operation
    /// </summary>
    public int LastSearchesWarmed { get; set; }

    /// <summary>
    /// Any errors from the last background operation
    /// </summary>
    public List<string> LastErrors { get; set; } = new();

    /// <summary>
    /// Next scheduled maintenance operation
    /// </summary>
    public DateTimeOffset? NextMaintenanceAt { get; set; }

    /// <summary>
    /// Total uptime of background operations
    /// </summary>
    public TimeSpan? Uptime => StartedAt.HasValue ? DateTimeOffset.UtcNow - StartedAt.Value : null;
}