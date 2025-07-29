namespace MCPHub.Common.Services;

/// <summary>
/// MCP Hub specific metrics service for business and operational metrics
/// Supports: Console → Prometheus → Enterprise (multiple backends)
/// </summary>
public interface IMCPMetricsService
{
    // Package Management Metrics
    
    /// <summary>
    /// Records a package download event
    /// </summary>
    void RecordPackageDownload(string packageName, string version, string publisherId, string userId);
    
    /// <summary>
    /// Records a package installation event
    /// </summary>
    void RecordPackageInstallation(string packageName, string version, string userId, bool success, TimeSpan duration);
    
    /// <summary>
    /// Records a package uninstallation event
    /// </summary>
    void RecordPackageUninstallation(string packageName, string version, string userId, bool success);
    
    /// <summary>
    /// Records a package publication event
    /// </summary>
    void RecordPackagePublication(string packageName, string version, string publisherId, bool success);
    
    /// <summary>
    /// Updates package popularity metrics
    /// </summary>
    void UpdatePackagePopularity(string packageName, int downloadCount, int installationCount, double rating);
    
    // Security Metrics
    
    /// <summary>
    /// Records a security scan completion
    /// </summary>
    void RecordSecurityScan(string packageName, string version, string scanType, string result, TimeSpan duration, double score);
    
    /// <summary>
    /// Records security vulnerability detection
    /// </summary>
    void RecordSecurityVulnerability(string packageName, string version, string vulnerabilityType, string severity);
    
    /// <summary>
    /// Records trust tier changes
    /// </summary>
    void RecordTrustTierChange(string packageName, string oldTier, string newTier, string reason);
    
    /// <summary>
    /// Records security policy violations
    /// </summary>
    void RecordSecurityPolicyViolation(string packageName, string version, string policyType, string violation);
    
    // Search and Discovery Metrics
    
    /// <summary>
    /// Records search operations
    /// </summary>
    void RecordSearchOperation(string query, string userId, int resultCount, TimeSpan duration, string searchType);
    
    /// <summary>
    /// Records search result clicks
    /// </summary>
    void RecordSearchResultClick(string query, string packageName, int position, string userId);
    
    /// <summary>
    /// Records recommendation effectiveness
    /// </summary>
    void RecordRecommendationClick(string packageName, string recommendationType, string userId, bool converted);
    
    // User and Authentication Metrics
    
    /// <summary>
    /// Records user authentication events
    /// </summary>
    void RecordUserAuthentication(string userId, string authType, bool success, string? failureReason = null);
    
    /// <summary>
    /// Records user registration events
    /// </summary>
    void RecordUserRegistration(string userId, string registrationType, bool success);
    
    /// <summary>
    /// Records user activity metrics
    /// </summary>
    void RecordUserActivity(string userId, string activityType, string resourceId);
    
    /// <summary>
    /// Records API key usage
    /// </summary>
    void RecordApiKeyUsage(string apiKeyId, string userId, string endpoint, bool success);
    
    // System Performance Metrics
    
    /// <summary>
    /// Records HTTP request metrics
    /// </summary>
    void RecordHttpRequest(string method, string path, int statusCode, TimeSpan duration, long? responseSize = null);
    
    /// <summary>
    /// Records database operation metrics
    /// </summary>
    void RecordDatabaseOperation(string operation, string table, TimeSpan duration, bool success, int? rowCount = null);
    
    /// <summary>
    /// Records cache operation metrics
    /// </summary>
    void RecordCacheOperation(string operation, string key, TimeSpan duration, bool hit, long? dataSize = null);
    
    /// <summary>
    /// Records message processing metrics
    /// </summary>
    void RecordMessageProcessing(string messageType, string source, string destination, TimeSpan duration, bool success);
    
    // Business Intelligence Metrics
    
    /// <summary>
    /// Records revenue-related events
    /// </summary>
    void RecordRevenueEvent(string eventType, decimal amount, string currency, string packageName, string userId);
    
    /// <summary>
    /// Records subscription metrics
    /// </summary>
    void RecordSubscriptionEvent(string eventType, string planType, string userId, decimal monthlyValue);
    
    /// <summary>
    /// Records support ticket metrics
    /// </summary>
    void RecordSupportTicket(string ticketType, string severity, string userId, string packageName);
    
    // Infrastructure Metrics
    
    /// <summary>
    /// Records storage usage metrics
    /// </summary>
    void RecordStorageUsage(string storageType, long totalBytes, long usedBytes, int fileCount);
    
    /// <summary>
    /// Records resource utilization
    /// </summary>
    void RecordResourceUtilization(string resourceType, double cpuPercent, double memoryPercent, double diskPercent);
    
    /// <summary>
    /// Records external service health
    /// </summary>
    void RecordExternalServiceHealth(string serviceName, bool healthy, TimeSpan responseTime, string? errorType = null);
    
    // Custom Metrics
    
    /// <summary>
    /// Records a custom counter metric
    /// </summary>
    void RecordCounter(string name, long value, IDictionary<string, string>? tags = null);
    
    /// <summary>
    /// Records a custom gauge metric
    /// </summary>
    void RecordGauge(string name, double value, IDictionary<string, string>? tags = null);
    
    /// <summary>
    /// Records a custom histogram metric
    /// </summary>
    void RecordHistogram(string name, double value, IDictionary<string, string>? tags = null);
    
    // Metric Aggregation and Analysis
    
    /// <summary>
    /// Gets aggregated metrics for a time period
    /// </summary>
    Task<MetricAggregation> GetAggregatedMetricsAsync(string metricName, TimeRange timeRange, string[] groupBy, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets top packages by metric
    /// </summary>
    Task<IEnumerable<PackageMetricSummary>> GetTopPackagesByMetricAsync(string metricType, int count, TimeRange timeRange, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets user activity summary
    /// </summary>
    Task<UserActivitySummary> GetUserActivitySummaryAsync(string userId, TimeRange timeRange, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets system health metrics
    /// </summary>
    Task<SystemHealthMetrics> GetSystemHealthMetricsAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Time range for metric queries
/// </summary>
public record TimeRange(DateTime Start, DateTime End)
{
    public static TimeRange LastHour => new(DateTime.UtcNow.AddHours(-1), DateTime.UtcNow);
    public static TimeRange LastDay => new(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow);
    public static TimeRange LastWeek => new(DateTime.UtcNow.AddDays(-7), DateTime.UtcNow);
    public static TimeRange LastMonth => new(DateTime.UtcNow.AddDays(-30), DateTime.UtcNow);
}

/// <summary>
/// Aggregated metric data
/// </summary>
public record MetricAggregation(
    string MetricName,
    TimeRange TimeRange,
    double Sum,
    double Average,
    double Minimum,
    double Maximum,
    long Count,
    IDictionary<string, MetricAggregation> GroupedResults
);

/// <summary>
/// Package metric summary
/// </summary>
public record PackageMetricSummary(
    string PackageName,
    string PublisherId,
    double MetricValue,
    int Rank,
    Dictionary<string, object> AdditionalMetrics
);

/// <summary>
/// User activity summary
/// </summary>
public record UserActivitySummary(
    string UserId,
    int TotalDownloads,
    int TotalInstallations,
    int TotalSearches,
    DateTime LastActivity,
    Dictionary<string, int> ActivityBreakdown
);

/// <summary>
/// System health metrics snapshot
/// </summary>
public record SystemHealthMetrics(
    double CpuUsage,
    double MemoryUsage,
    double DiskUsage,
    int ActiveUsers,
    int TotalPackages,
    double AverageResponseTime,
    int ErrorRate,
    DateTime Timestamp
);