using MCPHub.Domain.Contracts.Requests;
using MCPHub.Domain.Contracts.Responses;

namespace MCPHub.AuthenticationService.Services;

/// <summary>
/// Interface for authentication operations
/// </summary>
public interface IAuthenticationService {
    /// <summary>
    /// Authenticates a user with email and password
    /// </summary>
    /// <param name="email">User email address</param>
    /// <param name="password">User password</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Authentication result with user information and tokens</returns>
    Task<AuthenticationResult> LoginAsync(string email, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers a new user account
    /// </summary>
    /// <param name="request">User registration request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Registration result with user information</returns>
    Task<RegistrationResult> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Refreshes an authentication token
    /// </summary>
    /// <param name="refreshToken">Refresh token</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>New authentication tokens</returns>
    Task<TokenResult> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs out a user and invalidates tokens
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Logout result</returns>
    Task<LogoutResult> LogoutAsync(Guid userId, CancellationToken cancellationToken = default);
}