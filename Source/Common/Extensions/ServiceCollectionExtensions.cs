using MCPHub.Common.Configuration;
using MCPHub.Common.Configuration.Validators;

namespace MCPHub.Common.Extensions;

public static class ServiceCollectionExtensions {
    /// <summary>
    /// Registers all configuration options from the Common.Configuration namespace
    /// </summary>
    public static IServiceCollection AddConfigurationOptions(this IServiceCollection services, IConfiguration configuration) {
        services.Configure<CacheOptions>(configuration.GetSection(CacheOptions.SectionName));
        services.Configure<ObservabilityOptions>(configuration.GetSection(ObservabilityOptions.SectionName));

        // Validate options on startup
        services.AddSingleton<IValidateOptions<CacheOptions>, CacheOptionsValidator>();
        services.AddSingleton<IValidateOptions<ObservabilityOptions>, ObservabilityOptionsValidator>();

        return services;
    }

    // Note: Service discovery and automatic registration methods removed
    // All service implementations removed - only configuration options registration available

}