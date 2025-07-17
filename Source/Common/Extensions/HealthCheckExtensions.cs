using Common.Configuration;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace Common.Extensions;

public static class HealthCheckExtensions {
    /// <summary>
    /// Adds comprehensive health checks for all infrastructure components
    /// </summary>
    public static IServiceCollection AddInfrastructureHealthChecks(this IServiceCollection services) {
        var healthChecksBuilder = services.AddHealthChecks();

        // Add database health check
        healthChecksBuilder.AddCheck<DatabaseHealthCheck>(
            "database",
            HealthStatus.Unhealthy,
            ["database", "infrastructure"]);

        // Add cache health check
        healthChecksBuilder.AddCheck<CacheHealthCheck>(
            "cache",
            HealthStatus.Degraded,
            ["cache", "infrastructure"]);

        // Add search health check
        healthChecksBuilder.AddCheck<SearchHealthCheck>(
            "search",
            HealthStatus.Degraded,
            ["search", "infrastructure"]);

        // Add message queue health check
        healthChecksBuilder.AddCheck<MessageQueueHealthCheck>(
            "messagequeue",
            HealthStatus.Degraded,
            ["messagequeue", "infrastructure"]);

        // Add application health check
        healthChecksBuilder.AddCheck<ApplicationHealthCheck>(
            "application",
            HealthStatus.Unhealthy,
            ["application", "readiness"]);

        return services;
    }
}

/// <summary>
/// Database health check implementation
/// </summary>
public class DatabaseHealthCheck(IOptions<DatabaseOptions> options) : IHealthCheck {
    private readonly DatabaseOptions _options = options.Value;

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default) {
        try {
            using var connection = new Npgsql.NpgsqlConnection(_options.ConnectionString);
            using var timeout = new CancellationTokenSource(_options.HealthCheckTimeout);
            using var combinedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeout.Token);

            await connection.OpenAsync(combinedCts.Token);

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT 1";
            await command.ExecuteScalarAsync(combinedCts.Token);

            return HealthCheckResult.Healthy("Database connection is healthy");
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) {
            return HealthCheckResult.Unhealthy("Database health check was cancelled");
        }
        catch (OperationCanceledException) {
            return HealthCheckResult.Unhealthy("Database health check timed out");
        }
        catch (Exception ex) {
            return HealthCheckResult.Unhealthy("Database connection failed", ex);
        }
    }
}

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

/// <summary>
/// Search health check implementation
/// </summary>
public class SearchHealthCheck(IOptions<ElasticsearchOptions> options) : IHealthCheck {
    private readonly ElasticsearchOptions _options = options.Value;

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default) {
        try {
            var settings = new Elastic.Clients.Elasticsearch.ElasticsearchClientSettings(new Uri(_options.ConnectionString))
                .RequestTimeout(_options.HealthCheckTimeout);

            using var client = new Elastic.Clients.Elasticsearch.ElasticsearchClient(settings);
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

/// <summary>
/// Message queue health check implementation
/// </summary>
public class MessageQueueHealthCheck(IOptions<RabbitMqOptions> options) : IHealthCheck {
    private readonly RabbitMqOptions _options = options.Value;

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default) {
        try {
            using var timeout = new CancellationTokenSource(_options.HealthCheckTimeout);
            using var combinedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeout.Token);

            var factory = new RabbitMQ.Client.ConnectionFactory() {
                Uri = new Uri(_options.ConnectionString),
                RequestedHeartbeat = TimeSpan.FromSeconds(10),
                RequestedConnectionTimeout = _options.HealthCheckTimeout
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            // Simple check to verify the connection is working
            await Task.Run(() => channel.QueueDeclarePassive("healthcheck.temp"), combinedCts.Token);

            return HealthCheckResult.Healthy("Message queue connection is healthy");
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) {
            return HealthCheckResult.Degraded("Message queue health check was cancelled");
        }
        catch (OperationCanceledException) {
            return HealthCheckResult.Degraded("Message queue health check timed out");
        }
        catch (Exception ex) {
            return HealthCheckResult.Degraded("Message queue connection failed", ex);
        }
    }
}

/// <summary>
/// Application health check implementation
/// </summary>
public class ApplicationHealthCheck : IHealthCheck {
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default) {
        // Check application-specific health indicators
        var memoryUsage = GC.GetTotalMemory(false);
        var workingSet = Environment.WorkingSet;

        // Simple memory pressure check
        return memoryUsage > 500 * 1024 * 1024
            ? Task.FromResult(HealthCheckResult.Degraded(
                "High memory usage detected",
                data: new Dictionary<string, object> {
                    ["memoryUsage"] = memoryUsage,
                    ["workingSet"] = workingSet
                }))
            : Task.FromResult(HealthCheckResult.Healthy("Application is healthy",
            data: new Dictionary<string, object> {
                ["memoryUsage"] = memoryUsage,
                ["workingSet"] = workingSet,
                ["uptime"] = Environment.TickCount64
            }));
    }
}