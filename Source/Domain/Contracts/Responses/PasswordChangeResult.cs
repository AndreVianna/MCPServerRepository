namespace MCPHub.Domain.Contracts.Responses;

/// <summary>
/// Result of password change operation
/// </summary>
public class PasswordChangeResult {
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
}