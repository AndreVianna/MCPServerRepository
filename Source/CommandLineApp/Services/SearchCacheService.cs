using System.Security.Cryptography;
using System.Text;

using MCPHub.CommandLineApp.Configuration;
using MCPHub.CommandLineApp.Models;

namespace MCPHub.CommandLineApp.Services;

/// <summary>
/// Search-specific caching service implementation with query normalization and similarity matching
/// </summary>
public class SearchCacheService(
    ILogger<SearchCacheService> logger,
    ICacheService cacheService,
    McpmConfiguration configuration) : ISearchCacheService {
    private readonly ILogger<SearchCacheService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly ICacheService _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
    private readonly McpmConfiguration _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

    private const string SEARCH_RESULTS_PREFIX = "search:results:";
    private const string SEARCH_QUERIES_PREFIX = "search:queries:";
    private const string POPULAR_QUERIES_KEY = "search:popular";

    public async Task CacheSearchResultsAsync(SearchRequest request, SearchResultResponse results, CancellationToken cancellationToken = default) {
        if (!_configuration.Cache.SearchResults.Enabled) {
            _logger.LogDebug("Search results caching disabled");
            return;
        }

        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(results);

        var normalizedRequest = NormalizeSearchRequest(request);
        var cacheKey = GenerateSearchCacheKey(normalizedRequest);
        var expiration = _configuration.Cache.SearchResults.Expiration;

        await _cacheService.SetAsync(cacheKey, results, expiration, cancellationToken).ConfigureAwait(false);

        // Also cache the query information for similarity matching
        await CacheQueryInfoAsync(normalizedRequest, results.TotalCount, cancellationToken).ConfigureAwait(false);

        _logger.LogDebug("Cached search results for query: {Query} (normalized key: {Key})", request.Query, cacheKey);
    }

    public async Task<SearchResultResponse?> GetCachedSearchResultsAsync(SearchRequest request, CancellationToken cancellationToken = default) {
        if (!_configuration.Cache.SearchResults.Enabled) {
            return null;
        }

        ArgumentNullException.ThrowIfNull(request);

        var normalizedRequest = NormalizeSearchRequest(request);
        var cacheKey = GenerateSearchCacheKey(normalizedRequest);

        var result = await _cacheService.GetAsync<SearchResultResponse>(cacheKey, cancellationToken).ConfigureAwait(false);

        if (result != null) {
            _logger.LogDebug("Retrieved cached search results for query: {Query}", request.Query);

            // Update query access information
            await UpdateQueryAccessAsync(normalizedRequest, cancellationToken).ConfigureAwait(false);
        }

        return result;
    }

    public async Task<IEnumerable<CachedSearchQuery>> FindSimilarQueriesAsync(string query, double similarityThreshold = 0.8, int maxResults = 5, CancellationToken cancellationToken = default) {
        if (string.IsNullOrWhiteSpace(query)) {
            return Enumerable.Empty<CachedSearchQuery>();
        }

        var normalizedQuery = NormalizeQuery(query);
        var allQueryKeys = await _cacheService.GetKeysAsync($"{SEARCH_QUERIES_PREFIX}*", cancellationToken).ConfigureAwait(false);

        var similarQueries = new List<CachedSearchQuery>();

        foreach (var queryKey in allQueryKeys) {
            try {
                var cachedQuery = await _cacheService.GetAsync<CachedSearchQuery>(queryKey, cancellationToken).ConfigureAwait(false);
                if (cachedQuery == null)
                    continue;

                var similarity = CalculateQuerySimilarity(normalizedQuery, cachedQuery.Query);
                if (similarity >= similarityThreshold) {
                    cachedQuery.SimilarityScore = similarity;
                    similarQueries.Add(cachedQuery);
                }
            }
            catch (Exception ex) {
                _logger.LogWarning(ex, "Failed to process cached query: {QueryKey}", queryKey);
            }
        }

        var results = similarQueries
            .OrderByDescending(q => q.SimilarityScore)
            .ThenByDescending(q => q.AccessCount)
            .Take(maxResults)
            .ToList();

        _logger.LogDebug("Found {Count} similar queries for: {Query}", results.Count, query);
        return results;
    }

    public async Task<SearchCacheStatistics> GetSearchCacheStatisticsAsync(CancellationToken cancellationToken = default) {
        var statistics = new SearchCacheStatistics();

        // Get overall cache statistics
        var cacheStats = await _cacheService.GetStatisticsAsync(cancellationToken).ConfigureAwait(false);
        statistics.SearchCacheHits = cacheStats.HitCount;
        statistics.SearchCacheMisses = cacheStats.MissCount;

        // Count search-related entries
        var searchKeys = await _cacheService.GetKeysAsync($"{SEARCH_RESULTS_PREFIX}*", cancellationToken).ConfigureAwait(false);
        statistics.TotalCachedQueries = searchKeys.Count();

        var queryKeys = await _cacheService.GetKeysAsync($"{SEARCH_QUERIES_PREFIX}*", cancellationToken).ConfigureAwait(false);

        // Calculate total search cache size
        long totalSize = 0;
        var popularQueries = new List<PopularSearchQuery>();

        foreach (var queryKey in queryKeys) {
            try {
                var entryInfo = await _cacheService.GetEntryInfoAsync(queryKey, cancellationToken).ConfigureAwait(false);
                if (entryInfo != null) {
                    totalSize += entryInfo.SizeBytes;
                }

                var cachedQuery = await _cacheService.GetAsync<CachedSearchQuery>(queryKey, cancellationToken).ConfigureAwait(false);
                if (cachedQuery != null) {
                    popularQueries.Add(new PopularSearchQuery {
                        Query = cachedQuery.Query,
                        SearchCount = cachedQuery.AccessCount,
                        FirstSearchedAt = cachedQuery.CachedAt,
                        LastSearchedAt = cachedQuery.CachedAt, // Would need to track this separately
                        AverageResultCount = cachedQuery.ResultCount
                    });
                }
            }
            catch (Exception ex) {
                _logger.LogWarning(ex, "Failed to analyze cached query: {QueryKey}", queryKey);
            }
        }

        statistics.TotalCacheSizeBytes = totalSize;
        statistics.PopularQueries = popularQueries
            .OrderByDescending(q => q.SearchCount)
            .Take(10)
            .ToList();

        return statistics;
    }

    public Task<int> WarmSearchCacheAsync(IEnumerable<string> popularQueries, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(popularQueries);

        var warmedCount = 0;
        foreach (var query in popularQueries) {
            try {
                // This would typically require integration with the API client to perform searches
                // For now, we'll just log the intention
                _logger.LogDebug("Would warm cache for query: {Query}", query);

                // TODO: Implement actual cache warming
                // 1. Create SearchRequest from query
                // 2. Execute search via API client
                // 3. Cache the results

                warmedCount++;

                // Respect cancellation
                cancellationToken.ThrowIfCancellationRequested();
            }
            catch (Exception ex) {
                _logger.LogWarning(ex, "Failed to warm cache for query: {Query}", query);
            }
        }

        _logger.LogInformation("Warmed search cache for {Count} queries", warmedCount);
        return Task.FromResult(warmedCount);
    }

    public async Task<int> ClearOldSearchCacheAsync(TimeSpan olderThan, CancellationToken cancellationToken = default) {
        var cutoffTime = DateTimeOffset.UtcNow - olderThan;
        var removedCount = 0;

        // Clear old search results
        var searchKeys = await _cacheService.GetKeysAsync($"{SEARCH_RESULTS_PREFIX}*", cancellationToken).ConfigureAwait(false);
        foreach (var key in searchKeys) {
            try {
                var entryInfo = await _cacheService.GetEntryInfoAsync(key, cancellationToken).ConfigureAwait(false);
                if (entryInfo != null && entryInfo.CreatedAt < cutoffTime) {
                    if (await _cacheService.RemoveAsync(key, cancellationToken).ConfigureAwait(false)) {
                        removedCount++;
                    }
                }
            }
            catch (Exception ex) {
                _logger.LogWarning(ex, "Failed to check/remove old search cache entry: {Key}", key);
            }
        }

        // Clear old query info
        var queryKeys = await _cacheService.GetKeysAsync($"{SEARCH_QUERIES_PREFIX}*", cancellationToken).ConfigureAwait(false);
        foreach (var key in queryKeys) {
            try {
                var entryInfo = await _cacheService.GetEntryInfoAsync(key, cancellationToken).ConfigureAwait(false);
                if (entryInfo != null && entryInfo.CreatedAt < cutoffTime) {
                    if (await _cacheService.RemoveAsync(key, cancellationToken).ConfigureAwait(false)) {
                        removedCount++;
                    }
                }
            }
            catch (Exception ex) {
                _logger.LogWarning(ex, "Failed to check/remove old query cache entry: {Key}", key);
            }
        }

        _logger.LogInformation("Cleared {Count} old search cache entries (older than {Age})", removedCount, olderThan);
        return removedCount;
    }

    public SearchRequest NormalizeSearchRequest(SearchRequest request) {
        ArgumentNullException.ThrowIfNull(request);

        return new SearchRequest {
            Query = NormalizeQuery(request.Query),
            Categories = request.Categories?.Select(c => c.Trim().ToLowerInvariant()).OrderBy(c => c).ToList(),
            TrustTier = request.TrustTier?.Trim(),
            Page = Math.Max(1, request.Page),
            PageSize = Math.Max(1, Math.Min(100, request.PageSize)), // Clamp between 1 and 100
            SortBy = request.SortBy?.Trim().ToLowerInvariant(),
            SortDirection = request.SortDirection?.Trim().ToLowerInvariant() ?? "ascending"
        };
    }

    public string GenerateSearchCacheKey(SearchRequest request) {
        ArgumentNullException.ThrowIfNull(request);

        var normalizedRequest = NormalizeSearchRequest(request);

        // Create a consistent string representation of the search parameters
        var keyBuilder = new StringBuilder();
        keyBuilder.Append($"q:{normalizedRequest.Query}");

        if (normalizedRequest.Categories?.Any() == true) {
            keyBuilder.Append($"|cat:{string.Join(",", normalizedRequest.Categories)}");
        }

        if (!string.IsNullOrEmpty(normalizedRequest.TrustTier)) {
            keyBuilder.Append($"|trust:{normalizedRequest.TrustTier}");
        }

        keyBuilder.Append($"|page:{normalizedRequest.Page}");
        keyBuilder.Append($"|size:{normalizedRequest.PageSize}");

        if (!string.IsNullOrEmpty(normalizedRequest.SortBy)) {
            keyBuilder.Append($"|sort:{normalizedRequest.SortBy}:{normalizedRequest.SortDirection}");
        }

        // Hash the key to keep it manageable and consistent
        var keyString = keyBuilder.ToString();
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(keyString));
        var hashString = Convert.ToHexString(hash).ToLowerInvariant();

        return $"{SEARCH_RESULTS_PREFIX}{hashString}";
    }

    public double CalculateQuerySimilarity(string query1, string query2) {
        if (string.IsNullOrWhiteSpace(query1) || string.IsNullOrWhiteSpace(query2)) {
            return 0.0;
        }

        var normalizedQuery1 = NormalizeQuery(query1);
        var normalizedQuery2 = NormalizeQuery(query2);

        if (normalizedQuery1 == normalizedQuery2) {
            return 1.0;
        }

        // Simple Jaccard similarity based on words
        var words1 = new HashSet<string>(normalizedQuery1.Split(' ', StringSplitOptions.RemoveEmptyEntries));
        var words2 = new HashSet<string>(normalizedQuery2.Split(' ', StringSplitOptions.RemoveEmptyEntries));

        if (words1.Count == 0 && words2.Count == 0) {
            return 1.0;
        }

        var intersection = words1.Intersect(words2).Count();
        var union = words1.Union(words2).Count();

        return (double)intersection / union;
    }

    private static string NormalizeQuery(string query) {
        if (string.IsNullOrWhiteSpace(query)) {
            return string.Empty;
        }

        return query.Trim().ToLowerInvariant()
            .Replace("  ", " ") // Replace multiple spaces with single space
            .Replace("\t", " ") // Replace tabs with spaces
            .Replace("\n", " ") // Replace newlines with spaces
            .Replace("\r", " "); // Replace carriage returns with spaces
    }

    private async Task CacheQueryInfoAsync(SearchRequest request, int resultCount, CancellationToken cancellationToken) {
        try {
            var queryInfo = new CachedSearchQuery {
                Query = request.Query,
                SimilarityScore = 1.0, // Perfect match with itself
                ResultCount = resultCount,
                CachedAt = DateTimeOffset.UtcNow,
                AccessCount = 1,
                Request = request
            };

            var queryKey = GetQueryInfoKey(request);
            var expiration = _configuration.Cache.SearchResults.Expiration;

            await _cacheService.SetAsync(queryKey, queryInfo, expiration, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex) {
            _logger.LogWarning(ex, "Failed to cache query information for: {Query}", request.Query);
        }
    }

    private async Task UpdateQueryAccessAsync(SearchRequest request, CancellationToken cancellationToken) {
        try {
            var queryKey = GetQueryInfoKey(request);
            var queryInfo = await _cacheService.GetAsync<CachedSearchQuery>(queryKey, cancellationToken).ConfigureAwait(false);

            if (queryInfo != null) {
                queryInfo.AccessCount++;
                var expiration = _configuration.Cache.SearchResults.Expiration;
                await _cacheService.SetAsync(queryKey, queryInfo, expiration, cancellationToken).ConfigureAwait(false);
            }
        }
        catch (Exception ex) {
            _logger.LogWarning(ex, "Failed to update query access information for: {Query}", request.Query);
        }
    }

    private string GetQueryInfoKey(SearchRequest request) {
        var cacheKey = GenerateSearchCacheKey(request);
        return cacheKey.Replace(SEARCH_RESULTS_PREFIX, SEARCH_QUERIES_PREFIX);
    }
}