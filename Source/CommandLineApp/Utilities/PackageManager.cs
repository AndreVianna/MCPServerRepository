using System.IO.Compression;
using System.Text.Json;

using MCPHub.CommandLineApp.Configuration;
using MCPHub.CommandLineApp.Models;

namespace MCPHub.CommandLineApp.Utilities;

/// <summary>
/// Manages local package installation and registry
/// </summary>
public class PackageManager(ILogger<PackageManager> logger, McpmConfiguration configuration, HttpClient httpClient) {
    private readonly ILogger<PackageManager> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly McpmConfiguration _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    private readonly HttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    /// <summary>
    /// Gets the local package registry file path
    /// </summary>
    public string GetPackageRegistryPath(bool global = false) {
        var basePath = global ? Path.Combine(_configuration.Paths.Packages, "global") : _configuration.Paths.Packages;
        Directory.CreateDirectory(basePath);
        return Path.Combine(basePath, "registry.json");
    }

    /// <summary>
    /// Gets the package installation directory
    /// </summary>
    public string GetPackageInstallPath(string packageName, bool global = false) {
        var basePath = global ? Path.Combine(_configuration.Paths.Packages, "global") : _configuration.Paths.Packages;
        var packagePath = Path.Combine(basePath, packageName);
        Directory.CreateDirectory(packagePath);
        return packagePath;
    }

    /// <summary>
    /// Loads the local package registry
    /// </summary>
    public async Task<LocalPackageRegistry> LoadLocalRegistryAsync(bool global = false) {
        var registryPath = GetPackageRegistryPath(global);

        if (!File.Exists(registryPath)) {
            var newRegistry = new LocalPackageRegistry();
            await SaveLocalRegistryAsync(newRegistry, global);
            return newRegistry;
        }

        try {
            var json = await File.ReadAllTextAsync(registryPath);
            var registry = JsonSerializer.Deserialize<LocalPackageRegistry>(json) ?? new LocalPackageRegistry();
            return registry;
        }
        catch (Exception ex) {
            _logger.LogWarning(ex, "Failed to load local package registry, creating new one");
            return new LocalPackageRegistry();
        }
    }

    /// <summary>
    /// Saves the local package registry
    /// </summary>
    public async Task SaveLocalRegistryAsync(LocalPackageRegistry registry, bool global = false) {
        var registryPath = GetPackageRegistryPath(global);
        var json = JsonSerializer.Serialize(registry, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(registryPath, json);
    }

    /// <summary>
    /// Downloads and extracts a package
    /// </summary>
    public async Task<string> DownloadAndExtractPackageAsync(
        DownloadPackageResponse downloadResponse,
        string packageName,
        string version,
        bool global = false,
        IProgress<DownloadProgress>? progress = null,
        CancellationToken cancellationToken = default) {
        if (string.IsNullOrEmpty(downloadResponse.DownloadUrl)) {
            throw new InvalidOperationException("Download URL is not available");
        }

        var installPath = GetPackageInstallPath(packageName, global);
        var versionPath = Path.Combine(installPath, version);
        var tempFile = Path.Combine(_configuration.Paths.Temp, $"{packageName}-{version}-{Guid.NewGuid()}.zip");

        try {
            Directory.CreateDirectory(_configuration.Paths.Temp);
            Directory.CreateDirectory(versionPath);

            // Download the package
            using var response = await _httpClient.GetAsync(downloadResponse.DownloadUrl, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            response.EnsureSuccessStatusCode();

            var totalBytes = response.Content.Headers.ContentLength ?? downloadResponse.FileSizeBytes ?? 0;
            var downloadedBytes = 0L;

            using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var fileStream = new FileStream(tempFile, FileMode.Create, FileAccess.Write, FileShare.None, 8192, FileOptions.Asynchronous);

            var buffer = new byte[8192];
            int bytesRead;

            while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0) {
                await fileStream.WriteAsync(buffer, 0, bytesRead, cancellationToken);
                downloadedBytes += bytesRead;

                if (totalBytes > 0) {
                    var progressPercentage = (double)downloadedBytes / totalBytes * 100;
                    progress?.Report(new DownloadProgress {
                        DownloadedBytes = downloadedBytes,
                        TotalBytes = totalBytes,
                        ProgressPercentage = progressPercentage
                    });
                }
            }

            // Extract the package
            ZipFile.ExtractToDirectory(tempFile, versionPath, overwriteFiles: true);

            _logger.LogInformation("Package {PackageName}@{Version} downloaded and extracted to {Path}",
                packageName, version, versionPath);

            return versionPath;
        }
        finally {
            // Clean up temp file
            if (File.Exists(tempFile)) {
                try {
                    File.Delete(tempFile);
                }
                catch { }
            }
        }
    }

    /// <summary>
    /// Registers a package in the local registry
    /// </summary>
    public async Task RegisterPackageAsync(
        string packageName,
        string version,
        string installPath,
        PackageInfoResponse packageInfo,
        bool global = false,
        bool isDevelopmentDependency = false) {
        var registry = await LoadLocalRegistryAsync(global);

        var localPackage = registry.InstalledPackages.FirstOrDefault(p => p.Name == packageName);
        if (localPackage == null) {
            localPackage = new LocalPackage {
                Name = packageName,
                Versions = []
            };
            registry.InstalledPackages.Add(localPackage);
        }

        // Remove existing version if present
        localPackage.Versions.RemoveAll(v => v.Version == version);

        // Add new version
        localPackage.Versions.Add(new LocalPackageVersion {
            Version = version,
            InstallPath = installPath,
            InstalledAt = DateTimeOffset.UtcNow,
            IsDevelopmentDependency = isDevelopmentDependency,
            PackageId = packageInfo.Id,
            TrustTier = packageInfo.TrustTier,
            SecurityGrade = packageInfo.SecurityGrade
        });

        await SaveLocalRegistryAsync(registry, global);

        _logger.LogInformation("Package {PackageName}@{Version} registered in local registry", packageName, version);
    }

    /// <summary>
    /// Checks if a package version is installed locally
    /// </summary>
    public async Task<LocalPackageVersion?> GetInstalledPackageAsync(string packageName, string? version = null, bool global = false) {
        var registry = await LoadLocalRegistryAsync(global);
        var localPackage = registry.InstalledPackages.FirstOrDefault(p => p.Name == packageName);

        if (localPackage == null)
            return null;

        if (string.IsNullOrEmpty(version)) {
            // Return the latest version
            return localPackage.Versions.OrderByDescending(v => v.InstalledAt).FirstOrDefault();
        }

        return localPackage.Versions.FirstOrDefault(v => v.Version == version);
    }

    /// <summary>
    /// Gets all installed packages
    /// </summary>
    public async Task<List<LocalPackage>> GetInstalledPackagesAsync(bool global = false) {
        var registry = await LoadLocalRegistryAsync(global);
        return registry.InstalledPackages;
    }

    /// <summary>
    /// Unregisters a package from the local registry
    /// </summary>
    public async Task UnregisterPackageAsync(string packageName, string? version = null, bool global = false) {
        var registry = await LoadLocalRegistryAsync(global);
        var localPackage = registry.InstalledPackages.FirstOrDefault(p => p.Name == packageName);

        if (localPackage == null) {
            _logger.LogWarning("Package {PackageName} not found in registry", packageName);
            return;
        }

        if (string.IsNullOrEmpty(version)) {
            // Remove all versions
            registry.InstalledPackages.Remove(localPackage);
            _logger.LogInformation("Package {PackageName} (all versions) unregistered from local registry", packageName);
        }
        else {
            // Remove specific version
            localPackage.Versions.RemoveAll(v => v.Version == version);

            // If no versions left, remove the package entirely
            if (!localPackage.Versions.Any()) {
                registry.InstalledPackages.Remove(localPackage);
            }

            _logger.LogInformation("Package {PackageName}@{Version} unregistered from local registry", packageName, version);
        }

        await SaveLocalRegistryAsync(registry, global);
    }

    /// <summary>
    /// Creates a backup of a package before modifications
    /// </summary>
    public async Task<string> CreatePackageBackupAsync(string packageName, string version, bool global = false) {
        var packagePath = GetPackageInstallPath(packageName, global);
        var versionPath = Path.Combine(packagePath, version);

        if (!Directory.Exists(versionPath)) {
            throw new DirectoryNotFoundException($"Package {packageName}@{version} not found at {versionPath}");
        }

        var backupDir = Path.Combine(_configuration.Paths.Temp, "backups");
        Directory.CreateDirectory(backupDir);

        var timestamp = DateTimeOffset.UtcNow.ToString("yyyyMMdd-HHmmss");
        var backupFileName = $"{packageName}-{version}-{timestamp}.zip";
        var backupPath = Path.Combine(backupDir, backupFileName);

        using var archive = ZipFile.Open(backupPath, ZipArchiveMode.Create);
        await AddDirectoryToArchiveAsync(archive, versionPath, packageName);

        _logger.LogInformation("Created backup for {PackageName}@{Version} at {BackupPath}", packageName, version, backupPath);
        return backupPath;
    }

    /// <summary>
    /// Removes a package directory and files
    /// </summary>
    public Task<long> RemovePackageFilesAsync(string packageName, string? version = null, bool global = false) {
        var packagePath = GetPackageInstallPath(packageName, global);
        long removedBytes = 0;

        if (string.IsNullOrEmpty(version)) {
            // Remove all versions
            if (Directory.Exists(packagePath)) {
                removedBytes = GetDirectorySize(packagePath);
                Directory.Delete(packagePath, recursive: true);
                _logger.LogInformation("Removed all files for package {PackageName}", packageName);
            }
        }
        else {
            // Remove specific version
            var versionPath = Path.Combine(packagePath, version);
            if (Directory.Exists(versionPath)) {
                removedBytes = GetDirectorySize(versionPath);
                Directory.Delete(versionPath, recursive: true);
                _logger.LogInformation("Removed files for package {PackageName}@{Version}", packageName, version);

                // If no versions left, remove package directory
                if (Directory.Exists(packagePath) && !Directory.EnumerateDirectories(packagePath).Any()) {
                    Directory.Delete(packagePath);
                }
            }
        }

        return Task.FromResult(removedBytes);
    }

    /// <summary>
    /// Checks the integrity of a package installation
    /// </summary>
    public async Task<bool> VerifyPackageIntegrityAsync(string packageName, string version, bool global = false) {
        var packagePath = GetPackageInstallPath(packageName, global);
        var versionPath = Path.Combine(packagePath, version);

        if (!Directory.Exists(versionPath)) {
            return false;
        }

        // Check for required files (manifest, etc.)
        var manifestPath = Path.Combine(versionPath, "mcp-manifest.json");
        if (!File.Exists(manifestPath)) {
            _logger.LogWarning("Package {PackageName}@{Version} missing manifest file", packageName, version);
            return false;
        }

        try {
            // Validate manifest JSON
            var manifestContent = await File.ReadAllTextAsync(manifestPath);
            using var manifestDoc = JsonDocument.Parse(manifestContent);

            // Basic validation - ensure required properties exist
            if (!manifestDoc.RootElement.TryGetProperty("name", out _) ||
                !manifestDoc.RootElement.TryGetProperty("version", out _)) {
                _logger.LogWarning("Package {PackageName}@{Version} has invalid manifest", packageName, version);
                return false;
            }

            return true;
        }
        catch (Exception ex) {
            _logger.LogWarning(ex, "Package {PackageName}@{Version} integrity check failed", packageName, version);
            return false;
        }
    }

    /// <summary>
    /// Gets the size of a directory in bytes
    /// </summary>
    private static long GetDirectorySize(string directoryPath) {
        if (!Directory.Exists(directoryPath)) {
            return 0;
        }

        var directoryInfo = new DirectoryInfo(directoryPath);
        return directoryInfo.EnumerateFiles("*", SearchOption.AllDirectories).Sum(file => file.Length);
    }

    /// <summary>
    /// Adds a directory to a zip archive recursively
    /// </summary>
    private static async Task AddDirectoryToArchiveAsync(ZipArchive archive, string directoryPath, string entryPrefix) {
        var directoryInfo = new DirectoryInfo(directoryPath);

        foreach (var file in directoryInfo.EnumerateFiles("*", SearchOption.AllDirectories)) {
            var relativePath = Path.GetRelativePath(directoryPath, file.FullName);
            var entryName = Path.Combine(entryPrefix, relativePath).Replace('\\', '/');

            var entry = archive.CreateEntry(entryName);

            using var entryStream = entry.Open();
            using var fileStream = file.OpenRead();

            await fileStream.CopyToAsync(entryStream);
        }
    }

    /// <summary>
    /// Gets the package lock file path
    /// </summary>
    public string GetPackageLockPath(bool global = false) {
        var basePath = global ? Path.Combine(_configuration.Paths.Packages, "global") : _configuration.Paths.Packages;
        Directory.CreateDirectory(basePath);
        return Path.Combine(basePath, "package-lock.json");
    }

    /// <summary>
    /// Loads the package lock file
    /// </summary>
    public async Task<PackageLockFile> LoadPackageLockAsync(bool global = false) {
        var lockPath = GetPackageLockPath(global);

        if (!File.Exists(lockPath)) {
            var newLock = new PackageLockFile();
            await SavePackageLockAsync(newLock, global);
            return newLock;
        }

        try {
            var json = await File.ReadAllTextAsync(lockPath);
            var lockFile = JsonSerializer.Deserialize<PackageLockFile>(json) ?? new PackageLockFile();
            return lockFile;
        }
        catch (Exception ex) {
            _logger.LogWarning(ex, "Failed to load package lock file, creating new one");
            return new PackageLockFile();
        }
    }

    /// <summary>
    /// Saves the package lock file
    /// </summary>
    public async Task SavePackageLockAsync(PackageLockFile lockFile, bool global = false) {
        var lockPath = GetPackageLockPath(global);
        var json = JsonSerializer.Serialize(lockFile, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(lockPath, json);
    }

    /// <summary>
    /// Records a transaction in the transaction log
    /// </summary>
    public async Task RecordTransactionAsync(PackageTransaction transaction, bool global = false) {
        var logPath = GetTransactionLogPath(global);
        var json = JsonSerializer.Serialize(transaction, new JsonSerializerOptions { WriteIndented = false });

        // Append to log file
        await File.AppendAllTextAsync(logPath, json + Environment.NewLine);

        _logger.LogInformation("Recorded transaction: {Type} {Package}@{Version}",
            transaction.Type, transaction.PackageName, transaction.Version);
    }

    /// <summary>
    /// Gets the transaction log file path
    /// </summary>
    private string GetTransactionLogPath(bool global = false) {
        var basePath = global ? Path.Combine(_configuration.Paths.Packages, "global") : _configuration.Paths.Packages;
        Directory.CreateDirectory(basePath);
        return Path.Combine(basePath, "transactions.log");
    }
}

/// <summary>
/// Local package registry model
/// </summary>
public class LocalPackageRegistry {
    public List<LocalPackage> InstalledPackages { get; set; } = [];
    public DateTimeOffset LastUpdated { get; set; } = DateTimeOffset.UtcNow;
}

/// <summary>
/// Local package information
/// </summary>
public class LocalPackage {
    public string Name { get; set; } = string.Empty;
    public List<LocalPackageVersion> Versions { get; set; } = [];
}

/// <summary>
/// Local package version information
/// </summary>
public class LocalPackageVersion {
    public string Version { get; set; } = string.Empty;
    public string InstallPath { get; set; } = string.Empty;
    public DateTimeOffset InstalledAt { get; set; }
    public bool IsDevelopmentDependency { get; set; }
    public Guid PackageId { get; set; }
    public string? TrustTier { get; set; }
    public string? SecurityGrade { get; set; }
}

/// <summary>
/// Download progress information
/// </summary>
public class DownloadProgress {
    public long DownloadedBytes { get; set; }
    public long TotalBytes { get; set; }
    public double ProgressPercentage { get; set; }
}

/// <summary>
/// Package lock file model for dependency management
/// </summary>
public class PackageLockFile {
    public int Version { get; set; } = 1;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    public Dictionary<string, LockedPackage> Packages { get; set; } = [];
}

/// <summary>
/// Locked package information with exact versions and dependencies
/// </summary>
public class LockedPackage {
    public string Version { get; set; } = string.Empty;
    public string? ResolvedVersion { get; set; }
    public Dictionary<string, string> Dependencies { get; set; } = [];
    public bool IsDevelopmentDependency { get; set; }
    public string? Integrity { get; set; }
    public DateTimeOffset InstalledAt { get; set; } = DateTimeOffset.UtcNow;
}

/// <summary>
/// Package transaction log entry
/// </summary>
public class PackageTransaction {
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Type { get; set; } = string.Empty; // Install, Update, Uninstall
    public string PackageName { get; set; } = string.Empty;
    public string? Version { get; set; }
    public string? PreviousVersion { get; set; }
    public bool IsGlobal { get; set; }
    public bool Success { get; set; }
    public string? Error { get; set; }
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
    public TimeSpan Duration { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = [];
}