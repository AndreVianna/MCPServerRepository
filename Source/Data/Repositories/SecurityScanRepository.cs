namespace Data.Repositories;

public class SecurityScanRepository(McpHubContext context) : Repository<SecurityScan>(context), ISecurityScanRepository {
    public async Task<List<SecurityScan>> GetByVersionIdAsync(Guid versionId, CancellationToken cancellationToken = default) => await _dbSet.Where(s => s.VersionId == versionId).ToListAsync(cancellationToken);

    public async Task<List<SecurityScan>> GetByScanTypeAsync(ScanType scanType, CancellationToken cancellationToken = default) => await _dbSet.Where(s => s.ScanType == scanType).ToListAsync(cancellationToken);

    public async Task<List<SecurityScan>> GetByStatusAsync(ScanStatus status, CancellationToken cancellationToken = default) => await _dbSet.Where(s => s.Status == status).ToListAsync(cancellationToken);

    public async Task<List<SecurityScan>> GetFailedScansAsync(CancellationToken cancellationToken = default) => await _dbSet.Where(s => s.Status == ScanStatus.Failed).ToListAsync(cancellationToken);

    public async Task<List<SecurityScan>> GetPendingScansAsync(CancellationToken cancellationToken = default) => await _dbSet.Where(s => s.Status == ScanStatus.Pending).ToListAsync(cancellationToken);

    public async Task<SecurityScan?> GetLatestScanAsync(Guid versionId, ScanType scanType, CancellationToken cancellationToken = default) => await _dbSet
            .Where(s => s.VersionId == versionId && s.ScanType == scanType)
            .OrderByDescending(s => s.StartedAt)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<List<SecurityScan>> GetScansWithCriticalIssuesAsync(CancellationToken cancellationToken = default) => await _dbSet.Where(s => s.CriticalIssues > 0).ToListAsync(cancellationToken);
}