namespace MCPHub.Domain.Contracts.Responses;

/// <summary>
/// Response for server indexing
/// </summary>
public class IndexServerResponse {
    /// <summary>
    /// The ID of the server
    /// </summary>
    public string ServerId { get; set; } = string.Empty;

    /// <summary>
    /// The type of indexing
    /// </summary>
    public string IndexType { get; set; } = string.Empty;

    /// <summary>
    /// The status of the indexing
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Additional message
    /// </summary>
    public string Message { get; set; } = string.Empty;
}