using Domain.Messaging;

namespace Domain.Events;

/// <summary>
/// Event raised when a new server is registered
/// </summary>
public class ServerRegisteredEvent : BaseEvent
{
    public ServerRegisteredEvent(string serverId, string name, string description, string publisherId)
        : base("Server", 1)
    {
        ServerId = serverId;
        Name = name;
        Description = description;
        PublisherId = publisherId;
        AggregateId = serverId;
    }

    public ServerRegisteredEvent(string serverId, string name, string description, string publisherId, string? correlationId, string? initiatedBy)
        : base("Server", correlationId, initiatedBy, serverId, 1)
    {
        ServerId = serverId;
        Name = name;
        Description = description;
        PublisherId = publisherId;
    }

    /// <summary>
    /// The ID of the registered server
    /// </summary>
    public string ServerId { get; init; }

    /// <summary>
    /// The name of the server
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// The description of the server
    /// </summary>
    public string Description { get; init; }

    /// <summary>
    /// The ID of the publisher who registered the server
    /// </summary>
    public string PublisherId { get; init; }
}