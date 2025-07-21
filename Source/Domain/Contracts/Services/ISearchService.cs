using MCPHub.Domain.Entities;

namespace MCPHub.Domain.Contracts.Services;

/// <summary>
/// Application service interface for search operations
/// </summary>
public interface ISearchService {
    /// <summary>
    /// Performs a full-text search across packages
    /// </summary>
    /// <param name="query">Search query</param>
    /// <param name="pageSize">Number of results per page</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Search results</returns>
    Task<SearchResult<Package>> SearchPackagesAsync(string query, int pageSize = 20, int pageIndex = 0, CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs a semantic search using vector embeddings
    /// </summary>
    /// <param name="query">Search query</param>
    /// <param name="pageSize">Number of results per page</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Semantic search results</returns>
    Task<SearchResult<Package>> SemanticSearchAsync(string query, int pageSize = 20, int pageIndex = 0, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets search suggestions for a partial query
    /// </summary>
    /// <param name="partialQuery">Partial search query</param>
    /// <param name="maxSuggestions">Maximum number of suggestions</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of search suggestions</returns>
    Task<IEnumerable<string>> GetSearchSuggestionsAsync(string partialQuery, int maxSuggestions = 10, CancellationToken cancellationToken = default);

    /// <summary>
    /// Indexes a package for search
    /// </summary>
    /// <param name="packageId">Package identifier to index</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if indexing was successful</returns>
    Task<bool> IndexPackageAsync(string packageId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a package from the search index
    /// </summary>
    /// <param name="packageId">Package identifier to remove</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if removal was successful</returns>
    Task<bool> RemovePackageFromIndexAsync(string packageId, CancellationToken cancellationToken = default);
}

