using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Common.Messaging;
using Common.Messaging.DependencyInjection;
using Common.Messaging.RabbitMQ;
using Domain.Events;
using Domain.Commands;
using Common.UnitTests.TestUtilities;

namespace Common.UnitTests.Messaging;

/// <summary>
/// Integration tests for RabbitMQ message queue infrastructure
/// </summary>
[Collection("Integration")]
public class RabbitMQIntegrationTests : IClassFixture<RabbitMQTestFixture>
{
    private readonly RabbitMQTestFixture _fixture;
    private readonly IServiceProvider _serviceProvider;

    public RabbitMQIntegrationTests(RabbitMQTestFixture fixture)
    {
        _fixture = fixture;
        _serviceProvider = _fixture.ServiceProvider;
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task PublishMessage_ShouldSuccessfullyPublishMessage()
    {
        // Arrange
        var publisher = _serviceProvider.GetRequiredService<IMessagePublisher>();
        var serverRegisteredEvent = new ServerRegisteredEvent(
            Guid.NewGuid().ToString(),
            "test-server",
            "Test server description",
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            "test-user");

        // Act
        await publisher.PublishAsync(serverRegisteredEvent);

        // Assert
        // If no exception is thrown, the message was published successfully
        Assert.True(true);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task PublishMessageWithExchange_ShouldSuccessfullyPublishMessage()
    {
        // Arrange
        var publisher = _serviceProvider.GetRequiredService<IMessagePublisher>();
        var scanCommand = new ScanServerCommand(
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            "StaticAnalysis",
            Guid.NewGuid().ToString(),
            "test-user");

        // Act
        await publisher.PublishAsync(scanCommand, "commands", "security.scan");

        // Assert
        // If no exception is thrown, the message was published successfully
        Assert.True(true);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task PublishBatchMessages_ShouldSuccessfullyPublishAllMessages()
    {
        // Arrange
        var publisher = _serviceProvider.GetRequiredService<IMessagePublisher>();
        var messages = new List<ServerRegisteredEvent>();

        for (int i = 0; i < 5; i++)
        {
            messages.Add(new ServerRegisteredEvent(
                Guid.NewGuid().ToString(),
                $"test-server-{i}",
                $"Test server description {i}",
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                "test-user"));
        }

        // Act
        await publisher.PublishBatchAsync(messages);

        // Assert
        // If no exception is thrown, all messages were published successfully
        Assert.True(true);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task PublishDelayedMessage_ShouldSuccessfullyPublishMessage()
    {
        // Arrange
        var publisher = _serviceProvider.GetRequiredService<IMessagePublisher>();
        var indexCommand = new IndexServerCommand(
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            "Full",
            Guid.NewGuid().ToString(),
            "test-user");

        // Act
        await publisher.PublishDelayedAsync(indexCommand, TimeSpan.FromSeconds(1));

        // Assert
        // If no exception is thrown, the message was published successfully
        Assert.True(true);
    }

    [Fact]
    [Trait("Category", "Performance")]
    public async Task PublishMessage_PerformanceTest_ShouldPublishWithinTimeLimit()
    {
        // Arrange
        var publisher = _serviceProvider.GetRequiredService<IMessagePublisher>();
        var serverRegisteredEvent = new ServerRegisteredEvent(
            Guid.NewGuid().ToString(),
            "perf-test-server",
            "Performance test server",
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            "test-user");

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        await publisher.PublishAsync(serverRegisteredEvent);

        // Assert
        stopwatch.Stop();
        Assert.True(stopwatch.ElapsedMilliseconds < 100, $"Message publishing took {stopwatch.ElapsedMilliseconds}ms, which exceeds the 100ms limit");
    }

    [Fact]
    [Trait("Category", "Performance")]
    public async Task PublishBatchMessages_PerformanceTest_ShouldPublishWithinTimeLimit()
    {
        // Arrange
        var publisher = _serviceProvider.GetRequiredService<IMessagePublisher>();
        var messages = new List<ServerRegisteredEvent>();

        for (int i = 0; i < 100; i++)
        {
            messages.Add(new ServerRegisteredEvent(
                Guid.NewGuid().ToString(),
                $"perf-test-server-{i}",
                $"Performance test server {i}",
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                "test-user"));
        }

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        await publisher.PublishBatchAsync(messages);

        // Assert
        stopwatch.Stop();
        Assert.True(stopwatch.ElapsedMilliseconds < 5000, $"Batch message publishing took {stopwatch.ElapsedMilliseconds}ms, which exceeds the 5000ms limit");
    }

    [Fact]
    [Trait("Category", "ErrorHandling")]
    public async Task PublishMessage_WithInvalidConfiguration_ShouldThrowException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["RabbitMQ:Host"] = "invalid-host",
                ["RabbitMQ:Port"] = "5672",
                ["RabbitMQ:Username"] = "guest",
                ["RabbitMQ:Password"] = "guest"
            })
            .Build();

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddRabbitMQMessaging(configuration);

        var invalidServiceProvider = services.BuildServiceProvider();
        var publisher = invalidServiceProvider.GetRequiredService<IMessagePublisher>();

        var serverRegisteredEvent = new ServerRegisteredEvent(
            Guid.NewGuid().ToString(),
            "test-server",
            "Test server description",
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            "test-user");

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(async () =>
        {
            await publisher.PublishAsync(serverRegisteredEvent);
        });
    }
}