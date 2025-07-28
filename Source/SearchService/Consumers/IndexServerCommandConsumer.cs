using MCPHub.Common.Messaging;
using MCPHub.Domain.Commands;

namespace MCPHub.SearchService.Consumers;

/// <summary>
/// Consumer for IndexServerCommand messages
/// </summary>
public class IndexServerCommandConsumer(
    ILogger<IndexServerCommandConsumer> logger,
    IMessagePublisher messagePublisher) {
    private readonly IMessagePublisher _messagePublisher = messagePublisher;
    private readonly ILogger<IndexServerCommandConsumer> _logger = logger;

    /// <summary>
    /// Processes an IndexServerCommand message
    /// </summary>
    public Task ConsumeAsync(IndexServerCommand message, CancellationToken cancellationToken = default) 
        => throw new NotImplementedException("Search indexing command processing will be implemented when search infrastructure is available");
}