using MCPHub.Common.Extensions;

using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

using RequestMonitoringExtensions = MCPHub.PublicApi.Middleware.RequestMonitoringExtensions;

namespace MCPHub.PublicApi.Extensions;

/// <summary>
/// Extension methods for configuring web applications with full observability stack
/// </summary>
public static class WebApplicationExtensions {
    /// <summary>
    /// Adds observability services configuration
    /// </summary>
    public static IServiceCollection AddObservabilityServices(this IServiceCollection services) {
        // Note: Service implementations removed - only interfaces and configuration available
        
        // Add health check services
        services.AddInfrastructureHealthChecks();

        // Note: OpenTelemetry implementations removed - only standard ILogger interfaces available

        return services;
    }

    /// <summary>
    /// Configures the web application with full observability middleware pipeline
    /// </summary>
    public static WebApplication UseObservabilityMiddleware(this WebApplication app) {
        // Add request monitoring middleware early in the pipeline
        RequestMonitoringExtensions.UseRequestMonitoring(app);

        // Configure health checks with detailed responses
        app.UseHealthChecks("/health", new HealthCheckOptions {
            ResponseWriter = async (context, report) => {
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new {
                    status = report.Status.ToString(),
                    totalDuration = report.TotalDuration,
                    entries = report.Entries.Select(e => new {
                        name = e.Key,
                        status = e.Value.Status.ToString(),
                        duration = e.Value.Duration,
                        description = e.Value.Description
                    })
                }));
            }
        });

        // Add health check endpoints for different categories
        app.UseHealthChecks("/health/ready", new HealthCheckOptions {
            Predicate = check => check.Tags.Contains("readiness"),
            ResponseWriter = async (context, report) => {
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync(report.Status.ToString());
            }
        });

        app.UseHealthChecks("/health/live", new HealthCheckOptions {
            Predicate = check => check.Tags.Contains("liveness"),
            ResponseWriter = async (context, report) => {
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync(report.Status.ToString());
            }
        });

        return app;
    }

    /// <summary>
    /// Adds application services configuration
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration) {
        // Add configuration options
        services.AddConfigurationOptions(configuration);

        // Add observability services
        services.AddObservabilityServices();

        // Note: Command handlers registration removed - implementations not available
        // Configuration validation is handled by IValidateOptions<T> validators

        return services;
    }
}