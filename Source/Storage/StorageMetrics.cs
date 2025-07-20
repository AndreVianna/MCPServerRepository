namespace MCPHub.Storage;

/// <summary>
/// Storage metrics aggregate
/// </summary>
public class StorageMetrics {
    public TimeSpan Period { get; set; }
    public DateTimeOffset GeneratedAt { get; set; }
    public int TotalOperations { get; set; }
    public int SuccessfulOperations { get; set; }
    public int FailedOperations { get; set; }
    public long TotalBytesTransferred { get; set; }
    public TimeSpan AverageResponseTime { get; set; }
    public Dictionary<StorageOperationType, int> OperationsByType { get; set; } = [];
    public Dictionary<string, int> ErrorsByType { get; set; } = [];
    public double SuccessRate => TotalOperations > 0 ? (double)SuccessfulOperations / TotalOperations : 1.0;
    public double ErrorRate => TotalOperations > 0 ? (double)FailedOperations / TotalOperations : 0.0;
}