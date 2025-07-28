using System.ComponentModel.DataAnnotations;

namespace MCPHub.PublicApi.Configuration;

/// <summary>
/// Configuration options for JWT authentication
/// </summary>
public class JwtOptions
{
    public const string SectionName = "Jwt";

    /// <summary>
    /// Secret key for signing JWT tokens (minimum 512 bits)
    /// </summary>
    [Required]
    [MinLength(64, ErrorMessage = "Secret key must be at least 64 characters long")]
    public required string SecretKey { get; init; }

    /// <summary>
    /// Token issuer
    /// </summary>
    [Required]
    public required string Issuer { get; init; }

    /// <summary>
    /// Token audience
    /// </summary>
    [Required]
    public required string Audience { get; init; }

    /// <summary>
    /// Access token expiration time in minutes (default: 15 minutes)
    /// </summary>
    [Range(1, 1440, ErrorMessage = "Access token expiration must be between 1 and 1440 minutes")]
    public int AccessTokenExpirationMinutes { get; init; } = 15;

    /// <summary>
    /// Refresh token expiration time in days (default: 7 days)
    /// </summary>
    [Range(1, 365, ErrorMessage = "Refresh token expiration must be between 1 and 365 days")]
    public int RefreshTokenExpirationDays { get; init; } = 7;

    /// <summary>
    /// Clock skew tolerance in minutes (default: 5 minutes)
    /// </summary>
    [Range(0, 60, ErrorMessage = "Clock skew must be between 0 and 60 minutes")]
    public int ClockSkewMinutes { get; init; } = 5;

    /// <summary>
    /// Whether to validate the token issuer
    /// </summary>
    public bool ValidateIssuer { get; init; } = true;

    /// <summary>
    /// Whether to validate the token audience
    /// </summary>
    public bool ValidateAudience { get; init; } = true;

    /// <summary>
    /// Whether to validate token lifetime
    /// </summary>
    public bool ValidateLifetime { get; init; } = true;

    /// <summary>
    /// Whether to validate the signing key
    /// </summary>
    public bool ValidateIssuerSigningKey { get; init; } = true;
}