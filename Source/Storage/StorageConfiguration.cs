namespace MCPHub.Storage;

/// <summary>
/// Configuration for storage services
/// </summary>
public class StorageConfiguration {
    public const string ConfigurationKey = "Storage";

    /// <summary>
    /// Note: Cloud storage provider configurations removed - IStorageService is unimplemented
    /// </summary>
    public string ProviderNote { get; set; } = "Storage service interface available but implementations removed";

    /// <summary>
    /// General storage options
    /// </summary>
    public StorageOptions Options { get; set; } = new();

    // Note: Advanced configuration properties removed following contracts-first approach
    // Lifecycle, Security, Monitoring, and Backup settings will be implemented when needed
}