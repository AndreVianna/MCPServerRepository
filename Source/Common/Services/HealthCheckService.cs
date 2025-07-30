using MCPHub.Common.Providers;

namespace MCPHub.Common.Services;

/// <summary>
/// MCP health check service skeleton implementation
/// Following contracts-first approach - implementation when needed
/// </summary>
public class MCPHealthCheckService : IHealthCheckService {
    public Task<OverallHealthResult> CheckHealthAsync(CancellationToken cancellationToken = default)
        => throw new NotImplementedException("Comprehensive health checks will be implemented when system health monitoring is needed");

    public Task<ComponentHealthResult> CheckComponentAsync(string componentName, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("Component-specific health checks will be implemented when individual component monitoring is needed");

    public void RegisterHealthCheck(string name, Func<CancellationToken, Task<ComponentHealthResult>> healthCheck)
        => throw new NotImplementedException("Health check registration will be implemented when dynamic health check configuration is needed");

    public Task<IEnumerable<HealthSnapshot>> GetHealthHistoryAsync(TimeSpan period, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("Health history will be implemented when health trending is needed");

    public Task ConfigureAlertAsync(string componentName, HealthThreshold threshold, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("Health check alerting will be implemented when health monitoring alerts are needed");
}