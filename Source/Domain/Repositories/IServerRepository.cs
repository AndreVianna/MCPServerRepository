namespace Domain.Repositories;

public interface IServerRepository : IRepository<Server>
{
    Task<Server?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    
    Task<List<Server>> GetByPublisherIdAsync(Guid publisherId, CancellationToken cancellationToken = default);
    
    Task<List<Server>> GetByStatusAsync(ServerStatus status, CancellationToken cancellationToken = default);
    
    Task<List<Server>> GetByTrustTierAsync(TrustTier trustTier, CancellationToken cancellationToken = default);
    
    Task<List<Server>> GetByTagsAsync(List<string> tags, CancellationToken cancellationToken = default);
    
    Task<bool> IsNameAvailableAsync(string name, CancellationToken cancellationToken = default);
    
    Task<List<Server>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
}