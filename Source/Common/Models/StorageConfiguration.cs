namespace Common.Models;

/// <summary>
/// Configuration for storage services
/// </summary>
public class StorageConfiguration {
    public const string ConfigurationKey = "Storage";

    /// <summary>
    /// Storage provider type
    /// </summary>
    public StorageProviderType Provider { get; set; } = StorageProviderType.AzureBlob;

    /// <summary>
    /// Azure Blob Storage configuration
    /// </summary>
    public AzureBlobStorageConfiguration AzureBlob { get; set; } = new();

    /// <summary>
    /// AWS S3 configuration
    /// </summary>
    public AwsS3StorageConfiguration AwsS3 { get; set; } = new();

    /// <summary>
    /// S3-compatible storage configuration (MinIO, etc.)
    /// </summary>
    public S3CompatibleStorageConfiguration S3Compatible { get; set; } = new();

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

/// <summary>
/// Storage provider types
/// </summary>
public enum StorageProviderType {
    AzureBlob,
    AwsS3,
    S3Compatible
}

/// <summary>
/// Azure Blob Storage configuration
/// </summary>
public class AzureBlobStorageConfiguration {
    public string ConnectionString { get; set; } = string.Empty;
    public string StorageAccountName { get; set; } = string.Empty;
    public string StorageAccountKey { get; set; } = string.Empty;
    public string? ManagedIdentityClientId { get; set; }
    public bool UseManagedIdentity { get; set; } = true;
    public string DefaultContainer { get; set; } = "mcphub";
    public int MaxRetryAttempts { get; set; } = 3;
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(1);
}

/// <summary>
/// AWS S3 configuration
/// </summary>
public class AwsS3StorageConfiguration {
    public string AccessKeyId { get; set; } = string.Empty;
    public string SecretAccessKey { get; set; } = string.Empty;
    public string? SessionToken { get; set; }
    public string Region { get; set; } = "us-east-1";
    public string DefaultBucket { get; set; } = "mcphub";
    public bool UseInstanceProfile { get; set; } = true;
    public string? ProfileName { get; set; }
    public int MaxRetryAttempts { get; set; } = 3;
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(1);
}

/// <summary>
/// S3-compatible storage configuration
/// </summary>
public class S3CompatibleStorageConfiguration {
    public string ServiceUrl { get; set; } = string.Empty;
    public string AccessKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string Region { get; set; } = "us-east-1";
    public string DefaultBucket { get; set; } = "mcphub";
    public bool UseHttps { get; set; } = true;
    public int MaxRetryAttempts { get; set; } = 3;
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(1);
}

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

/// <summary>
/// Storage lifecycle actions
/// </summary>
public enum StorageLifecycleAction {
    Delete,
    Archive,
    MoveToStorageClass,
    Compress
}

/// <summary>
/// Storage security settings
/// </summary>
public class StorageSecuritySettings {
    public bool EnableEncryptionAtRest { get; set; } = true;
    public bool EnableEncryptionInTransit { get; set; } = true;
    public string? EncryptionKey { get; set; }
    public string? KeyVaultUrl { get; set; }
    public List<string> AllowedIpAddresses { get; set; } = [];
    public List<string> BlockedIpAddresses { get; set; } = [];
    public bool EnableAccessLogging { get; set; } = true;
    public bool EnableVirusScanning { get; set; } = true;
    public int MaxDownloadAttemptsPerHour { get; set; } = 100;
}

/// <summary>
/// Storage monitoring settings
/// </summary>
public class StorageMonitoringSettings {
    public bool EnableMetrics { get; set; } = true;
    public bool EnableHealthChecks { get; set; } = true;
    public TimeSpan MetricsInterval { get; set; } = TimeSpan.FromMinutes(5);
    public TimeSpan HealthCheckInterval { get; set; } = TimeSpan.FromMinutes(1);
    public List<string> AlertRecipients { get; set; } = [];
    public StorageThresholds Thresholds { get; set; } = new();
}

/// <summary>
/// Storage monitoring thresholds
/// </summary>
public class StorageThresholds {
    public double HighUsagePercentage { get; set; } = 80.0;
    public double CriticalUsagePercentage { get; set; } = 95.0;
    public int MaxFailedOperationsPerMinute { get; set; } = 10;
    public TimeSpan MaxResponseTime { get; set; } = TimeSpan.FromSeconds(30);
}

/// <summary>
/// Storage backup settings
/// </summary>
public class StorageBackupSettings {
    public bool EnableBackup { get; set; } = true;
    public StorageBackupType BackupType { get; set; } = StorageBackupType.CrossRegion;
    public string? BackupDestination { get; set; }
    public string? BackupSchedule { get; set; } // Cron expression
    public int BackupRetentionDays { get; set; } = 30;
    public bool EnableGeoReplication { get; set; } = true;
    public List<string> ReplicationRegions { get; set; } = [];
}

/// <summary>
/// Storage backup types
/// </summary>
public enum StorageBackupType {
    Local,
    CrossRegion,
    CrossProvider
}