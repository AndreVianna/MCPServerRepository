namespace MCPHub.AuthenticationService.Services;

/// <summary>
/// Implementation of authentication service
/// Skeleton implementation - actual logic will be implemented when first consumer requires it
/// </summary>
public class AuthenticationService : IAuthenticationService {
    /// <inheritdoc />
    public Task<AuthenticationResult> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("Authentication login logic will be implemented when first consumer requires it");

    /// <inheritdoc />
    public Task<RegistrationResult> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("User registration logic will be implemented when first consumer requires it");

    /// <inheritdoc />
    public Task<TokenResult> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("Token refresh logic will be implemented when first consumer requires it");

    /// <inheritdoc />
    public Task<LogoutResult> LogoutAsync(Guid userId, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("User logout logic will be implemented when first consumer requires it");
}