namespace MCPHub.Common.Services;

/// <summary>
/// Security event logging service for comprehensive security event tracking
/// Supports: In-Memory → Database → SIEM → Enterprise Security Platform
/// </summary>
public interface ISecurityEventService {
    /// <summary>
    /// Logs a security event
    /// </summary>
    /// <param name="securityEvent">Security event to log</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Event logging result</returns>
    Task<SecurityEventResult> LogEventAsync(SecurityEventRequest securityEvent, CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs multiple security events in batch
    /// </summary>
    /// <param name="securityEvents">Security events to log</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Batch logging result</returns>
    Task<SecurityEventBatchResult> LogEventsAsync(IEnumerable<SecurityEventRequest> securityEvents, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves security events based on criteria
    /// </summary>
    /// <param name="criteria">Query criteria</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Matching security events</returns>
    Task<SecurityEventQueryResult> QueryEventsAsync(SecurityEventQueryCriteria criteria, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets security events for a specific user
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="eventTypes">Optional event types to filter</param>
    /// <param name="timeRange">Optional time range</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User security events</returns>
    Task<IReadOnlyList<SecurityEventDetails>> GetUserEventsAsync(
        Guid userId,
        IEnumerable<SecurityEventType>? eventTypes = null,
        DateTimeRange? timeRange = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets security events for a specific resource
    /// </summary>
    /// <param name="resourceType">Resource type (Package, Server, etc.)</param>
    /// <param name="resourceId">Resource identifier</param>
    /// <param name="eventTypes">Optional event types to filter</param>
    /// <param name="timeRange">Optional time range</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Resource security events</returns>
    Task<IReadOnlyList<SecurityEventDetails>> GetResourceEventsAsync(
        string resourceType,
        Guid resourceId,
        IEnumerable<SecurityEventType>? eventTypes = null,
        DateTimeRange? timeRange = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets security event statistics
    /// </summary>
    /// <param name="timeRange">Time range for statistics</param>
    /// <param name="groupBy">How to group statistics</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Security event statistics</returns>
    Task<SecurityEventStatistics> GetStatisticsAsync(
        DateTimeRange timeRange,
        SecurityEventGroupBy groupBy = SecurityEventGroupBy.EventType,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches security events using full-text search
    /// </summary>
    /// <param name="searchQuery">Search query</param>
    /// <param name="filters">Optional filters</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="pageNumber">Page number</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Search results</returns>
    Task<SecurityEventSearchResult> SearchEventsAsync(
        string searchQuery,
        SecurityEventFilters? filters = null,
        int pageSize = 50,
        int pageNumber = 1,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a security event correlation to link related events
    /// </summary>
    /// <param name="correlationRequest">Correlation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Correlation result</returns>
    Task<SecurityEventCorrelationResult> CreateCorrelationAsync(SecurityEventCorrelationRequest correlationRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Exports security events to external format (CSV, JSON, SIEM)
    /// </summary>
    /// <param name="exportRequest">Export request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Export result with download information</returns>
    Task<SecurityEventExportResult> ExportEventsAsync(SecurityEventExportRequest exportRequest, CancellationToken cancellationToken = default);
}

/// <summary>
/// Security event request for logging
/// </summary>
public record SecurityEventRequest(
    SecurityEventType EventType,
    SecurityEventSeverity Severity,
    string Message,
    Guid? UserId = null,
    string? ResourceType = null,
    Guid? ResourceId = null,
    string? IpAddress = null,
    string? UserAgent = null,
    IDictionary<string, object>? Metadata = null,
    DateTimeOffset? OccurredAt = null);

/// <summary>
/// Security event logging result
/// </summary>
public record SecurityEventResult(
    Guid EventId,
    bool Success,
    string? ErrorMessage = null);

/// <summary>
/// Batch security event logging result
/// </summary>
public record SecurityEventBatchResult(
    int TotalEvents,
    int SuccessfulEvents,
    int FailedEvents,
    IReadOnlyList<SecurityEventResult> Results);

/// <summary>
/// Security event query criteria
/// </summary>
public record SecurityEventQueryCriteria(
    IEnumerable<SecurityEventType>? EventTypes = null,
    IEnumerable<SecurityEventSeverity>? Severities = null,
    Guid? UserId = null,
    string? ResourceType = null,
    Guid? ResourceId = null,
    string? IpAddress = null,
    DateTimeRange? TimeRange = null,
    int PageSize = 50,
    int PageNumber = 1,
    SecurityEventSortBy SortBy = SecurityEventSortBy.OccurredAt,
    SortDirection SortDirection = SortDirection.Descending);

/// <summary>
/// Security event query result
/// </summary>
public record SecurityEventQueryResult(
    IReadOnlyList<SecurityEventDetails> Events,
    int TotalCount,
    int PageSize,
    int PageNumber,
    int TotalPages);

/// <summary>
/// Detailed security event information
/// </summary>
public record SecurityEventDetails(
    Guid EventId,
    SecurityEventType EventType,
    SecurityEventSeverity Severity,
    string Message,
    Guid? UserId,
    string? Username,
    string? ResourceType,
    Guid? ResourceId,
    string? ResourceName,
    string? IpAddress,
    string? UserAgent,
    IDictionary<string, object> Metadata,
    DateTimeOffset OccurredAt,
    Guid? CorrelationId = null);

/// <summary>
/// Date and time range for queries
/// </summary>
public record DateTimeRange(
    DateTimeOffset StartTime,
    DateTimeOffset EndTime);

/// <summary>
/// Security event statistics
/// </summary>
public record SecurityEventStatistics(
    DateTimeRange TimeRange,
    int TotalEvents,
    IDictionary<SecurityEventType, int> EventsByType,
    IDictionary<SecurityEventSeverity, int> EventsBySeverity,
    IDictionary<string, int> EventsByHour,
    IDictionary<string, int> TopUsers,
    IDictionary<string, int> TopIpAddresses,
    IDictionary<string, int> TopResourceTypes);

/// <summary>
/// Security event search filters
/// </summary>
public record SecurityEventFilters(
    IEnumerable<SecurityEventType>? EventTypes = null,
    IEnumerable<SecurityEventSeverity>? Severities = null,
    DateTimeRange? TimeRange = null,
    string? UserId = null,
    string? ResourceType = null,
    string? IpAddress = null);

/// <summary>
/// Security event search result
/// </summary>
public record SecurityEventSearchResult(
    IReadOnlyList<SecurityEventDetails> Events,
    int TotalCount,
    string SearchQuery,
    SecurityEventFilters? Filters,
    int PageSize,
    int PageNumber,
    int TotalPages,
    double SearchDurationMs);

/// <summary>
/// Security event correlation request
/// </summary>
public record SecurityEventCorrelationRequest(
    IReadOnlyList<Guid> EventIds,
    string CorrelationType,
    string Description,
    IDictionary<string, object>? Metadata = null);

/// <summary>
/// Security event correlation result
/// </summary>
public record SecurityEventCorrelationResult(
    Guid CorrelationId,
    bool Success,
    string? ErrorMessage = null);

/// <summary>
/// Security event export request
/// </summary>
public record SecurityEventExportRequest(
    SecurityEventQueryCriteria Criteria,
    SecurityEventExportFormat Format,
    bool IncludeMetadata = true,
    string? FileName = null);

/// <summary>
/// Security event export result
/// </summary>
public record SecurityEventExportResult(
    bool Success,
    string? DownloadUrl = null,
    string? FileName = null,
    long? FileSizeBytes = null,
    string? ErrorMessage = null);

/// <summary>
/// Security event types
/// </summary>
public enum SecurityEventType {
    Authentication,
    Authorization,
    DataAccess,
    DataModification,
    PolicyViolation,
    ThreatDetected,
    IncidentCreated,
    IncidentResolved,
    ConfigurationChange,
    ApiKeyCreated,
    ApiKeyRevoked,
    PackagePublished,
    PackageDeleted,
    ServerRegistered,
    ServerDeregistered,
    ScanCompleted,
    TrustTierUpdated,
    ComplianceAudit,
}

/// <summary>
/// Security event severity levels
/// </summary>
public enum SecurityEventSeverity {
    Informational = 0,
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4,
}

/// <summary>
/// Security event grouping options
/// </summary>
public enum SecurityEventGroupBy {
    EventType,
    Severity,
    User,
    ResourceType,
    Hour,
    Day,
}

/// <summary>
/// Security event sorting options
/// </summary>
public enum SecurityEventSortBy {
    OccurredAt,
    EventType,
    Severity,
    UserId,
    ResourceType,
}

/// <summary>
/// Security event export formats
/// </summary>
public enum SecurityEventExportFormat {
    Json,
    Csv,
    Siem,
    Excel,
}

/// <summary>
/// Sort direction enumeration
/// </summary>
public enum SortDirection {
    Ascending,
    Descending,
}