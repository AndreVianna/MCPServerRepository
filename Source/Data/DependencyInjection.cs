using Common;
using Common.Services;

using Data.Repositories;

namespace Data;

public static class DependencyInjection {
    public static IServiceCollection AddDataServices(this IServiceCollection services, IConfiguration configuration) {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<McpHubContext>(options =>
            options.UseNpgsql(connectionString, npgsqlOptions => {
                npgsqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                npgsqlOptions.EnableRetryOnFailure(maxRetryCount: 3, maxRetryDelay: TimeSpan.FromSeconds(5), errorCodesToAdd: null);
                npgsqlOptions.CommandTimeout(30);
            })
            .EnableSensitiveDataLogging(false)
            .EnableServiceProviderCaching()
            .EnableDetailedErrors(false));

        // Add Common services (including cache)
        services.AddCommonServices(configuration);

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register base repositories
        services.AddScoped<PublisherRepository>();
        services.AddScoped<ServerRepository>();
        services.AddScoped<ServerVersionRepository>();
        services.AddScoped<PackageRepository>();
        services.AddScoped<PackageVersionRepository>();
        services.AddScoped<SecurityScanRepository>();

        // Register repository interfaces with cached decorators
        services.AddScoped<IPublisherRepository, PublisherRepository>();
        services.AddScoped<IServerRepository, ServerRepository>();
        services.AddScoped<IServerVersionRepository, ServerVersionRepository>();
        services.AddScoped<IPackageRepository>(provider => {
            var baseRepo = provider.GetRequiredService<PackageRepository>();
            var cacheService = provider.GetRequiredService<ICacheService>();
            return new CachedPackageRepository(baseRepo, cacheService);
        });
        services.AddScoped<IPackageVersionRepository, PackageVersionRepository>();
        services.AddScoped<ISecurityScanRepository, SecurityScanRepository>();

        return services;
    }
}