namespace Domain.Messaging;

/// <summary>
/// Interface for commands
/// </summary>
public interface ICommand;

/// <summary>
/// Interface for commands that return a result
/// </summary>
public interface ICommand<out TResult> : ICommand;
