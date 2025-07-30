using System.ComponentModel.DataAnnotations;

namespace MCPHub.Domain.Contracts.Requests;

/// <summary>
/// Enhanced user registration request with support for multi-step registration
/// </summary>
public class RegisterRequest {
    [Required]
    [StringLength(30, MinimumLength = 3)]
    public required string UserName { get; init; }

    [Required]
    [EmailAddress]
    public required string Email { get; init; }

    [Required]
    [StringLength(100, MinimumLength = 8)]
    public required string Password { get; init; }

    [StringLength(100)]
    public string? DisplayName { get; init; }

    [StringLength(50)]
    public string? FirstName { get; init; }

    [StringLength(50)]
    public string? LastName { get; init; }

    /// <summary>
    /// User account type: Individual, Organization, Enterprise
    /// </summary>
    public required string AccountType { get; init; } = "Individual";

    public bool AcceptTerms { get; init; } = false;
    public bool ReceiveUpdates { get; init; } = true;

    /// <summary>
    /// Optional organization/company name for non-individual accounts
    /// </summary>
    [StringLength(100)]
    public string? OrganizationName { get; init; }

    /// <summary>
    /// Optional website URL
    /// </summary>
    [Url]
    public string? Website { get; init; }

    /// <summary>
    /// Optional phone number for account verification
    /// </summary>
    [Phone]
    public string? PhoneNumber { get; init; }
}

/// <summary>
/// Social login registration request
/// </summary>
public class SocialLoginRequest {
    public required string Provider { get; init; }
    public required string AccessToken { get; init; }
    public string? Email { get; init; }
    public string? Name { get; init; }
    public string? ProfileImageUrl { get; init; }
}

/// <summary>
/// Email verification request
/// </summary>
public class VerifyEmailRequest {
    [Required]
    public required string Token { get; init; }

    [Required]
    [EmailAddress]
    public required string Email { get; init; }
}

/// <summary>
/// Resend email verification request
/// </summary>
public class ResendVerificationRequest {
    [Required]
    [EmailAddress]
    public required string Email { get; init; }
}