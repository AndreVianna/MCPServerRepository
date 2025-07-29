using MCPHub.Common.Extensions;
using MCPHub.Common.Services;
using MCPHub.Data.Configuration;
using MCPHub.Data.Extensions;
using MCPHub.Data.Repositories;
using MCPHub.Domain.Entities;
using MCPHub.Domain.Repositories;

namespace MCPHub.Data;

public static class DependencyInjection {
    public static IServiceCollection AddDataServices(this IServiceCollection services, IConfiguration configuration) {
        // Add database configuration
        services.AddDatabaseConfiguration(configuration);

        // Get database options from configuration
        var databaseOptions = configuration.GetSection(DatabaseOptions.SectionName).Get<DatabaseOptions>()
            ?? throw new InvalidOperationException("Database configuration not found.");

        services.AddDbContext<McpHubContext>(options =>
            options.UseNpgsql(databaseOptions.ConnectionString, npgsqlOptions => {
                npgsqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: databaseOptions.MaxRetryCount,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorCodesToAdd: null);
                npgsqlOptions.CommandTimeout((int)databaseOptions.CommandTimeout.TotalSeconds);
            })
            .EnableSensitiveDataLogging(databaseOptions.EnableSensitiveDataLogging)
            .EnableServiceProviderCaching()
            .EnableDetailedErrors(databaseOptions.EnableDetailedErrors));

        // Note: Identity services will be configured in the AuthenticationService
        // Note: Common services removed during cleanup - only configuration options available
        // Add configuration options from Common
        services.AddConfigurationOptions(configuration);

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register base repositories
        services.AddScoped<ApplicationUserRepository>();
        services.AddScoped<PublisherRepository>();
        services.AddScoped<ServerRepository>();
        services.AddScoped<ServerVersionRepository>();
        services.AddScoped<PackageRepository>();
        services.AddScoped<PackageVersionRepository>();
        services.AddScoped<SecurityScanRepository>();
        services.AddScoped<TrustTierHistoryRepository>();
        services.AddScoped<PackageDownloadRepository>();
        services.AddScoped<PackageInstallationRepository>();

        // Register repository interfaces with cached decorators
        services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();
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
        services.AddScoped<ITrustTierHistoryRepository, TrustTierHistoryRepository>();
        services.AddScoped<IPackageDownloadRepository, PackageDownloadRepository>();
        services.AddScoped<IPackageInstallationRepository, PackageInstallationRepository>();

        return services;
    }
}