using Domain.Messaging;

namespace Domain.Commands;

/// <summary>
/// Command to index a server for search
/// </summary>
public record IndexServerCommand : BaseMessage, ICommand {
    public IndexServerCommand(string serverId, string serverVersionId, string indexType) {
        ServerId = serverId;
        ServerVersionId = serverVersionId;
        IndexType = indexType;
    }

    public IndexServerCommand(string serverId, string serverVersionId, string indexType, string? correlationId, string? initiatedBy)
        : base(correlationId, initiatedBy) {
        ServerId = serverId;
        ServerVersionId = serverVersionId;
        IndexType = indexType;
    }

    /// <summary>
    /// The ID of the server to index
    /// </summary>
    public string ServerId { get; init; }

    /// <summary>
    /// The ID of the server version to index
    /// </summary>
    public string ServerVersionId { get; init; }

    /// <summary>
    /// The type of indexing to perform (Full, Incremental, Metadata)
    /// </summary>
    public string IndexType { get; init; }
}