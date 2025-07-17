namespace Domain.Repositories;

public interface IUnitOfWork : IDisposable {
    IPublisherRepository Publishers { get; }
    IServerRepository Servers { get; }
    IServerVersionRepository ServerVersions { get; }
    IPackageRepository Packages { get; }
    IPackageVersionRepository PackageVersions { get; }
    ISecurityScanRepository SecurityScans { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}