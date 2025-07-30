using MCPHub.CommandLineApp.Configuration;
using MCPHub.CommandLineApp.Models;

namespace MCPHub.CommandLineApp.Services;

/// <summary>
/// API client decorator that adds caching and offline support to MCP Hub API operations
/// </summary>
public class CachedMcpHubApiClient(
    ILogger<CachedMcpHubApiClient> logger,
    IMcpHubApiClient innerClient,
    IPackageCacheService packageCache,
    ISearchCacheService searchCache,
    IOfflineModeService offlineMode,
    McpmConfiguration configuration) : IMcpHubApiClient {
    private readonly ILogger<CachedMcpHubApiClient> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IMcpHubApiClient _innerClient = innerClient ?? throw new ArgumentNullException(nameof(innerClient));
    private readonly IPackageCacheService _packageCache = packageCache ?? throw new ArgumentNullException(nameof(packageCache));
    private readonly ISearchCacheService _searchCache = searchCache ?? throw new ArgumentNullException(nameof(searchCache));
    private readonly IOfflineModeService _offlineMode = offlineMode ?? throw new ArgumentNullException(nameof(offlineMode));
    private readonly McpmConfiguration _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

    public async Task<SearchResultResponse> SearchPackagesAsync(SearchRequest request, CancellationToken cancellationToken = default) => await _offlineMode.ExecuteWithFallbackAsync(
            onlineOperation: async ct => {
                var result = await _innerClient.SearchPackagesAsync(request, ct).ConfigureAwait(false);

                // Cache the results
                await _searchCache.CacheSearchResultsAsync(request, result, ct).ConfigureAwait(false);

                return result;
            },
            offlineOperation: async ct => {
                var cachedResult = await _searchCache.GetCachedSearchResultsAsync(request, ct).ConfigureAwait(false);

                if (cachedResult != null) {
                    _logger.LogInformation("Returning cached search results for query: {Query}", request.Query);
                    return cachedResult;
                }

                // Try to find similar cached queries
                var similarQueries = await _searchCache.FindSimilarQueriesAsync(
                    request.Query,
                    similarityThreshold: 0.7,
                    maxResults: 1,
                    cancellationToken: ct).ConfigureAwait(false);

                var similarQuery = similarQueries.FirstOrDefault();
                if (similarQuery != null) {
                    _logger.LogInformation("Returning similar cached search results for query: {Query} (similar to: {SimilarQuery})",
                        request.Query, similarQuery.Query);

                    var similarRequest = similarQuery.Request;
                    return await _searchCache.GetCachedSearchResultsAsync(similarRequest, ct).ConfigureAwait(false)
                           ?? throw new InvalidOperationException("Similar cached search result not found");
                }

                return null;
            },
            cacheKey: _searchCache.GenerateSearchCacheKey(request),
            cancellationToken: cancellationToken
        ).ConfigureAwait(false) ?? throw new InvalidOperationException("No search results available offline");

    public async Task<PackageInfoResponse> GetPackageInfoAsync(string packageName, CancellationToken cancellationToken = default) => await _offlineMode.ExecuteWithFallbackAsync(
            onlineOperation: async ct => {
                var result = await _innerClient.GetPackageInfoAsync(packageName, ct).ConfigureAwait(false);

                // Cache the package info
                await _packageCache.CachePackageInfoAsync(packageName, result, ct).ConfigureAwait(false);

                return result;
            },
            offlineOperation: async ct => {
                var cachedResult = await _packageCache.GetCachedPackageInfoAsync(packageName, ct).ConfigureAwait(false);

                if (cachedResult != null) {
                    _logger.LogInformation("Returning cached package info for: {PackageName}", packageName);
                    return cachedResult;
                }

                return null;
            },
            cacheKey: $"package-info:{packageName}",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false) ?? throw new InvalidOperationException($"Package info for '{packageName}' not available offline");

    public async Task<PackageListResponse> GetPackagesAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default) {
        // Package lists are not typically cached due to their dynamic nature
        // But we could implement a simple cache with short expiration for recent requests

        var cacheKey = $"package-list:page:{page}:size:{pageSize}";

        return await _offlineMode.ExecuteWithFallbackAsync(
            onlineOperation: async ct => await _innerClient.GetPackagesAsync(page, pageSize, ct).ConfigureAwait(false),
            offlineOperation: ct => {
                // For package lists, we could return a list based on cached package info
                // This is a simplified fallback implementation
                _logger.LogWarning("Package list not available offline, limited functionality");
                return Task.FromResult<PackageListResponse?>(new PackageListResponse {
                    Packages = [],
                    Page = page,
                    PageSize = pageSize,
                    TotalCount = 0,
                    TotalPages = 0
                });
            },
            cacheKey: cacheKey,
            cancellationToken: cancellationToken
        ).ConfigureAwait(false) ?? new PackageListResponse {
            Packages = [],
            Page = page,
            PageSize = pageSize,
            TotalCount = 0,
            TotalPages = 0
        };
    }

    public async Task<PackageVersionsResponse> GetPackageVersionsAsync(string packageName, bool includePrerelease = false, CancellationToken cancellationToken = default) => await _offlineMode.ExecuteWithFallbackAsync(
            onlineOperation: async ct => {
                var result = await _innerClient.GetPackageVersionsAsync(packageName, includePrerelease, ct).ConfigureAwait(false);

                // Cache the package versions
                await _packageCache.CachePackageVersionsAsync(packageName, result, includePrerelease, ct).ConfigureAwait(false);

                return result;
            },
            offlineOperation: async ct => {
                var cachedResult = await _packageCache.GetCachedPackageVersionsAsync(packageName, includePrerelease, ct).ConfigureAwait(false);

                if (cachedResult != null) {
                    _logger.LogInformation("Returning cached package versions for: {PackageName} (prerelease: {IncludePrerelease})",
                        packageName, includePrerelease);
                    return cachedResult;
                }

                return null;
            },
            cacheKey: $"package-versions:{packageName}:{includePrerelease}",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false) ?? throw new InvalidOperationException($"Package versions for '{packageName}' not available offline");

    public async Task<SecuritySummaryResponse> GetPackageSecuritySummaryAsync(string packageName, CancellationToken cancellationToken = default) => await _offlineMode.ExecuteWithFallbackAsync(
            onlineOperation: async ct => {
                var result = await _innerClient.GetPackageSecuritySummaryAsync(packageName, ct).ConfigureAwait(false);

                // Cache the security summary
                await _packageCache.CacheSecuritySummaryAsync(packageName, result, ct).ConfigureAwait(false);

                return result;
            },
            offlineOperation: async ct => {
                var cachedResult = await _packageCache.GetCachedSecuritySummaryAsync(packageName, ct).ConfigureAwait(false);

                if (cachedResult != null) {
                    _logger.LogInformation("Returning cached security summary for: {PackageName}", packageName);
                    return cachedResult;
                }

                return null;
            },
            cacheKey: $"security-summary:{packageName}",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false) ?? throw new InvalidOperationException($"Security summary for '{packageName}' not available offline");

    public async Task<TrustTierResponse> GetPackageTrustTierAsync(string packageName, CancellationToken cancellationToken = default) => await _offlineMode.ExecuteWithFallbackAsync(
            onlineOperation: async ct => {
                var result = await _innerClient.GetPackageTrustTierAsync(packageName, ct).ConfigureAwait(false);

                // Cache the trust tier
                await _packageCache.CacheTrustTierAsync(packageName, result, ct).ConfigureAwait(false);

                return result;
            },
            offlineOperation: async ct => {
                var cachedResult = await _packageCache.GetCachedTrustTierAsync(packageName, ct).ConfigureAwait(false);

                if (cachedResult != null) {
                    _logger.LogInformation("Returning cached trust tier for: {PackageName}", packageName);
                    return cachedResult;
                }

                return null;
            },
            cacheKey: $"trust-tier:{packageName}",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false) ?? throw new InvalidOperationException($"Trust tier for '{packageName}' not available offline");

    public async Task<PackageDependenciesResponse> GetPackageDependenciesAsync(string packageName, string version, CancellationToken cancellationToken = default) => await _offlineMode.ExecuteWithFallbackAsync(
            onlineOperation: async ct => {
                var result = await _innerClient.GetPackageDependenciesAsync(packageName, version, ct).ConfigureAwait(false);

                // Cache the dependencies
                await _packageCache.CachePackageDependenciesAsync(packageName, version, result, ct).ConfigureAwait(false);

                return result;
            },
            offlineOperation: async ct => {
                var cachedResult = await _packageCache.GetCachedPackageDependenciesAsync(packageName, version, ct).ConfigureAwait(false);

                if (cachedResult != null) {
                    _logger.LogInformation("Returning cached dependencies for: {PackageName} v{Version}", packageName, version);
                    return cachedResult;
                }

                return null;
            },
            cacheKey: $"dependencies:{packageName}:{version}",
            cancellationToken: cancellationToken
        ).ConfigureAwait(false) ?? throw new InvalidOperationException($"Dependencies for '{packageName}' v{version} not available offline");

    public async Task<bool> TestConnectivityAsync(CancellationToken cancellationToken = default)
        // Connectivity testing should always go through the inner client
        => await _innerClient.TestConnectivityAsync(cancellationToken).ConfigureAwait(false);

    // Write operations are not cached and require online connectivity
    public async Task<ValidateManifestResponse> ValidateManifestAsync(ValidateManifestRequest request, CancellationToken cancellationToken = default) => _offlineMode.IsOfflineMode
            ? throw new InvalidOperationException("Manifest validation requires online connectivity")
            : await _innerClient.ValidateManifestAsync(request, cancellationToken).ConfigureAwait(false);

    public async Task<PublishPackageResponse> PublishPackageAsync(PublishPackageRequest request, CancellationToken cancellationToken = default) {
        if (_offlineMode.IsOfflineMode) {
            throw new InvalidOperationException("Package publishing requires online connectivity");
        }

        var result = await _innerClient.PublishPackageAsync(request, cancellationToken).ConfigureAwait(false);

        // Invalidate related cache entries after successful publish
        if (result.Success) {
            // TODO: Parse package name from manifest content
            var packageName = "unknown"; // Need to parse from request.ManifestContent
            await _packageCache.InvalidatePackageAsync(packageName, cancellationToken).ConfigureAwait(false);
            _logger.LogInformation("Invalidated cache for published package: {PackageName}", packageName);
        }

        return result;
    }

    public async Task<DownloadPackageResponse> DownloadPackageAsync(string packageName, string? version, DownloadPackageRequest downloadRequest, CancellationToken cancellationToken = default) => _offlineMode.IsOfflineMode
            ? throw new InvalidOperationException("Package download requires online connectivity")
            : await _innerClient.DownloadPackageAsync(packageName, version, downloadRequest, cancellationToken).ConfigureAwait(false);

    public async Task<InstallPackageResponse> InstallPackageAsync(string packageName, string version, InstallPackageRequest installRequest, CancellationToken cancellationToken = default) => _offlineMode.IsOfflineMode
            ? throw new InvalidOperationException("Package installation requires online connectivity")
            : await _innerClient.InstallPackageAsync(packageName, version, installRequest, cancellationToken).ConfigureAwait(false);
}