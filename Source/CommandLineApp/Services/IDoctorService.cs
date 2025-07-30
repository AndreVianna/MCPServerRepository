namespace MCPHub.CommandLineApp.Services;

/// <summary>
/// Service for comprehensive system health checking and package integrity verification
/// </summary>
public interface IDoctorService {
    /// <summary>
    /// Performs a comprehensive system health check
    /// </summary>
    /// <param name="includePackageChecks">Whether to check individual packages</param>
    /// <param name="includeSecurityScan">Whether to perform security vulnerability scanning</param>
    /// <param name="includePerformanceAnalysis">Whether to analyze performance metrics</param>
    /// <param name="autoFix">Whether to automatically attempt to fix found issues</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Comprehensive health check result</returns>
    Task<SystemHealthCheckResult> PerformHealthCheckAsync(
        bool includePackageChecks = true,
        bool includeSecurityScan = true,
        bool includePerformanceAnalysis = false,
        bool autoFix = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies the integrity of installed packages
    /// </summary>
    /// <param name="packageName">Specific package to verify (null for all packages)</param>
    /// <param name="global">Whether to check global packages</param>
    /// <param name="deep">Whether to perform deep integrity checks</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Package integrity verification result</returns>
    Task<PackageIntegrityResult> VerifyPackageIntegrityAsync(
        string? packageName = null,
        bool global = false,
        bool deep = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates dependency consistency across all installed packages
    /// </summary>
    /// <param name="global">Whether to check global dependencies</param>
    /// <param name="autoResolve">Whether to automatically resolve conflicts</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Dependency validation result</returns>
    Task<DependencyConsistencyResult> ValidateDependencyConsistencyAsync(
        bool global = false,
        bool autoResolve = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks and optimizes cache health
    /// </summary>
    /// <param name="clearCorrupted">Whether to clear corrupted cache entries</param>
    /// <param name="compactCache">Whether to compact cache files</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Cache health check result</returns>
    Task<CacheHealthResult> CheckCacheHealthAsync(
        bool clearCorrupted = true,
        bool compactCache = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates and suggests optimizations for configuration
    /// </summary>
    /// <param name="checkPermissions">Whether to check file permissions</param>
    /// <param name="validatePaths">Whether to validate configured paths</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Configuration validation result</returns>
    Task<ConfigurationValidationResult> ValidateConfigurationAsync(
        bool checkPermissions = true,
        bool validatePaths = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs security audit of installed packages
    /// </summary>
    /// <param name="scanVulnerabilities">Whether to scan for known vulnerabilities</param>
    /// <param name="checkTrustTiers">Whether to validate trust tiers</param>
    /// <param name="analyzePermissions">Whether to analyze package permissions</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Security audit result</returns>
    Task<SecurityAuditResult> PerformSecurityAuditAsync(
        bool scanVulnerabilities = true,
        bool checkTrustTiers = true,
        bool analyzePermissions = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Analyzes system performance and provides optimization recommendations
    /// </summary>
    /// <param name="measureStartupTime">Whether to measure CLI startup performance</param>
    /// <param name="analyzeDiskUsage">Whether to analyze disk usage patterns</param>
    /// <param name="checkNetworkLatency">Whether to check network connectivity performance</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Performance analysis result</returns>
    Task<PerformanceAnalysisResult> AnalyzePerformanceAsync(
        bool measureStartupTime = true,
        bool analyzeDiskUsage = true,
        bool checkNetworkLatency = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Attempts to repair identified issues automatically
    /// </summary>
    /// <param name="issues">List of issues to repair</param>
    /// <param name="createBackup">Whether to create backup before repairs</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Repair operation result</returns>
    Task<RepairResult> RepairIssuesAsync(
        IEnumerable<SystemIssue> issues,
        bool createBackup = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets system environment information for diagnostics
    /// </summary>
    /// <param name="includeSystemInfo">Whether to include system information</param>
    /// <param name="includeEnvironmentVars">Whether to include relevant environment variables</param>
    /// <param name="includePaths">Whether to include path information</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>System environment information</returns>
    Task<SystemEnvironmentInfo> GetSystemEnvironmentAsync(
        bool includeSystemInfo = true,
        bool includeEnvironmentVars = false,
        bool includePaths = true,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Comprehensive system health check result
/// </summary>
public record SystemHealthCheckResult {
    public bool IsHealthy { get; init; }
    public HealthScore OverallScore { get; init; }
    public List<HealthCheckCategory> Categories { get; init; } = [];
    public List<SystemIssue> Issues { get; init; } = [];
    public List<string> Recommendations { get; init; } = [];
    public SystemEnvironmentInfo Environment { get; init; } = new();
    public TimeSpan CheckDuration { get; init; }
}

/// <summary>
/// Health check category result
/// </summary>
public record HealthCheckCategory {
    public string Name { get; init; } = string.Empty;
    public HealthScore Score { get; init; }
    public bool IsCritical { get; init; }
    public List<SystemIssue> Issues { get; init; } = [];
    public List<string> Recommendations { get; init; } = [];
    public string Status { get; init; } = string.Empty;
    public TimeSpan CheckDuration { get; init; }
}

/// <summary>
/// Health score enumeration
/// </summary>
public enum HealthScore {
    Excellent,
    Good,
    Fair,
    Poor,
    Critical
}

/// <summary>
/// System issue information
/// </summary>
public record SystemIssue {
    public string Id { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public IssueSeverity Severity { get; init; }
    public string Category { get; init; } = string.Empty;
    public bool CanAutoFix { get; init; }
    public string? FixDescription { get; init; }
    public string? FixCommand { get; init; }
    public List<string> AffectedComponents { get; init; } = [];
}

/// <summary>
/// Issue severity levels
/// </summary>
public enum IssueSeverity {
    Info,
    Warning,
    Error,
    Critical
}

/// <summary>
/// Package integrity verification result
/// </summary>
public record PackageIntegrityResult {
    public bool IsValid { get; init; }
    public int TotalPackagesChecked { get; init; }
    public int ValidPackages { get; init; }
    public int CorruptedPackages { get; init; }
    public List<PackageIntegrityIssue> Issues { get; init; } = [];
    public TimeSpan CheckDuration { get; init; }
}

/// <summary>
/// Package integrity issue
/// </summary>
public record PackageIntegrityIssue {
    public string PackageName { get; init; } = string.Empty;
    public string Version { get; init; } = string.Empty;
    public string IssueType { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public bool CanRepair { get; init; }
    public string? RepairAction { get; init; }
}

/// <summary>
/// Dependency consistency validation result
/// </summary>
public record DependencyConsistencyResult {
    public bool IsConsistent { get; init; }
    public List<DependencyConflict> Conflicts { get; init; } = [];
    public List<string> Warnings { get; init; } = [];
    public List<string> Suggestions { get; init; } = [];
    public bool AutoResolved { get; init; }
    public List<string> ResolvedConflicts { get; init; } = [];
}

/// <summary>
/// Dependency conflict information
/// </summary>
public record DependencyConflict {
    public string DependencyName { get; init; } = string.Empty;
    public string RequiredByPackage { get; init; } = string.Empty;
    public string RequiredVersion { get; init; } = string.Empty;
    public string ActualVersion { get; init; } = string.Empty;
    public ConflictSeverity Severity { get; init; }
    public string Resolution { get; init; } = string.Empty;
}

/// <summary>
/// Conflict severity levels
/// </summary>
public enum ConflictSeverity {
    Low,
    Medium,
    High,
    Critical
}

/// <summary>
/// Cache health check result
/// </summary>
public record CacheHealthResult {
    public bool IsHealthy { get; init; }
    public long TotalCacheSize { get; init; } // Bytes
    public long CorruptedCacheSize { get; init; } // Bytes
    public int TotalEntries { get; init; }
    public int CorruptedEntries { get; init; }
    public int ClearedEntries { get; init; }
    public List<string> Issues { get; init; } = [];
    public List<string> Optimizations { get; init; } = [];
    public bool WasOptimized { get; init; }
    public long SpaceFreed { get; init; } // Bytes
}

/// <summary>
/// Configuration validation result
/// </summary>
public record ConfigurationValidationResult {
    public bool IsValid { get; init; }
    public List<ConfigurationIssue> Issues { get; init; } = [];
    public List<string> Recommendations { get; init; } = [];
    public List<string> OptimizationSuggestions { get; init; } = [];
    public bool HasPermissionIssues { get; init; }
    public bool HasPathIssues { get; init; }
}

/// <summary>
/// Configuration issue
/// </summary>
public record ConfigurationIssue {
    public string ConfigKey { get; init; } = string.Empty;
    public string IssueType { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public IssueSeverity Severity { get; init; }
    public string? SuggestedValue { get; init; }
    public string? FixCommand { get; init; }
}

/// <summary>
/// Security audit result
/// </summary>
public record SecurityAuditResult {
    public bool IsSecure { get; init; }
    public HealthScore SecurityScore { get; init; }
    public int TotalPackagesScanned { get; init; }
    public int VulnerablePackages { get; init; }
    public int CriticalVulnerabilities { get; init; }
    public int HighVulnerabilities { get; init; }
    public int MediumVulnerabilities { get; init; }
    public int LowVulnerabilities { get; init; }
    public List<SecurityIssue> Issues { get; init; } = [];
    public List<string> Recommendations { get; init; } = [];
}

/// <summary>
/// Security issue information
/// </summary>
public record SecurityIssue {
    public string PackageName { get; init; } = string.Empty;
    public string Version { get; init; } = string.Empty;
    public string VulnerabilityId { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public SecuritySeverity Severity { get; init; }
    public string? CvssScore { get; init; }
    public string? FixVersion { get; init; }
    public List<string> References { get; init; } = [];
}

/// <summary>
/// Security severity levels
/// </summary>
public enum SecuritySeverity {
    Low,
    Medium,
    High,
    Critical
}

/// <summary>
/// Performance analysis result
/// </summary>
public record PerformanceAnalysisResult {
    public HealthScore PerformanceScore { get; init; }
    public TimeSpan StartupTime { get; init; }
    public DiskUsageAnalysis DiskUsage { get; init; } = new();
    public NetworkLatencyAnalysis NetworkLatency { get; init; } = new();
    public List<string> Optimizations { get; init; } = [];
    public List<string> Warnings { get; init; } = [];
}

/// <summary>
/// Disk usage analysis
/// </summary>
public record DiskUsageAnalysis {
    public long TotalPackageSize { get; init; } // Bytes
    public long CacheSize { get; init; } // Bytes
    public long TempSize { get; init; } // Bytes
    public int PackageCount { get; init; }
    public string LargestPackage { get; init; } = string.Empty;
    public long LargestPackageSize { get; init; } // Bytes
    public List<string> Recommendations { get; init; } = [];
}

/// <summary>
/// Network latency analysis
/// </summary>
public record NetworkLatencyAnalysis {
    public bool IsReachable { get; init; }
    public TimeSpan RegistryLatency { get; init; }
    public TimeSpan DownloadSpeed { get; init; } // Time per MB
    public string ConnectionQuality { get; init; } = string.Empty;
    public List<string> Issues { get; init; } = [];
}

/// <summary>
/// Repair operation result
/// </summary>
public record RepairResult {
    public bool Success { get; init; }
    public int TotalIssues { get; init; }
    public int RepairedIssues { get; init; }
    public int SkippedIssues { get; init; }
    public int FailedIssues { get; init; }
    public List<RepairAction> Actions { get; init; } = [];
    public string? BackupPath { get; init; }
    public TimeSpan RepairDuration { get; init; }
}

/// <summary>
/// Individual repair action
/// </summary>
public record RepairAction {
    public string IssueId { get; init; } = string.Empty;
    public string Action { get; init; } = string.Empty;
    public bool Success { get; init; }
    public string? Error { get; init; }
    public TimeSpan Duration { get; init; }
}

/// <summary>
/// System environment information
/// </summary>
public record SystemEnvironmentInfo {
    public string OperatingSystem { get; init; } = string.Empty;
    public string Architecture { get; init; } = string.Empty;
    public string DotNetVersion { get; init; } = string.Empty;
    public string CliVersion { get; init; } = string.Empty;
    public string ConfigPath { get; init; } = string.Empty;
    public string CachePath { get; init; } = string.Empty;
    public string PackagesPath { get; init; } = string.Empty;
    public string TempPath { get; init; } = string.Empty;
    public Dictionary<string, string> EnvironmentVariables { get; init; } = [];
    public Dictionary<string, string> SystemInfo { get; init; } = [];
}