namespace MCPHub.Storage;

public class StorageBackupValidationResult {
    public string BackupId { get; set; } = string.Empty;
    public bool IsValid { get; set; }
    public int FileCount { get; set; }
    public long TotalSize { get; set; }
    public List<string> ValidationErrors { get; set; } = [];
    public DateTimeOffset ValidatedAt { get; set; }
}
