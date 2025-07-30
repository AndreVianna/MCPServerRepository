namespace MCPHub.Common.Services;

/// <summary>
/// Incident response service for automated security incident handling and response
/// Supports: Manual → Automated → AI-Enhanced → Enterprise SOAR Integration
/// </summary>
public interface IIncidentResponseService {
    /// <summary>
    /// Creates a new security incident
    /// </summary>
    /// <param name="incidentRequest">Incident creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created incident result</returns>
    Task<SecurityIncidentResult> CreateIncidentAsync(CreateSecurityIncidentRequest incidentRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing security incident
    /// </summary>
    /// <param name="incidentId">Incident identifier</param>
    /// <param name="updateRequest">Incident update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated incident result</returns>
    Task<SecurityIncidentResult> UpdateIncidentAsync(Guid incidentId, UpdateSecurityIncidentRequest updateRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Assigns an incident to a user or team
    /// </summary>
    /// <param name="incidentId">Incident identifier</param>
    /// <param name="assignmentRequest">Assignment request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Assignment result</returns>
    Task<IncidentAssignmentResult> AssignIncidentAsync(Guid incidentId, IncidentAssignmentRequest assignmentRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Escalates an incident to higher priority or different team
    /// </summary>
    /// <param name="incidentId">Incident identifier</param>
    /// <param name="escalationRequest">Escalation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Escalation result</returns>
    Task<IncidentEscalationResult> EscalateIncidentAsync(Guid incidentId, IncidentEscalationRequest escalationRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Resolves a security incident
    /// </summary>
    /// <param name="incidentId">Incident identifier</param>
    /// <param name="resolutionRequest">Resolution request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Resolution result</returns>
    Task<IncidentResolutionResult> ResolveIncidentAsync(Guid incidentId, IncidentResolutionRequest resolutionRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets details of a specific incident
    /// </summary>
    /// <param name="incidentId">Incident identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Incident details or null if not found</returns>
    Task<SecurityIncidentDetails?> GetIncidentAsync(Guid incidentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Queries incidents based on criteria
    /// </summary>
    /// <param name="criteria">Query criteria</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Matching incidents</returns>
    Task<SecurityIncidentQueryResult> QueryIncidentsAsync(SecurityIncidentQueryCriteria criteria, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets active incidents that require attention
    /// </summary>
    /// <param name="userId">Optional user ID to filter assigned incidents</param>
    /// <param name="teamId">Optional team ID to filter team incidents</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Active incidents</returns>
    Task<IReadOnlyList<SecurityIncidentSummary>> GetActiveIncidentsAsync(Guid? userId = null, Guid? teamId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes automated response actions for an incident
    /// </summary>
    /// <param name="incidentId">Incident identifier</param>
    /// <param name="responseActions">List of response actions to execute</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Automated response result</returns>
    Task<AutomatedResponseResult> ExecuteAutomatedResponseAsync(Guid incidentId, IEnumerable<IncidentResponseAction> responseActions, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates an incident response playbook
    /// </summary>
    /// <param name="playbookRequest">Playbook creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Playbook creation result</returns>
    Task<IncidentPlaybookResult> CreatePlaybookAsync(CreateIncidentPlaybookRequest playbookRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes an incident response playbook
    /// </summary>
    /// <param name="incidentId">Incident identifier</param>
    /// <param name="playbookId">Playbook identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Playbook execution result</returns>
    Task<PlaybookExecutionResult> ExecutePlaybookAsync(Guid incidentId, Guid playbookId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets incident response statistics
    /// </summary>
    /// <param name="timeRange">Time range for statistics</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Incident response statistics</returns>
    Task<IncidentResponseStatistics> GetStatisticsAsync(DateTimeRange timeRange, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates incident response team
    /// </summary>
    /// <param name="teamRequest">Team creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Team creation result</returns>
    Task<IncidentResponseTeamResult> CreateResponseTeamAsync(CreateIncidentResponseTeamRequest teamRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets available response teams
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Available response teams</returns>
    Task<IReadOnlyList<IncidentResponseTeam>> GetResponseTeamsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends notifications about incident status changes
    /// </summary>
    /// <param name="incidentId">Incident identifier</param>
    /// <param name="notificationRequest">Notification request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Notification result</returns>
    Task<IncidentNotificationResult> SendIncidentNotificationAsync(Guid incidentId, IncidentNotificationRequest notificationRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates incident response reports
    /// </summary>
    /// <param name="reportRequest">Report generation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Report generation result</returns>
    Task<IncidentResponseReportResult> GenerateIncidentReportAsync(IncidentResponseReportRequest reportRequest, CancellationToken cancellationToken = default);
}

/// <summary>
/// Create security incident request
/// </summary>
public record CreateSecurityIncidentRequest(
    string Title,
    string Description,
    IncidentSeverity Severity,
    IncidentType Type,
    Guid? DetectedById = null,
    string? SourceSystem = null,
    IDictionary<string, object>? Metadata = null,
    IEnumerable<string>? AffectedResources = null,
    DateTimeOffset? OccurredAt = null);

/// <summary>
/// Update security incident request
/// </summary>
public record UpdateSecurityIncidentRequest(
    string? Title = null,
    string? Description = null,
    IncidentSeverity? Severity = null,
    IncidentStatus? Status = null,
    IDictionary<string, object>? Metadata = null,
    IEnumerable<string>? AffectedResources = null);

/// <summary>
/// Security incident result
/// </summary>
public record SecurityIncidentResult(
    Guid IncidentId,
    bool Success,
    string? ErrorMessage = null);

/// <summary>
/// Incident assignment request
/// </summary>
public record IncidentAssignmentRequest(
    Guid? AssigneeId = null,
    Guid? TeamId = null,
    string AssignmentReason = "",
    bool NotifyAssignee = true);

/// <summary>
/// Incident assignment result
/// </summary>
public record IncidentAssignmentResult(
    bool Success,
    Guid? PreviousAssigneeId = null,
    string? ErrorMessage = null);

/// <summary>
/// Incident escalation request
/// </summary>
public record IncidentEscalationRequest(
    IncidentSeverity NewSeverity,
    Guid? EscalateToUserId = null,
    Guid? EscalateToTeamId = null,
    string EscalationReason = "",
    bool RequireApproval = false);

/// <summary>
/// Incident escalation result
/// </summary>
public record IncidentEscalationResult(
    bool Success,
    IncidentSeverity PreviousSeverity,
    string? ErrorMessage = null);

/// <summary>
/// Incident resolution request
/// </summary>
public record IncidentResolutionRequest(
    IncidentResolutionType ResolutionType,
    string ResolutionSummary,
    IEnumerable<string>? ActionsTaken = null,
    IEnumerable<string>? LessonsLearned = null,
    bool PreventRecurrence = false);

/// <summary>
/// Incident resolution result
/// </summary>
public record IncidentResolutionResult(
    bool Success,
    DateTimeOffset? ResolvedAt = null,
    TimeSpan? ResolutionTime = null,
    string? ErrorMessage = null);

/// <summary>
/// Security incident details
/// </summary>
public record SecurityIncidentDetails(
    Guid IncidentId,
    string Title,
    string Description,
    IncidentSeverity Severity,
    IncidentType Type,
    IncidentStatus Status,
    Guid? AssigneeId,
    string? AssigneeName,
    Guid? TeamId,
    string? TeamName,
    Guid? DetectedById,
    string? DetectedByName,
    string? SourceSystem,
    IReadOnlyList<string> AffectedResources,
    IDictionary<string, object> Metadata,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    DateTimeOffset? ResolvedAt,
    TimeSpan? ResolutionTime,
    IReadOnlyList<IncidentActivity> ActivityLog);

/// <summary>
/// Security incident query criteria
/// </summary>
public record SecurityIncidentQueryCriteria(
    IEnumerable<IncidentSeverity>? Severities = null,
    IEnumerable<IncidentType>? Types = null,
    IEnumerable<IncidentStatus>? Statuses = null,
    Guid? AssigneeId = null,
    Guid? TeamId = null,
    Guid? DetectedById = null,
    string? SourceSystem = null,
    DateTimeRange? TimeRange = null,
    int PageSize = 50,
    int PageNumber = 1,
    IncidentSortBy SortBy = IncidentSortBy.CreatedAt,
    SortDirection SortDirection = SortDirection.Descending);

/// <summary>
/// Security incident query result
/// </summary>
public record SecurityIncidentQueryResult(
    IReadOnlyList<SecurityIncidentSummary> Incidents,
    int TotalCount,
    int PageSize,
    int PageNumber,
    int TotalPages);

/// <summary>
/// Security incident summary
/// </summary>
public record SecurityIncidentSummary(
    Guid IncidentId,
    string Title,
    IncidentSeverity Severity,
    IncidentType Type,
    IncidentStatus Status,
    string? AssigneeName,
    string? TeamName,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    TimeSpan? Age);

/// <summary>
/// Incident activity log entry
/// </summary>
public record IncidentActivity(
    Guid ActivityId,
    IncidentActivityType ActivityType,
    string Description,
    Guid? UserId,
    string? Username,
    DateTimeOffset OccurredAt,
    IDictionary<string, object>? Metadata = null);

/// <summary>
/// Automated response result
/// </summary>
public record AutomatedResponseResult(
    bool Success,
    int ActionsExecuted,
    int ActionsSucceeded,
    int ActionsFailed,
    IReadOnlyList<ResponseActionResult> ActionResults,
    string? ErrorMessage = null);

/// <summary>
/// Response action result
/// </summary>
public record ResponseActionResult(
    IncidentResponseAction Action,
    bool Success,
    string? Result = null,
    string? ErrorMessage = null,
    TimeSpan ExecutionTime = default);

/// <summary>
/// Create incident playbook request
/// </summary>
public record CreateIncidentPlaybookRequest(
    string Name,
    string Description,
    IncidentType IncidentType,
    IEnumerable<PlaybookStep> Steps,
    bool IsActive = true);

/// <summary>
/// Incident playbook result
/// </summary>
public record IncidentPlaybookResult(
    Guid PlaybookId,
    bool Success,
    string? ErrorMessage = null);

/// <summary>
/// Playbook step
/// </summary>
public record PlaybookStep(
    int StepNumber,
    string Name,
    string Description,
    IncidentResponseAction Action,
    IDictionary<string, object>? Parameters = null,
    bool IsAutomated = false,
    TimeSpan? Timeout = null);

/// <summary>
/// Playbook execution result
/// </summary>
public record PlaybookExecutionResult(
    Guid ExecutionId,
    bool Success,
    int TotalSteps,
    int CompletedSteps,
    int FailedSteps,
    IReadOnlyList<PlaybookStepResult> StepResults,
    string? ErrorMessage = null);

/// <summary>
/// Playbook step result
/// </summary>
public record PlaybookStepResult(
    int StepNumber,
    string StepName,
    bool Success,
    string? Result = null,
    string? ErrorMessage = null,
    TimeSpan ExecutionTime = default);

/// <summary>
/// Incident response statistics
/// </summary>
public record IncidentResponseStatistics(
    DateTimeRange TimeRange,
    int TotalIncidents,
    int ResolvedIncidents,
    int ActiveIncidents,
    IDictionary<IncidentSeverity, int> IncidentsBySeverity,
    IDictionary<IncidentType, int> IncidentsByType,
    IDictionary<IncidentStatus, int> IncidentsByStatus,
    TimeSpan AverageResolutionTime,
    TimeSpan MedianResolutionTime,
    double IncidentResolutionRate);

/// <summary>
/// Create incident response team request
/// </summary>
public record CreateIncidentResponseTeamRequest(
    string Name,
    string Description,
    IEnumerable<Guid> MemberIds,
    IEnumerable<IncidentType> Specializations,
    bool IsActive = true);

/// <summary>
/// Incident response team result
/// </summary>
public record IncidentResponseTeamResult(
    Guid TeamId,
    bool Success,
    string? ErrorMessage = null);

/// <summary>
/// Incident response team
/// </summary>
public record IncidentResponseTeam(
    Guid TeamId,
    string Name,
    string Description,
    IReadOnlyList<IncidentResponseTeamMember> Members,
    IReadOnlyList<IncidentType> Specializations,
    bool IsActive,
    DateTimeOffset CreatedAt);

/// <summary>
/// Incident response team member
/// </summary>
public record IncidentResponseTeamMember(
    Guid UserId,
    string Username,
    string Role,
    bool IsLead,
    IReadOnlyList<string> Skills);

/// <summary>
/// Incident notification request
/// </summary>
public record IncidentNotificationRequest(
    IEnumerable<Guid> RecipientIds,
    IncidentNotificationType NotificationType,
    string? CustomMessage = null,
    IncidentNotificationChannel Channel = IncidentNotificationChannel.Email);

/// <summary>
/// Incident notification result
/// </summary>
public record IncidentNotificationResult(
    bool Success,
    int NotificationsSent,
    int NotificationsFailed,
    string? ErrorMessage = null);

/// <summary>
/// Incident response report request
/// </summary>
public record IncidentResponseReportRequest(
    DateTimeRange TimeRange,
    IncidentReportType ReportType,
    IEnumerable<IncidentType>? IncidentTypes = null,
    IEnumerable<IncidentSeverity>? Severities = null,
    bool IncludeDetails = true);

/// <summary>
/// Incident response report result
/// </summary>
public record IncidentResponseReportResult(
    Guid ReportId,
    bool Success,
    string? DownloadUrl = null,
    string? FileName = null,
    string? ErrorMessage = null);

/// <summary>
/// Incident severity levels
/// </summary>
public enum IncidentSeverity {
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4,
}

/// <summary>
/// Incident types
/// </summary>
public enum IncidentType {
    SecurityBreach,
    DataLeak,
    Malware,
    PhishingAttack,
    DenialOfService,
    UnauthorizedAccess,
    PolicyViolation,
    SystemCompromise,
    ServiceDisruption,
    ComplianceViolation,
}

/// <summary>
/// Incident status
/// </summary>
public enum IncidentStatus {
    New,
    Assigned,
    InProgress,
    Escalated,
    Resolved,
    Closed,
    Cancelled,
}

/// <summary>
/// Incident resolution types
/// </summary>
public enum IncidentResolutionType {
    Resolved,
    Mitigated,
    WorkedAround,
    FalsePositive,
    Duplicate,
    CannotReproduce,
}

/// <summary>
/// Incident activity types
/// </summary>
public enum IncidentActivityType {
    Created,
    Updated,
    Assigned,
    Escalated,
    CommentAdded,
    StatusChanged,
    ResolutionAdded,
    Closed,
    Reopened,
}

/// <summary>
/// Incident response actions
/// </summary>
public enum IncidentResponseAction {
    BlockIpAddress,
    DisableUserAccount,
    RevokeApiKey,
    QuarantineResource,
    SendAlert,
    CreateTicket,
    RunSecurityScan,
    BackupData,
    RotateCredentials,
    UpdateFirewallRules,
    NotifyStakeholders,
    CollectForensics,
}

/// <summary>
/// Incident sorting options
/// </summary>
public enum IncidentSortBy {
    CreatedAt,
    UpdatedAt,
    Severity,
    Status,
    Type,
}

/// <summary>
/// Incident notification types
/// </summary>
public enum IncidentNotificationType {
    Created,
    Assigned,
    Escalated,
    Updated,
    Resolved,
    Closed,
}

/// <summary>
/// Notification channel types for incident response
/// </summary>
public enum IncidentNotificationChannel {
    Email,
    Sms,
    Slack,
    Teams,
    Webhook,
}

/// <summary>
/// Incident report types
/// </summary>
public enum IncidentReportType {
    Summary,
    Detailed,
    Trending,
    Performance,
    Compliance,
}