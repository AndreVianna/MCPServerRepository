namespace MCPHub.Common.Services;

/// <summary>
/// Threat detection service for real-time security monitoring and anomaly detection
/// Supports: Rule-based → ML-based → Enterprise Threat Intelligence
/// </summary>
public interface IThreatDetectionService {
    /// <summary>
    /// Analyzes a request for potential threats
    /// </summary>
    /// <param name="request">Threat analysis request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Threat analysis result</returns>
    Task<ThreatAnalysisResult> AnalyzeRequestAsync(ThreatAnalysisRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Analyzes user behavior patterns for anomalies
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="behaviorData">Current behavior data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Behavior analysis result</returns>
    Task<BehaviorAnalysisResult> AnalyzeBehaviorAsync(Guid userId, UserBehaviorData behaviorData, CancellationToken cancellationToken = default);

    /// <summary>
    /// Monitors network traffic patterns for anomalies
    /// </summary>
    /// <param name="trafficData">Network traffic data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Traffic analysis result</returns>
    Task<TrafficAnalysisResult> AnalyzeTrafficAsync(NetworkTrafficData trafficData, CancellationToken cancellationToken = default);

    /// <summary>
    /// Scans content for malicious patterns
    /// </summary>
    /// <param name="contentRequest">Content scanning request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Content scan result</returns>
    Task<ContentScanResult> ScanContentAsync(ContentScanRequest contentRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets active threat indicators
    /// </summary>
    /// <param name="threatTypes">Optional threat types to filter</param>
    /// <param name="severity">Optional minimum severity level</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Active threat indicators</returns>
    Task<IReadOnlyList<ThreatIndicator>> GetActiveThreatIndicatorsAsync(
        IEnumerable<ThreatType>? threatTypes = null, 
        ThreatSeverity? severity = null, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a custom threat detection rule
    /// </summary>
    /// <param name="ruleRequest">Threat rule creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Rule creation result</returns>
    Task<ThreatRuleResult> CreateThreatRuleAsync(CreateThreatRuleRequest ruleRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing threat detection rule
    /// </summary>
    /// <param name="ruleId">Rule identifier</param>
    /// <param name="updateRequest">Rule update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Rule update result</returns>
    Task<ThreatRuleResult> UpdateThreatRuleAsync(Guid ruleId, UpdateThreatRuleRequest updateRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a threat detection rule
    /// </summary>
    /// <param name="ruleId">Rule identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Rule deletion result</returns>
    Task<ThreatRuleDeletionResult> DeleteThreatRuleAsync(Guid ruleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets threat detection statistics
    /// </summary>
    /// <param name="timeRange">Time range for statistics</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Threat detection statistics</returns>
    Task<ThreatDetectionStatistics> GetStatisticsAsync(DateTimeRange timeRange, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates threat intelligence data from external sources
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Intelligence update result</returns>
    Task<ThreatIntelligenceUpdateResult> UpdateThreatIntelligenceAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs real-time threat assessment for an IP address
    /// </summary>
    /// <param name="ipAddress">IP address to assess</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>IP threat assessment result</returns>
    Task<IpThreatAssessment> AssessIpThreatAsync(string ipAddress, CancellationToken cancellationToken = default);

    /// <summary>
    /// Blocks or unblocks an IP address based on threat assessment
    /// </summary>
    /// <param name="ipAddress">IP address to block/unblock</param>
    /// <param name="action">Block or unblock action</param>
    /// <param name="reason">Reason for the action</param>
    /// <param name="duration">Block duration (null for permanent)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>IP blocking result</returns>
    Task<IpBlockingResult> ManageIpBlockAsync(string ipAddress, IpBlockAction action, string reason, TimeSpan? duration = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets threat detection configuration
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Current threat detection configuration</returns>
    Task<ThreatDetectionConfiguration> GetConfigurationAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates threat detection configuration
    /// </summary>
    /// <param name="configuration">New configuration</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Configuration update result</returns>
    Task<ThreatDetectionConfigurationResult> UpdateConfigurationAsync(ThreatDetectionConfiguration configuration, CancellationToken cancellationToken = default);
}

/// <summary>
/// Threat analysis request
/// </summary>
public record ThreatAnalysisRequest(
    string RequestPath,
    string HttpMethod,
    string IpAddress,
    string? UserAgent = null,
    IDictionary<string, string>? Headers = null,
    IDictionary<string, string>? QueryParameters = null,
    string? RequestBody = null,
    Guid? UserId = null,
    DateTimeOffset RequestTime = default);

/// <summary>
/// Threat analysis result
/// </summary>
public record ThreatAnalysisResult(
    ThreatLevel ThreatLevel,
    ThreatSeverity Severity,
    IReadOnlyList<ThreatIndicator> Indicators,
    double ThreatScore,
    string? RecommendedAction = null,
    IDictionary<string, object>? Details = null,
    TimeSpan AnalysisDuration = default);

/// <summary>
/// User behavior data for analysis
/// </summary>
public record UserBehaviorData(
    Guid UserId,
    string IpAddress,
    string? UserAgent = null,
    IReadOnlyList<string>? RecentActions = null,
    IDictionary<string, int>? ActionCounts = null,
    DateTimeOffset? LastActivity = null,
    TimeSpan? SessionDuration = null,
    int? RequestsPerMinute = null);

/// <summary>
/// Behavior analysis result
/// </summary>
public record BehaviorAnalysisResult(
    bool IsAnomalous,
    double AnomalyScore,
    IReadOnlyList<string> AnomalyReasons,
    BehaviorRiskLevel RiskLevel,
    string? RecommendedAction = null);

/// <summary>
/// Network traffic data for analysis
/// </summary>
public record NetworkTrafficData(
    string SourceIp,
    string? DestinationIp = null,
    int? SourcePort = null,
    int? DestinationPort = null,
    string? Protocol = null,
    long? BytesSent = null,
    long? BytesReceived = null,
    int? PacketCount = null,
    TimeSpan? ConnectionDuration = null,
    DateTimeOffset Timestamp = default);

/// <summary>
/// Traffic analysis result
/// </summary>
public record TrafficAnalysisResult(
    bool IsSuspicious,
    ThreatLevel ThreatLevel,
    IReadOnlyList<string> SuspiciousPatterns,
    string? RecommendedAction = null);

/// <summary>
/// Content scanning request
/// </summary>
public record ContentScanRequest(
    string Content,
    ContentType ContentType,
    string? FileName = null,
    long? FileSize = null,
    string? MimeType = null,
    IDictionary<string, object>? Metadata = null);

/// <summary>
/// Content scan result
/// </summary>
public record ContentScanResult(
    bool IsMalicious,
    ThreatSeverity Severity,
    IReadOnlyList<string> ThreatTypes,
    IReadOnlyList<MalwareSignature> DetectedSignatures,
    string? RecommendedAction = null);

/// <summary>
/// Threat indicator
/// </summary>
public record ThreatIndicator(
    Guid IndicatorId,
    ThreatType ThreatType,
    ThreatSeverity Severity,
    string Name,
    string Description,
    string Pattern,
    double Confidence,
    DateTimeOffset CreatedAt,
    DateTimeOffset? ExpiresAt = null);

/// <summary>
/// Create threat rule request
/// </summary>
public record CreateThreatRuleRequest(
    string Name,
    string Description,
    ThreatType ThreatType,
    ThreatSeverity Severity,
    string Pattern,
    ThreatRuleCondition Condition,
    ThreatRuleAction Action,
    bool IsEnabled = true);

/// <summary>
/// Update threat rule request
/// </summary>
public record UpdateThreatRuleRequest(
    string? Name = null,
    string? Description = null,
    ThreatSeverity? Severity = null,
    string? Pattern = null,
    ThreatRuleCondition? Condition = null,
    ThreatRuleAction? Action = null,
    bool? IsEnabled = null);

/// <summary>
/// Threat rule result
/// </summary>
public record ThreatRuleResult(
    Guid RuleId,
    bool Success,
    string? ErrorMessage = null);

/// <summary>
/// Threat rule deletion result
/// </summary>
public record ThreatRuleDeletionResult(
    bool Success,
    string? ErrorMessage = null);

/// <summary>
/// Threat detection statistics
/// </summary>
public record ThreatDetectionStatistics(
    DateTimeRange TimeRange,
    int TotalThreatsDetected,
    int TotalRequestsAnalyzed,
    double ThreatDetectionRate,
    IDictionary<ThreatType, int> ThreatsByType,
    IDictionary<ThreatSeverity, int> ThreatsBySeverity,
    IDictionary<string, int> TopThreatSources,
    int FalsePositives,
    int FalseNegatives,
    double Accuracy);

/// <summary>
/// Threat intelligence update result
/// </summary>
public record ThreatIntelligenceUpdateResult(
    bool Success,
    int NewIndicators,
    int UpdatedIndicators,
    int ExpiredIndicators,
    DateTimeOffset LastUpdate,
    string? ErrorMessage = null);

/// <summary>
/// IP threat assessment result
/// </summary>
public record IpThreatAssessment(
    string IpAddress,
    ThreatLevel ThreatLevel,
    double ThreatScore,
    IReadOnlyList<string> ThreatCategories,
    bool IsBlocked,
    string? GeolocationCountry = null,
    string? Organization = null,
    DateTimeOffset AssessedAt = default);

/// <summary>
/// IP blocking result
/// </summary>
public record IpBlockingResult(
    bool Success,
    string IpAddress,
    IpBlockAction Action,
    DateTimeOffset? BlockedUntil = null,
    string? ErrorMessage = null);

/// <summary>
/// Threat detection configuration
/// </summary>
public record ThreatDetectionConfiguration(
    bool IsEnabled,
    ThreatSeverity MinimumSeverityLevel,
    int MaxRequestsPerMinute,
    int SuspiciousBehaviorThreshold,
    bool EnableMachineLearning,
    bool EnableThreatIntelligence,
    TimeSpan ThreatIntelligenceUpdateInterval,
    IReadOnlyList<string> WhitelistedIpRanges,
    IReadOnlyList<string> BlacklistedIpRanges);

/// <summary>
/// Threat detection configuration result
/// </summary>
public record ThreatDetectionConfigurationResult(
    bool Success,
    string? ErrorMessage = null);

/// <summary>
/// Malware signature detection
/// </summary>
public record MalwareSignature(
    string SignatureName,
    string SignatureType,
    double Confidence,
    string Description);

/// <summary>
/// Threat levels
/// </summary>
public enum ThreatLevel {
    None = 0,
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4
}

/// <summary>
/// Threat severity levels
/// </summary>
public enum ThreatSeverity {
    Informational = 0,
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4
}

/// <summary>
/// Threat types
/// </summary>
public enum ThreatType {
    SqlInjection,
    CrossSiteScripting,
    CommandInjection,
    PathTraversal,
    BruteForce,
    DenialOfService,
    Malware,
    Phishing,
    SuspiciousBehavior,
    AnomalousTraffic,
    DataExfiltration,
    PrivilegeEscalation,
    UnauthorizedAccess
}

/// <summary>
/// Behavior risk levels
/// </summary>
public enum BehaviorRiskLevel {
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4
}

/// <summary>
/// Content types for scanning
/// </summary>
public enum ContentType {
    Text,
    Binary,
    Archive,
    Executable,
    Script,
    Document
}

/// <summary>
/// Threat rule conditions
/// </summary>
public enum ThreatRuleCondition {
    Contains,
    Equals,
    StartsWith,
    EndsWith,
    Regex,
    IpRange,
    GreaterThan,
    LessThan
}

/// <summary>
/// Threat rule actions
/// </summary>
public enum ThreatRuleAction {
    Log,
    Block,
    Alert,
    Quarantine,
    RateLimit
}

/// <summary>
/// IP blocking actions
/// </summary>
public enum IpBlockAction {
    Block,
    Unblock
}