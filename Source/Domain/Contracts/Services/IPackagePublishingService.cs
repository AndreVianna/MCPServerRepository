using MCPHub.Domain.Contracts.Requests;
using MCPHub.Domain.Contracts.Responses;
using MCPHub.Domain.ValueObjects;

namespace MCPHub.Domain.Contracts.Services;

/// <summary>
/// Service for publishing MCP packages and managing versions
/// </summary>
public interface IPackagePublishingService {
    /// <summary>
    /// Validates a package manifest without publishing
    /// </summary>
    /// <param name="manifestContent">Raw manifest JSON content</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Validation result with detailed feedback</returns>
    Task<PublishResult> ValidateManifestAsync(
        string manifestContent,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Publishes a new MCP package
    /// </summary>
    /// <param name="request">Package publishing request</param>
    /// <param name="userId">ID of the user publishing the package</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Publishing result with package details or errors</returns>
    Task<PublishResult> PublishPackageAsync(
        PublishRequest request,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Publishes a new version of an existing package
    /// </summary>
    /// <param name="packageName">Name of the existing package</param>
    /// <param name="request">Version publishing request</param>
    /// <param name="userId">ID of the user publishing the version</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Publishing result with version details or errors</returns>
    Task<PublishResult> PublishPackageVersionAsync(
        string packageName,
        PublishVersionRequest request,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs comprehensive pre-publish validation
    /// </summary>
    /// <param name="request">Package publishing request</param>
    /// <param name="userId">ID of the user attempting to publish</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Validation result with all checks performed</returns>
    Task<ValidationSummary> PrePublishValidationAsync(
        PublishRequest request,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all versions of a specific package
    /// </summary>
    /// <param name="packageName">Name of the package</param>
    /// <param name="includePrerelease">Whether to include prerelease versions</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of package versions</returns>
    Task<IEnumerable<PackageVersionInfo>> GetPackageVersionsAsync(
        string packageName,
        bool includePrerelease = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a package name is available for publishing
    /// </summary>
    /// <param name="packageName">Package name to check</param>
    /// <param name="userId">ID of the user checking availability</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if available, false if taken or user lacks permission</returns>
    Task<bool> IsPackageNameAvailableAsync(
        string packageName,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a secure download URL for a published package version
    /// </summary>
    /// <param name="packageName">Package name</param>
    /// <param name="version">Package version</param>
    /// <param name="expirationMinutes">URL expiration time in minutes (default: 60)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Secure download URL</returns>
    Task<string> GenerateDownloadUrlAsync(
        string packageName,
        string version,
        int expirationMinutes = 60,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Unpublishes a package version (admin operation)
    /// </summary>
    /// <param name="packageName">Package name</param>
    /// <param name="version">Version to unpublish</param>
    /// <param name="userId">ID of the user performing the operation</param>
    /// <param name="reason">Reason for unpublishing</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if unpublished successfully</returns>
    Task<bool> UnpublishPackageVersionAsync(
        string packageName,
        string version,
        Guid userId,
        string reason,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Information about a package version
/// </summary>
public class PackageVersionInfo {
    /// <summary>
    /// Version string
    /// </summary>
    public string Version { get; set; } = string.Empty;

    /// <summary>
    /// Publication date
    /// </summary>
    public DateTimeOffset PublishedAt { get; set; }

    /// <summary>
    /// Whether this is a prerelease version
    /// </summary>
    public bool IsPrerelease { get; set; }

    /// <summary>
    /// Download URL
    /// </summary>
    public string DownloadUrl { get; set; } = string.Empty;

    /// <summary>
    /// File size in bytes
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// SHA256 checksum
    /// </summary>
    public string ChecksumSha256 { get; set; } = string.Empty;

    /// <summary>
    /// Release notes for this version
    /// </summary>
    public string? ReleaseNotes { get; set; }

    /// <summary>
    /// Security scan status
    /// </summary>
    public SecurityScanStatus ScanStatus { get; set; } = SecurityScanStatus.Pending;
}