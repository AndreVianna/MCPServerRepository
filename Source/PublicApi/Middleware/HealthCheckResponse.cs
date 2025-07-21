namespace MCPHub.PublicApi.Middleware;

public class HealthCheckResponse {
    public string Status { get; set; } = string.Empty;
    public TimeSpan Duration { get; set; }
    public DateTime Timestamp { get; set; }
    public Dictionary<string, HealthCheckResult> Results { get; set; } = [];
}