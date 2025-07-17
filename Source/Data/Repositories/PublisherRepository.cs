namespace Data.Repositories;

public class PublisherRepository(McpHubContext context) : Repository<Publisher>(context), IPublisherRepository {
    public async Task<Publisher?> GetByNameAsync(string name, CancellationToken cancellationToken = default) => await _dbSet.FirstOrDefaultAsync(p => p.Name == name, cancellationToken);

    public async Task<List<Publisher>> GetVerifiedPublishersAsync(CancellationToken cancellationToken = default) => await _dbSet.Where(p => p.Verified).ToListAsync(cancellationToken);

    public async Task<List<Publisher>> GetPublishersByTypeAsync(PublisherType type, CancellationToken cancellationToken = default) => await _dbSet.Where(p => p.Type == type).ToListAsync(cancellationToken);

    public async Task<bool> IsNameAvailableAsync(string name, CancellationToken cancellationToken = default) => !await _dbSet.AnyAsync(p => p.Name == name, cancellationToken);
}