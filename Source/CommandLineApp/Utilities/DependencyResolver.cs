using MCPHub.CommandLineApp.Configuration;
using MCPHub.CommandLineApp.Models;
using MCPHub.CommandLineApp.Services;

using Microsoft.Extensions.Logging;

namespace MCPHub.CommandLineApp.Utilities;

/// <summary>
/// Resolves package dependencies and handles version conflicts
/// </summary>
public class DependencyResolver(
    ILogger<DependencyResolver> logger,
    McpmConfiguration configuration,
    IMcpHubApiClient apiClient,
    PackageManager packageManager) {
    private readonly ILogger<DependencyResolver> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly McpmConfiguration _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    private readonly IMcpHubApiClient _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
    private readonly PackageManager _packageManager = packageManager ?? throw new ArgumentNullException(nameof(packageManager));

    /// <summary>
    /// Resolves dependencies for a package installation
    /// </summary>
    public async Task<DependencyResolutionResult> ResolveDependenciesAsync(
        string packageName,
        string version,
        bool includeDevelopmentDependencies = false,
        bool global = false,
        CancellationToken cancellationToken = default) {
        var result = new DependencyResolutionResult();
        var visited = new HashSet<string>();
        var installQueue = new List<ResolvedPackage>();

        try {
            await ResolveDependenciesRecursive(
                packageName,
                version,
                includeDevelopmentDependencies,
                global,
                visited,
                installQueue,
                result,
                isRootPackage: true,
                cancellationToken);

            // Remove duplicates and resolve conflicts
            result.PackagesToInstall = ResolveVersionConflicts(installQueue);
            result.Success = true;

            _logger.LogInformation("Dependency resolution completed: {PackageCount} packages to install",
                result.PackagesToInstall.Count);
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Failed to resolve dependencies for {PackageName}@{Version}", packageName, version);
            result.Success = false;
            result.Errors.Add($"Failed to resolve dependencies: {ex.Message}");
        }

        return result;
    }

    private async Task ResolveDependenciesRecursive(
        string packageName,
        string version,
        bool includeDevelopmentDependencies,
        bool global,
        HashSet<string> visited,
        List<ResolvedPackage> installQueue,
        DependencyResolutionResult result,
        bool isRootPackage = false,
        CancellationToken cancellationToken = default) {
        var packageKey = $"{packageName}@{version}";

        if (visited.Contains(packageKey)) {
            _logger.LogDebug("Skipping already visited package: {PackageKey}", packageKey);
            return;
        }

        visited.Add(packageKey);

        // Check if package is already installed
        var installedVersion = await _packageManager.GetInstalledPackageAsync(packageName, version, global);
        if (installedVersion != null && !isRootPackage) {
            _logger.LogDebug("Package {PackageName}@{Version} is already installed", packageName, version);
            return;
        }

        // Get package information
        PackageInfoResponse packageInfo;
        try {
            packageInfo = await _apiClient.GetPackageInfoAsync(packageName, cancellationToken);
        }
        catch (PackageNotFoundException) {
            result.Errors.Add($"Package '{packageName}' not found");
            return;
        }

        // Validate package version
        var packageVersions = await _apiClient.GetPackageVersionsAsync(packageName, false, cancellationToken);
        var targetVersion = packageVersions.Versions.FirstOrDefault(v => v.Version == version);
        if (targetVersion == null) {
            result.Errors.Add($"Version '{version}' not found for package '{packageName}'");
            return;
        }

        // Check trust tier
        if (!IsPackageTrusted(packageInfo.TrustTier)) {
            result.UntrustedPackages.Add(new UntrustedPackage {
                Name = packageName,
                Version = version,
                TrustTier = packageInfo.TrustTier,
                SecurityGrade = packageInfo.SecurityGrade
            });
        }

        // Add to install queue
        installQueue.Add(new ResolvedPackage {
            Name = packageName,
            Version = version,
            PackageInfo = packageInfo,
            VersionInfo = targetVersion,
            IsRootPackage = isRootPackage
        });

        // Get and resolve dependencies
        PackageDependenciesResponse dependencies;
        try {
            dependencies = await _apiClient.GetPackageDependenciesAsync(packageName, version, cancellationToken);
        }
        catch (Exception ex) {
            _logger.LogWarning(ex, "Failed to get dependencies for {PackageName}@{Version}", packageName, version);
            result.Warnings.Add($"Could not retrieve dependencies for '{packageName}@{version}': {ex.Message}");
            return;
        }

        // Resolve runtime dependencies
        foreach (var dependency in dependencies.Dependencies) {
            if (!dependency.Available) {
                result.Errors.Add($"Dependency '{dependency.Name}' is not available: {dependency.UnavailableReason}");
                continue;
            }

            var resolvedVersion = dependency.ResolvedVersion ?? GetLatestCompatibleVersion(dependency.VersionConstraint);
            if (resolvedVersion != null) {
                await ResolveDependenciesRecursive(
                    dependency.Name,
                    resolvedVersion,
                    false, // Don't include dev dependencies for sub-dependencies
                    global,
                    visited,
                    installQueue,
                    result,
                    isRootPackage: false,
                    cancellationToken);
            }
            else {
                result.Errors.Add($"Could not resolve version for dependency '{dependency.Name}' with constraint '{dependency.VersionConstraint}'");
            }
        }

        // Resolve development dependencies (only for root package)
        if (includeDevelopmentDependencies && isRootPackage) {
            foreach (var devDependency in dependencies.DevDependencies) {
                if (!devDependency.Available) {
                    result.Warnings.Add($"Development dependency '{devDependency.Name}' is not available: {devDependency.UnavailableReason}");
                    continue;
                }

                var resolvedVersion = devDependency.ResolvedVersion ?? GetLatestCompatibleVersion(devDependency.VersionConstraint);
                if (resolvedVersion != null) {
                    await ResolveDependenciesRecursive(
                        devDependency.Name,
                        resolvedVersion,
                        false,
                        global,
                        visited,
                        installQueue,
                        result,
                        isRootPackage: false,
                        cancellationToken);
                }
                else {
                    result.Warnings.Add($"Could not resolve version for dev dependency '{devDependency.Name}' with constraint '{devDependency.VersionConstraint}'");
                }
            }
        }
    }

    private bool IsPackageTrusted(string trustTier) {
        var minimumTier = _configuration.Security.TrustTierMinimum;
        var tierOrder = new[] { "Unverified", "Community", "Professional", "Enterprise" };

        var minIndex = Array.IndexOf(tierOrder, minimumTier);
        var packageIndex = Array.IndexOf(tierOrder, trustTier);

        return packageIndex >= minIndex;
    }

    private string? GetLatestCompatibleVersion(string versionConstraint) {
        // Simplified version resolution - in a real implementation,
        // this would parse semantic version ranges and find compatible versions
        // For now, just return the constraint as-is if it looks like a specific version
        if (versionConstraint.Contains("^") || versionConstraint.Contains("~") || versionConstraint.Contains(">=")) {
            // Remove constraint operators for now
            return versionConstraint.TrimStart('^', '~', '>', '=', ' ');
        }

        return versionConstraint;
    }

    private List<ResolvedPackage> ResolveVersionConflicts(List<ResolvedPackage> packages) {
        var resolved = new List<ResolvedPackage>();
        var packageGroups = packages.GroupBy(p => p.Name);

        foreach (var group in packageGroups) {
            if (group.Count() == 1) {
                resolved.Add(group.First());
            }
            else {
                // For conflicts, choose the latest version
                // In a real implementation, this would be more sophisticated
                var latest = group.OrderByDescending(p => p.Version).First();
                resolved.Add(latest);

                _logger.LogWarning("Version conflict resolved for '{PackageName}': choosing {Version}",
                    group.Key, latest.Version);
            }
        }

        return resolved;
    }
}

/// <summary>
/// Result of dependency resolution
/// </summary>
public class DependencyResolutionResult {
    public bool Success { get; set; }
    public List<ResolvedPackage> PackagesToInstall { get; set; } = new();
    public List<UntrustedPackage> UntrustedPackages { get; set; } = new();
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
}

/// <summary>
/// Resolved package for installation
/// </summary>
public class ResolvedPackage {
    public string Name { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public PackageInfoResponse PackageInfo { get; set; } = new();
    public PackageVersionInfo VersionInfo { get; set; } = new();
    public bool IsRootPackage { get; set; }
}

/// <summary>
/// Untrusted package information
/// </summary>
public class UntrustedPackage {
    public string Name { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string TrustTier { get; set; } = string.Empty;
    public string? SecurityGrade { get; set; }
}