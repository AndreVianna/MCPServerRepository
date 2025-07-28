using MCPHub.Domain.ValueObjects;

namespace MCPHub.Domain.DomainServices;

/// <summary>
/// Service for validating MCP package manifests
/// </summary>
public interface IPackageManifestValidator
{
    /// <summary>
    /// Validates the manifest JSON content and structure
    /// </summary>
    /// <param name="manifestContent">Raw JSON content of the manifest</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Validation summary with errors and warnings</returns>
    Task<ValidationSummary> ValidateAsync(string manifestContent, CancellationToken cancellationToken = default);

    /// <summary>
    /// Parses and validates the manifest content into a structured object
    /// </summary>
    /// <param name="manifestContent">Raw JSON content of the manifest</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Parsed manifest and validation summary</returns>
    Task<(MCPManifest? Manifest, ValidationSummary ValidationSummary)> ParseAndValidateAsync(
        string manifestContent, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates namespace ownership for the package
    /// </summary>
    /// <param name="packageName">Package name (potentially with namespace)</param>
    /// <param name="publisherId">Publisher attempting to publish</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Validation summary indicating if publisher owns the namespace</returns>
    Task<ValidationSummary> ValidateNamespaceOwnershipAsync(
        string packageName, 
        Guid publisherId, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates that the package version doesn't already exist
    /// </summary>
    /// <param name="packageName">Package name</param>
    /// <param name="version">Version to check</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Validation summary indicating if version is unique</returns>
    Task<ValidationSummary> ValidateVersionUniquenessAsync(
        string packageName, 
        string version, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates package dependencies exist and are accessible
    /// </summary>
    /// <param name="dependencies">Dictionary of package dependencies</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Validation summary for dependency resolution</returns>
    Task<ValidationSummary> ValidateDependenciesAsync(
        Dictionary<string, string> dependencies, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates security permissions and capabilities
    /// </summary>
    /// <param name="manifest">Parsed manifest to validate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Validation summary for security analysis</returns>
    Task<ValidationSummary> ValidateSecurityAsync(
        MCPManifest manifest, 
        CancellationToken cancellationToken = default);
}