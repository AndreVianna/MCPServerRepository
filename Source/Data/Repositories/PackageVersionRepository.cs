using MCPHub.Domain.Entities;
using MCPHub.Domain.Repositories;

namespace MCPHub.Data.Repositories;

/// <summary>
/// Package version repository implementation following contracts-first approach
/// </summary>
public class PackageVersionRepository(McpHubContext context) : Repository<PackageVersion>(context), IPackageVersionRepository {
    public Task<PackageVersion?> GetByPackageAndVersionAsync(Guid packageId, string version, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("Package version lookup will be implemented when first consumer requires it");

    public Task<List<PackageVersion>> GetByPackageIdAsync(Guid packageId, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("Package versions query will be implemented when package service requires it");

    public Task<PackageVersion?> GetLatestVersionAsync(Guid packageId, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("Latest package version query will be implemented when package service requires it");

    public Task<List<PackageVersion>> GetPrereleasesAsync(Guid packageId, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("Prerelease versions query will be implemented when package service requires it");

    public Task<List<PackageVersion>> GetStableVersionsAsync(Guid packageId, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("Stable versions query will be implemented when package service requires it");

    public Task<bool> VersionExistsAsync(Guid packageId, string version, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("Version existence check will be implemented when package service requires it");
}