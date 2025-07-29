using System.ComponentModel.DataAnnotations;

namespace MCPHub.Domain.Contracts.Requests;

/// <summary>
/// Request model for publishing a new version of an existing MCP package
/// </summary>
public class PublishVersionRequest {
    /// <summary>
    /// Semantic version string (e.g., "1.2.0", "2.0.0-beta.1")
    /// </summary>
    [Required]
    [MaxLength(32)]
    public string Version { get; set; } = string.Empty;

    /// <summary>
    /// Package manifest content (mcp-manifest.json) for this version
    /// </summary>
    [Required]
    [MaxLength(50000)] // 50KB limit for manifest
    public string ManifestContent { get; set; } = string.Empty;

    /// <summary>
    /// Package archive content (optional - URL can be used instead)
    /// </summary>
    public byte[]? PackageArchive { get; set; }

    /// <summary>
    /// URL to package archive (alternative to PackageArchive)
    /// </summary>
    [Url]
    [MaxLength(2048)]
    public string? PackageUrl { get; set; }

    /// <summary>
    /// Changelog content for this version
    /// </summary>
    [MaxLength(50000)] // 50KB limit for changelog
    public string? ChangelogContent { get; set; }

    /// <summary>
    /// Indicates if this is a prerelease version
    /// </summary>
    public bool IsPrerelease { get; set; } = false;

    /// <summary>
    /// Validates that either PackageArchive or PackageUrl is provided
    /// </summary>
    public bool IsValid() => PackageArchive?.Length > 0 || !string.IsNullOrWhiteSpace(PackageUrl);
}