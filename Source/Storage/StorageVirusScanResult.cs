namespace MCPHub.Storage;

/// <summary>
/// Storage virus scan result
/// </summary>
public class StorageVirusScanResult {
    public bool IsClean { get; set; }
    public string? ThreatName { get; set; }
    public string? ThreatType { get; set; }
    public string FileName { get; set; } = string.Empty;
    public DateTimeOffset ScanTimestamp { get; set; }
}