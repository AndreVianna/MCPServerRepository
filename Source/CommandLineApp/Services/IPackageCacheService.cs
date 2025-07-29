using MCPHub.CommandLineApp.Models;

namespace MCPHub.CommandLineApp.Services;

/// <summary>
/// Interface for package-specific caching operations
/// </summary>
public interface IPackageCacheService {
    /// <summary>
    /// Caches package information
    /// </summary>
    /// <param name="packageName">Name of the package</param>
    /// <param name="packageInfo">Package information to cache</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task indicating completion</returns>
    Task CachePackageInfoAsync(string packageName, PackageInfoResponse packageInfo, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves cached package information
    /// </summary>
    /// <param name="packageName">Name of the package</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Cached package information or null if not found/expired</returns>
    Task<PackageInfoResponse?> GetCachedPackageInfoAsync(string packageName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Caches package versions
    /// </summary>
    /// <param name="packageName">Name of the package</param>
    /// <param name="versions">Package versions to cache</param>
    /// <param name="includePrerelease">Whether prerelease versions were included</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task indicating completion</returns>
    Task CachePackageVersionsAsync(string packageName, PackageVersionsResponse versions, bool includePrerelease, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves cached package versions
    /// </summary>
    /// <param name="packageName">Name of the package</param>
    /// <param name="includePrerelease">Whether to include prerelease versions</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Cached package versions or null if not found/expired</returns>
    Task<PackageVersionsResponse?> GetCachedPackageVersionsAsync(string packageName, bool includePrerelease, CancellationToken cancellationToken = default);

    /// <summary>
    /// Caches package security summary
    /// </summary>
    /// <param name="packageName">Name of the package</param>
    /// <param name="securitySummary">Security summary to cache</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task indicating completion</returns>
    Task CacheSecuritySummaryAsync(string packageName, SecuritySummaryResponse securitySummary, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves cached package security summary
    /// </summary>
    /// <param name="packageName">Name of the package</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Cached security summary or null if not found/expired</returns>
    Task<SecuritySummaryResponse?> GetCachedSecuritySummaryAsync(string packageName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Caches package trust tier information
    /// </summary>
    /// <param name="packageName">Name of the package</param>
    /// <param name="trustTier">Trust tier information to cache</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task indicating completion</returns>
    Task CacheTrustTierAsync(string packageName, TrustTierResponse trustTier, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves cached package trust tier information
    /// </summary>
    /// <param name="packageName">Name of the package</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Cached trust tier information or null if not found/expired</returns>
    Task<TrustTierResponse?> GetCachedTrustTierAsync(string packageName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Caches package dependencies
    /// </summary>
    /// <param name="packageName">Name of the package</param>
    /// <param name="version">Package version</param>
    /// <param name="dependencies">Dependencies to cache</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task indicating completion</returns>
    Task CachePackageDependenciesAsync(string packageName, string version, PackageDependenciesResponse dependencies, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves cached package dependencies
    /// </summary>
    /// <param name="packageName">Name of the package</param>
    /// <param name="version">Package version</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Cached dependencies or null if not found/expired</returns>
    Task<PackageDependenciesResponse?> GetCachedPackageDependenciesAsync(string packageName, string version, CancellationToken cancellationToken = default);

    /// <summary>
    /// Invalidates all cached data for a specific package
    /// </summary>
    /// <param name="packageName">Name of the package</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Number of cache entries removed</returns>
    Task<int> InvalidatePackageAsync(string packageName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets cache freshness information for a package
    /// </summary>
    /// <param name="packageName">Name of the package</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Cache freshness information</returns>
    Task<PackageCacheFreshness> GetPackageCacheFreshnessAsync(string packageName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Preloads cache with popular packages or packages matching a pattern
    /// </summary>
    /// <param name="pattern">Optional pattern to match packages</param>
    /// <param name="maxPackages">Maximum number of packages to preload</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Number of packages preloaded</returns>
    Task<int> PreloadPackagesAsync(string? pattern = null, int maxPackages = 100, CancellationToken cancellationToken = default);
}

/// <summary>
/// Information about package cache freshness
/// </summary>
public class PackageCacheFreshness {
    /// <summary>
    /// Package name
    /// </summary>
    public string PackageName { get; set; } = string.Empty;

    /// <summary>
    /// Whether package info is cached and fresh
    /// </summary>
    public bool HasFreshPackageInfo { get; set; }

    /// <summary>
    /// When package info was last cached
    /// </summary>
    public DateTimeOffset? PackageInfoCachedAt { get; set; }

    /// <summary>
    /// Whether package versions are cached and fresh
    /// </summary>
    public bool HasFreshVersions { get; set; }

    /// <summary>
    /// When package versions were last cached
    /// </summary>
    public DateTimeOffset? VersionsCachedAt { get; set; }

    /// <summary>
    /// Whether security summary is cached and fresh
    /// </summary>
    public bool HasFreshSecuritySummary { get; set; }

    /// <summary>
    /// When security summary was last cached
    /// </summary>
    public DateTimeOffset? SecuritySummaryCachedAt { get; set; }

    /// <summary>
    /// Whether trust tier is cached and fresh
    /// </summary>
    public bool HasFreshTrustTier { get; set; }

    /// <summary>
    /// When trust tier was last cached
    /// </summary>
    public DateTimeOffset? TrustTierCachedAt { get; set; }

    /// <summary>
    /// Overall cache freshness score (0-100)
    /// </summary>
    public int FreshnessScore {
        get {
            var fresh = 0;
            var total = 0;

            if (PackageInfoCachedAt.HasValue) {
                total++;
                if (HasFreshPackageInfo) fresh++;
            }

            if (VersionsCachedAt.HasValue) {
                total++;
                if (HasFreshVersions) fresh++;
            }

            if (SecuritySummaryCachedAt.HasValue) {
                total++;
                if (HasFreshSecuritySummary) fresh++;
            }

            if (TrustTierCachedAt.HasValue) {
                total++;
                if (HasFreshTrustTier) fresh++;
            }

            return total > 0 ? (int)((double)fresh / total * 100) : 0;
        }
    }

    /// <summary>
    /// Whether the package has any cached data
    /// </summary>
    public bool HasAnyCache => PackageInfoCachedAt.HasValue || VersionsCachedAt.HasValue || 
                               SecuritySummaryCachedAt.HasValue || TrustTierCachedAt.HasValue;
}