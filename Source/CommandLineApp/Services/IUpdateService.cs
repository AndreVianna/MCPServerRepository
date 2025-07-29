using MCPHub.CommandLineApp.Models;
using MCPHub.Domain.ValueObjects;

namespace MCPHub.CommandLineApp.Services;

/// <summary>
/// Service for updating MCP packages with dependency checking and rollback capability
/// </summary>
public interface IUpdateService {
    /// <summary>
    /// Checks for available updates for all installed packages
    /// </summary>
    /// <param name="global">Whether to check global packages</param>
    /// <param name="includePrerelease">Whether to include prerelease versions</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of packages with available updates</returns>
    Task<List<PackageUpdateInfo>> CheckForUpdatesAsync(bool global = false, bool includePrerelease = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks for available updates for a specific package
    /// </summary>
    /// <param name="packageName">Name of the package to check</param>
    /// <param name="global">Whether to check global packages</param>
    /// <param name="includePrerelease">Whether to include prerelease versions</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Update information if available</returns>
    Task<PackageUpdateInfo?> CheckPackageUpdatesAsync(string packageName, bool global = false, bool includePrerelease = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a specific package to the latest or specified version
    /// </summary>
    /// <param name="packageName">Name of the package to update</param>
    /// <param name="targetVersion">Target version (null for latest)</param>
    /// <param name="global">Whether to update global package</param>
    /// <param name="force">Force update even if version is newer</param>
    /// <param name="skipDependencyCheck">Skip dependency compatibility checks</param>
    /// <param name="createBackup">Create backup before update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Update result</returns>
    Task<PackageUpdateResult> UpdatePackageAsync(
        string packageName,
        string? targetVersion = null,
        bool global = false,
        bool force = false,
        bool skipDependencyCheck = false,
        bool createBackup = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates all packages to their latest versions
    /// </summary>
    /// <param name="global">Whether to update global packages</param>
    /// <param name="includePrerelease">Whether to include prerelease versions</param>
    /// <param name="skipDependencyCheck">Skip dependency compatibility checks</param>
    /// <param name="createBackup">Create backup before updates</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Batch update result</returns>
    Task<BatchUpdateResult> UpdateAllPackagesAsync(
        bool global = false,
        bool includePrerelease = false,
        bool skipDependencyCheck = false,
        bool createBackup = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Rolls back a package to its previous version
    /// </summary>
    /// <param name="packageName">Name of the package to rollback</param>
    /// <param name="global">Whether to rollback global package</param>
    /// <param name="restoreFromBackup">Whether to restore from backup</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Rollback result</returns>
    Task<PackageRollbackResult> RollbackPackageAsync(
        string packageName,
        bool global = false,
        bool restoreFromBackup = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the update changelog for a package version upgrade
    /// </summary>
    /// <param name="packageName">Name of the package</param>
    /// <param name="fromVersion">Current version</param>
    /// <param name="toVersion">Target version</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Changelog information</returns>
    Task<PackageChangelog?> GetChangelogAsync(
        string packageName,
        string fromVersion,
        string toVersion,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates dependency compatibility for an update
    /// </summary>
    /// <param name="packageName">Name of the package to update</param>
    /// <param name="targetVersion">Target version</param>
    /// <param name="global">Whether to check global dependencies</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Dependency validation result</returns>
    Task<DependencyValidationResult> ValidateUpdateDependenciesAsync(
        string packageName,
        string targetVersion,
        bool global = false,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Information about available package updates
/// </summary>
public record PackageUpdateInfo {
    public string PackageName { get; init; } = string.Empty;
    public string CurrentVersion { get; init; } = string.Empty;
    public string LatestVersion { get; init; } = string.Empty;
    public bool IsPrerelease { get; init; }
    public bool HasBreakingChanges { get; init; }
    public bool HasSecurityFixes { get; init; }
    public DateTime ReleaseDate { get; init; }
    public string? ReleaseNotes { get; init; }
    public List<string> DependencyImpacts { get; init; } = new();
}

/// <summary>
/// Result of a package update operation
/// </summary>
public record PackageUpdateResult {
    public bool Success { get; init; }
    public string PackageName { get; init; } = string.Empty;
    public string? PreviousVersion { get; init; }
    public string? NewVersion { get; init; }
    public string? BackupPath { get; init; }
    public List<string> Messages { get; init; } = new();
    public List<string> Warnings { get; init; } = new();
    public List<string> Errors { get; init; } = new();
    public TimeSpan Duration { get; init; }
}

/// <summary>
/// Result of a batch update operation
/// </summary>
public record BatchUpdateResult {
    public bool Success { get; init; }
    public int TotalPackages { get; init; }
    public int UpdatedPackages { get; init; }
    public int FailedPackages { get; init; }
    public int SkippedPackages { get; init; }
    public List<PackageUpdateResult> Results { get; init; } = new();
    public string? BackupPath { get; init; }
    public TimeSpan Duration { get; init; }
}

/// <summary>
/// Result of a package rollback operation
/// </summary>
public record PackageRollbackResult {
    public bool Success { get; init; }
    public string PackageName { get; init; } = string.Empty;
    public string? PreviousVersion { get; init; }
    public string? RolledBackToVersion { get; init; }
    public List<string> Messages { get; init; } = new();
    public List<string> Errors { get; init; } = new();
    public TimeSpan Duration { get; init; }
}

/// <summary>
/// Package changelog information
/// </summary>
public record PackageChangelog {
    public string PackageName { get; init; } = string.Empty;
    public string FromVersion { get; init; } = string.Empty;
    public string ToVersion { get; init; } = string.Empty;
    public List<ChangelogEntry> Entries { get; init; } = new();
    public bool HasBreakingChanges { get; init; }
    public bool HasSecurityFixes { get; init; }
}

/// <summary>
/// Individual changelog entry
/// </summary>
public record ChangelogEntry {
    public ChangelogEntryType Type { get; init; }
    public string Description { get; init; } = string.Empty;
    public string? IssueId { get; init; }
    public string? Author { get; init; }
    public DateTime Date { get; init; }
}

/// <summary>
/// Types of changes in a changelog
/// </summary>
public enum ChangelogEntryType {
    Feature,
    BugFix,
    SecurityFix,
    BreakingChange,
    Performance,
    Documentation,
    Dependency,
    Other
}

/// <summary>
/// Result of dependency validation for updates
/// </summary>
public record DependencyValidationResult {
    public bool IsValid { get; init; }
    public List<string> Conflicts { get; init; } = new();
    public List<string> Warnings { get; init; } = new();
    public List<DependencyImpact> Impacts { get; init; } = new();
}

/// <summary>
/// Impact of an update on dependencies
/// </summary>
public record DependencyImpact {
    public string DependentPackage { get; init; } = string.Empty;
    public string CurrentVersion { get; init; } = string.Empty;
    public string RequiredVersion { get; init; } = string.Empty;
    public DependencyImpactType Type { get; init; }
    public string Description { get; init; } = string.Empty;
}

/// <summary>
/// Types of dependency impacts
/// </summary>
public enum DependencyImpactType {
    Compatible,
    RequiresUpdate,
    BreakingChange,
    Conflict
}