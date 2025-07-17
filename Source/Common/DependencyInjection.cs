using Common.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using StackExchange.Redis;

namespace Common;

public static class DependencyInjection {
    public static IServiceCollection AddCommonServices(this IServiceCollection services, IConfiguration configuration) {
        // Redis Configuration
        var redisConnectionString = configuration.GetConnectionString("Redis")
            ?? throw new InvalidOperationException("Redis connection string not found.");

        services.AddSingleton<IConnectionMultiplexer>(provider => {
            var config = ConfigurationOptions.Parse(redisConnectionString);
            config.AbortOnConnectFail = false;
            config.ConnectRetry = 3;
            config.ConnectTimeout = 5000;
            config.SyncTimeout = 5000;
            config.AsyncTimeout = 5000;
            config.KeepAlive = 60;
            config.DefaultDatabase = 0;
            return ConnectionMultiplexer.Connect(config);
        });

        // Distributed Cache
        services.AddStackExchangeRedisCache(options => {
            options.Configuration = redisConnectionString;
            options.InstanceName = "MCPHub";
        });

        // Cache Service
        services.AddScoped<ICacheService, RedisCacheService>();

        return services;
    }
}