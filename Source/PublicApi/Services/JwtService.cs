using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

using MCPHub.Domain.Entities;
using MCPHub.PublicApi.Configuration;

using Microsoft.Extensions.Options;

namespace MCPHub.PublicApi.Services;

/// <summary>
/// JWT service implementation for PublicApi
/// </summary>
public class JwtService : IJwtService {
    private readonly JwtOptions _jwtOptions;
    private readonly ILogger<JwtService> _logger;
    private readonly SymmetricSecurityKey _signingKey;
    private readonly HashSet<string> _revokedTokens = []; // Simple in-memory blacklist for demo

    public JwtService(IOptions<JwtOptions> jwtOptions, ILogger<JwtService> logger) {
        _jwtOptions = jwtOptions?.Value ?? throw new ArgumentNullException(nameof(jwtOptions));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
    }

    /// <inheritdoc />
    public Task<string> GenerateAccessTokenAsync(ApplicationUser user, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(user);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName ?? string.Empty),
            new(ClaimTypes.Email, user.Email ?? string.Empty)
        };

        if (!string.IsNullOrEmpty(user.DisplayName)) {
            claims.Add(new Claim("display_name", user.DisplayName));
        }

        if (user.IsPublisher) {
            claims.Add(new Claim("is_publisher", "true"));
        }

        var expires = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpirationMinutes);
        var credentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: credentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        _logger.LogDebug("Generated access token for user {UserId} with expiration {Expires}", user.Id, expires);

        return Task.FromResult(tokenString);
    }

    /// <inheritdoc />
    public Task<string> GenerateRefreshTokenAsync(ApplicationUser user, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(user);

        // Generate a cryptographically random refresh token
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);

        var refreshToken = Convert.ToBase64String(randomBytes);

        _logger.LogDebug("Generated refresh token for user {UserId}", user.Id);

        return Task.FromResult(refreshToken);
    }

    /// <inheritdoc />
    public Task<ClaimsPrincipal?> ValidateAccessTokenAsync(string token, CancellationToken cancellationToken = default) {
        if (string.IsNullOrWhiteSpace(token)) {
            return Task.FromResult<ClaimsPrincipal?>(null);
        }

        try {
            var tokenHandler = new JwtSecurityTokenHandler();

            var validationParameters = new TokenValidationParameters {
                ValidateIssuer = _jwtOptions.ValidateIssuer,
                ValidateAudience = _jwtOptions.ValidateAudience,
                ValidateLifetime = _jwtOptions.ValidateLifetime,
                ValidateIssuerSigningKey = _jwtOptions.ValidateIssuerSigningKey,
                ValidIssuer = _jwtOptions.Issuer,
                ValidAudience = _jwtOptions.Audience,
                IssuerSigningKey = _signingKey,
                ClockSkew = TimeSpan.FromMinutes(_jwtOptions.ClockSkewMinutes)
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

            // Check if token is revoked
            var jti = principal.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
            if (!string.IsNullOrEmpty(jti) && _revokedTokens.Contains(jti)) {
                _logger.LogWarning("Token validation failed - token is revoked (JTI: {Jti})", jti);
                return Task.FromResult<ClaimsPrincipal?>(null);
            }

            _logger.LogDebug("Token validation successful");
            return Task.FromResult<ClaimsPrincipal?>(principal);
        }
        catch (SecurityTokenException ex) {
            _logger.LogWarning("Token validation failed: {Error}", ex.Message);
            return Task.FromResult<ClaimsPrincipal?>(null);
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Unexpected error during token validation");
            return Task.FromResult<ClaimsPrincipal?>(null);
        }
    }

    /// <inheritdoc />
    public Task<Guid?> ValidateRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default) {
        if (string.IsNullOrWhiteSpace(refreshToken)) {
            return Task.FromResult<Guid?>(null);
        }

        // For this implementation, we'll use a simple validation
        // In a production system, refresh tokens should be stored in a database with expiration
        try {
            // Check if token is revoked
            if (_revokedTokens.Contains(refreshToken)) {
                _logger.LogWarning("Refresh token validation failed - token is revoked");
                return Task.FromResult<Guid?>(null);
            }

            // For demo purposes, we'll extract user ID from the token structure
            // In production, this should look up the token in a database
            _logger.LogDebug("Refresh token validation successful");

            // This is a simplified implementation - in production you'd store refresh tokens in DB
            // For now, we'll return null to indicate the token is valid but we can't extract user ID
            return Task.FromResult<Guid?>(null);
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error validating refresh token");
            return Task.FromResult<Guid?>(null);
        }
    }

    /// <inheritdoc />
    public Task<bool> RevokeRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default) {
        if (string.IsNullOrWhiteSpace(refreshToken)) {
            return Task.FromResult(false);
        }

        try {
            _revokedTokens.Add(refreshToken);
            _logger.LogDebug("Refresh token revoked successfully");
            return Task.FromResult(true);
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error revoking refresh token");
            return Task.FromResult(false);
        }
    }

    /// <inheritdoc />
    public Guid? GetUserIdFromToken(string token) {
        if (string.IsNullOrWhiteSpace(token)) {
            return null;
        }

        try {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jsonToken = tokenHandler.ReadJwtToken(token);

            var userIdClaim = jsonToken.Claims.FirstOrDefault(c =>
                c.Type == JwtRegisteredClaimNames.Sub ||
                c.Type == ClaimTypes.NameIdentifier);

            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId)) {
                return userId;
            }

            return null;
        }
        catch (Exception ex) {
            _logger.LogWarning(ex, "Error extracting user ID from token");
            return null;
        }
    }

    /// <inheritdoc />
    public DateTime? GetTokenExpiration(string token) {
        if (string.IsNullOrWhiteSpace(token)) {
            return null;
        }

        try {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jsonToken = tokenHandler.ReadJwtToken(token);

            return jsonToken.ValidTo;
        }
        catch (Exception ex) {
            _logger.LogWarning(ex, "Error extracting expiration from token");
            return null;
        }
    }
}