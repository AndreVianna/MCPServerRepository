namespace Domain.Messaging;

/// <summary>
/// Base class for all messages in the system
/// </summary>
public abstract record BaseMessage {
    protected BaseMessage() {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        Metadata = [];
    }

    protected BaseMessage(string? correlationId, string? userId) : this() {
        CorrelationId = correlationId;
        UserId = userId;
    }

    public Guid Id { get; init; }
    public DateTime CreatedAt { get; init; }
    public string? CorrelationId { get; init; }
    public string? UserId { get; init; }
    public Dictionary<string, object> Metadata { get; } = [];
}

/// <summary>
/// Base class for all events in the system
/// </summary>
public abstract record BaseEvent : BaseMessage {
    protected BaseEvent() {
        EventType = string.Empty;
        Version = 1;
    }

    protected BaseEvent(string eventType, int version = 1) : base() {
        EventType = eventType;
        Version = version;
    }

    protected BaseEvent(string eventType, string? correlationId, string? userId, int version = 1) : base(correlationId, userId) {
        EventType = eventType;
        Version = version;
    }

    protected BaseEvent(string eventType, string? correlationId, string? userId, string? aggregateId, int version = 1) : base(correlationId, userId) {
        EventType = eventType;
        Version = version;
        AggregateId = aggregateId;
    }

    public string EventType { get; init; }
    public int Version { get; init; }
    public string? AggregateId { get; init; }
}