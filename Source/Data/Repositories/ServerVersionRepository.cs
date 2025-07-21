using MCPHub.Domain.Entities;
using MCPHub.Domain.Repositories;

namespace MCPHub.Data.Repositories;

/// <summary>
/// Server version repository implementation following contracts-first approach
/// </summary>
public class ServerVersionRepository(McpHubContext context) : Repository<ServerVersion>(context), IServerVersionRepository {
    public Task<List<ServerVersion>> GetByServerIdAsync(Guid serverId, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("Server versions query will be implemented when server service requires it");

    public Task<ServerVersion?> GetByVersionAsync(Guid serverId, string version, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("Server version lookup will be implemented when first consumer requires it");

    public Task<List<ServerVersion>> GetByStatusAsync(VersionStatus status, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("Server version status filtering will be implemented when first consumer requires it");

    public Task<ServerVersion?> GetLatestVersionAsync(Guid serverId, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("Latest server version query will be implemented when server service requires it");

    public Task<List<ServerVersion>> GetVersionsWithSecurityIssuesAsync(CancellationToken cancellationToken = default)
        => throw new NotImplementedException("Security issues versions query will be implemented when security service requires it");

    public Task<List<ServerVersion>> GetPendingVersionsAsync(CancellationToken cancellationToken = default)
        => throw new NotImplementedException("Pending versions query will be implemented when server service requires it");
}