using System.Text.Json;

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

using StackExchange.Redis;

namespace Common.Services;

public class RedisCacheService : ICacheService {
    private readonly IDistributedCache _cache;
    private readonly IDatabase _database;
    private readonly ILogger<RedisCacheService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public RedisCacheService(
        IDistributedCache cache,
        IConnectionMultiplexer connectionMultiplexer,
        ILogger<RedisCacheService> logger) {
        _cache = cache;
        _database = connectionMultiplexer.GetDatabase();
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) {
        try {
            var cachedValue = await _cache.GetStringAsync(key, cancellationToken);
            return cachedValue == null ? default : JsonSerializer.Deserialize<T>(cachedValue, _jsonOptions);
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error getting cache value for key: {Key}", key);
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) {
        try {
            var serializedValue = JsonSerializer.Serialize(value, _jsonOptions);

            var options = new DistributedCacheEntryOptions();
            if (expiration.HasValue)
                options.SetAbsoluteExpiration(expiration.Value);
            else
                options.SetSlidingExpiration(TimeSpan.FromMinutes(30)); // Default 30 minutes

            await _cache.SetStringAsync(key, serializedValue, options, cancellationToken);
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error setting cache value for key: {Key}", key);
        }
    }

    public async Task SetAsync<T>(string key, T value, DateTimeOffset expiration, CancellationToken cancellationToken = default) {
        try {
            var serializedValue = JsonSerializer.Serialize(value, _jsonOptions);

            var options = new DistributedCacheEntryOptions {
                AbsoluteExpiration = expiration
            };

            await _cache.SetStringAsync(key, serializedValue, options, cancellationToken);
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error setting cache value for key: {Key}", key);
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default) {
        try {
            await _cache.RemoveAsync(key, cancellationToken);
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error removing cache value for key: {Key}", key);
        }
    }

    public async Task RemovePatternAsync(string pattern, CancellationToken cancellationToken = default) {
        try {
            var server = _database.Multiplexer.GetServer(_database.Multiplexer.GetEndPoints().First());
            var keys = server.Keys(pattern: pattern);

            var keyArray = keys.ToArray();
            if (keyArray.Length > 0) {
                await _database.KeyDeleteAsync(keyArray);
            }
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error removing cache values for pattern: {Pattern}", pattern);
        }
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default) {
        try {
            return await _database.KeyExistsAsync(key);
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error checking cache existence for key: {Key}", key);
            return false;
        }
    }
}