namespace Data.Repositories;

public class ServerVersionRepository(McpHubContext context) : Repository<ServerVersion>(context), IServerVersionRepository {
    public async Task<ServerVersion?> GetByServerIdAndVersionAsync(Guid serverId, string version, CancellationToken cancellationToken = default) => await _dbSet.FirstOrDefaultAsync(v => v.ServerId == serverId && v.Version == version, cancellationToken);

    public async Task<List<ServerVersion>> GetByServerIdAsync(Guid serverId, CancellationToken cancellationToken = default) => await _dbSet.Where(v => v.ServerId == serverId).ToListAsync(cancellationToken);

    public async Task<ServerVersion?> GetLatestVersionAsync(Guid serverId, CancellationToken cancellationToken = default) => await _dbSet
            .Where(v => v.ServerId == serverId)
            .OrderByDescending(v => v.PublishedAt)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<List<ServerVersion>> GetVersionsByTrustTierAsync(TrustTier trustTier, CancellationToken cancellationToken = default) => await _dbSet.Where(v => v.TrustTier == trustTier).ToListAsync(cancellationToken);

    public async Task<List<ServerVersion>> GetVersionsRequiringScansAsync(CancellationToken cancellationToken = default) => await _dbSet
            .Where(v => !v.SecurityScans.Any(s => s.Status == ScanStatus.Completed))
            .ToListAsync(cancellationToken);

    public async Task<ServerVersion?> GetWithSecurityScansAsync(Guid id, CancellationToken cancellationToken = default) => await _dbSet
            .Include(v => v.SecurityScans)
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
}