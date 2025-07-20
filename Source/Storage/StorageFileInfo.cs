namespace MCPHub.Storage;

/// <summary>
/// Storage file information
/// </summary>
public class StorageFileInfo {
    public string FileName { get; set; } = string.Empty;
    public long Size { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public DateTimeOffset LastModified { get; set; }
    public string? ETag { get; set; }
    public bool IsDirectory { get; set; }
}