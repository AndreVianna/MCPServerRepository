namespace MCPHub.Domain.Contracts.Services;

/// <summary>
/// Search result container
/// </summary>
/// <typeparam name="T">Type of search results</typeparam>
public class SearchResult<T> {
    /// <summary>
    /// Search results
    /// </summary>
    public IEnumerable<T> Results { get; init; } = [];

    /// <summary>
    /// Total number of results available
    /// </summary>
    public int TotalCount { get; init; }

    /// <summary>
    /// Current page index
    /// </summary>
    public int PageIndex { get; init; }

    /// <summary>
    /// Number of results per page
    /// </summary>
    public int PageSize { get; init; }

    /// <summary>
    /// Search query that was executed
    /// </summary>
    public string Query { get; init; } = string.Empty;

    /// <summary>
    /// Time taken to execute the search in milliseconds
    /// </summary>
    public long SearchTimeMs { get; init; }
}