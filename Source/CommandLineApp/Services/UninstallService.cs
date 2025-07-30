using MCPHub.CommandLineApp.Configuration;
using MCPHub.CommandLineApp.Utilities;

namespace MCPHub.CommandLineApp.Services;

/// <summary>
/// Service for uninstalling MCP packages with dependency checking and cleanup
/// </summary>
public class UninstallService(
    ILogger<UninstallService> logger,
    McpmConfiguration configuration,
    IMcpHubApiClient apiClient,
    PackageManager packageManager,
    DependencyResolver dependencyResolver) : IUninstallService {
    private readonly ILogger<UninstallService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly McpmConfiguration _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    private readonly IMcpHubApiClient _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
    private readonly PackageManager _packageManager = packageManager ?? throw new ArgumentNullException(nameof(packageManager));
    private readonly DependencyResolver _dependencyResolver = dependencyResolver ?? throw new ArgumentNullException(nameof(dependencyResolver));

    /// <inheritdoc />
    public Task<PackageUninstallResult> UninstallPackageAsync(
        string packageName,
        string? version = null,
        bool global = false,
        bool force = false,
        bool removeDependencies = false,
        bool createBackup = true,
        bool purge = false,
        CancellationToken cancellationToken = default) => throw new NotImplementedException("Package uninstall logic will be implemented when first consumer requires it");

    /// <inheritdoc />
    public Task<BatchUninstallResult> UninstallPackagesAsync(
        IEnumerable<string> packageNames,
        bool global = false,
        bool force = false,
        bool removeDependencies = false,
        bool createBackup = true,
        bool purge = false,
        CancellationToken cancellationToken = default) => throw new NotImplementedException("Batch uninstall logic will be implemented when first consumer requires it");

    /// <inheritdoc />
    public Task<UninstallDependencyAnalysis> AnalyzeDependenciesAsync(
        string packageName,
        string? version = null,
        bool global = false,
        CancellationToken cancellationToken = default) => throw new NotImplementedException("Dependency analysis logic will be implemented when first consumer requires it");

    /// <inheritdoc />
    public Task<OrphanedDependencyCleanupResult> CleanupOrphanedDependenciesAsync(
        bool global = false,
        bool dryRun = false,
        CancellationToken cancellationToken = default) => throw new NotImplementedException("Orphaned dependency cleanup logic will be implemented when first consumer requires it");

    /// <inheritdoc />
    public Task<PackageBackupInfo> CreatePackageBackupAsync(
        string packageName,
        string version,
        bool global = false,
        CancellationToken cancellationToken = default) => throw new NotImplementedException("Package backup logic will be implemented when first consumer requires it");

    /// <inheritdoc />
    public Task<PackageRestoreResult> RestorePackageFromBackupAsync(
        string backupPath,
        bool global = false,
        CancellationToken cancellationToken = default) => throw new NotImplementedException("Package restore logic will be implemented when first consumer requires it");

    /// <inheritdoc />
    public Task<UninstallValidationResult> ValidateUninstallAsync(
        string packageName,
        string? version = null,
        bool global = false,
        CancellationToken cancellationToken = default) => throw new NotImplementedException("Uninstall validation logic will be implemented when first consumer requires it");

    /// <inheritdoc />
    public Task<PackagePurgeResult> PurgePackageAsync(
        string packageName,
        bool global = false,
        CancellationToken cancellationToken = default) => throw new NotImplementedException("Package purge logic will be implemented when first consumer requires it");

    /// <inheritdoc />
    public Task<List<PackageBackupInfo>> ListBackupsAsync(
        string? packageName = null,
        bool global = false,
        CancellationToken cancellationToken = default) => throw new NotImplementedException("Backup listing logic will be implemented when first consumer requires it");
}