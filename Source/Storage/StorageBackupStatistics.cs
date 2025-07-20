namespace MCPHub.Storage;

public class StorageBackupStatistics {
    public int TotalBackups { get; set; }
    public long TotalBackupSize { get; set; }
    public Dictionary<string, int> BackupsByContainer { get; set; } = [];
    public DateTimeOffset? OldestBackup { get; set; }
    public DateTimeOffset? NewestBackup { get; set; }
    public DateTimeOffset GeneratedAt { get; set; }
}
