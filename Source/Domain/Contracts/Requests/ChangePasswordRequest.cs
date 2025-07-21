using System.ComponentModel.DataAnnotations;

namespace MCPHub.Domain.Contracts.Requests;

/// <summary>
/// Password change request model
/// </summary>
public class ChangePasswordRequest {
    [Required]
    public required string CurrentPassword { get; init; }

    [Required]
    public required string NewPassword { get; init; }

    [Required]
    public required string ConfirmPassword { get; init; }
}