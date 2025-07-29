using MCPHub.Common.Configuration;
using MCPHub.Common.Configuration.Validators;
using MCPHub.Common.Services;

namespace MCPHub.Common.Extensions;

public static class ServiceCollectionExtensions {
    /// <summary>
    /// Registers all configuration options from the Common.Configuration namespace
    /// </summary>
    public static IServiceCollection AddConfigurationOptions(this IServiceCollection services, IConfiguration configuration) {
        services.Configure<CacheOptions>(configuration.GetSection(CacheOptions.SectionName));
        services.Configure<ObservabilityOptions>(configuration.GetSection(ObservabilityOptions.SectionName));

        // Validate options on startup
        services.AddSingleton<IValidateOptions<CacheOptions>, CacheOptionsValidator>();
        services.AddSingleton<IValidateOptions<ObservabilityOptions>, ObservabilityOptionsValidator>();

        return services;
    }

    /// <summary>
    /// Registers observability services with their Enhanced skeleton implementations
    /// </summary>
    public static IServiceCollection AddObservabilityServices(this IServiceCollection services) {
        // Core monitoring services
        services.AddSingleton<IHealthCheckService, MCPHealthCheckService>();
        services.AddSingleton<IMonitoringService, MonitoringService>();

        // Multi-interface observability service (provides distributed tracing, MCP tracing, and metrics)
        services.AddSingleton<EnhancedObservabilityService>();
        services.AddSingleton<IDistributedTracingService>(provider => provider.GetRequiredService<EnhancedObservabilityService>());
        services.AddSingleton<IMCPTracingService>(provider => provider.GetRequiredService<EnhancedObservabilityService>());
        services.AddSingleton<IMCPMetricsService>(provider => provider.GetRequiredService<EnhancedObservabilityService>());

        // Logging services
        services.AddSingleton<EnhancedStructuredLoggingService>();
        services.AddSingleton<IStructuredLoggingService>(provider => provider.GetRequiredService<EnhancedStructuredLoggingService>());
        services.AddSingleton<IMCPStructuredLoggingService>(provider => provider.GetRequiredService<EnhancedStructuredLoggingService>());
        services.AddSingleton<ILogAggregationService, LogAggregationService>();
        services.AddSingleton<ISecurityEventLoggingService, SecurityEventLoggingService>();
        services.AddSingleton<ILogRetentionService, LogRetentionService>();

        // Dashboard and alerting services (all defined in EnhancedDashboardService.cs)
        services.AddSingleton<IDashboardService, DashboardService>();
        services.AddSingleton<IAlertingService, AlertingService>();
        services.AddSingleton<IUptimeTrackingService, UptimeTrackingService>();

        // Infrastructure monitoring services (all defined in EnhancedInfrastructureMonitoringService.cs)
        services.AddSingleton<IClusterMonitoringService, ClusterMonitoringService>();
        services.AddSingleton<IPerformanceInsightsService, PerformanceInsightsService>();
        services.AddSingleton<INetworkMonitoringService, NetworkMonitoringService>();
        services.AddSingleton<ICostMonitoringService, CostMonitoringService>();

        return services;
    }

    // Note: Service discovery and automatic registration methods removed
    // All service implementations removed - only configuration options registration available

}