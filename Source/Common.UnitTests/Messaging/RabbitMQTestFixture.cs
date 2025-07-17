using Common.Messaging.DependencyInjection;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Common.UnitTests.Messaging;

/// <summary>
/// Test fixture for RabbitMQ integration tests
/// </summary>
public class RabbitMQTestFixture : IDisposable {
    public IServiceProvider ServiceProvider { get; private set; }
    private readonly IHost _host;

    public RabbitMQTestFixture() {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> {
                ["RabbitMQ:Host"] = "localhost",
                ["RabbitMQ:Port"] = "5672",
                ["RabbitMQ:Username"] = "admin",
                ["RabbitMQ:Password"] = "admin123",
                ["RabbitMQ:VirtualHost"] = "/",
                ["RabbitMQ:ConnectionTimeout"] = "30",
                ["RabbitMQ:RequestTimeout"] = "30",
                ["RabbitMQ:HeartbeatInterval"] = "60",
                ["RabbitMQ:EnablePublisherConfirms"] = "true",
                ["RabbitMQ:EnableMessagePersistence"] = "true",
                ["RabbitMQ:PrefetchCount"] = "10",
                ["RabbitMQ:MaxRetryAttempts"] = "3",
                ["RabbitMQ:BaseRetryDelay"] = "5",
                ["RabbitMQ:MaxRetryDelay"] = "300",
                ["RabbitMQ:DeadLetterQueue:ExchangeName"] = "test-dlx",
                ["RabbitMQ:DeadLetterQueue:QueueName"] = "test-dead-letters",
                ["RabbitMQ:DeadLetterQueue:RoutingKey"] = "dead-letter",
                ["RabbitMQ:DeadLetterQueue:Enabled"] = "true",
                ["RabbitMQ:DeadLetterQueue:MessageTtl"] = "86400000",
                ["RabbitMQ:Exchanges:commands:Name"] = "test-commands",
                ["RabbitMQ:Exchanges:commands:Type"] = "topic",
                ["RabbitMQ:Exchanges:commands:Durable"] = "true",
                ["RabbitMQ:Exchanges:commands:AutoDelete"] = "false",
                ["RabbitMQ:Exchanges:events:Name"] = "test-events",
                ["RabbitMQ:Exchanges:events:Type"] = "topic",
                ["RabbitMQ:Exchanges:events:Durable"] = "true",
                ["RabbitMQ:Exchanges:events:AutoDelete"] = "false",
                ["RabbitMQ:Queues:test-queue:Name"] = "test-queue",
                ["RabbitMQ:Queues:test-queue:Durable"] = "true",
                ["RabbitMQ:Queues:test-queue:Exclusive"] = "false",
                ["RabbitMQ:Queues:test-queue:AutoDelete"] = "false",
                ["RabbitMQ:Queues:test-queue:Exchange"] = "test-commands",
                ["RabbitMQ:Queues:test-queue:RoutingKey"] = "test.*"
            })
            .Build();

        var hostBuilder = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) => {
                services.AddLogging(builder => {
                    builder.SetMinimumLevel(LogLevel.Information);
                    builder.AddConsole();
                });

                services.AddRabbitMQMessaging(configuration);
            });

        _host = hostBuilder.Build();
        ServiceProvider = _host.Services;
    }

    public void Dispose() => _host?.Dispose();
}