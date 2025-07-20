namespace MCPHub.Storage;

/// <summary>
/// Storage monitoring thresholds
/// </summary>
public class StorageThresholds {
    public double HighUsagePercentage { get; set; } = 80.0;
    public double CriticalUsagePercentage { get; set; } = 95.0;
    public int MaxFailedOperationsPerMinute { get; set; } = 10;
    public TimeSpan MaxResponseTime { get; set; } = TimeSpan.FromSeconds(30);
}