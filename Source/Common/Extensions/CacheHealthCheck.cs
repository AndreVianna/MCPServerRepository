using Common.Configuration;

using Elastic.Clients.Elasticsearch;

namespace Common.Extensions;

/// <summary>
/// Cache health check implementation
/// </summary>
public class CacheHealthCheck(IOptions<CacheOptions> options) : IHealthCheck {
    private readonly CacheOptions _options = options.Value;

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default) {
        try {
            using var redis = StackExchange.Redis.ConnectionMultiplexer.Connect(_options.ConnectionString);
            using var timeout = new CancellationTokenSource(_options.HealthCheckTimeout);
            using var combinedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeout.Token);

            var database = redis.GetDatabase(_options.Database);
            await database.PingAsync();

            return HealthCheckResult.Healthy("Cache connection is healthy");
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) {
            return HealthCheckResult.Degraded("Cache health check was cancelled");
        }
        catch (OperationCanceledException) {
            return HealthCheckResult.Degraded("Cache health check timed out");
        }
        catch (Exception ex) {
            return HealthCheckResult.Degraded("Cache connection failed", ex);
        }
    }
}
