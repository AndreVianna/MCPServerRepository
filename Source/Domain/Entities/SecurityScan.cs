namespace Domain.Entities;

/// <summary>
/// Represents a security scan performed on a server version
/// </summary>
public class SecurityScan {
    public Guid Id { get; set; }
    public Guid ServerVersionId { get; set; }
    public ServerVersion ServerVersion { get; set; } = null!;
    public DateTime ScanStartedAt { get; set; }
    public DateTime? ScanCompletedAt { get; set; }
    public ScanStatus Status { get; set; }
    public SecurityScanResult? Result { get; set; }
    [MaxLength(4096)]
    public string? ErrorMessage { get; set; }
    [MaxLength(32)]
    public string ScannerVersion { get; set; } = string.Empty;
    public Dictionary<string, object> Metadata { get; set; } = [];

    private SecurityScan() { } // For EF Core

    public SecurityScan(
        Guid serverVersionId,
        string scannerVersion) {
        Id = Guid.NewGuid();
        ServerVersionId = serverVersionId;
        ScannerVersion = scannerVersion ?? throw new ArgumentNullException(nameof(scannerVersion));
        ScanStartedAt = DateTime.UtcNow;
        Status = ScanStatus.InProgress;
    }

    public void Complete(SecurityScanResult result) {
        Result = result ?? throw new ArgumentNullException(nameof(result));
        Status = ScanStatus.Completed;
        ScanCompletedAt = DateTime.UtcNow;
        ErrorMessage = null;
    }

    public void Fail(string errorMessage) {
        ErrorMessage = errorMessage ?? throw new ArgumentNullException(nameof(errorMessage));
        Status = ScanStatus.Failed;
        ScanCompletedAt = DateTime.UtcNow;
    }

    public void AddMetadata(string key, object value) => Metadata[key] = value;
}

public enum ScanStatus {
    InProgress,
    Completed,
    Failed,
    Cancelled
}