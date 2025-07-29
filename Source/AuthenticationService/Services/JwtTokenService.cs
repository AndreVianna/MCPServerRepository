using System.Security.Claims;

using MCPHub.Domain.Entities;

namespace MCPHub.AuthenticationService.Services;

/// <summary>
/// Implementation of JWT token service
/// Skeleton implementation - actual logic will be implemented when first consumer requires it
/// </summary>
public class JwtTokenService : IJwtTokenService {
    /// <inheritdoc />
    public Task<string> GenerateAccessTokenAsync(ApplicationUser user, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("JWT access token generation will be implemented when first consumer requires it");

    /// <inheritdoc />
    public Task<string> GenerateRefreshTokenAsync(ApplicationUser user, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("JWT refresh token generation will be implemented when first consumer requires it");

    /// <inheritdoc />
    public Task<ClaimsPrincipal?> ValidateTokenAsync(string token, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("JWT token validation will be implemented when first consumer requires it");

    /// <inheritdoc />
    public Task<Guid?> ValidateRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("JWT refresh token validation will be implemented when first consumer requires it");

    /// <inheritdoc />
    public Task<bool> RevokeRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("JWT token revocation will be implemented when first consumer requires it");
}