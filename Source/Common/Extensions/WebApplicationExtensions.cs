using Common.Middleware;
using Common.Services;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Common.Extensions;

/// <summary>
/// Extension methods for configuring web applications with full observability stack
/// </summary>
public static class WebApplicationExtensions {
    /// <summary>
    /// Adds comprehensive observability services to the service collection
    /// </summary>
    public static IServiceCollection AddObservabilityServices(this IServiceCollection services) {
        // Add monitoring service
        services.AddSingleton<IMonitoringService, MonitoringService>();

        // Add tracing service
        services.AddScoped<ITracingService, TracingService>();

        // Add health check services
        services.AddInfrastructureHealthChecks();

        // Add OpenTelemetry
        services.AddOpenTelemetryObservability();

        return services;
    }

    /// <summary>
    /// Configures the web application with full observability middleware pipeline
    /// </summary>
    public static WebApplication UseObservabilityMiddleware(this WebApplication app) {
        // Add request monitoring middleware early in the pipeline
        app.UseRequestMonitoring();

        // Configure health checks with detailed responses
        app.UseHealthChecks("/health", new HealthCheckOptions {
            ResponseWriter = async (context, report) =>                 // Use custom health check middleware for detailed responses
                await new Common.Middleware.HealthCheckMiddleware(
                    async (ctx) => await Task.CompletedTask,
                    context.RequestServices.GetRequiredService<HealthCheckService>(),
                    context.RequestServices.GetRequiredService<ILogger<Common.Middleware.HealthCheckMiddleware>>())
                    .InvokeAsync(context)
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
    /// Adds comprehensive application services including configuration and infrastructure
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration) {
        // Add configuration options
        services.AddConfigurationOptions(configuration);

        // Add observability services
        services.AddObservabilityServices();

        // Add MediatR from calling assembly
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(System.Reflection.Assembly.GetCallingAssembly()));

        // Add AutoMapper from calling assembly
        services.AddAutoMapper(System.Reflection.Assembly.GetCallingAssembly());

        // Add FluentValidation from calling assembly
        services.AddValidatorsFromAssembly(System.Reflection.Assembly.GetCallingAssembly());

        return services;
    }
}