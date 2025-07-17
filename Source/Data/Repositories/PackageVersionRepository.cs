namespace Data.Repositories;

public class PackageVersionRepository(McpHubContext context) : Repository<PackageVersion>(context), IPackageVersionRepository {
    public async Task<PackageVersion?> GetByPackageAndVersionAsync(Guid packageId, string version, CancellationToken cancellationToken = default) => await _dbSet
            .Include(pv => pv.Package)
            .ThenInclude(p => p.Publisher)
            .FirstOrDefaultAsync(pv => pv.PackageId == packageId && pv.Version == version, cancellationToken);

    public async Task<List<PackageVersion>> GetByPackageIdAsync(Guid packageId, CancellationToken cancellationToken = default) => await _dbSet
            .Include(pv => pv.Package)
            .Where(pv => pv.PackageId == packageId)
            .OrderByDescending(pv => pv.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task<PackageVersion?> GetLatestVersionAsync(Guid packageId, CancellationToken cancellationToken = default) => await _dbSet
            .Include(pv => pv.Package)
            .Where(pv => pv.PackageId == packageId && !pv.IsPrerelease)
            .OrderByDescending(pv => pv.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<List<PackageVersion>> GetPrereleasesAsync(Guid packageId, CancellationToken cancellationToken = default) => await _dbSet
            .Include(pv => pv.Package)
            .Where(pv => pv.PackageId == packageId && pv.IsPrerelease)
            .OrderByDescending(pv => pv.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task<List<PackageVersion>> GetStableVersionsAsync(Guid packageId, CancellationToken cancellationToken = default) => await _dbSet
            .Include(pv => pv.Package)
            .Where(pv => pv.PackageId == packageId && !pv.IsPrerelease)
            .OrderByDescending(pv => pv.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task<bool> VersionExistsAsync(Guid packageId, string version, CancellationToken cancellationToken = default) => await _dbSet
            .AnyAsync(pv => pv.PackageId == packageId && pv.Version == version, cancellationToken);
}