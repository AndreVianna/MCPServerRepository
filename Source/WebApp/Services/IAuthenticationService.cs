namespace MCPHub.WebApp.Services;

/// <summary>
/// Enhanced interface for authentication operations in the web application
/// </summary>
public interface IAuthenticationService {
    /// <summary>
    /// Authenticates a user with email and password
    /// </summary>
    /// <param name="request">Login request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Authentication result</returns>
    Task<AuthenticationResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers a new user account with enhanced registration flow
    /// </summary>
    /// <param name="request">Registration request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Registration result</returns>
    Task<RegistrationResult> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Authenticates a user via social login provider
    /// </summary>
    /// <param name="request">Social login request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Authentication result</returns>
    Task<AuthenticationResult> SocialLoginAsync(SocialLoginRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies user email address with token
    /// </summary>
    /// <param name="request">Email verification request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Email verification result</returns>
    Task<EmailVerificationResult> VerifyEmailAsync(VerifyEmailRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Resends email verification token
    /// </summary>
    /// <param name="request">Resend verification request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Email verification result</returns>
    Task<EmailVerificationResult> ResendVerificationAsync(ResendVerificationRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Refreshes the current authentication token
    /// </summary>
    /// <param name="refreshToken">Refresh token</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>New token result</returns>
    Task<TokenResult> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs out the current user
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Logout result</returns>
    Task<LogoutResult> LogoutAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the current user profile
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User profile result</returns>
    Task<UserProfileResult> GetProfileAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Initiates password reset process
    /// </summary>
    /// <param name="email">User email address</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Password reset result</returns>
    Task<PasswordChangeResult> InitiatePasswordResetAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Completes password reset with token
    /// </summary>
    /// <param name="token">Reset token</param>
    /// <param name="newPassword">New password</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Password reset result</returns>
    Task<PasswordChangeResult> CompletePasswordResetAsync(string token, string newPassword, CancellationToken cancellationToken = default);
}