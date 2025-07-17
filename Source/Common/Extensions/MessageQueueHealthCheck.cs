using Common.Configuration;

namespace Common.Extensions;

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

            await using var connection = await factory.CreateConnectionAsync(combinedCts.Token);
            await using var channel = await connection.CreateChannelAsync(null, combinedCts.Token);

            // Simple check to verify the connection is working
            await channel.QueueDeclarePassiveAsync("healthcheck.temp", combinedCts.Token);

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
