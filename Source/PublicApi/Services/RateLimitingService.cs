using System.Collections.Concurrent;
using MCPHub.Common.Services;
using MCPHub.PublicApi.Configuration;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace MCPHub.PublicApi.Services;

/// <summary>
/// Rate limiting service implementation supporting both in-memory and Redis-based rate limiting
/// </summary>
public class RateLimitingService : IRateLimitingService, IDisposable
{
    private readonly RateLimitingOptions _options;
    private readonly IDistributedCache? _distributedCache;
    private readonly ILogger<RateLimitingService> _logger;
    
    // In-memory storage for development
    private readonly ConcurrentDictionary<string, RateLimitEntry> _inMemoryStore = new();
    private readonly ConcurrentDictionary<string, RateLimitPolicy> _policies = new();
    private readonly Timer _cleanupTimer;
    
    public RateLimitingService(
        IOptions<RateLimitingOptions> options,
        IDistributedCache? distributedCache,
        ILogger<RateLimitingService> logger)
    {
        _options = options.Value ?? throw new ArgumentNullException(nameof(options));
        _distributedCache = distributedCache;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        // Initialize cleanup timer for in-memory store
        _cleanupTimer = new Timer(CleanupExpiredEntries, null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
        
        // Initialize default policies
        InitializeDefaultPolicies();
    }

    /// <summary>
    /// Checks if request is allowed under rate limits
    /// </summary>
    public async Task<RateLimitResult> CheckRateLimitAsync(string identifier, string policy, CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled)
        {
            return new RateLimitResult(true, long.MaxValue, TimeSpan.Zero, GetDefaultPolicy());
        }

        if (!_policies.TryGetValue(policy, out var rateLimitPolicy))
        {
            _logger.LogWarning("Rate limit policy '{Policy}' not found, using default", policy);
            rateLimitPolicy = GetDefaultPolicy();
        }

        var key = GetCacheKey(identifier, policy);
        var now = DateTimeOffset.UtcNow;

        try
        {
            if (_options.UseInMemory || _distributedCache == null)
            {
                return await CheckRateLimitInMemoryAsync(key, rateLimitPolicy, now);
            }
            else
            {
                return await CheckRateLimitDistributedAsync(key, rateLimitPolicy, now, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking rate limit for key: {Key}", key);
            // Fail open - allow request if rate limiting check fails
            return new RateLimitResult(true, rateLimitPolicy.RequestLimit, TimeSpan.Zero, rateLimitPolicy);
        }
    }

    /// <summary>
    /// Records a request for rate limiting
    /// </summary>
    public async Task RecordRequestAsync(string identifier, string policy, CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled)
            return;

        if (!_policies.TryGetValue(policy, out var rateLimitPolicy))
        {
            rateLimitPolicy = GetDefaultPolicy();
        }

        var key = GetCacheKey(identifier, policy);
        var now = DateTimeOffset.UtcNow;

        try
        {
            if (_options.UseInMemory || _distributedCache == null)
            {
                await RecordRequestInMemoryAsync(key, rateLimitPolicy, now);
            }
            else
            {
                await RecordRequestDistributedAsync(key, rateLimitPolicy, now, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording request for key: {Key}", key);
        }
    }

    /// <summary>
    /// Gets current usage for an identifier
    /// </summary>
    public async Task<RateLimitUsage> GetUsageAsync(string identifier, string policy, CancellationToken cancellationToken = default)
    {
        if (!_policies.TryGetValue(policy, out var rateLimitPolicy))
        {
            rateLimitPolicy = GetDefaultPolicy();
        }

        var key = GetCacheKey(identifier, policy);
        
        try
        {
            if (_options.UseInMemory || _distributedCache == null)
            {
                return GetUsageInMemory(key, rateLimitPolicy);
            }
            else
            {
                return await GetUsageDistributedAsync(key, rateLimitPolicy, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting usage for key: {Key}", key);
            return new RateLimitUsage(0, rateLimitPolicy.WindowDuration, DateTimeOffset.UtcNow, null);
        }
    }

    /// <summary>
    /// Resets rate limit counters
    /// </summary>
    public async Task ResetAsync(string identifier, string policy, CancellationToken cancellationToken = default)
    {
        var key = GetCacheKey(identifier, policy);
        
        try
        {
            if (_options.UseInMemory || _distributedCache == null)
            {
                _inMemoryStore.TryRemove(key, out _);
            }
            else
            {
                await _distributedCache.RemoveAsync(key, cancellationToken);
            }
            
            _logger.LogInformation("Rate limit reset for key: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting rate limit for key: {Key}", key);
        }
    }

    /// <summary>
    /// Configures rate limit policies
    /// </summary>
    public Task SetPolicyAsync(string policyName, RateLimitPolicy policy, CancellationToken cancellationToken = default)
    {
        _policies.AddOrUpdate(policyName, policy, (_, _) => policy);
        _logger.LogInformation("Rate limit policy '{PolicyName}' configured", policyName);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Gets rate limiting statistics
    /// </summary>
    public Task<RateLimitStatistics> GetStatisticsAsync(CancellationToken cancellationToken = default)
    {
        // For now, return basic statistics
        // In a production environment, this would aggregate from distributed cache or monitoring system
        var totalRequests = _inMemoryStore.Values.Sum(e => e.RequestCount);
        var blockedRequests = _inMemoryStore.Values.Sum(e => e.BlockedCount);
        var blockRate = totalRequests > 0 ? (double)blockedRequests / totalRequests : 0.0;
        
        var policyUsage = _policies.ToDictionary(
            kvp => kvp.Key,
            kvp => _inMemoryStore.Values.Where(e => e.PolicyName == kvp.Key).Sum(e => e.RequestCount));

        var statistics = new RateLimitStatistics(
            totalRequests,
            blockedRequests,
            blockRate,
            policyUsage,
            DateTime.UtcNow);

        return Task.FromResult(statistics);
    }

    private async Task<RateLimitResult> CheckRateLimitInMemoryAsync(string key, RateLimitPolicy policy, DateTimeOffset now)
    {
        var entry = _inMemoryStore.GetOrAdd(key, _ => new RateLimitEntry
        {
            WindowStart = now,
            RequestCount = 0,
            BlockedCount = 0,
            PolicyName = key.Split(':').LastOrDefault() ?? "default"
        });

        // Check if window has expired
        if (now - entry.WindowStart > policy.WindowDuration)
        {
            entry.WindowStart = now;
            entry.RequestCount = 0;
        }

        var isAllowed = entry.RequestCount < policy.RequestLimit;
        var remaining = Math.Max(0, policy.RequestLimit - entry.RequestCount);
        var resetTime = entry.WindowStart.Add(policy.WindowDuration) - now;

        if (!isAllowed)
        {
            entry.BlockedCount++;
        }

        return new RateLimitResult(isAllowed, remaining, resetTime, policy);
    }

    private async Task<RateLimitResult> CheckRateLimitDistributedAsync(string key, RateLimitPolicy policy, DateTimeOffset now, CancellationToken cancellationToken)
    {
        if (_distributedCache == null)
            throw new InvalidOperationException("Distributed cache is not available");

        var entryJson = await _distributedCache.GetStringAsync(key, cancellationToken);
        var entry = entryJson != null 
            ? JsonSerializer.Deserialize<RateLimitEntry>(entryJson) 
            : new RateLimitEntry { WindowStart = now, RequestCount = 0, BlockedCount = 0 };

        // Check if window has expired
        if (now - entry.WindowStart > policy.WindowDuration)
        {
            entry.WindowStart = now;
            entry.RequestCount = 0;
        }

        var isAllowed = entry.RequestCount < policy.RequestLimit;
        var remaining = Math.Max(0, policy.RequestLimit - entry.RequestCount);
        var resetTime = entry.WindowStart.Add(policy.WindowDuration) - now;

        if (!isAllowed)
        {
            entry.BlockedCount++;
            // Update cache with blocked count
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = policy.WindowDuration
            };
            await _distributedCache.SetStringAsync(key, JsonSerializer.Serialize(entry), options, cancellationToken);
        }

        return new RateLimitResult(isAllowed, remaining, resetTime, policy);
    }

    private async Task RecordRequestInMemoryAsync(string key, RateLimitPolicy policy, DateTimeOffset now)
    {
        _inMemoryStore.AddOrUpdate(key,
            new RateLimitEntry { WindowStart = now, RequestCount = 1, BlockedCount = 0 },
            (_, existing) =>
            {
                if (now - existing.WindowStart > policy.WindowDuration)
                {
                    existing.WindowStart = now;
                    existing.RequestCount = 1;
                }
                else
                {
                    existing.RequestCount++;
                }
                return existing;
            });
    }

    private async Task RecordRequestDistributedAsync(string key, RateLimitPolicy policy, DateTimeOffset now, CancellationToken cancellationToken)
    {
        if (_distributedCache == null)
            throw new InvalidOperationException("Distributed cache is not available");

        var entryJson = await _distributedCache.GetStringAsync(key, cancellationToken);
        var entry = entryJson != null 
            ? JsonSerializer.Deserialize<RateLimitEntry>(entryJson) 
            : new RateLimitEntry { WindowStart = now, RequestCount = 0, BlockedCount = 0 };

        if (now - entry.WindowStart > policy.WindowDuration)
        {
            entry.WindowStart = now;
            entry.RequestCount = 1;
        }
        else
        {
            entry.RequestCount++;
        }

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = policy.WindowDuration
        };

        await _distributedCache.SetStringAsync(key, JsonSerializer.Serialize(entry), options, cancellationToken);
    }

    private RateLimitUsage GetUsageInMemory(string key, RateLimitPolicy policy)
    {
        if (_inMemoryStore.TryGetValue(key, out var entry))
        {
            var nextReset = entry.WindowStart.Add(policy.WindowDuration);
            return new RateLimitUsage(entry.RequestCount, policy.WindowDuration, entry.WindowStart, nextReset);
        }

        return new RateLimitUsage(0, policy.WindowDuration, DateTimeOffset.UtcNow, null);
    }

    private async Task<RateLimitUsage> GetUsageDistributedAsync(string key, RateLimitPolicy policy, CancellationToken cancellationToken)
    {
        if (_distributedCache == null)
            throw new InvalidOperationException("Distributed cache is not available");

        var entryJson = await _distributedCache.GetStringAsync(key, cancellationToken);
        if (entryJson != null)
        {
            var entry = JsonSerializer.Deserialize<RateLimitEntry>(entryJson);
            var nextReset = entry?.WindowStart.Add(policy.WindowDuration);
            return new RateLimitUsage(entry?.RequestCount ?? 0, policy.WindowDuration, entry?.WindowStart ?? DateTimeOffset.UtcNow, nextReset);
        }

        return new RateLimitUsage(0, policy.WindowDuration, DateTimeOffset.UtcNow, null);
    }

    private void InitializeDefaultPolicies()
    {
        // Authentication policy (5 req/min)
        _policies["Authentication"] = new RateLimitPolicy(
            _options.Authentication.PermitLimit, 
            TimeSpan.FromMinutes(_options.Authentication.WindowMinutes), 
            ParseStrategy(_options.Authentication.Strategy));

        // Search policy (100 req/min)
        _policies["Search"] = new RateLimitPolicy(
            _options.Search.PermitLimit, 
            TimeSpan.FromMinutes(_options.Search.WindowMinutes), 
            ParseStrategy(_options.Search.Strategy));

        // Package CRUD policy (50 req/min)
        _policies["PackageCrud"] = new RateLimitPolicy(
            _options.PackageCrud.PermitLimit, 
            TimeSpan.FromMinutes(_options.PackageCrud.WindowMinutes), 
            ParseStrategy(_options.PackageCrud.Strategy));

        // Public read policy (200 req/min)
        _policies["PublicRead"] = new RateLimitPolicy(
            _options.PublicRead.PermitLimit, 
            TimeSpan.FromMinutes(_options.PublicRead.WindowMinutes), 
            ParseStrategy(_options.PublicRead.Strategy));

        // Global fallback policy
        _policies["Global"] = new RateLimitPolicy(
            _options.Global.PermitLimit, 
            TimeSpan.FromMinutes(_options.Global.WindowMinutes), 
            ParseStrategy(_options.Global.Strategy));
    }

    private static RateLimitStrategy ParseStrategy(string strategy)
    {
        return strategy.ToLowerInvariant() switch
        {
            "fixedwindow" => RateLimitStrategy.FixedWindow,
            "slidingwindow" => RateLimitStrategy.SlidingWindow,
            "tokenbucket" => RateLimitStrategy.TokenBucket,
            "leaky" => RateLimitStrategy.Leaky,
            _ => RateLimitStrategy.FixedWindow
        };
    }

    private RateLimitPolicy GetDefaultPolicy()
    {
        return _policies.GetValueOrDefault("Global", new RateLimitPolicy(1000, TimeSpan.FromMinutes(1), RateLimitStrategy.FixedWindow));
    }

    private static string GetCacheKey(string identifier, string policy)
    {
        return $"rate_limit:{policy}:{identifier}";
    }

    private void CleanupExpiredEntries(object? state)
    {
        try
        {
            var now = DateTimeOffset.UtcNow;
            var expiredKeys = _inMemoryStore.Where(kvp => 
                now - kvp.Value.WindowStart > TimeSpan.FromHours(1)) // Keep entries for 1 hour max
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var key in expiredKeys)
            {
                _inMemoryStore.TryRemove(key, out _);
            }

            if (expiredKeys.Count > 0)
            {
                _logger.LogDebug("Cleaned up {Count} expired rate limit entries", expiredKeys.Count);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during rate limit cleanup");
        }
    }

    public void Dispose()
    {
        _cleanupTimer?.Dispose();
    }
}

/// <summary>
/// Internal rate limit entry for tracking requests
/// </summary>
internal class RateLimitEntry
{
    public DateTimeOffset WindowStart { get; set; }
    public long RequestCount { get; set; }
    public long BlockedCount { get; set; }
    public string PolicyName { get; set; } = string.Empty;
}