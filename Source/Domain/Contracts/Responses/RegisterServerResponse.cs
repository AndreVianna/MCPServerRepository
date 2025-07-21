namespace MCPHub.Domain.Contracts.Responses;

/// <summary>
/// Response for server registration
/// </summary>
public class RegisterServerResponse {
    /// <summary>
    /// The ID of the registered server
    /// </summary>
    public string ServerId { get; set; } = string.Empty;

    /// <summary>
    /// The status of the registration
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Additional message
    /// </summary>
    public string Message { get; set; } = string.Empty;
}