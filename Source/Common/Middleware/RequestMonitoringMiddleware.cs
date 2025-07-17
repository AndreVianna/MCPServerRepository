using Common.Services;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace Common.Middleware;

/// <summary>
/// Middleware for monitoring HTTP requests and responses
/// </summary>
public class RequestMonitoringMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IMonitoringService _monitoringService;
    private readonly ILogger<RequestMonitoringMiddleware> _logger;

    public RequestMonitoringMiddleware(
        RequestDelegate next,
        IMonitoringService monitoringService,
        ILogger<RequestMonitoringMiddleware> logger)
    {
        _next = next;
        _monitoringService = monitoringService;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var path = context.Request.Path.Value ?? "/";
        var method = context.Request.Method;
        
        try
        {
            // Add correlation ID if not present
            if (!context.Request.Headers.ContainsKey("X-Correlation-ID"))
            {
                context.Request.Headers.Add("X-Correlation-ID", Guid.NewGuid().ToString());
            }

            var correlationId = context.Request.Headers["X-Correlation-ID"].ToString();
            
            // Add correlation ID to response headers
            context.Response.Headers.Add("X-Correlation-ID", correlationId);

            // Log request start
            _logger.LogInformation("Request started: {Method} {Path} - CorrelationId: {CorrelationId}", 
                method, path, correlationId);

            await _next(context);

            stopwatch.Stop();
            var statusCode = context.Response.StatusCode;

            // Record metrics
            _monitoringService.RecordHttpRequest(method, path, statusCode, stopwatch.Elapsed);

            // Log request completion
            _logger.LogInformation("Request completed: {Method} {Path} - Status: {StatusCode} - Duration: {Duration}ms - CorrelationId: {CorrelationId}", 
                method, path, statusCode, stopwatch.ElapsedMilliseconds, correlationId);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            
            // Record error metrics
            _monitoringService.RecordError("HTTP", ex.GetType().Name, ex.Message);
            _monitoringService.RecordHttpRequest(method, path, 500, stopwatch.Elapsed);

            // Log error
            _logger.LogError(ex, "Request failed: {Method} {Path} - Duration: {Duration}ms", 
                method, path, stopwatch.ElapsedMilliseconds);

            throw;
        }
    }
}

/// <summary>
/// Extension methods for adding request monitoring middleware
/// </summary>
public static class RequestMonitoringExtensions
{
    /// <summary>
    /// Adds request monitoring middleware to the pipeline
    /// </summary>
    public static IApplicationBuilder UseRequestMonitoring(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestMonitoringMiddleware>();
    }
}