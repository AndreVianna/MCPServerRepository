using MCPHub.Domain.Entities;
using MCPHub.Domain.Repositories;

namespace MCPHub.Data.Repositories;

/// <summary>
/// Server repository implementation following contracts-first approach
/// </summary>
public class ServerRepository(McpHubContext context) : Repository<Server>(context), IServerRepository {
    public Task<Server?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("Server name lookup will be implemented when first consumer requires it");

    public Task<List<Server>> GetByPublisherIdAsync(Guid publisherId, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("Server publisher filtering will be implemented when first consumer requires it");

    public Task<List<Server>> GetByStatusAsync(ServerStatus status, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("Server status filtering will be implemented when first consumer requires it");

    public Task<List<Server>> GetByTrustTierAsync(TrustTier trustTier, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("Server trust tier filtering will be implemented when first consumer requires it");

    public Task<List<Server>> GetByTagsAsync(List<string> tags, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("Server tag filtering will be implemented when first consumer requires it");

    public Task<bool> IsNameAvailableAsync(string name, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("Server name availability check will be implemented when registration service requires it");

    public Task<List<Server>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("Server search functionality will be implemented when search service requires it");
}