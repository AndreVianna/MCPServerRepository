namespace MCPHub.Storage;

public class StorageRestoreResult {
    public bool IsSuccess { get; set; }
    public string BackupId { get; set; } = string.Empty;
    public string TargetContainerName { get; set; } = string.Empty;
    public int RestoredFileCount { get; set; }
    public long RestoredBytes { get; set; }
    public DateTimeOffset RestoredAt { get; set; }
    public string? ErrorMessage { get; set; }
}