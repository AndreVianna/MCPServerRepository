using Common.Configuration;

using Elastic.Clients.Elasticsearch;

namespace Common.Extensions;

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