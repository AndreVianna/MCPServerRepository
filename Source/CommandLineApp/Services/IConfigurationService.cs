namespace MCPHub.CommandLineApp.Services;

/// <summary>
/// Service for advanced configuration management
/// </summary>
public interface IConfigurationService {
    /// <summary>
    /// Gets a configuration value by key
    /// </summary>
    /// <param name="key">Configuration key</param>
    /// <returns>Configuration value or null if not set</returns>
    Task<object?> GetValueAsync(string key);

    /// <summary>
    /// Sets a configuration value
    /// </summary>
    /// <param name="key">Configuration key</param>
    /// <param name="value">Value to set</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SetValueAsync(string key, object? value, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all configuration values
    /// </summary>
    /// <returns>Dictionary of all configuration values</returns>
    Task<Dictionary<string, object?>> GetAllValuesAsync();

    /// <summary>
    /// Resets configuration to default values
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    Task ResetToDefaultsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates a configuration value
    /// </summary>
    /// <param name="key">Configuration key</param>
    /// <param name="value">Value to validate</param>
    /// <returns>Validation result</returns>
    ValidationResult ValidateValue(string key, object? value);

    /// <summary>
    /// Exports configuration to a file
    /// </summary>
    /// <param name="filePath">File path to export to</param>
    /// <param name="includeSecrets">Whether to include secret values</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task ExportConfigurationAsync(string filePath, bool includeSecrets = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Imports configuration from a file
    /// </summary>
    /// <param name="filePath">File path to import from</param>
    /// <param name="overwriteExisting">Whether to overwrite existing values</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task ImportConfigurationAsync(string filePath, bool overwriteExisting = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a backup of the current configuration
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Path to the backup file</returns>
    Task<string> CreateBackupAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Restores configuration from a backup
    /// </summary>
    /// <param name="backupPath">Path to the backup file</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task RestoreFromBackupAsync(string backupPath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists all available backup files
    /// </summary>
    /// <returns>List of backup file paths</returns>
    Task<IEnumerable<string>> ListBackupsAsync();

    /// <summary>
    /// Gets the current configuration
    /// </summary>
    /// <returns>Current configuration</returns>
    Task<McpmConfiguration> GetConfigurationAsync();
}

/// <summary>
/// Configuration validation result
/// </summary>
public class ValidationResult {
    public bool IsValid { get; init; }
    public string? ErrorMessage { get; init; }
    public object? NormalizedValue { get; init; }

    public static ValidationResult Success(object? normalizedValue = null)
        => new() { IsValid = true, NormalizedValue = normalizedValue };

    public static ValidationResult Error(string errorMessage)
        => new() { IsValid = false, ErrorMessage = errorMessage };
}