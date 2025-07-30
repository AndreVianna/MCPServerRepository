using MCPHub.Domain.Messaging;

namespace MCPHub.Domain.Events;

/// <summary>
/// Domain event triggered when a package is downloaded
/// </summary>
public record PackageDownloadedEvent : BaseMessage {
    /// <summary>
    /// Gets or sets the download tracking identifier
    /// </summary>
    public required Guid DownloadId { get; init; }

    /// <summary>
    /// Gets or sets the package identifier that was downloaded
    /// </summary>
    public required Guid PackageId { get; init; }

    /// <summary>
    /// Gets or sets the package name
    /// </summary>
    public required string PackageName { get; init; }

    /// <summary>
    /// Gets or sets the version of the package that was downloaded
    /// </summary>
    public required string Version { get; init; }

    /// <summary>
    /// Gets or sets the user ID who downloaded the package (null for anonymous downloads)
    /// </summary>
    public new Guid? UserId { get; init; }

    /// <summary>
    /// Gets or sets the IP address from which the download was initiated
    /// </summary>
    public required string IpAddress { get; init; }

    /// <summary>
    /// Gets or sets the download method (CLI, Web, API)
    /// </summary>
    public required string DownloadMethod { get; init; }

    /// <summary>
    /// Gets or sets the user agent string from the client
    /// </summary>
    public required string UserAgent { get; init; }

    /// <summary>
    /// Gets or sets the timestamp when the download occurred
    /// </summary>
    public required DateTimeOffset DownloadedAt { get; init; }

    /// <summary>
    /// Gets or sets the client version used for download
    /// </summary>
    public string? ClientVersion { get; init; }

    /// <summary>
    /// Gets or sets additional metadata about the download
    /// </summary>
    public new Dictionary<string, object>? Metadata { get; init; }

    /// <summary>
    /// Creates a new PackageDownloadedEvent
    /// </summary>
    /// <param name="downloadId">Download tracking identifier</param>
    /// <param name="packageId">Package identifier</param>
    /// <param name="packageName">Package name</param>
    /// <param name="version">Package version</param>
    /// <param name="ipAddress">Client IP address</param>
    /// <param name="downloadMethod">Download method</param>
    /// <param name="userAgent">User agent string</param>
    /// <param name="downloadedAt">Download timestamp</param>
    /// <param name="userId">User ID (optional)</param>
    /// <param name="clientVersion">Client version (optional)</param>
    /// <param name="metadata">Additional metadata (optional)</param>
    /// <returns>New PackageDownloadedEvent instance</returns>
    public static PackageDownloadedEvent Create(
        Guid downloadId,
        Guid packageId,
        string packageName,
        string version,
        string ipAddress,
        string downloadMethod,
        string userAgent,
        DateTimeOffset downloadedAt,
        Guid? userId = null,
        string? clientVersion = null,
        Dictionary<string, object>? metadata = null) => new() {
            DownloadId = downloadId,
            PackageId = packageId,
            PackageName = packageName,
            Version = version,
            UserId = userId,
            IpAddress = ipAddress,
            DownloadMethod = downloadMethod,
            UserAgent = userAgent,
            DownloadedAt = downloadedAt,
            ClientVersion = clientVersion,
            Metadata = metadata
        };
}