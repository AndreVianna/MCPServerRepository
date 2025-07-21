using MCPHub.Domain.Entities;

namespace MCPHub.Domain.Contracts.Responses;

/// <summary>
/// Result of authentication operation
/// </summary>
public class AuthenticationResult {
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
    public ApplicationUser? User { get; init; }
    public string? AccessToken { get; init; }
    public string? RefreshToken { get; init; }
    public DateTime? ExpiresAt { get; init; }
}