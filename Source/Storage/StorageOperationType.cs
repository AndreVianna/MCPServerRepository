namespace MCPHub.Storage;

/// <summary>
/// Storage operation types
/// </summary>
public enum StorageOperationType {
    Upload,
    Download,
    Delete,
    List,
    GetMetadata,
    Copy,
    Exists,
    Unknown,
}