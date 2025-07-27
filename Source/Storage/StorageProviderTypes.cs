namespace MCPHub.Storage;

/// <summary>
/// Storage provider types for technology progression
/// </summary>
public enum StorageProviderType
{
    LocalFile,    // Development tier
    AzureBlob,    // Production tier
    AWSS3,        // Production tier alternative
    MultiRegion   // Enterprise tier
}

/// <summary>
/// Storage service health information
/// </summary>
public record StorageHealthInfo(
    bool IsHealthy,
    TimeSpan ResponseTime,
    long TotalStorage,
    long UsedStorage,
    double AvailabilityPercentage,
    string? ErrorMessage = null);

/// <summary>
/// Storage performance metrics
/// </summary>
public record StoragePerformanceMetrics(
    double AverageUploadTime,
    double AverageDownloadTime,
    long OperationsPerSecond,
    double ThroughputMBps,
    double ErrorRate,
    DateTime Timestamp);