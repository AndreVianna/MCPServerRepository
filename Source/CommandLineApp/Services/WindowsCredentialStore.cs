namespace MCPHub.CommandLineApp.Services;

/// <summary>
/// Windows Credential Manager implementation of ICredentialStore
/// Uses Windows Credential Manager API for secure credential storage
/// </summary>
public class WindowsCredentialStore : ICredentialStore {
    private const string CredentialTargetPrefix = "mcpm:";

    /// <inheritdoc />
    public string StorageMechanism => "Windows Credential Manager";

    /// <inheritdoc />
    public bool IsSecure => true;

    /// <inheritdoc />
    public Task<bool> StoreTokenAsync(string registryUrl, string token, string username, CancellationToken cancellationToken = default) => throw new NotImplementedException("Windows Credential Manager storage will be implemented when first consumer requires it");

    /// <inheritdoc />
    public Task<StoredCredential?> GetTokenAsync(string registryUrl, CancellationToken cancellationToken = default) => throw new NotImplementedException("Windows Credential Manager retrieval will be implemented when first consumer requires it");

    /// <inheritdoc />
    public Task<bool> RemoveTokenAsync(string registryUrl, CancellationToken cancellationToken = default) => throw new NotImplementedException("Windows Credential Manager removal will be implemented when first consumer requires it");

    /// <inheritdoc />
    public Task<IEnumerable<string>> ListStoredRegistriesAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException("Windows Credential Manager listing will be implemented when first consumer requires it");

    /// <inheritdoc />
    public Task<bool> ClearAllAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException("Windows Credential Manager clear all will be implemented when first consumer requires it");

    /// <inheritdoc />
    public Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default)
        // Return true on Windows platforms, false otherwise
        => Task.FromResult(OperatingSystem.IsWindows());

    private static string GetCredentialTarget(string registryUrl) => $"{CredentialTargetPrefix}{registryUrl}";
}