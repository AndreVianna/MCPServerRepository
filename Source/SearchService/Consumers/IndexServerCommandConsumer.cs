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
    public async Task ConsumeAsync(IndexServerCommand message, CancellationToken cancellationToken = default) {
        _logger.LogInformation("Processing search index for server {ServerId}, version {ServerVersionId}, index type {IndexType}",
            message.ServerId, message.ServerVersionId, message.IndexType);

        try {
            // Simulate search indexing processing
            await SimulateSearchIndexing(message, cancellationToken);

            _logger.LogInformation("Search indexing completed for server {ServerId}, version {ServerVersionId}",
                message.ServerId, message.ServerVersionId);
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Failed to process search indexing for server {ServerId}, version {ServerVersionId}",
                message.ServerId, message.ServerVersionId);
            throw;
        }
    }

    /// <summary>
    /// Simulates search indexing processing
    /// </summary>
    /// <param name="command">The index command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    private async Task SimulateSearchIndexing(IndexServerCommand command, CancellationToken cancellationToken) {
        // Simulate different index types with different processing times
        var processingTime = command.IndexType switch {
            "Full" => TimeSpan.FromSeconds(5),
            "Incremental" => TimeSpan.FromSeconds(2),
            "Metadata" => TimeSpan.FromSeconds(1),
            _ => TimeSpan.FromSeconds(3)
        };

        _logger.LogDebug("Simulating {IndexType} indexing for {ProcessingTime}ms",
            command.IndexType, processingTime.TotalMilliseconds);

        await Task.Delay(processingTime, cancellationToken);
    }
}