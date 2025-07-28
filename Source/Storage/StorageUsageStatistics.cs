namespace MCPHub.Storage;

/// <summary>
/// Storage usage statistics
/// </summary>
public class StorageUsageStatistics {
    public DateTimeOffset GeneratedAt { get; set; }
    public long TotalStorageUsed { get; set; }
    public long TotalFileCount { get; set; }
    // Note: Advanced statistics properties removed following contracts-first approach
    // Container statistics will be implemented when needed
}