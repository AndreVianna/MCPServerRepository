namespace MCPHub.CommandLineApp.Services;

/// <summary>
/// macOS Keychain implementation of ICredentialStore
/// Uses macOS Keychain Services for secure credential storage
/// </summary>
public class MacOSKeychainStore : ICredentialStore {
    private const string ServiceName = "mcpm";

    /// <inheritdoc />
    public string StorageMechanism => "macOS Keychain";

    /// <inheritdoc />
    public bool IsSecure => true;

    /// <inheritdoc />
    public Task<bool> StoreTokenAsync(string registryUrl, string token, string username, CancellationToken cancellationToken = default) => throw new NotImplementedException("macOS Keychain storage will be implemented when first consumer requires it");

    /// <inheritdoc />
    public Task<StoredCredential?> GetTokenAsync(string registryUrl, CancellationToken cancellationToken = default) => throw new NotImplementedException("macOS Keychain retrieval will be implemented when first consumer requires it");

    /// <inheritdoc />
    public Task<bool> RemoveTokenAsync(string registryUrl, CancellationToken cancellationToken = default) => throw new NotImplementedException("macOS Keychain removal will be implemented when first consumer requires it");

    /// <inheritdoc />
    public Task<IEnumerable<string>> ListStoredRegistriesAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException("macOS Keychain listing will be implemented when first consumer requires it");

    /// <inheritdoc />
    public Task<bool> ClearAllAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException("macOS Keychain clear all will be implemented when first consumer requires it");

    /// <inheritdoc />
    public Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default)
        // Return true on macOS platforms, false otherwise
        => Task.FromResult(OperatingSystem.IsMacOS());

    private static string GetKeychainAccount(string registryUrl) => registryUrl;
}