using System.Security.Claims;

using MCPHub.Domain.Entities;

namespace MCPHub.PublicApi.Services;

/// <summary>
/// Interface for JWT token operations in PublicApi
/// </summary>
public interface IJwtService {
    /// <summary>
    /// Generates a JWT access token for the specified user
    /// </summary>
    /// <param name="user">User to generate token for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>JWT access token</returns>
    Task<string> GenerateAccessTokenAsync(ApplicationUser user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a refresh token for the specified user
    /// </summary>
    /// <param name="user">User to generate refresh token for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Refresh token</returns>
    Task<string> GenerateRefreshTokenAsync(ApplicationUser user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates a JWT access token and returns the claims principal
    /// </summary>
    /// <param name="token">JWT token to validate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Claims principal if valid, null otherwise</returns>
    Task<ClaimsPrincipal?> ValidateAccessTokenAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates a refresh token and returns the associated user ID
    /// </summary>
    /// <param name="refreshToken">Refresh token to validate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User ID if valid, null otherwise</returns>
    Task<Guid?> ValidateRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokes a refresh token by adding it to blacklist
    /// </summary>
    /// <param name="refreshToken">Refresh token to revoke</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if revoked successfully</returns>
    Task<bool> RevokeRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the user ID from a JWT token without full validation
    /// </summary>
    /// <param name="token">JWT token</param>
    /// <returns>User ID if found in token claims</returns>
    Guid? GetUserIdFromToken(string token);

    /// <summary>
    /// Gets the token expiration time
    /// </summary>
    /// <param name="token">JWT token</param>
    /// <returns>Token expiration time if found</returns>
    DateTime? GetTokenExpiration(string token);
}