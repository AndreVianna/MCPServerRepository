using HealthStatus = Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus;

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
