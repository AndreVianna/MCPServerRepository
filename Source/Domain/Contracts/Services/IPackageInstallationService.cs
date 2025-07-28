using MCPHub.Domain.Contracts.Requests;
using MCPHub.Domain.Contracts.Responses;
using MCPHub.Domain.Entities;

namespace MCPHub.Domain.Contracts.Services;

/// <summary>
/// Service interface for package download and installation tracking operations
/// </summary>
public interface IPackageInstallationService
{
    /// <summary>
    /// Records a package download and generates a secure download URL
    /// </summary>
    /// <param name="packageId">Package identifier</param>
    /// <param name="version">Package version</param>
    /// <param name="request">Download request details</param>
    /// <param name="ipAddress">Client IP address</param>
    /// <param name="userId">User ID (null for anonymous downloads)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Download result with secure URL</returns>
    Task<DownloadResult> RecordDownloadAsync(
        Guid packageId,
        string version,
        DownloadRequest request,
        string ipAddress,
        Guid? userId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Records a package installation
    /// </summary>
    /// <param name="packageId">Package identifier</param>
    /// <param name="version">Package version</param>
    /// <param name="request">Installation request details</param>
    /// <param name="userId">User performing the installation</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Installation result with tracking ID</returns>
    Task<InstallationResult> RecordInstallationAsync(
        Guid packageId,
        string version,
        InstallationRequest request,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the status of an existing installation
    /// </summary>
    /// <param name="installationId">Installation tracking ID</param>
    /// <param name="status">New installation status</param>
    /// <param name="errorMessage">Error message if status is Failed</param>
    /// <param name="userId">User updating the status</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated installation result</returns>
    Task<InstallationResult> UpdateInstallationStatusAsync(
        Guid installationId,
        InstallationStatus status,
        string? errorMessage = null,
        Guid? userId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all installations for a specific user
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="includeUninstalled">Whether to include uninstalled packages</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of user installations</returns>
    Task<IEnumerable<PackageInstallation>> GetUserInstallationsAsync(
        Guid userId,
        bool includeUninstalled = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets download statistics for a specific package
    /// </summary>
    /// <param name="packageId">Package identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Package download statistics</returns>
    Task<PackageDownloadStats> GetPackageDownloadStatsAsync(
        Guid packageId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a specific installation by its ID
    /// </summary>
    /// <param name="installationId">Installation identifier</param>
    /// <param name="userId">User requesting the installation (for authorization)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Installation details or null if not found/authorized</returns>
    Task<PackageInstallation?> GetInstallationAsync(
        Guid installationId,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a package is currently installed for a user
    /// </summary>
    /// <param name="packageId">Package identifier</param>
    /// <param name="userId">User identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if package is currently installed, false otherwise</returns>
    Task<bool> IsPackageInstalledAsync(
        Guid packageId,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates that a package exists and is available for download
    /// </summary>
    /// <param name="packageId">Package identifier</param>
    /// <param name="version">Package version</param>
    /// <param name="userId">User requesting download (for authorization)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if package is available for download, false otherwise</returns>
    Task<bool> ValidatePackageAvailabilityAsync(
        Guid packageId,
        string version,
        Guid? userId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets installation statistics for analytics
    /// </summary>
    /// <param name="packageId">Package identifier (optional, for specific package stats)</param>
    /// <param name="userId">User identifier (optional, for user-specific stats)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Installation statistics</returns>
    Task<Dictionary<string, object>> GetInstallationStatsAsync(
        Guid? packageId = null,
        Guid? userId = null,
        CancellationToken cancellationToken = default);
}