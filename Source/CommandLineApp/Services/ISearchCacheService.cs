namespace MCPHub.CommandLineApp.Services;

/// <summary>
/// Interface for search-specific caching operations
/// </summary>
public interface ISearchCacheService {
    /// <summary>
    /// Caches search results
    /// </summary>
    /// <param name="request">Search request that generated the results</param>
    /// <param name="results">Search results to cache</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task indicating completion</returns>
    Task CacheSearchResultsAsync(SearchRequest request, SearchResultResponse results, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves cached search results
    /// </summary>
    /// <param name="request">Search request to find cached results for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Cached search results or null if not found/expired</returns>
    Task<SearchResultResponse?> GetCachedSearchResultsAsync(SearchRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds similar cached search queries
    /// </summary>
    /// <param name="query">Search query to find similar cached queries for</param>
    /// <param name="similarityThreshold">Similarity threshold (0-1)</param>
    /// <param name="maxResults">Maximum number of similar queries to return</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of similar cached search queries</returns>
    Task<IEnumerable<CachedSearchQuery>> FindSimilarQueriesAsync(string query, double similarityThreshold = 0.8, int maxResults = 5, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets search cache statistics
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Search cache statistics</returns>
    Task<SearchCacheStatistics> GetSearchCacheStatisticsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Warms the search cache with popular queries
    /// </summary>
    /// <param name="popularQueries">List of popular queries to pre-cache</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Number of queries successfully cached</returns>
    Task<int> WarmSearchCacheAsync(IEnumerable<string> popularQueries, CancellationToken cancellationToken = default);

    /// <summary>
    /// Clears search cache entries older than specified time
    /// </summary>
    /// <param name="olderThan">Time threshold for clearing entries</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Number of entries cleared</returns>
    Task<int> ClearOldSearchCacheAsync(TimeSpan olderThan, CancellationToken cancellationToken = default);

    /// <summary>
    /// Normalizes a search request for consistent caching
    /// </summary>
    /// <param name="request">Search request to normalize</param>
    /// <returns>Normalized search request</returns>
    SearchRequest NormalizeSearchRequest(SearchRequest request);

    /// <summary>
    /// Generates a cache key for a search request
    /// </summary>
    /// <param name="request">Search request to generate key for</param>
    /// <returns>Cache key string</returns>
    string GenerateSearchCacheKey(SearchRequest request);

    /// <summary>
    /// Calculates similarity between two search queries
    /// </summary>
    /// <param name="query1">First query</param>
    /// <param name="query2">Second query</param>
    /// <returns>Similarity score (0-1)</returns>
    double CalculateQuerySimilarity(string query1, string query2);
}

/// <summary>
/// Information about a cached search query
/// </summary>
public class CachedSearchQuery {
    /// <summary>
    /// The original search query
    /// </summary>
    public string Query { get; set; } = string.Empty;

    /// <summary>
    /// Similarity score to the compared query
    /// </summary>
    public double SimilarityScore { get; set; }

    /// <summary>
    /// Number of results found for this query
    /// </summary>
    public int ResultCount { get; set; }

    /// <summary>
    /// When this query was cached
    /// </summary>
    public DateTimeOffset CachedAt { get; set; }

    /// <summary>
    /// Number of times this query has been accessed
    /// </summary>
    public int AccessCount { get; set; }

    /// <summary>
    /// The full search request parameters
    /// </summary>
    public SearchRequest Request { get; set; } = new();
}

/// <summary>
/// Statistics about search cache usage
/// </summary>
public class SearchCacheStatistics {
    /// <summary>
    /// Total number of cached search queries
    /// </summary>
    public int TotalCachedQueries { get; set; }

    /// <summary>
    /// Number of search cache hits
    /// </summary>
    public long SearchCacheHits { get; set; }

    /// <summary>
    /// Number of search cache misses
    /// </summary>
    public long SearchCacheMisses { get; set; }

    /// <summary>
    /// Search cache hit rate
    /// </summary>
    public double SearchCacheHitRate => SearchCacheHits + SearchCacheMisses > 0
        ? (double)SearchCacheHits / (SearchCacheHits + SearchCacheMisses) * 100
        : 0;

    /// <summary>
    /// Most popular search queries
    /// </summary>
    public List<PopularSearchQuery> PopularQueries { get; set; } = new();

    /// <summary>
    /// Average response time improvement due to caching (in milliseconds)
    /// </summary>
    public double AverageResponseTimeImprovement { get; set; }

    /// <summary>
    /// Total storage used by search cache
    /// </summary>
    public long TotalCacheSizeBytes { get; set; }

    /// <summary>
    /// Human-readable cache size
    /// </summary>
    public string TotalCacheSizeFormatted => FormatBytes(TotalCacheSizeBytes);

    private static string FormatBytes(long bytes) {
        string[] sizes = ["B", "KB", "MB", "GB", "TB"];
        var order = 0;
        double size = bytes;

        while (size >= 1024 && order < sizes.Length - 1) {
            order++;
            size /= 1024;
        }

        return $"{size:0.##} {sizes[order]}";
    }
}

/// <summary>
/// Information about a popular search query
/// </summary>
public class PopularSearchQuery {
    /// <summary>
    /// The search query
    /// </summary>
    public string Query { get; set; } = string.Empty;

    /// <summary>
    /// Number of times this query has been searched
    /// </summary>
    public int SearchCount { get; set; }

    /// <summary>
    /// When this query was first searched
    /// </summary>
    public DateTimeOffset FirstSearchedAt { get; set; }

    /// <summary>
    /// When this query was last searched
    /// </summary>
    public DateTimeOffset LastSearchedAt { get; set; }

    /// <summary>
    /// Average number of results returned
    /// </summary>
    public double AverageResultCount { get; set; }
}