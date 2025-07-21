namespace MCPHub.Domain.Contracts.Requests;

/// <summary>
/// User registration request
/// </summary>
public class RegisterRequest {
    public required string UserName { get; init; }
    public required string Email { get; init; }
    public required string Password { get; init; }
    public string? DisplayName { get; init; }
}