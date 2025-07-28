using System.Diagnostics.CodeAnalysis;

using MCPHub.Domain.Common;
using MCPHub.Domain.ValueObjects;

namespace MCPHub.Domain.Entities;

/// <summary>
/// Represents a security scan performed on a server version or package version
/// </summary>
public class SecurityScan : BaseEntity {
    public Guid VersionId { get; set; }
    
    // Server-specific properties (nullable for package scans)
    public Guid? ServerVersionId { get; set; }
    public ServerVersion? ServerVersion { get; set; }
    
    // Package-specific properties (nullable for server scans)
    public Guid? PackageVersionId { get; set; }
    public PackageVersion? PackageVersion { get; set; }
    
    // Common scan properties
    public ScanType ScanType { get; set; }
    public DateTime ScanStartedAt { get; set; }
    public DateTime? ScanCompletedAt { get; set; }
    public ScanStatus Status { get; set; } = ScanStatus.Pending;
    public SecurityScanResult? Result { get; set; }
    public int CriticalIssues { get; set; } = 0;
    [MaxLength(4096)]
    public string? ErrorMessage { get; set; }
    [MaxLength(32)]
    public string ScannerVersion { get; set; } = string.Empty;
    public Dictionary<string, object> Metadata { get; set; } = [];
    
    /// <summary>
    /// Type of entity being scanned (Server or Package)
    /// </summary>
    [MaxLength(16)]
    public string EntityType { get; set; } = string.Empty;

    private SecurityScan() { } // For EF Core

    /// <summary>
    /// Creates a security scan for a server version
    /// </summary>
    [SetsRequiredMembers]
    public SecurityScan(
        Guid serverVersionId,
        ScanType scanType,
        string scannerVersion) {
        ArgumentException.ThrowIfNullOrWhiteSpace(scannerVersion);

        VersionId = serverVersionId;
        ServerVersionId = serverVersionId;
        EntityType = "Server";
        ScanType = scanType;
        ScannerVersion = scannerVersion;
        ScanStartedAt = DateTime.UtcNow;
        Status = ScanStatus.InProgress;
        CriticalIssues = 0;
        AuditTrail.Add(new AuditEntry {
            Action = $"Security Scan Started ({scanType})",
            UserId = Guid.Empty, // System action
            DateTime = DateTimeOffset.UtcNow
        });
    }
    
    /// <summary>
    /// Creates a security scan for a package version
    /// </summary>
    [SetsRequiredMembers]
    public SecurityScan(
        Guid packageVersionId,
        ScanType scanType,
        string scannerVersion,
        bool isPackageScan) {
        ArgumentException.ThrowIfNullOrWhiteSpace(scannerVersion);

        VersionId = packageVersionId;
        PackageVersionId = packageVersionId;
        EntityType = "Package";
        ScanType = scanType;
        ScannerVersion = scannerVersion;
        ScanStartedAt = DateTime.UtcNow;
        Status = ScanStatus.InProgress;
        CriticalIssues = 0;
        AuditTrail.Add(new AuditEntry {
            Action = $"Package Security Scan Started ({scanType})",
            UserId = Guid.Empty, // System action
            DateTime = DateTimeOffset.UtcNow
        });
    }

    public void Complete(SecurityScanResult result) {
        ArgumentNullException.ThrowIfNull(result);

        Result = result;
        Status = ScanStatus.Completed;
        ScanCompletedAt = DateTime.UtcNow;
        ErrorMessage = null;
        AuditTrail.Add(new AuditEntry {
            Action = "Security Scan Completed",
            UserId = Guid.Empty, // System action
            DateTime = DateTimeOffset.UtcNow
        });
    }

    public void Fail(string errorMessage) {
        ArgumentException.ThrowIfNullOrWhiteSpace(errorMessage);

        ErrorMessage = errorMessage;
        Status = ScanStatus.Failed;
        ScanCompletedAt = DateTime.UtcNow;
        AuditTrail.Add(new AuditEntry {
            Action = "Security Scan Failed",
            UserId = Guid.Empty, // System action
            DateTime = DateTimeOffset.UtcNow
        });
    }

    public void AddMetadata(string key, object value) => Metadata[key] = value;
}

