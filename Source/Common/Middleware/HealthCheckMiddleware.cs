using System.Text.Json;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Common.Middleware;

/// <summary>
/// Custom health check middleware for enhanced monitoring
/// </summary>
public class HealthCheckMiddleware(
    RequestDelegate next,
    HealthCheckService healthCheckService,
    ILogger<HealthCheckMiddleware> logger) {
    private readonly RequestDelegate _next = next;
    private readonly HealthCheckService _healthCheckService = healthCheckService;
    private readonly ILogger<HealthCheckMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context) {
        if (!context.Request.Path.StartsWithSegments("/health")) {
            await _next(context);
            return;
        }

        try {
            var healthReport = await _healthCheckService.CheckHealthAsync();

            var response = new HealthCheckResponse {
                Status = healthReport.Status.ToString(),
                Duration = healthReport.TotalDuration,
                Timestamp = DateTime.UtcNow,
                Results = healthReport.Entries.ToDictionary(
                    kvp => kvp.Key,
                    kvp => new HealthCheckResult {
                        Status = kvp.Value.Status.ToString(),
                        Duration = kvp.Value.Duration,
                        Description = kvp.Value.Description,
                        Tags = kvp.Value.Tags,
                        Exception = kvp.Value.Exception?.Message,
                        Data = kvp.Value.Data
                    })
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = healthReport.Status switch {
                HealthStatus.Healthy => StatusCodes.Status200OK,
                HealthStatus.Degraded => StatusCodes.Status200OK,
                HealthStatus.Unhealthy => StatusCodes.Status503ServiceUnavailable,
                _ => StatusCodes.Status500InternalServerError
            };

            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            });

            await context.Response.WriteAsync(json);

            // Log health check results
            if (healthReport.Status == HealthStatus.Unhealthy) {
                _logger.LogError("Health check failed with status {Status}. Duration: {Duration}ms",
                    healthReport.Status, healthReport.TotalDuration.TotalMilliseconds);
            }
            else if (healthReport.Status == HealthStatus.Degraded) {
                _logger.LogWarning("Health check degraded with status {Status}. Duration: {Duration}ms",
                    healthReport.Status, healthReport.TotalDuration.TotalMilliseconds);
            }
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error executing health checks");

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsync("Health check error");
        }
    }
}

public class HealthCheckResponse {
    public string Status { get; set; } = string.Empty;
    public TimeSpan Duration { get; set; }
    public DateTime Timestamp { get; set; }
    public Dictionary<string, HealthCheckResult> Results { get; set; } = [];
}

public class HealthCheckResult {
    public string Status { get; set; } = string.Empty;
    public TimeSpan Duration { get; set; }
    public string? Description { get; set; }
    public IEnumerable<string> Tags { get; set; } = [];
    public string? Exception { get; set; }
    public IReadOnlyDictionary<string, object> Data { get; set; } = new Dictionary<string, object>();
}