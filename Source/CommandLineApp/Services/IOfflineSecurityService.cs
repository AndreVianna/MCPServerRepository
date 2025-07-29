using MCPHub.Domain.ValueObjects;

namespace MCPHub.CommandLineApp.Services;

/// <summary>
/// Interface for offline security operations that don't require API connectivity
/// </summary>
public interface IOfflineSecurityService {
    /// <summary>
    /// Verifies package signature and integrity using local certificate store
    /// </summary>
    /// <param name="packagePath">Path to the package file</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Signature verification result</returns>
    Task<PackageSignatureVerificationResult> VerifyPackageSignatureAsync(
        string packagePath,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Calculates and verifies package checksums
    /// </summary>
    /// <param name="packagePath">Path to the package file</param>
    /// <param name="expectedChecksum">Expected checksum (optional)</param>
    /// <param name="hashAlgorithm">Hash algorithm to use (SHA256, SHA512, etc.)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Checksum verification result</returns>
    Task<PackageChecksumVerificationResult> VerifyPackageChecksumAsync(
        string packagePath,
        string? expectedChecksum = null,
        string hashAlgorithm = "SHA256",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Scans package using local vulnerability database
    /// </summary>
    /// <param name="packageName">Package name</param>
    /// <param name="version">Package version</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Offline vulnerability scan result</returns>
    Task<OfflineVulnerabilityScanResult> ScanPackageOfflineAsync(
        string packageName,
        string version,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates local vulnerability database from multiple sources
    /// </summary>
    /// <param name="sources">Data sources to update from</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Database update result</returns>
    Task<VulnerabilityDatabaseUpdateResult> UpdateVulnerabilityDatabaseAsync(
        IEnumerable<VulnerabilityDataSource>? sources = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets local vulnerability database status and statistics
    /// </summary>
    /// <returns>Database status information</returns>
    Task<LocalVulnerabilityDatabaseStatus> GetVulnerabilityDatabaseStatusAsync();

    /// <summary>
    /// Validates package manifest for security issues
    /// </summary>
    /// <param name="manifestPath">Path to package manifest file</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Manifest security validation result</returns>
    Task<ManifestSecurityValidationResult> ValidateManifestSecurityAsync(
        string manifestPath,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs static analysis on package contents
    /// </summary>
    /// <param name="packagePath">Path to extracted package directory</param>
    /// <param name="analysisOptions">Analysis configuration options</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Static analysis result</returns>
    Task<StaticAnalysisResult> PerformStaticAnalysisAsync(
        string packagePath,
        StaticAnalysisOptions? analysisOptions = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates offline security report for a package
    /// </summary>
    /// <param name="packagePath">Path to package file or directory</param>
    /// <param name="reportOptions">Report generation options</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Offline security report</returns>
    Task<OfflineSecurityReport> GenerateOfflineSecurityReportAsync(
        string packagePath,
        OfflineReportOptions? reportOptions = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if offline security operations are available
    /// </summary>
    /// <returns>Availability status</returns>
    Task<OfflineSecurityCapabilities> GetOfflineCapabilitiesAsync();
}

/// <summary>
/// Result of package signature verification
/// </summary>
public record PackageSignatureVerificationResult {
    public bool IsValid { get; init; }
    public bool IsSigned { get; init; }
    public string? SignerName { get; init; }
    public string? SignerEmail { get; init; }
    public DateTimeOffset? SignedAt { get; init; }
    public string? CertificateThumbprint { get; init; }
    public bool IsCertificateTrusted { get; init; }
    public IEnumerable<string> ValidationErrors { get; init; } = Enumerable.Empty<string>();
    public IEnumerable<string> Warnings { get; init; } = Enumerable.Empty<string>();
}

/// <summary>
/// Result of package checksum verification
/// </summary>
public record PackageChecksumVerificationResult {
    public bool IsValid { get; init; }
    public string CalculatedChecksum { get; init; } = string.Empty;
    public string? ExpectedChecksum { get; init; }
    public string HashAlgorithm { get; init; } = string.Empty;
    public long FileSizeBytes { get; init; }
    public TimeSpan CalculationTime { get; init; }
    public IEnumerable<string> Issues { get; init; } = Enumerable.Empty<string>();
}

/// <summary>
/// Result of offline vulnerability scanning
/// </summary>
public record OfflineVulnerabilityScanResult {
    public string PackageName { get; init; } = string.Empty;
    public string Version { get; init; } = string.Empty;
    public int VulnerabilityCount { get; init; }
    public SecurityScanSeverity HighestSeverity { get; init; }
    public IEnumerable<OfflineVulnerability> Vulnerabilities { get; init; } = Enumerable.Empty<OfflineVulnerability>();
    public DateTimeOffset ScannedAt { get; init; }
    public string DatabaseVersion { get; init; } = string.Empty;
    public bool IsUpToDate { get; init; }
}

/// <summary>
/// Offline vulnerability information
/// </summary>
public record OfflineVulnerability {
    public string Id { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public SecurityScanSeverity Severity { get; init; }
    public string? CveId { get; init; }
    public string AffectedVersions { get; init; } = string.Empty;
    public string? FixedInVersion { get; init; }
    public DateTimeOffset PublishedAt { get; init; }
    public IEnumerable<string> References { get; init; } = Enumerable.Empty<string>();
    public VulnerabilitySource Source { get; init; }
}

/// <summary>
/// Vulnerability data source configuration
/// </summary>
public record VulnerabilityDataSource {
    public string Name { get; init; } = string.Empty;
    public string Url { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty; // "nvd", "osv", "github", "custom"
    public bool IsEnabled { get; init; } = true;
    public TimeSpan UpdateInterval { get; init; } = TimeSpan.FromHours(24);
    public Dictionary<string, string> Configuration { get; init; } = new();
}

/// <summary>
/// Result of vulnerability database update
/// </summary>
public record VulnerabilityDatabaseUpdateResult {
    public bool Success { get; init; }
    public int UpdatedRecords { get; init; }
    public int NewRecords { get; init; }
    public int RemovedRecords { get; init; }
    public DateTimeOffset UpdatedAt { get; init; }
    public IEnumerable<VulnerabilityDataSource> UpdatedSources { get; init; } = Enumerable.Empty<VulnerabilityDataSource>();
    public IEnumerable<string> Errors { get; init; } = Enumerable.Empty<string>();
    public IEnumerable<string> Warnings { get; init; } = Enumerable.Empty<string>();
}

/// <summary>
/// Local vulnerability database status
/// </summary>
public record LocalVulnerabilityDatabaseStatus {
    public bool IsAvailable { get; init; }
    public int TotalVulnerabilities { get; init; }
    public DateTimeOffset? LastUpdate { get; init; }
    public TimeSpan? Age { get; init; }
    public bool IsOutdated { get; init; }
    public long DatabaseSizeBytes { get; init; }
    public IEnumerable<VulnerabilitySourceStatus> Sources { get; init; } = Enumerable.Empty<VulnerabilitySourceStatus>();
    public string DatabaseVersion { get; init; } = string.Empty;
    public string DatabasePath { get; init; } = string.Empty;
}

/// <summary>
/// Status of individual vulnerability data source
/// </summary>
public record VulnerabilitySourceStatus {
    public string Name { get; init; } = string.Empty;
    public int RecordCount { get; init; }
    public DateTimeOffset? LastUpdate { get; init; }
    public bool IsHealthy { get; init; }
    public string? LastError { get; init; }
}

/// <summary>
/// Result of manifest security validation
/// </summary>
public record ManifestSecurityValidationResult {
    public bool IsValid { get; init; }
    public int IssueCount { get; init; }
    public IEnumerable<ManifestSecurityIssue> Issues { get; init; } = Enumerable.Empty<ManifestSecurityIssue>();
    public IEnumerable<string> Recommendations { get; init; } = Enumerable.Empty<string>();
    public string ManifestVersion { get; init; } = string.Empty;
}

/// <summary>
/// Security issue found in package manifest
/// </summary>
public record ManifestSecurityIssue {
    public string Type { get; init; } = string.Empty;
    public SecurityScanSeverity Severity { get; init; }
    public string Description { get; init; } = string.Empty;
    public string? Field { get; init; }
    public string? Recommendation { get; init; }
}

/// <summary>
/// Static analysis configuration options
/// </summary>
public record StaticAnalysisOptions {
    public bool AnalyzeJavaScript { get; init; } = true;
    public bool AnalyzePython { get; init; } = true;
    public bool AnalyzeShellScripts { get; init; } = true;
    public bool CheckForSecrets { get; init; } = true;
    public bool CheckForVulnerablePatterns { get; init; } = true;
    public bool CheckDependencies { get; init; } = true;
    public int MaxFileSizeBytes { get; init; } = 10 * 1024 * 1024; // 10MB
    public IEnumerable<string> ExcludePatterns { get; init; } = Enumerable.Empty<string>();
}

/// <summary>
/// Result of static code analysis
/// </summary>
public record StaticAnalysisResult {
    public bool HasIssues { get; init; }
    public int TotalIssues { get; init; }
    public int CriticalIssues { get; init; }
    public int HighIssues { get; init; }
    public int MediumIssues { get; init; }
    public int LowIssues { get; init; }
    public IEnumerable<StaticAnalysisIssue> Issues { get; init; } = Enumerable.Empty<StaticAnalysisIssue>();
    public int FilesAnalyzed { get; init; }
    public int LinesOfCode { get; init; }
    public TimeSpan AnalysisDuration { get; init; }
}

/// <summary>
/// Issue found during static analysis
/// </summary>
public record StaticAnalysisIssue {
    public string Type { get; init; } = string.Empty;
    public SecurityScanSeverity Severity { get; init; }
    public string Description { get; init; } = string.Empty;
    public string FilePath { get; init; } = string.Empty;
    public int LineNumber { get; init; }
    public int ColumnNumber { get; init; }
    public string? CodeSnippet { get; init; }
    public string? Recommendation { get; init; }
    public string RuleId { get; init; } = string.Empty;
}

/// <summary>
/// Comprehensive offline security report
/// </summary>
public record OfflineSecurityReport {
    public string PackageName { get; init; } = string.Empty;
    public string Version { get; init; } = string.Empty;
    public DateTimeOffset GeneratedAt { get; init; }
    public PackageSignatureVerificationResult SignatureVerification { get; init; } = new();
    public PackageChecksumVerificationResult ChecksumVerification { get; init; } = new();
    public OfflineVulnerabilityScanResult VulnerabilityScan { get; init; } = new();
    public ManifestSecurityValidationResult ManifestValidation { get; init; } = new();
    public StaticAnalysisResult StaticAnalysis { get; init; } = new();
    public OfflineSecurityScore SecurityScore { get; init; } = new();
    public IEnumerable<string> Recommendations { get; init; } = Enumerable.Empty<string>();
}

/// <summary>
/// Offline security score calculation
/// </summary>
public record OfflineSecurityScore {
    public int TotalScore { get; init; }
    public int MaxScore { get; init; }
    public decimal Percentage { get; init; }
    public string Grade { get; init; } = string.Empty;
    public Dictionary<string, int> CategoryScores { get; init; } = new();
}

/// <summary>
/// Options for offline report generation
/// </summary>
public record OfflineReportOptions {
    public bool IncludeSignatureVerification { get; init; } = true;
    public bool IncludeChecksumVerification { get; init; } = true;
    public bool IncludeVulnerabilityScanning { get; init; } = true;
    public bool IncludeManifestValidation { get; init; } = true;
    public bool IncludeStaticAnalysis { get; init; } = true;
    public bool IncludeDetailedIssues { get; init; } = true;
    public string ReportFormat { get; init; } = "json"; // json, xml, html
}

/// <summary>
/// Available offline security capabilities
/// </summary>
public record OfflineSecurityCapabilities {
    public bool SignatureVerificationAvailable { get; init; }
    public bool ChecksumVerificationAvailable { get; init; }
    public bool VulnerabilityDatabaseAvailable { get; init; }
    public bool StaticAnalysisAvailable { get; init; }
    public bool ManifestValidationAvailable { get; init; }
    public IEnumerable<string> SupportedHashAlgorithms { get; init; } = Enumerable.Empty<string>();
    public IEnumerable<string> SupportedFileTypes { get; init; } = Enumerable.Empty<string>();
    public string Version { get; init; } = string.Empty;
}

/// <summary>
/// Source of vulnerability information
/// </summary>
public enum VulnerabilitySource {
    NVD,           // National Vulnerability Database
    OSV,           // Open Source Vulnerabilities
    GitHub,        // GitHub Security Advisories
    Snyk,          // Snyk vulnerability database
    NPM,           // NPM security advisories
    Custom,        // Custom vulnerability feeds
    Unknown
}