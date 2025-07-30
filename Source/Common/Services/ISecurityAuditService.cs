namespace MCPHub.Common.Services;

/// <summary>
/// Security audit service for comprehensive audit logging and compliance tracking
/// Supports: Database → File → SIEM → Compliance Systems
/// </summary>
public interface ISecurityAuditService {
    /// <summary>
    /// Creates an audit entry for a security-relevant action
    /// </summary>
    /// <param name="auditRequest">Audit entry request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Audit entry result</returns>
    Task<SecurityAuditResult> CreateAuditEntryAsync(SecurityAuditRequest auditRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates multiple audit entries in batch
    /// </summary>
    /// <param name="auditRequests">Collection of audit requests</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Batch audit result</returns>
    Task<SecurityAuditBatchResult> CreateAuditEntriesAsync(IEnumerable<SecurityAuditRequest> auditRequests, CancellationToken cancellationToken = default);

    /// <summary>
    /// Queries audit entries based on criteria
    /// </summary>
    /// <param name="criteria">Query criteria</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Matching audit entries</returns>
    Task<SecurityAuditQueryResult> QueryAuditEntriesAsync(SecurityAuditQueryCriteria criteria, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets audit trail for a specific resource
    /// </summary>
    /// <param name="resourceType">Resource type (Package, Server, User, etc.)</param>
    /// <param name="resourceId">Resource identifier</param>
    /// <param name="timeRange">Optional time range filter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Resource audit trail</returns>
    Task<IReadOnlyList<SecurityAuditEntry>> GetResourceAuditTrailAsync(
        string resourceType,
        Guid resourceId,
        DateTimeRange? timeRange = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets audit trail for a specific user's actions
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="actionTypes">Optional action types to filter</param>
    /// <param name="timeRange">Optional time range filter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User audit trail</returns>
    Task<IReadOnlyList<SecurityAuditEntry>> GetUserAuditTrailAsync(
        Guid userId,
        IEnumerable<SecurityAuditAction>? actionTypes = null,
        DateTimeRange? timeRange = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates compliance reports for audit data
    /// </summary>
    /// <param name="reportRequest">Compliance report request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Compliance report result</returns>
    Task<ComplianceReportResult> GenerateComplianceReportAsync(ComplianceReportRequest reportRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Exports audit data in various formats
    /// </summary>
    /// <param name="exportRequest">Export request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Export result with download information</returns>
    Task<SecurityAuditExportResult> ExportAuditDataAsync(SecurityAuditExportRequest exportRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets audit statistics for a given time period
    /// </summary>
    /// <param name="timeRange">Time range for statistics</param>
    /// <param name="groupBy">How to group the statistics</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Audit statistics</returns>
    Task<SecurityAuditStatistics> GetAuditStatisticsAsync(
        DateTimeRange timeRange,
        SecurityAuditGroupBy groupBy = SecurityAuditGroupBy.Action,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches audit entries using full-text search
    /// </summary>
    /// <param name="searchQuery">Search query</param>
    /// <param name="filters">Optional filters</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="pageNumber">Page number</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Search results</returns>
    Task<SecurityAuditSearchResult> SearchAuditEntriesAsync(
        string searchQuery,
        SecurityAuditFilters? filters = null,
        int pageSize = 50,
        int pageNumber = 1,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates audit integrity using cryptographic signatures
    /// </summary>
    /// <param name="validationRequest">Validation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Integrity validation result</returns>
    Task<AuditIntegrityValidationResult> ValidateAuditIntegrityAsync(AuditIntegrityValidationRequest validationRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Archives old audit entries based on retention policy
    /// </summary>
    /// <param name="archiveRequest">Archive request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Archive result</returns>
    Task<SecurityAuditArchiveResult> ArchiveAuditEntriesAsync(SecurityAuditArchiveRequest archiveRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets audit configuration settings
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Current audit configuration</returns>
    Task<SecurityAuditConfiguration> GetAuditConfigurationAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates audit configuration settings
    /// </summary>
    /// <param name="configuration">New audit configuration</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Configuration update result</returns>
    Task<SecurityAuditConfigurationResult> UpdateAuditConfigurationAsync(SecurityAuditConfiguration configuration, CancellationToken cancellationToken = default);
}

/// <summary>
/// Security audit request
/// </summary>
public record SecurityAuditRequest(
    SecurityAuditAction Action,
    SecurityAuditSeverity Severity,
    string Description,
    Guid? UserId = null,
    string? ResourceType = null,
    Guid? ResourceId = null,
    string? IpAddress = null,
    string? UserAgent = null,
    IDictionary<string, object>? Metadata = null,
    DateTimeOffset? OccurredAt = null);

/// <summary>
/// Security audit result
/// </summary>
public record SecurityAuditResult(
    Guid AuditId,
    bool Success,
    string? ErrorMessage = null);

/// <summary>
/// Batch security audit result
/// </summary>
public record SecurityAuditBatchResult(
    int TotalEntries,
    int SuccessfulEntries,
    int FailedEntries,
    IReadOnlyList<SecurityAuditResult> Results);

/// <summary>
/// Security audit query criteria
/// </summary>
public record SecurityAuditQueryCriteria(
    IEnumerable<SecurityAuditAction>? Actions = null,
    IEnumerable<SecurityAuditSeverity>? Severities = null,
    Guid? UserId = null,
    string? ResourceType = null,
    Guid? ResourceId = null,
    string? IpAddress = null,
    DateTimeRange? TimeRange = null,
    int PageSize = 50,
    int PageNumber = 1,
    SecurityAuditSortBy SortBy = SecurityAuditSortBy.OccurredAt,
    SortDirection SortDirection = SortDirection.Descending);

/// <summary>
/// Security audit query result
/// </summary>
public record SecurityAuditQueryResult(
    IReadOnlyList<SecurityAuditEntry> Entries,
    int TotalCount,
    int PageSize,
    int PageNumber,
    int TotalPages);

/// <summary>
/// Security audit entry
/// </summary>
public record SecurityAuditEntry(
    Guid AuditId,
    SecurityAuditAction Action,
    SecurityAuditSeverity Severity,
    string Description,
    Guid? UserId,
    string? Username,
    string? ResourceType,
    Guid? ResourceId,
    string? ResourceName,
    string? IpAddress,
    string? UserAgent,
    IDictionary<string, object> Metadata,
    DateTimeOffset OccurredAt,
    string? IntegrityHash = null);

/// <summary>
/// Compliance report request
/// </summary>
public record ComplianceReportRequest(
    ComplianceStandard Standard,
    DateTimeRange TimeRange,
    Guid? OrganizationId = null,
    IEnumerable<string>? ResourceTypes = null,
    bool IncludeDetails = true);

/// <summary>
/// Compliance report result
/// </summary>
public record ComplianceReportResult(
    Guid ReportId,
    ComplianceStandard Standard,
    DateTimeRange TimeRange,
    ComplianceStatus OverallStatus,
    IDictionary<string, ComplianceControlResult> ControlResults,
    int TotalAuditEntries,
    int ComplianceViolations,
    string? DownloadUrl = null,
    DateTimeOffset GeneratedAt = default);

/// <summary>
/// Compliance control result
/// </summary>
public record ComplianceControlResult(
    string ControlId,
    string ControlName,
    ComplianceStatus Status,
    int TotalChecks,
    int PassedChecks,
    int FailedChecks,
    IReadOnlyList<string> Violations);

/// <summary>
/// Security audit export request
/// </summary>
public record SecurityAuditExportRequest(
    SecurityAuditQueryCriteria Criteria,
    SecurityAuditExportFormat Format,
    bool IncludeMetadata = true,
    bool IncludeIntegrityHashes = false,
    string? FileName = null);

/// <summary>
/// Security audit export result
/// </summary>
public record SecurityAuditExportResult(
    bool Success,
    string? DownloadUrl = null,
    string? FileName = null,
    long? FileSizeBytes = null,
    string? ErrorMessage = null);

/// <summary>
/// Security audit statistics
/// </summary>
public record SecurityAuditStatistics(
    DateTimeRange TimeRange,
    int TotalEntries,
    IDictionary<SecurityAuditAction, int> EntriesByAction,
    IDictionary<SecurityAuditSeverity, int> EntriesBySeverity,
    IDictionary<string, int> EntriesByResourceType,
    IDictionary<string, int> TopUsers,
    IDictionary<string, int> TopIpAddresses,
    IDictionary<string, int> EntriesByHour);

/// <summary>
/// Security audit search filters
/// </summary>
public record SecurityAuditFilters(
    IEnumerable<SecurityAuditAction>? Actions = null,
    IEnumerable<SecurityAuditSeverity>? Severities = null,
    DateTimeRange? TimeRange = null,
    string? UserId = null,
    string? ResourceType = null,
    string? IpAddress = null);

/// <summary>
/// Security audit search result
/// </summary>
public record SecurityAuditSearchResult(
    IReadOnlyList<SecurityAuditEntry> Entries,
    int TotalCount,
    string SearchQuery,
    SecurityAuditFilters? Filters,
    int PageSize,
    int PageNumber,
    int TotalPages,
    double SearchDurationMs);

/// <summary>
/// Audit integrity validation request
/// </summary>
public record AuditIntegrityValidationRequest(
    DateTimeRange? TimeRange = null,
    IEnumerable<Guid>? AuditIds = null,
    bool ValidateAllEntries = false);

/// <summary>
/// Audit integrity validation result
/// </summary>
public record AuditIntegrityValidationResult(
    bool IsValid,
    int TotalValidated,
    int ValidEntries,
    int InvalidEntries,
    IReadOnlyList<Guid> TamperedEntries,
    DateTimeOffset ValidatedAt);

/// <summary>
/// Security audit archive request
/// </summary>
public record SecurityAuditArchiveRequest(
    DateTimeOffset ArchiveBefore,
    SecurityAuditArchiveDestination Destination,
    bool DeleteAfterArchive = false);

/// <summary>
/// Security audit archive result
/// </summary>
public record SecurityAuditArchiveResult(
    bool Success,
    int ArchivedEntries,
    long ArchiveSizeBytes,
    string? ArchiveLocation = null,
    string? ErrorMessage = null);

/// <summary>
/// Security audit configuration
/// </summary>
public record SecurityAuditConfiguration(
    bool IsEnabled,
    SecurityAuditSeverity MinimumSeverityLevel,
    TimeSpan RetentionPeriod,
    bool EnableIntegrityChecking,
    bool EnableEncryption,
    SecurityAuditArchiveDestination ArchiveDestination,
    IReadOnlyList<SecurityAuditAction> ExcludedActions,
    bool LogMetadata,
    int MaxBatchSize);

/// <summary>
/// Security audit configuration result
/// </summary>
public record SecurityAuditConfigurationResult(
    bool Success,
    string? ErrorMessage = null);

/// <summary>
/// Security audit actions
/// </summary>
public enum SecurityAuditAction {
    UserLogin,
    UserLogout,
    UserRegistration,
    UserDeactivation,
    PasswordChange,
    PasswordReset,
    ProfileUpdate,
    RoleAssignment,
    PermissionGrant,
    PermissionRevoke,
    ResourceCreated,
    ResourceUpdated,
    ResourceDeleted,
    ResourceAccessed,
    PolicyCreated,
    PolicyUpdated,
    PolicyDeleted,
    PolicyViolation,
    SecurityScanStarted,
    SecurityScanCompleted,
    ThreatDetected,
    IncidentCreated,
    IncidentResolved,
    ComplianceViolation,
    DataExport,
    DataImport,
    ConfigurationChange,
    ApiKeyCreated,
    ApiKeyRevoked,
    SystemStartup,
    SystemShutdown,
}

/// <summary>
/// Security audit severity levels
/// </summary>
public enum SecurityAuditSeverity {
    Informational = 0,
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4,
}

/// <summary>
/// Security audit grouping options
/// </summary>
public enum SecurityAuditGroupBy {
    Action,
    Severity,
    User,
    ResourceType,
    Hour,
    Day,
}

/// <summary>
/// Security audit sorting options
/// </summary>
public enum SecurityAuditSortBy {
    OccurredAt,
    Action,
    Severity,
    UserId,
    ResourceType,
}

/// <summary>
/// Security audit export formats
/// </summary>
public enum SecurityAuditExportFormat {
    Json,
    Csv,
    Xml,
    Siem,
    Excel,
}

/// <summary>
/// Compliance standards
/// </summary>
public enum ComplianceStandard {
    SOC2,
    ISO27001,
    GDPR,
    HIPAA,
    PCI_DSS,
    SOX,
    Custom,
}

/// <summary>
/// Compliance status
/// </summary>
public enum ComplianceStatus {
    Compliant,
    NonCompliant,
    PartiallyCompliant,
    NotAssessed,
}

/// <summary>
/// Security audit archive destinations
/// </summary>
public enum SecurityAuditArchiveDestination {
    LocalFile,
    CloudStorage,
    ExternalSiem,
    ComplianceSystem,
}