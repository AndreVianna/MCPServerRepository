using Common.Configuration;

using Elastic.Clients.Elasticsearch;

namespace Common.Extensions;

/// <summary>
/// Search health check implementation
/// </summary>
public class SearchHealthCheck(IOptions<ElasticsearchOptions> options) : IHealthCheck {
    private readonly ElasticsearchOptions _options = options.Value;

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default) {
        try {
            var settings = new ElasticsearchClientSettings(new Uri(_options.ConnectionString))
                .RequestTimeout(_options.HealthCheckTimeout);

            var client = new ElasticsearchClient(settings);
            using var timeout = new CancellationTokenSource(_options.HealthCheckTimeout);
            using var combinedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeout.Token);

            var response = await client.PingAsync(combinedCts.Token);

            return response.IsValidResponse
                ? HealthCheckResult.Healthy("Search connection is healthy")
                : HealthCheckResult.Degraded("Search connection failed");
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) {
            return HealthCheckResult.Degraded("Search health check was cancelled");
        }
        catch (OperationCanceledException) {
            return HealthCheckResult.Degraded("Search health check timed out");
        }
        catch (Exception ex) {
            return HealthCheckResult.Degraded("Search connection failed", ex);
        }
    }
}
