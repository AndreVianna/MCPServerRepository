namespace MCPHub.Storage;

/// <summary>
/// Storage lifecycle statistics
/// </summary>
public class StorageLifecycleStatistics {
    public int TotalPolicies { get; set; }
    public int EnabledPolicies { get; set; }
    public DateTimeOffset LastExecutionTime { get; set; }
    public List<StorageLifecyclePolicyStatistics> PolicyStatistics { get; set; } = [];
}