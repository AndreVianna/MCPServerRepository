using Common.Messaging.Configuration;
using Common.Messaging.RabbitMQ;

using Domain.Commands;
using Domain.Events;

using Microsoft.Extensions.Options;

namespace PublicApi.Consumers;

/// <summary>
/// Consumer for ServerRegisteredEvent messages
/// </summary>
public class ServerRegisteredEventConsumer(
    ILogger<ServerRegisteredEventConsumer> logger,
    IOptions<RabbitMQConfiguration> configuration,
    IMessagePublisher messagePublisher) : BaseMessageConsumer<ServerRegisteredEvent>(logger, configuration), BaseMessageConsumer<ServerRegisteredEvent> {
    private readonly IMessagePublisher _messagePublisher = messagePublisher;
    private readonly ILogger<ServerRegisteredEventConsumer> _logger = logger;

    /// <inheritdoc />
    public override async Task ConsumeAsync(ServerRegisteredEvent message, CancellationToken cancellationToken = default) {
        _logger.LogInformation("Processing server registered event for server {ServerId} ({ServerName})",
            message.ServerId, message.Name);

        try {
            // Trigger security scan
            var scanCommand = new ScanServerCommand(
                message.ServerId,
                message.ServerId, // Using serverId as version for this example
                "StaticAnalysis",
                message.CorrelationId,
                "system");

            await _messagePublisher.PublishAsync(scanCommand, cancellationToken);

            // Trigger search indexing
            var indexCommand = new IndexServerCommand(
                message.ServerId,
                message.ServerId, // Using serverId as version for this example
                "Full",
                message.CorrelationId,
                "system");

            await _messagePublisher.PublishAsync(indexCommand, cancellationToken);

            _logger.LogInformation("Successfully processed server registered event for server {ServerId}",
                message.ServerId);
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Failed to process server registered event for server {ServerId}",
                message.ServerId);
            throw;
        }
    }
}