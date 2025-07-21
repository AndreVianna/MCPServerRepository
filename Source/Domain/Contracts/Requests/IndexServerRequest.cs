namespace MCPHub.Domain.Contracts.Requests;

/// <summary>
/// Request to index a server
/// </summary>
public class IndexServerRequest {
    /// <summary>
    /// The ID of the server version to index
    /// </summary>
    public string ServerVersionId { get; set; } = string.Empty;

    /// <summary>
    /// The type of indexing to perform
    /// </summary>
    public string IndexType { get; set; } = "Full";
}