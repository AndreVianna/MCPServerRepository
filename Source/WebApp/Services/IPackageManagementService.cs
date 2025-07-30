using MCPHub.Domain.Entities;
using MCPHub.Domain.ValueObjects;

namespace MCPHub.WebApp.Services;

/// <summary>
/// Service interface for comprehensive package management operations
/// </summary>
public interface IPackageManagementService {
    /// <summary>
    /// Gets packages owned by a publisher with filtering and pagination
    /// </summary>
    /// <param name="publisherId">The publisher's unique identifier</param>
    /// <param name="searchQuery">Optional search query</param>
    /// <param name="status">Filter by package status</param>
    /// <param name="sortBy">Sort by field (name, downloads, updated, rating)</param>
    /// <param name="sortDirection">Sort direction</param>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated package listing</returns>
    Task<PackageListResult> GetPublisherPackagesAsync(
        Guid publisherId,
        string? searchQuery = null,
        PackageStatus? status = null,
        string sortBy = "updated",
        SortDirection sortDirection = SortDirection.Descending,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets detailed package information for management
    /// </summary>
    /// <param name="packageId">The package identifier</param>
    /// <param name="publisherId">The publisher identifier (for authorization)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Detailed package management information</returns>
    Task<PackageManagementDetails?> GetPackageDetailsAsync(Guid packageId, Guid publisherId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates package metadata and settings
    /// </summary>
    /// <param name="packageId">The package identifier</param>
    /// <param name="publisherId">The publisher identifier (for authorization)</param>
    /// <param name="updateRequest">Package update data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Update result</returns>
    Task<PackageUpdateResult> UpdatePackageAsync(Guid packageId, Guid publisherId, PackageUpdateRequest updateRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all versions of a package with management capabilities
    /// </summary>
    /// <param name="packageId">The package identifier</param>
    /// <param name="publisherId">The publisher identifier (for authorization)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Package versions with management information</returns>
    Task<IEnumerable<PackageVersionManagement>> GetPackageVersionsAsync(Guid packageId, Guid publisherId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Publishes a new version of an existing package
    /// </summary>
    /// <param name="packageId">The package identifier</param>
    /// <param name="publisherId">The publisher identifier (for authorization)</param>
    /// <param name="versionRequest">New version data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Version publish result</returns>
    Task<VersionPublishResult> PublishVersionAsync(Guid packageId, Guid publisherId, VersionPublishRequest versionRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates version status (publish, deprecate, restore)
    /// </summary>
    /// <param name="versionId">The version identifier</param>
    /// <param name="publisherId">The publisher identifier (for authorization)</param>
    /// <param name="status">New version status</param>
    /// <param name="reason">Reason for status change</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Status update result</returns>
    Task<VersionStatusResult> UpdateVersionStatusAsync(Guid versionId, Guid publisherId, VersionStatus status, string? reason = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets package dependencies and compatibility information
    /// </summary>
    /// <param name="packageId">The package identifier</param>
    /// <param name="versionId">Optional specific version identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Dependency information</returns>
    Task<PackageDependencyInfo> GetPackageDependenciesAsync(Guid packageId, Guid? versionId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates package dependencies and compatibility settings
    /// </summary>
    /// <param name="packageId">The package identifier</param>
    /// <param name="publisherId">The publisher identifier (for authorization)</param>
    /// <param name="dependencies">Dependency update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Dependency update result</returns>
    Task<DependencyUpdateResult> UpdatePackageDependenciesAsync(Guid packageId, Guid publisherId, DependencyUpdateRequest dependencies, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets package usage statistics and analytics
    /// </summary>
    /// <param name="packageId">The package identifier</param>
    /// <param name="publisherId">The publisher identifier (for authorization)</param>
    /// <param name="timeRange">Time range for statistics</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Package usage statistics</returns>
    Task<PackageUsageStatistics> GetPackageStatisticsAsync(Guid packageId, Guid publisherId, string timeRange = "30d", CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets package settings and configuration options
    /// </summary>
    /// <param name="packageId">The package identifier</param>
    /// <param name="publisherId">The publisher identifier (for authorization)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Package settings</returns>
    Task<PackageSettings> GetPackageSettingsAsync(Guid packageId, Guid publisherId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates package settings and configuration
    /// </summary>
    /// <param name="packageId">The package identifier</param>
    /// <param name="publisherId">The publisher identifier (for authorization)</param>
    /// <param name="settings">Updated package settings</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Settings update result</returns>
    Task<SettingsUpdateResult> UpdatePackageSettingsAsync(Guid packageId, Guid publisherId, PackageSettingsRequest settings, CancellationToken cancellationToken = default);

    /// <summary>
    /// Initiates package deletion process (soft delete with confirmation)
    /// </summary>
    /// <param name="packageId">The package identifier</param>
    /// <param name="publisherId">The publisher identifier (for authorization)</param>
    /// <param name="reason">Reason for deletion</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Deletion initiation result</returns>
    Task<PackageDeletionResult> InitiatePackageDeletionAsync(Guid packageId, Guid publisherId, string reason, CancellationToken cancellationToken = default);

    /// <summary>
    /// Confirms package deletion after review period
    /// </summary>
    /// <param name="packageId">The package identifier</param>
    /// <param name="publisherId">The publisher identifier (for authorization)</param>
    /// <param name="confirmationToken">Deletion confirmation token</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Deletion confirmation result</returns>
    Task<PackageDeletionResult> ConfirmPackageDeletionAsync(Guid packageId, Guid publisherId, string confirmationToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Restores a soft-deleted package
    /// </summary>
    /// <param name="packageId">The package identifier</param>
    /// <param name="publisherId">The publisher identifier (for authorization)</param>
    /// <param name="reason">Reason for restoration</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Restoration result</returns>
    Task<PackageRestorationResult> RestorePackageAsync(Guid packageId, Guid publisherId, string reason, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets package collaborators and access permissions
    /// </summary>
    /// <param name="packageId">The package identifier</param>
    /// <param name="publisherId">The publisher identifier (for authorization)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Package collaborator information</returns>
    Task<PackageCollaborators> GetPackageCollaboratorsAsync(Guid packageId, Guid publisherId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Manages package collaborator permissions
    /// </summary>
    /// <param name="packageId">The package identifier</param>
    /// <param name="publisherId">The publisher identifier (for authorization)</param>
    /// <param name="collaboratorRequest">Collaborator management request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collaborator management result</returns>
    Task<CollaboratorResult> ManageCollaboratorAsync(Guid packageId, Guid publisherId, CollaboratorRequest collaboratorRequest, CancellationToken cancellationToken = default);
}

/// <summary>
/// Paginated package listing result
/// </summary>
public class PackageListResult {
    public IEnumerable<PackageListItem> Packages { get; set; } = [];
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
}

/// <summary>
/// Package list item with management information
/// </summary>
public class PackageListItem {
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string LatestVersion { get; set; } = string.Empty;
    public PackageStatus Status { get; set; }
    public long WeeklyDownloads { get; set; }
    public double Rating { get; set; }
    public string SecurityGrade { get; set; } = string.Empty;
    public double SecurityScore { get; set; }
    public TrustTier TrustTier { get; set; }
    public DateTime LastUpdated { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool HasActiveAlerts { get; set; }
    public int ActiveAlertCount { get; set; }
    public bool IsTrending { get; set; }
    public double GrowthPercentage { get; set; }
    public IEnumerable<string> Tags { get; set; } = [];
}

/// <summary>
/// Detailed package management information
/// </summary>
public class PackageManagementDetails {
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string LongDescription { get; set; } = string.Empty;
    public PackageStatus Status { get; set; }
    public string LatestVersion { get; set; } = string.Empty;
    public int TotalVersions { get; set; }
    public long TotalDownloads { get; set; }
    public long WeeklyDownloads { get; set; }
    public double Rating { get; set; }
    public int ReviewCount { get; set; }
    public double SecurityScore { get; set; }
    public string SecurityGrade { get; set; } = string.Empty;
    public TrustTier TrustTier { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastUpdated { get; set; }
    public IEnumerable<string> Tags { get; set; } = [];
    public IEnumerable<string> Categories { get; set; } = [];
    public string? HomepageUrl { get; set; }
    public string? RepositoryUrl { get; set; }
    public string? DocumentationUrl { get; set; }
    public string? License { get; set; }
    public PackageSettings Settings { get; set; } = new();
    public IEnumerable<SecurityAlert> ActiveAlerts { get; set; } = [];
}

/// <summary>
/// Package update request
/// </summary>
public class PackageUpdateRequest {
    public string? Description { get; set; }
    public string? LongDescription { get; set; }
    public IEnumerable<string>? Tags { get; set; }
    public IEnumerable<string>? Categories { get; set; }
    public string? HomepageUrl { get; set; }
    public string? RepositoryUrl { get; set; }
    public string? DocumentationUrl { get; set; }
    public string? License { get; set; }
    public bool? IsPrivate { get; set; }
    public bool? AllowCommunityContributions { get; set; }
}

/// <summary>
/// Package update result
/// </summary>
public class PackageUpdateResult {
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public IEnumerable<string> ValidationErrors { get; set; } = [];
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Package version management information
/// </summary>
public class PackageVersionManagement {
    public Guid Id { get; set; }
    public string Version { get; set; } = string.Empty;
    public VersionStatus Status { get; set; }
    public bool IsLatest { get; set; }
    public bool IsPrerelease { get; set; }
    public long Downloads { get; set; }
    public DateTime PublishedAt { get; set; }
    public string? ReleaseNotes { get; set; }
    public double? SecurityScore { get; set; }
    public string? SecurityGrade { get; set; }
    public IEnumerable<string> BreakingChanges { get; set; } = [];
    public IEnumerable<SecurityAlert> SecurityIssues { get; set; } = [];
    public bool CanDeprecate { get; set; }
    public bool CanRestore { get; set; }
    public string? DeprecationReason { get; set; }
}

/// <summary>
/// Version publish request
/// </summary>
public class VersionPublishRequest {
    public string Version { get; set; } = string.Empty;
    public string? ReleaseNotes { get; set; }
    public bool IsPrerelease { get; set; }
    public IEnumerable<string> BreakingChanges { get; set; } = [];
    public byte[] PackageData { get; set; } = [];
    public string? PackageHash { get; set; }
    public bool AutoPublish { get; set; } = true;
}

/// <summary>
/// Version publish result
/// </summary>
public class VersionPublishResult {
    public bool Success { get; set; }
    public Guid? VersionId { get; set; }
    public string? ErrorMessage { get; set; }
    public IEnumerable<string> ValidationErrors { get; set; } = [];
    public IEnumerable<string> Warnings { get; set; } = [];
    public SecurityScanResult? SecurityScanResult { get; set; }
    public DateTime? PublishedAt { get; set; }
}

/// <summary>
/// Version status update result
/// </summary>
public class VersionStatusResult {
    public bool Success { get; set; }
    public VersionStatus NewStatus { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Package dependency information
/// </summary>
public class PackageDependencyInfo {
    public Guid PackageId { get; set; }
    public Guid? VersionId { get; set; }
    public IEnumerable<PackageDependency> Dependencies { get; set; } = [];
    public IEnumerable<PackageDependency> DevDependencies { get; set; } = [];
    public IEnumerable<DependencyConflict> Conflicts { get; set; } = [];
    public IEnumerable<DependencyVulnerability> Vulnerabilities { get; set; } = [];
    public bool HasOutdatedDependencies { get; set; }
    public bool HasSecurityIssues { get; set; }
}

/// <summary>
/// Package dependency
/// </summary>
public class PackageDependency {
    public string Name { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string LatestVersion { get; set; } = string.Empty;
    public bool IsOutdated { get; set; }
    public bool HasSecurityIssues { get; set; }
    public bool IsOptional { get; set; }
    public string? License { get; set; }
    public string? Repository { get; set; }
}

/// <summary>
/// Dependency conflict
/// </summary>
public class DependencyConflict {
    public string PackageName { get; set; } = string.Empty;
    public string RequiredVersion { get; set; } = string.Empty;
    public string ConflictingVersion { get; set; } = string.Empty;
    public string ConflictingPackage { get; set; } = string.Empty;
    public ConflictSeverity Severity { get; set; }
}

/// <summary>
/// Dependency vulnerability
/// </summary>
public class DependencyVulnerability {
    public string PackageName { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string VulnerabilityId { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public SecurityScanSeverity Severity { get; set; }
    public string? FixedVersion { get; set; }
    public string? CveId { get; set; }
}

/// <summary>
/// Dependency update request
/// </summary>
public class DependencyUpdateRequest {
    public IEnumerable<DependencyUpdate> Updates { get; set; } = [];
    public bool UpdateAll { get; set; }
    public bool IncludeDevDependencies { get; set; }
    public bool OnlySecurityUpdates { get; set; }
}

/// <summary>
/// Dependency update
/// </summary>
public class DependencyUpdate {
    public string Name { get; set; } = string.Empty;
    public string FromVersion { get; set; } = string.Empty;
    public string ToVersion { get; set; } = string.Empty;
    public DependencyUpdateType Type { get; set; }
}

/// <summary>
/// Dependency update result
/// </summary>
public class DependencyUpdateResult {
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public IEnumerable<DependencyUpdate> AppliedUpdates { get; set; } = [];
    public IEnumerable<DependencyUpdate> FailedUpdates { get; set; } = [];
    public IEnumerable<string> Warnings { get; set; } = [];
}

/// <summary>
/// Package usage statistics
/// </summary>
public class PackageUsageStatistics {
    public Guid PackageId { get; set; }
    public long TotalDownloads { get; set; }
    public long WeeklyDownloads { get; set; }
    public long MonthlyDownloads { get; set; }
    public double GrowthPercentage { get; set; }
    public IEnumerable<DataPoint> DownloadTrend { get; set; } = [];
    public IEnumerable<VersionUsage> VersionBreakdown { get; set; } = [];
    public IEnumerable<CountryDownloads> GeographicDistribution { get; set; } = [];
    public double AverageRating { get; set; }
    public int ReviewCount { get; set; }
    public DateTime LastDownload { get; set; }
    public string MostPopularVersion { get; set; } = string.Empty;
}

/// <summary>
/// Version usage statistics
/// </summary>
public class VersionUsage {
    public string Version { get; set; } = string.Empty;
    public long Downloads { get; set; }
    public double Percentage { get; set; }
    public bool IsLatest { get; set; }
}

/// <summary>
/// Package settings
/// </summary>
public class PackageSettings {
    public bool IsPrivate { get; set; }
    public bool AllowCommunityContributions { get; set; }
    public bool EnableSecurityScanning { get; set; }
    public bool AutoPublishOnTag { get; set; }
    public bool SendNotificationOnDownloadMilestone { get; set; }
    public bool SendNotificationOnSecurityAlert { get; set; }
    public IEnumerable<string> NotificationEmails { get; set; } = [];
    public string? CustomLicense { get; set; }
    public bool EnableAnalytics { get; set; }
    public bool EnableTelemetry { get; set; }
    public IEnumerable<string> BlockedCountries { get; set; } = [];
    public IEnumerable<string> AllowedDomains { get; set; } = [];
}

/// <summary>
/// Package settings update request
/// </summary>
public class PackageSettingsRequest {
    public bool? IsPrivate { get; set; }
    public bool? AllowCommunityContributions { get; set; }
    public bool? EnableSecurityScanning { get; set; }
    public bool? AutoPublishOnTag { get; set; }
    public bool? SendNotificationOnDownloadMilestone { get; set; }
    public bool? SendNotificationOnSecurityAlert { get; set; }
    public IEnumerable<string>? NotificationEmails { get; set; }
    public string? CustomLicense { get; set; }
    public bool? EnableAnalytics { get; set; }
    public bool? EnableTelemetry { get; set; }
    public IEnumerable<string>? BlockedCountries { get; set; }
    public IEnumerable<string>? AllowedDomains { get; set; }
}

/// <summary>
/// Settings update result
/// </summary>
public class SettingsUpdateResult {
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public IEnumerable<string> ValidationErrors { get; set; } = [];
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Package deletion result
/// </summary>
public class PackageDeletionResult {
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public string? ConfirmationToken { get; set; }
    public DateTime? DeletionScheduledAt { get; set; }
    public DateTime? DeletionConfirmedAt { get; set; }
    public bool RequiresConfirmation { get; set; }
    public IEnumerable<string> Warnings { get; set; } = [];
}

/// <summary>
/// Package restoration result
/// </summary>
public class PackageRestorationResult {
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime? RestoredAt { get; set; }
    public PackageStatus NewStatus { get; set; }
}

/// <summary>
/// Package collaborators information
/// </summary>
public class PackageCollaborators {
    public Guid PackageId { get; set; }
    public Guid OwnerId { get; set; }
    public string OwnerName { get; set; } = string.Empty;
    public IEnumerable<PackageCollaborator> Collaborators { get; set; } = [];
    public IEnumerable<PendingInvitation> PendingInvitations { get; set; } = [];
    public bool CanManageCollaborators { get; set; }
}

/// <summary>
/// Package collaborator
/// </summary>
public class PackageCollaborator {
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public CollaboratorRole Role { get; set; }
    public DateTime AddedAt { get; set; }
    public DateTime? LastActivity { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>
/// Pending collaborator invitation
/// </summary>
public class PendingInvitation {
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public CollaboratorRole Role { get; set; }
    public DateTime InvitedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public string InvitedBy { get; set; } = string.Empty;
}

/// <summary>
/// Collaborator management request
/// </summary>
public class CollaboratorRequest {
    public CollaboratorAction Action { get; set; }
    public Guid? UserId { get; set; }
    public string? Email { get; set; }
    public CollaboratorRole Role { get; set; }
    public string? Message { get; set; }
}

/// <summary>
/// Collaborator management result
/// </summary>
public class CollaboratorResult {
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public CollaboratorAction Action { get; set; }
    public DateTime ActionDate { get; set; }
}

/// <summary>
/// Conflict severity levels
/// </summary>
public enum ConflictSeverity {
    Low,
    Medium,
    High,
    Critical
}

/// <summary>
/// Dependency update types
/// </summary>
public enum DependencyUpdateType {
    Patch,
    Minor,
    Major,
    Security
}

/// <summary>
/// Collaborator roles
/// </summary>
public enum CollaboratorRole {
    Maintainer,
    Developer,
    Contributor,
    Viewer
}

/// <summary>
/// Collaborator actions
/// </summary>
public enum CollaboratorAction {
    Invite,
    UpdateRole,
    Remove,
    ResendInvitation,
    CancelInvitation
}