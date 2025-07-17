namespace Domain.Entities;

/// <summary>
/// Represents a security scan performed on a server version
/// </summary>
public class SecurityScan
{
    public Guid Id { get; private set; }
    public Guid ServerVersionId { get; private set; }
    public ServerVersion ServerVersion { get; private set; } = null!;
    public DateTime ScanStartedAt { get; private set; }
    public DateTime? ScanCompletedAt { get; private set; }
    public ScanStatus Status { get; private set; }
    public SecurityScanResult? Result { get; private set; }
    public string? ErrorMessage { get; private set; }
    public string ScannerVersion { get; private set; }
    public Dictionary<string, object> Metadata { get; private set; } = new();

    private SecurityScan() { } // For EF Core

    public SecurityScan(
        Guid serverVersionId,
        string scannerVersion)
    {
        Id = Guid.NewGuid();
        ServerVersionId = serverVersionId;
        ScannerVersion = scannerVersion ?? throw new ArgumentNullException(nameof(scannerVersion));
        ScanStartedAt = DateTime.UtcNow;
        Status = ScanStatus.InProgress;
    }

    public void Complete(SecurityScanResult result)
    {
        Result = result ?? throw new ArgumentNullException(nameof(result));
        Status = ScanStatus.Completed;
        ScanCompletedAt = DateTime.UtcNow;
        ErrorMessage = null;
    }

    public void Fail(string errorMessage)
    {
        ErrorMessage = errorMessage ?? throw new ArgumentNullException(nameof(errorMessage));
        Status = ScanStatus.Failed;
        ScanCompletedAt = DateTime.UtcNow;
    }

    public void AddMetadata(string key, object value)
    {
        Metadata[key] = value;
    }
}

public enum ScanStatus
{
    InProgress,
    Completed,
    Failed,
    Cancelled
}