namespace MCPHub.Common.Services;

/// <summary>
/// Event store interface for event sourcing and audit trails
/// Supports: File-based → Database → EventStore DB
/// </summary>
public interface IEventStore
{
    /// <summary>
    /// Appends events to a stream
    /// </summary>
    Task<long> AppendToStreamAsync(string streamId, IEnumerable<IEvent> events, long expectedVersion = -1, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Reads events from a stream
    /// </summary>
    Task<IEnumerable<IEvent>> ReadStreamAsync(string streamId, long fromVersion = 0, int maxCount = int.MaxValue, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Reads all events across streams with filtering
    /// </summary>
    Task<IEnumerable<IEvent>> ReadAllEventsAsync(DateTimeOffset? fromTimestamp = null, string? eventTypeFilter = null, int maxCount = 1000, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Creates a snapshot of current state
    /// </summary>
    Task SaveSnapshotAsync<T>(string streamId, long version, T snapshot, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Loads the latest snapshot
    /// </summary>
    Task<Snapshot<T>?> LoadSnapshotAsync<T>(string streamId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Subscribes to events in real-time
    /// </summary>
    Task<IEventSubscription> SubscribeAsync(string streamPattern, Func<IEvent, Task> handler, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets event store health information
    /// </summary>
    Task<EventStoreHealth> GetHealthAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Event interface for event sourcing
/// </summary>
public interface IEvent
{
    string EventId { get; }
    string EventType { get; }
    DateTimeOffset Timestamp { get; }
    string StreamId { get; }
    long Version { get; }
    object Data { get; }
    IDictionary<string, string> Metadata { get; }
}

/// <summary>
/// Snapshot for state reconstruction
/// </summary>
public record Snapshot<T>(string StreamId, long Version, T Data, DateTimeOffset Timestamp);

/// <summary>
/// Event subscription for real-time processing
/// </summary>
public interface IEventSubscription : IDisposable
{
    string SubscriptionId { get; }
    bool IsActive { get; }
    Task StopAsync();
}

/// <summary>
/// Event store health information
/// </summary>
public record EventStoreHealth(
    bool IsHealthy,
    long EventCount,
    long StreamCount,
    double WriteLatency,
    double ReadLatency,
    string? ErrorMessage = null);