namespace MCPHub.Storage;

public class StorageDisasterRecoveryTestResult {
    public bool OverallSuccess { get; set; }
    public DateTimeOffset TestStartedAt { get; set; }
    public DateTimeOffset TestCompletedAt { get; set; }
    public TimeSpan Duration { get; set; }
    public List<StorageDisasterRecoveryTestCase> TestResults { get; set; } = [];
    public string? ErrorMessage { get; set; }
}
