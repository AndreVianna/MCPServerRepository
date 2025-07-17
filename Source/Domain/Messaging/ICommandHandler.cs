namespace Domain.Messaging;

/// <summary>
/// Interface for command handlers
/// </summary>
public interface ICommandHandler<in TCommand> where TCommand : ICommand {
    Task HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}