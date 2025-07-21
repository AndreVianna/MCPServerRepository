namespace MCPHub.Domain.Contracts.Requests;

/// <summary>
/// Request to register a new server
/// </summary>
public class RegisterServerRequest {
    /// <summary>
    /// The name of the server
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The description of the server
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// The ID of the publisher
    /// </summary>
    public string PublisherId { get; set; } = string.Empty;

    /// <summary>
    /// Whether to require immediate security scan
    /// </summary>
    public bool RequireImmediateScan { get; set; } = false;
}