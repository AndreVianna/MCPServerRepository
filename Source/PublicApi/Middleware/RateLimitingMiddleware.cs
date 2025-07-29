using System.Net;
using System.Security.Claims;

using MCPHub.Common.Services;
using MCPHub.PublicApi.Configuration;

using Microsoft.Extensions.Options;

namespace MCPHub.PublicApi.Middleware;

/// <summary>
/// Middleware for enforcing API rate limiting
/// </summary>
public class RateLimitingMiddleware(
    RequestDelegate next,
    IRateLimitingService rateLimitingService,
    IOptions<RateLimitingOptions> options,
    ILogger<RateLimitingMiddleware> logger) {
    private readonly RequestDelegate _next = next ?? throw new ArgumentNullException(nameof(next));
    private readonly IRateLimitingService _rateLimitingService = rateLimitingService ?? throw new ArgumentNullException(nameof(rateLimitingService));
    private readonly RateLimitingOptions _options = options.Value ?? throw new ArgumentNullException(nameof(options));
    private readonly ILogger<RateLimitingMiddleware> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task InvokeAsync(HttpContext context) {
        // Skip rate limiting if disabled
        if (!_options.Enabled) {
            await _next(context);
            return;
        }

        try {
            // Skip rate limiting for exempt IPs
            var clientIp = GetClientIpAddress(context);
            if (_options.IpWhitelist.Contains(clientIp)) {
                await _next(context);
                return;
            }

            // Skip rate limiting for exempt user roles
            if (context.User.Identity?.IsAuthenticated == true) {
                var userRoles = context.User.Claims
                    .Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value);

                if (userRoles.Any(_options.ExemptRoles.Contains)) {
                    await _next(context);
                    return;
                }
            }

            // Determine rate limiting policy and identifier
            var (policy, identifier) = DetermineRateLimitPolicy(context, clientIp);

            // Check rate limit
            var rateLimitResult = await _rateLimitingService.CheckRateLimitAsync(identifier, policy);

            // Add rate limit headers
            AddRateLimitHeaders(context.Response, rateLimitResult);

            if (!rateLimitResult.IsAllowed) {
                // Rate limit exceeded
                _logger.LogWarning("Rate limit exceeded for {Identifier} on policy {Policy}", identifier, policy);

                context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                context.Response.Headers["Retry-After"] = ((int)rateLimitResult.ResetTime.TotalSeconds).ToString();

                await context.Response.WriteAsync("Rate limit exceeded. Please try again later.");
                return;
            }

            // Record the request
            await _rateLimitingService.RecordRequestAsync(identifier, policy);

            // Continue to next middleware
            await _next(context);
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error in rate limiting middleware");
            // Continue processing on error to avoid blocking requests due to rate limiting issues
            await _next(context);
        }
    }

    private (string policy, string identifier) DetermineRateLimitPolicy(HttpContext context, string clientIp) {
        var path = context.Request.Path.Value?.ToLowerInvariant() ?? string.Empty;
        var method = context.Request.Method.ToUpperInvariant();

        // Authentication endpoints (login, refresh, logout)
        if (path.Contains("/auth/") || path.Contains("/authentication/")) {
            return ("Authentication", clientIp);
        }

        // Search endpoints
        if (path.Contains("/search") || (path.Contains("/packages") && method == "GET" && context.Request.Query.ContainsKey("q"))) {
            return ("Search", clientIp);
        }

        // Package CRUD operations (POST, PUT, DELETE on packages)
        if (path.Contains("/packages") && (method is "POST" or "PUT" or "DELETE" or "PATCH")) {
            var userId = GetUserId(context);
            if (userId != null) {
                return ("PackageCrud", $"user:{userId}");
            }
            // Fall back to IP-based limiting if user not authenticated
            return ("PackageCrud", clientIp);
        }

        // Public read operations (GET requests for packages, servers, etc.)
        if (method == "GET" && (path.Contains("/packages") || path.Contains("/servers") || path.Contains("/public"))) {
            return ("PublicRead", clientIp);
        }

        // Default/Global policy
        return ("Global", clientIp);
    }

    private string GetClientIpAddress(HttpContext context) {
        // Check for forwarded IP first (in case of proxy/load balancer)
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor)) {
            // Take the first IP if multiple are present
            var firstIp = forwardedFor.Split(',')[0].Trim();
            if (IPAddress.TryParse(firstIp, out _)) {
                return firstIp;
            }
        }

        // Check for real IP header
        var realIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(realIp) && IPAddress.TryParse(realIp, out _)) {
            return realIp;
        }

        // Fall back to connection remote IP
        return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }

    private string? GetUserId(HttpContext context) {
        if (context.User.Identity?.IsAuthenticated != true)
            return null;

        // Try different claim types for user ID
        return context.User.FindFirst("sub")?.Value ??
               context.User.FindFirst("id")?.Value ??
               context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

    private void AddRateLimitHeaders(HttpResponse response, RateLimitResult rateLimitResult) {
        response.Headers["X-RateLimit-Limit"] = rateLimitResult.Policy.RequestLimit.ToString();
        response.Headers["X-RateLimit-Remaining"] = rateLimitResult.RequestsRemaining.ToString();
        response.Headers["X-RateLimit-Reset"] = DateTimeOffset.UtcNow.Add(rateLimitResult.ResetTime).ToUnixTimeSeconds().ToString();
        response.Headers["X-RateLimit-Policy"] = GetPolicyDisplayName(rateLimitResult.Policy);
    }

    private string GetPolicyDisplayName(RateLimitPolicy policy) {
        // Create a readable policy description
        var windowUnit = policy.WindowDuration.TotalMinutes >= 1 ? "minute" : "second";
        var windowValue = policy.WindowDuration.TotalMinutes >= 1
            ? policy.WindowDuration.TotalMinutes
            : policy.WindowDuration.TotalSeconds;

        return $"{policy.RequestLimit} requests per {windowValue} {windowUnit}(s)";
    }
}

/// <summary>
/// Extension methods for configuring rate limiting middleware
/// </summary>
public static class RateLimitingMiddlewareExtensions {
    /// <summary>
    /// Adds rate limiting middleware to the application pipeline
    /// </summary>
    /// <param name="app">The application builder</param>
    /// <returns>The application builder</returns>
    public static IApplicationBuilder UseRateLimiting(this IApplicationBuilder app) => app.UseMiddleware<RateLimitingMiddleware>();
}