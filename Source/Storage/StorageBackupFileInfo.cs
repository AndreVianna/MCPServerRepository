namespace MCPHub.Storage;

public class StorageBackupFileInfo {
    public string FileName { get; set; } = string.Empty;
    public long OriginalSize { get; set; }
    public long CompressedSize { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public DateTime? LastModified { get; set; }
    public string? ETag { get; set; }
    public string BackupFileName { get; set; } = string.Empty;
}