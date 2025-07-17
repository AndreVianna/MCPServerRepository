using Domain.Messaging;

namespace Domain.Commands;

/// <summary>
/// Command to initiate a security scan for a server version
/// </summary>
public class ScanServerCommand : BaseMessage, ICommand<ScanServerCommandResult>
{
    public ScanServerCommand(string serverId, string serverVersionId, string scanType)
    {
        ServerId = serverId;
        ServerVersionId = serverVersionId;
        ScanType = scanType;
    }

    public ScanServerCommand(string serverId, string serverVersionId, string scanType, string? correlationId, string? initiatedBy)
        : base(correlationId, initiatedBy)
    {
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

/// <summary>
/// Result of the scan server command
/// </summary>
public class ScanServerCommandResult
{
    public ScanServerCommandResult(string scanId, bool isSuccess, string? errorMessage = null)
    {
        ScanId = scanId;
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
    }

    /// <summary>
    /// The ID of the created scan
    /// </summary>
    public string ScanId { get; init; }

    /// <summary>
    /// Whether the scan was initiated successfully
    /// </summary>
    public bool IsSuccess { get; init; }

    /// <summary>
    /// Error message if scan initiation failed
    /// </summary>
    public string? ErrorMessage { get; init; }
}