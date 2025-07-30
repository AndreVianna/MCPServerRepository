using MCPHub.CommandLineApp.Configuration;

namespace MCPHub.CommandLineApp.Services;

/// <summary>
/// Offline mode service implementation with fallback support and connectivity management
/// </summary>
public class OfflineModeService : IOfflineModeService {
    private readonly ILogger<OfflineModeService> _logger;
    private readonly IMcpHubApiClient _apiClient;
    private readonly ICacheService _cacheService;
    private readonly IPackageCacheService _packageCache;
    private readonly McpmConfiguration _configuration;
    private bool _isManuallyEnabled;
    private DateTimeOffset? _offlineSince;
    private DateTimeOffset? _lastConnectivityTest;
    private bool? _lastConnectivityResult;
    private string? _offlineReason;

    private readonly SemaphoreSlim _connectivityLock = new(1, 1);
    private readonly Timer? _connectivityTimer;

    public OfflineModeService(
        ILogger<OfflineModeService> logger,
        IMcpHubApiClient apiClient,
        ICacheService cacheService,
        IPackageCacheService packageCache,
        McpmConfiguration configuration) {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        _packageCache = packageCache ?? throw new ArgumentNullException(nameof(packageCache));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

        // Start periodic connectivity checks if not manually disabled
        if (_configuration.Cache.OfflineModeEnabled) {
            _connectivityTimer = new Timer(
                async _ => await PeriodicConnectivityCheckAsync(),
                null,
                TimeSpan.FromMinutes(1), // Initial delay
                TimeSpan.FromMinutes(5)  // Check every 5 minutes
            );
        }
    }

    public bool IsOfflineMode { get; private set; }

    public async Task<bool> TestConnectivityAsync(CancellationToken cancellationToken = default) {
        await _connectivityLock.WaitAsync(cancellationToken).ConfigureAwait(false);

        try {
            _logger.LogDebug("Testing API connectivity");

            var isConnected = await _apiClient.TestConnectivityAsync(cancellationToken).ConfigureAwait(false);

            _lastConnectivityTest = DateTimeOffset.UtcNow;
            _lastConnectivityResult = isConnected;

            if (isConnected && IsOfflineMode && !_isManuallyEnabled) {
                // Connectivity restored, exit offline mode
                _logger.LogInformation("Connectivity restored, exiting offline mode");
                IsOfflineMode = false;
                _offlineSince = null;
                _offlineReason = null;
            }
            else if (!isConnected && !IsOfflineMode) {
                // Lost connectivity, enter offline mode
                _logger.LogWarning("Lost connectivity to API, entering offline mode");
                IsOfflineMode = true;
                _offlineSince = DateTimeOffset.UtcNow;
                _offlineReason = "API connectivity lost";
            }

            return isConnected;
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Failed to test API connectivity");

            _lastConnectivityTest = DateTimeOffset.UtcNow;
            _lastConnectivityResult = false;

            if (!IsOfflineMode) {
                _logger.LogWarning("Entering offline mode due to connectivity test failure");
                IsOfflineMode = true;
                _offlineSince = DateTimeOffset.UtcNow;
                _offlineReason = $"Connectivity test failed: {ex.Message}";
            }

            return false;
        }
        finally {
            _connectivityLock.Release();
        }
    }

    public void EnableOfflineMode() {
        _logger.LogInformation("Manually enabling offline mode");
        IsOfflineMode = true;
        _isManuallyEnabled = true;
        _offlineSince = DateTimeOffset.UtcNow;
        _offlineReason = "Manually enabled";
    }

    public void DisableOfflineMode() {
        _logger.LogInformation("Manually disabling offline mode");
        IsOfflineMode = false;
        _isManuallyEnabled = false;
        _offlineSince = null;
        _offlineReason = null;
    }

    public OfflineModeStatus GetOfflineModeStatus() {
        var cachedDataSummary = GetCachedDataSummaryAsync().GetAwaiter().GetResult();

        return new OfflineModeStatus {
            IsOffline = IsOfflineMode,
            IsManuallyEnabled = _isManuallyEnabled,
            OfflineSince = _offlineSince,
            LastConnectivityTest = _lastConnectivityTest,
            LastConnectivityResult = _lastConnectivityResult,
            OfflineReason = _offlineReason,
            CachedData = cachedDataSummary,
        };
    }

    public async Task<T?> ExecuteWithFallbackAsync<T>(
        Func<CancellationToken, Task<T>> onlineOperation,
        Func<CancellationToken, Task<T?>> offlineOperation,
        string cacheKey,
        CancellationToken cancellationToken = default) where T : class {

        ArgumentNullException.ThrowIfNull(onlineOperation);
        ArgumentNullException.ThrowIfNull(offlineOperation);

        // If we're offline or caching is disabled, try fallback first
        if (IsOfflineMode || !_configuration.Cache.Enabled) {
            _logger.LogDebug("Executing fallback operation for cache key: {CacheKey}", cacheKey);

            try {
                var fallbackResult = await offlineOperation(cancellationToken).ConfigureAwait(false);
                if (fallbackResult != null) {
                    return fallbackResult;
                }
            }
            catch (Exception ex) {
                _logger.LogWarning(ex, "Fallback operation failed for cache key: {CacheKey}", cacheKey);
            }

            // If manually offline, don't try online operation
            if (_isManuallyEnabled) {
                return null;
            }
        }

        // Try online operation
        try {
            _logger.LogDebug("Executing online operation for cache key: {CacheKey}", cacheKey);
            var result = await onlineOperation(cancellationToken).ConfigureAwait(false);

            // If we were offline but online operation succeeded, we might be back online
            if (IsOfflineMode && !_isManuallyEnabled) {
                _logger.LogInformation("Online operation succeeded, connectivity may be restored");
                _ = Task.Run(async () => await TestConnectivityAsync(CancellationToken.None), CancellationToken.None);
            }

            return result;
        }
        catch (Exception ex) {
            _logger.LogWarning(ex, "Online operation failed for cache key: {CacheKey}, trying fallback", cacheKey);

            // Online operation failed, enter offline mode if not already
            if (!IsOfflineMode) {
                IsOfflineMode = true;
                _offlineSince = DateTimeOffset.UtcNow;
                _offlineReason = $"Online operation failed: {ex.Message}";
            }

            // Try fallback operation
            try {
                return await offlineOperation(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception fallbackEx) {
                _logger.LogError(fallbackEx, "Both online and fallback operations failed for cache key: {CacheKey}", cacheKey);
                throw; // Re-throw the fallback exception
            }
        }
    }

    public async Task<SynchronizationResult> SynchronizeAsync(CancellationToken cancellationToken = default) {
        var startTime = DateTimeOffset.UtcNow;
        var result = new SynchronizationResult();

        if (IsOfflineMode) {
            result.Errors.Add("Cannot synchronize while in offline mode");
            return result;
        }

        try {
            // Test connectivity first
            var isConnected = await TestConnectivityAsync(cancellationToken).ConfigureAwait(false);
            if (!isConnected) {
                result.Errors.Add("No connectivity available for synchronization");
                return result;
            }

            _logger.LogInformation("Starting cache synchronization");

            // TODO: Implement actual synchronization logic
            // This would involve:
            // 1. Getting list of cached items that might be stale
            // 2. Checking for updates from the API
            // 3. Updating cache with fresh data
            // 4. Removing obsolete entries

            // For now, just perform cache maintenance
            var maintenanceResult = await _cacheService.PerformMaintenanceAsync(cancellationToken).ConfigureAwait(false);

            result.ItemsRemoved = maintenanceResult.ExpiredEntriesRemoved;
            result.Success = maintenanceResult.Success;
            result.Errors.AddRange(maintenanceResult.Errors);

            result.SynchronizationTime = DateTimeOffset.UtcNow - startTime;

            _logger.LogInformation("Cache synchronization completed: {Success}", result.Success);
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Cache synchronization failed");
            result.Errors.Add($"Synchronization failed: {ex.Message}");
        }

        return result;
    }

    public Dictionary<string, OfflineCapability> GetOfflineCapabilities() => new() {
        ["search"] = new OfflineCapability {
            OperationName = "Package Search",
            IsAvailableOffline = true,
            DataFreshnessConfidence = CalculateSearchDataFreshness(),
            Limitations = [
                    "Results may be outdated",
                    "New packages not included",
                    "Limited to cached search queries",
                          ],
            RequiredCachedData = ["Search results", "Package metadata"],
            FallbackBehavior = "Return cached search results matching query",
        },
        ["package-info"] = new OfflineCapability {
            OperationName = "Package Information",
            IsAvailableOffline = true,
            DataFreshnessConfidence = CalculatePackageDataFreshness(),
            Limitations = [
                    "Information may be outdated",
                    "Download counts not current",
                    "Recent versions may be missing",
                          ],
            RequiredCachedData = ["Package metadata", "Version information"],
            FallbackBehavior = "Return cached package information",
        },
        ["install"] = new OfflineCapability {
            OperationName = "Package Installation",
            IsAvailableOffline = false,
            DataFreshnessConfidence = 0,
            Limitations = [
                    "Requires package download",
                    "Cannot verify latest version",
                    "Dependency resolution limited",
                          ],
            RequiredCachedData = ["Package manifest", "Dependency information"],
            FallbackBehavior = "Installation not possible offline",
        },
        ["publish"] = new OfflineCapability {
            OperationName = "Package Publishing",
            IsAvailableOffline = false,
            DataFreshnessConfidence = 0,
            Limitations = ["Requires API connectivity"],
            RequiredCachedData = [],
            FallbackBehavior = "Publishing not possible offline",
        },
    };

    public OfflineOperationValidation ValidateOfflineOperation(string operationType) {
        var capabilities = GetOfflineCapabilities();
        var validation = new OfflineOperationValidation();

        if (!capabilities.TryGetValue(operationType, out var capability)) {
            validation.CanProceedOffline = false;
            validation.Errors.Add($"Unknown operation type: {operationType}");
            return validation;
        }

        validation.Capability = capability;
        validation.CanProceedOffline = capability.IsAvailableOffline;

        if (!capability.IsAvailableOffline) {
            validation.Errors.Add($"Operation '{operationType}' requires online connectivity");
        }
        else {
            if (capability.DataFreshnessConfidence < 50) {
                validation.Warnings.Add("Cached data may be significantly outdated");
            }

            if (capability.Limitations.Any()) {
                validation.Warnings.AddRange(capability.Limitations.Select(l => $"Limitation: {l}"));
            }

            // Check if required cached data is available
            var missingData = new List<string>();
            foreach (var requiredData in capability.RequiredCachedData) {
                // TODO: Actually check if the required data types are available in cache
                // This would require more sophisticated cache introspection
            }

            if (missingData.Any()) {
                validation.SuggestedActions.Add($"Cache the following data types: {string.Join(", ", missingData)}");
            }
        }

        return validation;
    }

    public async Task<OfflinePreparationResult> PrepareForOfflineAsync(IEnumerable<string>? essentialPackages = null, CancellationToken cancellationToken = default) {
        var startTime = DateTimeOffset.UtcNow;
        var result = new OfflinePreparationResult();

        try {
            _logger.LogInformation("Preparing for offline mode");

            // Test connectivity before preparation
            var isConnected = await TestConnectivityAsync(cancellationToken).ConfigureAwait(false);
            if (!isConnected) {
                result.Errors.Add("Cannot prepare for offline mode without connectivity");
                return result;
            }

            // TODO: Implement actual offline preparation
            // This would involve:
            // 1. Preload popular packages if configured
            // 2. Cache essential packages provided by user
            // 3. Cache popular search queries
            // 4. Ensure all cache types have some baseline data

            if (_configuration.Cache.PreloadPopularPackages) {
                var preloadCount = await _packageCache.PreloadPackagesAsync(
                    maxPackages: _configuration.Cache.PopularPackagesPreloadCount,
                    cancellationToken: cancellationToken
                ).ConfigureAwait(false);

                result.PackagesCached = preloadCount;
            }

            if (essentialPackages?.Any() == true) {
                foreach (var packageName in essentialPackages) {
                    try {
                        // TODO: Cache essential package data
                        _logger.LogDebug("Would cache essential package: {PackageName}", packageName);
                        // This requires integration with API client to fetch and cache package data
                    }
                    catch (Exception ex) {
                        _logger.LogWarning(ex, "Failed to cache essential package: {PackageName}", packageName);
                        result.FailedPackages.Add(packageName);
                    }
                }
            }

            result.Success = true;
            result.PreparationTime = DateTimeOffset.UtcNow - startTime;

            _logger.LogInformation("Offline preparation completed successfully");
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Offline preparation failed");
            result.Errors.Add($"Preparation failed: {ex.Message}");
        }

        return result;
    }

    private async Task PeriodicConnectivityCheckAsync() {
        if (_isManuallyEnabled) {
            return; // Don't check connectivity if manually offline
        }

        try {
            await TestConnectivityAsync(CancellationToken.None).ConfigureAwait(false);
        }
        catch (Exception ex) {
            _logger.LogDebug(ex, "Periodic connectivity check failed");
        }
    }

    private async Task<CachedDataSummary> GetCachedDataSummaryAsync() {
        try {
            var cacheStats = await _cacheService.GetStatisticsAsync().ConfigureAwait(false);

            return new CachedDataSummary {
                CachedPackages = cacheStats.EntriesByType.GetValueOrDefault("PackageInfoResponse", 0),
                CachedSearchResults = cacheStats.EntriesByType.GetValueOrDefault("SearchResultResponse", 0),
                TotalCacheSize = cacheStats.TotalSizeBytes,
                // TODO: Calculate oldest and newest entries from cache statistics
            };
        }
        catch (Exception ex) {
            _logger.LogWarning(ex, "Failed to get cached data summary");
            return new CachedDataSummary();
        }
    }

    private int CalculateSearchDataFreshness()
        // TODO: Implement actual freshness calculation based on cache ages
        // For now, return a placeholder value
        => IsOfflineMode ? 70 : 90;

    private int CalculatePackageDataFreshness()
        // TODO: Implement actual freshness calculation based on cache ages
        // For now, return a placeholder value
        => IsOfflineMode ? 75 : 95;

    public void Dispose() {
        _connectivityTimer?.Dispose();
        _connectivityLock?.Dispose();
    }
}