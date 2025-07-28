using MCPHub.Domain.Contracts.Requests;
using MCPHub.Domain.Entities;

namespace MCPHub.Domain.Contracts.Services;

/// <summary>
/// Application service interface for package management operations
/// </summary>
public interface IPackageService {
    /// <summary>
    /// Gets all packages
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of all packages</returns>
    Task<IEnumerable<Package>> GetAllPackagesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a package by its identifier
    /// </summary>
    /// <param name="id">Package identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Package entity or null if not found</returns>
    Task<Package?> GetPackageByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a package by its name
    /// </summary>
    /// <param name="name">Package name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Package entity or null if not found</returns>
    Task<Package?> GetPackageByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets packages by publisher
    /// </summary>
    /// <param name="publisherId">Publisher identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of packages for the publisher</returns>
    Task<IEnumerable<Package>> GetPackagesByPublisherAsync(Guid publisherId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches packages by query
    /// </summary>
    /// <param name="query">Search query</param>
    /// <param name="pageSize">Number of results per page</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of matching packages</returns>
    Task<IEnumerable<Package>> SearchPackagesAsync(string query, int pageSize = 20, int pageIndex = 0, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches packages with advanced filtering, pagination, and sorting
    /// </summary>
    /// <param name="request">Search request with advanced parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Search result with paginated packages and metadata</returns>
    Task<SearchResult<Package>> SearchPackagesAsync(SearchRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new package
    /// </summary>
    /// <param name="package">Package to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created package</returns>
    Task<Package> CreatePackageAsync(Package package, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing package
    /// </summary>
    /// <param name="package">Package to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated package</returns>
    Task<Package> UpdatePackageAsync(Package package, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a package by its identifier
    /// </summary>
    /// <param name="id">Package identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if package was deleted, false if not found</returns>
    Task<bool> DeletePackageAsync(Guid id, CancellationToken cancellationToken = default);
}