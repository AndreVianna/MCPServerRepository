namespace MCPHub.Common.Services;

/// <summary>
/// Kubernetes cluster monitoring service
/// </summary>
public interface IClusterMonitoringService
{
    /// <summary>
    /// Gets cluster health status
    /// </summary>
    Task<ClusterHealth> GetClusterHealthAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets node resource utilization
    /// </summary>
    Task<IEnumerable<NodeMetrics>> GetNodeMetricsAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets pod status and resource usage
    /// </summary>
    Task<IEnumerable<PodMetrics>> GetPodMetricsAsync(string? namespaceFilter = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets service mesh metrics
    /// </summary>
    Task<ServiceMeshMetrics> GetServiceMeshMetricsAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets persistent volume usage
    /// </summary>
    Task<IEnumerable<VolumeMetrics>> GetVolumeMetricsAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets network policy effectiveness
    /// </summary>
    Task<NetworkPolicyMetrics> GetNetworkPolicyMetricsAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets horizontal pod autoscaler status
    /// </summary>
    Task<IEnumerable<HPAStatus>> GetHPAStatusAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets cluster events
    /// </summary>
    Task<IEnumerable<ClusterEvent>> GetClusterEventsAsync(TimeRange timeRange, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Triggers cluster health check
    /// </summary>
    Task<ClusterHealthCheckResult> PerformHealthCheckAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets resource quotas and limits
    /// </summary>
    Task<IEnumerable<ResourceQuotaStatus>> GetResourceQuotasAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Database performance monitoring service
/// </summary>
public interface IPerformanceInsightsService
{
    /// <summary>
    /// Gets database performance metrics
    /// </summary>
    Task<DatabasePerformanceMetrics> GetPerformanceMetricsAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets slow query analysis
    /// </summary>
    Task<IEnumerable<SlowQueryAnalysis>> GetSlowQueriesAsync(TimeRange timeRange, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets connection pool metrics
    /// </summary>
    Task<ConnectionPoolMetrics> GetConnectionPoolMetricsAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets database size and growth metrics
    /// </summary>
    Task<DatabaseSizeMetrics> GetDatabaseSizeMetricsAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets index usage statistics
    /// </summary>
    Task<IEnumerable<IndexUsageStats>> GetIndexUsageStatsAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets lock contention analysis
    /// </summary>
    Task<LockContentionAnalysis> GetLockContentionAnalysisAsync(TimeRange timeRange, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets replication lag metrics (if applicable)
    /// </summary>
    Task<ReplicationMetrics?> GetReplicationMetricsAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets vacuum/maintenance statistics
    /// </summary>
    Task<MaintenanceStats> GetMaintenanceStatsAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Analyzes query plan performance
    /// </summary>
    Task<QueryPlanAnalysis> AnalyzeQueryPlanAsync(string query, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets database health recommendations
    /// </summary>
    Task<IEnumerable<PerformanceRecommendation>> GetRecommendationsAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Network monitoring and service mesh observability
/// </summary>
public interface INetworkMonitoringService
{
    /// <summary>
    /// Gets service-to-service communication metrics
    /// </summary>
    Task<IEnumerable<ServiceCommunicationMetrics>> GetServiceCommunicationMetricsAsync(TimeRange timeRange, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets network latency metrics
    /// </summary>
    Task<NetworkLatencyMetrics> GetNetworkLatencyMetricsAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets bandwidth utilization
    /// </summary>
    Task<BandwidthUtilizationMetrics> GetBandwidthUtilizationAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets load balancer metrics
    /// </summary>
    Task<LoadBalancerMetrics> GetLoadBalancerMetricsAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets DNS resolution metrics
    /// </summary>
    Task<DNSMetrics> GetDNSMetricsAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets TLS/SSL certificate status
    /// </summary>
    Task<IEnumerable<CertificateStatus>> GetCertificateStatusAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets ingress controller metrics
    /// </summary>
    Task<IngressControllerMetrics> GetIngressControllerMetricsAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets CDN performance metrics
    /// </summary>
    Task<CDNMetrics?> GetCDNMetricsAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Traces network path performance
    /// </summary>
    Task<NetworkPathTrace> TraceNetworkPathAsync(string source, string destination, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets security group/firewall rule effectiveness
    /// </summary>
    Task<SecurityGroupMetrics> GetSecurityGroupMetricsAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Cost monitoring and resource optimization service
/// </summary>
public interface ICostMonitoringService
{
    /// <summary>
    /// Gets current cost breakdown
    /// </summary>
    Task<CostBreakdown> GetCurrentCostsAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets cost trends over time
    /// </summary>
    Task<CostTrends> GetCostTrendsAsync(TimeRange timeRange, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets resource utilization efficiency
    /// </summary>
    Task<ResourceEfficiencyMetrics> GetResourceEfficiencyAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets cost optimization recommendations
    /// </summary>
    Task<IEnumerable<CostOptimizationRecommendation>> GetOptimizationRecommendationsAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets budget alerts and forecasts
    /// </summary>
    Task<BudgetStatus> GetBudgetStatusAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets rightsizing recommendations
    /// </summary>
    Task<IEnumerable<RightsizingRecommendation>> GetRightsizingRecommendationsAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets reserved capacity recommendations
    /// </summary>
    Task<IEnumerable<ReservedCapacityRecommendation>> GetReservedCapacityRecommendationsAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets cost allocation by team/project
    /// </summary>
    Task<CostAllocation> GetCostAllocationAsync(string groupBy, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Forecasts future costs
    /// </summary>
    Task<CostForecast> ForecastCostsAsync(int daysAhead, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets cost anomaly detection
    /// </summary>
    Task<IEnumerable<CostAnomaly>> GetCostAnomaliesAsync(TimeRange timeRange, CancellationToken cancellationToken = default);
}

// Supporting data structures for Cluster Monitoring

/// <summary>
/// Cluster health status
/// </summary>
public record ClusterHealth(
    bool IsHealthy,
    int TotalNodes,
    int HealthyNodes,
    int TotalPods,
    int RunningPods,
    double CpuUtilization,
    double MemoryUtilization,
    List<string> Issues
);

/// <summary>
/// Node metrics
/// </summary>
public record NodeMetrics(
    string NodeName,
    string Status,
    double CpuUsage,
    double MemoryUsage,
    double DiskUsage,
    double NetworkIn,
    double NetworkOut,
    int PodCount,
    DateTime LastHeartbeat
);

/// <summary>
/// Pod metrics
/// </summary>
public record PodMetrics(
    string PodName,
    string Namespace,
    string Status,
    double CpuUsage,
    double MemoryUsage,
    int RestartCount,
    DateTime StartTime,
    string NodeName
);

/// <summary>
/// Service mesh metrics
/// </summary>
public record ServiceMeshMetrics(
    int TotalServices,
    double AverageLatency,
    double ErrorRate,
    double ThroughputRPS,
    Dictionary<string, double> ServiceLatencies
);

/// <summary>
/// Volume metrics
/// </summary>
public record VolumeMetrics(
    string VolumeName,
    string StorageClass,
    long TotalBytes,
    long UsedBytes,
    double UsagePercentage,
    string Status
);

/// <summary>
/// Network policy metrics
/// </summary>
public record NetworkPolicyMetrics(
    int TotalPolicies,
    int ActivePolicies,
    long AllowedConnections,
    long BlockedConnections,
    double BlockedPercentage
);

/// <summary>
/// HPA status
/// </summary>
public record HPAStatus(
    string Name,
    string Namespace,
    int CurrentReplicas,
    int DesiredReplicas,
    int MinReplicas,
    int MaxReplicas,
    double TargetCpuUtilization,
    double CurrentCpuUtilization
);

/// <summary>
/// Cluster event
/// </summary>
public record ClusterEvent(
    DateTime Timestamp,
    string Type,
    string Reason,
    string Message,
    string ObjectKind,
    string ObjectName,
    string Namespace
);

/// <summary>
/// Cluster health check result
/// </summary>
public record ClusterHealthCheckResult(
    bool IsHealthy,
    Dictionary<string, bool> ComponentHealth,
    List<string> Recommendations,
    DateTime CheckTime
);

/// <summary>
/// Resource quota status
/// </summary>
public record ResourceQuotaStatus(
    string Name,
    string Namespace,
    Dictionary<string, ResourceQuotaUsage> Resources
);

/// <summary>
/// Resource quota usage
/// </summary>
public record ResourceQuotaUsage(
    string Resource,
    string Used,
    string Hard,
    double UsagePercentage
);

// Supporting data structures for Database Performance

// DatabasePerformanceMetrics moved to IDatabaseProvider.cs to avoid duplication

/// <summary>
/// Slow query analysis
/// </summary>
public record SlowQueryAnalysis(
    string QueryHash,
    string QueryText,
    double AverageExecutionTime,
    double MaxExecutionTime,
    int ExecutionCount,
    double TotalTime,
    DateTime LastSeen
);

/// <summary>
/// Connection pool metrics
/// </summary>
public record ConnectionPoolMetrics(
    int TotalConnections,
    int ActiveConnections,
    int IdleConnections,
    int WaitingConnections,
    double AverageWaitTime,
    int MaxConnections
);

/// <summary>
/// Database size metrics
/// </summary>
public record DatabaseSizeMetrics(
    long TotalSizeBytes,
    long DataSizeBytes,
    long IndexSizeBytes,
    long LogSizeBytes,
    double GrowthRatePerDay,
    Dictionary<string, long> TableSizes
);

/// <summary>
/// Index usage statistics
/// </summary>
public record IndexUsageStats(
    string SchemaName,
    string TableName,
    string IndexName,
    long IndexScans,
    long TupleReads,
    long TupleFetches,
    long IndexSize,
    double EfficiencyRatio
);

/// <summary>
/// Lock contention analysis
/// </summary>
public record LockContentionAnalysis(
    int TotalLocks,
    int BlockedQueries,
    double AverageWaitTime,
    double MaxWaitTime,
    List<LockContention> TopContentions
);

/// <summary>
/// Lock contention detail
/// </summary>
public record LockContention(
    string LockType,
    string ObjectName,
    int WaitingQueries,
    double AverageWaitTime,
    string BlockingQuery
);

/// <summary>
/// Replication metrics
/// </summary>
public record ReplicationMetrics(
    int ReplicaCount,
    double AverageReplicationLag,
    double MaxReplicationLag,
    bool AllReplicasHealthy,
    Dictionary<string, double> ReplicaLags
);

/// <summary>
/// Maintenance statistics
/// </summary>
public record MaintenanceStats(
    DateTime LastVacuum,
    DateTime LastAnalyze,
    DateTime LastReindex,
    int DeadTuples,
    double DatabaseBloat,
    List<string> MaintenanceRecommendations
);

/// <summary>
/// Query plan analysis
/// </summary>
public record QueryPlanAnalysis(
    string Query,
    double EstimatedCost,
    double ActualTime,
    string ExecutionPlan,
    List<string> Recommendations,
    Dictionary<string, object> PlanMetrics
);

/// <summary>
/// Performance recommendation
/// </summary>
public record PerformanceRecommendation(
    string Category,
    string Title,
    string Description,
    string Severity,
    double PotentialImprovement,
    string ActionRequired
);

// Supporting data structures for Network Monitoring

/// <summary>
/// Service communication metrics
/// </summary>
public record ServiceCommunicationMetrics(
    string SourceService,
    string DestinationService,
    long RequestCount,
    double AverageLatency,
    double ErrorRate,
    double ThroughputRPS
);

/// <summary>
/// Network latency metrics
/// </summary>
public record NetworkLatencyMetrics(
    double AverageLatency,
    double P50Latency,
    double P95Latency,
    double P99Latency,
    Dictionary<string, double> ServiceLatencies
);

/// <summary>
/// Bandwidth utilization metrics
/// </summary>
public record BandwidthUtilizationMetrics(
    double InboundBandwidthMbps,
    double OutboundBandwidthMbps,
    double PeakBandwidthMbps,
    double UtilizationPercentage,
    Dictionary<string, double> ServiceBandwidth
);

/// <summary>
/// Load balancer metrics
/// </summary>
public record LoadBalancerMetrics(
    long TotalRequests,
    double RequestsPerSecond,
    double AverageResponseTime,
    double ErrorRate,
    Dictionary<string, int> BackendHealthy,
    Dictionary<string, double> BackendLatency
);

/// <summary>
/// DNS metrics
/// </summary>
public record DNSMetrics(
    long TotalQueries,
    double QueriesPerSecond,
    double AverageResponseTime,
    double CacheHitRate,
    int FailedQueries,
    Dictionary<string, int> QueryTypes
);

/// <summary>
/// Certificate status
/// </summary>
public record CertificateStatus(
    string Domain,
    DateTime ExpiryDate,
    int DaysUntilExpiry,
    bool IsValid,
    string Issuer,
    List<string> Warnings
);

/// <summary>
/// Ingress controller metrics
/// </summary>
public record IngressControllerMetrics(
    long TotalRequests,
    double RequestsPerSecond,
    double AverageResponseTime,
    Dictionary<int, int> StatusCodeCounts,
    Dictionary<string, double> RouteLatencies
);

/// <summary>
/// CDN metrics
/// </summary>
public record CDNMetrics(
    long TotalRequests,
    double CacheHitRate,
    double AverageOriginLatency,
    double DataTransferGB,
    Dictionary<string, long> EdgeLocationRequests,
    Dictionary<int, int> StatusCodeCounts
);

/// <summary>
/// Network path trace
/// </summary>
public record NetworkPathTrace(
    string Source,
    string Destination,
    List<NetworkHop> Hops,
    double TotalLatency,
    bool Successful
);

/// <summary>
/// Network hop information
/// </summary>
public record NetworkHop(
    int HopNumber,
    string Address,
    double Latency,
    bool Responsive
);

/// <summary>
/// Security group metrics
/// </summary>
public record SecurityGroupMetrics(
    int TotalRules,
    long AllowedConnections,
    long BlockedConnections,
    double BlockedPercentage,
    List<string> UnusedRules
);

// Supporting data structures for Cost Monitoring

/// <summary>
/// Cost breakdown
/// </summary>
public record CostBreakdown(
    decimal TotalCost,
    string Currency,
    DateTime Period,
    Dictionary<string, decimal> ServiceCosts,
    Dictionary<string, decimal> ResourceTypeCosts,
    Dictionary<string, decimal> RegionCosts
);

/// <summary>
/// Cost trends
/// </summary>
public record CostTrends(
    TimeRange TimeRange,
    List<DailyCost> DailyCosts,
    decimal TotalCost,
    decimal AverageDailyCost,
    double GrowthRate,
    TrendDirection Trend
);

/// <summary>
/// Daily cost
/// </summary>
public record DailyCost(
    DateTime Date,
    decimal Cost,
    Dictionary<string, decimal> ServiceBreakdown
);

/// <summary>
/// Resource efficiency metrics
/// </summary>
public record ResourceEfficiencyMetrics(
    double CpuEfficiency,
    double MemoryEfficiency,
    double StorageEfficiency,
    decimal WastedCost,
    List<string> UnderutilizedResources
);

/// <summary>
/// Cost optimization recommendation
/// </summary>
public record CostOptimizationRecommendation(
    string Category,
    string Title,
    string Description,
    decimal PotentialSavings,
    string Priority,
    string ActionRequired,
    double ImplementationEffort
);

/// <summary>
/// Budget status
/// </summary>
public record BudgetStatus(
    decimal BudgetAmount,
    decimal SpentAmount,
    decimal RemainingAmount,
    double UtilizationPercentage,
    decimal ForecastedMonthEnd,
    bool IsOverBudget,
    List<string> Alerts
);

/// <summary>
/// Rightsizing recommendation
/// </summary>
public record RightsizingRecommendation(
    string ResourceId,
    string CurrentSize,
    string RecommendedSize,
    decimal MonthlySavings,
    double UtilizationImprovement,
    string Confidence
);

/// <summary>
/// Reserved capacity recommendation
/// </summary>
public record ReservedCapacityRecommendation(
    string ResourceType,
    string RecommendedTerm,
    int RecommendedQuantity,
    decimal MonthlySavings,
    double PaybackMonths
);

/// <summary>
/// Cost allocation
/// </summary>
public record CostAllocation(
    string GroupByField,
    Dictionary<string, decimal> Allocations,
    DateTime Period,
    decimal TotalCost
);

/// <summary>
/// Cost forecast
/// </summary>
public record CostForecast(
    int ForecastDays,
    decimal ForecastedCost,
    decimal ConfidenceInterval,
    List<DailyCostForecast> DailyForecasts
);

/// <summary>
/// Daily cost forecast
/// </summary>
public record DailyCostForecast(
    DateTime Date,
    decimal ForecastedCost,
    decimal LowerBound,
    decimal UpperBound
);

/// <summary>
/// Cost anomaly
/// </summary>
public record CostAnomaly(
    DateTime Date,
    string Service,
    decimal ActualCost,
    decimal ExpectedCost,
    decimal Variance,
    double VariancePercentage,
    string Severity,
    string? PossibleCause
);