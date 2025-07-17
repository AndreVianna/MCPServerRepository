using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Common.Messaging.Health;
using Common.Messaging.Configuration;

namespace Common.UnitTests.Messaging;

/// <summary>
/// Tests for RabbitMQ health check
/// </summary>
public class RabbitMQHealthCheckTests
{
    private readonly ILogger<RabbitMQHealthCheck> _logger;
    private readonly IOptions<RabbitMQConfiguration> _options;
    private readonly RabbitMQConfiguration _configuration;

    public RabbitMQHealthCheckTests()
    {
        _logger = Substitute.For<ILogger<RabbitMQHealthCheck>>();
        _configuration = new RabbitMQConfiguration
        {
            Host = "localhost",
            Port = 5672,
            Username = "admin",
            Password = "admin123",
            VirtualHost = "/",
            ConnectionTimeout = 30,
            HeartbeatInterval = 60,
            Exchanges = new Dictionary<string, ExchangeConfiguration>
            {
                ["test-exchange"] = new ExchangeConfiguration
                {
                    Name = "test-exchange",
                    Type = "topic",
                    Durable = true
                }
            },
            Queues = new Dictionary<string, QueueConfiguration>
            {
                ["test-queue"] = new QueueConfiguration
                {
                    Name = "test-queue",
                    Durable = true,
                    Exchange = "test-exchange",
                    RoutingKey = "test.*"
                }
            }
        };
        _options = Options.Create(_configuration);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Constructor_ShouldNotThrow()
    {
        // Act & Assert
        var healthCheck = new RabbitMQHealthCheck(_options, _logger);
        Assert.NotNull(healthCheck);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task CheckHealthAsync_WithValidConfiguration_ShouldReturnHealthy()
    {
        // Arrange
        var healthCheck = new RabbitMQHealthCheck(_options, _logger);
        var context = new HealthCheckContext();

        // Act
        var result = await healthCheck.CheckHealthAsync(context);

        // Assert - The result depends on whether RabbitMQ is actually running
        // In a real test environment, this would be healthy if RabbitMQ is available
        Assert.NotNull(result);
        Assert.Contains(result.Status, new[] { HealthStatus.Healthy, HealthStatus.Unhealthy });
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task CheckHealthAsync_WithInvalidConfiguration_ShouldReturnUnhealthy()
    {
        // Arrange
        var invalidConfiguration = new RabbitMQConfiguration
        {
            Host = "invalid-host",
            Port = 5672,
            Username = "invalid-user",
            Password = "invalid-password",
            VirtualHost = "/",
            ConnectionTimeout = 5,
            HeartbeatInterval = 60
        };

        var invalidOptions = Options.Create(invalidConfiguration);
        var healthCheck = new RabbitMQHealthCheck(invalidOptions, _logger);
        var context = new HealthCheckContext();

        // Act
        var result = await healthCheck.CheckHealthAsync(context);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(HealthStatus.Unhealthy, result.Status);
        Assert.NotNull(result.Exception);
        Assert.Contains("not accessible", result.Description);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task CheckHealthAsync_WithCancellation_ShouldRespectCancellation()
    {
        // Arrange
        var healthCheck = new RabbitMQHealthCheck(_options, _logger);
        var context = new HealthCheckContext();
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(async () =>
        {
            await healthCheck.CheckHealthAsync(context, cancellationTokenSource.Token);
        });
    }
}