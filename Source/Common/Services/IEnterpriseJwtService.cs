using System.Security.Claims;

namespace MCPHub.Common.Services;

/// <summary>
/// Enterprise-grade JWT token service with advanced security features
/// Supports: Basic JWT → Token Blacklisting → Automatic Rotation → Enterprise Key Management
/// </summary>
public interface IEnterpriseJwtService {
    /// <summary>
    /// Generates a JWT access token with scoped permissions
    /// </summary>
    /// <param name="tokenRequest">Token generation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>JWT token result</returns>
    Task<JwtTokenResult> GenerateAccessTokenAsync(JwtTokenRequest tokenRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a refresh token with enhanced security
    /// </summary>
    /// <param name="refreshTokenRequest">Refresh token request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Refresh token result</returns>
    Task<RefreshTokenResult> GenerateRefreshTokenAsync(RefreshTokenRequest refreshTokenRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates a JWT token with comprehensive security checks
    /// </summary>
    /// <param name="token">JWT token to validate</param>
    /// <param name="validationOptions">Optional validation options</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Token validation result</returns>
    Task<JwtValidationResult> ValidateTokenAsync(string token, JwtValidationOptions? validationOptions = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates a refresh token and checks for revocation
    /// </summary>
    /// <param name="refreshToken">Refresh token to validate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Refresh token validation result</returns>
    Task<RefreshTokenValidationResult> ValidateRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokes a specific token (access or refresh)
    /// </summary>
    /// <param name="revokeRequest">Token revocation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Token revocation result</returns>
    Task<TokenRevocationResult> RevokeTokenAsync(TokenRevocationRequest revokeRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokes all tokens for a specific user
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="reason">Revocation reason</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Bulk revocation result</returns>
    Task<BulkTokenRevocationResult> RevokeAllUserTokensAsync(Guid userId, string reason, CancellationToken cancellationToken = default);

    /// <summary>
    /// Rotates tokens proactively before expiration
    /// </summary>
    /// <param name="rotationRequest">Token rotation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Token rotation result</returns>
    Task<TokenRotationResult> RotateTokensAsync(TokenRotationRequest rotationRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets token information and metadata
    /// </summary>
    /// <param name="token">JWT token</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Token information</returns>
    Task<JwtTokenInfo> GetTokenInfoAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a token is blacklisted
    /// </summary>
    /// <param name="tokenId">Token identifier (jti claim)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if token is blacklisted</returns>
    Task<bool> IsTokenBlacklistedAsync(string tokenId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a token to the blacklist
    /// </summary>
    /// <param name="blacklistRequest">Blacklist request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Blacklist result</returns>
    Task<TokenBlacklistResult> BlacklistTokenAsync(TokenBlacklistRequest blacklistRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets active tokens for a user
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="tokenType">Optional token type filter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of active tokens</returns>
    Task<IReadOnlyList<ActiveTokenInfo>> GetActiveTokensAsync(Guid userId, JwtTokenType? tokenType = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates token scopes for existing token
    /// </summary>
    /// <param name="tokenId">Token identifier</param>
    /// <param name="newScopes">New scopes</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Scope update result</returns>
    Task<TokenScopeUpdateResult> UpdateTokenScopesAsync(string tokenId, IEnumerable<string> newScopes, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets token usage statistics
    /// </summary>
    /// <param name="userId">Optional user ID to filter statistics</param>
    /// <param name="timeRange">Optional time range</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Token usage statistics</returns>
    Task<JwtTokenStatistics> GetTokenStatisticsAsync(Guid? userId = null, DateTimeRange? timeRange = null, CancellationToken cancellationToken = default);
}

/// <summary>
/// JWT token generation request
/// </summary>
public record JwtTokenRequest(
    Guid UserId,
    string Username,
    IEnumerable<string> Roles,
    IEnumerable<string> Scopes,
    IDictionary<string, object>? AdditionalClaims = null,
    TimeSpan? CustomExpiration = null,
    string? Audience = null,
    string? Subject = null,
    bool AllowRefresh = true);

/// <summary>
/// JWT token result
/// </summary>
public record JwtTokenResult(
    string AccessToken,
    string TokenType,
    DateTimeOffset ExpiresAt,
    IEnumerable<string> Scopes,
    string? RefreshToken = null,
    bool Success = true,
    string? ErrorMessage = null);

/// <summary>
/// Refresh token generation request
/// </summary>
public record RefreshTokenRequest(
    Guid UserId,
    string? ExistingRefreshToken = null,
    TimeSpan? CustomExpiration = null,
    bool RevokeExisting = true);

/// <summary>
/// Refresh token result
/// </summary>
public record RefreshTokenResult(
    string RefreshToken,
    DateTimeOffset ExpiresAt,
    bool Success = true,
    string? ErrorMessage = null);

/// <summary>
/// JWT validation options
/// </summary>
public record JwtValidationOptions(
    bool ValidateExpiration = true,
    bool ValidateIssuer = true,
    bool ValidateAudience = true,
    bool ValidateSignature = true,
    bool CheckBlacklist = true,
    bool RequireHttps = true,
    IEnumerable<string>? RequiredScopes = null,
    IEnumerable<string>? RequiredRoles = null);

/// <summary>
/// JWT validation result
/// </summary>
public record JwtValidationResult(
    bool IsValid,
    ClaimsPrincipal? Principal = null,
    IEnumerable<string>? ValidationErrors = null,
    JwtTokenInfo? TokenInfo = null,
    DateTimeOffset? ExpiresAt = null,
    IEnumerable<string>? Scopes = null);

/// <summary>
/// Refresh token validation result
/// </summary>
public record RefreshTokenValidationResult(
    bool IsValid,
    Guid? UserId = null,
    DateTimeOffset? ExpiresAt = null,
    bool IsRevoked = false,
    string? ErrorMessage = null);

/// <summary>
/// Token revocation request
/// </summary>
public record TokenRevocationRequest(
    string Token,
    JwtTokenType TokenType,
    string Reason,
    Guid? RevokedBy = null);

/// <summary>
/// Token revocation result
/// </summary>
public record TokenRevocationResult(
    bool Success,
    string? TokenId = null,
    DateTimeOffset? RevokedAt = null,
    string? ErrorMessage = null);

/// <summary>
/// Bulk token revocation result
/// </summary>
public record BulkTokenRevocationResult(
    bool Success,
    int TokensRevoked,
    int TotalTokens,
    IEnumerable<string>? FailedTokens = null,
    string? ErrorMessage = null);

/// <summary>
/// Token rotation request
/// </summary>
public record TokenRotationRequest(
    string CurrentRefreshToken,
    bool RevokeOldTokens = true,
    TimeSpan? NewTokenLifetime = null);

/// <summary>
/// Token rotation result
/// </summary>
public record TokenRotationResult(
    bool Success,
    string? NewAccessToken = null,
    string? NewRefreshToken = null,
    DateTimeOffset? ExpiresAt = null,
    string? ErrorMessage = null);

/// <summary>
/// JWT token information
/// </summary>
public record JwtTokenInfo(
    string TokenId,
    Guid UserId,
    string Username,
    IEnumerable<string> Roles,
    IEnumerable<string> Scopes,
    DateTimeOffset IssuedAt,
    DateTimeOffset ExpiresAt,
    string? Audience = null,
    string? Subject = null,
    bool IsExpired = false,
    bool IsBlacklisted = false);

/// <summary>
/// Token blacklist request
/// </summary>
public record TokenBlacklistRequest(
    string TokenId,
    string Reason,
    Guid? BlacklistedBy = null,
    DateTimeOffset? ExpiresAt = null);

/// <summary>
/// Token blacklist result
/// </summary>
public record TokenBlacklistResult(
    bool Success,
    DateTimeOffset? BlacklistedAt = null,
    string? ErrorMessage = null);

/// <summary>
/// Active token information
/// </summary>
public record ActiveTokenInfo(
    string TokenId,
    JwtTokenType TokenType,
    DateTimeOffset IssuedAt,
    DateTimeOffset ExpiresAt,
    IEnumerable<string> Scopes,
    string? ClientInfo = null,
    string? IpAddress = null,
    bool IsActive = true);

/// <summary>
/// Token scope update result
/// </summary>
public record TokenScopeUpdateResult(
    bool Success,
    IEnumerable<string>? UpdatedScopes = null,
    string? ErrorMessage = null);

/// <summary>
/// JWT token statistics
/// </summary>
public record JwtTokenStatistics(
    DateTimeRange TimeRange,
    int TotalTokensIssued,
    int ActiveTokens,
    int RevokedTokens,
    int ExpiredTokens,
    IDictionary<JwtTokenType, int> TokensByType,
    IDictionary<string, int> TokensByScope,
    double AverageTokenLifetime,
    int TokensRotated,
    int TokensBlacklisted);

/// <summary>
/// JWT token types
/// </summary>
public enum JwtTokenType {
    AccessToken,
    RefreshToken,
    IdToken,
}

/// <summary>
/// Token scopes for different access levels
/// </summary>
public static class JwtScopes {
    public const string ReadPackages = "packages:read";
    public const string WritePackages = "packages:write";
    public const string DeletePackages = "packages:delete";
    public const string ReadServers = "servers:read";
    public const string WriteServers = "servers:write";
    public const string DeleteServers = "servers:delete";
    public const string ReadUsers = "users:read";
    public const string WriteUsers = "users:write";
    public const string AdminAccess = "admin:full";
    public const string ApiKeyManagement = "apikeys:manage";
    public const string SecurityManagement = "security:manage";
    public const string ComplianceAccess = "compliance:read";
    public const string AuditAccess = "audit:read";
}

/// <summary>
/// Standard JWT claims
/// </summary>
public static class JwtClaimTypes {
    public const string UserId = "uid";
    public const string Username = "username";
    public const string Email = "email";
    public const string Roles = "roles";
    public const string Scopes = "scopes";
    public const string ApiKeyId = "api_key_id";
    public const string ClientId = "client_id";
    public const string SessionId = "session_id";
    public const string TrustTier = "trust_tier";
    public const string OrganizationId = "org_id";
}