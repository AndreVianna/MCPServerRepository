using AwesomeAssertions;

using Common.Configuration;
using Common.Extensions;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace Common.UnitTests.Extensions;

public class HealthCheckExtensionsTests {
    private readonly IServiceCollection _services;
    private readonly IConfiguration _configuration;

    public HealthCheckExtensionsTests() {
        _services = new ServiceCollection();

        var configurationBuilder = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> {
                ["Database:ConnectionString"] = "Host=localhost;Database=test;Username=user;Password=pass",
                ["Database:HealthCheckTimeout"] = "00:00:05",
                ["Cache:ConnectionString"] = "localhost:6379",
                ["Cache:HealthCheckTimeout"] = "00:00:05",
                ["Elasticsearch:ConnectionString"] = "http://localhost:9200",
                ["Elasticsearch:HealthCheckTimeout"] = "00:00:05",
                ["RabbitMQ:ConnectionString"] = "amqp://guest:guest@localhost:5672/",
                ["RabbitMQ:HealthCheckTimeout"] = "00:00:05"
            });

        _configuration = configurationBuilder.Build();

        // Add required configuration options
        _services.AddConfigurationOptions(_configuration);
    }

    [Fact]
    public void AddInfrastructureHealthChecks_RegistersAllHealthChecks() {
        // Act
        _services.AddInfrastructureHealthChecks();
        var serviceProvider = _services.BuildServiceProvider();

        // Assert
        var healthCheckService = serviceProvider.GetService<HealthCheckService>();
        healthCheckService.Should().NotBeNull();

        var databaseHealthCheck = serviceProvider.GetService<DatabaseHealthCheck>();
        databaseHealthCheck.Should().NotBeNull();

        var cacheHealthCheck = serviceProvider.GetService<CacheHealthCheck>();
        cacheHealthCheck.Should().NotBeNull();

        var searchHealthCheck = serviceProvider.GetService<SearchHealthCheck>();
        searchHealthCheck.Should().NotBeNull();

        var messageQueueHealthCheck = serviceProvider.GetService<MessageQueueHealthCheck>();
        messageQueueHealthCheck.Should().NotBeNull();

        var applicationHealthCheck = serviceProvider.GetService<ApplicationHealthCheck>();
        applicationHealthCheck.Should().NotBeNull();
    }

    [Fact]
    public void AddInfrastructureHealthChecks_RegistersHealthChecksWithCorrectNames() {
        // Arrange
        _services.AddInfrastructureHealthChecks();
        var serviceProvider = _services.BuildServiceProvider();

        // Act
        var healthCheckService = serviceProvider.GetRequiredService<HealthCheckService>();
        var healthReport = healthCheckService.CheckHealthAsync().Result;

        // Assert
        healthReport.Entries.Should().ContainKey("database");
        healthReport.Entries.Should().ContainKey("cache");
        healthReport.Entries.Should().ContainKey("search");
        healthReport.Entries.Should().ContainKey("messagequeue");
        healthReport.Entries.Should().ContainKey("application");
    }

    [Fact]
    public void AddInfrastructureHealthChecks_RegistersHealthChecksWithCorrectTags() {
        // Arrange
        _services.AddInfrastructureHealthChecks();
        var serviceProvider = _services.BuildServiceProvider();

        // Act
        var healthCheckService = serviceProvider.GetRequiredService<HealthCheckService>();
        var healthReport = healthCheckService.CheckHealthAsync().Result;

        // Assert
        healthReport.Entries["database"].Tags.Should().Contain("database");
        healthReport.Entries["database"].Tags.Should().Contain("infrastructure");

        healthReport.Entries["cache"].Tags.Should().Contain("cache");
        healthReport.Entries["cache"].Tags.Should().Contain("infrastructure");

        healthReport.Entries["search"].Tags.Should().Contain("search");
        healthReport.Entries["search"].Tags.Should().Contain("infrastructure");

        healthReport.Entries["messagequeue"].Tags.Should().Contain("messagequeue");
        healthReport.Entries["messagequeue"].Tags.Should().Contain("infrastructure");

        healthReport.Entries["application"].Tags.Should().Contain("application");
        healthReport.Entries["application"].Tags.Should().Contain("readiness");
    }

    [Fact]
    public void ApplicationHealthCheck_ReturnsHealthy_WhenMemoryUsageIsLow() {
        // Arrange
        var healthCheck = new ApplicationHealthCheck();
        var context = new HealthCheckContext();

        // Act
        var result = healthCheck.CheckHealthAsync(context).Result;

        // Assert
        result.Status.Should().Be(HealthStatus.Healthy);
        result.Description.Should().Be("Application is healthy");
        result.Data.Should().ContainKey("memoryUsage");
        result.Data.Should().ContainKey("workingSet");
        result.Data.Should().ContainKey("uptime");
    }

    [Fact]
    public void ApplicationHealthCheck_CanBeCancelled() {
        // Arrange
        var healthCheck = new ApplicationHealthCheck();
        var context = new HealthCheckContext();
        var cancellationToken = new CancellationToken(true);

        // Act
        var result = healthCheck.CheckHealthAsync(context, cancellationToken).Result;

        // Assert - ApplicationHealthCheck should complete even if cancelled since it's synchronous
        result.Status.Should().Be(HealthStatus.Healthy);
    }
}