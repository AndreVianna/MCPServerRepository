using MCPHub.Domain.Entities;
using MCPHub.Domain.Repositories;

namespace MCPHub.Data.Repositories;

public class ServerVersionRepository(McpHubContext context) : Repository<ServerVersion>(context), IServerVersionRepository {
    public async Task<List<ServerVersion>> GetByServerIdAsync(Guid serverId, CancellationToken cancellationToken = default) => await _dbSet.Where(v => v.ServerId == serverId).ToListAsync(cancellationToken);

    public async Task<ServerVersion?> GetByVersionAsync(Guid serverId, string version, CancellationToken cancellationToken = default) => await _dbSet.FirstOrDefaultAsync(v => v.ServerId == serverId && v.Version == version, cancellationToken);

    public async Task<List<ServerVersion>> GetByStatusAsync(VersionStatus status, CancellationToken cancellationToken = default) => await _dbSet.Where(v => v.Status == status).ToListAsync(cancellationToken);

    public async Task<ServerVersion?> GetLatestVersionAsync(Guid serverId, CancellationToken cancellationToken = default) => await _dbSet
            .Where(v => v.ServerId == serverId)
            .OrderByDescending(v => v.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<List<ServerVersion>> GetVersionsWithSecurityIssuesAsync(CancellationToken cancellationToken = default) => await _dbSet
            .Where(v => v.SecurityScans.Any(s => s.Status == ScanStatus.Failed || s.CriticalIssues > 0))
            .ToListAsync(cancellationToken);

    public async Task<List<ServerVersion>> GetPendingVersionsAsync(CancellationToken cancellationToken = default) => await _dbSet.Where(v => v.Status == VersionStatus.Pending).ToListAsync(cancellationToken);
}