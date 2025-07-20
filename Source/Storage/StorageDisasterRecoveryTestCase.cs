namespace MCPHub.Storage;

public class StorageDisasterRecoveryTestCase {
    public string TestName { get; set; } = string.Empty;
    public bool IsSuccess { get; set; }
    public string? BackupId { get; set; }
    public string? ErrorMessage { get; set; }
    public TimeSpan Duration { get; set; }
}
