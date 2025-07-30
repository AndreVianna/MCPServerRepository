namespace MCPHub.WebApp.Services;

/// <summary>
/// Service interface for comprehensive analytics and reporting functionality
/// </summary>
public interface IAnalyticsService {
    /// <summary>
    /// Gets comprehensive analytics dashboard data for a publisher
    /// </summary>
    /// <param name="publisherId">The publisher's unique identifier</param>
    /// <param name="timeRange">Time range for analytics (7d, 30d, 90d, 1y)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Comprehensive analytics dashboard data</returns>
    Task<AnalyticsDashboard> GetAnalyticsDashboardAsync(Guid publisherId, string timeRange = "30d", CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets detailed download analytics with multiple metrics
    /// </summary>
    /// <param name="publisherId">The publisher's unique identifier</param>
    /// <param name="packageId">Optional specific package identifier</param>
    /// <param name="timeRange">Time range for analytics</param>
    /// <param name="granularity">Data granularity (hour, day, week, month)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Detailed download analytics</returns>
    Task<DetailedDownloadAnalytics> GetDownloadAnalyticsAsync(Guid publisherId, Guid? packageId = null, string timeRange = "30d", string granularity = "day", CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets user engagement metrics and behavioral analytics
    /// </summary>
    /// <param name="publisherId">The publisher's unique identifier</param>
    /// <param name="packageId">Optional specific package identifier</param>
    /// <param name="timeRange">Time range for engagement data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User engagement analytics</returns>
    Task<UserEngagementAnalytics> GetUserEngagementAsync(Guid publisherId, Guid? packageId = null, string timeRange = "30d", CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets geographic distribution and regional analytics
    /// </summary>
    /// <param name="publisherId">The publisher's unique identifier</param>
    /// <param name="packageId">Optional specific package identifier</param>
    /// <param name="timeRange">Time range for geographic data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Geographic analytics data</returns>
    Task<GeographicAnalytics> GetGeographicAnalyticsAsync(Guid publisherId, Guid? packageId = null, string timeRange = "30d", CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets performance comparison analytics across packages
    /// </summary>
    /// <param name="publisherId">The publisher's unique identifier</param>
    /// <param name="packageIds">Specific packages to compare (optional)</param>
    /// <param name="metrics">Metrics to compare (downloads, ratings, security)</param>
    /// <param name="timeRange">Time range for comparison</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Performance comparison data</returns>
    Task<PerformanceComparison> GetPerformanceComparisonAsync(Guid publisherId, IEnumerable<Guid>? packageIds = null, IEnumerable<string>? metrics = null, string timeRange = "30d", CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets security analytics and trend data
    /// </summary>
    /// <param name="publisherId">The publisher's unique identifier</param>
    /// <param name="packageId">Optional specific package identifier</param>
    /// <param name="timeRange">Time range for security analytics</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Security analytics data</returns>
    Task<SecurityAnalytics> GetSecurityAnalyticsAsync(Guid publisherId, Guid? packageId = null, string timeRange = "30d", CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets trust tier analytics and progression insights
    /// </summary>
    /// <param name="publisherId">The publisher's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Trust tier analytics</returns>
    Task<TrustTierAnalytics> GetTrustTierAnalyticsAsync(Guid publisherId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets dependency analytics and ecosystem insights
    /// </summary>
    /// <param name="publisherId">The publisher's unique identifier</param>
    /// <param name="packageId">Optional specific package identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Dependency analytics</returns>
    Task<DependencyAnalytics> GetDependencyAnalyticsAsync(Guid publisherId, Guid? packageId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets market insights and competitive analytics
    /// </summary>
    /// <param name="publisherId">The publisher's unique identifier</param>
    /// <param name="categories">Categories to analyze</param>
    /// <param name="timeRange">Time range for market analysis</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Market insights data</returns>
    Task<MarketInsights> GetMarketInsightsAsync(Guid publisherId, IEnumerable<string>? categories = null, string timeRange = "30d", CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets financial analytics for monetized packages
    /// </summary>
    /// <param name="publisherId">The publisher's unique identifier</param>
    /// <param name="timeRange">Time range for financial analytics</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Financial analytics data (null if not applicable)</returns>
    Task<FinancialAnalytics?> GetFinancialAnalyticsAsync(Guid publisherId, string timeRange = "30d", CancellationToken cancellationToken = default);

    /// <summary>
    /// Exports analytics data in various formats
    /// </summary>
    /// <param name="publisherId">The publisher's unique identifier</param>
    /// <param name="exportRequest">Export configuration</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Export result with download information</returns>
    Task<AnalyticsExportResult> ExportAnalyticsAsync(Guid publisherId, AnalyticsExportRequest exportRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Schedules recurring analytics reports
    /// </summary>
    /// <param name="publisherId">The publisher's unique identifier</param>
    /// <param name="scheduleRequest">Report schedule configuration</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Schedule creation result</returns>
    Task<ReportScheduleResult> ScheduleReportAsync(Guid publisherId, ReportScheduleRequest scheduleRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets configured report schedules
    /// </summary>
    /// <param name="publisherId">The publisher's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of configured report schedules</returns>
    Task<IEnumerable<ReportSchedule>> GetReportSchedulesAsync(Guid publisherId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates or deletes a report schedule
    /// </summary>
    /// <param name="scheduleId">The schedule identifier</param>
    /// <param name="publisherId">The publisher identifier (for authorization)</param>
    /// <param name="updateRequest">Schedule update request (null to delete)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Schedule update result</returns>
    Task<ReportScheduleResult> UpdateReportScheduleAsync(Guid scheduleId, Guid publisherId, ReportScheduleRequest? updateRequest = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets predictive analytics and forecasting data
    /// </summary>
    /// <param name="publisherId">The publisher's unique identifier</param>
    /// <param name="forecastPeriod">Period to forecast (30d, 90d, 1y)</param>
    /// <param name="metrics">Metrics to forecast</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Predictive analytics data</returns>
    Task<PredictiveAnalytics> GetPredictiveAnalyticsAsync(Guid publisherId, string forecastPeriod = "30d", IEnumerable<string>? metrics = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets real-time analytics data for live dashboards
    /// </summary>
    /// <param name="publisherId">The publisher's unique identifier</param>
    /// <param name="metrics">Specific metrics to retrieve</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Real-time analytics data</returns>
    Task<RealTimeAnalytics> GetRealTimeAnalyticsAsync(Guid publisherId, IEnumerable<string>? metrics = null, CancellationToken cancellationToken = default);

    // Publisher-specific Analytics Methods (matching Razor component expectations)

    /// <summary>
    /// Gets comprehensive publisher analytics data for dashboard display
    /// </summary>
    /// <param name="publisherId">The publisher's unique identifier</param>
    /// <param name="timeRange">Time range for analytics (7d, 30d, 90d, 1y)</param>
    /// <param name="packageId">Optional specific package identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Publisher analytics data</returns>
    Task<PublisherAnalytics> GetPublisherAnalyticsAsync(Guid publisherId, string timeRange = "30d", string? packageId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets download trend data for chart visualization
    /// </summary>
    /// <param name="publisherId">The publisher's unique identifier</param>
    /// <param name="timeRange">Time range for download trends</param>
    /// <param name="packageId">Optional specific package identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Download trend data points</returns>
    Task<IEnumerable<ChartDataPoint>> GetDownloadTrendsAsync(Guid publisherId, string timeRange = "30d", string? packageId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets geographic distribution data for map visualization
    /// </summary>
    /// <param name="publisherId">The publisher's unique identifier</param>
    /// <param name="timeRange">Time range for geographic data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Geographic distribution data points</returns>
    Task<IEnumerable<GeographicDataPoint>> GetGeographicDistributionAsync(Guid publisherId, string timeRange = "30d", CancellationToken cancellationToken = default);

    // Global Analytics Methods for Platform-wide Statistics

    /// <summary>
    /// Gets global platform statistics and metrics
    /// </summary>
    /// <param name="timeRange">Time range for statistics</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Global platform statistics</returns>
    Task<GlobalPlatformStatistics> GetGlobalStatisticsAsync(string timeRange = "30d", CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets trending packages across the platform
    /// </summary>
    /// <param name="timeRange">Time range for trending analysis</param>
    /// <param name="category">Optional category filter</param>
    /// <param name="limit">Maximum number of trending packages to return</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Trending packages data</returns>
    Task<IEnumerable<TrendingPackageData>> GetTrendingPackagesAsync(string timeRange = "7d", string? category = null, int limit = 10, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets technology stack analysis across all packages
    /// </summary>
    /// <param name="timeRange">Time range for analysis</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Technology stack analysis</returns>
    Task<TechnologyStackAnalysis> GetTechnologyStackAnalysisAsync(string timeRange = "30d", CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets package ecosystem health metrics
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Ecosystem health metrics</returns>
    Task<EcosystemHealthMetrics> GetEcosystemHealthAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets detailed package analytics for a specific package (accessible to all users)
    /// </summary>
    /// <param name="packageId">The package identifier</param>
    /// <param name="timeRange">Time range for analytics</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Detailed package analytics</returns>
    Task<DetailedPackageAnalytics> GetPackageAnalyticsAsync(Guid packageId, string timeRange = "30d", CancellationToken cancellationToken = default);

    /// <summary>
    /// Compares multiple packages side-by-side
    /// </summary>
    /// <param name="packageIds">Package identifiers to compare</param>
    /// <param name="timeRange">Time range for comparison</param>
    /// <param name="metrics">Specific metrics to compare</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Package comparison data</returns>
    Task<PackageComparisonResult> ComparePackagesAsync(IEnumerable<Guid> packageIds, string timeRange = "30d", IEnumerable<string>? metrics = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets global market insights and competitive analysis
    /// </summary>
    /// <param name="timeRange">Time range for market analysis</param>
    /// <param name="categories">Categories to analyze</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Global market insights</returns>
    Task<GlobalMarketInsights> GetGlobalMarketInsightsAsync(string timeRange = "30d", IEnumerable<string>? categories = null, CancellationToken cancellationToken = default);
}

/// <summary>
/// Comprehensive analytics dashboard data
/// </summary>
public class AnalyticsDashboard {
    public Guid PublisherId { get; set; }
    public string TimeRange { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public OverviewMetrics Overview { get; set; } = new();
    public DownloadTrends Downloads { get; set; } = new();
    public UserEngagement Engagement { get; set; } = new();
    public GeographicSummary Geographic { get; set; } = new();
    public SecuritySummary Security { get; set; } = new();
    public IEnumerable<TopPackage> TopPackages { get; set; } = [];
    public IEnumerable<TrendingMetric> TrendingMetrics { get; set; } = [];
}

/// <summary>
/// Overview metrics summary
/// </summary>
public class OverviewMetrics {
    public long TotalDownloads { get; set; }
    public long DownloadsChange { get; set; }
    public double DownloadsChangePercentage { get; set; }
    public int TotalPackages { get; set; }
    public int ActivePackages { get; set; }
    public double AverageRating { get; set; }
    public double RatingChange { get; set; }
    public double AverageSecurityScore { get; set; }
    public double SecurityScoreChange { get; set; }
    public int CountriesReached { get; set; }
    public int NewCountries { get; set; }
}

/// <summary>
/// Download trends data
/// </summary>
public class DownloadTrends {
    public IEnumerable<DataPoint> TrendData { get; set; } = [];
    public long PeakDownloads { get; set; }
    public DateTime PeakDate { get; set; }
    public double GrowthRate { get; set; }
    public string TrendDirection { get; set; } = string.Empty;
    public IEnumerable<DownloadMilestone> Milestones { get; set; } = [];
}

/// <summary>
/// User engagement summary
/// </summary>
public class UserEngagement {
    public double AverageSessionDuration { get; set; }
    public double RetentionRate { get; set; }
    public int UniqueUsers { get; set; }
    public int ReturningUsers { get; set; }
    public double EngagementScore { get; set; }
    public IEnumerable<EngagementMetric> DetailedMetrics { get; set; } = [];
}

/// <summary>
/// Geographic summary
/// </summary>
public class GeographicSummary {
    public int TotalCountries { get; set; }
    public IEnumerable<CountryMetric> TopCountries { get; set; } = [];
    public string GrowthRegion { get; set; } = string.Empty;
    public double InternationalPercentage { get; set; }
}

/// <summary>
/// Security summary
/// </summary>
public class SecuritySummary {
    public double AverageSecurityScore { get; set; }
    public int TotalVulnerabilities { get; set; }
    public int ResolvedVulnerabilities { get; set; }
    public int ActiveAlerts { get; set; }
    public IEnumerable<SecurityTrendPoint> ScoreTrend { get; set; } = [];
}

/// <summary>
/// Top package information
/// </summary>
public class TopPackage {
    public Guid PackageId { get; set; }
    public string Name { get; set; } = string.Empty;
    public long Downloads { get; set; }
    public double Rating { get; set; }
    public double GrowthPercentage { get; set; }
    public string SecurityGrade { get; set; } = string.Empty;
}

/// <summary>
/// Trending metric
/// </summary>
public class TrendingMetric {
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public double ChangePercentage { get; set; }
    public TrendDirection Direction { get; set; }
    public string Description { get; set; } = string.Empty;
}

/// <summary>
/// Detailed download analytics
/// </summary>
public class DetailedDownloadAnalytics {
    public string TimeRange { get; set; } = string.Empty;
    public string Granularity { get; set; } = string.Empty;
    public IEnumerable<DownloadDataPoint> DownloadData { get; set; } = [];
    public DownloadStatistics Statistics { get; set; } = new();
    public IEnumerable<VersionDownloads> VersionBreakdown { get; set; } = [];
    public IEnumerable<PlatformDownloads> PlatformBreakdown { get; set; } = [];
    public IEnumerable<DownloadSource> SourceBreakdown { get; set; } = [];
    public IEnumerable<DownloadPattern> Patterns { get; set; } = [];
}

/// <summary>
/// Download data point with additional context
/// </summary>
public class DownloadDataPoint : DataPoint {
    public long DownloadCount { get; set; }
    public int UniqueUsers { get; set; }
    public IEnumerable<string> TopPackages { get; set; } = [];
}

/// <summary>
/// Download statistics
/// </summary>
public class DownloadStatistics {
    public long TotalDownloads { get; set; }
    public long AverageDaily { get; set; }
    public long MedianDaily { get; set; }
    public long PeakDaily { get; set; }
    public long LowDaily { get; set; }
    public double StandardDeviation { get; set; }
    public double GrowthRate { get; set; }
}

/// <summary>
/// Version downloads
/// </summary>
public class VersionDownloads {
    public string Version { get; set; } = string.Empty;
    public long Downloads { get; set; }
    public double Percentage { get; set; }
    public bool IsLatest { get; set; }
    public DateTime ReleaseDate { get; set; }
}

/// <summary>
/// Platform downloads
/// </summary>
public class PlatformDownloads {
    public string Platform { get; set; } = string.Empty;
    public long Downloads { get; set; }
    public double Percentage { get; set; }
    public IEnumerable<string> TopVersions { get; set; } = [];
}

/// <summary>
/// Download source information
/// </summary>
public class DownloadSource {
    public string Source { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public long Downloads { get; set; }
    public double Percentage { get; set; }
}

/// <summary>
/// Download pattern analysis
/// </summary>
public class DownloadPattern {
    public string Pattern { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double Confidence { get; set; }
    public IEnumerable<string> AffectedPackages { get; set; } = [];
}

/// <summary>
/// User engagement analytics
/// </summary>
public class UserEngagementAnalytics {
    public string TimeRange { get; set; } = string.Empty;
    public UserMetrics UserMetrics { get; set; } = new();
    public SessionMetrics SessionMetrics { get; set; } = new();
    public RetentionMetrics RetentionMetrics { get; set; } = new();
    public IEnumerable<EngagementTrend> EngagementTrends { get; set; } = [];
    public IEnumerable<UserSegment> UserSegments { get; set; } = [];
    public IEnumerable<BehaviorPattern> BehaviorPatterns { get; set; } = [];
}

/// <summary>
/// User metrics
/// </summary>
public class UserMetrics {
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int NewUsers { get; set; }
    public int ReturningUsers { get; set; }
    public double NewUserPercentage { get; set; }
    public double UserGrowthRate { get; set; }
}

/// <summary>
/// Session metrics
/// </summary>
public class SessionMetrics {
    public TimeSpan AverageSessionDuration { get; set; }
    public TimeSpan MedianSessionDuration { get; set; }
    public int AverageDownloadsPerSession { get; set; }
    public double BounceRate { get; set; }
    public int TotalSessions { get; set; }
}

/// <summary>
/// Retention metrics
/// </summary>
public class RetentionMetrics {
    public double Day1Retention { get; set; }
    public double Day7Retention { get; set; }
    public double Day30Retention { get; set; }
    public double ChurnRate { get; set; }
    public IEnumerable<RetentionCohort> Cohorts { get; set; } = [];
}

/// <summary>
/// Engagement trend
/// </summary>
public class EngagementTrend {
    public DateTime Date { get; set; }
    public double EngagementScore { get; set; }
    public int ActiveUsers { get; set; }
    public TimeSpan AverageSessionDuration { get; set; }
}

/// <summary>
/// User segment
/// </summary>
public class UserSegment {
    public string SegmentName { get; set; } = string.Empty;
    public int UserCount { get; set; }
    public double Percentage { get; set; }
    public double EngagementScore { get; set; }
    public IEnumerable<string> TopPackages { get; set; } = [];
}

/// <summary>
/// Behavior pattern
/// </summary>
public class BehaviorPattern {
    public string Pattern { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int UserCount { get; set; }
    public double Impact { get; set; }
}

/// <summary>
/// Retention cohort
/// </summary>
public class RetentionCohort {
    public DateTime CohortDate { get; set; }
    public int InitialUsers { get; set; }
    public double Day1Retention { get; set; }
    public double Day7Retention { get; set; }
    public double Day30Retention { get; set; }
}

/// <summary>
/// Geographic analytics
/// </summary>
public class GeographicAnalytics {
    public string TimeRange { get; set; } = string.Empty;
    public int TotalCountries { get; set; }
    public IEnumerable<CountryAnalytics> CountryData { get; set; } = [];
    public IEnumerable<RegionAnalytics> RegionData { get; set; } = [];
    public IEnumerable<CityAnalytics> CityData { get; set; } = [];
    public GrowthAnalysis GrowthAnalysis { get; set; } = new();
}

/// <summary>
/// Country analytics
/// </summary>
public class CountryAnalytics {
    public string CountryCode { get; set; } = string.Empty;
    public string CountryName { get; set; } = string.Empty;
    public long Downloads { get; set; }
    public double Percentage { get; set; }
    public double GrowthRate { get; set; }
    public int UniqueUsers { get; set; }
    public double AverageRating { get; set; }
    public IEnumerable<string> PopularPackages { get; set; } = [];
}

/// <summary>
/// Region analytics
/// </summary>
public class RegionAnalytics {
    public string Region { get; set; } = string.Empty;
    public long Downloads { get; set; }
    public double Percentage { get; set; }
    public double GrowthRate { get; set; }
    public int CountryCount { get; set; }
}

/// <summary>
/// City analytics
/// </summary>
public class CityAnalytics {
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public long Downloads { get; set; }
    public double Percentage { get; set; }
}

/// <summary>
/// Growth analysis
/// </summary>
public class GrowthAnalysis {
    public string FastestGrowingCountry { get; set; } = string.Empty;
    public double FastestGrowthRate { get; set; }
    public IEnumerable<string> EmergingMarkets { get; set; } = [];
    public IEnumerable<string> MatureMarkets { get; set; } = [];
}

/// <summary>
/// Performance comparison analytics
/// </summary>
public class PerformanceComparison {
    public string TimeRange { get; set; } = string.Empty;
    public IEnumerable<string> ComparedMetrics { get; set; } = [];
    public IEnumerable<PackageComparison> PackageComparisons { get; set; } = [];
    public ComparisonSummary Summary { get; set; } = new();
    public IEnumerable<PerformanceInsight> Insights { get; set; } = [];
}

/// <summary>
/// Package comparison data
/// </summary>
public class PackageComparison {
    public Guid PackageId { get; set; }
    public string PackageName { get; set; } = string.Empty;
    public Dictionary<string, double> Metrics { get; set; } = [];
    public Dictionary<string, double> Changes { get; set; } = [];
    public int Rank { get; set; }
    public string Category { get; set; } = string.Empty;
}

/// <summary>
/// Comparison summary
/// </summary>
public class ComparisonSummary {
    public string TopPerformer { get; set; } = string.Empty;
    public string FastestGrowing { get; set; } = string.Empty;
    public string MostConsistent { get; set; } = string.Empty;
    public double AverageGrowth { get; set; }
    public string TrendingCategory { get; set; } = string.Empty;
}

/// <summary>
/// Performance insight
/// </summary>
public class PerformanceInsight {
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public InsightType Type { get; set; }
    public IEnumerable<string> AffectedPackages { get; set; } = [];
    public IEnumerable<string> Recommendations { get; set; } = [];
}

/// <summary>
/// Security analytics
/// </summary>
public class SecurityAnalytics {
    public string TimeRange { get; set; } = string.Empty;
    public SecurityOverview Overview { get; set; } = new();
    public IEnumerable<SecurityTrendPoint> ScoreTrends { get; set; } = [];
    public IEnumerable<VulnerabilityAnalysis> Vulnerabilities { get; set; } = [];
    public IEnumerable<SecurityIncident> Incidents { get; set; } = [];
    public SecurityComparison Comparison { get; set; } = new();
    public IEnumerable<SecurityRecommendation> Recommendations { get; set; } = [];
}

/// <summary>
/// Security overview
/// </summary>
public class SecurityOverview {
    public double AverageSecurityScore { get; set; }
    public double SecurityScoreChange { get; set; }
    public int TotalScans { get; set; }
    public int PassedScans { get; set; }
    public int FailedScans { get; set; }
    public int ActiveVulnerabilities { get; set; }
    public int ResolvedVulnerabilities { get; set; }
}

/// <summary>
/// Security trend point
/// </summary>
public class SecurityTrendPoint {
    public DateTime Date { get; set; }
    public double Score { get; set; }
    public string Grade { get; set; } = string.Empty;
    public int VulnerabilityCount { get; set; }
}

/// <summary>
/// Vulnerability analysis
/// </summary>
public class VulnerabilityAnalysis {
    public string VulnerabilityId { get; set; } = string.Empty;
    public SecurityScanSeverity Severity { get; set; }
    public string Description { get; set; } = string.Empty;
    public IEnumerable<string> AffectedPackages { get; set; } = [];
    public DateTime DetectedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public TimeSpan? ResolutionTime { get; set; }
}

/// <summary>
/// Security incident
/// </summary>
public class SecurityIncident {
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public SecurityScanSeverity Severity { get; set; }
    public DateTime OccurredAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public IEnumerable<string> AffectedPackages { get; set; } = [];
}

/// <summary>
/// Security comparison
/// </summary>
public class SecurityComparison {
    public double IndustryAverageScore { get; set; }
    public double PublisherScore { get; set; }
    public int IndustryRanking { get; set; }
    public string PerformanceLevel { get; set; } = string.Empty;
}

/// <summary>
/// Security recommendation
/// </summary>
public class SecurityRecommendation {
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public RecommendationPriority Priority { get; set; }
    public IEnumerable<string> AffectedPackages { get; set; } = [];
    public string Action { get; set; } = string.Empty;
}

/// <summary>
/// Analytics export request
/// </summary>
public class AnalyticsExportRequest {
    public ExportFormat Format { get; set; } = ExportFormat.Csv;
    public string TimeRange { get; set; } = "30d";
    public IEnumerable<string> Metrics { get; set; } = [];
    public IEnumerable<Guid> PackageIds { get; set; } = [];
    public bool IncludeRawData { get; set; }
    public bool IncludeCharts { get; set; }
    public string? CustomQuery { get; set; }
}

/// <summary>
/// Analytics export result
/// </summary>
public class AnalyticsExportResult {
    public bool Success { get; set; }
    public string? FileUrl { get; set; }
    public string? FileName { get; set; }
    public long FileSizeBytes { get; set; }
    public DateTime ExpiresAt { get; set; }
    public string? ErrorMessage { get; set; }
    public ExportFormat Format { get; set; }
}

/// <summary>
/// Download milestone
/// </summary>
public class DownloadMilestone {
    public long Threshold { get; set; }
    public DateTime AchievedAt { get; set; }
    public string PackageName { get; set; } = string.Empty;
}

/// <summary>
/// Engagement metric
/// </summary>
public class EngagementMetric {
    public string Name { get; set; } = string.Empty;
    public double Value { get; set; }
    public string Unit { get; set; } = string.Empty;
    public double Change { get; set; }
}

/// <summary>
/// Country metric
/// </summary>
public class CountryMetric {
    public string CountryCode { get; set; } = string.Empty;
    public string CountryName { get; set; } = string.Empty;
    public long Downloads { get; set; }
    public double Percentage { get; set; }
}

/// <summary>
/// Trend direction
/// </summary>
public enum TrendDirection {
    Up,
    Down,
    Stable,
    Volatile,
}

/// <summary>
/// Insight types
/// </summary>
public enum InsightType {
    Opportunity,
    Warning,
    Achievement,
    Trend,
    Recommendation,
}

/// <summary>
/// Recommendation priority
/// </summary>
public enum RecommendationPriority {
    Low,
    Medium,
    High,
    Critical,
}

// Additional classes for trust tier, dependency, market insights, financial, predictive, and real-time analytics would be defined here
// These follow similar patterns but are omitted for brevity

/// <summary>
/// Trust tier analytics (placeholder for additional implementation)
/// </summary>
public class TrustTierAnalytics {
    public TrustTier CurrentTier { get; set; }
    public double ProgressToNext { get; set; }
    public IEnumerable<TrustTierRequirement> Requirements { get; set; } = [];
    // Additional properties...
}

/// <summary>
/// Dependency analytics (placeholder for additional implementation)
/// </summary>
public class DependencyAnalytics {
    public int TotalDependencies { get; set; }
    public int OutdatedDependencies { get; set; }
    public IEnumerable<DependencyInsight> Insights { get; set; } = [];
    // Additional properties...
}

/// <summary>
/// Market insights (placeholder for additional implementation)
/// </summary>
public class MarketInsights {
    public IEnumerable<MarketTrend> Trends { get; set; } = [];
    public IEnumerable<CompetitorAnalysis> Competitors { get; set; } = [];
    // Additional properties...
}

/// <summary>
/// Financial analytics (placeholder for additional implementation)
/// </summary>
public class FinancialAnalytics {
    public decimal TotalRevenue { get; set; }
    public IEnumerable<RevenueDataPoint> RevenueTrend { get; set; } = [];
    // Additional properties...
}

/// <summary>
/// Predictive analytics (placeholder for additional implementation)
/// </summary>
public class PredictiveAnalytics {
    public IEnumerable<PredictionModel> Predictions { get; set; } = [];
    public double ConfidenceLevel { get; set; }
    // Additional properties...
}

/// <summary>
/// Real-time analytics (placeholder for additional implementation)
/// </summary>
public class RealTimeAnalytics {
    public Dictionary<string, object> LiveMetrics { get; set; } = [];
    public DateTime LastUpdated { get; set; }
    // Additional properties...
}

// Report scheduling classes (placeholder for additional implementation)
public class ReportScheduleRequest { }
public class ReportScheduleResult { }
public class ReportSchedule { }

// Placeholder classes for market insights
public class MarketTrend { }
public class CompetitorAnalysis { }
public class DependencyInsight { }
public class PredictionModel { }

// Global Analytics Model Classes

/// <summary>
/// Global platform statistics
/// </summary>
public class GlobalPlatformStatistics {
    public string TimeRange { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public GlobalOverviewMetrics Overview { get; set; } = new();
    public GlobalDownloadTrends Downloads { get; set; } = new();
    public GlobalUserEngagement Users { get; set; } = new();
    public GlobalGeographicDistribution Geographic { get; set; } = new();
    public GlobalSecurityMetrics Security { get; set; } = new();
    public GlobalTrustTierDistribution TrustTiers { get; set; } = new();
    public IEnumerable<CategoryStatistics> Categories { get; set; } = [];
    public IEnumerable<TrendingPackageData> TrendingPackages { get; set; } = [];
    public EcosystemGrowthMetrics Growth { get; set; } = new();
}

/// <summary>
/// Global overview metrics
/// </summary>
public class GlobalOverviewMetrics {
    public long TotalPackages { get; set; }
    public long TotalDownloads { get; set; }
    public int ActivePublishers { get; set; }
    public int TotalUsers { get; set; }
    public double AveragePackageRating { get; set; }
    public double AverageSecurityScore { get; set; }
    public int CountriesReached { get; set; }
    public double GrowthRate { get; set; }
    public long NewPackages { get; set; }
    public long PackageUpdates { get; set; }
    public Dictionary<string, long> MetricChanges { get; set; } = [];
}

/// <summary>
/// Global download trends
/// </summary>
public class GlobalDownloadTrends {
    public IEnumerable<DataPoint> TrendData { get; set; } = [];
    public long DailyAverageDownloads { get; set; }
    public long PeakDailyDownloads { get; set; }
    public DateTime PeakDate { get; set; }
    public double GrowthRate { get; set; }
    public IEnumerable<DownloadMilestone> RecentMilestones { get; set; } = [];
}

/// <summary>
/// Global user engagement metrics
/// </summary>
public class GlobalUserEngagement {
    public int TotalActiveUsers { get; set; }
    public int NewUsersThisPeriod { get; set; }
    public double UserRetentionRate { get; set; }
    public TimeSpan AverageSessionDuration { get; set; }
    public double AveragePackagesPerUser { get; set; }
    public IEnumerable<UserSegmentMetrics> UserSegments { get; set; } = [];
}

/// <summary>
/// Global geographic distribution
/// </summary>
public class GlobalGeographicDistribution {
    public int TotalCountries { get; set; }
    public IEnumerable<CountryAnalytics> TopCountries { get; set; } = [];
    public IEnumerable<RegionAnalytics> RegionData { get; set; } = [];
    public string FastestGrowingRegion { get; set; } = string.Empty;
    public double InternationalPercentage { get; set; }
}

/// <summary>
/// Global security metrics
/// </summary>
public class GlobalSecurityMetrics {
    public double AverageSecurityScore { get; set; }
    public int TotalSecurityScans { get; set; }
    public int ActiveVulnerabilities { get; set; }
    public int ResolvedVulnerabilities { get; set; }
    public Dictionary<string, int> SecurityGradeDistribution { get; set; } = [];
    public IEnumerable<SecurityTrendPoint> ScoreTrend { get; set; } = [];
}

/// <summary>
/// Global trust tier distribution
/// </summary>
public class GlobalTrustTierDistribution {
    public Dictionary<TrustTier, int> TierDistribution { get; set; } = [];
    public Dictionary<TrustTier, double> TierPercentages { get; set; } = [];
    public TrustTier MostCommonTier { get; set; }
    public double AverageTrustScore { get; set; }
    public IEnumerable<TrustTierTrend> TierTrends { get; set; } = [];
}

/// <summary>
/// Category statistics
/// </summary>
public class CategoryStatistics {
    public string Category { get; set; } = string.Empty;
    public int PackageCount { get; set; }
    public long TotalDownloads { get; set; }
    public double AverageRating { get; set; }
    public double GrowthRate { get; set; }
    public IEnumerable<string> TopPackages { get; set; } = [];
}

/// <summary>
/// Trending package data
/// </summary>
public class TrendingPackageData {
    public Guid PackageId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Publisher { get; set; } = string.Empty;
    public long Downloads { get; set; }
    public double GrowthPercentage { get; set; }
    public double Rating { get; set; }
    public string SecurityGrade { get; set; } = string.Empty;
    public TrustTier TrustTier { get; set; }
    public DateTime LastUpdated { get; set; }
    public IEnumerable<string> Tags { get; set; } = [];
}

/// <summary>
/// Ecosystem growth metrics
/// </summary>
public class EcosystemGrowthMetrics {
    public double PackageGrowthRate { get; set; }
    public double PublisherGrowthRate { get; set; }
    public double UserGrowthRate { get; set; }
    public double DownloadGrowthRate { get; set; }
    public IEnumerable<GrowthMilestone> RecentMilestones { get; set; } = [];
    public IEnumerable<DataPoint> GrowthTrend { get; set; } = [];
}

/// <summary>
/// Technology stack analysis
/// </summary>
public class TechnologyStackAnalysis {
    public Dictionary<string, TechnologyUsage> Technologies { get; set; } = [];
    public IEnumerable<TechnologyTrend> TrendingTechnologies { get; set; } = [];
    public IEnumerable<TechnologyCompatibility> CompatibilityMatrix { get; set; } = [];
    public IEnumerable<TechnologyInsight> Insights { get; set; } = [];
}

/// <summary>
/// Technology usage metrics
/// </summary>
public class TechnologyUsage {
    public string Technology { get; set; } = string.Empty;
    public int PackageCount { get; set; }
    public double MarketShare { get; set; }
    public double GrowthRate { get; set; }
    public IEnumerable<string> PopularPackages { get; set; } = [];
}

/// <summary>
/// Technology trend data
/// </summary>
public class TechnologyTrend {
    public string Technology { get; set; } = string.Empty;
    public double TrendScore { get; set; }
    public TrendDirection Direction { get; set; }
    public string Description { get; set; } = string.Empty;
}

/// <summary>
/// Technology compatibility information
/// </summary>
public class TechnologyCompatibility {
    public string Technology1 { get; set; } = string.Empty;
    public string Technology2 { get; set; } = string.Empty;
    public double CompatibilityScore { get; set; }
    public int PackagesUsing { get; set; }
}

/// <summary>
/// Technology insight
/// </summary>
public class TechnologyInsight {
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public InsightType Type { get; set; }
    public IEnumerable<string> AffectedTechnologies { get; set; } = [];
}

/// <summary>
/// Ecosystem health metrics
/// </summary>
public class EcosystemHealthMetrics {
    public double OverallHealthScore { get; set; }
    public EcosystemHealthIndicators Indicators { get; set; } = new();
    public IEnumerable<HealthTrend> HealthTrends { get; set; } = [];
    public IEnumerable<HealthAlert> Alerts { get; set; } = [];
    public IEnumerable<HealthRecommendation> Recommendations { get; set; } = [];
}

/// <summary>
/// Ecosystem health indicators
/// </summary>
public class EcosystemHealthIndicators {
    public double SecurityHealth { get; set; }
    public double QualityHealth { get; set; }
    public double DiversityHealth { get; set; }
    public double ActivityHealth { get; set; }
    public double CommunityHealth { get; set; }
    public double InnovationHealth { get; set; }
}

/// <summary>
/// Health trend data
/// </summary>
public class HealthTrend {
    public DateTime Date { get; set; }
    public double HealthScore { get; set; }
    public string Category { get; set; } = string.Empty;
}

/// <summary>
/// Health alert
/// </summary>
public class HealthAlert {
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public AlertSeverity Severity { get; set; }
    public DateTime DetectedAt { get; set; }
    public string Category { get; set; } = string.Empty;
}

/// <summary>
/// Health recommendation
/// </summary>
public class HealthRecommendation {
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public RecommendationPriority Priority { get; set; }
    public string Category { get; set; } = string.Empty;
}

/// <summary>
/// Detailed package analytics for individual packages
/// </summary>
public class DetailedPackageAnalytics {
    public Guid PackageId { get; set; }
    public string PackageName { get; set; } = string.Empty;
    public string TimeRange { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public PackageOverviewMetrics Overview { get; set; } = new();
    public DetailedDownloadAnalytics Downloads { get; set; } = new();
    public PackageUserEngagement UserEngagement { get; set; } = new();
    public PackageVersionAnalytics Versions { get; set; } = new();
    public PackageDependencyAnalysis Dependencies { get; set; } = new();
    public PackageSecurityAnalysis Security { get; set; } = new();
    public PackagePerformanceMetrics Performance { get; set; } = new();
    public IEnumerable<PackageFeedback> RecentFeedback { get; set; } = [];
}

/// <summary>
/// Package overview metrics
/// </summary>
public class PackageOverviewMetrics {
    public long TotalDownloads { get; set; }
    public long DownloadsThisPeriod { get; set; }
    public double GrowthRate { get; set; }
    public double AverageRating { get; set; }
    public int TotalRatings { get; set; }
    public double SecurityScore { get; set; }
    public TrustTier TrustTier { get; set; }
    public int FavoriteCount { get; set; }
    public DateTime LastUpdated { get; set; }
    public int IssueCount { get; set; }
}

/// <summary>
/// Package user engagement metrics
/// </summary>
public class PackageUserEngagement {
    public int UniqueUsers { get; set; }
    public int ActiveUsers { get; set; }
    public double RetentionRate { get; set; }
    public TimeSpan AverageUsageSession { get; set; }
    public IEnumerable<UserSegmentData> UserSegments { get; set; } = [];
    public IEnumerable<EngagementTrend> EngagementTrends { get; set; } = [];
}

/// <summary>
/// Package version analytics
/// </summary>
public class PackageVersionAnalytics {
    public IEnumerable<VersionDownloads> VersionBreakdown { get; set; } = [];
    public double AdoptionRate { get; set; }
    public TimeSpan AverageUpgradeTime { get; set; }
    public IEnumerable<VersionTrend> VersionTrends { get; set; } = [];
    public IEnumerable<VersionCompatibility> CompatibilityData { get; set; } = [];
}

/// <summary>
/// Package dependency analysis
/// </summary>
public class PackageDependencyAnalysis {
    public IEnumerable<DependencyUsage> Dependencies { get; set; } = [];
    public IEnumerable<DependentPackage> Dependents { get; set; } = [];
    public double DependencyHealth { get; set; }
    public IEnumerable<DependencyAlert> Alerts { get; set; } = [];
    public IEnumerable<DependencyRecommendation> Recommendations { get; set; } = [];
}

/// <summary>
/// Package security analysis
/// </summary>
public class PackageSecurityAnalysis {
    public double SecurityScore { get; set; }
    public string SecurityGrade { get; set; } = string.Empty;
    public IEnumerable<SecurityScanResult> RecentScans { get; set; } = [];
    public IEnumerable<SecurityVulnerability> Vulnerabilities { get; set; } = [];
    public IEnumerable<SecurityInsight> SecurityInsights { get; set; } = [];
}

/// <summary>
/// Package performance metrics
/// </summary>
public class PackagePerformanceMetrics {
    public TimeSpan AverageInstallTime { get; set; }
    public double SuccessRate { get; set; }
    public IEnumerable<PerformanceBenchmark> Benchmarks { get; set; } = [];
    public IEnumerable<CompatibilityReport> CompatibilityReports { get; set; } = [];
}

/// <summary>
/// Package feedback
/// </summary>
public class PackageFeedback {
    public string Type { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public double? Rating { get; set; }
    public DateTime CreatedAt { get; set; }
    public string UserName { get; set; } = string.Empty;
}

/// <summary>
/// Package comparison result
/// </summary>
public class PackageComparisonResult {
    public string TimeRange { get; set; } = string.Empty;
    public IEnumerable<Guid> ComparedPackages { get; set; } = [];
    public IEnumerable<PackageComparisonData> ComparisonData { get; set; } = [];
    public ComparisonMatrix ComparisonMatrix { get; set; } = new();
    public IEnumerable<ComparisonInsight> Insights { get; set; } = [];
    public ComparisonSummary Summary { get; set; } = new();
}

/// <summary>
/// Package comparison data
/// </summary>
public class PackageComparisonData {
    public Guid PackageId { get; set; }
    public string PackageName { get; set; } = string.Empty;
    public Dictionary<string, object> Metrics { get; set; } = [];
    public Dictionary<string, double> NormalizedScores { get; set; } = [];
    public int OverallRank { get; set; }
}

/// <summary>
/// Comparison matrix
/// </summary>
public class ComparisonMatrix {
    public IEnumerable<string> Metrics { get; set; } = [];
    public Dictionary<string, Dictionary<Guid, double>> Matrix { get; set; } = [];
    public Dictionary<string, Guid> Winners { get; set; } = [];
}

/// <summary>
/// Comparison insight
/// </summary>
public class ComparisonInsight {
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public InsightType Type { get; set; }
    public IEnumerable<Guid> AffectedPackages { get; set; } = [];
}

/// <summary>
/// Global market insights
/// </summary>
public class GlobalMarketInsights {
    public string TimeRange { get; set; } = string.Empty;
    public MarketOverview Overview { get; set; } = new();
    public IEnumerable<MarketTrendData> Trends { get; set; } = [];
    public IEnumerable<CategoryInsight> CategoryInsights { get; set; } = [];
    public IEnumerable<CompetitiveAnalysis> CompetitiveAnalysis { get; set; } = [];
    public IEnumerable<MarketOpportunity> Opportunities { get; set; } = [];
    public MarketForecast Forecast { get; set; } = new();
}

/// <summary>
/// Market overview
/// </summary>
public class MarketOverview {
    public long TotalMarketSize { get; set; }
    public double GrowthRate { get; set; }
    public int ActiveCategories { get; set; }
    public string LeadingCategory { get; set; } = string.Empty;
    public double MarketConcentration { get; set; }
    public IEnumerable<MarketLeader> MarketLeaders { get; set; } = [];
}

/// <summary>
/// Market trend data
/// </summary>
public class MarketTrendData {
    public string Trend { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double Impact { get; set; }
    public TrendDirection Direction { get; set; }
    public IEnumerable<string> AffectedCategories { get; set; } = [];
}

/// <summary>
/// Category insight
/// </summary>
public class CategoryInsight {
    public string Category { get; set; } = string.Empty;
    public long MarketSize { get; set; }
    public double GrowthRate { get; set; }
    public int PackageCount { get; set; }
    public double CompetitionLevel { get; set; }
    public IEnumerable<string> KeyPlayers { get; set; } = [];
    public IEnumerable<string> EmergingTrends { get; set; } = [];
}

/// <summary>
/// Competitive analysis
/// </summary>
public class CompetitiveAnalysis {
    public string Category { get; set; } = string.Empty;
    public IEnumerable<CompetitorProfile> Competitors { get; set; } = [];
    public MarketPositioning Positioning { get; set; } = new();
    public IEnumerable<CompetitiveAdvantage> Advantages { get; set; } = [];
}

/// <summary>
/// Market opportunity
/// </summary>
public class MarketOpportunity {
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double OpportunitySize { get; set; }
    public double DifficultyLevel { get; set; }
    public string Category { get; set; } = string.Empty;
    public IEnumerable<string> Requirements { get; set; } = [];
}

/// <summary>
/// Market forecast
/// </summary>
public class MarketForecast {
    public string ForecastPeriod { get; set; } = string.Empty;
    public IEnumerable<ForecastDataPoint> Projections { get; set; } = [];
    public double ConfidenceLevel { get; set; }
    public IEnumerable<ForecastAssumption> Assumptions { get; set; } = [];
}

// Additional supporting classes
public class UserSegmentMetrics { }
public class TrustTierTrend { }
public class GrowthMilestone { }
public class UserSegmentData { }
public class VersionTrend { }
public class VersionCompatibility { }
public class DependencyUsage { }
public class DependentPackage { }
public class DependencyAlert { }
public class DependencyRecommendation { }
public class SecurityInsight { }
public class PerformanceBenchmark { }
public class CompatibilityReport { }
public class MarketLeader { }
public class CompetitiveAdvantage { }
public class CompetitorProfile { }
public class MarketPositioning { }
public class ForecastDataPoint { }
public class ForecastAssumption { }

/// <summary>
/// Alert severity levels
/// </summary>
public enum AlertSeverity {
    Low,
    Medium,
    High,
    Critical,
}

// Publisher Analytics Specific Types (matching Razor component expectations)

/// <summary>
/// Publisher analytics data for dashboard display
/// </summary>
public class PublisherAnalytics {
    public long TotalDownloads { get; set; }
    public double DownloadGrowth { get; set; }
    public int ActiveUsers { get; set; }
    public double UserGrowth { get; set; }
    public double AverageRating { get; set; }
    public double RatingChange { get; set; }
    public double SecurityScore { get; set; }
    public double SecurityScoreChange { get; set; }
    public int TotalReviews { get; set; }
    public int TotalFavorites { get; set; }
    public int TotalIssues { get; set; }
    public double CommunityScore { get; set; }
}

/// <summary>
/// Chart data point for time series visualization
/// </summary>
public class ChartDataPoint {
    public DateTime Date { get; set; }
    public long Value { get; set; }
}

/// <summary>
/// Geographic data point for map visualization
/// </summary>
public class GeographicDataPoint {
    public string Region { get; set; } = string.Empty;
    public long Downloads { get; set; }
    public double Percentage { get; set; }
}