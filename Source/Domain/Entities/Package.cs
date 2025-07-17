namespace Domain.Entities;

/// <summary>
/// Represents an MCP package in the registry
/// </summary>
public class Package
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string Version { get; private set; }
    public Guid PublisherId { get; private set; }
    public Publisher Publisher { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public PackageStatus Status { get; private set; }
    public string? Repository { get; private set; }
    public string? License { get; private set; }
    public List<string> Tags { get; private set; } = new();
    public List<PackageVersion> Versions { get; private set; } = new();
    public SecurityScanResult? SecurityScan { get; private set; }
    public TrustTier TrustTier { get; private set; }

    private Package() { } // For EF Core

    public Package(
        string name,
        string description,
        string version,
        Guid publisherId,
        string? repository = null,
        string? license = null,
        List<string>? tags = null)
    {
        Id = Guid.NewGuid();
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Version = version ?? throw new ArgumentNullException(nameof(version));
        PublisherId = publisherId;
        Repository = repository;
        License = license;
        Tags = tags ?? new List<string>();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        Status = PackageStatus.Pending;
        TrustTier = TrustTier.Unverified;
    }

    public void UpdateStatus(PackageStatus status)
    {
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateSecurityScan(SecurityScanResult scanResult)
    {
        SecurityScan = scanResult;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateTrustTier(TrustTier trustTier)
    {
        TrustTier = trustTier;
        UpdatedAt = DateTime.UtcNow;
    }
}

public enum PackageStatus
{
    Pending,
    Approved,
    Rejected,
    Suspended
}

public enum TrustTier
{
    Unverified,
    CommunityTrusted,
    SecurityAudited,
    Certified
}