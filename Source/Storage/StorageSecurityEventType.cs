namespace MCPHub.Storage;

/// <summary>
/// Storage security event types
/// </summary>
public enum StorageSecurityEventType {
    FileUploadValidation,
    FileDownloadValidation,
    VirusScanCompleted,
    AccessDenied,
    RateLimitExceeded,
    SuspiciousActivity
}