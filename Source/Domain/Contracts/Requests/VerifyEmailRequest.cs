using System.ComponentModel.DataAnnotations;

namespace MCPHub.Domain.Contracts.Requests;

/// <summary>
/// Email verification request model
/// </summary>
public class VerifyEmailRequest {
    [Required]
    public required Guid UserId { get; init; }

    [Required]
    public required string VerificationToken { get; init; }
}