using Domain.Messaging;

namespace Domain.Events;

/// <summary>
/// Event raised when a security scan is completed
/// </summary>
public class SecurityScanCompletedEvent : BaseEvent
{
    public SecurityScanCompletedEvent(string scanId, string serverId, string serverVersionId, string status, int vulnerabilityCount)
        : base("SecurityScan", 1)
    {
        ScanId = scanId;
        ServerId = serverId;
        ServerVersionId = serverVersionId;
        Status = status;
        VulnerabilityCount = vulnerabilityCount;
        CompletedAt = DateTime.UtcNow;
        AggregateId = scanId;
    }

    public SecurityScanCompletedEvent(string scanId, string serverId, string serverVersionId, string status, int vulnerabilityCount, string? correlationId, string? initiatedBy)
        : base("SecurityScan", correlationId, initiatedBy, scanId, 1)
    {
        ScanId = scanId;
        ServerId = serverId;
        ServerVersionId = serverVersionId;
        Status = status;
        VulnerabilityCount = vulnerabilityCount;
        CompletedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// The ID of the security scan
    /// </summary>
    public string ScanId { get; init; }

    /// <summary>
    /// The ID of the server that was scanned
    /// </summary>
    public string ServerId { get; init; }

    /// <summary>
    /// The ID of the server version that was scanned
    /// </summary>
    public string ServerVersionId { get; init; }

    /// <summary>
    /// The status of the scan (Passed, Failed, Warning)
    /// </summary>
    public string Status { get; init; }

    /// <summary>
    /// The number of vulnerabilities found
    /// </summary>
    public int VulnerabilityCount { get; init; }

    /// <summary>
    /// When the scan was completed
    /// </summary>
    public DateTime CompletedAt { get; init; }
}