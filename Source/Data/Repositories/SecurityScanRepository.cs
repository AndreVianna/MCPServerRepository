using MCPHub.Domain.Entities;
using MCPHub.Domain.Repositories;

namespace MCPHub.Data.Repositories;

/// <summary>
/// Security scan repository implementation following contracts-first approach
/// </summary>
public class SecurityScanRepository(McpHubContext context) : Repository<SecurityScan>(context), ISecurityScanRepository {
    public Task<List<SecurityScan>> GetByVersionIdAsync(Guid versionId, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("Security scans by version query will be implemented when security service requires it");

    public Task<List<SecurityScan>> GetByScanTypeAsync(ScanType scanType, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("Security scans by type query will be implemented when security service requires it");

    public Task<List<SecurityScan>> GetByStatusAsync(ScanStatus status, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("Security scans by status query will be implemented when security service requires it");

    public Task<List<SecurityScan>> GetFailedScansAsync(CancellationToken cancellationToken = default)
        => throw new NotImplementedException("Failed security scans query will be implemented when monitoring service requires it");

    public Task<List<SecurityScan>> GetPendingScansAsync(CancellationToken cancellationToken = default)
        => throw new NotImplementedException("Pending security scans query will be implemented when security service requires it");

    public Task<SecurityScan?> GetLatestScanAsync(Guid versionId, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("Latest security scan query will be implemented when security service requires it");

    public Task<List<SecurityScan>> GetScansWithCriticalIssuesAsync(CancellationToken cancellationToken = default)
        => throw new NotImplementedException("Critical security scans query will be implemented when monitoring service requires it");
}