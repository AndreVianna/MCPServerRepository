using MCPHub.CommandLineApp.Configuration;
using MCPHub.CommandLineApp.Services;
using MCPHub.CommandLineApp.Utilities;

using Microsoft.Extensions.DependencyInjection;

namespace MCPHub.CommandLineApp.Extensions;

/// <summary>
/// Extension methods for registering application services
/// </summary>
public static class ServiceCollectionExtensions {
    /// <summary>
    /// Registers authentication services for secure credential management
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddAuthenticationServices(this IServiceCollection services) {
        // Register credential store implementations
        services.AddSingleton<WindowsCredentialStore>();
        services.AddSingleton<MacOSKeychainStore>();
        services.AddSingleton<LinuxCredentialStore>();
        services.AddSingleton<FallbackCredentialStore>();

        // Register credential store factory
        services.AddSingleton<ICredentialStoreFactory, CredentialStoreFactory>();

        // Register the primary credential store (determined by factory)
        services.AddSingleton<ICredentialStore>(serviceProvider => {
            var factory = serviceProvider.GetRequiredService<ICredentialStoreFactory>();
            // This will block during startup, but it's necessary for DI registration
            return factory.CreateCredentialStoreAsync().GetAwaiter().GetResult();
        });

        // Register authentication manager
        services.AddSingleton<IAuthenticationManager, AuthenticationManager>();

        return services;
    }

    /// <summary>
    /// Registers package management services
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddPackageManagementServices(this IServiceCollection services) {
        // Register package management utilities
        services.AddSingleton<PackageManager>();
        services.AddSingleton<DependencyResolver>();

        // Register new Phase 2B services
        services.AddSingleton<IUpdateService, UpdateService>();
        services.AddSingleton<IUninstallService, UninstallService>();
        services.AddSingleton<IDoctorService, DoctorService>();

        return services;
    }

    /// <summary>
    /// Registers output formatting services
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddOutputServices(this IServiceCollection services) {
        services.AddSingleton<IOutputFormatter, ConsoleOutputFormatter>();

        return services;
    }

    /// <summary>
    /// Registers interactive UI services for enhanced CLI experience
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddInteractiveServices(this IServiceCollection services) {
        services.AddSingleton<IInteractionService, InteractionService>();
        services.AddSingleton<IProgressReporter, ProgressReporter>();

        return services;
    }

    /// <summary>
    /// Registers configuration management services
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddConfigurationServices(this IServiceCollection services) {
        services.AddSingleton<IMcpmConfigurationManager, McpmConfigurationManager>();
        services.AddSingleton<IConfigurationService, ConfigurationService>();

        return services;
    }

    /// <summary>
    /// Registers security and trust tier services
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddSecurityServices(this IServiceCollection services) {
        services.AddSingleton<ISecurityService, SecurityService>();
        services.AddSingleton<ITrustTierService, TrustTierService>();
        services.AddSingleton<IOfflineSecurityService, OfflineSecurityService>();

        return services;
    }

    /// <summary>
    /// Registers caching services for performance optimization and offline support
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddCachingServices(this IServiceCollection services) {
        // Register core cache service
        services.AddSingleton<ICacheService, FileCacheService>();

        // Register specialized cache services
        services.AddSingleton<IPackageCacheService, PackageCacheService>();
        services.AddSingleton<ISearchCacheService, SearchCacheService>();
        services.AddSingleton<IOfflineModeService, OfflineModeService>();
        services.AddSingleton<ICacheWarmupService, CacheWarmupService>();

        return services;
    }

    /// <summary>
    /// Registers API client services with caching support
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddApiClientServices(this IServiceCollection services) {
        // Register the actual API client implementation
        services.AddSingleton<McpHubApiClient>();

        // Register the cached API client as the primary interface
        services.AddSingleton<IMcpHubApiClient>(serviceProvider => {
            var innerClient = serviceProvider.GetRequiredService<McpHubApiClient>();
            var packageCache = serviceProvider.GetRequiredService<IPackageCacheService>();
            var searchCache = serviceProvider.GetRequiredService<ISearchCacheService>();
            var offlineMode = serviceProvider.GetRequiredService<IOfflineModeService>();
            var configuration = serviceProvider.GetRequiredService<McpmConfiguration>();
            var logger = serviceProvider.GetRequiredService<ILogger<CachedMcpHubApiClient>>();

            return new CachedMcpHubApiClient(logger, innerClient, packageCache, searchCache, offlineMode, configuration);
        });

        return services;
    }
}