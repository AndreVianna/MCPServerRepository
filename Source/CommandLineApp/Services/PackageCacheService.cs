namespace MCPHub.CommandLineApp.Services;

/// <summary>
/// Package-specific caching service implementation
/// </summary>
public class PackageCacheService(
    ILogger<PackageCacheService> logger,
    ICacheService cacheService,
    McpmConfiguration configuration) : IPackageCacheService {
    private readonly ILogger<PackageCacheService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly ICacheService _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
    private readonly McpmConfiguration _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

    private const string PACKAGE_INFO_PREFIX = "pkg:info:";
    private const string PACKAGE_VERSIONS_PREFIX = "pkg:versions:";
    private const string SECURITY_SUMMARY_PREFIX = "pkg:security:";
    private const string TRUST_TIER_PREFIX = "pkg:trust:";
    private const string DEPENDENCIES_PREFIX = "pkg:deps:";

    public async Task CachePackageInfoAsync(string packageName, PackageInfoResponse packageInfo, CancellationToken cancellationToken = default) {
        if (!_configuration.Cache.PackageInfo.Enabled) {
            _logger.LogDebug("Package info caching disabled");
            return;
        }

        ValidatePackageName(packageName);
        ArgumentNullException.ThrowIfNull(packageInfo);

        var key = GetPackageInfoKey(packageName);
        var expiration = _configuration.Cache.PackageInfo.Expiration;

        await _cacheService.SetAsync(key, packageInfo, expiration, cancellationToken).ConfigureAwait(false);
        _logger.LogDebug("Cached package info for: {PackageName}", packageName);
    }

    public async Task<PackageInfoResponse?> GetCachedPackageInfoAsync(string packageName, CancellationToken cancellationToken = default) {
        if (!_configuration.Cache.PackageInfo.Enabled) {
            return null;
        }

        ValidatePackageName(packageName);

        var key = GetPackageInfoKey(packageName);
        var result = await _cacheService.GetAsync<PackageInfoResponse>(key, cancellationToken).ConfigureAwait(false);

        if (result != null) {
            _logger.LogDebug("Retrieved cached package info for: {PackageName}", packageName);
        }

        return result;
    }

    public async Task CachePackageVersionsAsync(string packageName, PackageVersionsResponse versions, bool includePrerelease, CancellationToken cancellationToken = default) {
        if (!_configuration.Cache.PackageVersions.Enabled) {
            _logger.LogDebug("Package versions caching disabled");
            return;
        }

        ValidatePackageName(packageName);
        ArgumentNullException.ThrowIfNull(versions);

        var key = GetPackageVersionsKey(packageName, includePrerelease);
        var expiration = _configuration.Cache.PackageVersions.Expiration;

        await _cacheService.SetAsync(key, versions, expiration, cancellationToken).ConfigureAwait(false);
        _logger.LogDebug("Cached package versions for: {PackageName} (prerelease: {IncludePrerelease})", packageName, includePrerelease);
    }

    public async Task<PackageVersionsResponse?> GetCachedPackageVersionsAsync(string packageName, bool includePrerelease, CancellationToken cancellationToken = default) {
        if (!_configuration.Cache.PackageVersions.Enabled) {
            return null;
        }

        ValidatePackageName(packageName);

        var key = GetPackageVersionsKey(packageName, includePrerelease);
        var result = await _cacheService.GetAsync<PackageVersionsResponse>(key, cancellationToken).ConfigureAwait(false);

        if (result != null) {
            _logger.LogDebug("Retrieved cached package versions for: {PackageName} (prerelease: {IncludePrerelease})", packageName, includePrerelease);
        }

        return result;
    }

    public async Task CacheSecuritySummaryAsync(string packageName, SecuritySummaryResponse securitySummary, CancellationToken cancellationToken = default) {
        if (!_configuration.Cache.SecuritySummary.Enabled) {
            _logger.LogDebug("Security summary caching disabled");
            return;
        }

        ValidatePackageName(packageName);
        ArgumentNullException.ThrowIfNull(securitySummary);

        var key = GetSecuritySummaryKey(packageName);
        var expiration = _configuration.Cache.SecuritySummary.Expiration;

        await _cacheService.SetAsync(key, securitySummary, expiration, cancellationToken).ConfigureAwait(false);
        _logger.LogDebug("Cached security summary for: {PackageName}", packageName);
    }

    public async Task<SecuritySummaryResponse?> GetCachedSecuritySummaryAsync(string packageName, CancellationToken cancellationToken = default) {
        if (!_configuration.Cache.SecuritySummary.Enabled) {
            return null;
        }

        ValidatePackageName(packageName);

        var key = GetSecuritySummaryKey(packageName);
        var result = await _cacheService.GetAsync<SecuritySummaryResponse>(key, cancellationToken).ConfigureAwait(false);

        if (result != null) {
            _logger.LogDebug("Retrieved cached security summary for: {PackageName}", packageName);
        }

        return result;
    }

    public async Task CacheTrustTierAsync(string packageName, TrustTierResponse trustTier, CancellationToken cancellationToken = default) {
        if (!_configuration.Cache.TrustTier.Enabled) {
            _logger.LogDebug("Trust tier caching disabled");
            return;
        }

        ValidatePackageName(packageName);
        ArgumentNullException.ThrowIfNull(trustTier);

        var key = GetTrustTierKey(packageName);
        var expiration = _configuration.Cache.TrustTier.Expiration;

        await _cacheService.SetAsync(key, trustTier, expiration, cancellationToken).ConfigureAwait(false);
        _logger.LogDebug("Cached trust tier for: {PackageName}", packageName);
    }

    public async Task<TrustTierResponse?> GetCachedTrustTierAsync(string packageName, CancellationToken cancellationToken = default) {
        if (!_configuration.Cache.TrustTier.Enabled) {
            return null;
        }

        ValidatePackageName(packageName);

        var key = GetTrustTierKey(packageName);
        var result = await _cacheService.GetAsync<TrustTierResponse>(key, cancellationToken).ConfigureAwait(false);

        if (result != null) {
            _logger.LogDebug("Retrieved cached trust tier for: {PackageName}", packageName);
        }

        return result;
    }

    public async Task CachePackageDependenciesAsync(string packageName, string version, PackageDependenciesResponse dependencies, CancellationToken cancellationToken = default) {
        if (!_configuration.Cache.Dependencies.Enabled) {
            _logger.LogDebug("Dependencies caching disabled");
            return;
        }

        ValidatePackageName(packageName);
        ValidateVersion(version);
        ArgumentNullException.ThrowIfNull(dependencies);

        var key = GetDependenciesKey(packageName, version);
        var expiration = _configuration.Cache.Dependencies.Expiration;

        await _cacheService.SetAsync(key, dependencies, expiration, cancellationToken).ConfigureAwait(false);
        _logger.LogDebug("Cached dependencies for: {PackageName} v{Version}", packageName, version);
    }

    public async Task<PackageDependenciesResponse?> GetCachedPackageDependenciesAsync(string packageName, string version, CancellationToken cancellationToken = default) {
        if (!_configuration.Cache.Dependencies.Enabled) {
            return null;
        }

        ValidatePackageName(packageName);
        ValidateVersion(version);

        var key = GetDependenciesKey(packageName, version);
        var result = await _cacheService.GetAsync<PackageDependenciesResponse>(key, cancellationToken).ConfigureAwait(false);

        if (result != null) {
            _logger.LogDebug("Retrieved cached dependencies for: {PackageName} v{Version}", packageName, version);
        }

        return result;
    }

    public async Task<int> InvalidatePackageAsync(string packageName, CancellationToken cancellationToken = default) {
        ValidatePackageName(packageName);

        var patterns = new[] {
            $"{PACKAGE_INFO_PREFIX}{packageName}*",
            $"{PACKAGE_VERSIONS_PREFIX}{packageName}*",
            $"{SECURITY_SUMMARY_PREFIX}{packageName}*",
            $"{TRUST_TIER_PREFIX}{packageName}*",
            $"{DEPENDENCIES_PREFIX}{packageName}*",
                             };

        var totalRemoved = 0;
        foreach (var pattern in patterns) {
            var removed = await _cacheService.RemoveByPatternAsync(pattern, cancellationToken).ConfigureAwait(false);
            totalRemoved += removed;
        }

        _logger.LogInformation("Invalidated {Count} cache entries for package: {PackageName}", totalRemoved, packageName);
        return totalRemoved;
    }

    public async Task<PackageCacheFreshness> GetPackageCacheFreshnessAsync(string packageName, CancellationToken cancellationToken = default) {
        ValidatePackageName(packageName);

        var freshness = new PackageCacheFreshness {
            PackageName = packageName,
        };

        // Check package info
        var packageInfoKey = GetPackageInfoKey(packageName);
        var packageInfoEntry = await _cacheService.GetEntryInfoAsync(packageInfoKey, cancellationToken).ConfigureAwait(false);
        if (packageInfoEntry != null) {
            freshness.PackageInfoCachedAt = packageInfoEntry.CreatedAt;
            freshness.HasFreshPackageInfo = !packageInfoEntry.IsExpired;
        }

        // Check package versions (stable)
        var versionsKey = GetPackageVersionsKey(packageName, false);
        var versionsEntry = await _cacheService.GetEntryInfoAsync(versionsKey, cancellationToken).ConfigureAwait(false);
        if (versionsEntry != null) {
            freshness.VersionsCachedAt = versionsEntry.CreatedAt;
            freshness.HasFreshVersions = !versionsEntry.IsExpired;
        }

        // Check security summary
        var securityKey = GetSecuritySummaryKey(packageName);
        var securityEntry = await _cacheService.GetEntryInfoAsync(securityKey, cancellationToken).ConfigureAwait(false);
        if (securityEntry != null) {
            freshness.SecuritySummaryCachedAt = securityEntry.CreatedAt;
            freshness.HasFreshSecuritySummary = !securityEntry.IsExpired;
        }

        // Check trust tier
        var trustTierKey = GetTrustTierKey(packageName);
        var trustTierEntry = await _cacheService.GetEntryInfoAsync(trustTierKey, cancellationToken).ConfigureAwait(false);
        if (trustTierEntry != null) {
            freshness.TrustTierCachedAt = trustTierEntry.CreatedAt;
            freshness.HasFreshTrustTier = !trustTierEntry.IsExpired;
        }

        return freshness;
    }

    public async Task<int> PreloadPackagesAsync(string? pattern = null, int maxPackages = 100, CancellationToken cancellationToken = default) {
        // This would typically require integration with the API client to fetch popular packages
        // For now, we'll return 0 as this is a placeholder implementation
        _logger.LogInformation("Package preloading not yet implemented (pattern: {Pattern}, max: {Max})", pattern, maxPackages);

        // TODO: Implement package preloading
        // 1. Get list of popular packages from API
        // 2. Filter by pattern if provided
        // 3. Preload package info, versions, security data for each
        // 4. Return count of successfully preloaded packages

        await Task.CompletedTask;
        return 0;
    }

    private static string GetPackageInfoKey(string packageName) => $"{PACKAGE_INFO_PREFIX}{packageName.ToLowerInvariant()}";

    private static string GetPackageVersionsKey(string packageName, bool includePrerelease) {
        var suffix = includePrerelease ? ":pre" : ":stable";
        return $"{PACKAGE_VERSIONS_PREFIX}{packageName.ToLowerInvariant()}{suffix}";
    }

    private static string GetSecuritySummaryKey(string packageName) => $"{SECURITY_SUMMARY_PREFIX}{packageName.ToLowerInvariant()}";

    private static string GetTrustTierKey(string packageName) => $"{TRUST_TIER_PREFIX}{packageName.ToLowerInvariant()}";

    private static string GetDependenciesKey(string packageName, string version) => $"{DEPENDENCIES_PREFIX}{packageName.ToLowerInvariant()}:{version.ToLowerInvariant()}";

    private static void ValidatePackageName(string packageName) {
        if (string.IsNullOrWhiteSpace(packageName)) {
            throw new ArgumentException("Package name cannot be null or empty", nameof(packageName));
        }

        if (packageName.Length > 100) {
            throw new ArgumentException("Package name too long (max 100 characters)", nameof(packageName));
        }
    }

    private static void ValidateVersion(string version) {
        if (string.IsNullOrWhiteSpace(version)) {
            throw new ArgumentException("Version cannot be null or empty", nameof(version));
        }

        if (version.Length > 50) {
            throw new ArgumentException("Version too long (max 50 characters)", nameof(version));
        }
    }
}