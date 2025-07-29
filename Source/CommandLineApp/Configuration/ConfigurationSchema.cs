namespace MCPHub.CommandLineApp.Configuration;

/// <summary>
/// Defines the schema for all available configuration keys
/// </summary>
public static class ConfigurationSchema {
    /// <summary>
    /// All available configuration keys with their metadata
    /// </summary>
    public static readonly Dictionary<string, ConfigurationKey> Keys = new() {
        // Registry Configuration
        ["registry.url"] = new ConfigurationKey {
            Key = "registry.url",
            Description = "Default registry URL",
            ValueType = typeof(string),
            DefaultValue = "https://api.mcphub.dev",
            Category = "Registry",
            Pattern = @"^https?://.+"
        },
        ["registry.name"] = new ConfigurationKey {
            Key = "registry.name",
            Description = "Display name for the default registry",
            ValueType = typeof(string),
            DefaultValue = null,
            Category = "Registry"
        },
        ["registry.timeout"] = new ConfigurationKey {
            Key = "registry.timeout",
            Description = "Request timeout in milliseconds",
            ValueType = typeof(int),
            DefaultValue = 30000,
            MinValue = 1000,
            MaxValue = 300000,
            Category = "Registry"
        },
        ["registry.retries"] = new ConfigurationKey {
            Key = "registry.retries",
            Description = "Number of retry attempts for failed requests",
            ValueType = typeof(int),
            DefaultValue = 3,
            MinValue = 0,
            MaxValue = 10,
            Category = "Registry"
        },
        ["registry.apiVersion"] = new ConfigurationKey {
            Key = "registry.apiVersion",
            Description = "API version to use",
            ValueType = typeof(string),
            DefaultValue = "1.0",
            Category = "Registry"
        },
        ["registry.requiresAuthentication"] = new ConfigurationKey {
            Key = "registry.requiresAuthentication",
            Description = "Whether this registry requires authentication for read operations",
            ValueType = typeof(bool),
            DefaultValue = false,
            Category = "Registry"
        },
        ["registry.verifySSL"] = new ConfigurationKey {
            Key = "registry.verifySSL",
            Description = "Whether to verify SSL certificates",
            ValueType = typeof(bool),
            DefaultValue = true,
            Category = "Registry"
        },

        // Security Configuration
        ["security.autoVerify"] = new ConfigurationKey {
            Key = "security.autoVerify",
            Description = "Automatically verify packages during install",
            ValueType = typeof(bool),
            DefaultValue = true,
            Category = "Security"
        },
        ["security.trustTierMinimum"] = new ConfigurationKey {
            Key = "security.trustTierMinimum",
            Description = "Minimum trust tier required for installation",
            ValueType = typeof(string),
            DefaultValue = "Community",
            ValidValues = new[] { "Unverified", "Community", "Professional", "Enterprise" },
            Category = "Security"
        },
        ["security.sandboxTimeout"] = new ConfigurationKey {
            Key = "security.sandboxTimeout",
            Description = "Sandbox timeout in seconds",
            ValueType = typeof(int),
            DefaultValue = 300,
            MinValue = 30,
            MaxValue = 3600,
            Category = "Security"
        },
        ["security.allowInsecureConnections"] = new ConfigurationKey {
            Key = "security.allowInsecureConnections",
            Description = "Allow insecure connections (for development only)",
            ValueType = typeof(bool),
            DefaultValue = false,
            Category = "Security"
        },

        // UI Configuration
        ["ui.colorOutput"] = new ConfigurationKey {
            Key = "ui.colorOutput",
            Description = "Use colored output",
            ValueType = typeof(bool),
            DefaultValue = true,
            Category = "UI"
        },
        ["ui.progressBars"] = new ConfigurationKey {
            Key = "ui.progressBars",
            Description = "Show progress bars",
            ValueType = typeof(bool),
            DefaultValue = true,
            Category = "UI"
        },
        ["ui.verboseErrors"] = new ConfigurationKey {
            Key = "ui.verboseErrors",
            Description = "Show verbose error messages",
            ValueType = typeof(bool),
            DefaultValue = false,
            Category = "UI"
        },
        ["ui.defaultFormat"] = new ConfigurationKey {
            Key = "ui.defaultFormat",
            Description = "Default output format for commands",
            ValueType = typeof(string),
            DefaultValue = "table",
            ValidValues = new[] { "table", "json", "detailed" },
            Category = "UI"
        },
        ["ui.defaultPageSize"] = new ConfigurationKey {
            Key = "ui.defaultPageSize",
            Description = "Number of items to show per page by default",
            ValueType = typeof(int),
            DefaultValue = 20,
            MinValue = 1,
            MaxValue = 100,
            Category = "UI"
        },

        // Path Configuration
        ["paths.cache"] = new ConfigurationKey {
            Key = "paths.cache",
            Description = "Cache directory path",
            ValueType = typeof(string),
            DefaultValue = GetDefaultPath("cache"),
            Category = "Paths"
        },
        ["paths.packages"] = new ConfigurationKey {
            Key = "paths.packages",
            Description = "Packages directory path",
            ValueType = typeof(string),
            DefaultValue = GetDefaultPath("packages"),
            Category = "Paths"
        },
        ["paths.temp"] = new ConfigurationKey {
            Key = "paths.temp",
            Description = "Temporary files directory path",
            ValueType = typeof(string),
            DefaultValue = GetDefaultPath("temp"),
            Category = "Paths"
        },
        ["paths.config"] = new ConfigurationKey {
            Key = "paths.config",
            Description = "Configuration directory path",
            ValueType = typeof(string),
            DefaultValue = GetDefaultPath(""),
            Category = "Paths"
        }
    };

    /// <summary>
    /// Gets all configuration keys grouped by category
    /// </summary>
    public static IEnumerable<IGrouping<string?, ConfigurationKey>> GetKeysByCategory() => Keys.Values.GroupBy(k => k.Category ?? "Other");

    /// <summary>
    /// Gets a configuration key by its key name (case insensitive)
    /// </summary>
    public static ConfigurationKey? GetKey(string keyName) => Keys.FirstOrDefault(kvp =>
            string.Equals(kvp.Key, keyName, StringComparison.OrdinalIgnoreCase)).Value;

    /// <summary>
    /// Searches for configuration keys by partial match
    /// </summary>
    public static IEnumerable<ConfigurationKey> SearchKeys(string searchTerm) => Keys.Values.Where(k =>
            k.Key.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
            k.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));

    private static string GetDefaultPath(string subdirectory) {
        var homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var mcpmDir = Path.Combine(homeDir, ".mcpm");

        return string.IsNullOrEmpty(subdirectory)
            ? mcpmDir
            : Path.Combine(mcpmDir, subdirectory);
    }
}