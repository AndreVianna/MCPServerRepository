namespace MCPHub.AuthenticationService.Services;

/// <summary>
/// Implementation of user profile service
/// Skeleton implementation - actual logic will be implemented when first consumer requires it
/// </summary>
public class UserProfileService : IUserProfileService {
    /// <inheritdoc />
    public Task<UserProfileResult> GetProfileAsync(Guid userId, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("User profile retrieval will be implemented when first consumer requires it");

    /// <inheritdoc />
    public Task<ProfileUpdateResult> UpdateProfileAsync(Guid userId, UpdateProfileRequest request, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("User profile update will be implemented when first consumer requires it");

    /// <inheritdoc />
    public Task<PasswordChangeResult> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("Password change functionality will be implemented when first consumer requires it");

    /// <inheritdoc />
    public Task<EmailVerificationResult> VerifyEmailAsync(Guid userId, string verificationToken, CancellationToken cancellationToken = default)
        => throw new NotImplementedException("Email verification will be implemented when first consumer requires it");
}