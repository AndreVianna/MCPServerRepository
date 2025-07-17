using Domain.Entities;
using Domain.ValueObjects;

namespace Domain.DomainServices;

/// <summary>
/// Domain service for package validation business logic
/// </summary>
public class PackageValidationService
{
    private readonly List<string> _reservedNames = new()
    {
        "admin", "api", "app", "aspire", "auth", "core", "data", "debug", "dev", "docs", "download",
        "example", "framework", "help", "home", "hub", "index", "info", "lib", "library", "login",
        "mcp", "mcphub", "microsoft", "net", "new", "official", "old", "package", "packages",
        "portal", "registry", "root", "sample", "search", "security", "service", "support", "system",
        "test", "tools", "user", "utils", "web", "www"
    };

    public ValidationResult ValidatePackageName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return ValidationResult.Failure("Package name cannot be empty");

        if (name.Length < 2)
            return ValidationResult.Failure("Package name must be at least 2 characters long");

        if (name.Length > 100)
            return ValidationResult.Failure("Package name cannot exceed 100 characters");

        if (!IsValidPackageNameFormat(name))
            return ValidationResult.Failure("Package name must contain only letters, numbers, hyphens, and underscores");

        if (_reservedNames.Contains(name.ToLowerInvariant()))
            return ValidationResult.Failure("Package name is reserved");

        return ValidationResult.Success();
    }

    public ValidationResult ValidatePackageVersion(string version)
    {
        if (string.IsNullOrWhiteSpace(version))
            return ValidationResult.Failure("Package version cannot be empty");

        if (!IsValidSemanticVersion(version))
            return ValidationResult.Failure("Package version must follow semantic versioning (e.g., 1.0.0)");

        return ValidationResult.Success();
    }

    public ValidationResult ValidatePackageDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            return ValidationResult.Failure("Package description cannot be empty");

        if (description.Length < 10)
            return ValidationResult.Failure("Package description must be at least 10 characters long");

        if (description.Length > 1000)
            return ValidationResult.Failure("Package description cannot exceed 1000 characters");

        return ValidationResult.Success();
    }

    public bool CanApprovePackage(Package package)
    {
        if (package.Status != PackageStatus.Pending)
            return false;

        if (package.SecurityScan?.Status != SecurityScanStatus.Passed)
            return false;

        if (package.SecurityScan?.HasCriticalVulnerabilities == true)
            return false;

        return true;
    }

    public TrustTier DetermineTrustTier(Package package, string authorId)
    {
        // This is simplified logic - in reality, this would involve more complex business rules
        if (package.SecurityScan?.IsClean == true)
        {
            // Logic to determine trust tier based on author reputation, package history, etc.
            return TrustTier.CommunityTrusted;
        }

        return TrustTier.Unverified;
    }

    private bool IsValidPackageNameFormat(string name)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(name, @"^[a-zA-Z0-9_-]+$");
    }

    private bool IsValidSemanticVersion(string version)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(version, 
            @"^(0|[1-9]\d*)\.(0|[1-9]\d*)\.(0|[1-9]\d*)(?:-((?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\.(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\+([0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?$");
    }
}

public class ValidationResult
{
    public bool IsValid { get; private set; }
    public string? ErrorMessage { get; private set; }

    private ValidationResult(bool isValid, string? errorMessage = null)
    {
        IsValid = isValid;
        ErrorMessage = errorMessage;
    }

    public static ValidationResult Success() => new(true);
    public static ValidationResult Failure(string errorMessage) => new(false, errorMessage);
}