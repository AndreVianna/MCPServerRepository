using System.Diagnostics.CodeAnalysis;

using MCPHub.Domain.Common;
using MCPHub.Domain.ValueObjects;

namespace MCPHub.Domain.Entities;

/// <summary>
/// Represents an MCP package in the registry
/// </summary>
public class Package : BaseEntity {
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public Guid PublisherId { get; set; }
    public Publisher Publisher { get; set; } = null!;
    public PackageStatus Status { get; set; } = PackageStatus.Pending;
    public string? Repository { get; set; }
    public string? License { get; set; }
    public List<string> Tags { get; set; } = [];
    public List<PackageVersion> Versions { get; set; } = [];
    public SecurityScanResult? ScanResult { get; set; }
    public TrustTier TrustTier { get; set; } = TrustTier.Unverified;

    private Package() { } // For EF Core

    [SetsRequiredMembers]
    public Package(
        string name,
        string description,
        string version,
        Guid publisherId,
        string? repository = null,
        string? license = null,
        List<string>? tags = null) {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        ArgumentException.ThrowIfNullOrWhiteSpace(version);

        Name = name;
        Description = description;
        Version = version;
        PublisherId = publisherId;
        Repository = repository;
        License = license;
        Tags = tags ?? [];
        Status = PackageStatus.Pending;
        TrustTier = TrustTier.Unverified;
        AuditTrail.Add(new AuditEntry {
            Action = "Created",
            UserId = Guid.Empty, // System action
            DateTime = DateTimeOffset.UtcNow,
        });
    }

    public void UpdateStatus(PackageStatus status) {
        Status = status;
        AuditTrail.Add(new AuditEntry {
            Action = $"Status Updated to {status}",
            UserId = Guid.Empty, // System action
            DateTime = DateTimeOffset.UtcNow,
        });
    }

    public void UpdateSecurityScan(SecurityScanResult scanResult) {
        ScanResult = scanResult;
        AuditTrail.Add(new AuditEntry {
            Action = "Security Scan Updated",
            UserId = Guid.Empty, // System action
            DateTime = DateTimeOffset.UtcNow,
        });
    }

    public void UpdateTrustTier(TrustTier trustTier) {
        TrustTier = trustTier;
        AuditTrail.Add(new AuditEntry {
            Action = $"Trust Tier Updated to {trustTier}",
            UserId = Guid.Empty, // System action
            DateTime = DateTimeOffset.UtcNow,
        });
    }
}