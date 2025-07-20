namespace MCPHub.Storage;

/// <summary>
/// Storage rate limit status
/// </summary>
public class StorageRateLimitStatus {
    public bool IsAllowed { get; set; }
    public int CurrentCount { get; set; }
    public int MaxAllowed { get; set; }
    public DateTimeOffset ResetTime { get; set; }
}