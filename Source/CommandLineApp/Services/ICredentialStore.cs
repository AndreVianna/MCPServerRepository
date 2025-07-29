namespace MCPHub.CommandLineApp.Services;

/// <summary>
/// Interface for secure credential storage across different platforms
/// </summary>
public interface ICredentialStore {
    /// <summary>
    /// Stores an authentication token securely for the specified registry
    /// </summary>
    /// <param name="registryUrl">The registry URL as the credential key</param>
    /// <param name="token">The authentication token to store</param>
    /// <param name="username">The username associated with the token</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if stored successfully</returns>
    Task<bool> StoreTokenAsync(string registryUrl, string token, string username, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an authentication token for the specified registry
    /// </summary>
    /// <param name="registryUrl">The registry URL to get credentials for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Stored credential or null if not found</returns>
    Task<StoredCredential?> GetTokenAsync(string registryUrl, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes the stored authentication token for the specified registry
    /// </summary>
    /// <param name="registryUrl">The registry URL to remove credentials for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if removed successfully</returns>
    Task<bool> RemoveTokenAsync(string registryUrl, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists all stored registries with credentials
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of registry URLs that have stored credentials</returns>
    Task<IEnumerable<string>> ListStoredRegistriesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Clears all stored credentials
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if cleared successfully</returns>
    Task<bool> ClearAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Tests if the credential store is available and functional
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the store is available</returns>
    Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the platform-specific storage mechanism name
    /// </summary>
    string StorageMechanism { get; }

    /// <summary>
    /// Gets whether this store supports secure storage
    /// </summary>
    bool IsSecure { get; }
}

/// <summary>
/// Represents a stored credential retrieved from the credential store
/// </summary>
public record StoredCredential {
    /// <summary>
    /// The authentication token
    /// </summary>
    public required string Token { get; init; }

    /// <summary>
    /// The username associated with the token
    /// </summary>
    public required string Username { get; init; }

    /// <summary>
    /// When the credential was stored
    /// </summary>
    public DateTimeOffset StoredAt { get; init; }

    /// <summary>
    /// When the credential expires (optional)
    /// </summary>
    public DateTimeOffset? ExpiresAt { get; init; }

    /// <summary>
    /// Additional metadata about the credential
    /// </summary>
    public IReadOnlyDictionary<string, string> Metadata { get; init; } = new Dictionary<string, string>();
}