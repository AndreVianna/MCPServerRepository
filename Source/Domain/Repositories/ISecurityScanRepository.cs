using Domain.Entities;

namespace Domain.Repositories;

public interface ISecurityScanRepository : IRepository<SecurityScan>
{
    Task<List<SecurityScan>> GetByVersionIdAsync(Guid versionId, CancellationToken cancellationToken = default);
    
    Task<List<SecurityScan>> GetByStatusAsync(ScanStatus status, CancellationToken cancellationToken = default);
    
    Task<List<SecurityScan>> GetFailedScansAsync(CancellationToken cancellationToken = default);
    
    Task<List<SecurityScan>> GetPendingScansAsync(CancellationToken cancellationToken = default);
    
    Task<SecurityScan?> GetLatestScanAsync(Guid versionId, CancellationToken cancellationToken = default);
    
    Task<List<SecurityScan>> GetScansWithCriticalIssuesAsync(CancellationToken cancellationToken = default);
}