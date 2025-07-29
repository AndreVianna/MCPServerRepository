using System.Reflection;
using System.Text.Json;

using MCPHub.CommandLineApp.Configuration;

using Microsoft.Extensions.Logging;

namespace MCPHub.CommandLineApp.Services;

/// <summary>
/// Implementation of configuration service with enhanced features
/// </summary>
public class ConfigurationService : IConfigurationService {
    private readonly ILogger<ConfigurationService> _logger;
    private readonly IMcpmConfigurationManager _configurationManager;
    private readonly JsonSerializerOptions _jsonOptions;

    public ConfigurationService(
        ILogger<ConfigurationService> logger,
        IMcpmConfigurationManager configurationManager) {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configurationManager = configurationManager ?? throw new ArgumentNullException(nameof(configurationManager));

        _jsonOptions = new JsonSerializerOptions {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            // Enable reflection fallback for JSON serialization
            TypeInfoResolver = new System.Text.Json.Serialization.Metadata.DefaultJsonTypeInfoResolver()
        };
    }

    /// <inheritdoc />
    public async Task<object?> GetValueAsync(string key) {
        var config = await _configurationManager.LoadConfigurationAsync();
        return GetValueFromConfiguration(config, key);
    }

    /// <inheritdoc />
    public async Task SetValueAsync(string key, object? value, CancellationToken cancellationToken = default) {
        // Validate the value first
        var validation = ValidateValue(key, value);
        if (!validation.IsValid) {
            throw new ArgumentException(validation.ErrorMessage, nameof(value));
        }

        var config = await _configurationManager.LoadConfigurationAsync();
        SetValueInConfiguration(config, key, validation.NormalizedValue);
        await _configurationManager.SaveConfigurationAsync(config, cancellationToken);

        _logger.LogInformation("Configuration value set: {Key} = {Value}", key, value);
    }

    /// <inheritdoc />
    public async Task<Dictionary<string, object?>> GetAllValuesAsync() {
        var config = await _configurationManager.LoadConfigurationAsync();
        var result = new Dictionary<string, object?>();

        foreach (var key in ConfigurationSchema.Keys.Keys) {
            result[key] = GetValueFromConfiguration(config, key);
        }

        return result;
    }

    /// <inheritdoc />
    public async Task ResetToDefaultsAsync(CancellationToken cancellationToken = default) {
        var config = new McpmConfiguration();
        await _configurationManager.SaveConfigurationAsync(config, cancellationToken);

        _logger.LogInformation("Configuration reset to defaults");
    }

    /// <inheritdoc />
    public ValidationResult ValidateValue(string key, object? value) {
        var schemaKey = ConfigurationSchema.GetKey(key);
        if (schemaKey == null) {
            return ValidationResult.Error($"Unknown configuration key: {key}");
        }

        if (schemaKey.IsReadOnly) {
            return ValidationResult.Error($"Configuration key '{key}' is read-only");
        }

        if (value == null) {
            return ValidationResult.Success(null);
        }

        // Convert string values to appropriate types
        var normalizedValue = ConvertValue(value, schemaKey.ValueType);
        if (normalizedValue == null && value != null) {
            return ValidationResult.Error($"Invalid value type for '{key}'. Expected {schemaKey.ValueType.Name}");
        }

        // Validate using schema
        if (!schemaKey.IsValid(normalizedValue)) {
            var constraints = new List<string>();

            if (schemaKey.ValidValues != null)
                constraints.Add($"must be one of: {string.Join(", ", schemaKey.ValidValues)}");

            if (schemaKey.MinValue != null)
                constraints.Add($"minimum value: {schemaKey.MinValue}");

            if (schemaKey.MaxValue != null)
                constraints.Add($"maximum value: {schemaKey.MaxValue}");

            if (!string.IsNullOrEmpty(schemaKey.Pattern))
                constraints.Add($"must match pattern: {schemaKey.Pattern}");

            var constraintText = constraints.Count > 0 ? $" ({string.Join(", ", constraints)})" : "";
            return ValidationResult.Error($"Invalid value for '{key}'{constraintText}");
        }

        return ValidationResult.Success(normalizedValue);
    }

    /// <inheritdoc />
    public async Task ExportConfigurationAsync(string filePath, bool includeSecrets = false, CancellationToken cancellationToken = default) {
        var config = await _configurationManager.LoadConfigurationAsync();

        // Create export object with metadata
        var exportData = new {
            ExportedAt = DateTimeOffset.UtcNow,
            Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString(),
            IncludesSecrets = includeSecrets,
            Configuration = includeSecrets ? config : SanitizeConfiguration(config)
        };

        var json = JsonSerializer.Serialize(exportData, _jsonOptions);
        await File.WriteAllTextAsync(filePath, json, cancellationToken);

        _logger.LogInformation("Configuration exported to: {FilePath}", filePath);
    }

    /// <inheritdoc />
    public async Task ImportConfigurationAsync(string filePath, bool overwriteExisting = false, CancellationToken cancellationToken = default) {
        if (!File.Exists(filePath)) {
            throw new FileNotFoundException($"Configuration file not found: {filePath}");
        }

        var json = await File.ReadAllTextAsync(filePath, cancellationToken);

        // Try to parse as export format first
        McpmConfiguration? importedConfig = null;
        try {
            var exportData = JsonSerializer.Deserialize<JsonElement>(json, _jsonOptions);
            if (exportData.TryGetProperty("Configuration", out var configElement)) {
                importedConfig = JsonSerializer.Deserialize<McpmConfiguration>(configElement.GetRawText(), _jsonOptions);
            }
        }
        catch {
            // Fall back to direct configuration format
            importedConfig = JsonSerializer.Deserialize<McpmConfiguration>(json, _jsonOptions);
        }

        if (importedConfig == null) {
            throw new InvalidOperationException("Failed to parse configuration file");
        }

        var currentConfig = overwriteExisting ? new McpmConfiguration() : await _configurationManager.LoadConfigurationAsync();

        // Merge configurations
        MergeConfigurations(currentConfig, importedConfig, overwriteExisting);

        await _configurationManager.SaveConfigurationAsync(currentConfig, cancellationToken);

        _logger.LogInformation("Configuration imported from: {FilePath}", filePath);
    }

    /// <inheritdoc />
    public async Task<string> CreateBackupAsync(CancellationToken cancellationToken = default) {
        var backupDir = Path.Combine(Path.GetDirectoryName(_configurationManager.GetConfigurationPath())!, "backups");
        Directory.CreateDirectory(backupDir);

        var timestamp = DateTimeOffset.UtcNow.ToString("yyyyMMdd-HHmmss");
        var backupPath = Path.Combine(backupDir, $"config-backup-{timestamp}.json");

        await ExportConfigurationAsync(backupPath, includeSecrets: true, cancellationToken);

        _logger.LogInformation("Configuration backup created: {BackupPath}", backupPath);
        return backupPath;
    }

    /// <inheritdoc />
    public async Task RestoreFromBackupAsync(string backupPath, CancellationToken cancellationToken = default) {
        await ImportConfigurationAsync(backupPath, overwriteExisting: true, cancellationToken);
        _logger.LogInformation("Configuration restored from backup: {BackupPath}", backupPath);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<string>> ListBackupsAsync() {
        var backupDir = Path.Combine(Path.GetDirectoryName(_configurationManager.GetConfigurationPath())!, "backups");

        if (!Directory.Exists(backupDir)) {
            return Enumerable.Empty<string>();
        }

        var backupFiles = Directory.GetFiles(backupDir, "config-backup-*.json")
            .OrderByDescending(File.GetCreationTime);

        return await Task.FromResult(backupFiles);
    }

    /// <inheritdoc />
    public async Task<McpmConfiguration> GetConfigurationAsync() => await _configurationManager.LoadConfigurationAsync();

    private object? GetValueFromConfiguration(McpmConfiguration config, string key) {
        var parts = key.Split('.');
        if (parts.Length != 2) {
            return null;
        }

        var section = parts[0].ToLowerInvariant();
        var property = parts[1];

        return section switch {
            "registry" => GetPropertyValue(config.Registry, property),
            "security" => GetPropertyValue(config.Security, property),
            "ui" => GetPropertyValue(config.Ui, property),
            "paths" => GetPropertyValue(config.Paths, property),
            _ => null
        };
    }

    private void SetValueInConfiguration(McpmConfiguration config, string key, object? value) {
        var parts = key.Split('.');
        if (parts.Length != 2) {
            throw new ArgumentException($"Invalid configuration key format: {key}");
        }

        var section = parts[0].ToLowerInvariant();
        var property = parts[1];

        switch (section) {
            case "registry":
                SetPropertyValue(config.Registry, property, value);
                break;
            case "security":
                SetPropertyValue(config.Security, property, value);
                break;
            case "ui":
                SetPropertyValue(config.Ui, property, value);
                break;
            case "paths":
                SetPropertyValue(config.Paths, property, value);
                break;
            default:
                throw new ArgumentException($"Unknown configuration section: {section}");
        }
    }

    private object? GetPropertyValue(object obj, string propertyName) {
        var property = obj.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        return property?.GetValue(obj);
    }

    private void SetPropertyValue(object obj, string propertyName, object? value) {
        var property = obj.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        if (property != null && property.CanWrite) {
            property.SetValue(obj, value);
        }
        else {
            throw new ArgumentException($"Property '{propertyName}' not found or is read-only");
        }
    }

    private object? ConvertValue(object value, Type targetType) {
        if (value.GetType() == targetType)
            return value;

        if (value is string stringValue) {
            if (targetType == typeof(bool))
                return bool.TryParse(stringValue, out var boolResult) ? boolResult : null;

            if (targetType == typeof(int))
                return int.TryParse(stringValue, out var intResult) ? intResult : null;

            if (targetType == typeof(string))
                return stringValue;
        }

        try {
            return Convert.ChangeType(value, targetType);
        }
        catch {
            return null;
        }
    }

    private McpmConfiguration SanitizeConfiguration(McpmConfiguration config) {
        // Create a copy without sensitive information
        var sanitized = new McpmConfiguration {
            Registry = new RegistryConfiguration {
                Url = config.Registry.Url,
                Name = config.Registry.Name,
                Timeout = config.Registry.Timeout,
                Retries = config.Registry.Retries,
                ApiVersion = config.Registry.ApiVersion,
                RequiresAuthentication = config.Registry.RequiresAuthentication,
                SupportsPrivatePackages = config.Registry.SupportsPrivatePackages,
                SupportsPublishing = config.Registry.SupportsPublishing,
                VerifySSL = config.Registry.VerifySSL,
                // Exclude AuthEndpoint, Headers, and SupportedAuthMethods for security
            },
            Security = config.Security,
            Ui = config.Ui,
            Paths = config.Paths,
            Registries = config.Registries.ToDictionary(
                kvp => kvp.Key,
                kvp => new RegistryConfiguration {
                    Url = kvp.Value.Url,
                    Name = kvp.Value.Name,
                    Timeout = kvp.Value.Timeout,
                    Retries = kvp.Value.Retries,
                    ApiVersion = kvp.Value.ApiVersion,
                    RequiresAuthentication = kvp.Value.RequiresAuthentication,
                    SupportsPrivatePackages = kvp.Value.SupportsPrivatePackages,
                    SupportsPublishing = kvp.Value.SupportsPublishing,
                    VerifySSL = kvp.Value.VerifySSL
                })
        };

        return sanitized;
    }

    private void MergeConfigurations(McpmConfiguration target, McpmConfiguration source, bool overwriteExisting) {
        // Registry configuration
        if (overwriteExisting || string.IsNullOrEmpty(target.Registry.Url))
            target.Registry.Url = source.Registry.Url;
        if (overwriteExisting || string.IsNullOrEmpty(target.Registry.Name))
            target.Registry.Name = source.Registry.Name;
        if (overwriteExisting || target.Registry.Timeout == 30000)
            target.Registry.Timeout = source.Registry.Timeout;
        if (overwriteExisting || target.Registry.Retries == 3)
            target.Registry.Retries = source.Registry.Retries;

        // Security configuration
        if (overwriteExisting || target.Security.AutoVerify == true)
            target.Security.AutoVerify = source.Security.AutoVerify;
        if (overwriteExisting || target.Security.TrustTierMinimum == "Community")
            target.Security.TrustTierMinimum = source.Security.TrustTierMinimum;

        // UI configuration
        if (overwriteExisting || target.Ui.ColorOutput == true)
            target.Ui.ColorOutput = source.Ui.ColorOutput;
        if (overwriteExisting || target.Ui.DefaultFormat == "table")
            target.Ui.DefaultFormat = source.Ui.DefaultFormat;

        // Paths configuration
        if (overwriteExisting || !string.IsNullOrEmpty(source.Paths.Cache))
            target.Paths.Cache = source.Paths.Cache;
        if (overwriteExisting || !string.IsNullOrEmpty(source.Paths.Packages))
            target.Paths.Packages = source.Paths.Packages;

        // Registries
        foreach (var registry in source.Registries) {
            if (overwriteExisting || !target.Registries.ContainsKey(registry.Key)) {
                target.Registries[registry.Key] = registry.Value;
            }
        }
    }
}