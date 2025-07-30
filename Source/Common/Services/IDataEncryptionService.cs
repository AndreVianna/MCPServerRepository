namespace MCPHub.Common.Services;

/// <summary>
/// Data encryption service for comprehensive data protection at rest and in transit
/// Supports: Basic Encryption → Key Rotation → Hardware Security Modules → Enterprise Key Management
/// </summary>
public interface IDataEncryptionService {
    /// <summary>
    /// Encrypts data using the specified encryption options
    /// </summary>
    /// <param name="data">Data to encrypt</param>
    /// <param name="options">Encryption options</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Encryption result</returns>
    Task<EncryptionResult> EncryptAsync(byte[] data, EncryptionOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Encrypts text data using the specified encryption options
    /// </summary>
    /// <param name="text">Text to encrypt</param>
    /// <param name="options">Encryption options</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Text encryption result</returns>
    Task<TextEncryptionResult> EncryptTextAsync(string text, EncryptionOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Decrypts data using the specified decryption options
    /// </summary>
    /// <param name="encryptedData">Encrypted data to decrypt</param>
    /// <param name="options">Decryption options</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Decryption result</returns>
    Task<DecryptionResult> DecryptAsync(byte[] encryptedData, DecryptionOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Decrypts text data using the specified decryption options
    /// </summary>
    /// <param name="encryptedText">Encrypted text to decrypt</param>
    /// <param name="options">Decryption options</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Text decryption result</returns>
    Task<TextDecryptionResult> DecryptTextAsync(string encryptedText, DecryptionOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a new encryption key
    /// </summary>
    /// <param name="keyType">Type of key to generate</param>
    /// <param name="keySize">Key size in bits</param>
    /// <param name="metadata">Optional key metadata</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Key generation result</returns>
    Task<KeyGenerationResult> GenerateKeyAsync(
        EncryptionKeyType keyType,
        int keySize = 256,
        IDictionary<string, string>? metadata = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Rotates an existing encryption key
    /// </summary>
    /// <param name="keyId">Key identifier to rotate</param>
    /// <param name="rotationOptions">Key rotation options</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Key rotation result</returns>
    Task<KeyRotationResult> RotateKeyAsync(string keyId, KeyRotationOptions rotationOptions, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves key information (without the actual key material)
    /// </summary>
    /// <param name="keyId">Key identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Key information or null if not found</returns>
    Task<EncryptionKeyInfo?> GetKeyInfoAsync(string keyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists all available encryption keys
    /// </summary>
    /// <param name="keyType">Optional key type filter</param>
    /// <param name="includeDisabled">Whether to include disabled keys</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of encryption keys</returns>
    Task<IReadOnlyList<EncryptionKeyInfo>> ListKeysAsync(
        EncryptionKeyType? keyType = null,
        bool includeDisabled = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Disables an encryption key
    /// </summary>
    /// <param name="keyId">Key identifier</param>
    /// <param name="reason">Reason for disabling</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Key disable result</returns>
    Task<KeyDisableResult> DisableKeyAsync(string keyId, string reason, CancellationToken cancellationToken = default);

    /// <summary>
    /// Enables a previously disabled encryption key
    /// </summary>
    /// <param name="keyId">Key identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Key enable result</returns>
    Task<KeyEnableResult> EnableKeyAsync(string keyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a data encryption key (DEK) encrypted with a key encryption key (KEK)
    /// </summary>
    /// <param name="kekId">Key encryption key identifier</param>
    /// <param name="dekOptions">Data encryption key options</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Data encryption key result</returns>
    Task<DataEncryptionKeyResult> CreateDataEncryptionKeyAsync(
        string kekId,
        DataEncryptionKeyOptions dekOptions,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Decrypts a data encryption key using a key encryption key
    /// </summary>
    /// <param name="encryptedDek">Encrypted data encryption key</param>
    /// <param name="kekId">Key encryption key identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Decrypted data encryption key</returns>
    Task<DecryptedDataEncryptionKeyResult> DecryptDataEncryptionKeyAsync(
        byte[] encryptedDek,
        string kekId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Encrypts data for field-level encryption
    /// </summary>
    /// <param name="fieldValue">Field value to encrypt</param>
    /// <param name="fieldName">Name of the field being encrypted</param>
    /// <param name="entityType">Type of entity containing the field</param>
    /// <param name="options">Field encryption options</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Field encryption result</returns>
    Task<FieldEncryptionResult> EncryptFieldAsync(
        string fieldValue,
        string fieldName,
        string entityType,
        FieldEncryptionOptions options,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Decrypts data for field-level encryption
    /// </summary>
    /// <param name="encryptedValue">Encrypted field value</param>
    /// <param name="fieldName">Name of the field being decrypted</param>
    /// <param name="entityType">Type of entity containing the field</param>
    /// <param name="options">Field decryption options</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Field decryption result</returns>
    Task<FieldDecryptionResult> DecryptFieldAsync(
        string encryptedValue,
        string fieldName,
        string entityType,
        FieldDecryptionOptions options,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Encrypts a file for secure storage
    /// </summary>
    /// <param name="filePath">Path to file to encrypt</param>
    /// <param name="options">File encryption options</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>File encryption result</returns>
    Task<FileEncryptionResult> EncryptFileAsync(string filePath, FileEncryptionOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Decrypts a file from secure storage
    /// </summary>
    /// <param name="encryptedFilePath">Path to encrypted file</param>
    /// <param name="options">File decryption options</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>File decryption result</returns>
    Task<FileDecryptionResult> DecryptFileAsync(string encryptedFilePath, FileDecryptionOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets encryption service health and status
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Service health information</returns>
    Task<EncryptionServiceHealth> GetServiceHealthAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets encryption usage statistics
    /// </summary>
    /// <param name="timeRange">Time range for statistics</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Encryption usage statistics</returns>
    Task<EncryptionUsageStatistics> GetUsageStatisticsAsync(DateTimeRange timeRange, CancellationToken cancellationToken = default);
}

/// <summary>
/// Encryption options
/// </summary>
public record EncryptionOptions(
    string? KeyId = null,
    EncryptionAlgorithm Algorithm = EncryptionAlgorithm.AES256_GCM,
    IDictionary<string, string>? Context = null,
    bool UseCompression = false);

/// <summary>
/// Encryption result
/// </summary>
public record EncryptionResult(
    byte[] EncryptedData,
    string KeyId,
    EncryptionAlgorithm Algorithm,
    byte[]? InitializationVector = null,
    byte[]? AuthenticationTag = null,
    bool Success = true,
    string? ErrorMessage = null);

/// <summary>
/// Text encryption result
/// </summary>
public record TextEncryptionResult(
    string EncryptedText,
    string KeyId,
    EncryptionAlgorithm Algorithm,
    bool Success = true,
    string? ErrorMessage = null);

/// <summary>
/// Decryption options
/// </summary>
public record DecryptionOptions(
    string? KeyId = null,
    IDictionary<string, string>? Context = null);

/// <summary>
/// Decryption result
/// </summary>
public record DecryptionResult(
    byte[] DecryptedData,
    bool Success = true,
    string? ErrorMessage = null);

/// <summary>
/// Text decryption result
/// </summary>
public record TextDecryptionResult(
    string DecryptedText,
    bool Success = true,
    string? ErrorMessage = null);

/// <summary>
/// Key generation result
/// </summary>
public record KeyGenerationResult(
    string KeyId,
    EncryptionKeyType KeyType,
    int KeySize,
    DateTimeOffset CreatedAt,
    bool Success = true,
    string? ErrorMessage = null);

/// <summary>
/// Key rotation options
/// </summary>
public record KeyRotationOptions(
    bool RetireOldKey = true,
    DateTimeOffset? RetirementDate = null,
    IDictionary<string, string>? NewKeyMetadata = null);

/// <summary>
/// Key rotation result
/// </summary>
public record KeyRotationResult(
    string NewKeyId,
    string OldKeyId,
    DateTimeOffset RotatedAt,
    bool Success = true,
    string? ErrorMessage = null);

/// <summary>
/// Encryption key information
/// </summary>
public record EncryptionKeyInfo(
    string KeyId,
    EncryptionKeyType KeyType,
    EncryptionAlgorithm Algorithm,
    int KeySize,
    KeyStatus Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset? DisabledAt,
    DateTimeOffset? ExpiresAt,
    IDictionary<string, string> Metadata,
    long UsageCount);

/// <summary>
/// Key disable result
/// </summary>
public record KeyDisableResult(
    bool Success,
    DateTimeOffset? DisabledAt = null,
    string? ErrorMessage = null);

/// <summary>
/// Key enable result
/// </summary>
public record KeyEnableResult(
    bool Success,
    DateTimeOffset? EnabledAt = null,
    string? ErrorMessage = null);

/// <summary>
/// Data encryption key options
/// </summary>
public record DataEncryptionKeyOptions(
    int KeySize = 256,
    EncryptionAlgorithm Algorithm = EncryptionAlgorithm.AES256_GCM,
    IDictionary<string, string>? Metadata = null);

/// <summary>
/// Data encryption key result
/// </summary>
public record DataEncryptionKeyResult(
    byte[] PlaintextKey,
    byte[] EncryptedKey,
    string KeyId,
    bool Success = true,
    string? ErrorMessage = null);

/// <summary>
/// Decrypted data encryption key result
/// </summary>
public record DecryptedDataEncryptionKeyResult(
    byte[] PlaintextKey,
    bool Success = true,
    string? ErrorMessage = null);

/// <summary>
/// Field encryption options
/// </summary>
public record FieldEncryptionOptions(
    string? KeyId = null,
    EncryptionAlgorithm Algorithm = EncryptionAlgorithm.AES256_GCM,
    bool UseSearchableEncryption = false,
    IDictionary<string, string>? Context = null);

/// <summary>
/// Field encryption result
/// </summary>
public record FieldEncryptionResult(
    string EncryptedValue,
    string? SearchableHash = null,
    bool Success = true,
    string? ErrorMessage = null);

/// <summary>
/// Field decryption options
/// </summary>
public record FieldDecryptionOptions(
    string? KeyId = null,
    IDictionary<string, string>? Context = null);

/// <summary>
/// Field decryption result
/// </summary>
public record FieldDecryptionResult(
    string DecryptedValue,
    bool Success = true,
    string? ErrorMessage = null);

/// <summary>
/// File encryption options
/// </summary>
public record FileEncryptionOptions(
    string? KeyId = null,
    EncryptionAlgorithm Algorithm = EncryptionAlgorithm.AES256_GCM,
    string? OutputPath = null,
    bool DeleteOriginal = false,
    bool UseCompression = true);

/// <summary>
/// File encryption result
/// </summary>
public record FileEncryptionResult(
    string EncryptedFilePath,
    long OriginalSize,
    long EncryptedSize,
    string FileHash,
    bool Success = true,
    string? ErrorMessage = null);

/// <summary>
/// File decryption options
/// </summary>
public record FileDecryptionOptions(
    string? KeyId = null,
    string? OutputPath = null,
    bool DeleteEncrypted = false,
    bool VerifyHash = true);

/// <summary>
/// File decryption result
/// </summary>
public record FileDecryptionResult(
    string DecryptedFilePath,
    long DecryptedSize,
    string FileHash,
    bool HashVerified,
    bool Success = true,
    string? ErrorMessage = null);

/// <summary>
/// Encryption service health
/// </summary>
public record EncryptionServiceHealth(
    bool IsHealthy,
    string ServiceVersion,
    int TotalKeys,
    int ActiveKeys,
    int DisabledKeys,
    double AverageResponseTime,
    DateTimeOffset LastHealthCheck,
    IEnumerable<string>? HealthIssues = null);

/// <summary>
/// Encryption usage statistics
/// </summary>
public record EncryptionUsageStatistics(
    DateTimeRange TimeRange,
    long TotalEncryptionOperations,
    long TotalDecryptionOperations,
    long TotalKeyRotations,
    IDictionary<EncryptionAlgorithm, long> OperationsByAlgorithm,
    IDictionary<string, long> OperationsByKeyType,
    double AverageEncryptionTime,
    double AverageDecryptionTime,
    long TotalDataEncrypted,
    long TotalDataDecrypted);

/// <summary>
/// Encryption key types
/// </summary>
public enum EncryptionKeyType {
    Symmetric,
    Asymmetric,
    KeyEncryptionKey,
    DataEncryptionKey
}

/// <summary>
/// Encryption algorithms
/// </summary>
public enum EncryptionAlgorithm {
    AES128_CBC,
    AES128_GCM,
    AES256_CBC,
    AES256_GCM,
    ChaCha20_Poly1305,
    RSA_2048,
    RSA_4096,
    ECC_P256,
    ECC_P384
}

/// <summary>
/// Key status
/// </summary>
public enum KeyStatus {
    Active,
    Disabled,
    Retired,
    Expired,
    Compromised
}

/// <summary>
/// Standard encryption contexts
/// </summary>
public static class EncryptionContexts {
    public const string UserData = "user-data";
    public const string PackageData = "package-data";
    public const string ServerData = "server-data";
    public const string ApiKeyData = "apikey-data";
    public const string AuditData = "audit-data";
    public const string ConfigurationData = "config-data";
    public const string BackupData = "backup-data";
    public const string TemporaryData = "temp-data";
}

/// <summary>
/// Field encryption types for different data sensitivity levels
/// </summary>
public static class FieldEncryptionTypes {
    public const string PersonalData = "personal";
    public const string SensitiveData = "sensitive";
    public const string FinancialData = "financial";
    public const string HealthData = "health";
    public const string CreditCardData = "creditcard";
    public const string SocialSecurityNumber = "ssn";
    public const string ApiKey = "apikey";
}