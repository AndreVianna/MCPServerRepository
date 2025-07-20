namespace MCPHub.Storage;

/// <summary>
/// Service for managing storage security and access controls
/// </summary>
public interface IStorageSecurityService {
    /// <summary>
    /// Validates file upload security
    /// </summary>
    Task<StorageSecurityValidationResult> ValidateFileUploadAsync(
        string fileName,
        Stream content,
        string contentType,
        string? clientIpAddress = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates file download security
    /// </summary>
    Task<StorageSecurityValidationResult> ValidateFileDownloadAsync(
        string containerName,
        string fileName,
        string? clientIpAddress = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Encrypts file content
    /// </summary>
    Task<Stream> EncryptContentAsync(Stream content, CancellationToken cancellationToken = default);

    /// <summary>
    /// Decrypts file content
    /// </summary>
    Task<Stream> DecryptContentAsync(Stream encryptedContent, CancellationToken cancellationToken = default);

    /// <summary>
    /// Scans file for malware
    /// </summary>
    Task<StorageVirusScanResult> ScanFileAsync(Stream content, string fileName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks IP address access permissions
    /// </summary>
    Task<bool> IsIpAddressAllowedAsync(string ipAddress, CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs security event
    /// </summary>
    Task LogSecurityEventAsync(StorageSecurityEvent securityEvent, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets access rate limit status
    /// </summary>
    Task<StorageRateLimitStatus> GetRateLimitStatusAsync(string clientIdentifier, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates access rate limit
    /// </summary>
    Task UpdateRateLimitAsync(string clientIdentifier, StorageOperation operation, CancellationToken cancellationToken = default);
}
