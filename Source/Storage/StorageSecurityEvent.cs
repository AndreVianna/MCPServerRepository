namespace MCPHub.Storage;

/// <summary>
/// Storage security event
/// </summary>
public class StorageSecurityEvent {
    public StorageSecurityEventType EventType { get; set; }
    public string? ContainerName { get; set; }
    public string? FileName { get; set; }
    public string? ContentType { get; set; }
    public string? ClientIpAddress { get; set; }
    public bool IsSuccess { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public string Details { get; set; } = string.Empty;
}