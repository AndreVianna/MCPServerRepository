namespace MCPHub.Storage;

public class StorageBackupResult {
    public bool IsSuccess { get; set; }
    public string BackupId { get; set; } = string.Empty;
    public string ContainerName { get; set; } = string.Empty;
    public int FileCount { get; set; }
    public long TotalSize { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public string? ErrorMessage { get; set; }
}