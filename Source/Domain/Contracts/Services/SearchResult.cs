namespace MCPHub.Domain.Contracts.Services;

/// <summary>
/// Search result container
/// </summary>
/// <typeparam name="T">Type of search results</typeparam>
public class SearchResult<T> {
    /// <summary>
    /// Search result items
    /// </summary>
    public IEnumerable<T> Items { get; init; } = [];

    /// <summary>
    /// Search results (alias for Items for backward compatibility)
    /// </summary>
    public IEnumerable<T> Results => Items;

    /// <summary>
    /// Total number of results available across all pages
    /// </summary>
    public int TotalCount { get; init; }

    /// <summary>
    /// Current page number (1-based)
    /// </summary>
    public int Page { get; init; }

    /// <summary>
    /// Current page index (0-based, for backward compatibility)
    /// </summary>
    public int PageIndex => Page - 1;

    /// <summary>
    /// Number of results per page
    /// </summary>
    public int PageSize { get; init; }

    /// <summary>
    /// Total number of pages available
    /// </summary>
    public int TotalPages => TotalCount > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;

    /// <summary>
    /// Search query that was executed
    /// </summary>
    public string Query { get; init; } = string.Empty;

    /// <summary>
    /// Time taken to execute the search in milliseconds
    /// </summary>
    public long SearchTimeMs { get; init; }
}