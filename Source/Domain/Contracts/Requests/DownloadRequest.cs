using System.ComponentModel.DataAnnotations;

namespace MCPHub.Domain.Contracts.Requests;

/// <summary>
/// Request model for recording package downloads
/// </summary>
public record DownloadRequest
{
    /// <summary>
    /// Gets or sets the user agent string from the client
    /// </summary>
    [Required]
    [StringLength(500, MinimumLength = 1)]
    public required string UserAgent { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the download method (CLI, Web, API)
    /// </summary>
    [Required]
    [StringLength(20, MinimumLength = 1)]
    public required string DownloadMethod { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the client version used for download
    /// </summary>
    [StringLength(50)]
    public string? ClientVersion { get; init; }

    /// <summary>
    /// Gets or sets additional metadata about the download
    /// </summary>
    public Dictionary<string, object>? Metadata { get; init; }

    /// <summary>
    /// Validates that the download method is one of the allowed values
    /// </summary>
    /// <returns>True if valid, false otherwise</returns>
    public bool IsValid()
    {
        var allowedMethods = new[] { "CLI", "Web", "API" };
        return allowedMethods.Contains(DownloadMethod, StringComparer.OrdinalIgnoreCase);
    }
}