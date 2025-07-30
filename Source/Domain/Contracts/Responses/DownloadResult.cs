namespace MCPHub.Domain.Contracts.Responses;

/// <summary>
/// Result model for package download operations
/// </summary>
public record DownloadResult {
    /// <summary>
    /// Gets or sets whether the download request was successful
    /// </summary>
    public required bool Success { get; init; }

    /// <summary>
    /// Gets or sets the pre-signed URL for package download
    /// </summary>
    public string? DownloadUrl { get; init; }

    /// <summary>
    /// Gets or sets when the download URL expires
    /// </summary>
    public DateTimeOffset? ExpiresAt { get; init; }

    /// <summary>
    /// Gets or sets the download tracking ID
    /// </summary>
    public Guid? DownloadId { get; init; }

    /// <summary>
    /// Gets or sets any error message if the request failed
    /// </summary>
    public string? ErrorMessage { get; init; }

    /// <summary>
    /// Gets or sets additional metadata about the download
    /// </summary>
    public Dictionary<string, object>? Metadata { get; init; }

    /// <summary>
    /// Creates a successful download result
    /// </summary>
    /// <param name="downloadUrl">Pre-signed download URL</param>
    /// <param name="expiresAt">URL expiration time</param>
    /// <param name="downloadId">Download tracking ID</param>
    /// <param name="metadata">Additional metadata</param>
    /// <returns>Successful download result</returns>
    public static DownloadResult CreateSuccess(string downloadUrl, DateTimeOffset expiresAt, Guid downloadId, Dictionary<string, object>? metadata = null) => new() {
        Success = true,
        DownloadUrl = downloadUrl,
        ExpiresAt = expiresAt,
        DownloadId = downloadId,
        Metadata = metadata
    };

    /// <summary>
    /// Creates a failed download result
    /// </summary>
    /// <param name="errorMessage">Error description</param>
    /// <returns>Failed download result</returns>
    public static DownloadResult CreateFailure(string errorMessage) => new() {
        Success = false,
        ErrorMessage = errorMessage
    };
}