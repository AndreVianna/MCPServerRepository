namespace MCPHub.Storage;

/// <summary>
/// Storage lifecycle rule
/// </summary>
public class StorageLifecycleRule {
    public StorageLifecycleAction Action { get; set; }
    public int DaysAfterCreation { get; set; }
    public int DaysAfterModification { get; set; }
    public long? MinimumFileSize { get; set; }
    public long? MaximumFileSize { get; set; }
}