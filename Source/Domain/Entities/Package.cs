namespace Domain.Entities;

/// <summary>
/// Represents an MCP package in the registry
/// </summary>
public class Package {
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public Guid PublisherId { get; set; }
    public Publisher Publisher { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public PackageStatus Status { get; set; }
    public string? Repository { get; set; }
    public string? License { get; set; }
    public List<string> Tags { get; set; } = [];
    public List<PackageVersion> Versions { get; set; } = [];
    public SecurityScanResult? SecurityScan { get; set; }
    public TrustTier TrustTier { get; set; }

    private Package() { } // For EF Core

    public Package(
        string name,
        string description,
        string version,
        Guid publisherId,
        string? repository = null,
        string? license = null,
        List<string>? tags = null) {
        Id = Guid.NewGuid();
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Version = version ?? throw new ArgumentNullException(nameof(version));
        PublisherId = publisherId;
        Repository = repository;
        License = license;
        Tags = tags ?? [];
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        Status = PackageStatus.Pending;
        TrustTier = TrustTier.Unverified;
    }

    public void UpdateStatus(PackageStatus status) {
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateSecurityScan(SecurityScanResult scanResult) {
        SecurityScan = scanResult;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateTrustTier(TrustTier trustTier) {
        TrustTier = trustTier;
        UpdatedAt = DateTime.UtcNow;
    }
}