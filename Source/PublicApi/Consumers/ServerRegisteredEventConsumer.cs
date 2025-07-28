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
    public Task ConsumeAsync(ServerRegisteredEvent message, CancellationToken cancellationToken = default) 
        => throw new NotImplementedException("Server registration event processing will be implemented when messaging infrastructure is available");
}