namespace MCPHub.Storage;

/// <summary>
/// Storage operation metric
/// </summary>
public class StorageOperationMetric {
    public string OperationName { get; set; } = string.Empty;
    public StorageOperationType OperationType { get; set; }
    public string ContainerName { get; set; } = string.Empty;
    public string? FileName { get; set; }
    public bool IsSuccess { get; set; }
    public TimeSpan ResponseTime { get; set; }
    public long BytesTransferred { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public string? ErrorType { get; set; }
    public string? ErrorMessage { get; set; }
}