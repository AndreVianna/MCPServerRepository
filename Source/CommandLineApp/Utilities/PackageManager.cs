using System.IO.Compression;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using MCPHub.CommandLineApp.Configuration;
using MCPHub.CommandLineApp.Models;

namespace MCPHub.CommandLineApp.Utilities;

/// <summary>
/// Manages local package installation and registry
/// </summary>
public class PackageManager
{
    private readonly ILogger<PackageManager> _logger;
    private readonly McpmConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public PackageManager(ILogger<PackageManager> logger, McpmConfiguration configuration, HttpClient httpClient)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    /// <summary>
    /// Gets the local package registry file path
    /// </summary>
    public string GetPackageRegistryPath(bool global = false)
    {
        var basePath = global ? Path.Combine(_configuration.Paths.Packages, "global") : _configuration.Paths.Packages;
        Directory.CreateDirectory(basePath);
        return Path.Combine(basePath, "registry.json");
    }

    /// <summary>
    /// Gets the package installation directory
    /// </summary>
    public string GetPackageInstallPath(string packageName, bool global = false)
    {
        var basePath = global ? Path.Combine(_configuration.Paths.Packages, "global") : _configuration.Paths.Packages;
        var packagePath = Path.Combine(basePath, packageName);
        Directory.CreateDirectory(packagePath);
        return packagePath;
    }

    /// <summary>
    /// Loads the local package registry
    /// </summary>
    public async Task<LocalPackageRegistry> LoadLocalRegistryAsync(bool global = false)
    {
        var registryPath = GetPackageRegistryPath(global);
        
        if (!File.Exists(registryPath))
        {
            var newRegistry = new LocalPackageRegistry();
            await SaveLocalRegistryAsync(newRegistry, global);
            return newRegistry;
        }

        try
        {
            var json = await File.ReadAllTextAsync(registryPath);
            var registry = JsonSerializer.Deserialize<LocalPackageRegistry>(json) ?? new LocalPackageRegistry();
            return registry;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to load local package registry, creating new one");
            return new LocalPackageRegistry();
        }
    }

    /// <summary>
    /// Saves the local package registry
    /// </summary>
    public async Task SaveLocalRegistryAsync(LocalPackageRegistry registry, bool global = false)
    {
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
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(downloadResponse.DownloadUrl))
        {
            throw new InvalidOperationException("Download URL is not available");
        }

        var installPath = GetPackageInstallPath(packageName, global);
        var versionPath = Path.Combine(installPath, version);
        var tempFile = Path.Combine(_configuration.Paths.Temp, $"{packageName}-{version}-{Guid.NewGuid()}.zip");

        try
        {
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

            while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
            {
                await fileStream.WriteAsync(buffer, 0, bytesRead, cancellationToken);
                downloadedBytes += bytesRead;

                if (totalBytes > 0)
                {
                    var progressPercentage = (double)downloadedBytes / totalBytes * 100;
                    progress?.Report(new DownloadProgress
                    {
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
        finally
        {
            // Clean up temp file
            if (File.Exists(tempFile))
            {
                try { File.Delete(tempFile); } catch { }
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
        bool isDevelopmentDependency = false)
    {
        var registry = await LoadLocalRegistryAsync(global);
        
        var localPackage = registry.InstalledPackages.FirstOrDefault(p => p.Name == packageName);
        if (localPackage == null)
        {
            localPackage = new LocalPackage
            {
                Name = packageName,
                Versions = new List<LocalPackageVersion>()
            };
            registry.InstalledPackages.Add(localPackage);
        }

        // Remove existing version if present
        localPackage.Versions.RemoveAll(v => v.Version == version);

        // Add new version
        localPackage.Versions.Add(new LocalPackageVersion
        {
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
    public async Task<LocalPackageVersion?> GetInstalledPackageAsync(string packageName, string? version = null, bool global = false)
    {
        var registry = await LoadLocalRegistryAsync(global);
        var localPackage = registry.InstalledPackages.FirstOrDefault(p => p.Name == packageName);
        
        if (localPackage == null)
            return null;

        if (string.IsNullOrEmpty(version))
        {
            // Return the latest version
            return localPackage.Versions.OrderByDescending(v => v.InstalledAt).FirstOrDefault();
        }

        return localPackage.Versions.FirstOrDefault(v => v.Version == version);
    }

    /// <summary>
    /// Gets all installed packages
    /// </summary>
    public async Task<List<LocalPackage>> GetInstalledPackagesAsync(bool global = false)
    {
        var registry = await LoadLocalRegistryAsync(global);
        return registry.InstalledPackages;
    }
}

/// <summary>
/// Local package registry model
/// </summary>
public class LocalPackageRegistry
{
    public List<LocalPackage> InstalledPackages { get; set; } = new();
    public DateTimeOffset LastUpdated { get; set; } = DateTimeOffset.UtcNow;
}

/// <summary>
/// Local package information
/// </summary>
public class LocalPackage
{
    public string Name { get; set; } = string.Empty;
    public List<LocalPackageVersion> Versions { get; set; } = new();
}

/// <summary>
/// Local package version information
/// </summary>
public class LocalPackageVersion
{
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
public class DownloadProgress
{
    public long DownloadedBytes { get; set; }
    public long TotalBytes { get; set; }
    public double ProgressPercentage { get; set; }
}