using System.Text.Json;

namespace MCPHub.CommandLineApp.Configuration;

/// <summary>
/// Manages MCPM CLI configuration loading, saving, and validation
/// </summary>
public interface IMcpmConfigurationManager {
    /// <summary>
    /// Loads the configuration from file and environment variables
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The loaded configuration</returns>
    Task<McpmConfiguration> LoadConfigurationAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves the configuration to file
    /// </summary>
    /// <param name="configuration">Configuration to save</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SaveConfigurationAsync(McpmConfiguration configuration, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the configuration file path
    /// </summary>
    /// <returns>Full path to the configuration file</returns>
    string GetConfigurationPath();

    /// <summary>
    /// Validates the configuration and ensures required directories exist
    /// </summary>
    /// <param name="configuration">Configuration to validate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task ValidateConfigurationAsync(McpmConfiguration configuration, CancellationToken cancellationToken = default);
}

/// <summary>
/// Implementation of configuration management for MCPM CLI
/// </summary>
public class McpmConfigurationManager : IMcpmConfigurationManager {
    private readonly ILogger<McpmConfigurationManager> _logger;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly string _configurationPath;

    public McpmConfigurationManager(ILogger<McpmConfigurationManager> logger) {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _jsonOptions = new JsonSerializerOptions {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            // Enable reflection fallback for JSON serialization
            TypeInfoResolver = new System.Text.Json.Serialization.Metadata.DefaultJsonTypeInfoResolver(),
        };

        _configurationPath = GetConfigurationPath();
    }

    /// <inheritdoc />
    public async Task<McpmConfiguration> LoadConfigurationAsync(CancellationToken cancellationToken = default) {
        try {
            _logger.LogDebug("Loading configuration from: {ConfigPath}", _configurationPath);

            var configuration = new McpmConfiguration();

            // Load from file if it exists
            if (File.Exists(_configurationPath)) {
                var jsonContent = await File.ReadAllTextAsync(_configurationPath, cancellationToken);

                if (!string.IsNullOrWhiteSpace(jsonContent)) {
                    var fileConfig = JsonSerializer.Deserialize<McpmConfiguration>(jsonContent, _jsonOptions);
                    if (fileConfig != null) {
                        configuration = fileConfig;
                        _logger.LogDebug("Configuration loaded from file successfully");
                    }
                }
            }
            else {
                _logger.LogDebug("Configuration file not found, using defaults");
            }

            // Override with environment variables
            ApplyEnvironmentVariables(configuration);

            // Validate and ensure directories exist
            await ValidateConfigurationAsync(configuration, cancellationToken);

            _logger.LogInformation("Configuration loaded successfully");
            return configuration;
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Failed to load configuration from: {ConfigPath}", _configurationPath);

            // Return default configuration on error
            var defaultConfig = new McpmConfiguration();
            ApplyEnvironmentVariables(defaultConfig);
            await ValidateConfigurationAsync(defaultConfig, cancellationToken);

            return defaultConfig;
        }
    }

    /// <inheritdoc />
    public async Task SaveConfigurationAsync(McpmConfiguration configuration, CancellationToken cancellationToken = default) {
        try {
            _logger.LogDebug("Saving configuration to: {ConfigPath}", _configurationPath);

            // Ensure directory exists
            var configDir = Path.GetDirectoryName(_configurationPath);
            if (!string.IsNullOrEmpty(configDir) && !Directory.Exists(configDir)) {
                Directory.CreateDirectory(configDir);
                _logger.LogDebug("Created configuration directory: {ConfigDir}", configDir);
            }

            var jsonContent = JsonSerializer.Serialize(configuration, _jsonOptions);
            await File.WriteAllTextAsync(_configurationPath, jsonContent, cancellationToken);

            _logger.LogInformation("Configuration saved successfully to: {ConfigPath}", _configurationPath);
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Failed to save configuration to: {ConfigPath}", _configurationPath);
            throw;
        }
    }

    /// <inheritdoc />
    public string GetConfigurationPath() {
        var homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        return Path.Combine(homeDir, ".mcpm", "config.json");
    }

    /// <inheritdoc />
    public async Task ValidateConfigurationAsync(McpmConfiguration configuration, CancellationToken cancellationToken = default) {
        _logger.LogDebug("Validating configuration");

        // Validate registry URL
        if (!Uri.TryCreate(configuration.Registry.Url, UriKind.Absolute, out var registryUri)) {
            _logger.LogWarning("Invalid registry URL: {Url}, using default", configuration.Registry.Url);
            configuration.Registry.Url = "https://api.mcphub.dev";
        }

        // Ensure required directories exist
        var paths = new[]
        {
            configuration.Paths.Cache,
            configuration.Paths.Packages,
            configuration.Paths.Temp,
            configuration.Paths.Config,
        };

        foreach (var path in paths) {
            try {
                if (!Directory.Exists(path)) {
                    Directory.CreateDirectory(path);
                    _logger.LogDebug("Created directory: {Path}", path);
                }
            }
            catch (Exception ex) {
                _logger.LogWarning(ex, "Failed to create directory: {Path}", path);
            }
        }

        // Validate numeric ranges
        if (configuration.Registry.Timeout is < 1000 or > 300000) {
            _logger.LogWarning("Invalid timeout value: {Timeout}, using default", configuration.Registry.Timeout);
            configuration.Registry.Timeout = 30000;
        }

        if (configuration.Registry.Retries is < 0 or > 10) {
            _logger.LogWarning("Invalid retries value: {Retries}, using default", configuration.Registry.Retries);
            configuration.Registry.Retries = 3;
        }

        if (configuration.Security.SandboxTimeout is < 30 or > 3600) {
            _logger.LogWarning("Invalid sandbox timeout: {Timeout}, using default", configuration.Security.SandboxTimeout);
            configuration.Security.SandboxTimeout = 300;
        }

        if (configuration.Ui.DefaultPageSize is < 1 or > 100) {
            _logger.LogWarning("Invalid default page size: {PageSize}, using default", configuration.Ui.DefaultPageSize);
            configuration.Ui.DefaultPageSize = 20;
        }

        _logger.LogDebug("Configuration validation completed");
        await Task.CompletedTask;
    }

    private void ApplyEnvironmentVariables(McpmConfiguration configuration) {
        _logger.LogDebug("Applying environment variable overrides");

        // Registry configuration
        if (Environment.GetEnvironmentVariable("MCPM_API_URL") is string apiUrl && !string.IsNullOrWhiteSpace(apiUrl)) {
            configuration.Registry.Url = apiUrl;
            _logger.LogDebug("Registry URL overridden by environment variable");
        }

        if (Environment.GetEnvironmentVariable("MCPM_API_TIMEOUT") is string timeoutStr &&
            int.TryParse(timeoutStr, out var timeout)) {
            configuration.Registry.Timeout = timeout;
            _logger.LogDebug("Registry timeout overridden by environment variable");
        }

        // TODO: Replace with ICredentialStore when authentication is fully implemented
        // Authentication configuration - temporarily disabled to avoid obsolete API usage
        if (Environment.GetEnvironmentVariable("MCPM_AUTH_TOKEN") is string authToken && !string.IsNullOrWhiteSpace(authToken)) {
            // configuration.Auth.Token = authToken; // Disabled to avoid CS0618 warning
            _logger.LogDebug("Auth token environment variable detected but not applied (pending ICredentialStore implementation)");
        }

        // Security configuration
        if (Environment.GetEnvironmentVariable("MCPM_TRUST_TIER_MINIMUM") is string trustTier && !string.IsNullOrWhiteSpace(trustTier)) {
            configuration.Security.TrustTierMinimum = trustTier;
            _logger.LogDebug("Minimum trust tier overridden by environment variable");
        }

        if (Environment.GetEnvironmentVariable("MCPM_ALLOW_INSECURE") is string allowInsecureStr &&
            bool.TryParse(allowInsecureStr, out var allowInsecure)) {
            configuration.Security.AllowInsecureConnections = allowInsecure;
            _logger.LogDebug("Allow insecure connections overridden by environment variable");
        }

        // UI configuration
        if (Environment.GetEnvironmentVariable("MCPM_NO_COLOR") is string noColorStr &&
            bool.TryParse(noColorStr, out var noColor)) {
            configuration.Ui.ColorOutput = !noColor;
            _logger.LogDebug("Color output overridden by environment variable");
        }

        if (Environment.GetEnvironmentVariable("MCPM_VERBOSE") is string verboseStr &&
            bool.TryParse(verboseStr, out var verbose)) {
            configuration.Ui.VerboseErrors = verbose;
            _logger.LogDebug("Verbose errors overridden by environment variable");
        }
    }
}