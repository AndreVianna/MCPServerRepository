namespace MCPHub.Storage;

/// <summary>
/// Storage container statistics
/// </summary>
public class StorageContainerStatistics {
    public string ContainerName { get; set; } = string.Empty;
    public long StorageUsed { get; set; }
    public long FileCount { get; set; }
    public DateTimeOffset LastModified { get; set; }
}