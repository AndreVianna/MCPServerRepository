namespace MCPHub.Storage;

public class StorageBackupManifest {
    public string BackupId { get; set; } = string.Empty;
    public string ContainerName { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public StorageBackupType BackupType { get; set; }
    public int FileCount { get; set; }
    public long TotalSize { get; set; }
    public List<StorageBackupFileInfo> Files { get; set; } = [];
}