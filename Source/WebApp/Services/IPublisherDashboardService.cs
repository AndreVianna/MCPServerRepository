namespace MCPHub.WebApp.Services;

/// <summary>
/// Service interface for publisher dashboard operations and analytics
/// </summary>
public interface IPublisherDashboardService {
    /// <summary>
    /// Gets comprehensive dashboard overview data for a publisher
    /// </summary>
    /// <param name="publisherId">The publisher's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Publisher dashboard overview data</returns>
    Task<PublisherDashboardOverview> GetDashboardOverviewAsync(Guid publisherId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets publisher's key performance metrics
    /// </summary>
    /// <param name="publisherId">The publisher's unique identifier</param>
    /// <param name="timeRange">Time range for metrics (7d, 30d, 90d, 1y)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Key performance metrics</returns>
    Task<PublisherMetrics> GetPublisherMetricsAsync(Guid publisherId, string timeRange = "30d", CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets recent activity feed for the publisher
    /// </summary>
    /// <param name="publisherId">The publisher's unique identifier</param>
    /// <param name="pageSize">Number of activities to retrieve</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Recent activity entries</returns>
    Task<IEnumerable<PublisherActivity>> GetRecentActivityAsync(Guid publisherId, int pageSize = 20, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets active security alerts for publisher's packages
    /// </summary>
    /// <param name="publisherId">The publisher's unique identifier</param>
    /// <param name="severity">Filter by severity level (optional)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Active security alerts</returns>
    Task<IEnumerable<SecurityAlert>> GetSecurityAlertsAsync(Guid publisherId, SecurityScanSeverity? severity = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets download analytics data for charts and trends
    /// </summary>
    /// <param name="publisherId">The publisher's unique identifier</param>
    /// <param name="timeRange">Time range for analytics</param>
    /// <param name="granularity">Data granularity (day, week, month)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Download analytics data</returns>
    Task<DownloadAnalytics> GetDownloadAnalyticsAsync(Guid publisherId, string timeRange = "30d", string granularity = "day", CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets geographic distribution of package downloads
    /// </summary>
    /// <param name="publisherId">The publisher's unique identifier</param>
    /// <param name="timeRange">Time range for geographic data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Geographic distribution data</returns>
    Task<GeographicDistribution> GetGeographicDistributionAsync(Guid publisherId, string timeRange = "30d", CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets top performing packages for the publisher
    /// </summary>
    /// <param name="publisherId">The publisher's unique identifier</param>
    /// <param name="metric">Sorting metric (downloads, rating, growth)</param>
    /// <param name="limit">Number of packages to return</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Top performing packages</returns>
    Task<IEnumerable<PackagePerformance>> GetTopPerformingPackagesAsync(Guid publisherId, string metric = "downloads", int limit = 10, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets security score trends over time
    /// </summary>
    /// <param name="publisherId">The publisher's unique identifier</param>
    /// <param name="timeRange">Time range for security trends</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Security score trend data</returns>
    Task<SecurityTrends> GetSecurityTrendsAsync(Guid publisherId, string timeRange = "30d", CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets trust tier progression data and recommendations
    /// </summary>
    /// <param name="publisherId">The publisher's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Trust tier progression information</returns>
    Task<TrustTierProgression> GetTrustTierProgressionAsync(Guid publisherId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets notifications and alerts for the publisher
    /// </summary>
    /// <param name="publisherId">The publisher's unique identifier</param>
    /// <param name="unreadOnly">Return only unread notifications</param>
    /// <param name="pageSize">Number of notifications to retrieve</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Publisher notifications</returns>
    Task<IEnumerable<PublisherNotification>> GetNotificationsAsync(Guid publisherId, bool unreadOnly = false, int pageSize = 50, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks a notification as read
    /// </summary>
    /// <param name="notificationId">The notification identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task MarkNotificationAsReadAsync(Guid notificationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets revenue and monetization analytics (if applicable)
    /// </summary>
    /// <param name="publisherId">The publisher's unique identifier</param>
    /// <param name="timeRange">Time range for revenue data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Revenue analytics data</returns>
    Task<RevenueAnalytics?> GetRevenueAnalyticsAsync(Guid publisherId, string timeRange = "30d", CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all packages owned by a publisher for filtering and selection
    /// </summary>
    /// <param name="publisherId">The publisher's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Publisher packages result with packages list</returns>
    Task<PublisherPackagesResult> GetPublisherPackagesAsync(Guid publisherId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Comprehensive dashboard overview data
/// </summary>
public class PublisherDashboardOverview {
    public Guid PublisherId { get; set; }
    public string PublisherName { get; set; } = string.Empty;
    public TrustTier TrustTier { get; set; }
    public PublisherMetrics Metrics { get; set; } = new();
    public IEnumerable<PublisherActivity> RecentActivity { get; set; } = [];
    public IEnumerable<SecurityAlert> ActiveAlerts { get; set; } = [];
    public IEnumerable<PackagePerformance> TopPackages { get; set; } = [];
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Publisher key performance metrics
/// </summary>
public class PublisherMetrics {
    public int TotalPackages { get; set; }
    public long TotalDownloads { get; set; }
    public long WeeklyDownloads { get; set; }
    public double AverageRating { get; set; }
    public double AverageSecurityScore { get; set; }
    public int ActiveAlerts { get; set; }
    public decimal? MonthlyRevenue { get; set; }
    public double DownloadGrowthPercentage { get; set; }
    public double RatingTrend { get; set; }
    public int CountriesReached { get; set; }
}

/// <summary>
/// Publisher activity entry
/// </summary>
public class PublisherActivity {
    public Guid Id { get; set; }
    public ActivityType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? PackageName { get; set; }
    public string? Version { get; set; }
    public DateTime Timestamp { get; set; }
    public ActivityPriority Priority { get; set; } = ActivityPriority.Normal;
    public Dictionary<string, object> Metadata { get; set; } = [];
}

/// <summary>
/// Security alert information
/// </summary>
public class SecurityAlert {
    public Guid Id { get; set; }
    public string PackageName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public SecurityScanSeverity Severity { get; set; }
    public SecurityAlertType Type { get; set; }
    public DateTime DetectedAt { get; set; }
    public string? RecommendedAction { get; set; }
    public string? CveId { get; set; }
    public bool CanAutoFix { get; set; }
    public SecurityAlertStatus Status { get; set; } = SecurityAlertStatus.Active;
}

/// <summary>
/// Download analytics data
/// </summary>
public class DownloadAnalytics {
    public string TimeRange { get; set; } = string.Empty;
    public string Granularity { get; set; } = string.Empty;
    public IEnumerable<DataPoint> DownloadTrend { get; set; } = [];
    public long TotalDownloads { get; set; }
    public long PeakDownloads { get; set; }
    public DateTime PeakDate { get; set; }
    public double GrowthPercentage { get; set; }
    public IEnumerable<PackageDownloadBreakdown> PackageBreakdown { get; set; } = [];
}

/// <summary>
/// Geographic distribution data
/// </summary>
public class GeographicDistribution {
    public IEnumerable<CountryDownloads> CountryData { get; set; } = [];
    public int TotalCountries { get; set; }
    public string TopCountry { get; set; } = string.Empty;
    public double TopCountryPercentage { get; set; }
}

/// <summary>
/// Package performance data
/// </summary>
public class PackagePerformance {
    public Guid PackageId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string LatestVersion { get; set; } = string.Empty;
    public long WeeklyDownloads { get; set; }
    public double Rating { get; set; }
    public string SecurityGrade { get; set; } = string.Empty;
    public double SecurityScore { get; set; }
    public TrustTier TrustTier { get; set; }
    public double GrowthPercentage { get; set; }
    public DateTime LastUpdated { get; set; }
    public bool IsTrending { get; set; }
    public PackageStatus Status { get; set; }
}

/// <summary>
/// Security trends over time
/// </summary>
public class SecurityTrends {
    public IEnumerable<SecurityScorePoint> ScoreTrend { get; set; } = [];
    public double AverageScore { get; set; }
    public double ScoreImprovement { get; set; }
    public int TotalScans { get; set; }
    public int IssuesResolved { get; set; }
    public int ActiveIssues { get; set; }
}

/// <summary>
/// Trust tier progression information
/// </summary>
public class TrustTierProgression {
    public TrustTier CurrentTier { get; set; }
    public TrustTier? NextTier { get; set; }
    public double CompletionPercentage { get; set; }
    public IEnumerable<TrustTierRequirement> Requirements { get; set; } = [];
    public IEnumerable<string> Recommendations { get; set; } = [];
    public bool CanAdvance { get; set; }
    public DateTime? EligibilityDate { get; set; }
}

/// <summary>
/// Publisher notification
/// </summary>
public class PublisherNotification {
    public Guid Id { get; set; }
    public NotificationType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }
    public NotificationPriority Priority { get; set; } = NotificationPriority.Normal;
    public string? ActionUrl { get; set; }
    public Dictionary<string, object> Data { get; set; } = [];
}

/// <summary>
/// Revenue analytics data (for monetized packages)
/// </summary>
public class RevenueAnalytics {
    public decimal TotalRevenue { get; set; }
    public decimal MonthlyRevenue { get; set; }
    public decimal RevenueGrowth { get; set; }
    public IEnumerable<RevenueDataPoint> RevenueTrend { get; set; } = [];
    public IEnumerable<PackageRevenue> PackageBreakdown { get; set; } = [];
    public decimal AverageRevenuePerDownload { get; set; }
}

/// <summary>
/// Data point for time series data
/// </summary>
public class DataPoint {
    public DateTime Date { get; set; }
    public double Value { get; set; }
    public string Label { get; set; } = string.Empty;
}

/// <summary>
/// Package download breakdown
/// </summary>
public class PackageDownloadBreakdown {
    public string PackageName { get; set; } = string.Empty;
    public long Downloads { get; set; }
    public double Percentage { get; set; }
}

/// <summary>
/// Country downloads data
/// </summary>
public class CountryDownloads {
    public string CountryCode { get; set; } = string.Empty;
    public string CountryName { get; set; } = string.Empty;
    public long Downloads { get; set; }
    public double Percentage { get; set; }
}

/// <summary>
/// Security score data point
/// </summary>
public class SecurityScorePoint {
    public DateTime Date { get; set; }
    public double Score { get; set; }
    public string Grade { get; set; } = string.Empty;
}

/// <summary>
/// Trust tier requirement
/// </summary>
public class TrustTierRequirement {
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public bool IsRequired { get; set; }
    public int Points { get; set; }
}

/// <summary>
/// Revenue data point
/// </summary>
public class RevenueDataPoint {
    public DateTime Date { get; set; }
    public decimal Revenue { get; set; }
    public string Label { get; set; } = string.Empty;
}

/// <summary>
/// Package revenue breakdown
/// </summary>
public class PackageRevenue {
    public string PackageName { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
    public double Percentage { get; set; }
}

/// <summary>
/// Publisher packages result containing list of packages
/// </summary>
public class PublisherPackagesResult {
    public IEnumerable<PackageListItem>? Packages { get; set; }
    public int TotalCount { get; set; }
    public bool Success { get; set; } = true;
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Activity types for publisher dashboard
/// </summary>
public enum ActivityType {
    PackagePublished,
    PackageUpdated,
    SecurityScanCompleted,
    SecurityAlertTriggered,
    SecurityAlertResolved,
    TrustTierUpgraded,
    PackageTrending,
    DownloadMilestone,
    ReviewReceived,
    PackageDeprecated,
    PackageRestored,
}

/// <summary>
/// Activity priority levels
/// </summary>
public enum ActivityPriority {
    Low,
    Normal,
    High,
    Critical,
}

/// <summary>
/// Security alert types
/// </summary>
public enum SecurityAlertType {
    Vulnerability,
    DependencyUpdate,
    LicenseIssue,
    ComplianceIssue,
    MalwareDetection,
    SuspiciousBehavior,
}

/// <summary>
/// Security alert status
/// </summary>
public enum SecurityAlertStatus {
    Active,
    Acknowledged,
    InProgress,
    Resolved,
    Ignored,
}

/// <summary>
/// Notification types
/// </summary>
public enum NotificationType {
    SecurityAlert,
    PackageUpdate,
    SystemNotification,
    TrustTierUpdate,
    DownloadMilestone,
    ReviewNotification,
    PaymentNotification,
    SystemMaintenance,
}

/// <summary>
/// Notification priority levels
/// </summary>
public enum NotificationPriority {
    Low,
    Normal,
    High,
    Urgent,
}