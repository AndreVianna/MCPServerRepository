using System.Text.Json.Serialization;

namespace MCPHub.CommandLineApp.Configuration;

/// <summary>
/// Configuration settings for the MCPM CLI application
/// </summary>
public class McpmConfiguration {
    /// <summary>
    /// Default registry configuration settings (for backward compatibility)
    /// </summary>
    public RegistryConfiguration Registry { get; set; } = new();

    /// <summary>
    /// Multiple registry configurations (preferred approach)
    /// </summary>
    public Dictionary<string, RegistryConfiguration> Registries { get; set; } = [];

    /// <summary>
    /// Authentication configuration settings (deprecated - use credential store instead)
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    [Obsolete("Use ICredentialStore for secure authentication storage")]
    public AuthConfiguration Auth { get; set; } = new();

    /// <summary>
    /// Security configuration settings
    /// </summary>
    public SecurityConfiguration Security { get; set; } = new();

    /// <summary>
    /// User interface configuration settings
    /// </summary>
    public UiConfiguration Ui { get; set; } = new();

    /// <summary>
    /// Path configuration settings
    /// </summary>
    public PathConfiguration Paths { get; set; } = new();

    /// <summary>
    /// Cache configuration settings
    /// </summary>
    public CacheConfiguration Cache { get; set; } = new();

    /// <summary>
    /// Gets the default registry URL for operations
    /// </summary>
    public string DefaultRegistryUrl => Registry.Url;

    /// <summary>
    /// Gets all configured registries including the default one
    /// </summary>
    /// <returns>Dictionary of registry configurations</returns>
    public IReadOnlyDictionary<string, RegistryConfiguration> GetAllRegistries() {
        var allRegistries = new Dictionary<string, RegistryConfiguration>(Registries);

        // Add default registry if not already present
        if (!allRegistries.ContainsKey(Registry.Url)) {
            allRegistries[Registry.Url] = Registry;
        }

        return allRegistries;
    }

    /// <summary>
    /// Gets a specific registry configuration by URL
    /// </summary>
    /// <param name="registryUrl">The registry URL to look up</param>
    /// <returns>Registry configuration or default if not found</returns>
    public RegistryConfiguration GetRegistryConfiguration(string registryUrl) {
        if (string.IsNullOrWhiteSpace(registryUrl))
            return Registry;

        // Check if it's the default registry
        if (registryUrl.Equals(Registry.Url, StringComparison.OrdinalIgnoreCase))
            return Registry;

        // Check configured registries
        return Registries.TryGetValue(registryUrl, out var config) ? config : Registry;
    }
}

/// <summary>
/// Registry configuration settings
/// </summary>
public class RegistryConfiguration {
    /// <summary>
    /// Registry base URL
    /// </summary>
    public string Url { get; set; } = "https://api.mcphub.dev";

    /// <summary>
    /// Display name for this registry
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Request timeout in milliseconds
    /// </summary>
    public int Timeout { get; set; } = 30000;

    /// <summary>
    /// Number of retry attempts for failed requests
    /// </summary>
    public int Retries { get; set; } = 3;

    /// <summary>
    /// API version to use
    /// </summary>
    public string ApiVersion { get; set; } = "1.0";

    /// <summary>
    /// Whether this registry requires authentication for read operations
    /// </summary>
    public bool RequiresAuthentication { get; set; } = false;

    /// <summary>
    /// Whether this registry supports private packages
    /// </summary>
    public bool SupportsPrivatePackages { get; set; } = true;

    /// <summary>
    /// Whether this registry supports package publishing
    /// </summary>
    public bool SupportsPublishing { get; set; } = true;

    /// <summary>
    /// Authentication endpoint URL (if different from base URL)
    /// </summary>
    public string? AuthEndpoint { get; set; }

    /// <summary>
    /// Supported authentication methods
    /// </summary>
    public IList<string> SupportedAuthMethods { get; set; } = ["bearer", "api-key"];

    /// <summary>
    /// Whether to verify SSL certificates for this registry
    /// </summary>
    public bool VerifySSL { get; set; } = true;

    /// <summary>
    /// Additional headers to send with requests to this registry
    /// </summary>
    public Dictionary<string, string> Headers { get; set; } = [];

    /// <summary>
    /// Gets the effective display name for this registry
    /// </summary>
    public string DisplayName => Name ?? Url;

    /// <summary>
    /// Gets the authentication endpoint URL
    /// </summary>
    public string GetAuthEndpoint() => AuthEndpoint ?? $"{Url.TrimEnd('/')}/auth";
}

/// <summary>
/// Authentication configuration settings
/// </summary>
public class AuthConfiguration {
    /// <summary>
    /// Authentication token (encrypted)
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Token { get; set; }

    /// <summary>
    /// Username for the authenticated user
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Username { get; set; }
}

/// <summary>
/// Security configuration settings
/// </summary>
public class SecurityConfiguration {
    /// <summary>
    /// Whether to automatically verify packages during install
    /// </summary>
    public bool AutoVerify { get; set; } = true;

    /// <summary>
    /// Minimum trust tier required for installation
    /// </summary>
    public string TrustTierMinimum { get; set; } = "Community";

    /// <summary>
    /// Sandbox timeout in seconds
    /// </summary>
    public int SandboxTimeout { get; set; } = 300;

    /// <summary>
    /// Whether to allow insecure connections (for development only)
    /// </summary>
    public bool AllowInsecureConnections { get; set; } = false;
}

/// <summary>
/// User interface configuration settings
/// </summary>
public class UiConfiguration {
    /// <summary>
    /// Whether to use colored output
    /// </summary>
    public bool ColorOutput { get; set; } = true;

    /// <summary>
    /// Whether to show progress bars
    /// </summary>
    public bool ProgressBars { get; set; } = true;

    /// <summary>
    /// Whether to show verbose error messages
    /// </summary>
    public bool VerboseErrors { get; set; } = false;

    /// <summary>
    /// Whether to run in non-interactive mode (useful for CI/CD environments)
    /// </summary>
    public bool NonInteractive { get; set; } = false;

    /// <summary>
    /// Default output format for commands
    /// </summary>
    public string DefaultFormat { get; set; } = "table";

    /// <summary>
    /// Number of items to show per page by default
    /// </summary>
    public int DefaultPageSize { get; set; } = 20;
}

/// <summary>
/// Path configuration settings
/// </summary>
public class PathConfiguration {
    /// <summary>
    /// Cache directory path
    /// </summary>
    public string Cache { get; set; } = GetDefaultPath("cache");

    /// <summary>
    /// Packages directory path
    /// </summary>
    public string Packages { get; set; } = GetDefaultPath("packages");

    /// <summary>
    /// Temporary files directory path
    /// </summary>
    public string Temp { get; set; } = GetDefaultPath("temp");

    /// <summary>
    /// Configuration directory path
    /// </summary>
    public string Config { get; set; } = GetDefaultPath("");

    private static string GetDefaultPath(string subdirectory) {
        var homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var mcpmDir = Path.Combine(homeDir, ".mcpm");

        return string.IsNullOrEmpty(subdirectory)
            ? mcpmDir
            : Path.Combine(mcpmDir, subdirectory);
    }
}

/// <summary>
/// Cache configuration settings
/// </summary>
public class CacheConfiguration {
    /// <summary>
    /// Whether caching is enabled globally
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Maximum cache size in bytes (default: 500MB)
    /// </summary>
    public long MaxSizeBytes { get; set; } = 500L * 1024 * 1024;

    /// <summary>
    /// Human-readable maximum cache size
    /// </summary>
    public string MaxSizeFormatted => FormatBytes(MaxSizeBytes);

    /// <summary>
    /// Default expiration time for cache entries
    /// </summary>
    public TimeSpan DefaultExpiration { get; set; } = TimeSpan.FromHours(24);

    /// <summary>
    /// Package information cache settings
    /// </summary>
    public CacheTypeConfiguration PackageInfo { get; set; } = new() {
        Enabled = true,
        Expiration = TimeSpan.FromHours(12),
        MaxEntries = 1000,
    };

    /// <summary>
    /// Package versions cache settings
    /// </summary>
    public CacheTypeConfiguration PackageVersions { get; set; } = new() {
        Enabled = true,
        Expiration = TimeSpan.FromHours(6),
        MaxEntries = 500,
    };

    /// <summary>
    /// Security summary cache settings
    /// </summary>
    public CacheTypeConfiguration SecuritySummary { get; set; } = new() {
        Enabled = true,
        Expiration = TimeSpan.FromHours(24),
        MaxEntries = 1000,
    };

    /// <summary>
    /// Trust tier cache settings
    /// </summary>
    public CacheTypeConfiguration TrustTier { get; set; } = new() {
        Enabled = true,
        Expiration = TimeSpan.FromHours(12),
        MaxEntries = 1000,
    };

    /// <summary>
    /// Search results cache settings
    /// </summary>
    public CacheTypeConfiguration SearchResults { get; set; } = new() {
        Enabled = true,
        Expiration = TimeSpan.FromMinutes(30),
        MaxEntries = 200,
    };

    /// <summary>
    /// Package dependencies cache settings
    /// </summary>
    public CacheTypeConfiguration Dependencies { get; set; } = new() {
        Enabled = true,
        Expiration = TimeSpan.FromHours(6),
        MaxEntries = 500,
    };

    /// <summary>
    /// Whether to enable background maintenance
    /// </summary>
    public bool BackgroundMaintenance { get; set; } = true;

    /// <summary>
    /// Interval for background maintenance operations
    /// </summary>
    public TimeSpan MaintenanceInterval { get; set; } = TimeSpan.FromHours(4);

    /// <summary>
    /// Whether to enable cache compression
    /// </summary>
    public bool EnableCompression { get; set; } = true;

    /// <summary>
    /// Compression level (1-9, where 9 is maximum compression)
    /// </summary>
    public int CompressionLevel { get; set; } = 6;

    /// <summary>
    /// Whether to enable offline mode support
    /// </summary>
    public bool OfflineModeEnabled { get; set; } = true;

    /// <summary>
    /// How long to keep offline data before warning about staleness
    /// </summary>
    public TimeSpan OfflineDataWarningThreshold { get; set; } = TimeSpan.FromDays(7);

    /// <summary>
    /// Whether to preload popular packages for offline use
    /// </summary>
    public bool PreloadPopularPackages { get; set; } = true;

    /// <summary>
    /// Number of popular packages to preload
    /// </summary>
    public int PopularPackagesPreloadCount { get; set; } = 50;

    /// <summary>
    /// Cache cleanup thresholds
    /// </summary>
    public CacheCleanupConfiguration Cleanup { get; set; } = new();

    private static string FormatBytes(long bytes) {
        string[] sizes = ["B", "KB", "MB", "GB", "TB"];
        var order = 0;
        double size = bytes;

        while (size >= 1024 && order < sizes.Length - 1) {
            order++;
            size /= 1024;
        }

        return $"{size:0.##} {sizes[order]}";
    }
}

/// <summary>
/// Configuration for a specific type of cache
/// </summary>
public class CacheTypeConfiguration {
    /// <summary>
    /// Whether this cache type is enabled
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Expiration time for entries of this type
    /// </summary>
    public TimeSpan Expiration { get; set; } = TimeSpan.FromHours(24);

    /// <summary>
    /// Maximum number of entries for this cache type
    /// </summary>
    public int MaxEntries { get; set; } = 1000;

    /// <summary>
    /// Whether to enable LRU (Least Recently Used) eviction for this cache type
    /// </summary>
    public bool EnableLruEviction { get; set; } = true;

    /// <summary>
    /// Priority for this cache type when cleaning up (1-10, where 10 is highest priority to keep)
    /// </summary>
    public int Priority { get; set; } = 5;
}

/// <summary>
/// Cache cleanup configuration
/// </summary>
public class CacheCleanupConfiguration {
    /// <summary>
    /// Cache size threshold to trigger cleanup (as percentage of max size)
    /// </summary>
    public double CleanupThresholdPercent { get; set; } = 80.0;

    /// <summary>
    /// Target size after cleanup (as percentage of max size)
    /// </summary>
    public double CleanupTargetPercent { get; set; } = 60.0;

    /// <summary>
    /// Whether to remove expired entries during cleanup
    /// </summary>
    public bool RemoveExpiredEntries { get; set; } = true;

    /// <summary>
    /// Whether to use LRU eviction during cleanup
    /// </summary>
    public bool UseLruEviction { get; set; } = true;

    /// <summary>
    /// Maximum time to spend on cleanup operations
    /// </summary>
    public TimeSpan MaxCleanupTime { get; set; } = TimeSpan.FromMinutes(5);
}