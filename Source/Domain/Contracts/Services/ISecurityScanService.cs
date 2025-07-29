using MCPHub.Domain.Contracts.Requests;
using MCPHub.Domain.Entities;
using MCPHub.Domain.ValueObjects;

namespace MCPHub.Domain.Contracts.Services;

/// <summary>
/// Application service interface for security scanning operations
/// </summary>
public interface ISecurityScanService {
    /// <summary>
    /// Initiates a security scan for a package
    /// </summary>
    /// <param name="packageId">Package identifier</param>
    /// <param name="scanType">Type of security scan to perform</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Security scan entity</returns>
    Task<SecurityScan> InitiateScanAsync(string packageId, ScanType scanType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Scans a package for security vulnerabilities
    /// </summary>
    /// <param name="packageId">Package identifier</param>
    /// <param name="version">Package version</param>
    /// <param name="request">Scan request with options</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Security scan result</returns>
    Task<SecurityScanResult> ScanPackageAsync(Guid packageId, string version, ScanRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Scans a package manifest for security issues
    /// </summary>
    /// <param name="manifestContent">Raw manifest JSON content</param>
    /// <param name="request">Scan request with options</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Security scan result</returns>
    Task<SecurityScanResult> ScanManifestAsync(string manifestContent, ScanRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all security scans for a package
    /// </summary>
    /// <param name="packageId">Package identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of security scans</returns>
    Task<IEnumerable<SecurityScan>> GetPackageScansAsync(Guid packageId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the latest security scan result for a package version
    /// </summary>
    /// <param name="packageId">Package identifier</param>
    /// <param name="version">Package version</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Latest security scan result or null if none found</returns>
    Task<SecurityScanResult?> GetLatestScanResultAsync(Guid packageId, string version, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a comprehensive security summary for a package across all versions
    /// </summary>
    /// <param name="packageId">Package identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Security scan summary</returns>
    Task<SecurityScanSummary> GetPackageSecuritySummaryAsync(Guid packageId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a security scan by its identifier
    /// </summary>
    /// <param name="scanId">Scan identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Security scan entity or null if not found</returns>
    Task<SecurityScan?> GetScanAsync(string scanId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all security scans for a package (legacy method)
    /// </summary>
    /// <param name="packageId">Package identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of security scans</returns>
    Task<IEnumerable<SecurityScan>> GetScansByPackageAsync(string packageId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the latest security scan for a package (legacy method)
    /// </summary>
    /// <param name="packageId">Package identifier</param>
    /// <param name="scanType">Optional scan type filter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Latest security scan or null if none found</returns>
    Task<SecurityScan?> GetLatestScanAsync(string packageId, ScanType? scanType = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the results of a security scan
    /// </summary>
    /// <param name="scanId">Scan identifier</param>
    /// <param name="result">Scan result</param>
    /// <param name="vulnerabilities">List of vulnerabilities found</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated security scan</returns>
    Task<SecurityScan> UpdateScanResultsAsync(string scanId, SecurityScanResult result, IEnumerable<SecurityVulnerability> vulnerabilities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets security scans that require attention based on severity
    /// </summary>
    /// <param name="minSeverity">Minimum severity level</param>
    /// <param name="pageSize">Number of results per page</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of security scans requiring attention</returns>
    Task<IEnumerable<SecurityScan>> GetScansRequiringAttentionAsync(SecurityScanSeverity minSeverity, int pageSize = 20, int pageIndex = 0, CancellationToken cancellationToken = default);
}