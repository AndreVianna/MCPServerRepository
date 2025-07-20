namespace MCPHub.Storage;

/// <summary>
/// Storage backup settings
/// </summary>
public class StorageBackupSettings {
    public bool EnableBackup { get; set; } = true;
    public StorageBackupType BackupType { get; set; } = StorageBackupType.CrossRegion;
    public string? BackupDestination { get; set; }
    public string? BackupSchedule { get; set; } // Cron expression
    public int BackupRetentionDays { get; set; } = 30;
    public bool EnableGeoReplication { get; set; } = true;
    public List<string> ReplicationRegions { get; set; } = [];
}