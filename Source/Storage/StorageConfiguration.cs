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

    /// <summary>
    /// Lifecycle policies
    /// </summary>
    public List<StorageLifecyclePolicy> LifecyclePolicies { get; set; } = [];

    /// <summary>
    /// Security settings
    /// </summary>
    public StorageSecuritySettings Security { get; set; } = new();

    /// <summary>
    /// Monitoring and metrics settings
    /// </summary>
    public StorageMonitoringSettings Monitoring { get; set; } = new();

    /// <summary>
    /// Backup and disaster recovery settings
    /// </summary>
    public StorageBackupSettings Backup { get; set; } = new();
}