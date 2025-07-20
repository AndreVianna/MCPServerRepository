namespace MCPHub.Storage;

/// <summary>
/// Storage operations for rate limiting
/// </summary>
public enum StorageOperation {
    Upload,
    Download,
    Delete,
    List,
    GetMetadata,
}