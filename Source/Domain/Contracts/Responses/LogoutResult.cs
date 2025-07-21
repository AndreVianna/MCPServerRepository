namespace MCPHub.Domain.Contracts.Responses;

/// <summary>
/// Result of logout operation
/// </summary>
public class LogoutResult {
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
}