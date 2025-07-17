using Common.Configuration;

using Elastic.Clients.Elasticsearch;

namespace Common.Extensions;

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
