namespace MCPHub.Storage;

/// <summary>
/// Storage threshold alert
/// </summary>
public class StorageThresholdAlert {
    public StorageAlertType AlertType { get; set; }
    public string Message { get; set; } = string.Empty;
    public AlertSeverity Severity { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public double Value { get; set; }
    public double Threshold { get; set; }
}