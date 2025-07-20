namespace MCPHub.Storage;

public class StorageDisasterRecoveryRequest {
    public DisasterRecoveryScenario Scenario { get; set; }
    public string? ContainerName { get; set; }
    public Dictionary<string, string> Parameters { get; set; } = [];
}
