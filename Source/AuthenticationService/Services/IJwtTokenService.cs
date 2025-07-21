using MCPHub.Domain.Entities;
using System.Security.Claims;

namespace MCPHub.AuthenticationService.Services;

/// <summary>
/// Interface for JWT token operations
/// </summary>
public interface IJwtTokenService {
    /// <summary>
    /// Generates a JWT access token for the specified user
    /// </summary>
    /// <param name="user">User to generate token for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>JWT token string</returns>
    Task<string> GenerateAccessTokenAsync(ApplicationUser user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a refresh token for the specified user
    /// </summary>
    /// <param name="user">User to generate refresh token for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Refresh token string</returns>
    Task<string> GenerateRefreshTokenAsync(ApplicationUser user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates and extracts claims from a JWT token
    /// </summary>
    /// <param name="token">JWT token to validate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Claims principal if token is valid, null otherwise</returns>
    Task<ClaimsPrincipal?> ValidateTokenAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates a refresh token and returns associated user ID
    /// </summary>
    /// <param name="refreshToken">Refresh token to validate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User ID if token is valid, null otherwise</returns>
    Task<Guid?> ValidateRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokes a refresh token
    /// </summary>
    /// <param name="refreshToken">Refresh token to revoke</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if token was revoked, false otherwise</returns>
    Task<bool> RevokeRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
}