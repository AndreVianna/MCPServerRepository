namespace MCPHub.Storage;

/// <summary>
/// Storage lifecycle actions
/// </summary>
public enum StorageLifecycleAction {
    Delete,
    Archive,
    MoveToStorageClass,
    Compress
}