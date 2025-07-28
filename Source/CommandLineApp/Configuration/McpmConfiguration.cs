using System.Text.Json.Serialization;

namespace MCPHub.CommandLineApp.Configuration;

/// <summary>
/// Configuration settings for the MCPM CLI application
/// </summary>
public class McpmConfiguration
{
    /// <summary>
    /// Registry configuration settings
    /// </summary>
    public RegistryConfiguration Registry { get; set; } = new();

    /// <summary>
    /// Authentication configuration settings
    /// </summary>
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
}

/// <summary>
/// Registry configuration settings
/// </summary>
public class RegistryConfiguration
{
    /// <summary>
    /// Registry base URL
    /// </summary>
    public string Url { get; set; } = "https://api.mcphub.dev";

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
}

/// <summary>
/// Authentication configuration settings
/// </summary>
public class AuthConfiguration
{
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
public class SecurityConfiguration
{
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
public class UiConfiguration
{
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
public class PathConfiguration
{
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

    private static string GetDefaultPath(string subdirectory)
    {
        var homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var mcpmDir = Path.Combine(homeDir, ".mcpm");
        
        return string.IsNullOrEmpty(subdirectory) 
            ? mcpmDir 
            : Path.Combine(mcpmDir, subdirectory);
    }
}