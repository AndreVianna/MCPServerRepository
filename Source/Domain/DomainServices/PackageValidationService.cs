using System.Text.RegularExpressions;

namespace Domain.DomainServices;

/// <summary>
/// Domain service for package validation business logic
/// </summary>
public partial class PackageValidationService {
    private readonly List<string> _reservedNames =
    [
        "admin", "api", "app", "aspire", "auth", "core", "data", "debug", "dev", "docs", "download",
        "example", "framework", "help", "home", "hub", "index", "info", "lib", "library", "login",
        "mcp", "mcphub", "microsoft", "net", "new", "official", "old", "package", "packages",
        "portal", "registry", "root", "sample", "search", "security", "service", "support", "system",
        "test", "tools", "user", "utils", "web", "www"
    ];
    [GeneratedRegex(@"^(0|[1-9]\d*)\.(0|[1-9]\d*)\.(0|[1-9]\d*)(?:-((?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\.(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\+([0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?$")]
    private static partial Regex SemanticVersionRegex();
    [GeneratedRegex(@"^[a-zA-Z0-9_-]+$")]
    private static partial Regex PackageNameRegex();

    public ValidationResult ValidatePackageName(string name)
        => string.IsNullOrWhiteSpace(name) ? ValidationResult.Failure("Package name cannot be empty")
        : name.Length < 2 ? ValidationResult.Failure("Package name must be at least 2 characters long")
        : name.Length > 100 ? ValidationResult.Failure("Package name cannot exceed 100 characters")
        : !IsValidPackageNameFormat(name) ? ValidationResult.Failure("Package name must contain only letters, numbers, hyphens, and underscores")
        : _reservedNames.Contains(name.ToLowerInvariant()) ? ValidationResult.Failure("Package name is reserved")
        : ValidationResult.Success();

    public static ValidationResult ValidatePackageVersion(string version)
        => string.IsNullOrWhiteSpace(version) ? ValidationResult.Failure("Package version cannot be empty")
        : !IsValidSemanticVersion(version) ? ValidationResult.Failure("Package version must follow semantic versioning (e.g., 1.0.0)")
        : ValidationResult.Success();

    public static ValidationResult ValidatePackageDescription(string description)
        => string.IsNullOrWhiteSpace(description) ? ValidationResult.Failure("Package description cannot be empty")
        : description.Length < 10 ? ValidationResult.Failure("Package description must be at least 10 characters long")
        : description.Length > 1000 ? ValidationResult.Failure("Package description cannot exceed 1000 characters")
        : ValidationResult.Success();

    public static bool CanApprovePackage(Package package)
        => package.Status == PackageStatus.Pending
        && package.SecurityScan?.Status == SecurityScanStatus.Passed
        && package.SecurityScan?.HasCriticalVulnerabilities != true;

    public static TrustTier DetermineTrustTier(Package package, string _)
        => package.SecurityScan?.IsClean == true ? TrustTier.CommunityTrusted
        : TrustTier.Unverified;

    private static bool IsValidPackageNameFormat(string name)
        => PackageNameRegex().IsMatch(name);

    private static bool IsValidSemanticVersion(string version)
        => SemanticVersionRegex().IsMatch(version);
}