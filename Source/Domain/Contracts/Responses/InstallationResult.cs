using MCPHub.Domain.Entities;

namespace MCPHub.Domain.Contracts.Responses;

/// <summary>
/// Result model for package installation operations
/// </summary>
public record InstallationResult {
    /// <summary>
    /// Gets or sets whether the installation request was successful
    /// </summary>
    public required bool Success { get; init; }

    /// <summary>
    /// Gets or sets the installation tracking ID
    /// </summary>
    public Guid? InstallationId { get; init; }

    /// <summary>
    /// Gets or sets the current installation status
    /// </summary>
    public InstallationStatus? Status { get; init; }

    /// <summary>
    /// Gets or sets any error message if the request failed
    /// </summary>
    public string? ErrorMessage { get; init; }

    /// <summary>
    /// Gets or sets additional information about the installation
    /// </summary>
    public Dictionary<string, object>? Metadata { get; init; }

    /// <summary>
    /// Creates a successful installation result
    /// </summary>
    /// <param name="installationId">Installation tracking ID</param>
    /// <param name="status">Current installation status</param>
    /// <param name="metadata">Additional metadata</param>
    /// <returns>Successful installation result</returns>
    public static InstallationResult CreateSuccess(Guid installationId, InstallationStatus status, Dictionary<string, object>? metadata = null) => new() {
        Success = true,
        InstallationId = installationId,
        Status = status,
        Metadata = metadata,
    };

    /// <summary>
    /// Creates a failed installation result
    /// </summary>
    /// <param name="errorMessage">Error description</param>
    /// <param name="status">Current installation status (if applicable)</param>
    /// <returns>Failed installation result</returns>
    public static InstallationResult CreateFailure(string errorMessage, InstallationStatus? status = null) => new() {
        Success = false,
        ErrorMessage = errorMessage,
        Status = status,
    };
}