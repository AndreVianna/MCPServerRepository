using System.ComponentModel.DataAnnotations;

namespace MCPHub.PublicApi.Configuration;

/// <summary>
/// Configuration options for rate limiting
/// </summary>
public class RateLimitingOptions {
    public const string SectionName = "RateLimiting";

    /// <summary>
    /// Authentication endpoint rate limiting configuration (5 req/min per IP)
    /// </summary>
    public EndpointRateLimitConfig Authentication { get; init; } = new() {
        PermitLimit = 5,
        WindowMinutes = 1,
        QueueLimit = 0,
        PerIpLimiting = true,
        PerUserLimiting = false
    };

    /// <summary>
    /// Search endpoint rate limiting configuration (100 req/min per IP)
    /// </summary>
    public EndpointRateLimitConfig Search { get; init; } = new() {
        PermitLimit = 100,
        WindowMinutes = 1,
        QueueLimit = 10,
        PerIpLimiting = true,
        PerUserLimiting = false
    };

    /// <summary>
    /// Package CRUD operation rate limiting (50 req/min per authenticated user)
    /// </summary>
    public EndpointRateLimitConfig PackageCrud { get; init; } = new() {
        PermitLimit = 50,
        WindowMinutes = 1,
        QueueLimit = 5,
        PerIpLimiting = false,
        PerUserLimiting = true
    };

    /// <summary>
    /// Public read operation rate limiting (200 req/min per IP)
    /// </summary>
    public EndpointRateLimitConfig PublicRead { get; init; } = new() {
        PermitLimit = 200,
        WindowMinutes = 1,
        QueueLimit = 20,
        PerIpLimiting = true,
        PerUserLimiting = false
    };

    /// <summary>
    /// Global fallback rate limiting configuration
    /// </summary>
    public EndpointRateLimitConfig Global { get; init; } = new() {
        PermitLimit = 1000,
        WindowMinutes = 1,
        QueueLimit = 100,
        PerIpLimiting = true,
        PerUserLimiting = false
    };

    /// <summary>
    /// IP addresses exempt from rate limiting (trusted sources)
    /// </summary>
    public IList<string> IpWhitelist { get; init; } = new List<string>();

    /// <summary>
    /// User roles exempt from rate limiting
    /// </summary>
    public IList<string> ExemptRoles { get; init; } = new List<string> { "Admin", "System" };

    /// <summary>
    /// Enable rate limiting globally
    /// </summary>
    public bool Enabled { get; init; } = true;

    /// <summary>
    /// Redis connection string for distributed rate limiting
    /// </summary>
    public string? RedisConnectionString { get; init; }

    /// <summary>
    /// Use in-memory rate limiting (for development)
    /// </summary>
    public bool UseInMemory { get; init; } = true;
}

/// <summary>
/// Rate limiting configuration for specific endpoint categories
/// </summary>
public class EndpointRateLimitConfig {
    /// <summary>
    /// Maximum number of requests per window
    /// </summary>
    [Range(1, 10000, ErrorMessage = "Permit limit must be between 1 and 10000")]
    public int PermitLimit { get; init; } = 100;

    /// <summary>
    /// Rate limiting window in minutes
    /// </summary>
    [Range(1, 60, ErrorMessage = "Window must be between 1 and 60 minutes")]
    public int WindowMinutes { get; init; } = 1;

    /// <summary>
    /// Number of requests to queue when limit is exceeded
    /// </summary>
    [Range(0, 1000, ErrorMessage = "Queue limit must be between 0 and 1000")]
    public int QueueLimit { get; init; } = 0;

    /// <summary>
    /// Apply rate limiting per IP address
    /// </summary>
    public bool PerIpLimiting { get; init; } = true;

    /// <summary>
    /// Apply rate limiting per authenticated user
    /// </summary>
    public bool PerUserLimiting { get; init; } = false;

    /// <summary>
    /// Rate limiting strategy to use
    /// </summary>
    public string Strategy { get; init; } = "FixedWindow";

    /// <summary>
    /// Custom policy name for this configuration
    /// </summary>
    public string? PolicyName { get; init; }
}