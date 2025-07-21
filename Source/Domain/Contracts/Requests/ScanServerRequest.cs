namespace MCPHub.Domain.Contracts.Requests;

/// <summary>
/// Request to scan a server
/// </summary>
public class ScanServerRequest {
    /// <summary>
    /// The ID of the server version to scan
    /// </summary>
    public string ServerVersionId { get; set; } = string.Empty;

    /// <summary>
    /// The type of scan to perform
    /// </summary>
    public string ScanType { get; set; } = "StaticAnalysis";
}