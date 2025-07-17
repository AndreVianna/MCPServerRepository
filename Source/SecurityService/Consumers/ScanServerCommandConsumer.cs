using Common.Messaging.Configuration;
using Common.Messaging.RabbitMQ;

using Domain.Commands;
using Domain.Events;

using Microsoft.Extensions.Options;

namespace SecurityService.Consumers;

/// <summary>
/// Consumer for ScanServerCommand messages
/// </summary>
public class ScanServerCommandConsumer(
    ILogger<ScanServerCommandConsumer> logger,
    IOptions<RabbitMQConfiguration> configuration,
    IMessagePublisher messagePublisher) : BaseMessageConsumer<ScanServerCommand>(logger, configuration), BaseMessageConsumer<ScanServerCommand> {
    private readonly IMessagePublisher _messagePublisher = messagePublisher;
    private readonly ILogger<ScanServerCommandConsumer> _logger = logger;

    /// <inheritdoc />
    public override async Task ConsumeAsync(ScanServerCommand message, CancellationToken cancellationToken = default) {
        _logger.LogInformation("Processing security scan for server {ServerId}, version {ServerVersionId}, scan type {ScanType}",
            message.ServerId, message.ServerVersionId, message.ScanType);

        try {
            // Simulate security scan processing
            await SimulateSecurityScan(message, cancellationToken);

            // Publish completion event
            var completionEvent = new SecurityScanCompletedEvent(
                Guid.NewGuid().ToString(),
                message.ServerId,
                message.ServerVersionId,
                "Passed",
                0,
                message.CorrelationId,
                message.InitiatedBy);

            await _messagePublisher.PublishAsync(completionEvent, cancellationToken);

            _logger.LogInformation("Security scan completed for server {ServerId}, version {ServerVersionId}",
                message.ServerId, message.ServerVersionId);
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Failed to process security scan for server {ServerId}, version {ServerVersionId}",
                message.ServerId, message.ServerVersionId);

            // Publish failure event
            var failureEvent = new SecurityScanCompletedEvent(
                Guid.NewGuid().ToString(),
                message.ServerId,
                message.ServerVersionId,
                "Failed",
                0,
                message.CorrelationId,
                message.InitiatedBy);

            await _messagePublisher.PublishAsync(failureEvent, cancellationToken);
            throw;
        }
    }

    /// <summary>
    /// Simulates security scan processing
    /// </summary>
    /// <param name="command">The scan command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    private async Task SimulateSecurityScan(ScanServerCommand command, CancellationToken cancellationToken) {
        // Simulate different scan types with different processing times
        var processingTime = command.ScanType switch {
            "StaticAnalysis" => TimeSpan.FromSeconds(2),
            "DependencyCheck" => TimeSpan.FromSeconds(5),
            "SecurityAudit" => TimeSpan.FromSeconds(10),
            _ => TimeSpan.FromSeconds(3)
        };

        _logger.LogDebug("Simulating {ScanType} scan for {ProcessingTime}ms",
            command.ScanType, processingTime.TotalMilliseconds);

        await Task.Delay(processingTime, cancellationToken);
    }
}