namespace MCPHub.WebApp.Services;

/// <summary>
/// Interface for security and authentication management operations
/// </summary>
public interface ISecurityService {
    /// <summary>
    /// Changes the user's password
    /// </summary>
    /// <param name="request">Password change request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Password change result</returns>
    Task<PasswordChangeResult> ChangePasswordAsync(ChangePasswordRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets up two-factor authentication
    /// </summary>
    /// <param name="request">Two-factor setup request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Two-factor setup result</returns>
    Task<TwoFactorSetupResult> SetupTwoFactorAsync(SetupTwoFactorRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies two-factor authentication setup
    /// </summary>
    /// <param name="request">Two-factor verification request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Two-factor verification result</returns>
    Task<TwoFactorVerificationResult> VerifyTwoFactorAsync(VerifyTwoFactorRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Disables two-factor authentication
    /// </summary>
    /// <param name="password">Current password for verification</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Disable result</returns>
    Task<TwoFactorVerificationResult> DisableTwoFactorAsync(string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a new API key
    /// </summary>
    /// <param name="request">API key generation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>API key generation result</returns>
    Task<ApiKeyGenerationResult> GenerateApiKeyAsync(GenerateApiKeyRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all user's API keys
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>API keys list result</returns>
    Task<ApiKeysListResult> GetApiKeysAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokes an API key
    /// </summary>
    /// <param name="request">API key revocation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Revocation result</returns>
    Task<LogoutResult> RevokeApiKeyAsync(RevokeApiKeyRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets user's active sessions
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User sessions result</returns>
    Task<UserSessionsResult> GetActiveSessionsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Terminates a specific session
    /// </summary>
    /// <param name="sessionId">Session ID to terminate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Termination result</returns>
    Task<LogoutResult> TerminateSessionAsync(Guid sessionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Terminates all sessions except current
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Termination result</returns>
    Task<LogoutResult> TerminateAllOtherSessionsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes user account
    /// </summary>
    /// <param name="request">Account deletion request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Account deletion result</returns>
    Task<AccountDeletionResult> DeleteAccountAsync(DeleteAccountRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Exports user data
    /// </summary>
    /// <param name="request">Data export request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Data export result</returns>
    Task<DataExportResult> ExportUserDataAsync(ExportUserDataRequest request, CancellationToken cancellationToken = default);
}