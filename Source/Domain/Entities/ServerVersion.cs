namespace Domain.Entities;

/// <summary>
/// Represents a version of an MCP server
/// </summary>
public class ServerVersion
{
    public Guid Id { get; private set; }
    public string Version { get; private set; }
    public Guid ServerId { get; private set; }
    public Server Server { get; private set; } = null!;
    public string? ReleaseNotes { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public VersionStatus Status { get; private set; }
    public SecurityScanResult? SecurityScan { get; private set; }
    public string? PackageUrl { get; private set; }
    public long? PackageSize { get; private set; }
    public string? Checksum { get; private set; }

    private ServerVersion() { } // For EF Core

    public ServerVersion(
        string version,
        Guid serverId,
        string? releaseNotes = null,
        string? packageUrl = null,
        long? packageSize = null,
        string? checksum = null)
    {
        Id = Guid.NewGuid();
        Version = version ?? throw new ArgumentNullException(nameof(version));
        ServerId = serverId;
        ReleaseNotes = releaseNotes;
        PackageUrl = packageUrl;
        PackageSize = packageSize;
        Checksum = checksum;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        Status = VersionStatus.Pending;
    }

    public void UpdateStatus(VersionStatus status)
    {
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateSecurityScan(SecurityScanResult scanResult)
    {
        SecurityScan = scanResult;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePackageInfo(string? packageUrl, long? packageSize, string? checksum)
    {
        PackageUrl = packageUrl;
        PackageSize = packageSize;
        Checksum = checksum;
        UpdatedAt = DateTime.UtcNow;
    }
}

public enum VersionStatus
{
    Pending,
    Approved,
    Rejected,
    Deprecated
}