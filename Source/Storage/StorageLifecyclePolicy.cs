namespace MCPHub.Storage;

/// <summary>
/// Storage lifecycle policy
/// </summary>
public class StorageLifecyclePolicy {
    public string Name { get; set; } = string.Empty;
    public string ContainerPattern { get; set; } = string.Empty;
    public string? FilePattern { get; set; }
    public List<StorageLifecycleRule> Rules { get; set; } = [];
    public bool IsEnabled { get; set; } = true;
}