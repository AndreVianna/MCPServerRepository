using System.ComponentModel.DataAnnotations;

namespace MCPHub.Domain.Contracts.Requests;

/// <summary>
/// Request model for recording package installations
/// </summary>
public record InstallationRequest {
    /// <summary>
    /// Gets or sets the installation path where the package will be installed
    /// </summary>
    [Required]
    [StringLength(500, MinimumLength = 1)]
    public required string InstallationPath { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets installation options and configuration
    /// </summary>
    public Dictionary<string, object>? InstallationOptions { get; init; }

    /// <summary>
    /// Gets or sets the client version used for installation
    /// </summary>
    [Required]
    [StringLength(50, MinimumLength = 1)]
    public required string ClientVersion { get; init; } = string.Empty;

    /// <summary>
    /// Validates the installation request
    /// </summary>
    /// <returns>True if valid, false otherwise</returns>
    public bool IsValid() {
        // Validate installation path is not empty and doesn't contain invalid characters
        if (string.IsNullOrWhiteSpace(InstallationPath))
            return false;

        // Check for directory traversal attempts
        var invalidChars = new[] { "..", "//", "\\\\" };
        if (invalidChars.Any(invalid => InstallationPath.Contains(invalid, StringComparison.OrdinalIgnoreCase)))
            return false;

        return true;
    }
}