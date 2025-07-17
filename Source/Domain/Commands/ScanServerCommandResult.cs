using Domain.Messaging;

namespace Domain.Commands;

/// <summary>
/// Result of the scan server command
/// </summary>
public class ScanServerCommandResult(string scanId, bool isSuccess, string? errorMessage = null) {
    /// <summary>
    /// The ID of the created scan
    /// </summary>
    public string ScanId { get; init; } = scanId;

    /// <summary>
    /// Whether the scan was initiated successfully
    /// </summary>
    public bool IsSuccess { get; init; } = isSuccess;

    /// <summary>
    /// Error message if scan initiation failed
    /// </summary>
    public string? ErrorMessage { get; init; } = errorMessage;
}