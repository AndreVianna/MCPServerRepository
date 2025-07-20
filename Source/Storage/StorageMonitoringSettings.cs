namespace MCPHub.Storage;

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