using System.ComponentModel.DataAnnotations;

namespace MCPHub.Domain.Contracts.Requests;

/// <summary>
/// Login request model
/// </summary>
public class LoginRequest {
    [Required]
    [EmailAddress]
    public required string Email { get; init; }

    [Required]
    public required string Password { get; init; }

    public bool RememberMe { get; init; } = false;
}