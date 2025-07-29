namespace MCPHub.Common.Services;

/// <summary>
/// Dashboard service for Grafana dashboard management
/// Supports: Development (JSON files) → Production (Grafana API) → Enterprise (Multiple systems)
/// </summary>
public interface IDashboardService
{
    /// <summary>
    /// Creates a new dashboard
    /// </summary>
    Task<Dashboard> CreateDashboardAsync(DashboardDefinition definition, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates an existing dashboard
    /// </summary>
    Task<Dashboard> UpdateDashboardAsync(string dashboardId, DashboardDefinition definition, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deletes a dashboard
    /// </summary>
    Task DeleteDashboardAsync(string dashboardId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a dashboard by ID
    /// </summary>
    Task<Dashboard?> GetDashboardAsync(string dashboardId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Lists all dashboards
    /// </summary>
    Task<IEnumerable<DashboardSummary>> ListDashboardsAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Creates dashboard from template
    /// </summary>
    Task<Dashboard> CreateFromTemplateAsync(string templateName, Dictionary<string, object> parameters, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Exports dashboard configuration
    /// </summary>
    Task<string> ExportDashboardAsync(string dashboardId, DashboardExportFormat format, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Imports dashboard configuration
    /// </summary>
    Task<Dashboard> ImportDashboardAsync(string dashboardConfig, DashboardExportFormat format, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Creates standard MCP Hub dashboards
    /// </summary>
    Task CreateStandardDashboardsAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets dashboard health status
    /// </summary>
    Task<DashboardHealth> GetHealthAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Alerting service with PagerDuty/Slack integration
/// </summary>
public interface IAlertingService
{
    /// <summary>
    /// Creates a new alert rule
    /// </summary>
    Task<AlertRule> CreateAlertRuleAsync(AlertRuleDefinition definition, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates an existing alert rule
    /// </summary>
    Task<AlertRule> UpdateAlertRuleAsync(string ruleId, AlertRuleDefinition definition, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deletes an alert rule
    /// </summary>
    Task DeleteAlertRuleAsync(string ruleId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets alert rule by ID
    /// </summary>
    Task<AlertRule?> GetAlertRuleAsync(string ruleId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Lists all alert rules
    /// </summary>
    Task<IEnumerable<AlertRuleSummary>> ListAlertRulesAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Triggers an alert manually
    /// </summary>
    Task TriggerAlertAsync(string ruleId, string message, AlertSeverity severity, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Resolves an active alert
    /// </summary>
    Task ResolveAlertAsync(string alertId, string resolution, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets active alerts
    /// </summary>
    Task<IEnumerable<ActiveAlert>> GetActiveAlertsAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets alert history
    /// </summary>
    Task<IEnumerable<AlertEvent>> GetAlertHistoryAsync(TimeRange timeRange, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Creates standard MCP Hub alert rules
    /// </summary>
    Task CreateStandardAlertRulesAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Tests alert notification channels
    /// </summary>
    Task<NotificationTestResult> TestNotificationChannelsAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets alerting system health
    /// </summary>
    Task<AlertingHealth> GetHealthAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// SLA/SLI monitoring service for uptime tracking
/// </summary>
public interface IUptimeTrackingService
{
    /// <summary>
    /// Defines a new SLI (Service Level Indicator)
    /// </summary>
    Task<ServiceLevelIndicator> CreateSLIAsync(SLIDefinition definition, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Defines a new SLO (Service Level Objective)
    /// </summary>
    Task<ServiceLevelObjective> CreateSLOAsync(SLODefinition definition, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Records uptime data point
    /// </summary>
    Task RecordUptimeAsync(string serviceId, bool isHealthy, TimeSpan responseTime, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets current uptime status
    /// </summary>
    Task<UptimeStatus> GetUptimeStatusAsync(string serviceId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets uptime statistics for a time period
    /// </summary>
    Task<UptimeStatistics> GetUptimeStatisticsAsync(string serviceId, TimeRange timeRange, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets SLO compliance status
    /// </summary>
    Task<SLOComplianceStatus> GetSLOComplianceAsync(string sloId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets all SLO compliance statuses
    /// </summary>
    Task<IEnumerable<SLOComplianceStatus>> GetAllSLOComplianceAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Creates incident report for downtime
    /// </summary>
    Task<IncidentReport> CreateIncidentReportAsync(IncidentReportDefinition definition, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets incident history
    /// </summary>
    Task<IEnumerable<IncidentReport>> GetIncidentHistoryAsync(TimeRange timeRange, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets availability report
    /// </summary>
    Task<AvailabilityReport> GetAvailabilityReportAsync(string serviceId, TimeRange timeRange, CancellationToken cancellationToken = default);
}

// Supporting data structures

/// <summary>
/// Dashboard definition
/// </summary>
public record DashboardDefinition(
    string Name,
    string Description,
    string Category,
    List<DashboardPanel> Panels,
    Dictionary<string, object> Variables,
    TimeRange DefaultTimeRange,
    int RefreshInterval
);

/// <summary>
/// Dashboard panel configuration
/// </summary>
public record DashboardPanel(
    string Title,
    string Type,
    Dictionary<string, object> QueryConfig,
    Dictionary<string, object> VisualizationConfig,
    GridPosition Position
);

/// <summary>
/// Grid position for dashboard panels
/// </summary>
public record GridPosition(int X, int Y, int Width, int Height);

/// <summary>
/// Dashboard summary
/// </summary>
public record DashboardSummary(
    string Id,
    string Name,
    string Description,
    string Category,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    bool IsStarred
);

/// <summary>
/// Complete dashboard data
/// </summary>
public record Dashboard(
    string Id,
    string Name,
    string Description,
    string Category,
    List<DashboardPanel> Panels,
    Dictionary<string, object> Variables,
    TimeRange DefaultTimeRange,
    int RefreshInterval,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

/// <summary>
/// Dashboard export formats
/// </summary>
public enum DashboardExportFormat
{
    Json,
    Yaml,
    Terraform
}

/// <summary>
/// Dashboard health status
/// </summary>
public record DashboardHealth(
    bool IsHealthy,
    int TotalDashboards,
    int HealthyDashboards,
    TimeSpan AverageLoadTime,
    List<string> Errors
);

/// <summary>
/// Alert rule definition
/// </summary>
public record AlertRuleDefinition(
    string Name,
    string Description,
    string Query,
    AlertCondition Condition,
    TimeSpan EvaluationInterval,
    TimeSpan ConditionDuration,
    AlertSeverity Severity,
    List<NotificationChannel> NotificationChannels,
    bool Enabled
);

/// <summary>
/// Alert condition
/// </summary>
public record AlertCondition(
    AlertOperator Operator,
    double Threshold,
    string? SecondaryThreshold = null
);

/// <summary>
/// Alert operators
/// </summary>
public enum AlertOperator
{
    GreaterThan,
    LessThan,
    EqualTo,
    NotEqualTo,
    Between,
    Outside
}

/// <summary>
/// Alert severity levels
/// </summary>
public enum AlertSeverity
{
    Info,
    Warning,
    Critical,
    Emergency
}

/// <summary>
/// Notification channel
/// </summary>
public record NotificationChannel(
    string Type,
    string Name,
    Dictionary<string, string> Configuration
);

/// <summary>
/// Alert rule summary
/// </summary>
public record AlertRuleSummary(
    string Id,
    string Name,
    AlertSeverity Severity,
    bool Enabled,
    DateTime LastEvaluation,
    AlertRuleStatus Status
);

/// <summary>
/// Complete alert rule data
/// </summary>
public record AlertRule(
    string Id,
    string Name,
    string Description,
    string Query,
    AlertCondition Condition,
    TimeSpan EvaluationInterval,
    TimeSpan ConditionDuration,
    AlertSeverity Severity,
    List<NotificationChannel> NotificationChannels,
    bool Enabled,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

/// <summary>
/// Alert rule status
/// </summary>
public enum AlertRuleStatus
{
    OK,
    Pending,
    Alerting,
    NoData,
    Error
}

/// <summary>
/// Active alert
/// </summary>
public record ActiveAlert(
    string Id,
    string RuleId,
    string RuleName,
    AlertSeverity Severity,
    string Message,
    DateTime TriggeredAt,
    Dictionary<string, string> Labels
);

/// <summary>
/// Alert event
/// </summary>
public record AlertEvent(
    string AlertId,
    string RuleId,
    AlertEventType EventType,
    string Message,
    DateTime Timestamp,
    Dictionary<string, object> EventData
);

/// <summary>
/// Alert event types
/// </summary>
public enum AlertEventType
{
    Triggered,
    Resolved,
    Acknowledged,
    Escalated,
    Suppressed
}

/// <summary>
/// Notification test result
/// </summary>
public record NotificationTestResult(
    Dictionary<string, bool> ChannelResults,
    List<string> Errors
);

/// <summary>
/// Alerting health
/// </summary>
public record AlertingHealth(
    bool IsHealthy,
    int TotalRules,
    int ActiveAlerts,
    int FailedEvaluations,
    TimeSpan AverageEvaluationTime,
    List<string> Errors
);

/// <summary>
/// Service Level Indicator definition
/// </summary>
public record SLIDefinition(
    string Name,
    string ServiceId,
    string Description,
    string Query,
    SLIType Type,
    Dictionary<string, object> Configuration
);

/// <summary>
/// SLI types
/// </summary>
public enum SLIType
{
    Availability,
    Latency,
    ErrorRate,
    Throughput,
    Quality
}

/// <summary>
/// Service Level Objective definition
/// </summary>
public record SLODefinition(
    string Name,
    string SLIId,
    string Description,
    double Target,
    TimeSpan TimeWindow,
    string? AlertRuleId = null
);

/// <summary>
/// Service Level Indicator
/// </summary>
public record ServiceLevelIndicator(
    string Id,
    string Name,
    string ServiceId,
    string Description,
    string Query,
    SLIType Type,
    Dictionary<string, object> Configuration,
    DateTime CreatedAt
);

/// <summary>
/// Service Level Objective
/// </summary>
public record ServiceLevelObjective(
    string Id,
    string Name,
    string SLIId,
    string Description,
    double Target,
    TimeSpan TimeWindow,
    string? AlertRuleId,
    DateTime CreatedAt
);

/// <summary>
/// Uptime status
/// </summary>
public record UptimeStatus(
    string ServiceId,
    bool IsHealthy,
    TimeSpan? LastDowntime,
    double UptimePercentage,
    TimeSpan AverageResponseTime,
    DateTime LastChecked
);

/// <summary>
/// Uptime statistics
/// </summary>
public record UptimeStatistics(
    string ServiceId,
    TimeRange TimeRange,
    double UptimePercentage,
    TimeSpan TotalDowntime,
    int IncidentCount,
    TimeSpan AverageResponseTime,
    TimeSpan MaxResponseTime,
    List<UptimeIncident> Incidents
);

/// <summary>
/// Uptime incident
/// </summary>
public record UptimeIncident(
    DateTime StartTime,
    DateTime? EndTime,
    TimeSpan Duration,
    string Severity,
    string? Cause
);

/// <summary>
/// SLO compliance status
/// </summary>
public record SLOComplianceStatus(
    string SLOId,
    string Name,
    double Target,
    double Actual,
    bool InCompliance,
    double ErrorBudgetRemaining,
    TimeSpan TimeWindow,
    DateTime LastEvaluated
);

/// <summary>
/// Incident report definition
/// </summary>
public record IncidentReportDefinition(
    string ServiceId,
    string Title,
    string Description,
    DateTime StartTime,
    DateTime? EndTime,
    string Severity,
    string? RootCause = null,
    List<string>? AffectedServices = null
);

/// <summary>
/// Incident report
/// </summary>
public record IncidentReport(
    string Id,
    string ServiceId,
    string Title,
    string Description,
    DateTime StartTime,
    DateTime? EndTime,
    TimeSpan Duration,
    string Severity,
    string? RootCause,
    List<string> AffectedServices,
    DateTime CreatedAt
);

/// <summary>
/// Availability report
/// </summary>
public record AvailabilityReport(
    string ServiceId,
    TimeRange TimeRange,
    double OverallUptime,
    Dictionary<string, double> DailyUptime,
    List<IncidentReport> Incidents,
    AvailabilityTrend Trend
);

/// <summary>
/// Availability trend
/// </summary>
public record AvailabilityTrend(
    TrendDirection Direction,
    double ChangePercent,
    string Analysis
);

/// <summary>
/// Trend direction
/// </summary>
public enum TrendDirection
{
    Up,
    Down,
    Stable
}