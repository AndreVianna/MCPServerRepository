using MCPHub.Data.Configuration;
using MCPHub.Data.Configuration.Validators;

using Microsoft.Extensions.Options;

namespace MCPHub.Data.Extensions;

public static class DataServiceCollectionExtensions {
    /// <summary>
    /// Registers database configuration options
    /// </summary>
    public static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration) {
        services.Configure<DatabaseOptions>(configuration.GetSection(DatabaseOptions.SectionName));
        services.AddSingleton<IValidateOptions<DatabaseOptions>, DatabaseOptionsValidator>();

        return services;
    }
}