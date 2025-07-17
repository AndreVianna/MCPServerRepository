namespace Domain.Entities;

/// <summary>
/// Represents a version of an MCP server
/// </summary>
public class ServerVersion {
    public Guid Id { get; set; } = Guid.CreateVersion7();
    [MaxLength(32)]
    public string Version { get; set; } = string.Empty;
    public Guid ServerId { get; set; }
    public Server Server { get; set; } = null!;
    [MaxLength(4096)]
    public string? ReleaseNotes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public VersionStatus Status { get; set; }
    public SecurityScanResult? SecurityScan { get; set; }
    [MaxLength(256)]
    public string? PackageUrl { get; set; }
    public long? PackageSize { get; set; }
    [MaxLength(64)]
    public string? Checksum { get; set; }

    private ServerVersion() { } // For EF Core

    public ServerVersion(
        string version,
        Guid serverId,
        string? releaseNotes = null,
        string? packageUrl = null,
        long? packageSize = null,
        string? checksum = null) {
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

    public void UpdateStatus(VersionStatus status) {
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateSecurityScan(SecurityScanResult scanResult) {
        SecurityScan = scanResult;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePackageInfo(string? packageUrl, long? packageSize, string? checksum) {
        PackageUrl = packageUrl;
        PackageSize = packageSize;
        Checksum = checksum;
        UpdatedAt = DateTime.UtcNow;
    }
}

public enum VersionStatus {
    Pending,
    Approved,
    Rejected,
    Deprecated
}