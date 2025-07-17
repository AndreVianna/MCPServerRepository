namespace Domain.Messaging;

/// <summary>
/// Base class for all messages in the system
/// </summary>
public abstract class BaseMessage
{
    protected BaseMessage()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        Metadata = new Dictionary<string, object>();
    }

    protected BaseMessage(string? correlationId, string? userId) : this()
    {
        CorrelationId = correlationId;
        UserId = userId;
    }

    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CorrelationId { get; set; }
    public string? UserId { get; set; }
    public Dictionary<string, object> Metadata { get; }
}

/// <summary>
/// Base class for all events in the system
/// </summary>
public abstract class BaseEvent : BaseMessage
{
    protected BaseEvent() : base()
    {
        EventType = string.Empty;
        Version = 1;
    }

    protected BaseEvent(string eventType, int version = 1) : base()
    {
        EventType = eventType;
        Version = version;
    }

    protected BaseEvent(string eventType, string? correlationId, string? userId, int version = 1) : base(correlationId, userId)
    {
        EventType = eventType;
        Version = version;
    }

    protected BaseEvent(string eventType, string? correlationId, string? userId, string? aggregateId, int version = 1) : base(correlationId, userId)
    {
        EventType = eventType;
        Version = version;
        AggregateId = aggregateId;
    }

    public string EventType { get; set; }
    public int Version { get; set; }
    public string? AggregateId { get; set; }
}

/// <summary>
/// Interface for commands
/// </summary>
public interface ICommand
{
}

/// <summary>
/// Interface for commands that return a result
/// </summary>
public interface ICommand<out TResult>
{
}

/// <summary>
/// Interface for command handlers
/// </summary>
public interface ICommandHandler<in TCommand> where TCommand : ICommand
{
    Task HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}

/// <summary>
/// Interface for command handlers that return a result
/// </summary>
public interface ICommandHandler<in TCommand, TResult> where TCommand : ICommand<TResult>
{
    Task<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}

/// <summary>
/// Interface for event handlers
/// </summary>
public interface IEventHandler<in TEvent> where TEvent : BaseEvent
{
    Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default);
}