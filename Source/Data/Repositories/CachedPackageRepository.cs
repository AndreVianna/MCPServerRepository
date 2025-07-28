using MCPHub.Common.Services;
using MCPHub.Domain.Contracts.Requests;
using MCPHub.Domain.Contracts.Services;
using MCPHub.Domain.Entities;
using MCPHub.Domain.Repositories;

namespace MCPHub.Data.Repositories;

public class CachedPackageRepository(IPackageRepository packageRepository, ICacheService cacheService) : CachedRepository<Package>(packageRepository, cacheService, "package"), IPackageRepository {
    private readonly IPackageRepository _packageRepository = packageRepository;

    public async Task<Package?> GetByNameAsync(string name, CancellationToken cancellationToken = default) {
        var cacheKey = $"{_cacheKeyPrefix}:name:{name}";
        var cachedPackage = await _cacheService.GetAsync<Package>(cacheKey, cancellationToken);

        if (cachedPackage != null)
            return cachedPackage;

        var package = await _packageRepository.GetByNameAsync(name, cancellationToken);
        if (package != null) {
            await _cacheService.SetAsync(cacheKey, package, TimeSpan.FromMinutes(30), cancellationToken);
        }

        return package;
    }

    public async Task<List<Package>> GetByPublisherIdAsync(Guid publisherId, CancellationToken cancellationToken = default) {
        var cacheKey = $"{_cacheKeyPrefix}:publisher:{publisherId}";
        var cachedPackages = await _cacheService.GetAsync<List<Package>>(cacheKey, cancellationToken);

        if (cachedPackages != null)
            return cachedPackages;

        var packages = await _packageRepository.GetByPublisherIdAsync(publisherId, cancellationToken);
        await _cacheService.SetAsync(cacheKey, packages, TimeSpan.FromMinutes(20), cancellationToken);

        return packages;
    }

    public async Task<List<Package>> GetByStatusAsync(PackageStatus status, CancellationToken cancellationToken = default) {
        var cacheKey = $"{_cacheKeyPrefix}:status:{status}";
        var cachedPackages = await _cacheService.GetAsync<List<Package>>(cacheKey, cancellationToken);

        if (cachedPackages != null)
            return cachedPackages;

        var packages = await _packageRepository.GetByStatusAsync(status, cancellationToken);
        await _cacheService.SetAsync(cacheKey, packages, TimeSpan.FromMinutes(15), cancellationToken);

        return packages;
    }

    public async Task<List<Package>> GetByTrustTierAsync(TrustTier trustTier, CancellationToken cancellationToken = default) {
        var cacheKey = $"{_cacheKeyPrefix}:trusttier:{trustTier}";
        var cachedPackages = await _cacheService.GetAsync<List<Package>>(cacheKey, cancellationToken);

        if (cachedPackages != null)
            return cachedPackages;

        var packages = await _packageRepository.GetByTrustTierAsync(trustTier, cancellationToken);
        await _cacheService.SetAsync(cacheKey, packages, TimeSpan.FromMinutes(15), cancellationToken);

        return packages;
    }

    public async Task<List<Package>> SearchAsync(string query, int page = 0, int pageSize = 20, CancellationToken cancellationToken = default) {
        var cacheKey = $"{_cacheKeyPrefix}:search:{query}:{page}:{pageSize}";
        var cachedPackages = await _cacheService.GetAsync<List<Package>>(cacheKey, cancellationToken);

        if (cachedPackages != null)
            return cachedPackages;

        var packages = await _packageRepository.SearchAsync(query, page, pageSize, cancellationToken);
        await _cacheService.SetAsync(cacheKey, packages, TimeSpan.FromMinutes(10), cancellationToken);

        return packages;
    }

    public async Task<SearchResult<Package>> SearchAsync(SearchRequest request, CancellationToken cancellationToken = default) {
        // Generate cache key based on search parameters
        var categoriesKey = request.Categories != null ? string.Join(",", request.Categories.OrderBy(c => c)) : "none";
        var trustTierKey = request.MinimumTrustTier?.ToString() ?? "none";
        var sortKey = $"{request.SortBy ?? "none"}:{request.SortDirection}";
        var cacheKey = $"{_cacheKeyPrefix}:search_advanced:{request.Query}:{categoriesKey}:{trustTierKey}:{request.Page}:{request.PageSize}:{sortKey}";
        
        var cachedResult = await _cacheService.GetAsync<SearchResult<Package>>(cacheKey, cancellationToken);

        if (cachedResult != null)
            return cachedResult;

        var searchResult = await _packageRepository.SearchAsync(request, cancellationToken);
        
        // Cache search results for 5 minutes (shorter than simple search due to complexity)
        await _cacheService.SetAsync(cacheKey, searchResult, TimeSpan.FromMinutes(5), cancellationToken);

        return searchResult;
    }

    public async Task<List<Package>> GetByTagsAsync(List<string> tags, CancellationToken cancellationToken = default) {
        var tagsKey = string.Join(",", tags.OrderBy(t => t));
        var cacheKey = $"{_cacheKeyPrefix}:tags:{tagsKey}";
        var cachedPackages = await _cacheService.GetAsync<List<Package>>(cacheKey, cancellationToken);

        if (cachedPackages != null)
            return cachedPackages;

        var packages = await _packageRepository.GetByTagsAsync(tags, cancellationToken);
        await _cacheService.SetAsync(cacheKey, packages, TimeSpan.FromMinutes(20), cancellationToken);

        return packages;
    }

    public async Task<List<Package>> GetRecentlyUpdatedAsync(int count = 10, CancellationToken cancellationToken = default) {
        var cacheKey = $"{_cacheKeyPrefix}:recent:{count}";
        var cachedPackages = await _cacheService.GetAsync<List<Package>>(cacheKey, cancellationToken);

        if (cachedPackages != null)
            return cachedPackages;

        var packages = await _packageRepository.GetRecentlyUpdatedAsync(count, cancellationToken);
        await _cacheService.SetAsync(cacheKey, packages, TimeSpan.FromMinutes(5), cancellationToken);

        return packages;
    }

    public async Task<List<Package>> GetMostDownloadedAsync(int count = 10, CancellationToken cancellationToken = default) {
        var cacheKey = $"{_cacheKeyPrefix}:popular:{count}";
        var cachedPackages = await _cacheService.GetAsync<List<Package>>(cacheKey, cancellationToken);

        if (cachedPackages != null)
            return cachedPackages;

        var packages = await _packageRepository.GetMostDownloadedAsync(count, cancellationToken);
        await _cacheService.SetAsync(cacheKey, packages, TimeSpan.FromMinutes(10), cancellationToken);

        return packages;
    }

    protected override async Task InvalidateCacheAsync(CancellationToken cancellationToken = default) {
        await base.InvalidateCacheAsync(cancellationToken);

        // Also invalidate related caches
        await _cacheService.RemovePatternAsync("packageversion:*", cancellationToken);
        await _cacheService.RemovePatternAsync("publisher:*", cancellationToken);
    }
}