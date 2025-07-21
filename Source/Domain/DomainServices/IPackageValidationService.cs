using MCPHub.Domain.Entities;
using MCPHub.Domain.ValueObjects;

namespace MCPHub.Domain.DomainServices;

/// <summary>
/// Domain service interface for package validation business logic
/// </summary>
public interface IPackageValidationService {
    /// <summary>
    /// Validates package name format and rules
    /// </summary>
    /// <param name="name">Package name to validate</param>
    /// <returns>Validation result</returns>
    ValidationResult ValidatePackageName(string name);

    /// <summary>
    /// Validates package version format (semantic versioning)
    /// </summary>
    /// <param name="version">Package version to validate</param>
    /// <returns>Validation result</returns>
    ValidationResult ValidatePackageVersion(string version);

    /// <summary>
    /// Validates package description content and length
    /// </summary>
    /// <param name="description">Package description to validate</param>
    /// <returns>Validation result</returns>
    ValidationResult ValidatePackageDescription(string description);

    /// <summary>
    /// Determines if a package can be approved based on current status and security scan
    /// </summary>
    /// <param name="package">Package to evaluate</param>
    /// <returns>True if package can be approved</returns>
    bool CanApprovePackage(Package package);

    /// <summary>
    /// Determines appropriate trust tier for a package
    /// </summary>
    /// <param name="package">Package to evaluate</param>
    /// <param name="publisherContext">Publisher context information</param>
    /// <returns>Appropriate trust tier</returns>
    TrustTier DetermineTrustTier(Package package, string publisherContext);
}