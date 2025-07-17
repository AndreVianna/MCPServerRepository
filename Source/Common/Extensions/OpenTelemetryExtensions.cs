using Common.Configuration;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Common.Extensions;

public static class OpenTelemetryExtensions {
    /// <summary>
    /// Adds OpenTelemetry tracing, metrics, and logging
    /// </summary>
    public static IServiceCollection AddOpenTelemetryObservability(this IServiceCollection services) {
        services.AddOpenTelemetry()
            .ConfigureResource(ConfigureResource)
            .WithTracing(ConfigureTracing)
            .WithMetrics(ConfigureMetrics);

        return services;
    }

    private static void ConfigureResource(ResourceBuilder resource) => resource.AddService(
            serviceName: "MCPHub",
            serviceVersion: "1.0.0",
            serviceInstanceId: Environment.MachineName);

    private static void ConfigureTracing(TracerProviderBuilder tracing) => tracing
            .AddAspNetCoreInstrumentation(options => {
                options.RecordException = true;
                options.EnrichWithHttpRequest = (activity, request) => {
                    activity.SetTag("http.request.user_agent", request.Headers.UserAgent.ToString());
                    activity.SetTag("http.request.content_length", request.ContentLength);
                };
                options.EnrichWithHttpResponse = (activity, response) => activity.SetTag("http.response.content_length", response.ContentLength);
                options.Filter = (httpContext) =>                     // Filter out health check requests
                    !httpContext.Request.Path.Value?.Contains("/health") ?? true;
            })
            .AddHttpClientInstrumentation(options => {
                options.RecordException = true;
                options.EnrichWithHttpRequestMessage = (activity, request) => {
                    activity.SetTag("http.request.method", request.Method.Method);
                    activity.SetTag("http.request.uri", request.RequestUri?.ToString());
                };
                options.EnrichWithHttpResponseMessage = (activity, response) => {
                    activity.SetTag("http.response.status_code", (int)response.StatusCode);
                    activity.SetTag("http.response.content_length", response.Content.Headers.ContentLength);
                };
            })
            .AddEntityFrameworkCoreInstrumentation(options => {
                options.SetDbStatementForText = true;
                options.SetDbStatementForStoredProcedure = true;
                options.EnrichWithIDbCommand = (activity, command) => {
                    activity.SetTag("db.command.timeout", command.CommandTimeout);
                    activity.SetTag("db.command.type", command.CommandType);
                };
            })
            .AddSource("MCPHub.*")
            .SetSampler(new TraceIdRatioBasedSampler(1.0))
            .AddConsoleExporter()
            .AddOtlpExporter();

    private static void ConfigureMetrics(MeterProviderBuilder metrics) => metrics
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddRuntimeInstrumentation()
            .AddMeter("MCPHub.*")
            .AddConsoleExporter()
            .AddOtlpExporter();
}

/// <summary>
/// Custom activity source for application-specific tracing
/// </summary>
public static class ActivitySources {
    public static readonly ActivitySource Main = new("MCPHub.Main");
    public static readonly ActivitySource Database = new("MCPHub.Database");
    public static readonly ActivitySource Cache = new("MCPHub.Cache");
    public static readonly ActivitySource Search = new("MCPHub.Search");
    public static readonly ActivitySource MessageQueue = new("MCPHub.MessageQueue");
    public static readonly ActivitySource Security = new("MCPHub.Security");
}

/// <summary>
/// Custom meters for application-specific metrics
/// </summary>
public static class Meters {
    public static readonly Meter Main = new("MCPHub.Main");
    public static readonly Meter Database = new("MCPHub.Database");
    public static readonly Meter Cache = new("MCPHub.Cache");
    public static readonly Meter Search = new("MCPHub.Search");
    public static readonly Meter MessageQueue = new("MCPHub.MessageQueue");
    public static readonly Meter Security = new("MCPHub.Security");
}