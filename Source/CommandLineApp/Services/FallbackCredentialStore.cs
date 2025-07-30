namespace MCPHub.CommandLineApp.Services;

/// <summary>
/// Fallback credential store using encrypted local file storage
/// Used when platform-specific secure storage is not available
/// </summary>
public class FallbackCredentialStore : ICredentialStore {
    private readonly string _credentialsFilePath;
    private readonly ILogger<FallbackCredentialStore>? _logger;

    public FallbackCredentialStore(ILogger<FallbackCredentialStore>? logger = null) {
        _logger = logger;
        var homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var mcpmDir = Path.Combine(homeDir, ".mcpm");
        _credentialsFilePath = Path.Combine(mcpmDir, "credentials.encrypted");
    }

    /// <inheritdoc />
    public string StorageMechanism => "Encrypted File";

    /// <inheritdoc />
    public bool IsSecure => true;

    /// <inheritdoc />
    public Task<bool> StoreTokenAsync(string registryUrl, string token, string username, CancellationToken cancellationToken = default) => throw new NotImplementedException("Encrypted file credential storage will be implemented when first consumer requires it");

    /// <inheritdoc />
    public Task<StoredCredential?> GetTokenAsync(string registryUrl, CancellationToken cancellationToken = default) => throw new NotImplementedException("Encrypted file credential retrieval will be implemented when first consumer requires it");

    /// <inheritdoc />
    public Task<bool> RemoveTokenAsync(string registryUrl, CancellationToken cancellationToken = default) => throw new NotImplementedException("Encrypted file credential removal will be implemented when first consumer requires it");

    /// <inheritdoc />
    public Task<IEnumerable<string>> ListStoredRegistriesAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException("Encrypted file credential listing will be implemented when first consumer requires it");

    /// <inheritdoc />
    public Task<bool> ClearAllAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException("Encrypted file credential clear all will be implemented when first consumer requires it");

    /// <inheritdoc />
    public Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default) => Task.FromResult(CanCreateCredentialsFile());

    private bool CanCreateCredentialsFile() {
        try {
            var directory = Path.GetDirectoryName(_credentialsFilePath);
            if (directory != null && !Directory.Exists(directory)) {
                Directory.CreateDirectory(directory);
            }
            return true;
        }
        catch (Exception ex) {
            _logger?.LogWarning(ex, "Cannot create credentials file directory");
            return false;
        }
    }
}

/// <summary>
/// Internal model for storing encrypted credentials in file
/// </summary>
internal record EncryptedCredentialsFile {
    public Dictionary<string, EncryptedCredentialEntry> Credentials { get; init; } = [];
    public string EncryptionMethod { get; init; } = "AES256";
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset LastModifiedAt { get; init; } = DateTimeOffset.UtcNow;
}

/// <summary>
/// Internal model for individual encrypted credential entries
/// </summary>
internal record EncryptedCredentialEntry {
    public required string EncryptedToken { get; init; }
    public required string EncryptedUsername { get; init; }
    public required string Salt { get; init; }
    public required string InitializationVector { get; init; }
    public DateTimeOffset StoredAt { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? ExpiresAt { get; init; }
    public Dictionary<string, string> Metadata { get; init; } = [];
}