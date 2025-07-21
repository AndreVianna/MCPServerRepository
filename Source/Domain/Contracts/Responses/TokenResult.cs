namespace MCPHub.Domain.Contracts.Responses;

/// <summary>
/// Result of token refresh operation
/// </summary>
public class TokenResult {
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
    public string? AccessToken { get; init; }
    public string? RefreshToken { get; init; }
    public DateTime? ExpiresAt { get; init; }
}