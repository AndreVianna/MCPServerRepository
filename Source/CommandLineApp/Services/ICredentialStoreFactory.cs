namespace MCPHub.CommandLineApp.Services;

/// <summary>
/// Factory interface for creating platform-appropriate credential stores
/// </summary>
public interface ICredentialStoreFactory {
    /// <summary>
    /// Creates the best available credential store for the current platform
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The most secure available credential store</returns>
    Task<ICredentialStore> CreateCredentialStoreAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all available credential stores for the current platform
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of available credential stores ordered by preference</returns>
    Task<IEnumerable<ICredentialStore>> GetAvailableStoresAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets information about credential store availability on the current platform
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Platform credential store information</returns>
    Task<CredentialStorePlatformInfo> GetPlatformInfoAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Information about credential store availability on the current platform
/// </summary>
public record CredentialStorePlatformInfo {
    /// <summary>
    /// The current operating system platform
    /// </summary>
    public required string Platform { get; init; }

    /// <summary>
    /// The preferred credential store for this platform
    /// </summary>
    public required string PreferredStore { get; init; }

    /// <summary>
    /// All available credential stores on this platform
    /// </summary>
    public required IReadOnlyList<CredentialStoreInfo> AvailableStores { get; init; }

    /// <summary>
    /// Whether secure storage is available
    /// </summary>
    public bool HasSecureStorage { get; init; }

    /// <summary>
    /// Platform-specific notes or limitations
    /// </summary>
    public string? Notes { get; init; }
}

/// <summary>
/// Information about a specific credential store
/// </summary>
public record CredentialStoreInfo {
    /// <summary>
    /// The name of the storage mechanism
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Whether this store is secure
    /// </summary>
    public bool IsSecure { get; init; }

    /// <summary>
    /// Whether this store is currently available
    /// </summary>
    public bool IsAvailable { get; init; }

    /// <summary>
    /// Description of the storage mechanism
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Preference order (lower numbers are preferred)
    /// </summary>
    public int PreferenceOrder { get; init; }
}