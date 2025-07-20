using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MCPHub.Storage;

/// <summary>
/// Storage health status
/// </summary>
public class StorageHealthStatus {
    public DateTimeOffset CheckedAt { get; set; }
    public HealthStatus OverallStatus { get; set; }
    public double SuccessRate { get; set; }
    public double ErrorRate { get; set; }
    public TimeSpan AverageResponseTime { get; set; }
}