namespace MCPHub.Storage;

/// <summary>
/// Storage security validation result
/// </summary>
public class StorageSecurityValidationResult {
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = [];
}