using MCPHub.Domain.Common;
using MCPHub.Domain.ValueObjects;

namespace MCPHub.Domain.Entities;

/// <summary>
/// Represents a specific version of an MCP package
/// </summary>
public class PackageVersion : BaseEntity {
    public Guid PackageId { get; set; }
    [MaxLength(32)]
    public string Version { get; set; } = string.Empty;
    [MaxLength(4096)]
    public string? ReleaseNotes { get; set; }
    [MaxLength(256)]
    public string DownloadUrl { get; set; } = string.Empty;
    [MaxLength(4096)]
    public string ChecksumSha256 { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public bool IsPrerelease { get; set; } = false;
    public SecurityScanResult? ScanResult { get; set; }

    // Navigation properties
    public Package Package { get; set; } = null!;

    private PackageVersion() { } // For EF Core

    public PackageVersion(
        Guid packageId,
        string version,
        string downloadUrl,
        string checksumSha256,
        long fileSize,
        string? releaseNotes = null,
        bool isPrerelease = false) {
        ArgumentException.ThrowIfNullOrWhiteSpace(version);
        ArgumentException.ThrowIfNullOrWhiteSpace(downloadUrl);
        ArgumentException.ThrowIfNullOrWhiteSpace(checksumSha256);

        PackageId = packageId;
        Version = version;
        DownloadUrl = downloadUrl;
        ChecksumSha256 = checksumSha256;
        FileSize = fileSize;
        ReleaseNotes = releaseNotes;
        IsPrerelease = isPrerelease;
        AuditTrail.Add(new AuditEntry {
            Action = "Created",
            UserId = Guid.Empty, // System action
            DateTime = DateTimeOffset.UtcNow
        });
    }

    public void UpdateSecurityScan(SecurityScanResult scanResult) {
        ScanResult = scanResult;
        AuditTrail.Add(new AuditEntry {
            Action = "Security Scan Updated",
            UserId = Guid.Empty, // System action
            DateTime = DateTimeOffset.UtcNow
        });
    }
}