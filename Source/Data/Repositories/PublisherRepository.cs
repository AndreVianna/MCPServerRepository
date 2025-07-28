using MCPHub.Domain.Entities;
using MCPHub.Domain.Repositories;

namespace MCPHub.Data.Repositories;

/// <summary>
/// Publisher repository implementation following contracts-first approach
/// </summary>
public class PublisherRepository(McpHubContext context) : Repository<Publisher>(context), IPublisherRepository {
    public Task<Publisher?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("Publisher name lookup will be implemented when first consumer requires it");

    public Task<List<Publisher>> GetVerifiedPublishersAsync(CancellationToken cancellationToken = default)
        => throw new NotImplementedException("Verified publishers query will be implemented when verification service requires it");

    public Task<List<Publisher>> GetPublishersByTypeAsync(PublisherType type, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("Publisher type filtering will be implemented when first consumer requires it");

    public Task<bool> IsNameAvailableAsync(string name, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("Publisher name availability check will be implemented when registration service requires it");

    public Task<Publisher?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("Publisher email lookup will be implemented when authentication service requires it");
}