using System.Diagnostics;

using MCPHub.Domain.Contracts.Requests;
using MCPHub.Domain.Contracts.Services;
using MCPHub.Domain.Entities;
using MCPHub.Domain.Repositories;

using Microsoft.EntityFrameworkCore;

namespace MCPHub.Data.Repositories;

/// <summary>
/// Package repository implementation following contracts-first approach
/// </summary>
public class PackageRepository(McpHubContext context) : Repository<Package>(context), IPackageRepository {
    public async Task<Package?> GetByNameAsync(string name, CancellationToken cancellationToken = default) => await _context.Set<Package>()
            .Include(p => p.Publisher)
            .FirstOrDefaultAsync(p => p.Name == name, cancellationToken);

    public async Task<List<Package>> GetByPublisherIdAsync(Guid publisherId, CancellationToken cancellationToken = default) => await _context.Set<Package>()
            .Include(p => p.Publisher)
            .Where(p => p.PublisherId == publisherId)
            .ToListAsync(cancellationToken);

    public async Task<List<Package>> GetByStatusAsync(PackageStatus status, CancellationToken cancellationToken = default) => await _context.Set<Package>()
            .Include(p => p.Publisher)
            .Where(p => p.Status == status)
            .ToListAsync(cancellationToken);

    public async Task<List<Package>> GetByTrustTierAsync(TrustTier trustTier, CancellationToken cancellationToken = default) => await _context.Set<Package>()
            .Include(p => p.Publisher)
            .Where(p => p.TrustTier == trustTier)
            .ToListAsync(cancellationToken);

    public async Task<List<Package>> SearchAsync(string query, int page = 0, int pageSize = 20, CancellationToken cancellationToken = default) {
        var packages = _context.Set<Package>()
            .Include(p => p.Publisher)
            .Where(p => EF.Functions.ToTsVector("english", p.Name + " " + p.Description + " " + string.Join(" ", p.Tags))
                       .Matches(EF.Functions.ToTsQuery("english", query)));

        return await packages
            .Skip(page * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<SearchResult<Package>> SearchAsync(SearchRequest request, CancellationToken cancellationToken = default) {
        var stopwatch = Stopwatch.StartNew();

        // Start with base query including publisher data
        var query = _context.Set<Package>()
            .Include(p => p.Publisher)
            .AsQueryable();

        // Apply text search using PostgreSQL full-text search
        if (!string.IsNullOrWhiteSpace(request.Query)) {
            var searchTerm = request.Query.Trim();
            query = query.Where(p =>
                EF.Functions.ToTsVector("english", p.Name + " " + p.Description + " " + string.Join(" ", p.Tags))
                    .Matches(EF.Functions.ToTsQuery("english", searchTerm)));
        }

        // Apply category filtering
        if (request.Categories != null && request.Categories.Any()) {
            query = query.Where(p => p.Tags.Any(tag => request.Categories.Contains(tag)));
        }

        // Apply trust tier filtering
        if (request.MinimumTrustTier.HasValue) {
            query = query.Where(p => p.TrustTier >= request.MinimumTrustTier.Value);
        }

        // Apply sorting
        query = ApplySorting(query, request.SortBy, request.SortDirection);

        // Get total count before pagination
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination
        var packages = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        stopwatch.Stop();

        return new SearchResult<Package> {
            Items = packages,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize,
            Query = request.Query,
            SearchTimeMs = stopwatch.ElapsedMilliseconds
        };
    }

    private static IQueryable<Package> ApplySorting(IQueryable<Package> query, string? sortBy, SortDirection sortDirection) {
        if (string.IsNullOrWhiteSpace(sortBy)) {
            // Default sorting by name ascending
            return sortDirection == SortDirection.Ascending
                ? query.OrderBy(p => p.Name)
                : query.OrderByDescending(p => p.Name);
        }

        return sortBy.ToLowerInvariant() switch {
            "name" => sortDirection == SortDirection.Ascending
                ? query.OrderBy(p => p.Name)
                : query.OrderByDescending(p => p.Name),
            "created" => sortDirection == SortDirection.Ascending
                ? query.OrderBy(p => p.AuditTrail.Where(a => a.Action.Contains("Created")).Min(a => a.DateTime))
                : query.OrderByDescending(p => p.AuditTrail.Where(a => a.Action.Contains("Created")).Min(a => a.DateTime)),
            "rating" => sortDirection == SortDirection.Ascending
                ? query.OrderBy(p => p.TrustTier)
                : query.OrderByDescending(p => p.TrustTier),
            "downloads" => sortDirection == SortDirection.Ascending
                ? query.OrderBy(p => p.Name) // Placeholder until download count is implemented
                : query.OrderByDescending(p => p.Name),
            _ => sortDirection == SortDirection.Ascending
                ? query.OrderBy(p => p.Name)
                : query.OrderByDescending(p => p.Name)
        };
    }

    public Task<List<Package>> GetByTagsAsync(List<string> tags, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("Package tag filtering will be implemented when first consumer requires it");

    public Task<List<Package>> GetRecentlyUpdatedAsync(int count = 10, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("Recently updated packages query will be implemented when dashboard requires it");

    public Task<List<Package>> GetMostDownloadedAsync(int count = 10, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("Most downloaded packages query will be implemented when analytics service requires it");
}