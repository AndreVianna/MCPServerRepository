using MCPHub.Domain.Entities;
using MCPHub.Domain.Repositories;

namespace MCPHub.Data.Repositories;

/// <summary>
/// Package repository implementation following contracts-first approach
/// </summary>
public class PackageRepository(McpHubContext context) : Repository<Package>(context), IPackageRepository {
    public Task<Package?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("Package name lookup will be implemented when first consumer requires it");

    public Task<List<Package>> GetByPublisherIdAsync(Guid publisherId, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("Package publisher filtering will be implemented when first consumer requires it");

    public Task<List<Package>> GetByStatusAsync(PackageStatus status, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("Package status filtering will be implemented when first consumer requires it");

    public Task<List<Package>> GetByTrustTierAsync(TrustTier trustTier, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("Package trust tier filtering will be implemented when first consumer requires it");

    public Task<List<Package>> SearchAsync(string query, int page = 0, int pageSize = 20, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("Package search functionality will be implemented when search service requires it");

    public Task<List<Package>> GetByTagsAsync(List<string> tags, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("Package tag filtering will be implemented when first consumer requires it");

    public Task<List<Package>> GetRecentlyUpdatedAsync(int count = 10, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("Recently updated packages query will be implemented when dashboard requires it");

    public Task<List<Package>> GetMostDownloadedAsync(int count = 10, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("Most downloaded packages query will be implemented when analytics service requires it");
}