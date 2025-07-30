namespace MCPHub.Domain.Contracts.Requests;

/// <summary>
/// Request to update user profile information
/// </summary>
public class UpdateUserProfileRequest {
    [StringLength(100)]
    public string? DisplayName { get; init; }

    [StringLength(50)]
    public string? FirstName { get; init; }

    [StringLength(50)]
    public string? LastName { get; init; }

    [StringLength(500)]
    public string? Bio { get; init; }

    [StringLength(100)]
    public string? JobTitle { get; init; }

    [StringLength(100)]
    public string? Company { get; init; }

    [StringLength(100)]
    public string? Location { get; init; }

    [Url]
    public string? Website { get; init; }

    [Url]
    public string? GitHubUrl { get; init; }

    [Url]
    public string? TwitterUrl { get; init; }

    [Url]
    public string? LinkedInUrl { get; init; }

    [Phone]
    public string? PhoneNumber { get; init; }

    public bool IsPublicProfile { get; init; } = true;
    public bool ShowEmail { get; init; } = false;
    public bool ReceiveUpdates { get; init; } = true;
    public bool ReceiveMarketingEmails { get; init; } = false;
}

/// <summary>
/// Request to upload and update user avatar
/// </summary>
public class UpdateAvatarRequest {
    [Required]
    public required byte[] ImageData { get; init; }

    [Required]
    [StringLength(10)]
    public required string FileExtension { get; init; }

    /// <summary>
    /// Crop coordinates (x, y, width, height) as percentages
    /// </summary>
    public double[]? CropCoordinates { get; init; }
}

/// <summary>
/// Request to change user password
/// </summary>
public class ChangePasswordRequest {
    [Required]
    public required string CurrentPassword { get; init; }

    [Required]
    [StringLength(100, MinimumLength = 8)]
    public required string NewPassword { get; init; }

    [Required]
    [Compare(nameof(NewPassword))]
    public required string ConfirmPassword { get; init; }
}

/// <summary>
/// Request to setup two-factor authentication
/// </summary>
public class SetupTwoFactorRequest {
    /// <summary>
    /// Type of 2FA: "app" or "sms"
    /// </summary>
    [Required]
    public required string Method { get; init; }

    [Phone]
    public string? PhoneNumber { get; init; }

    /// <summary>
    /// Verification code from authenticator app
    /// </summary>
    [StringLength(10, MinimumLength = 6)]
    public string? VerificationCode { get; init; }
}

/// <summary>
/// Request to verify two-factor authentication
/// </summary>
public class VerifyTwoFactorRequest {
    [Required]
    [StringLength(10, MinimumLength = 6)]
    public required string Code { get; init; }

    /// <summary>
    /// Whether to remember this device
    /// </summary>
    public bool RememberDevice { get; init; } = false;
}

/// <summary>
/// Request to generate API key
/// </summary>
public class GenerateApiKeyRequest {
    [Required]
    [StringLength(100)]
    public required string Name { get; init; }

    [StringLength(500)]
    public string? Description { get; init; }

    /// <summary>
    /// Scopes/permissions for the API key
    /// </summary>
    public string[]? Scopes { get; init; }

    /// <summary>
    /// Optional expiration date
    /// </summary>
    public DateTime? ExpiresAt { get; init; }
}

/// <summary>
/// Request to revoke API key
/// </summary>
public class RevokeApiKeyRequest {
    [Required]
    public required Guid ApiKeyId { get; init; }
}

/// <summary>
/// Request to delete user account
/// </summary>
public class DeleteAccountRequest {
    [Required]
    public required string Password { get; init; }

    [Required]
    [StringLength(100)]
    public required string Reason { get; init; }

    /// <summary>
    /// Whether to export user data before deletion
    /// </summary>
    public bool ExportData { get; init; } = false;
}

/// <summary>
/// Request to export user data
/// </summary>
public class ExportUserDataRequest {
    /// <summary>
    /// Data types to export: profile, packages, activity, etc.
    /// </summary>
    public string[]? DataTypes { get; init; }

    /// <summary>
    /// Export format: json, csv, xml
    /// </summary>
    public string Format { get; init; } = "json";
}