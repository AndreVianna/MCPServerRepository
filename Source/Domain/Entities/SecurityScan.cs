using System.Diagnostics.CodeAnalysis;

using MCPHub.Domain.Common;
using MCPHub.Domain.ValueObjects;

namespace MCPHub.Domain.Entities;

/// <summary>
/// Represents a security scan performed on a server version
/// </summary>
public class SecurityScan : BaseEntity {
    public Guid VersionId { get; set; }
    public Guid ServerVersionId { get; set; }
    public ServerVersion ServerVersion { get; set; } = null!;
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

    private SecurityScan() { } // For EF Core

    [SetsRequiredMembers]
    public SecurityScan(
        Guid serverVersionId,
        ScanType scanType,
        string scannerVersion) {
        ArgumentException.ThrowIfNullOrWhiteSpace(scannerVersion);

        VersionId = serverVersionId;
        ServerVersionId = serverVersionId;
        ScanType = scanType;
        ScannerVersion = scannerVersion;
        ScanStartedAt = DateTime.UtcNow;
        Status = ScanStatus.InProgress;
        CriticalIssues = 0;
    }

    public void Complete(SecurityScanResult result) {
        ArgumentNullException.ThrowIfNull(result);

        Result = result;
        Status = ScanStatus.Completed;
        ScanCompletedAt = DateTime.UtcNow;
        ErrorMessage = null;
        SetUpdatedBy(UpdatedBy ?? "system");
    }

    public void Fail(string errorMessage) {
        ArgumentException.ThrowIfNullOrWhiteSpace(errorMessage);

        ErrorMessage = errorMessage;
        Status = ScanStatus.Failed;
        ScanCompletedAt = DateTime.UtcNow;
        SetUpdatedBy(UpdatedBy ?? "system");
    }

    public void AddMetadata(string key, object value) => Metadata[key] = value;
}

public enum ScanStatus {
    Pending,
    InProgress,
    Completed,
    Failed,
    Cancelled
}