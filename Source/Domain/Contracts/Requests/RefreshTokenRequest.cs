namespace MCPHub.Domain.Contracts.Requests;

/// <summary>
/// Token refresh request model
/// </summary>
public class RefreshTokenRequest {
    [Required]
    public required string RefreshToken { get; init; }
}