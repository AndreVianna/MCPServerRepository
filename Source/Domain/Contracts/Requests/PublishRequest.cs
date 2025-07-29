using System.ComponentModel.DataAnnotations;

namespace MCPHub.Domain.Contracts.Requests;

/// <summary>
/// Request model for publishing a new MCP package
/// </summary>
public class PublishRequest {
    /// <summary>
    /// Package manifest content (mcp-manifest.json)
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
    /// Package tags for categorization and search
    /// </summary>
    public IEnumerable<string> Tags { get; set; } = [];

    /// <summary>
    /// README content in markdown format
    /// </summary>
    [MaxLength(100000)] // 100KB limit for README
    public string? ReadmeContent { get; set; }

    /// <summary>
    /// Changelog content for this version
    /// </summary>
    [MaxLength(50000)] // 50KB limit for changelog
    public string? ChangelogContent { get; set; }

    /// <summary>
    /// Validates that either PackageArchive or PackageUrl is provided
    /// </summary>
    public bool IsValid() => PackageArchive?.Length > 0 || !string.IsNullOrWhiteSpace(PackageUrl);
}