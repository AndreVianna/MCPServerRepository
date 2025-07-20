namespace MCPHub.Storage;

public class StorageBackupInfo {
    public string BackupId { get; set; } = string.Empty;
    public string ContainerName { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public int FileCount { get; set; }
    public long TotalSize { get; set; }
    public StorageBackupType BackupType { get; set; }
}