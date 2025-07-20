namespace MCPHub.Storage;

/// <summary>
/// Storage usage information
/// </summary>
public class StorageUsageInfo {
    public long TotalSize { get; set; }
    public long FileCount { get; set; }
    public string ContainerName { get; set; } = string.Empty;
    public DateTimeOffset LastUpdated { get; set; }
}