using MCPHub.Domain.Messaging;

namespace MCPHub.Domain.Events;

/// <summary>
/// Event raised when a new version of an existing MCP package is published
/// </summary>
public record PackageVersionPublishedEvent : BaseEvent
{
    /// <summary>
    /// ID of the parent package
    /// </summary>
    public Guid PackageId { get; init; }

    /// <summary>
    /// ID of the published package version
    /// </summary>
    public Guid PackageVersionId { get; init; }

    /// <summary>
    /// Package name
    /// </summary>
    public string PackageName { get; init; } = string.Empty;

    /// <summary>
    /// New version published
    /// </summary>
    public new string Version { get; init; } = string.Empty;

    /// <summary>
    /// Previous version (if any)
    /// </summary>
    public string? PreviousVersion { get; init; }

    /// <summary>
    /// Publisher ID
    /// </summary>
    public Guid PublisherId { get; init; }

    /// <summary>
    /// Publisher name/username
    /// </summary>
    public string PublisherName { get; init; } = string.Empty;

    /// <summary>
    /// Release notes for this version
    /// </summary>
    public string? ReleaseNotes { get; init; }

    /// <summary>
    /// Storage URL where the package version is stored
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
    /// Changes in MCP capabilities from previous version
    /// </summary>
    public PackageCapabilitiesChanges CapabilityChanges { get; init; } = new();

    /// <summary>
    /// Whether this is a breaking change from the previous version
    /// </summary>
    public bool IsBreakingChange { get; init; }

    /// <summary>
    /// Migration notes for breaking changes
    /// </summary>
    public string? MigrationNotes { get; init; }

    public PackageVersionPublishedEvent(
        Guid packageId,
        Guid packageVersionId,
        string packageName,
        string version,
        Guid publisherId,
        string publisherName,
        string storageUrl,
        long fileSize,
        string checksumSha256,
        bool isPrerelease = false,
        string? previousVersion = null,
        string? releaseNotes = null,
        PackageCapabilitiesChanges? capabilityChanges = null,
        bool isBreakingChange = false,
        string? migrationNotes = null)
    {
        PackageId = packageId;
        PackageVersionId = packageVersionId;
        PackageName = packageName;
        Version = version;
        PreviousVersion = previousVersion;
        PublisherId = publisherId;
        PublisherName = publisherName;
        ReleaseNotes = releaseNotes;
        StorageUrl = storageUrl;
        FileSize = fileSize;
        ChecksumSha256 = checksumSha256;
        IsPrerelease = isPrerelease;
        CapabilityChanges = capabilityChanges ?? new();
        IsBreakingChange = isBreakingChange;
        MigrationNotes = migrationNotes;
    }
}

/// <summary>
/// Summary of capability changes between package versions
/// </summary>
public class PackageCapabilitiesChanges
{
    /// <summary>
    /// Tools added in this version
    /// </summary>
    public IEnumerable<string> AddedTools { get; init; } = [];

    /// <summary>
    /// Tools removed in this version
    /// </summary>
    public IEnumerable<string> RemovedTools { get; init; } = [];

    /// <summary>
    /// Tools modified in this version
    /// </summary>
    public IEnumerable<string> ModifiedTools { get; init; } = [];

    /// <summary>
    /// Resources added in this version
    /// </summary>
    public IEnumerable<string> AddedResources { get; init; } = [];

    /// <summary>
    /// Resources removed in this version
    /// </summary>
    public IEnumerable<string> RemovedResources { get; init; } = [];

    /// <summary>
    /// Resources modified in this version
    /// </summary>
    public IEnumerable<string> ModifiedResources { get; init; } = [];

    /// <summary>
    /// Prompts added in this version
    /// </summary>
    public IEnumerable<string> AddedPrompts { get; init; } = [];

    /// <summary>
    /// Prompts removed in this version
    /// </summary>
    public IEnumerable<string> RemovedPrompts { get; init; } = [];

    /// <summary>
    /// Prompts modified in this version
    /// </summary>
    public IEnumerable<string> ModifiedPrompts { get; init; } = [];

    /// <summary>
    /// New permissions required in this version
    /// </summary>
    public IEnumerable<string> AddedPermissions { get; init; } = [];

    /// <summary>
    /// Permissions no longer required in this version
    /// </summary>
    public IEnumerable<string> RemovedPermissions { get; init; } = [];

    /// <summary>
    /// Whether there are any capability changes
    /// </summary>
    public bool HasChanges => 
        AddedTools.Any() || RemovedTools.Any() || ModifiedTools.Any() ||
        AddedResources.Any() || RemovedResources.Any() || ModifiedResources.Any() ||
        AddedPrompts.Any() || RemovedPrompts.Any() || ModifiedPrompts.Any() ||
        AddedPermissions.Any() || RemovedPermissions.Any();
}