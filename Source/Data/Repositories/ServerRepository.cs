namespace Data.Repositories;

public class ServerRepository(McpHubContext context) : Repository<Server>(context), IServerRepository {
    public async Task<Server?> GetByNameAsync(string name, CancellationToken cancellationToken = default) => await _dbSet.FirstOrDefaultAsync(s => s.Name == name, cancellationToken);

    public async Task<List<Server>> GetByPublisherIdAsync(Guid publisherId, CancellationToken cancellationToken = default) => await _dbSet.Where(s => s.PublisherId == publisherId).ToListAsync(cancellationToken);

    public async Task<List<Server>> GetPopularServersAsync(int count = 10, CancellationToken cancellationToken = default) => await _dbSet
            .OrderByDescending(s => s.Versions.Count)
            .Take(count)
            .ToListAsync(cancellationToken);

    public async Task<List<Server>> SearchServersAsync(string searchTerm, CancellationToken cancellationToken = default) => await _dbSet
            .Where(s => s.Name.Contains(searchTerm) || s.Description.Contains(searchTerm))
            .ToListAsync(cancellationToken);

    public async Task<bool> IsNameAvailableAsync(string name, CancellationToken cancellationToken = default) => !await _dbSet.AnyAsync(s => s.Name == name, cancellationToken);

    public async Task<Server?> GetWithVersionsAsync(Guid id, CancellationToken cancellationToken = default) => await _dbSet
            .Include(s => s.Versions)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
}