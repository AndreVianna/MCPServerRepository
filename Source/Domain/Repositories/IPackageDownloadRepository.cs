using MCPHub.Domain.Entities;

namespace MCPHub.Domain.Repositories;

/// <summary>
/// Repository interface for package download tracking operations
/// </summary>
public interface IPackageDownloadRepository : IRepository<PackageDownload> {
    /// <summary>
    /// Gets all downloads for a specific package
    /// </summary>
    /// <param name="packageId">Package identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of package downloads</returns>
    Task<IEnumerable<PackageDownload>> GetByPackageIdAsync(Guid packageId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets downloads for a specific package version
    /// </summary>
    /// <param name="packageId">Package identifier</param>
    /// <param name="version">Package version</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of downloads for the version</returns>
    Task<IEnumerable<PackageDownload>> GetByPackageVersionAsync(Guid packageId, string version, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets downloads by a specific user
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of user downloads</returns>
    Task<IEnumerable<PackageDownload>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets download count for a specific package
    /// </summary>
    /// <param name="packageId">Package identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Total download count</returns>
    Task<int> GetDownloadCountAsync(Guid packageId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets unique download count for a specific package (unique users/IPs)
    /// </summary>
    /// <param name="packageId">Package identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Unique download count</returns>
    Task<int> GetUniqueDownloadCountAsync(Guid packageId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets downloads within a specific date range
    /// </summary>
    /// <param name="packageId">Package identifier</param>
    /// <param name="startDate">Start date for the range</param>
    /// <param name="endDate">End date for the range</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of downloads in the date range</returns>
    Task<IEnumerable<PackageDownload>> GetDownloadsInDateRangeAsync(
        Guid packageId,
        DateTimeOffset startDate,
        DateTimeOffset endDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets download statistics grouped by version
    /// </summary>
    /// <param name="packageId">Package identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Dictionary with version as key and download count as value</returns>
    Task<Dictionary<string, int>> GetDownloadsByVersionAsync(Guid packageId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets download statistics grouped by download method
    /// </summary>
    /// <param name="packageId">Package identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Dictionary with download method as key and count as value</returns>
    Task<Dictionary<string, int>> GetDownloadsByMethodAsync(Guid packageId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets download statistics grouped by day
    /// </summary>
    /// <param name="packageId">Package identifier</param>
    /// <param name="days">Number of days to look back</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Dictionary with date as key and download count as value</returns>
    Task<Dictionary<string, int>> GetDownloadsByDayAsync(Guid packageId, int days = 30, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the most recent download for a package
    /// </summary>
    /// <param name="packageId">Package identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Most recent download or null if no downloads</returns>
    Task<PackageDownload?> GetMostRecentDownloadAsync(Guid packageId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a specific IP has downloaded a package recently (for rate limiting)
    /// </summary>
    /// <param name="packageId">Package identifier</param>
    /// <param name="ipAddress">IP address to check</param>
    /// <param name="timeWindow">Time window to check (default: 1 hour)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if IP has downloaded recently, false otherwise</returns>
    Task<bool> HasRecentDownloadFromIpAsync(
        Guid packageId,
        string ipAddress,
        TimeSpan? timeWindow = null,
        CancellationToken cancellationToken = default);
}