namespace MCPHub.Common.Providers;

/// <summary>
/// Generic factory for creating infrastructure providers
/// </summary>
public interface IProviderFactory<T> where T : class {
    /// <summary>
    /// Creates a provider instance based on configuration
    /// </summary>
    T CreateProvider(ProviderConfiguration configuration);

    /// <summary>
    /// Gets available provider types
    /// </summary>
    IEnumerable<ProviderTypeInfo> GetAvailableProviders();

    /// <summary>
    /// Validates provider configuration
    /// </summary>
    ProviderValidationResult ValidateConfiguration(ProviderConfiguration configuration);

    /// <summary>
    /// Tests provider connectivity
    /// </summary>
    Task<ProviderTestResult> TestProviderAsync(ProviderConfiguration configuration, CancellationToken cancellationToken = default);
}