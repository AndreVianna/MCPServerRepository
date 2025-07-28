using MCPHub.Common.Messaging;
using MCPHub.Domain.Commands;
using MCPHub.Domain.Events;

namespace MCPHub.SecurityService.Consumers;

/// <summary>
/// Consumer for ScanServerCommand messages
/// </summary>
public class ScanServerCommandConsumer(
    ILogger<ScanServerCommandConsumer> logger,
    IMessagePublisher messagePublisher) {
    private readonly IMessagePublisher _messagePublisher = messagePublisher;
    private readonly ILogger<ScanServerCommandConsumer> _logger = logger;

    /// <summary>
    /// Processes a ScanServerCommand message
    /// </summary>
    public Task ConsumeAsync(ScanServerCommand message, CancellationToken cancellationToken = default) 
        => throw new NotImplementedException("Security scanning command processing will be implemented when security infrastructure is available");
}