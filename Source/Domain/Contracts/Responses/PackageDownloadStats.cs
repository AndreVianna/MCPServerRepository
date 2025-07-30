namespace MCPHub.Domain.Contracts.Responses;

/// <summary>
/// Statistics model for package download analytics
/// </summary>
public record PackageDownloadStats {
    /// <summary>
    /// Gets or sets the total number of downloads for the package
    /// </summary>
    public required int TotalDownloads { get; init; }

    /// <summary>
    /// Gets or sets the number of unique downloads (unique users/IPs)
    /// </summary>
    public required int UniqueDownloads { get; init; }

    /// <summary>
    /// Gets or sets the number of downloads in the last 30 days
    /// </summary>
    public required int DownloadsLast30Days { get; init; }

    /// <summary>
    /// Gets or sets download counts by package version
    /// </summary>
    public required Dictionary<string, int> DownloadsByVersion { get; init; }

    /// <summary>
    /// Gets or sets download counts by download method (CLI, Web, API)
    /// </summary>
    public required Dictionary<string, int> DownloadsByMethod { get; init; }

    /// <summary>
    /// Gets or sets download counts by time period (daily for last 30 days)
    /// </summary>
    public Dictionary<string, int>? DownloadsByDay { get; init; }

    /// <summary>
    /// Gets or sets the most recent download timestamp
    /// </summary>
    public DateTimeOffset? LastDownloadAt { get; init; }

    /// <summary>
    /// Gets or sets the package ID these stats are for
    /// </summary>
    public Guid? PackageId { get; init; }

    /// <summary>
    /// Gets or sets when these statistics were calculated
    /// </summary>
    public DateTimeOffset GeneratedAt { get; init; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Creates empty stats for a package with no downloads
    /// </summary>
    /// <param name="packageId">Package identifier</param>
    /// <returns>Empty download stats</returns>
    public static PackageDownloadStats Empty(Guid packageId) => new() {
        PackageId = packageId,
        TotalDownloads = 0,
        UniqueDownloads = 0,
        DownloadsLast30Days = 0,
        DownloadsByVersion = [],
        DownloadsByMethod = [],
        DownloadsByDay = [],
        GeneratedAt = DateTimeOffset.UtcNow,
    };
}