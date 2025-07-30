namespace MCPHub.WebApp.Services;

/// <summary>
/// Service interface for publisher profile management and verification
/// </summary>
public interface IPublisherProfileService {
    /// <summary>
    /// Gets comprehensive publisher profile information
    /// </summary>
    /// <param name="publisherId">The publisher's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Complete publisher profile</returns>
    Task<PublisherProfile> GetPublisherProfileAsync(Guid publisherId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates publisher profile information
    /// </summary>
    /// <param name="publisherId">The publisher's unique identifier</param>
    /// <param name="updateRequest">Profile update data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Profile update result</returns>
    Task<ProfileUpdateResult> UpdateProfileAsync(Guid publisherId, ProfileUpdateRequest updateRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets publisher verification status and requirements
    /// </summary>
    /// <param name="publisherId">The publisher's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Verification status information</returns>
    Task<VerificationStatus> GetVerificationStatusAsync(Guid publisherId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Initiates publisher verification process
    /// </summary>
    /// <param name="publisherId">The publisher's unique identifier</param>
    /// <param name="verificationRequest">Verification request data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Verification initiation result</returns>
    Task<VerificationResult> InitiateVerificationAsync(Guid publisherId, VerificationRequest verificationRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Submits verification documents
    /// </summary>
    /// <param name="publisherId">The publisher's unique identifier</param>
    /// <param name="documents">Verification documents</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Document submission result</returns>
    Task<DocumentSubmissionResult> SubmitVerificationDocumentsAsync(Guid publisherId, IEnumerable<VerificationDocument> documents, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets trust tier information and progression details
    /// </summary>
    /// <param name="publisherId">The publisher's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Trust tier information</returns>
    Task<TrustTierInfo> GetTrustTierInfoAsync(Guid publisherId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets trust tier improvement recommendations
    /// </summary>
    /// <param name="publisherId">The publisher's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Trust tier improvement guidance</returns>
    Task<TrustTierGuidance> GetTrustTierGuidanceAsync(Guid publisherId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets publisher security settings
    /// </summary>
    /// <param name="publisherId">The publisher's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Security settings</returns>
    Task<PublisherSecuritySettings> GetSecuritySettingsAsync(Guid publisherId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates publisher security settings
    /// </summary>
    /// <param name="publisherId">The publisher's unique identifier</param>
    /// <param name="securitySettings">Updated security settings</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Security settings update result</returns>
    Task<SecuritySettingsResult> UpdateSecuritySettingsAsync(Guid publisherId, SecuritySettingsRequest securitySettings, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets two-factor authentication status and settings
    /// </summary>
    /// <param name="publisherId">The publisher's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Two-factor authentication information</returns>
    Task<TwoFactorAuthInfo> GetTwoFactorAuthInfoAsync(Guid publisherId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Enables two-factor authentication
    /// </summary>
    /// <param name="publisherId">The publisher's unique identifier</param>
    /// <param name="twoFactorRequest">2FA setup request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>2FA setup result</returns>
    Task<TwoFactorSetupResult> EnableTwoFactorAuthAsync(Guid publisherId, TwoFactorSetupRequest twoFactorRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Disables two-factor authentication
    /// </summary>
    /// <param name="publisherId">The publisher's unique identifier</param>
    /// <param name="confirmationCode">Confirmation code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>2FA disable result</returns>
    Task<TwoFactorResult> DisableTwoFactorAuthAsync(Guid publisherId, string confirmationCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates backup codes for two-factor authentication
    /// </summary>
    /// <param name="publisherId">The publisher's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Backup codes generation result</returns>
    Task<BackupCodesResult> GenerateBackupCodesAsync(Guid publisherId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets API keys and access tokens for the publisher
    /// </summary>
    /// <param name="publisherId">The publisher's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>API key information</returns>
    Task<ApiKeyInfo> GetApiKeysAsync(Guid publisherId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new API key
    /// </summary>
    /// <param name="publisherId">The publisher's unique identifier</param>
    /// <param name="keyRequest">API key creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>API key creation result</returns>
    Task<ApiKeyCreationResult> CreateApiKeyAsync(Guid publisherId, ApiKeyRequest keyRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokes an API key
    /// </summary>
    /// <param name="publisherId">The publisher's unique identifier</param>
    /// <param name="keyId">The API key identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>API key revocation result</returns>
    Task<ApiKeyResult> RevokeApiKeyAsync(Guid publisherId, Guid keyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets publisher notification preferences
    /// </summary>
    /// <param name="publisherId">The publisher's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Notification preferences</returns>
    Task<NotificationPreferences> GetNotificationPreferencesAsync(Guid publisherId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates publisher notification preferences
    /// </summary>
    /// <param name="publisherId">The publisher's unique identifier</param>
    /// <param name="preferences">Updated notification preferences</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Preferences update result</returns>
    Task<PreferencesUpdateResult> UpdateNotificationPreferencesAsync(Guid publisherId, NotificationPreferencesRequest preferences, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets publisher billing and subscription information
    /// </summary>
    /// <param name="publisherId">The publisher's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Billing information (null if not applicable)</returns>
    Task<BillingInfo?> GetBillingInfoAsync(Guid publisherId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates billing information
    /// </summary>
    /// <param name="publisherId">The publisher's unique identifier</param>
    /// <param name="billingRequest">Updated billing information</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Billing update result</returns>
    Task<BillingUpdateResult> UpdateBillingInfoAsync(Guid publisherId, BillingInfoRequest billingRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets publisher organization members and roles
    /// </summary>
    /// <param name="publisherId">The publisher's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Organization member information</returns>
    Task<OrganizationMembers> GetOrganizationMembersAsync(Guid publisherId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Manages organization member roles and permissions
    /// </summary>
    /// <param name="publisherId">The publisher's unique identifier</param>
    /// <param name="memberRequest">Member management request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Member management result</returns>
    Task<MemberManagementResult> ManageOrganizationMemberAsync(Guid publisherId, OrganizationMemberRequest memberRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets account activity log and audit trail
    /// </summary>
    /// <param name="publisherId">The publisher's unique identifier</param>
    /// <param name="pageSize">Number of entries to retrieve</param>
    /// <param name="pageNumber">Page number</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Account activity log</returns>
    Task<AccountActivityLog> GetAccountActivityAsync(Guid publisherId, int pageSize = 50, int pageNumber = 1, CancellationToken cancellationToken = default);

    /// <summary>
    /// Initiates account deletion process
    /// </summary>
    /// <param name="publisherId">The publisher's unique identifier</param>
    /// <param name="deletionRequest">Account deletion request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Account deletion initiation result</returns>
    Task<AccountDeletionResult> InitiateAccountDeletionAsync(Guid publisherId, AccountDeletionRequest deletionRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cancels account deletion process
    /// </summary>
    /// <param name="publisherId">The publisher's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Deletion cancellation result</returns>
    Task<AccountDeletionResult> CancelAccountDeletionAsync(Guid publisherId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Exports publisher data for backup or migration
    /// </summary>
    /// <param name="publisherId">The publisher's unique identifier</param>
    /// <param name="exportRequest">Data export request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Data export result</returns>
    Task<DataExportResult> ExportPublisherDataAsync(Guid publisherId, DataExportRequest exportRequest, CancellationToken cancellationToken = default);
}

/// <summary>
/// Complete publisher profile information
/// </summary>
public class PublisherProfile {
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public PublisherType Type { get; set; }
    public TrustTier TrustTier { get; set; }
    public string? Bio { get; set; }
    public string? Website { get; set; }
    public string? Location { get; set; }
    public string? Company { get; set; }
    public string? Avatar { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastActiveAt { get; set; }
    public bool IsVerified { get; set; }
    public VerificationStatus VerificationStatus { get; set; } = new();
    public ProfileStatistics Statistics { get; set; } = new();
    public IEnumerable<SocialLink> SocialLinks { get; set; } = [];
    public IEnumerable<string> Specializations { get; set; } = [];
    public IEnumerable<Achievement> Achievements { get; set; } = [];
    public bool IsPublic { get; set; } = true;
}

/// <summary>
/// Profile update request
/// </summary>
public class ProfileUpdateRequest {
    public string? DisplayName { get; set; }
    public string? Bio { get; set; }
    public string? Website { get; set; }
    public string? Location { get; set; }
    public string? Company { get; set; }
    public IEnumerable<SocialLink>? SocialLinks { get; set; }
    public IEnumerable<string>? Specializations { get; set; }
    public bool? IsPublic { get; set; }
    public string? Avatar { get; set; }
}

/// <summary>
/// Publisher verification status
/// </summary>
public class VerificationStatus {
    public bool IsVerified { get; set; }
    public VerificationLevel Level { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public string? VerifiedBy { get; set; }
    public IEnumerable<VerificationRequirement> Requirements { get; set; } = [];
    public IEnumerable<VerificationDocument> SubmittedDocuments { get; set; } = [];
    public VerificationProgress Progress { get; set; } = new();
    public string? RejectionReason { get; set; }
    public DateTime? NextReviewDate { get; set; }
}

/// <summary>
/// Verification request
/// </summary>
public class VerificationRequest {
    public VerificationLevel RequestedLevel { get; set; }
    public string BusinessName { get; set; } = string.Empty;
    public string BusinessType { get; set; } = string.Empty;
    public string ContactName { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public AddressInfo BusinessAddress { get; set; } = new();
    public string? Website { get; set; }
    public string? TaxId { get; set; }
    public string? RegistrationNumber { get; set; }
    public string Purpose { get; set; } = string.Empty;
    public IEnumerable<string> ExpectedPackageTypes { get; set; } = [];
}

/// <summary>
/// Verification result
/// </summary>
public class VerificationResult {
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public Guid? VerificationId { get; set; }
    public IEnumerable<VerificationRequirement> RequiredDocuments { get; set; } = [];
    public DateTime? EstimatedReviewDate { get; set; }
    public string? ReferenceNumber { get; set; }
}

/// <summary>
/// Verification document
/// </summary>
public class VerificationDocument {
    public Guid Id { get; set; }
    public DocumentType Type { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public DateTime UploadedAt { get; set; }
    public DocumentStatus Status { get; set; }
    public string? ReviewNotes { get; set; }
    public byte[] Content { get; set; } = [];
}

/// <summary>
/// Document submission result
/// </summary>
public class DocumentSubmissionResult {
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public IEnumerable<Guid> UploadedDocumentIds { get; set; } = [];
    public IEnumerable<string> ValidationErrors { get; set; } = [];
    public DateTime SubmittedAt { get; set; }
}

/// <summary>
/// Trust tier information
/// </summary>
public class TrustTierInfo {
    public TrustTier CurrentTier { get; set; }
    public TrustTier? NextTier { get; set; }
    public double CompletionPercentage { get; set; }
    public int PointsEarned { get; set; }
    public int PointsRequired { get; set; }
    public IEnumerable<TrustTierBenefit> CurrentBenefits { get; set; } = [];
    public IEnumerable<TrustTierBenefit> NextTierBenefits { get; set; } = [];
    public IEnumerable<TrustTierHistoryEntry> History { get; set; } = [];
    public DateTime? EligibilityDate { get; set; }
    public bool CanAdvance { get; set; }
}

/// <summary>
/// Trust tier guidance
/// </summary>
public class TrustTierGuidance {
    public TrustTier TargetTier { get; set; }
    public IEnumerable<TrustTierRequirement> Requirements { get; set; } = [];
    public IEnumerable<TierImprovementAction> RecommendedActions { get; set; } = [];
    public IEnumerable<string> Tips { get; set; } = [];
    public TimeSpan EstimatedTimeToAdvance { get; set; }
    public int Priority { get; set; }
}

/// <summary>
/// Publisher security settings
/// </summary>
public class PublisherSecuritySettings {
    public bool TwoFactorEnabled { get; set; }
    public bool EmailVerificationRequired { get; set; }
    public bool ApiKeyRotationEnabled { get; set; }
    public int ApiKeyRotationDays { get; set; }
    public bool LoginNotificationsEnabled { get; set; }
    public bool SuspiciousActivityAlertsEnabled { get; set; }
    public IEnumerable<string> TrustedIpAddresses { get; set; } = [];
    public IEnumerable<string> BlockedIpAddresses { get; set; } = [];
    public bool RequireSecureConnection { get; set; }
    public int SessionTimeoutMinutes { get; set; }
    public bool AllowMultipleSessions { get; set; }
    public SecurityLevel MinimumSecurityLevel { get; set; }
}

/// <summary>
/// Security settings request
/// </summary>
public class SecuritySettingsRequest {
    public bool? EmailVerificationRequired { get; set; }
    public bool? ApiKeyRotationEnabled { get; set; }
    public int? ApiKeyRotationDays { get; set; }
    public bool? LoginNotificationsEnabled { get; set; }
    public bool? SuspiciousActivityAlertsEnabled { get; set; }
    public IEnumerable<string>? TrustedIpAddresses { get; set; }
    public IEnumerable<string>? BlockedIpAddresses { get; set; }
    public bool? RequireSecureConnection { get; set; }
    public int? SessionTimeoutMinutes { get; set; }
    public bool? AllowMultipleSessions { get; set; }
    public SecurityLevel? MinimumSecurityLevel { get; set; }
}

/// <summary>
/// Security settings result
/// </summary>
public class SecuritySettingsResult {
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public IEnumerable<string> ValidationErrors { get; set; } = [];
    public DateTime UpdatedAt { get; set; }
    public IEnumerable<string> SecurityWarnings { get; set; } = [];
}

/// <summary>
/// Two-factor authentication information
/// </summary>
public class TwoFactorAuthInfo {
    public bool IsEnabled { get; set; }
    public IEnumerable<TwoFactorMethod> EnabledMethods { get; set; } = [];
    public IEnumerable<TwoFactorMethod> AvailableMethods { get; set; } = [];
    public int BackupCodesRemaining { get; set; }
    public DateTime? LastUsed { get; set; }
    public bool RequiredForLogin { get; set; }
    public bool RequiredForSensitiveActions { get; set; }
}

/// <summary>
/// Two-factor setup request
/// </summary>
public class TwoFactorSetupRequest {
    public required TwoFactorMethod Method { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? AuthenticatorSecret { get; set; }
    public string VerificationCode { get; set; } = string.Empty;
    public bool RequireForLogin { get; set; } = true;
    public bool RequireForSensitiveActions { get; set; } = true;
}

/// <summary>
/// Two-factor setup result
/// </summary>
public class TwoFactorSetupResult {
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public string? QrCodeUrl { get; set; }
    public string? Secret { get; set; }
    public IEnumerable<string> BackupCodes { get; set; } = [];
    public DateTime EnabledAt { get; set; }
}

/// <summary>
/// Two-factor result
/// </summary>
public class TwoFactorResult {
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime ActionDate { get; set; }
}

/// <summary>
/// Backup codes result
/// </summary>
public class BackupCodesResult {
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public IEnumerable<string> BackupCodes { get; set; } = [];
    public DateTime GeneratedAt { get; set; }
    public int CodesRemaining { get; set; }
}

/// <summary>
/// API key information
/// </summary>
public class ApiKeyInfo {
    public IEnumerable<ApiKey> ApiKeys { get; set; } = [];
    public int MaxAllowedKeys { get; set; }
    public bool CanCreateMore { get; set; }
    public DateTime? LastKeyCreated { get; set; }
    public DateTime? LastKeyUsed { get; set; }
}

/// <summary>
/// API key
/// </summary>
public class ApiKey {
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string KeyPrefix { get; set; } = string.Empty;
    public IEnumerable<string> Scopes { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime? LastUsed { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsActive { get; set; }
    public string? Description { get; set; }
    public IEnumerable<string> IpRestrictions { get; set; } = [];
}

/// <summary>
/// API key request
/// </summary>
public class ApiKeyRequest {
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public IEnumerable<string> Scopes { get; set; } = [];
    public DateTime? ExpiresAt { get; set; }
    public IEnumerable<string> IpRestrictions { get; set; } = [];
}

/// <summary>
/// API key creation result
/// </summary>
public class ApiKeyCreationResult {
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public Guid? KeyId { get; set; }
    public string? ApiKey { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
}

/// <summary>
/// API key result
/// </summary>
public class ApiKeyResult {
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime ActionDate { get; set; }
}

/// <summary>
/// Notification preferences
/// </summary>
public class NotificationPreferences {
    public EmailNotificationSettings Email { get; set; } = new();
    public WebNotificationSettings Web { get; set; } = new();
    public MobileNotificationSettings Mobile { get; set; } = new();
    public string TimeZone { get; set; } = "UTC";
    public bool QuietHoursEnabled { get; set; }
    public TimeSpan QuietHoursStart { get; set; }
    public TimeSpan QuietHoursEnd { get; set; }
    public NotificationFrequency DigestFrequency { get; set; } = NotificationFrequency.Weekly;
}

/// <summary>
/// Email notification settings
/// </summary>
public class EmailNotificationSettings {
    public bool SecurityAlerts { get; set; } = true;
    public bool PackageUpdates { get; set; } = true;
    public bool DownloadMilestones { get; set; } = true;
    public bool WeeklyDigest { get; set; } = true;
    public bool MarketingEmails { get; set; } = false;
    public bool SystemNotifications { get; set; } = true;
    public bool TrustTierUpdates { get; set; } = true;
    public bool CollaboratorInvitations { get; set; } = true;
}

/// <summary>
/// Web notification settings
/// </summary>
public class WebNotificationSettings {
    public bool Enabled { get; set; } = true;
    public bool SecurityAlerts { get; set; } = true;
    public bool RealTimeUpdates { get; set; } = true;
    public bool DownloadAlerts { get; set; } = false;
    public bool SystemMessages { get; set; } = true;
}

/// <summary>
/// Mobile notification settings
/// </summary>
public class MobileNotificationSettings {
    public bool Enabled { get; set; } = false;
    public bool PushNotifications { get; set; } = false;
    public bool SecurityAlerts { get; set; } = true;
    public bool CriticalOnly { get; set; } = true;
}

/// <summary>
/// Notification preferences request
/// </summary>
public class NotificationPreferencesRequest {
    public EmailNotificationSettings? Email { get; set; }
    public WebNotificationSettings? Web { get; set; }
    public MobileNotificationSettings? Mobile { get; set; }
    public string? TimeZone { get; set; }
    public bool? QuietHoursEnabled { get; set; }
    public TimeSpan? QuietHoursStart { get; set; }
    public TimeSpan? QuietHoursEnd { get; set; }
    public NotificationFrequency? DigestFrequency { get; set; }
}

/// <summary>
/// Preferences update result
/// </summary>
public class PreferencesUpdateResult {
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public IEnumerable<string> ValidationErrors { get; set; } = [];
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Profile statistics
/// </summary>
public class ProfileStatistics {
    public int TotalPackages { get; set; }
    public long TotalDownloads { get; set; }
    public double AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public int Followers { get; set; }
    public int Following { get; set; }
    public DateTime JoinedDate { get; set; }
    public int DaysActive { get; set; }
    public int ContributionsThisYear { get; set; }
}

/// <summary>
/// Social link
/// </summary>
public class SocialLink {
    public SocialPlatform Platform { get; set; }
    public string Url { get; set; } = string.Empty;
    public bool IsVerified { get; set; }
}

/// <summary>
/// Achievement
/// </summary>
public class Achievement {
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public DateTime EarnedAt { get; set; }
    public bool IsRare { get; set; }
    public int Points { get; set; }
}

/// <summary>
/// Verification requirement
/// </summary>
public class VerificationRequirement {
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsRequired { get; set; }
    public bool IsCompleted { get; set; }
    public DocumentType? RequiredDocumentType { get; set; }
    public int Priority { get; set; }
}

/// <summary>
/// Verification progress
/// </summary>
public class VerificationProgress {
    public int CompletedRequirements { get; set; }
    public int TotalRequirements { get; set; }
    public double CompletionPercentage { get; set; }
    public VerificationStage CurrentStage { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public DateTime? ReviewStartedAt { get; set; }
    public DateTime? EstimatedCompletionAt { get; set; }
}

/// <summary>
/// Address information
/// </summary>
public class AddressInfo {
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
}

/// <summary>
/// Trust tier benefit
/// </summary>
public class TrustTierBenefit {
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public BenefitType Type { get; set; }
    public bool IsUnlocked { get; set; }
}

/// <summary>
/// Tier improvement action
/// </summary>
public class TierImprovementAction {
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Points { get; set; }
    public ActionDifficulty Difficulty { get; set; }
    public TimeSpan EstimatedTime { get; set; }
    public bool IsCompleted { get; set; }
    public string? Url { get; set; }
}

/// <summary>
/// Two-factor method
/// </summary>
public class TwoFactorMethod {
    public TwoFactorType Type { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
    public bool IsDefault { get; set; }
    public DateTime? LastUsed { get; set; }
}

// Additional classes for billing, organization, activity log, and data export would be defined here
// These follow similar patterns but are omitted for brevity

/// <summary>
/// Billing information (placeholder)
/// </summary>
public class BillingInfo {
    public string PlanName { get; set; } = string.Empty;
    public decimal MonthlyCharge { get; set; }
    public DateTime NextBillingDate { get; set; }
    // Additional billing properties...
}

/// <summary>
/// Billing info request (placeholder)
/// </summary>
public class BillingInfoRequest { }

/// <summary>
/// Billing update result (placeholder)
/// </summary>
public class BillingUpdateResult { }

/// <summary>
/// Organization members (placeholder)
/// </summary>
public class OrganizationMembers { }

/// <summary>
/// Organization member request (placeholder)
/// </summary>
public class OrganizationMemberRequest { }

/// <summary>
/// Member management result (placeholder)
/// </summary>
public class MemberManagementResult { }

/// <summary>
/// Account activity log (placeholder)
/// </summary>
public class AccountActivityLog { }

/// <summary>
/// Account deletion request (placeholder)
/// </summary>
public class AccountDeletionRequest { }

/// <summary>
/// Account deletion result (placeholder)
/// </summary>
public class AccountDeletionResult { }

/// <summary>
/// Data export request (placeholder)
/// </summary>
public class DataExportRequest { }

/// <summary>
/// Data export result (placeholder)
/// </summary>
public class DataExportResult { }

/// <summary>
/// Verification levels
/// </summary>
public enum VerificationLevel {
    None,
    Basic,
    Enhanced,
    Enterprise
}

/// <summary>
/// Document types
/// </summary>
public enum DocumentType {
    GovernmentId,
    BusinessLicense,
    TaxDocument,
    BankStatement,
    UtilityBill,
    CertificateOfIncorporation,
    Other
}

/// <summary>
/// Document status
/// </summary>
public enum DocumentStatus {
    Pending,
    UnderReview,
    Approved,
    Rejected,
    Expired
}

/// <summary>
/// Verification stages
/// </summary>
public enum VerificationStage {
    NotStarted,
    DocumentSubmission,
    DocumentReview,
    BackgroundCheck,
    FinalReview,
    Completed,
    Rejected
}

/// <summary>
/// Social platforms
/// </summary>
public enum SocialPlatform {
    GitHub,
    Twitter,
    LinkedIn,
    Website,
    Blog,
    YouTube,
    Discord,
    Mastodon
}

/// <summary>
/// Security levels
/// </summary>
public enum SecurityLevel {
    Basic,
    Standard,
    High,
    Maximum
}

/// <summary>
/// Two-factor types
/// </summary>
public enum TwoFactorType {
    App,
    SMS,
    Email,
    Hardware
}

/// <summary>
/// Benefit types
/// </summary>
public enum BenefitType {
    Feature,
    Limit,
    Support,
    Recognition,
    Financial
}

/// <summary>
/// Action difficulty
/// </summary>
public enum ActionDifficulty {
    Easy,
    Medium,
    Hard,
    Expert
}

/// <summary>
/// Notification frequency
/// </summary>
public enum NotificationFrequency {
    Never,
    Daily,
    Weekly,
    Monthly,
    Quarterly
}