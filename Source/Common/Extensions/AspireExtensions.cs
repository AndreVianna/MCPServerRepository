using Common.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Common.Extensions;

public static class AspireExtensions
{
    /// <summary>
    /// Configures comprehensive infrastructure services for Aspire applications
    /// </summary>
    public static IHostApplicationBuilder AddInfrastructureServices(this IHostApplicationBuilder builder)
    {
        // Add configuration options
        builder.Services.AddConfigurationOptions(builder.Configuration);

        // Add OpenTelemetry observability
        builder.Services.AddOpenTelemetryObservability();

        // Add health checks
        builder.Services.AddInfrastructureHealthChecks();

        // Add PostgreSQL with Entity Framework
        builder.AddNpgsqlDbContext<Data.McpHubContext>("database");

        // Add Redis cache
        builder.AddRedis("cache");

        // Add Elasticsearch
        builder.AddElasticsearch("search");

        // Add RabbitMQ
        builder.AddRabbitMQ("messagequeue");

        // Add service discovery
        builder.Services.AddServiceDiscovery();

        // Add HTTP client factory with resilience
        builder.Services.AddHttpClient();
        builder.Services.AddResilienceEnricher();

        return builder;
    }

    /// <summary>
    /// Configures the Aspire distributed application
    /// </summary>
    public static IDistributedApplicationBuilder AddMcpHubInfrastructure(this IDistributedApplicationBuilder builder)
    {
        // Add PostgreSQL database
        var database = builder.AddPostgres("postgres")
            .WithPgAdmin()
            .WithLifetime(ContainerLifetime.Persistent);

        var mcpHubDb = database.AddDatabase("database");

        // Add Redis cache
        var cache = builder.AddRedis("cache")
            .WithRedisCommander()
            .WithLifetime(ContainerLifetime.Persistent);

        // Add Elasticsearch
        var search = builder.AddElasticsearch("search")
            .WithLifetime(ContainerLifetime.Persistent);

        // Add RabbitMQ
        var messageQueue = builder.AddRabbitMQ("messagequeue")
            .WithManagementPlugin()
            .WithLifetime(ContainerLifetime.Persistent);

        // Add Grafana for monitoring
        var grafana = builder.AddGrafana("grafana")
            .WithLifetime(ContainerLifetime.Persistent);

        // Add Prometheus for metrics
        var prometheus = builder.AddPrometheus("prometheus")
            .WithLifetime(ContainerLifetime.Persistent);

        // Add Jaeger for tracing
        var jaeger = builder.AddJaeger("jaeger")
            .WithLifetime(ContainerLifetime.Persistent);

        // Configure service references
        builder.AddProject<Projects.PublicApi>("publicapi")
            .WithReference(mcpHubDb)
            .WithReference(cache)
            .WithReference(search)
            .WithReference(messageQueue)
            .WithReference(jaeger);

        builder.AddProject<Projects.WebApp>("webapp")
            .WithReference(mcpHubDb)
            .WithReference(cache)
            .WithReference(search)
            .WithReference(messageQueue)
            .WithReference(jaeger);

        builder.AddProject<Projects.SecurityService>("securityservice")
            .WithReference(mcpHubDb)
            .WithReference(cache)
            .WithReference(search)
            .WithReference(messageQueue)
            .WithReference(jaeger);

        builder.AddProject<Projects.SearchService>("searchservice")
            .WithReference(mcpHubDb)
            .WithReference(cache)
            .WithReference(search)
            .WithReference(messageQueue)
            .WithReference(jaeger);

        builder.AddProject<Projects.Data_MigrationService>("datamigrationservice")
            .WithReference(mcpHubDb)
            .WithReference(jaeger);

        return builder;
    }
}