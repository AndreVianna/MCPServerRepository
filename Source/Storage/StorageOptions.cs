namespace MCPHub.Storage;

/// <summary>
/// General storage options
/// </summary>
public class StorageOptions {
    public long MaxFileSize { get; set; } = 100 * 1024 * 1024; // 100MB
    public int MaxConcurrentOperations { get; set; } = 10;
    public bool EnableCompression { get; set; } = true;
    public List<string> AllowedFileExtensions { get; set; } = [];
    public List<string> BlockedFileExtensions { get; set; } = [];
    public TimeSpan DefaultPresignedUrlExpiration { get; set; } = TimeSpan.FromHours(1);
    public bool EnableVersioning { get; set; } = true;
    public int MaxVersionsToKeep { get; set; } = 10;
}