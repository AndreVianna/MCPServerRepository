using Microsoft.EntityFrameworkCore.Storage;

namespace Data.Repositories;

public class UnitOfWork : IUnitOfWork {
    private readonly McpHubContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(McpHubContext context) {
        _context = context;
        Publishers = new PublisherRepository(_context);
        Servers = new ServerRepository(_context);
        ServerVersions = new ServerVersionRepository(_context);
        Packages = new PackageRepository(_context);
        PackageVersions = new PackageVersionRepository(_context);
        SecurityScans = new SecurityScanRepository(_context);
    }

    public IPublisherRepository Publishers { get; }
    public IServerRepository Servers { get; }
    public IServerVersionRepository ServerVersions { get; }
    public IPackageRepository Packages { get; }
    public IPackageVersionRepository PackageVersions { get; }
    public ISecurityScanRepository SecurityScans { get; }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => await _context.SaveChangesAsync(cancellationToken);

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default) => _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default) {
        if (_transaction != null) {
            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default) {
        if (_transaction != null) {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose() {
        _transaction?.Dispose();
        _context.Dispose();
    }
}