using Domain.Messaging;

namespace Domain.Commands;

/// <summary>
/// Command to initiate a security scan for a server version
/// </summary>
public class ScanServerCommand : BaseMessage, ICommand<ScanServerCommandResult> {
    public ScanServerCommand(string serverId, string serverVersionId, string scanType) {
        ServerId = serverId;
        ServerVersionId = serverVersionId;
        ScanType = scanType;
    }

    public ScanServerCommand(string serverId, string serverVersionId, string scanType, string? correlationId, string? initiatedBy)
        : base(correlationId, initiatedBy) {
        ServerId = serverId;
        ServerVersionId = serverVersionId;
        ScanType = scanType;
    }

    /// <summary>
    /// The ID of the server to scan
    /// </summary>
    public string ServerId { get; init; }

    /// <summary>
    /// The ID of the server version to scan
    /// </summary>
    public string ServerVersionId { get; init; }

    /// <summary>
    /// The type of scan to perform (StaticAnalysis, DependencyCheck, SecurityAudit)
    /// </summary>
    public string ScanType { get; init; }
}