namespace MCPHub.Storage;

/// <summary>
/// Storage lifecycle policy statistics
/// </summary>
public class StorageLifecyclePolicyStatistics {
    public string PolicyName { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
    public int RuleCount { get; set; }
    public DateTimeOffset LastExecutionTime { get; set; }
    public long FilesProcessed { get; set; }
    public long FilesDeleted { get; set; }
    public long FilesArchived { get; set; }
    public long SpaceReclaimed { get; set; }
}