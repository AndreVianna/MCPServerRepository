namespace MCPHub.Common.Services;

/// <summary>
/// Rate limiting service for API protection
/// Supports: In-Memory → Redis → Enterprise Gateway
/// </summary>
public interface IRateLimitingService {
    /// <summary>
    /// Checks if request is allowed under rate limits
    /// </summary>
    Task<RateLimitResult> CheckRateLimitAsync(string identifier, string policy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Records a request for rate limiting
    /// </summary>
    Task RecordRequestAsync(string identifier, string policy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets current usage for an identifier
    /// </summary>
    Task<RateLimitUsage> GetUsageAsync(string identifier, string policy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Resets rate limit counters
    /// </summary>
    Task ResetAsync(string identifier, string policy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Configures rate limit policies
    /// </summary>
    Task SetPolicyAsync(string policyName, RateLimitPolicy policy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets rate limiting statistics
    /// </summary>
    Task<RateLimitStatistics> GetStatisticsAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Rate limit check result
/// </summary>
public record RateLimitResult(
    bool IsAllowed,
    long RequestsRemaining,
    TimeSpan ResetTime,
    RateLimitPolicy Policy);

/// <summary>
/// Rate limit usage information
/// </summary>
public record RateLimitUsage(
    long RequestCount,
    TimeSpan WindowDuration,
    DateTimeOffset WindowStart,
    DateTimeOffset? NextReset);

/// <summary>
/// Rate limit policy configuration
/// </summary>
public record RateLimitPolicy(
    long RequestLimit,
    TimeSpan WindowDuration,
    RateLimitStrategy Strategy);

/// <summary>
/// Rate limiting strategies
/// </summary>
public enum RateLimitStrategy {
    FixedWindow,
    SlidingWindow,
    TokenBucket,
    Leaky,
}

/// <summary>
/// Rate limiting statistics
/// </summary>
public record RateLimitStatistics(
    long TotalRequests,
    long BlockedRequests,
    double BlockRate,
    IDictionary<string, long> PolicyUsage,
    DateTime Timestamp);