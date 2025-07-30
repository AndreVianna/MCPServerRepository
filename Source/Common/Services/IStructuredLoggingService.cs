namespace MCPHub.Common.Services;

/// <summary>
/// Structured logging service with correlation IDs and contextual data
/// Supports: Console → Elasticsearch/Logstash → Enterprise (multiple sinks)
/// </summary>
public interface IStructuredLoggingService {
    /// <summary>
    /// Logs a message with structured data
    /// </summary>
    void Log(LogLevel level, string message, object? structuredData = null, Exception? exception = null);

    /// <summary>
    /// Logs with a specific correlation ID
    /// </summary>
    void LogWithCorrelation(LogLevel level, string correlationId, string message, object? structuredData = null, Exception? exception = null);

    /// <summary>
    /// Logs information with structured data
    /// </summary>
    void LogInformation(string message, object? structuredData = null);

    /// <summary>
    /// Logs warning with structured data
    /// </summary>
    void LogWarning(string message, object? structuredData = null, Exception? exception = null);

    /// <summary>
    /// Logs error with structured data
    /// </summary>
    void LogError(string message, object? structuredData = null, Exception? exception = null);

    /// <summary>
    /// Logs critical error with structured data
    /// </summary>
    void LogCritical(string message, object? structuredData = null, Exception? exception = null);

    /// <summary>
    /// Logs debug information with structured data
    /// </summary>
    void LogDebug(string message, object? structuredData = null);

    /// <summary>
    /// Logs trace information with structured data
    /// </summary>
    void LogTrace(string message, object? structuredData = null);

    /// <summary>
    /// Creates a logging scope with additional context
    /// </summary>
    IDisposable BeginScope(string scopeName, object? scopeData = null);

    /// <summary>
    /// Creates a logging scope with correlation ID
    /// </summary>
    IDisposable BeginScopeWithCorrelation(string correlationId, string scopeName, object? scopeData = null);

    /// <summary>
    /// Gets the current correlation ID
    /// </summary>
    string? GetCurrentCorrelationId();

    /// <summary>
    /// Sets the correlation ID for the current context
    /// </summary>
    void SetCorrelationId(string correlationId);

    /// <summary>
    /// Adds contextual data to all logs in the current scope
    /// </summary>
    void AddContext(string key, object value);

    /// <summary>
    /// Removes contextual data from the current scope
    /// </summary>
    void RemoveContext(string key);

    /// <summary>
    /// Gets all contextual data for the current scope
    /// </summary>
    IReadOnlyDictionary<string, object> GetCurrentContext();
}

/// <summary>
/// MCP-specific structured logging extensions
/// </summary>
public interface IMCPStructuredLoggingService : IStructuredLoggingService {
    /// <summary>
    /// Logs package-related events
    /// </summary>
    void LogPackageEvent(LogLevel level, string eventType, string packageName, string version, string userId, object? additionalData = null);

    /// <summary>
    /// Logs security-related events
    /// </summary>
    void LogSecurityEvent(LogLevel level, string eventType, string resourceId, string userId, string? threat = null, object? additionalData = null);

    /// <summary>
    /// Logs authentication events
    /// </summary>
    void LogAuthenticationEvent(LogLevel level, string eventType, string userId, bool success, string? failureReason = null, object? additionalData = null);

    /// <summary>
    /// Logs search and discovery events
    /// </summary>
    void LogSearchEvent(LogLevel level, string query, string userId, int resultCount, TimeSpan duration, object? additionalData = null);

    /// <summary>
    /// Logs API events with performance data
    /// </summary>
    void LogApiEvent(LogLevel level, string method, string path, int statusCode, TimeSpan duration, string userId, object? additionalData = null);

    /// <summary>
    /// Logs database events with performance data
    /// </summary>
    void LogDatabaseEvent(LogLevel level, string operation, string table, TimeSpan duration, bool success, int? affectedRows = null, object? additionalData = null);

    /// <summary>
    /// Logs external service calls
    /// </summary>
    void LogExternalServiceEvent(LogLevel level, string serviceName, string operation, TimeSpan duration, bool success, string? errorCode = null, object? additionalData = null);

    /// <summary>
    /// Logs business events for analytics
    /// </summary>
    void LogBusinessEvent(string eventType, string userId, object businessData);

    /// <summary>
    /// Logs performance metrics
    /// </summary>
    void LogPerformanceEvent(string operationType, string resourceId, TimeSpan duration, object? performanceData = null);

    /// <summary>
    /// Logs compliance and audit events
    /// </summary>
    void LogAuditEvent(string action, string resourceType, string resourceId, string userId, object? auditData = null);
}

/// <summary>
/// Log aggregation service for centralized log collection
/// </summary>
public interface ILogAggregationService {
    /// <summary>
    /// Sends logs to centralized logging system (ELK Stack)
    /// </summary>
    Task SendLogsAsync(IEnumerable<StructuredLogEntry> logs, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a single log entry
    /// </summary>
    Task SendLogAsync(StructuredLogEntry logEntry, CancellationToken cancellationToken = default);

    /// <summary>
    /// Queries logs from the aggregation system
    /// </summary>
    Task<LogQueryResult> QueryLogsAsync(LogQuery query, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets log statistics for a time period
    /// </summary>
    Task<LogStatistics> GetLogStatisticsAsync(TimeRange timeRange, string? filter = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates log aggregation rules
    /// </summary>
    Task CreateAggregationRuleAsync(LogAggregationRule rule, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets health status of log aggregation system
    /// </summary>
    Task<LogAggregationHealth> GetHealthAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Security event logging service for SIEM integration
/// </summary>
public interface ISecurityEventLoggingService {
    /// <summary>
    /// Logs security events in SIEM-compatible format
    /// </summary>
    Task LogSecurityEventAsync(SecurityEvent securityEvent, CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs authentication events
    /// </summary>
    Task LogAuthenticationEventAsync(string userId, string action, bool success, string? sourceIp = null, string? userAgent = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs authorization events
    /// </summary>
    Task LogAuthorizationEventAsync(string userId, string resource, string action, bool granted, string? reason = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs data access events
    /// </summary>
    Task LogDataAccessEventAsync(string userId, string dataType, string resourceId, string action, CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs threat detection events
    /// </summary>
    Task LogThreatDetectionEventAsync(string threatType, string sourceIp, string targetResource, string severity, object? threatData = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs compliance events
    /// </summary>
    Task LogComplianceEventAsync(string complianceType, string resourceId, bool compliant, string? violations = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Queries security events
    /// </summary>
    Task<IEnumerable<SecurityEvent>> QuerySecurityEventsAsync(SecurityEventQuery query, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets security event statistics
    /// </summary>
    Task<SecurityEventStatistics> GetSecurityStatisticsAsync(TimeRange timeRange, CancellationToken cancellationToken = default);
}

/// <summary>
/// Log retention service for automated cleanup and archival
/// </summary>
public interface ILogRetentionService {
    /// <summary>
    /// Archives old logs to long-term storage
    /// </summary>
    Task ArchiveLogsAsync(DateTime beforeDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes logs older than retention period
    /// </summary>
    Task DeleteExpiredLogsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets current log storage statistics
    /// </summary>
    Task<LogStorageStatistics> GetStorageStatisticsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates retention policies
    /// </summary>
    Task CreateRetentionPolicyAsync(LogRetentionPolicy policy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active retention policies
    /// </summary>
    Task<IEnumerable<LogRetentionPolicy>> GetRetentionPoliciesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Estimates storage requirements
    /// </summary>
    Task<StorageEstimate> EstimateStorageRequirementsAsync(int days, CancellationToken cancellationToken = default);
}

// Supporting data structures

/// <summary>
/// Structured log entry
/// </summary>
public record StructuredLogEntry(
    DateTime Timestamp,
    LogLevel Level,
    string Message,
    string? CorrelationId,
    string? UserId,
    string ServiceName,
    string Environment,
    object? StructuredData,
    Exception? Exception,
    IDictionary<string, object> Context
);

/// <summary>
/// Log query parameters
/// </summary>
public record LogQuery(
    TimeRange TimeRange,
    LogLevel? MinimumLevel,
    string? CorrelationId,
    string? UserId,
    string? ServiceName,
    string? MessageFilter,
    Dictionary<string, object>? ContextFilters,
    int MaxResults = 1000
);

/// <summary>
/// Log query result
/// </summary>
public record LogQueryResult(
    IEnumerable<StructuredLogEntry> Logs,
    int TotalCount,
    TimeSpan QueryDuration
);

/// <summary>
/// Log statistics
/// </summary>
public record LogStatistics(
    TimeRange TimeRange,
    Dictionary<LogLevel, int> LogCounts,
    int TotalLogs,
    int UniqueUsers,
    int UniqueCorrelationIds,
    Dictionary<string, int> TopErrors
);

/// <summary>
/// Log aggregation rule
/// </summary>
public record LogAggregationRule(
    string Name,
    string Filter,
    TimeSpan AggregationWindow,
    string[] GroupByFields,
    string[] MetricFields
);

/// <summary>
/// Log aggregation health
/// </summary>
public record LogAggregationHealth(
    bool IsHealthy,
    TimeSpan Latency,
    int QueueDepth,
    DateTime LastSuccessfulIngestion,
    string? ErrorMessage
);

/// <summary>
/// Security event data
/// </summary>
public record SecurityEvent(
    DateTime Timestamp,
    string EventType,
    string Severity,
    string UserId,
    string? SourceIp,
    string? UserAgent,
    string ResourceId,
    string Action,
    bool Success,
    object? EventData
);

/// <summary>
/// Security event query
/// </summary>
public record SecurityEventQuery(
    TimeRange TimeRange,
    string? EventType,
    string? Severity,
    string? UserId,
    string? SourceIp,
    string? ResourceId,
    bool? Success
);

// SecurityEventStatistics moved to ISecurityEventService.cs to avoid duplication

/// <summary>
/// Log storage statistics
/// </summary>
public record LogStorageStatistics(
    long TotalSizeBytes,
    int TotalLogCount,
    DateTime OldestLog,
    DateTime NewestLog,
    Dictionary<LogLevel, int> LogLevelCounts
);

/// <summary>
/// Log retention policy
/// </summary>
public record LogRetentionPolicy(
    string Name,
    LogLevel MinimumLevel,
    TimeSpan RetentionPeriod,
    bool ArchiveBeforeDelete,
    string? ArchiveLocation
);

/// <summary>
/// Storage estimate
/// </summary>
public record StorageEstimate(
    int Days,
    long EstimatedSizeBytes,
    int EstimatedLogCount,
    decimal EstimatedCost
);