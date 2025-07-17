namespace Domain.Entities;

/// <summary>
/// Represents a specific version of an MCP package
/// </summary>
public class PackageVersion
{
    public Guid Id { get; private set; }
    public Guid PackageId { get; private set; }
    public string Version { get; private set; }
    public string? ReleaseNotes { get; private set; }
    public string DownloadUrl { get; private set; }
    public string ChecksumSha256 { get; private set; }
    public long FileSize { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsPrerelease { get; private set; }
    public SecurityScanResult? SecurityScan { get; private set; }

    // Navigation properties
    public Package Package { get; private set; } = null!;

    private PackageVersion() { } // For EF Core

    public PackageVersion(
        Guid packageId,
        string version,
        string downloadUrl,
        string checksumSha256,
        long fileSize,
        string? releaseNotes = null,
        bool isPrerelease = false)
    {
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

    public void UpdateSecurityScan(SecurityScanResult scanResult)
    {
        SecurityScan = scanResult;
    }
}