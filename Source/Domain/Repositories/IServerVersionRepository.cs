namespace Domain.Repositories;

public interface IServerVersionRepository : IRepository<ServerVersion>
{
    Task<List<ServerVersion>> GetByServerIdAsync(Guid serverId, CancellationToken cancellationToken = default);
    
    Task<ServerVersion?> GetByVersionAsync(Guid serverId, string version, CancellationToken cancellationToken = default);
    
    Task<List<ServerVersion>> GetByStatusAsync(VersionStatus status, CancellationToken cancellationToken = default);
    
    Task<ServerVersion?> GetLatestVersionAsync(Guid serverId, CancellationToken cancellationToken = default);
    
    Task<List<ServerVersion>> GetVersionsWithSecurityIssuesAsync(CancellationToken cancellationToken = default);
    
    Task<List<ServerVersion>> GetPendingVersionsAsync(CancellationToken cancellationToken = default);
}