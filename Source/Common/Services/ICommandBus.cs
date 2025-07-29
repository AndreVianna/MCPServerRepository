using MCPHub.Domain.Messaging;

namespace MCPHub.Common.Services;

/// <summary>
/// Simple command bus interface for dispatching commands
/// </summary>
public interface ICommandBus {
    Task SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default) where TCommand : ICommand;
}