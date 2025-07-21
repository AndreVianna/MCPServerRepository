namespace MCPHub.Domain.Contracts.Responses;

/// <summary>
/// Response for server scan
/// </summary>
public class ScanServerResponse {
    /// <summary>
    /// The ID of the server
    /// </summary>
    public string ServerId { get; set; } = string.Empty;

    /// <summary>
    /// The type of scan
    /// </summary>
    public string ScanType { get; set; } = string.Empty;

    /// <summary>
    /// The status of the scan
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Additional message
    /// </summary>
    public string Message { get; set; } = string.Empty;
}