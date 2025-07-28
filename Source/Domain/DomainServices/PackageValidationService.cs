using MCPHub.Domain.Entities;
using MCPHub.Domain.ValueObjects;

namespace MCPHub.Domain.DomainServices;

/// <summary>
/// Domain service for package validation business logic following contracts-first approach
/// </summary>
public class PackageValidationService : IPackageValidationService {
    public ValidationResult ValidatePackageName(string name)
        => throw new NotImplementedException("Package name validation will be implemented when registration service requires it");

    public ValidationResult ValidatePackageVersion(string version)
        => throw new NotImplementedException("Package version validation will be implemented when registration service requires it");

    public ValidationResult ValidatePackageDescription(string description)
        => throw new NotImplementedException("Package description validation will be implemented when registration service requires it");

    public bool CanApprovePackage(Package package)
        => throw new NotImplementedException("Package approval logic will be implemented when moderation service requires it");

    public TrustTier DetermineTrustTier(Package package, string publisherContext)
        => throw new NotImplementedException("Trust tier determination will be implemented when trust system requires it");
}