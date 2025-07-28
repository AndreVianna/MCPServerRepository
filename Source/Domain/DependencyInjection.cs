using MCPHub.Domain.Contracts.Services;
using MCPHub.Domain.DomainServices;
using MCPHub.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MCPHub.Domain;

/// <summary>
/// Dependency injection configuration for Domain layer services
/// </summary>
public static class DependencyInjection {
    /// <summary>
    /// Registers Domain layer services with the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddDomainServices(this IServiceCollection services) {
        // Register application services
        services.AddScoped<IPackageService, PackageService>();
        
        // Register package publishing services
        services.AddScoped<IPackagePublishingService, PackagePublishingService>();
        
        // Register package installation services
        services.AddScoped<IPackageInstallationService, PackageInstallationService>();
        
        // Register security scanning services
        services.AddScoped<ISecurityScanService, SecurityScanService>();
        services.AddScoped<ISecurityGradeCalculator, SecurityGradeCalculator>();
        
        // Register trust tier calculation services
        services.AddScoped<ITrustTierCalculationService, TrustTierCalculationService>();
        
        // Register static analyzers
        services.AddScoped<IStaticAnalyzer, ManifestSecurityScanner>();
        services.AddScoped<IStaticAnalyzer, DependencyScanner>();
        services.AddScoped<IStaticAnalyzer, ContentScanner>();
        
        // Register domain services
        services.AddScoped<IPackageManifestValidator, PackageManifestValidator>();
        
        return services;
    }
}