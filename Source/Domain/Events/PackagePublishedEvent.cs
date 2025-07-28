using MCPHub.Domain.Messaging;

namespace MCPHub.Domain.Events;

/// <summary>
/// Event raised when a new MCP package is published
/// </summary>
public record PackagePublishedEvent : BaseEvent
{
    /// <summary>
    /// ID of the published package
    /// </summary>
    public Guid PackageId { get; init; }

    /// <summary>
    /// Package name
    /// </summary>
    public string PackageName { get; init; } = string.Empty;

    /// <summary>
    /// Initial version published
    /// </summary>
    public new string Version { get; init; } = string.Empty;

    /// <summary>
    /// Publisher ID
    /// </summary>
    public Guid PublisherId { get; init; }

    /// <summary>
    /// Publisher name/username
    /// </summary>
    public string PublisherName { get; init; } = string.Empty;

    /// <summary>
    /// Package description
    /// </summary>
    public string Description { get; init; } = string.Empty;

    /// <summary>
    /// Package tags for categorization
    /// </summary>
    public IEnumerable<string> Tags { get; init; } = [];

    /// <summary>
    /// Package license
    /// </summary>
    public string License { get; init; } = string.Empty;

    /// <summary>
    /// Repository URL if provided
    /// </summary>
    public string? Repository { get; init; }

    /// <summary>
    /// Storage URL where the package is stored
    /// </summary>
    public string StorageUrl { get; init; } = string.Empty;

    /// <summary>
    /// File size in bytes
    /// </summary>
    public long FileSize { get; init; }

    /// <summary>
    /// SHA256 checksum of the package
    /// </summary>
    public string ChecksumSha256 { get; init; } = string.Empty;

    /// <summary>
    /// Whether this is a prerelease version
    /// </summary>
    public bool IsPrerelease { get; init; }

    /// <summary>
    /// Publication timestamp
    /// </summary>
    public DateTimeOffset PublishedAt { get; init; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// MCP capabilities provided by this package
    /// </summary>
    public PackageCapabilitiesSummary Capabilities { get; init; } = new();

    public PackagePublishedEvent(
        Guid packageId,
        string packageName,
        string version,
        Guid publisherId,
        string publisherName,
        string description,
        IEnumerable<string> tags,
        string license,
        string storageUrl,
        long fileSize,
        string checksumSha256,
        bool isPrerelease = false,
        string? repository = null,
        PackageCapabilitiesSummary? capabilities = null)
    {
        PackageId = packageId;
        PackageName = packageName;
        Version = version;
        PublisherId = publisherId;
        PublisherName = publisherName;
        Description = description;
        Tags = tags;
        License = license;
        Repository = repository;
        StorageUrl = storageUrl;
        FileSize = fileSize;
        ChecksumSha256 = checksumSha256;
        IsPrerelease = isPrerelease;
        Capabilities = capabilities ?? new();
    }
}

/// <summary>
/// Summary of MCP capabilities for events
/// </summary>
public class PackageCapabilitiesSummary
{
    /// <summary>
    /// Number of tools provided
    /// </summary>
    public int ToolsCount { get; init; }

    /// <summary>
    /// Number of resources provided
    /// </summary>
    public int ResourcesCount { get; init; }

    /// <summary>
    /// Number of prompts provided
    /// </summary>
    public int PromptsCount { get; init; }

    /// <summary>
    /// Names of tools provided
    /// </summary>
    public IEnumerable<string> ToolNames { get; init; } = [];

    /// <summary>
    /// Types of resources provided
    /// </summary>
    public IEnumerable<string> ResourceTypes { get; init; } = [];

    /// <summary>
    /// Names of prompts provided
    /// </summary>
    public IEnumerable<string> PromptNames { get; init; } = [];

    /// <summary>
    /// Whether network access is required
    /// </summary>
    public bool RequiresNetwork { get; init; }

    /// <summary>
    /// Whether filesystem access is required
    /// </summary>
    public bool RequiresFilesystem { get; init; }

    /// <summary>
    /// Whether environment variables are required
    /// </summary>
    public bool RequiresEnvironment { get; init; }
}