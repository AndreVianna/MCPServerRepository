namespace MCPHub.Storage;

/// <summary>
/// Storage usage statistics
/// </summary>
public class StorageUsageStatistics {
    public DateTimeOffset GeneratedAt { get; set; }
    public long TotalStorageUsed { get; set; }
    public long TotalFileCount { get; set; }
    public List<StorageContainerStatistics> ContainerStatistics { get; set; } = [];
}