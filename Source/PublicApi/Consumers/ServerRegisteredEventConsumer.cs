using MCPHub.Common.Messaging;
using MCPHub.Domain.Commands;
using MCPHub.Domain.Events;

namespace MCPHub.PublicApi.Consumers;

/// <summary>
/// Consumer for ServerRegisteredEvent messages
/// </summary>
public class ServerRegisteredEventConsumer(
    ILogger<ServerRegisteredEventConsumer> logger,
    IMessagePublisher messagePublisher) {
    private readonly IMessagePublisher _messagePublisher = messagePublisher;
    private readonly ILogger<ServerRegisteredEventConsumer> _logger = logger;

    /// <summary>
    /// Processes a ServerRegisteredEvent message
    /// </summary>
    public async Task ConsumeAsync(ServerRegisteredEvent message, CancellationToken cancellationToken = default) {
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