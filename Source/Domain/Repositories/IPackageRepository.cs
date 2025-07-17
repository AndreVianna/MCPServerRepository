namespace Domain.Repositories;

public interface IPackageRepository : IRepository<Package>
{
    Task<Package?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<List<Package>> GetByPublisherIdAsync(Guid publisherId, CancellationToken cancellationToken = default);
    Task<List<Package>> GetByStatusAsync(PackageStatus status, CancellationToken cancellationToken = default);
    Task<List<Package>> GetByTrustTierAsync(TrustTier trustTier, CancellationToken cancellationToken = default);
    Task<List<Package>> SearchAsync(string query, int page = 0, int pageSize = 20, CancellationToken cancellationToken = default);
    Task<List<Package>> GetByTagsAsync(List<string> tags, CancellationToken cancellationToken = default);
    Task<List<Package>> GetRecentlyUpdatedAsync(int count = 10, CancellationToken cancellationToken = default);
    Task<List<Package>> GetMostDownloadedAsync(int count = 10, CancellationToken cancellationToken = default);
}