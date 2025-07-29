namespace MCPHub.CommandLineApp.Services;

/// <summary>
/// Linux credential store implementation with libsecret and encrypted file fallback
/// Attempts to use libsecret first, then falls back to encrypted file storage
/// </summary>
public class LinuxCredentialStore : ICredentialStore {
    private const string SecretServiceName = "mcpm";
    private readonly string _fallbackFilePath;
    private readonly ILogger<LinuxCredentialStore>? _logger;

    public LinuxCredentialStore(ILogger<LinuxCredentialStore>? logger = null) {
        _logger = logger;
        var homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var mcpmDir = Path.Combine(homeDir, ".mcpm");
        _fallbackFilePath = Path.Combine(mcpmDir, "credentials.encrypted");
    }

    /// <inheritdoc />
    public string StorageMechanism => IsLibSecretAvailable() ? "libsecret" : "Encrypted File";

    /// <inheritdoc />
    public bool IsSecure => true;

    /// <inheritdoc />
    public Task<bool> StoreTokenAsync(string registryUrl, string token, string username, CancellationToken cancellationToken = default) => throw new NotImplementedException("Linux credential storage will be implemented when first consumer requires it");

    /// <inheritdoc />
    public Task<StoredCredential?> GetTokenAsync(string registryUrl, CancellationToken cancellationToken = default) => throw new NotImplementedException("Linux credential retrieval will be implemented when first consumer requires it");

    /// <inheritdoc />
    public Task<bool> RemoveTokenAsync(string registryUrl, CancellationToken cancellationToken = default) => throw new NotImplementedException("Linux credential removal will be implemented when first consumer requires it");

    /// <inheritdoc />
    public Task<IEnumerable<string>> ListStoredRegistriesAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException("Linux credential listing will be implemented when first consumer requires it");

    /// <inheritdoc />
    public Task<bool> ClearAllAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException("Linux credential clear all will be implemented when first consumer requires it");

    /// <inheritdoc />
    public Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default)
        // Available on Linux, or when libsecret is available, or fallback file can be created
        => Task.FromResult(OperatingSystem.IsLinux() || CanCreateFallbackFile());

    private static bool IsLibSecretAvailable()
        // This will be implemented to check if libsecret is available
        // For now, return false to use encrypted file fallback
        => false;

    private bool CanCreateFallbackFile() {
        try {
            var directory = Path.GetDirectoryName(_fallbackFilePath);
            if (directory != null && !Directory.Exists(directory)) {
                Directory.CreateDirectory(directory);
            }
            return true;
        }
        catch (Exception ex) {
            _logger?.LogWarning(ex, "Cannot create fallback credential storage directory");
            return false;
        }
    }

    private static string GetSecretAttributes(string registryUrl) => $"registry={registryUrl}";
}