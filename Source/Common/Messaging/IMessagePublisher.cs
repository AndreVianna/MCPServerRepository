using MCPHub.Domain.Messaging;

namespace MCPHub.Common.Messaging;

/// <summary>
/// Interface for publishing messages
/// </summary>
public interface IMessagePublisher {
    /// <summary>
    /// Publishes a message asynchronously
    /// </summary>
    /// <typeparam name="T">The type of message to publish</typeparam>
    /// <param name="message">The message to publish</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : BaseMessage;

    /// <summary>
    /// Publishes a message asynchronously with routing key
    /// </summary>
    /// <typeparam name="T">The type of message to publish</typeparam>
    /// <param name="message">The message to publish</param>
    /// <param name="routingKey">The routing key for the message</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task PublishAsync<T>(T message, string routingKey, CancellationToken cancellationToken = default) where T : BaseMessage;
}