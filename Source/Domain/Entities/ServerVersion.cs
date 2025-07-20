using MCPHub.Domain.Common;
using MCPHub.Domain.ValueObjects;

namespace MCPHub.Domain.Entities;

/// <summary>
/// Represents a version of an MCP server
/// </summary>
public class ServerVersion : BaseEntity {
    [MaxLength(32)]
    public string Version { get; set; } = string.Empty;
    public Guid ServerId { get; set; }
    public Server Server { get; set; } = null!;
    [MaxLength(4096)]
    public string? ReleaseNotes { get; set; }
    public VersionStatus Status { get; set; } = VersionStatus.Pending;
    public SecurityScanResult? SecurityScan { get; set; }
    public List<SecurityScan> SecurityScans { get; set; } = [];
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
        ArgumentException.ThrowIfNullOrWhiteSpace(version);

        Version = version;
        ServerId = serverId;
        ReleaseNotes = releaseNotes;
        PackageUrl = packageUrl;
        PackageSize = packageSize;
        Checksum = checksum;
        Status = VersionStatus.Pending;
    }

    public void UpdateStatus(VersionStatus status) {
        Status = status;
        SetUpdatedBy(UpdatedBy ?? "system");
    }

    public void UpdateSecurityScan(SecurityScanResult scanResult) {
        SecurityScan = scanResult;
        SetUpdatedBy(UpdatedBy ?? "system");
    }

    public void UpdatePackageInfo(string? packageUrl, long? packageSize, string? checksum) {
        PackageUrl = packageUrl;
        PackageSize = packageSize;
        Checksum = checksum;
        SetUpdatedBy(UpdatedBy ?? "system");
    }
}

public enum VersionStatus {
    Pending,
    Approved,
    Rejected,
    Deprecated
}