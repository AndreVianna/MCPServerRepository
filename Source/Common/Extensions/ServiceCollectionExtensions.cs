using System.Reflection;

using Common.Configuration;

using FluentValidation;

namespace Common.Extensions;

public static class ServiceCollectionExtensions {
    /// <summary>
    /// Registers all configuration options from the Common.Configuration namespace
    /// </summary>
    public static IServiceCollection AddConfigurationOptions(this IServiceCollection services, IConfiguration configuration) {
        services.Configure<DatabaseOptions>(configuration.GetSection(DatabaseOptions.SectionName));
        services.Configure<CacheOptions>(configuration.GetSection(CacheOptions.SectionName));
        services.Configure<ElasticsearchOptions>(configuration.GetSection(ElasticsearchOptions.SectionName));
        services.Configure<RabbitMqOptions>(configuration.GetSection(RabbitMqOptions.SectionName));
        services.Configure<ObservabilityOptions>(configuration.GetSection(ObservabilityOptions.SectionName));

        // Validate options on startup
        services.AddSingleton<IValidateOptions<DatabaseOptions>, DatabaseOptionsValidator>();
        services.AddSingleton<IValidateOptions<CacheOptions>, CacheOptionsValidator>();
        services.AddSingleton<IValidateOptions<ElasticsearchOptions>, ElasticsearchOptionsValidator>();
        services.AddSingleton<IValidateOptions<RabbitMqOptions>, RabbitMqOptionsValidator>();
        services.AddSingleton<IValidateOptions<ObservabilityOptions>, ObservabilityOptionsValidator>();

        return services;
    }

    /// <summary>
    /// Registers services from an assembly using automatic discovery
    /// </summary>
    public static IServiceCollection AddServicesFromAssembly(this IServiceCollection services, Assembly assembly) {
        var types = assembly.GetTypes()
            .Where(type => type.IsClass && !type.IsAbstract)
            .Where(type => type.Name.EndsWith("Service"))
            .ToList();

        foreach (var type in types) {
            var interfaces = type.GetInterfaces()
                .Where(i => i.Name.EndsWith("Service"))
                .ToList();

            if (interfaces.Count > 0) {
                foreach (var interfaceType in interfaces) {
                    services.AddScoped(interfaceType, type);
                }
            }
            else {
                services.AddScoped(type);
            }
        }

        return services;
    }

    /// <summary>
    /// Registers Mediator handlers from an assembly
    /// </summary>
    public static IServiceCollection AddMediatorFromAssembly(this IServiceCollection services, Assembly assembly) {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        return services;
    }

    /// <summary>
    /// Registers AutoMapper profiles from an assembly
    /// </summary>
    public static IServiceCollection AddAutoMapperFromAssembly(this IServiceCollection services, Assembly assembly) {
        services.AddAutoMapper(_ => { }, assembly);
        return services;
    }

    /// <summary>
    /// Registers FluentValidation validators from an assembly
    /// </summary>
    public static IServiceCollection AddFluentValidationFromAssembly(this IServiceCollection services, Assembly assembly) {
        services.AddValidatorsFromAssembly(assembly);
        return services;
    }
}