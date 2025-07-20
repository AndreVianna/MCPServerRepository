namespace MCPHub.Storage;

public class StorageDisasterRecoveryResult {
    public DisasterRecoveryScenario Scenario { get; set; }
    public bool IsSuccess { get; set; }
    public DateTimeOffset StartedAt { get; set; }
    public DateTimeOffset CompletedAt { get; set; }
    public TimeSpan Duration { get; set; }
    public List<string> RecoveryActions { get; set; } = [];
    public string? ErrorMessage { get; set; }
}
