namespace MCPHub.Storage;

/// <summary>
/// Storage file metadata
/// </summary>
public class StorageFileMetadata {
    public string FileName { get; set; } = string.Empty;
    public long Size { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public string? ETag { get; set; }
    public DateTime? LastModified { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = [];
}