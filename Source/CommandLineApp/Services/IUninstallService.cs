using MCPHub.CommandLineApp.Models;

namespace MCPHub.CommandLineApp.Services;

/// <summary>
/// Service for uninstalling MCP packages with dependency checking and cleanup
/// </summary>
public interface IUninstallService {
    /// <summary>
    /// Uninstalls a specific package
    /// </summary>
    /// <param name="packageName">Name of the package to uninstall</param>
    /// <param name="version">Specific version to uninstall (null for all versions)</param>
    /// <param name="global">Whether to uninstall global package</param>
    /// <param name="force">Force uninstall even if dependencies exist</param>
    /// <param name="removeDependencies">Remove unused dependencies after uninstallation</param>
    /// <param name="createBackup">Create backup before uninstallation</param>
    /// <param name="purge">Remove all configuration and cache files</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Uninstall result</returns>
    Task<PackageUninstallResult> UninstallPackageAsync(
        string packageName,
        string? version = null,
        bool global = false,
        bool force = false,
        bool removeDependencies = false,
        bool createBackup = true,
        bool purge = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Uninstalls multiple packages in a batch operation
    /// </summary>
    /// <param name="packageNames">Names of packages to uninstall</param>
    /// <param name="global">Whether to uninstall global packages</param>
    /// <param name="force">Force uninstall even if dependencies exist</param>
    /// <param name="removeDependencies">Remove unused dependencies after uninstallation</param>
    /// <param name="createBackup">Create backup before uninstallation</param>
    /// <param name="purge">Remove all configuration and cache files</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Batch uninstall result</returns>
    Task<BatchUninstallResult> UninstallPackagesAsync(
        IEnumerable<string> packageNames,
        bool global = false,
        bool force = false,
        bool removeDependencies = false,
        bool createBackup = true,
        bool purge = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Analyzes dependencies before uninstallation to identify potential issues
    /// </summary>
    /// <param name="packageName">Name of the package to analyze</param>
    /// <param name="version">Specific version to analyze</param>
    /// <param name="global">Whether to analyze global dependencies</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Dependency analysis result</returns>
    Task<UninstallDependencyAnalysis> AnalyzeDependenciesAsync(
        string packageName,
        string? version = null,
        bool global = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds and removes orphaned dependencies (packages no longer needed)
    /// </summary>
    /// <param name="global">Whether to clean global dependencies</param>
    /// <param name="dryRun">Show what would be removed without actually removing</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Cleanup result</returns>
    Task<OrphanedDependencyCleanupResult> CleanupOrphanedDependenciesAsync(
        bool global = false,
        bool dryRun = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a backup of a package before uninstallation
    /// </summary>
    /// <param name="packageName">Name of the package to backup</param>
    /// <param name="version">Version to backup</param>
    /// <param name="global">Whether it's a global package</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Backup information</returns>
    Task<PackageBackupInfo> CreatePackageBackupAsync(
        string packageName,
        string version,
        bool global = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Restores a package from backup
    /// </summary>
    /// <param name="backupPath">Path to the backup</param>
    /// <param name="global">Whether to restore as global package</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Restore result</returns>
    Task<PackageRestoreResult> RestorePackageFromBackupAsync(
        string backupPath,
        bool global = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates if a package can be safely uninstalled
    /// </summary>
    /// <param name="packageName">Name of the package to validate</param>
    /// <param name="version">Version to validate</param>
    /// <param name="global">Whether it's a global package</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Validation result</returns>
    Task<UninstallValidationResult> ValidateUninstallAsync(
        string packageName,
        string? version = null,
        bool global = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Purges all traces of a package including configuration and cache files
    /// </summary>
    /// <param name="packageName">Name of the package to purge</param>
    /// <param name="global">Whether to purge global package data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Purge result</returns>
    Task<PackagePurgeResult> PurgePackageAsync(
        string packageName,
        bool global = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists all backup files available for restoration
    /// </summary>
    /// <param name="packageName">Optional package name filter</param>
    /// <param name="global">Whether to list global backups</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of available backups</returns>
    Task<List<PackageBackupInfo>> ListBackupsAsync(
        string? packageName = null,
        bool global = false,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Result of a package uninstall operation
/// </summary>
public record PackageUninstallResult {
    public bool Success { get; init; }
    public string PackageName { get; init; } = string.Empty;
    public string? UninstalledVersion { get; init; }
    public string? BackupPath { get; init; }
    public List<string> RemovedDependencies { get; init; } = new();
    public List<string> Messages { get; init; } = new();
    public List<string> Warnings { get; init; } = new();
    public List<string> Errors { get; init; } = new();
    public TimeSpan Duration { get; init; }
    public long FreedSpace { get; init; } // Bytes
}

/// <summary>
/// Result of a batch uninstall operation
/// </summary>
public record BatchUninstallResult {
    public bool Success { get; init; }
    public int TotalPackages { get; init; }
    public int UninstalledPackages { get; init; }
    public int FailedPackages { get; init; }
    public int SkippedPackages { get; init; }
    public List<PackageUninstallResult> Results { get; init; } = new();
    public string? BatchBackupPath { get; init; }
    public TimeSpan Duration { get; init; }
    public long TotalFreedSpace { get; init; } // Bytes
}

/// <summary>
/// Analysis of dependencies for uninstallation
/// </summary>
public record UninstallDependencyAnalysis {
    public string PackageName { get; init; } = string.Empty;
    public string Version { get; init; } = string.Empty;
    public bool CanUninstallSafely { get; init; }
    public List<DependentPackageInfo> DependentPackages { get; init; } = new();
    public List<string> Warnings { get; init; } = new();
    public List<string> BlockingIssues { get; init; } = new();
    public List<string> OrphanedDependencies { get; init; } = new();
}

/// <summary>
/// Information about a package that depends on the package being uninstalled
/// </summary>
public record DependentPackageInfo {
    public string PackageName { get; init; } = string.Empty;
    public string Version { get; init; } = string.Empty;
    public string RequiredVersion { get; init; } = string.Empty;
    public DependencyType DependencyType { get; init; }
    public bool IsOptional { get; init; }
    public string Description { get; init; } = string.Empty;
}

/// <summary>
/// Result of orphaned dependency cleanup
/// </summary>
public record OrphanedDependencyCleanupResult {
    public bool Success { get; init; }
    public List<string> RemovedPackages { get; init; } = new();
    public List<string> Errors { get; init; } = new();
    public long FreedSpace { get; init; } // Bytes
    public TimeSpan Duration { get; init; }
}

/// <summary>
/// Information about a package backup
/// </summary>
public record PackageBackupInfo {
    public string PackageName { get; init; } = string.Empty;
    public string Version { get; init; } = string.Empty;
    public string BackupPath { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public long BackupSize { get; init; } // Bytes
    public bool IsGlobal { get; init; }
    public string? Description { get; init; }
}

/// <summary>
/// Result of package restoration
/// </summary>
public record PackageRestoreResult {
    public bool Success { get; init; }
    public string PackageName { get; init; } = string.Empty;
    public string Version { get; init; } = string.Empty;
    public string RestorePath { get; init; } = string.Empty;
    public List<string> Messages { get; init; } = new();
    public List<string> Errors { get; init; } = new();
    public TimeSpan Duration { get; init; }
}

/// <summary>
/// Result of uninstall validation
/// </summary>
public record UninstallValidationResult {
    public bool CanUninstall { get; init; }
    public bool RequiresForce { get; init; }
    public List<string> Issues { get; init; } = new();
    public List<string> Warnings { get; init; } = new();
    public List<string> Recommendations { get; init; } = new();
}

/// <summary>
/// Result of package purge operation
/// </summary>
public record PackagePurgeResult {
    public bool Success { get; init; }
    public string PackageName { get; init; } = string.Empty;
    public List<string> RemovedPaths { get; init; } = new();
    public List<string> Messages { get; init; } = new();
    public List<string> Errors { get; init; } = new();
    public long FreedSpace { get; init; } // Bytes
    public TimeSpan Duration { get; init; }
}

/// <summary>
/// Types of package dependencies
/// </summary>
public enum DependencyType {
    Runtime,
    Development,
    Optional,
    Peer
}