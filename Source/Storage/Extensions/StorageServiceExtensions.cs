using Common.Models;

namespace Common.Extensions;

/// <summary>
/// Extension methods for configuring storage services
/// </summary>
public static class StorageServiceExtensions {
    /// <summary>
    /// Configures storage settings (interfaces only)
    /// </summary>
    public static IServiceCollection AddStorageConfiguration(this IServiceCollection services, IConfiguration configuration) {
        // Configure storage settings only - no implementations registered
        services.Configure<StorageConfiguration>(configuration.GetSection(StorageConfiguration.ConfigurationKey));

        // Note: All storage service implementations removed - only configuration and interfaces available
        // Concrete implementations must be registered separately when needed

        return services;
    }
}