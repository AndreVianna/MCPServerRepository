using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

using Common.Models;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Common.Services;

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

/// <summary>
/// Implementation of storage security service
/// </summary>
public class StorageSecurityService : IStorageSecurityService {
    private readonly StorageConfiguration _configuration;
    private readonly ICacheService _cacheService;
    private readonly ILogger<StorageSecurityService> _logger;
    private readonly byte[] _encryptionKey;

    public StorageSecurityService(
        IOptions<StorageConfiguration> configuration,
        ICacheService cacheService,
        ILogger<StorageSecurityService> logger) {
        _configuration = configuration.Value;
        _cacheService = cacheService;
        _logger = logger;

        // In production, this would come from Azure Key Vault or similar
        _encryptionKey = GenerateEncryptionKey(_configuration.Security.EncryptionKey);
    }

    public async Task<StorageSecurityValidationResult> ValidateFileUploadAsync(
        string fileName,
        Stream content,
        string contentType,
        string? clientIpAddress = null,
        CancellationToken cancellationToken = default) {
        var result = new StorageSecurityValidationResult { IsValid = true };

        try {
            // Validate file extension
            if (!IsFileExtensionAllowed(fileName)) {
                result.IsValid = false;
                result.Errors.Add($"File extension not allowed: {Path.GetExtension(fileName)}");
            }

            // Validate file size
            if (content.Length > _configuration.Options.MaxFileSize) {
                result.IsValid = false;
                result.Errors.Add($"File size exceeds maximum allowed size: {content.Length} > {_configuration.Options.MaxFileSize}");
            }

            // Validate content type
            if (!IsContentTypeAllowed(contentType)) {
                result.IsValid = false;
                result.Errors.Add($"Content type not allowed: {contentType}");
            }

            // Validate IP address
            if (!string.IsNullOrEmpty(clientIpAddress) && !await IsIpAddressAllowedAsync(clientIpAddress, cancellationToken)) {
                result.IsValid = false;
                result.Errors.Add($"IP address not allowed: {clientIpAddress}");
            }

            // Scan for malware if enabled
            if (_configuration.Security.EnableVirusScanning) {
                var scanResult = await ScanFileAsync(content, fileName, cancellationToken);
                if (!scanResult.IsClean) {
                    result.IsValid = false;
                    result.Errors.Add($"Malware detected: {scanResult.ThreatName}");
                }
            }

            // Log security event
            await LogSecurityEventAsync(new StorageSecurityEvent {
                EventType = StorageSecurityEventType.FileUploadValidation,
                FileName = fileName,
                ContentType = contentType,
                ClientIpAddress = clientIpAddress,
                IsSuccess = result.IsValid,
                Timestamp = DateTimeOffset.UtcNow,
                Details = result.IsValid ? "Upload validation passed" : string.Join(", ", result.Errors)
            }, cancellationToken);

            return result;
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error validating file upload for {FileName}", fileName);
            result.IsValid = false;
            result.Errors.Add("Internal security validation error");
            return result;
        }
    }

    public async Task<StorageSecurityValidationResult> ValidateFileDownloadAsync(
        string containerName,
        string fileName,
        string? clientIpAddress = null,
        CancellationToken cancellationToken = default) {
        var result = new StorageSecurityValidationResult { IsValid = true };

        try {
            // Validate IP address
            if (!string.IsNullOrEmpty(clientIpAddress) && !await IsIpAddressAllowedAsync(clientIpAddress, cancellationToken)) {
                result.IsValid = false;
                result.Errors.Add($"IP address not allowed: {clientIpAddress}");
            }

            // Check rate limits
            if (!string.IsNullOrEmpty(clientIpAddress)) {
                var rateLimitStatus = await GetRateLimitStatusAsync(clientIpAddress, cancellationToken);
                if (!rateLimitStatus.IsAllowed) {
                    result.IsValid = false;
                    result.Errors.Add($"Rate limit exceeded for IP: {clientIpAddress}");
                }
            }

            // Log security event
            await LogSecurityEventAsync(new StorageSecurityEvent {
                EventType = StorageSecurityEventType.FileDownloadValidation,
                ContainerName = containerName,
                FileName = fileName,
                ClientIpAddress = clientIpAddress,
                IsSuccess = result.IsValid,
                Timestamp = DateTimeOffset.UtcNow,
                Details = result.IsValid ? "Download validation passed" : string.Join(", ", result.Errors)
            }, cancellationToken);

            return result;
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error validating file download for {ContainerName}/{FileName}", containerName, fileName);
            result.IsValid = false;
            result.Errors.Add("Internal security validation error");
            return result;
        }
    }

    public async Task<Stream> EncryptContentAsync(Stream content, CancellationToken cancellationToken = default) {
        try {
            if (!_configuration.Security.EnableEncryptionAtRest) {
                return content;
            }

            using var aes = Aes.Create();
            aes.Key = _encryptionKey;
            aes.GenerateIV();

            var encryptedStream = new MemoryStream();

            // Write IV to the beginning of the encrypted stream
            await encryptedStream.WriteAsync(aes.IV, 0, aes.IV.Length, cancellationToken);

            using var cryptoStream = new CryptoStream(encryptedStream, aes.CreateEncryptor(), CryptoStreamMode.Write);
            await content.CopyToAsync(cryptoStream, cancellationToken);
            await cryptoStream.FlushFinalBlockAsync();

            encryptedStream.Position = 0;
            return encryptedStream;
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error encrypting content");
            throw;
        }
    }

    public async Task<Stream> DecryptContentAsync(Stream encryptedContent, CancellationToken cancellationToken = default) {
        try {
            if (!_configuration.Security.EnableEncryptionAtRest) {
                return encryptedContent;
            }

            using var aes = Aes.Create();
            aes.Key = _encryptionKey;

            // Read IV from the beginning of the encrypted stream
            var iv = new byte[aes.IV.Length];
            await encryptedContent.ReadExactlyAsync(iv, cancellationToken);
            aes.IV = iv;

            var decryptedStream = new MemoryStream();

            using var cryptoStream = new CryptoStream(encryptedContent, aes.CreateDecryptor(), CryptoStreamMode.Read);
            await cryptoStream.CopyToAsync(decryptedStream, cancellationToken);

            decryptedStream.Position = 0;
            return decryptedStream;
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error decrypting content");
            throw;
        }
    }

    public async Task<StorageVirusScanResult> ScanFileAsync(Stream content, string fileName, CancellationToken cancellationToken = default) {
        try {
            // In a real implementation, this would integrate with a virus scanning service
            // For now, we'll implement a basic signature-based scan

            var result = new StorageVirusScanResult {
                IsClean = true,
                ScanTimestamp = DateTimeOffset.UtcNow,
                FileName = fileName
            };

            // Reset stream position
            content.Position = 0;

            // Check for suspicious patterns
            var buffer = new byte[1024];
            var bytesRead = await content.ReadAsync(buffer, 0, buffer.Length, cancellationToken);

            if (bytesRead > 0) {
                var content_string = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                // Basic malware signatures (this is just for demonstration)
                var malwareSignatures = new[]
                {
                    "EICAR-STANDARD-ANTIVIRUS-TEST-FILE",
                    "X5O!P%@AP[4\\PZX54(P^)7CC)7}$EICAR-STANDARD-ANTIVIRUS-TEST-FILE!$H+H*",
                    "eval(",
                    "javascript:",
                    "<script>"
                };

                foreach (var signature in malwareSignatures) {
                    if (content_string.Contains(signature, StringComparison.OrdinalIgnoreCase)) {
                        result.IsClean = false;
                        result.ThreatName = "Suspicious content detected";
                        result.ThreatType = "Potential malware";
                        break;
                    }
                }
            }

            // Reset stream position
            content.Position = 0;

            return result;
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error scanning file {FileName}", fileName);
            return new StorageVirusScanResult {
                IsClean = false,
                ThreatName = "Scan error",
                ThreatType = "Unknown",
                FileName = fileName,
                ScanTimestamp = DateTimeOffset.UtcNow
            };
        }
    }

    public async Task<bool> IsIpAddressAllowedAsync(string ipAddress, CancellationToken cancellationToken = default) {
        try {
            var security = _configuration.Security;

            // Check blocked IPs first
            if (security.BlockedIpAddresses.Any(blocked => IsIpInRange(ipAddress, blocked))) {
                return false;
            }

            // If allowed IPs are specified, check if IP is in the allowed list
            if (security.AllowedIpAddresses.Count > 0) {
                return security.AllowedIpAddresses.Any(allowed => IsIpInRange(ipAddress, allowed));
            }

            // If no specific allowed IPs, allow all (except blocked ones)
            return true;
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error checking IP address permission for {IpAddress}", ipAddress);
            return false;
        }
    }

    public async Task LogSecurityEventAsync(StorageSecurityEvent securityEvent, CancellationToken cancellationToken = default) {
        try {
            if (!_configuration.Security.EnableAccessLogging) {
                return;
            }

            var logEntry = JsonSerializer.Serialize(securityEvent, new JsonSerializerOptions {
                WriteIndented = false
            });

            _logger.LogInformation("Security Event: {LogEntry}", logEntry);

            // In a real implementation, this would also store to a security log database
            // and potentially send alerts for critical events
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error logging security event");
        }
    }

    public async Task<StorageRateLimitStatus> GetRateLimitStatusAsync(string clientIdentifier, CancellationToken cancellationToken = default) {
        try {
            var key = $"rate_limit:{clientIdentifier}";
            var currentCount = await _cacheService.GetAsync<int?>(key, cancellationToken) ?? 0;
            var maxAllowed = _configuration.Security.MaxDownloadAttemptsPerHour;

            return new StorageRateLimitStatus {
                IsAllowed = currentCount < maxAllowed,
                CurrentCount = currentCount,
                MaxAllowed = maxAllowed,
                ResetTime = DateTimeOffset.UtcNow.AddHours(1)
            };
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error getting rate limit status for {ClientIdentifier}", clientIdentifier);
            return new StorageRateLimitStatus { IsAllowed = false };
        }
    }

    public async Task UpdateRateLimitAsync(string clientIdentifier, StorageOperation operation, CancellationToken cancellationToken = default) {
        try {
            var key = $"rate_limit:{clientIdentifier}";
            var currentCount = await _cacheService.GetAsync<int?>(key, cancellationToken) ?? 0;

            await _cacheService.SetAsync(key, currentCount + 1, TimeSpan.FromHours(1), cancellationToken);
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error updating rate limit for {ClientIdentifier}", clientIdentifier);
        }
    }

    private bool IsFileExtensionAllowed(string fileName) {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();

        // Check blocked extensions first
        if (_configuration.Options.BlockedFileExtensions.Any(blocked =>
            blocked.Equals(extension, StringComparison.OrdinalIgnoreCase))) {
            return false;
        }

        // If allowed extensions are specified, check if extension is in the allowed list
        if (_configuration.Options.AllowedFileExtensions.Count > 0) {
            return _configuration.Options.AllowedFileExtensions.Any(allowed =>
                allowed.Equals(extension, StringComparison.OrdinalIgnoreCase));
        }

        // If no specific allowed extensions, allow all (except blocked ones)
        return true;
    }

    private static bool IsContentTypeAllowed(string contentType) {
        // Basic content type validation
        var allowedTypes = new[]
        {
            "application/json",
            "application/xml",
            "application/zip",
            "application/pdf",
            "text/plain",
            "text/csv",
            "image/jpeg",
            "image/png",
            "image/gif",
            "image/webp"
        };

        return allowedTypes.Any(allowed => contentType.StartsWith(allowed, StringComparison.OrdinalIgnoreCase));
    }

    private static bool IsIpInRange(string ipAddress, string range) {
        try {
            // Simple IP matching - in production, this would support CIDR notation
            if (range.Contains('/')) {
                // CIDR notation support would be implemented here
                return false;
            }

            return ipAddress.Equals(range, StringComparison.OrdinalIgnoreCase);
        }
        catch {
            return false;
        }
    }

    private static byte[] GenerateEncryptionKey(string? keyString) {
        if (!string.IsNullOrEmpty(keyString)) {
            return Encoding.UTF8.GetBytes(keyString.PadRight(32)[..32]);
        }

        // Generate a random key if none provided (in production, this would be managed externally)
        using var rng = RandomNumberGenerator.Create();
        var key = new byte[32];
        rng.GetBytes(key);
        return key;
    }
}

/// <summary>
/// Storage security validation result
/// </summary>
public class StorageSecurityValidationResult {
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = [];
}

/// <summary>
/// Storage virus scan result
/// </summary>
public class StorageVirusScanResult {
    public bool IsClean { get; set; }
    public string? ThreatName { get; set; }
    public string? ThreatType { get; set; }
    public string FileName { get; set; } = string.Empty;
    public DateTimeOffset ScanTimestamp { get; set; }
}

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

/// <summary>
/// Storage rate limit status
/// </summary>
public class StorageRateLimitStatus {
    public bool IsAllowed { get; set; }
    public int CurrentCount { get; set; }
    public int MaxAllowed { get; set; }
    public DateTimeOffset ResetTime { get; set; }
}

/// <summary>
/// Storage operations for rate limiting
/// </summary>
public enum StorageOperation {
    Upload,
    Download,
    Delete,
    List,
    GetMetadata
}