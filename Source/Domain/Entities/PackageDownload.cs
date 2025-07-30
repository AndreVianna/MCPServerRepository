using System.Diagnostics.CodeAnalysis;

using MCPHub.Domain.Common;

namespace MCPHub.Domain.Entities;

/// <summary>
/// Represents a package download record for analytics and tracking
/// </summary>
public class PackageDownload : BaseEntity {
    /// <summary>
    /// Gets or sets the package identifier that was downloaded
    /// </summary>
    public Guid PackageId { get; set; }

    /// <summary>
    /// Gets or sets the navigation property to the package
    /// </summary>
    public Package Package { get; set; } = null!;

    /// <summary>
    /// Gets or sets the version of the package that was downloaded
    /// </summary>
    public string Version { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user ID who downloaded the package (null for anonymous downloads)
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// Gets or sets the IP address from which the download was initiated
    /// </summary>
    public string IpAddress { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user agent string from the client
    /// </summary>
    public string UserAgent { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the timestamp when the download occurred
    /// </summary>
    public DateTimeOffset DownloadedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets or sets the method used for download (CLI, Web, API)
    /// </summary>
    public string DownloadMethod { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets additional metadata about the download
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = [];

    private PackageDownload() { } // For EF Core

    [SetsRequiredMembers]
    public PackageDownload(
        Guid packageId,
        string version,
        string ipAddress,
        string userAgent,
        string downloadMethod,
        Guid? userId = null,
        Dictionary<string, object>? metadata = null) {
        ArgumentException.ThrowIfNullOrWhiteSpace(version);
        ArgumentException.ThrowIfNullOrWhiteSpace(ipAddress);
        ArgumentException.ThrowIfNullOrWhiteSpace(userAgent);
        ArgumentException.ThrowIfNullOrWhiteSpace(downloadMethod);

        PackageId = packageId;
        Version = version;
        UserId = userId;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        DownloadMethod = downloadMethod;
        DownloadedAt = DateTimeOffset.UtcNow;
        Metadata = metadata ?? [];

        AuditTrail.Add(new AuditEntry {
            Action = "Downloaded",
            UserId = userId ?? Guid.Empty,
            DateTime = DateTimeOffset.UtcNow,
        });
    }

    /// <summary>
    /// Updates the download metadata
    /// </summary>
    /// <param name="metadata">New metadata to add or update</param>
    /// <param name="userId">User performing the update</param>
    public void UpdateMetadata(Dictionary<string, object> metadata, Guid? userId = null) {
        ArgumentNullException.ThrowIfNull(metadata);

        foreach (var kvp in metadata) {
            Metadata[kvp.Key] = kvp.Value;
        }

        AuditTrail.Add(new AuditEntry {
            Action = "Metadata Updated",
            UserId = userId ?? Guid.Empty,
            DateTime = DateTimeOffset.UtcNow,
        });
    }
}