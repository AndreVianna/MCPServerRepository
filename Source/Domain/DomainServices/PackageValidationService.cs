using System.Text.RegularExpressions;

using MCPHub.Domain.Entities;
using MCPHub.Domain.ValueObjects;

namespace MCPHub.Domain.DomainServices;

/// <summary>
/// Domain service for package validation business logic
/// </summary>
public partial class PackageValidationService {
    private static readonly IReadOnlySet<string> ReservedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "admin", "api", "app", "aspire", "auth", "core", "data", "debug", "dev", "docs", "download",
        "example", "framework", "help", "home", "hub", "index", "info", "lib", "library", "login",
        "mcp", "mcphub", "microsoft", "net", "new", "official", "old", "package", "packages",
        "portal", "registry", "root", "sample", "search", "security", "service", "support", "system",
        "test", "tools", "user", "utils", "web", "www"
    };

    [GeneratedRegex(@"^(0|[1-9]\d*)\.(0|[1-9]\d*)\.(0|[1-9]\d*)(?:-((?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\.(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\+([0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?$")]
    private static partial Regex SemanticVersionRegex();

    [GeneratedRegex(@"^[a-zA-Z0-9_-]+$")]
    private static partial Regex PackageNameRegex();

    public ValidationResult ValidatePackageName(string name) => name switch {
        null or "" => ValidationResult.Failure("Package name cannot be empty"),
        { Length: < 2 } => ValidationResult.Failure("Package name must be at least 2 characters long"),
        { Length: > 100 } => ValidationResult.Failure("Package name cannot exceed 100 characters"),
        _ when !IsValidPackageNameFormat(name) => ValidationResult.Failure("Package name must contain only letters, numbers, hyphens, and underscores"),
        _ when ReservedNames.Contains(name) => ValidationResult.Failure("Package name is reserved"),
        _ => ValidationResult.Success()
    };

    public static ValidationResult ValidatePackageVersion(string version) => version switch {
        null or "" => ValidationResult.Failure("Package version cannot be empty"),
        _ when !IsValidSemanticVersion(version) => ValidationResult.Failure("Package version must follow semantic versioning (e.g., 1.0.0)"),
        _ => ValidationResult.Success()
    };

    public static ValidationResult ValidatePackageDescription(string description) => description switch {
        null or "" => ValidationResult.Failure("Package description cannot be empty"),
        { Length: < 10 } => ValidationResult.Failure("Package description must be at least 10 characters long"),
        { Length: > 1000 } => ValidationResult.Failure("Package description cannot exceed 1000 characters"),
        _ => ValidationResult.Success()
    };

    public static bool CanApprovePackage(Package package)
        => package.Status == PackageStatus.Pending &&
        package.ScanResult?.Status == SecurityScanStatus.Passed &&
        package.ScanResult?.HasCriticalVulnerabilities != true;

    public static TrustTier DetermineTrustTier(Package package, string _)
        => package.ScanResult?.IsClean == true ? TrustTier.CommunityTrusted : TrustTier.Unverified;

    private static bool IsValidPackageNameFormat(string name)
        => PackageNameRegex().IsMatch(name);

    private static bool IsValidSemanticVersion(string version)
        => SemanticVersionRegex().IsMatch(version);
}