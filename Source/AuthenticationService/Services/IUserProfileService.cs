namespace MCPHub.AuthenticationService.Services;

/// <summary>
/// Interface for user profile management operations
/// </summary>
public interface IUserProfileService {
    /// <summary>
    /// Gets user profile by user ID
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User profile information</returns>
    Task<UserProfileResult> GetProfileAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates user profile information
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="request">Profile update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Profile update result</returns>
    Task<ProfileUpdateResult> UpdateProfileAsync(Guid userId, UpdateProfileRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Changes user password
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="currentPassword">Current password</param>
    /// <param name="newPassword">New password</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Password change result</returns>
    Task<PasswordChangeResult> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies user email address
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="verificationToken">Email verification token</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Email verification result</returns>
    Task<EmailVerificationResult> VerifyEmailAsync(Guid userId, string verificationToken, CancellationToken cancellationToken = default);
}