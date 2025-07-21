namespace MCPHub.PublicApi.Middleware;

public class HealthCheckResult {
    public string Status { get; set; } = string.Empty;
    public TimeSpan Duration { get; set; }
    public string? Description { get; set; }
    public IEnumerable<string> Tags { get; set; } = [];
    public string? Exception { get; set; }
    public IReadOnlyDictionary<string, object> Data { get; set; } = new Dictionary<string, object>();
}