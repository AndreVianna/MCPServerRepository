using MCPHub.CommandLineApp.Configuration;

namespace MCPHub.CommandLineApp.Services;

/// <summary>
/// Service for cache warming and background maintenance operations
/// </summary>
public class CacheWarmupService(
    ILogger<CacheWarmupService> logger,
    ICacheService cacheService,
    IPackageCacheService packageCache,
    ISearchCacheService searchCache,
    IOfflineModeService offlineMode,
    McpmConfiguration configuration) : ICacheWarmupService {
    private readonly ILogger<CacheWarmupService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly ICacheService _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
    private readonly IPackageCacheService _packageCache = packageCache ?? throw new ArgumentNullException(nameof(packageCache));
    private readonly ISearchCacheService _searchCache = searchCache ?? throw new ArgumentNullException(nameof(searchCache));
    private readonly IOfflineModeService _offlineMode = offlineMode ?? throw new ArgumentNullException(nameof(offlineMode));
    private readonly McpmConfiguration _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

    private Timer? _maintenanceTimer;
    private Timer? _warmupTimer;
    private readonly CacheWarmupStatus _status = new();
    private readonly SemaphoreSlim _operationLock = new(1, 1);

    public async Task StartAsync(CancellationToken cancellationToken = default) {
        if (_status.IsRunning) {
            _logger.LogWarning("Cache warmup service is already running");
            return;
        }

        await _operationLock.WaitAsync(cancellationToken).ConfigureAwait(false);

        try {
            _logger.LogInformation("Starting cache warmup service");

            _status.IsRunning = true;
            _status.StartedAt = DateTimeOffset.UtcNow;
            _status.LastErrors.Clear();

            // Start maintenance timer if enabled
            if (_configuration.Cache.BackgroundMaintenance) {
                _maintenanceTimer = new Timer(
                    async _ => await PerformBackgroundMaintenanceAsync(),
                    null,
                    _configuration.Cache.MaintenanceInterval,
                    _configuration.Cache.MaintenanceInterval);

                _status.NextMaintenanceAt = DateTimeOffset.UtcNow.Add(_configuration.Cache.MaintenanceInterval);
            }

            // Start cache warming timer if popular package preloading is enabled
            if (_configuration.Cache.PreloadPopularPackages) {
                var warmupInterval = TimeSpan.FromHours(6); // Warm cache every 6 hours
                _warmupTimer = new Timer(
                    async _ => await PerformBackgroundWarmupAsync(),
                    null,
                    TimeSpan.FromMinutes(5), // Initial delay
                    warmupInterval);
            }

            _logger.LogInformation("Cache warmup service started successfully");
        }
        finally {
            _operationLock.Release();
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken = default) {
        if (!_status.IsRunning) {
            return;
        }

        await _operationLock.WaitAsync(cancellationToken).ConfigureAwait(false);

        try {
            _logger.LogInformation("Stopping cache warmup service");

            _maintenanceTimer?.Dispose();
            _maintenanceTimer = null;

            _warmupTimer?.Dispose();
            _warmupTimer = null;

            _status.IsRunning = false;
            _status.NextMaintenanceAt = null;

            _logger.LogInformation("Cache warmup service stopped");
        }
        finally {
            _operationLock.Release();
        }
    }

    public async Task<int> WarmPopularPackagesAsync(int maxPackages = 50, CancellationToken cancellationToken = default) {
        if (_offlineMode.IsOfflineMode) {
            _logger.LogWarning("Cannot warm package cache while offline");
            return 0;
        }

        await _operationLock.WaitAsync(cancellationToken).ConfigureAwait(false);

        try {
            _logger.LogInformation("Warming cache with popular packages (max: {MaxPackages})", maxPackages);

            var warmedCount = await _packageCache.PreloadPackagesAsync(
                maxPackages: maxPackages,
                cancellationToken: cancellationToken
            ).ConfigureAwait(false);

            _status.LastPackageWarmupAt = DateTimeOffset.UtcNow;
            _status.LastPackagesWarmed = warmedCount;

            _logger.LogInformation("Package cache warmed: {Count} packages", warmedCount);
            return warmedCount;
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Failed to warm package cache");
            _status.LastErrors.Add($"Package warmup failed: {ex.Message}");
            return 0;
        }
        finally {
            _operationLock.Release();
        }
    }

    public async Task<int> WarmPopularSearchesAsync(int maxQueries = 20, CancellationToken cancellationToken = default) {
        if (_offlineMode.IsOfflineMode) {
            _logger.LogWarning("Cannot warm search cache while offline");
            return 0;
        }

        await _operationLock.WaitAsync(cancellationToken).ConfigureAwait(false);

        try {
            _logger.LogInformation("Warming cache with popular search queries (max: {MaxQueries})", maxQueries);

            // Common search terms that users might look for
            var popularQueries = new[] {
                "authentication", "auth", "database", "db", "web", "api", "rest", "graphql",
                "tool", "utility", "helper", "client", "server", "service", "manager",
                "test", "testing", "mock", "data", "json", "xml", "csv", "file",
                "email", "mail", "notification", "log", "logging", "monitor", "trace",
                                       }.Take(maxQueries);

            var warmedCount = await _searchCache.WarmSearchCacheAsync(
                popularQueries,
                cancellationToken
            ).ConfigureAwait(false);

            _status.LastSearchWarmupAt = DateTimeOffset.UtcNow;
            _status.LastSearchesWarmed = warmedCount;

            _logger.LogInformation("Search cache warmed: {Count} queries", warmedCount);
            return warmedCount;
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Failed to warm search cache");
            _status.LastErrors.Add($"Search warmup failed: {ex.Message}");
            return 0;
        }
        finally {
            _operationLock.Release();
        }
    }

    public async Task<CacheMaintenanceResult> PerformMaintenanceAsync(CancellationToken cancellationToken = default) {
        await _operationLock.WaitAsync(cancellationToken).ConfigureAwait(false);

        try {
            _logger.LogInformation("Performing cache maintenance");

            var result = await _cacheService.PerformMaintenanceAsync(cancellationToken).ConfigureAwait(false);

            _status.LastMaintenanceAt = DateTimeOffset.UtcNow;

            if (_configuration.Cache.BackgroundMaintenance) {
                _status.NextMaintenanceAt = DateTimeOffset.UtcNow.Add(_configuration.Cache.MaintenanceInterval);
            }

            if (!result.Success) {
                _status.LastErrors.AddRange(result.Errors);
            }

            _logger.LogInformation("Cache maintenance completed: {Success}, removed {Count} expired entries, reclaimed {Size}",
                result.Success, result.ExpiredEntriesRemoved, result.BytesReclaimedFormatted);

            return result;
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Cache maintenance failed");
            var errorResult = new CacheMaintenanceResult();
            errorResult.Errors.Add($"Maintenance failed: {ex.Message}");
            _status.LastErrors.Add($"Maintenance failed: {ex.Message}");
            return errorResult;
        }
        finally {
            _operationLock.Release();
        }
    }

    public CacheWarmupStatus GetStatus() => new() {
        IsRunning = _status.IsRunning,
        StartedAt = _status.StartedAt,
        LastMaintenanceAt = _status.LastMaintenanceAt,
        LastPackageWarmupAt = _status.LastPackageWarmupAt,
        LastSearchWarmupAt = _status.LastSearchWarmupAt,
        LastPackagesWarmed = _status.LastPackagesWarmed,
        LastSearchesWarmed = _status.LastSearchesWarmed,
        LastErrors = [.. _status.LastErrors],
        NextMaintenanceAt = _status.NextMaintenanceAt,
    };

    private async Task PerformBackgroundMaintenanceAsync() {
        try {
            await PerformMaintenanceAsync().ConfigureAwait(false);
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Background maintenance operation failed");
        }
    }

    private async Task PerformBackgroundWarmupAsync() {
        try {
            // Only warm cache if we're not offline and have connectivity
            if (!_offlineMode.IsOfflineMode) {
                var packageCount = Math.Min(10, _configuration.Cache.PopularPackagesPreloadCount / 5); // Smaller batches for background
                await WarmPopularPackagesAsync(packageCount).ConfigureAwait(false);

                // Small delay between operations
                await Task.Delay(TimeSpan.FromSeconds(30)).ConfigureAwait(false);

                await WarmPopularSearchesAsync(5).ConfigureAwait(false); // Just a few searches
            }
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Background warmup operation failed");
        }
    }

    public void Dispose() {
        try {
            StopAsync().GetAwaiter().GetResult();
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error stopping cache warmup service during disposal");
        }

        _maintenanceTimer?.Dispose();
        _warmupTimer?.Dispose();
        _operationLock?.Dispose();
    }
}