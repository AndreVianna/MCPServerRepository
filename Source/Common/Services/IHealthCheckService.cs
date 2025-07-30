namespace MCPHub.Common.Services;

/// <summary>
/// Comprehensive health checking service
/// </summary>
public interface IHealthCheckService {
    /// <summary>
    /// Performs comprehensive health check
    /// </summary>
    Task<OverallHealthResult> CheckHealthAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks specific component health
    /// </summary>
    Task<ComponentHealthResult> CheckComponentAsync(string componentName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers a custom health check
    /// </summary>
    void RegisterHealthCheck(string name, Func<CancellationToken, Task<ComponentHealthResult>> healthCheck);

    /// <summary>
    /// Gets health history for trending
    /// </summary>
    Task<IEnumerable<HealthSnapshot>> GetHealthHistoryAsync(TimeSpan period, CancellationToken cancellationToken = default);

    /// <summary>
    /// Configures health check alerts
    /// </summary>
    Task ConfigureAlertAsync(string componentName, HealthThreshold threshold, CancellationToken cancellationToken = default);
}

/// <summary>
/// Overall health check result
/// </summary>
public record OverallHealthResult(
    Providers.HealthStatus Status,
    TimeSpan ResponseTime,
    IEnumerable<ComponentHealthResult> Components,
    DateTime Timestamp);

/// <summary>
/// Health snapshot for trending
/// </summary>
public record HealthSnapshot(
    DateTime Timestamp,
    Providers.HealthStatus Status,
    TimeSpan ResponseTime,
    IDictionary<string, Providers.HealthStatus> ComponentStatuses);

/// <summary>
/// Health threshold configuration
/// </summary>
public record HealthThreshold(
    TimeSpan MaxResponseTime,
    Providers.HealthStatus MinStatus,
    string NotificationEndpoint);