using MCPHub.Domain.Entities;

namespace MCPHub.Domain.Repositories;

/// <summary>
/// Repository interface for package installation tracking operations
/// </summary>
public interface IPackageInstallationRepository : IRepository<PackageInstallation> {
    /// <summary>
    /// Gets all installations for a specific user
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="includeUninstalled">Whether to include uninstalled packages</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of user installations</returns>
    Task<IEnumerable<PackageInstallation>> GetByUserIdAsync(
        Guid userId,
        bool includeUninstalled = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all installations for a specific package
    /// </summary>
    /// <param name="packageId">Package identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of package installations</returns>
    Task<IEnumerable<PackageInstallation>> GetByPackageIdAsync(Guid packageId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets installations for a specific package version
    /// </summary>
    /// <param name="packageId">Package identifier</param>
    /// <param name="version">Package version</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of installations for the version</returns>
    Task<IEnumerable<PackageInstallation>> GetByPackageVersionAsync(
        Guid packageId,
        string version,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a specific installation by user and package
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="packageId">Package identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Installation record or null if not found</returns>
    Task<PackageInstallation?> GetByUserAndPackageAsync(
        Guid userId,
        Guid packageId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets installations with a specific status
    /// </summary>
    /// <param name="status">Installation status to filter by</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of installations with the specified status</returns>
    Task<IEnumerable<PackageInstallation>> GetByStatusAsync(
        InstallationStatus status,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a package is currently installed for a user
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="packageId">Package identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if package is installed, false otherwise</returns>
    Task<bool> IsPackageInstalledAsync(
        Guid userId,
        Guid packageId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets active installations (completed and not uninstalled) for a user
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of active installations</returns>
    Task<IEnumerable<PackageInstallation>> GetActiveInstallationsAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets installation count for a specific package
    /// </summary>
    /// <param name="packageId">Package identifier</param>
    /// <param name="activeOnly">Whether to count only active installations</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Installation count</returns>
    Task<int> GetInstallationCountAsync(
        Guid packageId,
        bool activeOnly = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets installations within a specific date range
    /// </summary>
    /// <param name="startDate">Start date for the range</param>
    /// <param name="endDate">End date for the range</param>
    /// <param name="packageId">Optional package identifier to filter by</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of installations in the date range</returns>
    Task<IEnumerable<PackageInstallation>> GetInstallationsInDateRangeAsync(
        DateTimeOffset startDate,
        DateTimeOffset endDate,
        Guid? packageId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets installation statistics grouped by status
    /// </summary>
    /// <param name="packageId">Optional package identifier to filter by</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Dictionary with status as key and count as value</returns>
    Task<Dictionary<InstallationStatus, int>> GetInstallationsByStatusAsync(
        Guid? packageId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets failed installations with error messages
    /// </summary>
    /// <param name="packageId">Optional package identifier to filter by</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of failed installations</returns>
    Task<IEnumerable<PackageInstallation>> GetFailedInstallationsAsync(
        Guid? packageId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets installations that have been pending for too long (potential timeouts)
    /// </summary>
    /// <param name="timeoutThreshold">Time threshold after which installations are considered stuck</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of potentially stuck installations</returns>
    Task<IEnumerable<PackageInstallation>> GetStuckInstallationsAsync(
        TimeSpan? timeoutThreshold = null,
        CancellationToken cancellationToken = default);
}