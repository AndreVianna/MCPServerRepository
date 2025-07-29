namespace MCPHub.Common.Providers;

/// <summary>
/// Centralized factory for all infrastructure providers
/// </summary>
public interface IInfrastructureProviderFactory {
    /// <summary>
    /// Creates database provider
    /// </summary>
    IDatabaseProvider CreateDatabaseProvider();

    /// <summary>
    /// Creates enhanced cache service
    /// </summary>
    IEnhancedCacheService CreateCacheService();

    /// <summary>
    /// Creates storage service
    /// </summary>
    IStorageService CreateStorageService();

    /// <summary>
    /// Creates message publisher
    /// </summary>
    IMessagePublisher CreateMessagePublisher();

    /// <summary>
    /// Creates event store
    /// </summary>
    IEventStore CreateEventStore();

    /// <summary>
    /// Creates background job service
    /// </summary>
    IBackgroundJobService CreateBackgroundJobService();

    /// <summary>
    /// Creates health check service
    /// </summary>
    IHealthCheckService CreateHealthCheckService();

    /// <summary>
    /// Creates rate limiting service
    /// </summary>
    IRateLimitingService CreateRateLimitingService();

    /// <summary>
    /// Creates telemetry service
    /// </summary>
    ITelemetryService CreateTelemetryService();

    /// <summary>
    /// Creates feature flag service
    /// </summary>
    IFeatureFlagService CreateFeatureFlagService();

    /// <summary>
    /// Validates entire infrastructure configuration
    /// </summary>
    Task<InfrastructureValidationResult> ValidateConfigurationAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets infrastructure health status
    /// </summary>
    Task<InfrastructureHealthResult> GetInfrastructureHealthAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Estimates total infrastructure costs
    /// </summary>
    Task<CostEstimation> EstimateCostsAsync(EnvironmentTier tier, UsageProjection usage, CancellationToken cancellationToken = default);
}

/// <summary>
/// Infrastructure validation result
/// </summary>
public record InfrastructureValidationResult(
    bool IsValid,
    IDictionary<string, ProviderValidationResult> ProviderResults,
    IEnumerable<string> GlobalErrors);

/// <summary>
/// Infrastructure health result
/// </summary>
public record InfrastructureHealthResult(
    HealthStatus OverallStatus,
    IDictionary<string, ComponentHealthResult> ComponentHealth,
    DateTime Timestamp);

/// <summary>
/// Overall cost estimation
/// </summary>
public record CostEstimation(
    decimal MonthlyEstimate,
    IDictionary<string, decimal> ServiceBreakdown,
    CostProjection ThreeMonthProjection,
    IEnumerable<CostOptimizationRecommendation> Recommendations);

/// <summary>
/// Health status enumeration
/// </summary>
public enum HealthStatus {
    Healthy,
    Degraded,
    Unhealthy
}

/// <summary>
/// Component health result
/// </summary>
public record ComponentHealthResult(
    string Name,
    HealthStatus Status,
    TimeSpan ResponseTime,
    string? Description,
    IDictionary<string, object> Data,
    string? ErrorMessage);