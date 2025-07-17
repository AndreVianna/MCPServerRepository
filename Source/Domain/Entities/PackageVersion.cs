namespace Domain.Entities;

/// <summary>
/// Represents a specific version of an MCP package
/// </summary>
public class PackageVersion {
    public Guid Id { get; set; }
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
    public DateTime CreatedAt { get; set; }
    public bool IsPrerelease { get; set; }
    public SecurityScanResult? SecurityScan { get; set; }

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
        Id = Guid.NewGuid();
        PackageId = packageId;
        Version = version ?? throw new ArgumentNullException(nameof(version));
        DownloadUrl = downloadUrl ?? throw new ArgumentNullException(nameof(downloadUrl));
        ChecksumSha256 = checksumSha256 ?? throw new ArgumentNullException(nameof(checksumSha256));
        FileSize = fileSize;
        ReleaseNotes = releaseNotes;
        IsPrerelease = isPrerelease;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateSecurityScan(SecurityScanResult scanResult) => SecurityScan = scanResult;
}