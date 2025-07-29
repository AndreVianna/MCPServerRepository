using MCPHub.Domain.Contracts.Responses;
using MCPHub.Domain.Entities;
using MCPHub.Domain.ValueObjects;

namespace MCPHub.CommandLineApp.Services;

/// <summary>
/// CLI service interface for security operations and scanning
/// </summary>
public interface ISecurityService {
    /// <summary>
    /// Scans a package for security vulnerabilities
    /// </summary>
    /// <param name="packageName">Package name</param>
    /// <param name="version">Package version</param>
    /// <param name="scanType">Type of security scan to perform</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Security scan result</returns>
    Task<SecurityScanResult> ScanPackageAsync(
        string packageName,
        string version,
        ScanType scanType,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the latest security scan results for a package
    /// </summary>
    /// <param name="packageName">Package name</param>
    /// <param name="version">Package version</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Security scan result or null if none found</returns>
    Task<SecurityScanResult?> GetLatestScanResultAsync(
        string packageName,
        string version,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets comprehensive security summary for a package
    /// </summary>
    /// <param name="packageName">Package name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Security scan summary</returns>
    Task<SecurityScanSummary> GetSecuritySummaryAsync(
        string packageName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates package against security policies
    /// </summary>
    /// <param name="packageName">Package name</param>
    /// <param name="version">Package version</param>
    /// <param name="policy">Security policy to validate against</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Validation result</returns>
    Task<SecurityPolicyValidationResult> ValidateSecurityPolicyAsync(
        string packageName,
        string version,
        SecurityPolicy? policy = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Calculates security grade for a package
    /// </summary>
    /// <param name="packageName">Package name</param>
    /// <param name="version">Package version</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Security grade (A+ to F)</returns>
    Task<string> CalculateSecurityGradeAsync(
        string packageName,
        string version,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies package signatures and checksums
    /// </summary>
    /// <param name="packagePath">Path to package file</param>
    /// <param name="expectedChecksum">Expected checksum (optional)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Verification result</returns>
    Task<PackageVerificationResult> VerifyPackageIntegrityAsync(
        string packagePath,
        string? expectedChecksum = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets security advisories for a package
    /// </summary>
    /// <param name="packageName">Package name</param>
    /// <param name="version">Package version (optional)</param>
    /// <param name="severity">Minimum severity level</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of security advisories</returns>
    Task<IEnumerable<SecurityAdvisory>> GetSecurityAdvisoriesAsync(
        string packageName,
        string? version = null,
        SecurityScanSeverity? severity = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates local security database
    /// </summary>
    /// <param name="forceUpdate">Force update even if recently updated</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Update result</returns>
    Task<SecurityDatabaseUpdateResult> UpdateSecurityDatabaseAsync(
        bool forceUpdate = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if security database is available and up to date
    /// </summary>
    /// <returns>Database status</returns>
    Task<SecurityDatabaseStatus> GetSecurityDatabaseStatusAsync();
}

/// <summary>
/// Result of security policy validation
/// </summary>
public record SecurityPolicyValidationResult {
    public bool IsValid { get; init; }
    public string SecurityGrade { get; init; } = string.Empty;
    public IEnumerable<string> Violations { get; init; } = Enumerable.Empty<string>();
    public IEnumerable<string> Warnings { get; init; } = Enumerable.Empty<string>();
    public SecurityPolicy AppliedPolicy { get; init; } = new("Default", "Default security policy");
}

/// <summary>
/// Result of package integrity verification
/// </summary>
public record PackageVerificationResult {
    public bool IsValid { get; init; }
    public bool SignatureValid { get; init; }
    public bool ChecksumValid { get; init; }
    public string CalculatedChecksum { get; init; } = string.Empty;
    public string? ExpectedChecksum { get; init; }
    public IEnumerable<string> Issues { get; init; } = Enumerable.Empty<string>();
}

/// <summary>
/// Security advisory information
/// </summary>
public record SecurityAdvisory {
    public string Id { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public SecurityScanSeverity Severity { get; init; }
    public string? CveId { get; init; }
    public string AffectedVersions { get; init; } = string.Empty;
    public string? PatchedVersion { get; init; }
    public DateTimeOffset PublishedAt { get; init; }
    public string Source { get; init; } = string.Empty;
    public IEnumerable<string> References { get; init; } = Enumerable.Empty<string>();
}

/// <summary>
/// Result of security database update
/// </summary>
public record SecurityDatabaseUpdateResult {
    public bool Success { get; init; }
    public int UpdatedRecords { get; init; }
    public DateTimeOffset LastUpdate { get; init; }
    public IEnumerable<string> Errors { get; init; } = Enumerable.Empty<string>();
}

/// <summary>
/// Status of local security database
/// </summary>
public record SecurityDatabaseStatus {
    public bool IsAvailable { get; init; }
    public DateTimeOffset? LastUpdate { get; init; }
    public int RecordCount { get; init; }
    public bool IsOutdated { get; init; }
    public TimeSpan? Age { get; init; }
}