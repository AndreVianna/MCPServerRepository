using System.CommandLine;

using MCPHub.CommandLineApp.Configuration;
using MCPHub.CommandLineApp.Services;
using MCPHub.CommandLineApp.Utilities;

namespace MCPHub.CommandLineApp.Commands;

/// <summary>
/// Command for managing CLI cache operations
/// </summary>
public class CacheCommand(
    ILogger<CacheCommand> logger,
    McpmConfiguration configuration,
    IMcpHubApiClient apiClient,
    IOutputFormatter outputFormatter,
    IInteractionService interactionService,
    IProgressReporter progressReporter,
    ICacheService cacheService,
    IPackageCacheService packageCache,
    ISearchCacheService searchCache,
    IOfflineModeService offlineMode) : BaseCommand(logger, configuration, apiClient, outputFormatter, interactionService, progressReporter) {

    private readonly ICacheService _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
    private readonly IPackageCacheService _packageCache = packageCache ?? throw new ArgumentNullException(nameof(packageCache));
    private readonly ISearchCacheService _searchCache = searchCache ?? throw new ArgumentNullException(nameof(searchCache));
    private readonly IOfflineModeService _offlineMode = offlineMode ?? throw new ArgumentNullException(nameof(offlineMode));

    /// <inheritdoc />
    public override Command CreateCommand() {
        var command = new Command("cache", "Manage CLI cache operations");

        // Subcommands
        command.AddCommand(CreateStatsCommand());
        command.AddCommand(CreateClearCommand());
        command.AddCommand(CreateRefreshCommand());
        command.AddCommand(CreateWarmCommand());
        command.AddCommand(CreateMaintenanceCommand());
        command.AddCommand(CreateOfflineCommand());

        return command;
    }

    private Command CreateStatsCommand() {
        var command = new Command("stats", "Show cache statistics and information");

        var detailedOption = new Option<bool>(
            aliases: ["--detailed", "-d"],
            description: "Show detailed cache information");
        command.AddOption(detailedOption);

        var formatOption = new Option<string?>(
            aliases: ["--format", "-f"],
            description: "Output format (table, json)");
        command.AddOption(formatOption);

        command.SetHandler(async (detailed, format) => {
            var exitCode = await ShowCacheStatsAsync(detailed, format);
            Environment.Exit(exitCode);
        }, detailedOption, formatOption);

        return command;
    }

    private Command CreateClearCommand() {
        var command = new Command("clear", "Clear cache entries");

        var allOption = new Option<bool>(
            aliases: ["--all", "-a"],
            description: "Clear all cache entries");
        command.AddOption(allOption);

        var patternOption = new Option<string?>(
            aliases: ["--pattern", "-p"],
            description: "Clear entries matching pattern (supports wildcards)");
        command.AddOption(patternOption);

        var typeOption = new Option<string?>(
            aliases: ["--type", "-t"],
            description: "Clear entries of specific type (packages, search, security, trust)");
        command.AddOption(typeOption);

        var expiredOption = new Option<bool>(
            aliases: ["--expired", "-e"],
            description: "Clear only expired entries");
        command.AddOption(expiredOption);

        var confirmOption = new Option<bool>(
            aliases: ["--yes", "-y"],
            description: "Skip confirmation prompt");
        command.AddOption(confirmOption);

        command.SetHandler(async (all, pattern, type, expired, confirm) => {
            var exitCode = await ClearCacheAsync(all, pattern, type, expired, confirm);
            Environment.Exit(exitCode);
        }, allOption, patternOption, typeOption, expiredOption, confirmOption);

        return command;
    }

    private Command CreateRefreshCommand() {
        var command = new Command("refresh", "Refresh cached data from API");

        var packageOption = new Option<string?>(
            aliases: ["--package", "-p"],
            description: "Refresh cache for specific package");
        command.AddOption(packageOption);

        var typeOption = new Option<string?>(
            aliases: ["--type", "-t"],
            description: "Refresh specific data type (info, versions, security, trust)");
        command.AddOption(typeOption);

        var allOption = new Option<bool>(
            aliases: ["--all", "-a"],
            description: "Refresh all cached data");
        command.AddOption(allOption);

        command.SetHandler(async (package, type, all) => {
            var exitCode = await RefreshCacheAsync(package, type, all);
            Environment.Exit(exitCode);
        }, packageOption, typeOption, allOption);

        return command;
    }

    private Command CreateWarmCommand() {
        var command = new Command("warm", "Pre-populate cache with popular data");

        var packagesOption = new Option<int>(
            aliases: ["--packages", "-p"],
            description: "Number of popular packages to cache",
            getDefaultValue: () => 50);
        command.AddOption(packagesOption);

        var searchOption = new Option<bool>(
            aliases: ["--search", "-s"],
            description: "Warm search cache with popular queries");
        command.AddOption(searchOption);

        var essentialOption = new Option<string[]?>(
            aliases: ["--essential", "-e"],
            description: "Essential packages to always cache (comma-separated)");
        command.AddOption(essentialOption);

        command.SetHandler(async (packages, search, essential) => {
            var exitCode = await WarmCacheAsync(packages, search, essential);
            Environment.Exit(exitCode);
        }, packagesOption, searchOption, essentialOption);

        return command;
    }

    private Command CreateMaintenanceCommand() {
        var command = new Command("maintenance", "Perform cache maintenance operations");

        var autoOption = new Option<bool>(
            aliases: ["--auto", "-a"],
            description: "Run automatic maintenance");
        command.AddOption(autoOption);

        var compactOption = new Option<bool>(
            aliases: ["--compact", "-c"],
            description: "Compact cache storage");
        command.AddOption(compactOption);

        var verifyOption = new Option<bool>(
            aliases: ["--verify", "-v"],
            description: "Verify cache integrity");
        command.AddOption(verifyOption);

        command.SetHandler(async (auto, compact, verify) => {
            var exitCode = await PerformMaintenanceAsync(auto, compact, verify);
            Environment.Exit(exitCode);
        }, autoOption, compactOption, verifyOption);

        return command;
    }

    private Command CreateOfflineCommand() {
        var command = new Command("offline", "Manage offline mode");

        var statusOption = new Option<bool>(
            aliases: ["--status", "-s"],
            description: "Show offline mode status");
        command.AddOption(statusOption);

        var enableOption = new Option<bool>(
            aliases: ["--enable", "-e"],
            description: "Enable offline mode");
        command.AddOption(enableOption);

        var disableOption = new Option<bool>(
            aliases: ["--disable", "-d"],
            description: "Disable offline mode");
        command.AddOption(disableOption);

        var prepareOption = new Option<bool>(
            aliases: ["--prepare", "-p"],
            description: "Prepare for offline use");
        command.AddOption(prepareOption);

        var syncOption = new Option<bool>(
            aliases: ["--sync"],
            description: "Synchronize with API when online");
        command.AddOption(syncOption);

        command.SetHandler(async (status, enable, disable, prepare, sync) => {
            var exitCode = await ManageOfflineModeAsync(status, enable, disable, prepare, sync);
            Environment.Exit(exitCode);
        }, statusOption, enableOption, disableOption, prepareOption, syncOption);

        return command;
    }

    private async Task<int> ShowCacheStatsAsync(bool detailed, string? format) {
        try {
            Logger.LogInformation("Retrieving cache statistics");

            using var spinner = ProgressReporter.CreateSpinner("Gathering cache statistics...");

            var cacheStats = await _cacheService.GetStatisticsAsync().ConfigureAwait(false);
            var searchStats = await _searchCache.GetSearchCacheStatisticsAsync().ConfigureAwait(false);
            var offlineStatus = _offlineMode.GetOfflineModeStatus();

            spinner.Success("Cache statistics retrieved");

            var outputFormat = ValidateOutputFormat(format);

            if (outputFormat == "json") {
                var jsonData = new {
                    cache = cacheStats,
                    search = searchStats,
                    offline = offlineStatus,
                    timestamp = DateTimeOffset.UtcNow
                };
                OutputFormatter.WriteJson(jsonData);
            }
            else {
                DisplayCacheStatsTable(cacheStats, searchStats, offlineStatus, detailed);
            }

            return 0;
        }
        catch (Exception ex) {
            return HandleError(ex, "cache stats");
        }
    }

    private async Task<int> ClearCacheAsync(bool all, string? pattern, string? type, bool expiredOnly, bool skipConfirmation) {
        try {
            if (!all && string.IsNullOrEmpty(pattern) && string.IsNullOrEmpty(type) && !expiredOnly) {
                OutputFormatter.WriteError("Must specify what to clear: --all, --pattern, --type, or --expired");
                return 400;
            }

            // Get confirmation unless skipped
            if (!skipConfirmation) {
                var prompt = all ? "Clear ALL cache entries?" :
                               expiredOnly ? "Clear expired cache entries?" :
                               !string.IsNullOrEmpty(pattern) ? $"Clear entries matching pattern '{pattern}'?" :
                               $"Clear entries of type '{type}'?";

                if (!await InteractionService.ConfirmAsync(prompt, false)) {
                    OutputFormatter.WriteInfo("Cache clear cancelled");
                    return 0;
                }
            }

            using var spinner = ProgressReporter.CreateSpinner("Clearing cache...");
            var removedCount = 0;

            if (all) {
                await _cacheService.ClearAsync().ConfigureAwait(false);
                var stats = await _cacheService.GetStatisticsAsync().ConfigureAwait(false);
                removedCount = stats.TotalEntries;
            }
            else if (expiredOnly) {
                var maintenanceResult = await _cacheService.PerformMaintenanceAsync().ConfigureAwait(false);
                removedCount = maintenanceResult.ExpiredEntriesRemoved;
            }
            else if (!string.IsNullOrEmpty(pattern)) {
                removedCount = await _cacheService.RemoveByPatternAsync(pattern).ConfigureAwait(false);
            }
            else if (!string.IsNullOrEmpty(type)) {
                // Clear by type
                var typePattern = type.ToLowerInvariant() switch {
                    "packages" or "package" => "pkg:*",
                    "search" => "search:*",
                    "security" => "pkg:security:*",
                    "trust" => "pkg:trust:*",
                    _ => throw new ArgumentException($"Unknown cache type: {type}")
                };
                removedCount = await _cacheService.RemoveByPatternAsync(typePattern).ConfigureAwait(false);
            }

            spinner.Success($"Cleared {removedCount} cache entries");
            return 0;
        }
        catch (Exception ex) {
            return HandleError(ex, "cache clear");
        }
    }

    private async Task<int> RefreshCacheAsync(string? packageName, string? type, bool all) {
        try {
            if (!all && string.IsNullOrEmpty(packageName)) {
                OutputFormatter.WriteError("Must specify --package or --all for refresh");
                return 400;
            }

            // Check connectivity
            if (!await ValidateApiConnectivityAsync()) {
                return 503;
            }

            using var spinner = ProgressReporter.CreateSpinner("Refreshing cache...");

            if (all) {
                // TODO: Implement full cache refresh
                OutputFormatter.WriteWarning("Full cache refresh not yet implemented");
                return 501; // Not implemented
            }
            else if (!string.IsNullOrEmpty(packageName)) {
                // Refresh specific package
                await RefreshPackageCacheAsync(packageName, type);
                spinner.Success($"Refreshed cache for package: {packageName}");
            }

            return 0;
        }
        catch (Exception ex) {
            return HandleError(ex, "cache refresh");
        }
    }

    private async Task<int> WarmCacheAsync(int packageCount, bool warmSearch, string[]? essentialPackages) {
        try {
            // Check connectivity
            if (!await ValidateApiConnectivityAsync()) {
                return 503;
            }

            using var progress = ProgressReporter.CreateProgressBar("Warming cache", packageCount + (warmSearch ? 1 : 0));

            var totalWarmed = 0;

            // Warm package cache
            if (packageCount > 0) {
                progress.UpdateStatus("Warming package cache...");
                var warmedPackages = await _packageCache.PreloadPackagesAsync(maxPackages: packageCount).ConfigureAwait(false);
                totalWarmed += warmedPackages;
                progress.Increment(warmedPackages);
            }

            // Warm search cache
            if (warmSearch) {
                progress.UpdateStatus("Warming search cache...");
                var popularQueries = new[] { "authentication", "database", "web", "api", "tool", "utility" };
                var warmedQueries = await _searchCache.WarmSearchCacheAsync(popularQueries).ConfigureAwait(false);
                totalWarmed += warmedQueries;
                progress.Increment(1);
            }

            // Cache essential packages
            if (essentialPackages?.Length > 0) {
                foreach (var packageName in essentialPackages) {
                    try {
                        progress.UpdateStatus($"Caching essential package: {packageName}");
                        await RefreshPackageCacheAsync(packageName);
                        totalWarmed++;
                    }
                    catch (Exception ex) {
                        Logger.LogWarning(ex, "Failed to cache essential package: {PackageName}", packageName);
                    }
                }
            }

            progress.Complete($"Cache warmed: {totalWarmed} items");
            return 0;
        }
        catch (Exception ex) {
            return HandleError(ex, "cache warm");
        }
    }

    private async Task<int> PerformMaintenanceAsync(bool auto, bool compact, bool verify) {
        try {
            using var spinner = ProgressReporter.CreateSpinner("Performing cache maintenance...");

            var maintenanceResult = await _cacheService.PerformMaintenanceAsync().ConfigureAwait(false);

            if (maintenanceResult.Success) {
                spinner.Success($"Maintenance completed: Removed {maintenanceResult.ExpiredEntriesRemoved} expired entries, reclaimed {maintenanceResult.BytesReclaimedFormatted}");
            }
            else {
                spinner.Fail("Maintenance completed with errors");
                foreach (var error in maintenanceResult.Errors) {
                    OutputFormatter.WriteError(error);
                }
            }

            if (compact) {
                // TODO: Implement cache compaction
                OutputFormatter.WriteWarning("Cache compaction not yet implemented");
            }

            if (verify) {
                // TODO: Implement cache verification
                OutputFormatter.WriteWarning("Cache verification not yet implemented");
            }

            return maintenanceResult.Success ? 0 : 1;
        }
        catch (Exception ex) {
            return HandleError(ex, "cache maintenance");
        }
    }

    private async Task<int> ManageOfflineModeAsync(bool showStatus, bool enable, bool disable, bool prepare, bool sync) {
        try {
            if (enable && disable) {
                OutputFormatter.WriteError("Cannot both enable and disable offline mode");
                return 400;
            }

            if (enable) {
                _offlineMode.EnableOfflineMode();
                OutputFormatter.WriteSuccess("Offline mode enabled");
            }
            else if (disable) {
                _offlineMode.DisableOfflineMode();
                OutputFormatter.WriteSuccess("Offline mode disabled");
            }
            else if (prepare) {
                using var spinner = ProgressReporter.CreateSpinner("Preparing for offline use...");
                var result = await _offlineMode.PrepareForOfflineAsync().ConfigureAwait(false);

                if (result.Success) {
                    spinner.Success($"Offline preparation completed: {result.PackagesCached} packages cached, {result.DataDownloadedFormatted} downloaded");
                }
                else {
                    spinner.Fail("Offline preparation failed");
                    foreach (var error in result.Errors) {
                        OutputFormatter.WriteError(error);
                    }
                    return 1;
                }
            }
            else if (sync) {
                using var spinner = ProgressReporter.CreateSpinner("Synchronizing with API...");
                var result = await _offlineMode.SynchronizeAsync().ConfigureAwait(false);

                if (result.Success) {
                    spinner.Success($"Synchronization completed: {result.ItemsSynchronized} items synchronized");
                }
                else {
                    spinner.Fail("Synchronization failed");
                    foreach (var error in result.Errors) {
                        OutputFormatter.WriteError(error);
                    }
                    return 1;
                }
            }

            if (showStatus || (!enable && !disable && !prepare && !sync)) {
                DisplayOfflineStatus();
            }

            return 0;
        }
        catch (Exception ex) {
            return HandleError(ex, "offline mode");
        }
    }

    private void DisplayCacheStatsTable(CacheStatistics cacheStats, SearchCacheStatistics searchStats, OfflineModeStatus offlineStatus, bool detailed) {
        OutputFormatter.WriteHeader("Cache Statistics");

        // General cache info
        OutputFormatter.WriteInfo($"Total Entries: {cacheStats.TotalEntries:N0}");
        OutputFormatter.WriteInfo($"Total Size: {cacheStats.TotalSizeFormatted}");
        OutputFormatter.WriteInfo($"Expired Entries: {cacheStats.ExpiredEntries:N0}");
        OutputFormatter.WriteInfo($"Hit Rate: {cacheStats.HitRate:F1}%");
        OutputFormatter.WriteInfo($"Statistics Reset: {cacheStats.StatisticsResetAt:yyyy-MM-dd HH:mm:ss}");

        OutputFormatter.WriteLine();

        // Search cache info
        OutputFormatter.WriteSubHeader("Search Cache");
        OutputFormatter.WriteInfo($"Cached Queries: {searchStats.TotalCachedQueries:N0}");
        OutputFormatter.WriteInfo($"Search Hit Rate: {searchStats.SearchCacheHitRate:F1}%");
        OutputFormatter.WriteInfo($"Search Cache Size: {searchStats.TotalCacheSizeFormatted}");

        if (detailed && searchStats.PopularQueries.Any()) {
            OutputFormatter.WriteLine();
            OutputFormatter.WriteSubHeader("Popular Search Queries");
            foreach (var query in searchStats.PopularQueries.Take(5)) {
                OutputFormatter.WriteInfo($"  • {query.Query} ({query.SearchCount} searches)");
            }
        }

        OutputFormatter.WriteLine();

        // Offline status
        OutputFormatter.WriteSubHeader("Offline Mode");
        OutputFormatter.WriteInfo($"Status: {(offlineStatus.IsOffline ? "Offline" : "Online")}");
        if (offlineStatus.IsOffline) {
            OutputFormatter.WriteInfo($"Offline Since: {offlineStatus.OfflineSince:yyyy-MM-dd HH:mm:ss}");
            if (!string.IsNullOrEmpty(offlineStatus.OfflineReason)) {
                OutputFormatter.WriteInfo($"Reason: {offlineStatus.OfflineReason}");
            }
        }
        OutputFormatter.WriteInfo($"Cached Packages: {offlineStatus.CachedData.CachedPackages:N0}");
        OutputFormatter.WriteInfo($"Cached Searches: {offlineStatus.CachedData.CachedSearchResults:N0}");

        if (detailed && cacheStats.EntriesByType.Any()) {
            OutputFormatter.WriteLine();
            OutputFormatter.WriteSubHeader("Cache Entries by Type");
            foreach (var kvp in cacheStats.EntriesByType.OrderByDescending(x => x.Value)) {
                OutputFormatter.WriteInfo($"  {kvp.Key}: {kvp.Value:N0}");
            }
        }
    }

    private void DisplayOfflineStatus() {
        var status = _offlineMode.GetOfflineModeStatus();

        OutputFormatter.WriteHeader("Offline Mode Status");
        OutputFormatter.WriteInfo($"Status: {(status.IsOffline ? "Offline" : "Online")}");

        if (status.IsOffline) {
            OutputFormatter.WriteInfo($"Mode: {(status.IsManuallyEnabled ? "Manual" : "Automatic")}");
            if (status.OfflineSince.HasValue) {
                OutputFormatter.WriteInfo($"Offline Since: {status.OfflineSince:yyyy-MM-dd HH:mm:ss}");
                OutputFormatter.WriteInfo($"Duration: {status.EstimatedOfflineDuration?.ToString(@"dd\.hh\:mm\:ss")}");
            }
            if (!string.IsNullOrEmpty(status.OfflineReason)) {
                OutputFormatter.WriteInfo($"Reason: {status.OfflineReason}");
            }
        }

        if (status.LastConnectivityTest.HasValue) {
            OutputFormatter.WriteInfo($"Last Connectivity Test: {status.LastConnectivityTest:yyyy-MM-dd HH:mm:ss}");
            OutputFormatter.WriteInfo($"Result: {(status.LastConnectivityResult == true ? "Connected" : "Failed")}");
        }

        OutputFormatter.WriteLine();
        OutputFormatter.WriteSubHeader("Available Offline Data");
        OutputFormatter.WriteInfo($"Cached Packages: {status.CachedData.CachedPackages:N0}");
        OutputFormatter.WriteInfo($"Cached Searches: {status.CachedData.CachedSearchResults:N0}");
        OutputFormatter.WriteInfo($"Total Cache Size: {status.CachedData.TotalCacheSizeFormatted}");

        // Show offline capabilities
        OutputFormatter.WriteLine();
        OutputFormatter.WriteSubHeader("Offline Capabilities");
        var capabilities = _offlineMode.GetOfflineCapabilities();
        foreach (var kvp in capabilities) {
            var icon = kvp.Value.IsAvailableOffline ? "✓" : "✗";
            var confidence = kvp.Value.IsAvailableOffline ? $" ({kvp.Value.DataFreshnessConfidence}% fresh)" : "";
            OutputFormatter.WriteInfo($"  {icon} {kvp.Value.OperationName}{confidence}");
        }
    }

    private async Task RefreshPackageCacheAsync(string packageName, string? type = null) {
        // TODO: Implement package cache refresh by fetching fresh data from API
        // This would require integration with the API client
        Logger.LogInformation("Would refresh cache for package: {PackageName}, type: {Type}", packageName, type ?? "all");
        await Task.CompletedTask;
    }
}