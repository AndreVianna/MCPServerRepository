namespace Data.Repositories;

public class ServerRepository : Repository<Server>, IServerRepository
{
    public ServerRepository(McpHubContext context) : base(context)
    {
    }
    
    public async Task<Server?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(s => s.Name == name, cancellationToken);
    }
    
    public async Task<List<Server>> GetByPublisherIdAsync(Guid publisherId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(s => s.PublisherId == publisherId).ToListAsync(cancellationToken);
    }
    
    public async Task<List<Server>> GetPopularServersAsync(int count = 10, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .OrderByDescending(s => s.Versions.Count)
            .Take(count)
            .ToListAsync(cancellationToken);
    }
    
    public async Task<List<Server>> SearchServersAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.Name.Contains(searchTerm) || s.Description.Contains(searchTerm))
            .ToListAsync(cancellationToken);
    }
    
    public async Task<bool> IsNameAvailableAsync(string name, CancellationToken cancellationToken = default)
    {
        return !await _dbSet.AnyAsync(s => s.Name == name, cancellationToken);
    }
    
    public async Task<Server?> GetWithVersionsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.Versions)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }
}