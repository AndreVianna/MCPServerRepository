namespace Data.Repositories;

public class PackageRepository : Repository<Package>, IPackageRepository
{
    public PackageRepository(McpHubContext context) : base(context)
    {
    }

    public async Task<Package?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Publisher)
            .Include(p => p.Versions)
            .FirstOrDefaultAsync(p => p.Name == name, cancellationToken);
    }

    public async Task<List<Package>> GetByPublisherIdAsync(Guid publisherId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Publisher)
            .Where(p => p.PublisherId == publisherId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Package>> GetByStatusAsync(PackageStatus status, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Publisher)
            .Where(p => p.Status == status)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Package>> GetByTrustTierAsync(TrustTier trustTier, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Publisher)
            .Where(p => p.TrustTier == trustTier)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Package>> SearchAsync(string query, int page = 0, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var searchQuery = _dbSet
            .Include(p => p.Publisher)
            .Where(p => 
                EF.Functions.ILike(p.Name, $"%{query}%") ||
                EF.Functions.ILike(p.Description, $"%{query}%") ||
                p.Tags.Any(t => EF.Functions.ILike(t, $"%{query}%")))
            .OrderByDescending(p => p.CreatedAt);

        return await searchQuery
            .Skip(page * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Package>> GetByTagsAsync(List<string> tags, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Publisher)
            .Where(p => tags.Any(tag => p.Tags.Contains(tag)))
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Package>> GetRecentlyUpdatedAsync(int count = 10, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Publisher)
            .OrderByDescending(p => p.UpdatedAt)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Package>> GetMostDownloadedAsync(int count = 10, CancellationToken cancellationToken = default)
    {
        // For now, we'll order by creation date. In the future, we can add download counts
        return await _dbSet
            .Include(p => p.Publisher)
            .OrderByDescending(p => p.CreatedAt)
            .Take(count)
            .ToListAsync(cancellationToken);
    }
}