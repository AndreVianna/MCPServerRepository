namespace MCPHub.CommandLineApp.Services;

/// <summary>
/// Interface for managing authentication tokens and validation
/// </summary>
public interface IAuthenticationManager {
    /// <summary>
    /// Authenticates with the specified registry using username and password
    /// </summary>
    /// <param name="registryUrl">The registry URL to authenticate with</param>
    /// <param name="username">Username for authentication</param>
    /// <param name="password">Password for authentication</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Authentication result with token information</returns>
    Task<AuthenticationResult> LoginAsync(string registryUrl, string username, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// Authenticates with the specified registry using an API key
    /// </summary>
    /// <param name="registryUrl">The registry URL to authenticate with</param>
    /// <param name="apiKey">API key for authentication</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Authentication result with token information</returns>
    Task<AuthenticationResult> LoginWithApiKeyAsync(string registryUrl, string apiKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes authentication for the specified registry
    /// </summary>
    /// <param name="registryUrl">The registry URL to logout from</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if logout was successful</returns>
    Task<bool> LogoutAsync(string registryUrl, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the currently authenticated user for the specified registry
    /// </summary>
    /// <param name="registryUrl">The registry URL to check authentication for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Current user information or null if not authenticated</returns>
    Task<CurrentUserInfo?> GetCurrentUserAsync(string registryUrl, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates if the current authentication token is valid and not expired
    /// </summary>
    /// <param name="registryUrl">The registry URL to validate authentication for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Validation result with token status</returns>
    Task<TokenValidationResult> ValidateTokenAsync(string registryUrl, CancellationToken cancellationToken = default);

    /// <summary>
    /// Refreshes the authentication token if possible
    /// </summary>
    /// <param name="registryUrl">The registry URL to refresh token for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Refresh result with new token information</returns>
    Task<TokenRefreshResult> RefreshTokenAsync(string registryUrl, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an authentication header for API requests to the specified registry
    /// </summary>
    /// <param name="registryUrl">The registry URL to get authentication header for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Authentication header value or null if not authenticated</returns>
    Task<string?> GetAuthenticationHeaderAsync(string registryUrl, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists all registries with stored authentication
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of authenticated registry information</returns>
    Task<IEnumerable<AuthenticatedRegistry>> GetAuthenticatedRegistriesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if the user is authenticated with the specified registry
    /// </summary>
    /// <param name="registryUrl">The registry URL to check</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if authenticated</returns>
    Task<bool> IsAuthenticatedAsync(string registryUrl, CancellationToken cancellationToken = default);

    /// <summary>
    /// Clears all stored authentication data
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if cleared successfully</returns>
    Task<bool> ClearAllAuthenticationAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Result of an authentication attempt
/// </summary>
public record AuthenticationResult {
    /// <summary>
    /// Whether authentication was successful
    /// </summary>
    public required bool IsSuccess { get; init; }

    /// <summary>
    /// The authentication token if successful
    /// </summary>
    public string? Token { get; init; }

    /// <summary>
    /// The refresh token if available
    /// </summary>
    public string? RefreshToken { get; init; }

    /// <summary>
    /// When the token expires
    /// </summary>
    public DateTimeOffset? ExpiresAt { get; init; }

    /// <summary>
    /// User information if available
    /// </summary>
    public CurrentUserInfo? UserInfo { get; init; }

    /// <summary>
    /// Error message if authentication failed
    /// </summary>
    public string? ErrorMessage { get; init; }

    /// <summary>
    /// Additional error details if available
    /// </summary>
    public string? ErrorDetails { get; init; }
}

/// <summary>
/// Information about the currently authenticated user
/// </summary>
public record CurrentUserInfo {
    /// <summary>
    /// The username
    /// </summary>
    public required string Username { get; init; }

    /// <summary>
    /// The user's email address
    /// </summary>
    public string? Email { get; init; }

    /// <summary>
    /// The user's display name
    /// </summary>
    public string? DisplayName { get; init; }

    /// <summary>
    /// User's roles or permissions
    /// </summary>
    public IReadOnlyList<string> Roles { get; init; } = Array.Empty<string>();

    /// <summary>
    /// When the user account was created
    /// </summary>
    public DateTimeOffset? CreatedAt { get; init; }

    /// <summary>
    /// User's avatar URL
    /// </summary>
    public string? AvatarUrl { get; init; }
}

/// <summary>
/// Result of token validation
/// </summary>
public record TokenValidationResult {
    /// <summary>
    /// Whether the token is valid
    /// </summary>
    public required bool IsValid { get; init; }

    /// <summary>
    /// Whether the token is expired
    /// </summary>
    public bool IsExpired { get; init; }

    /// <summary>
    /// When the token expires
    /// </summary>
    public DateTimeOffset? ExpiresAt { get; init; }

    /// <summary>
    /// Error message if validation failed
    /// </summary>
    public string? ErrorMessage { get; init; }

    /// <summary>
    /// Whether the token needs to be refreshed
    /// </summary>
    public bool NeedsRefresh { get; init; }
}

/// <summary>
/// Result of token refresh attempt
/// </summary>
public record TokenRefreshResult {
    /// <summary>
    /// Whether refresh was successful
    /// </summary>
    public required bool IsSuccess { get; init; }

    /// <summary>
    /// The new authentication token if successful
    /// </summary>
    public string? NewToken { get; init; }

    /// <summary>
    /// The new refresh token if available
    /// </summary>
    public string? NewRefreshToken { get; init; }

    /// <summary>
    /// When the new token expires
    /// </summary>
    public DateTimeOffset? ExpiresAt { get; init; }

    /// <summary>
    /// Error message if refresh failed
    /// </summary>
    public string? ErrorMessage { get; init; }
}

/// <summary>
/// Information about an authenticated registry
/// </summary>
public record AuthenticatedRegistry {
    /// <summary>
    /// The registry URL
    /// </summary>
    public required string Url { get; init; }

    /// <summary>
    /// The authenticated username
    /// </summary>
    public required string Username { get; init; }

    /// <summary>
    /// When authentication was established
    /// </summary>
    public DateTimeOffset AuthenticatedAt { get; init; }

    /// <summary>
    /// When the authentication expires
    /// </summary>
    public DateTimeOffset? ExpiresAt { get; init; }

    /// <summary>
    /// Whether the authentication is currently valid
    /// </summary>
    public bool IsValid { get; init; }
}