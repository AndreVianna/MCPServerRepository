using MCPHub.Domain.Entities;
using MCPHub.Domain.Repositories;

namespace MCPHub.Data.Repositories;

public class ServerRepository(McpHubContext context) : Repository<Server>(context), IServerRepository {
    public async Task<Server?> GetByNameAsync(string name, CancellationToken cancellationToken = default) => await _dbSet.FirstOrDefaultAsync(s => s.Name == name, cancellationToken);

    public async Task<List<Server>> GetByPublisherIdAsync(Guid publisherId, CancellationToken cancellationToken = default) => await _dbSet.Where(s => s.PublisherId == publisherId).ToListAsync(cancellationToken);

    public async Task<List<Server>> GetByStatusAsync(ServerStatus status, CancellationToken cancellationToken = default) => await _dbSet.Where(s => s.Status == status).ToListAsync(cancellationToken);

    public async Task<List<Server>> GetByTrustTierAsync(TrustTier trustTier, CancellationToken cancellationToken = default) => await _dbSet.Where(s => s.TrustTier == trustTier).ToListAsync(cancellationToken);

    public async Task<List<Server>> GetByTagsAsync(List<string> tags, CancellationToken cancellationToken = default) => await _dbSet
            .Where(s => tags.Any(tag => s.Tags.Contains(tag)))
            .ToListAsync(cancellationToken);

    public async Task<bool> IsNameAvailableAsync(string name, CancellationToken cancellationToken = default) => !await _dbSet.AnyAsync(s => s.Name == name, cancellationToken);

    public async Task<List<Server>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default) => await _dbSet
            .Where(s => s.Name.Contains(searchTerm) || s.Description.Contains(searchTerm))
            .ToListAsync(cancellationToken);
}