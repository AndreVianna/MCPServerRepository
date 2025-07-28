namespace MCPHub.PublicApi.Extensions;

public static class HealthCheckExtensions {
    /// <summary>
    /// Adds basic health checks configuration
    /// </summary>
    public static IServiceCollection AddInfrastructureHealthChecks(this IServiceCollection services) {
        // Note: All health check implementations removed - only basic health checks available
        // Concrete health check implementations must be provided when needed
        services.AddHealthChecks();
        
        return services;
    }
}