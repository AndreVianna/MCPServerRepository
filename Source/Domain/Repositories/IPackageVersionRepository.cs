namespace Domain.Repositories;

public interface IPackageVersionRepository : IRepository<PackageVersion>
{
    Task<PackageVersion?> GetByPackageAndVersionAsync(Guid packageId, string version, CancellationToken cancellationToken = default);
    Task<List<PackageVersion>> GetByPackageIdAsync(Guid packageId, CancellationToken cancellationToken = default);
    Task<PackageVersion?> GetLatestVersionAsync(Guid packageId, CancellationToken cancellationToken = default);
    Task<List<PackageVersion>> GetPrereleasesAsync(Guid packageId, CancellationToken cancellationToken = default);
    Task<List<PackageVersion>> GetStableVersionsAsync(Guid packageId, CancellationToken cancellationToken = default);
    Task<bool> VersionExistsAsync(Guid packageId, string version, CancellationToken cancellationToken = default);
}