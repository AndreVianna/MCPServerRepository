namespace MCPHub.Domain.Contracts.Responses;

/// <summary>
/// Enhanced user profile result with comprehensive user information
/// </summary>
public class UserProfileResult {
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
    public UserProfile? Profile { get; init; }
}

/// <summary>
/// Result of user profile update operation
/// </summary>
public class UserProfileUpdateResult {
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
    public UserProfile? UpdatedProfile { get; init; }
}

/// <summary>
/// Comprehensive user profile data
/// </summary>
public record UserProfile {
    public Guid Id { get; init; }
    public required string UserName { get; init; }
    public required string Email { get; init; }
    public string? DisplayName { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? Bio { get; init; }
    public string? JobTitle { get; init; }
    public string? Company { get; init; }
    public string? Location { get; init; }
    public string? Website { get; init; }
    public string? GitHubUrl { get; init; }
    public string? TwitterUrl { get; init; }
    public string? LinkedInUrl { get; init; }
    public string? PhoneNumber { get; init; }
    public string? AvatarUrl { get; init; }
    public required string AccountType { get; init; }
    public bool IsEmailVerified { get; init; }
    public bool IsPublicProfile { get; init; }
    public bool ShowEmail { get; init; }
    public bool ReceiveUpdates { get; init; }
    public bool ReceiveMarketingEmails { get; init; }
    public bool IsTwoFactorEnabled { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? LastLoginAt { get; init; }
    public UserStatistics? Statistics { get; init; }
}

/// <summary>
/// User statistics and activity metrics
/// </summary>
public class UserStatistics {
    public int PackagesInstalled { get; init; }
    public int PackagesFavorited { get; init; }
    public int PackagesPublished { get; init; }
    public long TotalDownloads { get; init; }
    public int LoginCount { get; init; }
    public DateTime? LastActivityAt { get; init; }
}

/// <summary>
/// Result of avatar upload operation
/// </summary>
public class AvatarUploadResult {
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
    public string? AvatarUrl { get; init; }
}

/// <summary>
/// Result of password change operation
/// </summary>
public class PasswordChangeResult {
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
}

/// <summary>
/// Result of two-factor authentication setup
/// </summary>
public class TwoFactorSetupResult {
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
    public string? QrCodeUrl { get; init; }
    public string? ManualEntryKey { get; init; }
    public string[]? RecoveryCodes { get; init; }
}

/// <summary>
/// Result of two-factor authentication verification
/// </summary>
public class TwoFactorVerificationResult {
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
    public bool IsEnabled { get; init; }
    public string[]? RecoveryCodes { get; init; }
}

/// <summary>
/// API key information
/// </summary>
public class ApiKeyInfo {
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required string Key { get; init; }
    public string[]? Scopes { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? ExpiresAt { get; init; }
    public DateTime? LastUsedAt { get; init; }
    public bool IsActive { get; init; }
}

/// <summary>
/// Result of API key generation
/// </summary>
public class ApiKeyGenerationResult {
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
    public ApiKeyInfo? ApiKey { get; init; }
}

/// <summary>
/// List of user's API keys
/// </summary>
public class ApiKeysListResult {
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
    public ApiKeyInfo[]? ApiKeys { get; init; }
}

/// <summary>
/// User session information
/// </summary>
public class UserSession {
    public Guid Id { get; init; }
    public required string DeviceInfo { get; init; }
    public required string IpAddress { get; init; }
    public required string Location { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime LastAccessedAt { get; init; }
    public bool IsCurrent { get; init; }
}

/// <summary>
/// List of user sessions
/// </summary>
public class UserSessionsResult {
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
    public UserSession[]? Sessions { get; init; }
}

/// <summary>
/// User activity log entry
/// </summary>
public class ActivityLogEntry {
    public Guid Id { get; init; }
    public required string Action { get; init; }
    public required string Description { get; init; }
    public string? IpAddress { get; init; }
    public string? UserAgent { get; init; }
    public DateTime Timestamp { get; init; }
    public Dictionary<string, object>? Metadata { get; init; }
}

/// <summary>
/// User activity log result
/// </summary>
public class ActivityLogResult {
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
    public ActivityLogEntry[]? Activities { get; init; }
    public int TotalCount { get; init; }
}

/// <summary>
/// Result of account deletion operation
/// </summary>
public class AccountDeletionResult {
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
    public string? ExportUrl { get; init; }
}

/// <summary>
/// Result of data export operation
/// </summary>
public class DataExportResult {
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
    public string? DownloadUrl { get; init; }
    public DateTime? ExpiresAt { get; init; }
}