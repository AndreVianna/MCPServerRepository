using MCPHub.CommandLineApp.Configuration;
using MCPHub.CommandLineApp.Services;
using MCPHub.CommandLineApp.Utilities;

namespace MCPHub.CommandLineApp.Commands;

/// <summary>
/// Command for managing MCPM configuration
/// </summary>
public class ConfigCommand(
    ILogger<ConfigCommand> logger,
    McpmConfiguration configuration,
    IMcpHubApiClient apiClient,
    IOutputFormatter outputFormatter,
    IInteractionService interactionService,
    IProgressReporter progressReporter,
    IConfigurationService configurationService) : BaseCommand(logger, configuration, apiClient, outputFormatter, interactionService, progressReporter) {
    private readonly IConfigurationService _configurationService = configurationService ?? throw new ArgumentNullException(nameof(configurationService));

    /// <inheritdoc />
    public override Command CreateCommand() {
        var command = new Command("config", "Manage MCPM configuration settings");

        // Add subcommands
        command.AddCommand(CreateGetCommand());
        command.AddCommand(CreateSetCommand());
        command.AddCommand(CreateListCommand());
        command.AddCommand(CreateResetCommand());
        command.AddCommand(CreateEditCommand());
        command.AddCommand(CreateExportCommand());
        command.AddCommand(CreateImportCommand());
        command.AddCommand(CreateBackupCommand());
        command.AddCommand(CreateRestoreCommand());

        return command;
    }

    private Command CreateGetCommand() {
        var command = new Command("get", "Get the value of a configuration key");

        var keyArgument = new Argument<string>("key", "Configuration key (e.g., registry.url)");
        command.AddArgument(keyArgument);

        var formatOption = new Option<string?>(
            aliases: ["--format", "-f"],
            description: "Output format (table, json, value)") {
            ArgumentHelpName = "format",
        };
        command.AddOption(formatOption);

        command.SetHandler(ExecuteGetAsync, keyArgument, formatOption);
        return command;
    }

    private Command CreateSetCommand() {
        var command = new Command("set", "Set the value of a configuration key");

        var keyArgument = new Argument<string>("key", "Configuration key (e.g., registry.url)");
        var valueArgument = new Argument<string>("value", "Value to set");
        command.AddArgument(keyArgument);
        command.AddArgument(valueArgument);

        var globalOption = new Option<bool>(
            aliases: ["--global", "-g"],
            description: "Set global configuration (default is user-specific)");
        command.AddOption(globalOption);

        command.SetHandler(ExecuteSetAsync, keyArgument, valueArgument, globalOption);
        return command;
    }

    private Command CreateListCommand() {
        var command = new Command("list", "List all configuration values");

        var categoryOption = new Option<string?>(
            aliases: ["--category", "-c"],
            description: "Filter by category (Registry, Security, UI, Paths)");
        command.AddOption(categoryOption);

        var formatOption = new Option<string?>(
            aliases: ["--format", "-f"],
            description: "Output format (table, json, detailed)") {
            ArgumentHelpName = "format",
        };
        command.AddOption(formatOption);

        var includeDefaultsOption = new Option<bool>(
            aliases: ["--include-defaults", "-d"],
            description: "Include default values for unset keys");
        command.AddOption(includeDefaultsOption);

        var showSecretsOption = new Option<bool>(
            aliases: ["--show-secrets", "-s"],
            description: "Show secret values (use with caution)");
        command.AddOption(showSecretsOption);

        command.SetHandler(ExecuteListAsync, categoryOption, formatOption, includeDefaultsOption, showSecretsOption);
        return command;
    }

    private Command CreateResetCommand() {
        var command = new Command("reset", "Reset configuration to default values");

        var keyOption = new Option<string?>(
            aliases: ["--key", "-k"],
            description: "Reset specific key only (if not specified, resets all)");
        command.AddOption(keyOption);

        var confirmOption = new Option<bool>(
            aliases: ["--yes", "-y"],
            description: "Skip confirmation prompt");
        command.AddOption(confirmOption);

        command.SetHandler(ExecuteResetAsync, keyOption, confirmOption);
        return command;
    }

    private Command CreateEditCommand() {
        var command = new Command("edit", "Interactively edit configuration");

        var wizardOption = new Option<bool>(
            aliases: ["--wizard", "-w"],
            description: "Use guided configuration wizard");
        command.AddOption(wizardOption);

        var categoryOption = new Option<string?>(
            aliases: ["--category", "-c"],
            description: "Edit specific category only");
        command.AddOption(categoryOption);

        command.SetHandler(ExecuteEditAsync, wizardOption, categoryOption);
        return command;
    }

    private Command CreateExportCommand() {
        var command = new Command("export", "Export configuration to a file");

        var pathArgument = new Argument<string>("path", "Export file path");
        command.AddArgument(pathArgument);

        var includeSecretsOption = new Option<bool>(
            aliases: ["--include-secrets", "-s"],
            description: "Include secret values in export");
        command.AddOption(includeSecretsOption);

        command.SetHandler(ExecuteExportAsync, pathArgument, includeSecretsOption);
        return command;
    }

    private Command CreateImportCommand() {
        var command = new Command("import", "Import configuration from a file");

        var pathArgument = new Argument<string>("path", "Import file path");
        command.AddArgument(pathArgument);

        var overwriteOption = new Option<bool>(
            aliases: ["--overwrite", "-o"],
            description: "Overwrite existing configuration values");
        command.AddOption(overwriteOption);

        command.SetHandler(ExecuteImportAsync, pathArgument, overwriteOption);
        return command;
    }

    private Command CreateBackupCommand() {
        var command = new Command("backup", "Create a backup of current configuration");

        command.SetHandler(ExecuteBackupAsync);
        return command;
    }

    private Command CreateRestoreCommand() {
        var command = new Command("restore", "Restore configuration from a backup");

        var pathArgument = new Argument<string?>("path", "Backup file path (if not specified, shows available backups)") {
            Arity = ArgumentArity.ZeroOrOne,
        };
        command.AddArgument(pathArgument);

        command.SetHandler(ExecuteRestoreAsync, pathArgument);
        return command;
    }

    private async Task<int> ExecuteGetAsync(string key, string? format) {
        try {
            Logger.LogInformation("Getting configuration value for key: {Key}", key);

            var schemaKey = ConfigurationSchema.GetKey(key);
            if (schemaKey == null) {
                OutputFormatter.WriteError($"Unknown configuration key: {key}");
                SuggestSimilarKeys(key);
                return 400;
            }

            var value = await _configurationService.GetValueAsync(key);
            var outputFormat = format?.ToLowerInvariant();

            if (outputFormat == "value") {
                // Output just the value for scripting
                AnsiConsole.WriteLine(value?.ToString() ?? "");
                return 0;
            }

            outputFormat = ValidateOutputFormat(format);

            if (outputFormat == "json") {
                var result = new { key, value, description = schemaKey.Description };
                OutputFormatter.WriteJson(result);
            }
            else {
                // Table format
                var table = new Table()
                    .AddColumn("Key")
                    .AddColumn("Value")
                    .AddColumn("Description");

                var displayValue = FormatValueForDisplay(value, schemaKey);
                table.AddRow(key, displayValue, schemaKey.Description);

                AnsiConsole.Write(table);

                if (schemaKey.DefaultValue != null) {
                    OutputFormatter.WriteLine();
                    OutputFormatter.WriteInfo($"Default value: {FormatValueForDisplay(schemaKey.DefaultValue, schemaKey)}");
                }
            }

            return 0;
        }
        catch (Exception ex) {
            return HandleError(ex, "config get");
        }
    }

    private async Task<int> ExecuteSetAsync(string key, string value, bool global) {
        try {
            Logger.LogInformation("Setting configuration value: {Key} = {Value}, Global: {Global}", key, value, global);

            var schemaKey = ConfigurationSchema.GetKey(key);
            if (schemaKey == null) {
                OutputFormatter.WriteError($"Unknown configuration key: {key}");
                SuggestSimilarKeys(key);
                return 400;
            }

            // Validate the value
            var validation = _configurationService.ValidateValue(key, value);
            if (!validation.IsValid) {
                OutputFormatter.WriteError($"Invalid value: {validation.ErrorMessage}");
                return 400;
            }

            // Show current value and confirm change
            var currentValue = await _configurationService.GetValueAsync(key);
            if (currentValue != null) {
                OutputFormatter.WriteInfo($"Current value: {FormatValueForDisplay(currentValue, schemaKey)}");
            }

            OutputFormatter.WriteInfo($"Setting '{key}' to: {FormatValueForDisplay(validation.NormalizedValue, schemaKey)}");

            // Set the value
            await _configurationService.SetValueAsync(key, validation.NormalizedValue);

            OutputFormatter.WriteSuccess($"Configuration updated successfully");

            // Show any additional information
            if (key.StartsWith("paths.", StringComparison.OrdinalIgnoreCase)) {
                OutputFormatter.WriteInfo("Note: Directory will be created if it doesn't exist");
            }
            else if (key.StartsWith("registry.", StringComparison.OrdinalIgnoreCase)) {
                OutputFormatter.WriteInfo("Note: Changes to registry settings affect future API requests");
            }

            return 0;
        }
        catch (Exception ex) {
            return HandleError(ex, "config set");
        }
    }

    private async Task<int> ExecuteListAsync(string? category, string? format, bool includeDefaults, bool showSecrets) {
        try {
            Logger.LogInformation("Listing configuration values: Category={Category}, Format={Format}", category, format);

            var allValues = await _configurationService.GetAllValuesAsync();
            var outputFormat = ValidateOutputFormat(format);

            if (outputFormat == "json") {
                var result = new { configuration = allValues };
                OutputFormatter.WriteJson(result);
                return 0;
            }

            // Group keys by category
            var keyGroups = ConfigurationSchema.GetKeysByCategory();

            if (!string.IsNullOrEmpty(category)) {
                keyGroups = keyGroups.Where(g =>
                    string.Equals(g.Key, category, StringComparison.OrdinalIgnoreCase));
            }

            foreach (var group in keyGroups) {
                var categoryName = group.Key ?? "Other";

                OutputFormatter.WriteLine();
                AnsiConsole.Write(new Rule($"[bold cyan]{categoryName} Configuration[/]").LeftJustified());
                OutputFormatter.WriteLine();

                var table = new Table()
                    .AddColumn("Key")
                    .AddColumn("Value")
                    .AddColumn("Description");

                foreach (var schemaKey in group.OrderBy(k => k.Key)) {
                    var value = allValues.GetValueOrDefault(schemaKey.Key);

                    if (!includeDefaults && value == null)
                        continue;

                    var displayValue = value ?? schemaKey.DefaultValue;
                    var formattedValue = FormatValueForDisplay(displayValue, schemaKey, showSecrets);

                    var keyDisplay = value == null ? $"[dim]{schemaKey.Key}[/]" : schemaKey.Key;
                    var valueDisplay = value == null ? $"[dim]{formattedValue} (default)[/]" : formattedValue;

                    table.AddRow(keyDisplay, valueDisplay, schemaKey.Description);
                }

                AnsiConsole.Write(table);
            }

            if (!includeDefaults) {
                OutputFormatter.WriteLine();
                OutputFormatter.WriteInfo("Use --include-defaults to show all available configuration keys");
            }

            return 0;
        }
        catch (Exception ex) {
            return HandleError(ex, "config list");
        }
    }

    private async Task<int> ExecuteResetAsync(string? key, bool confirm) {
        try {
            if (!string.IsNullOrEmpty(key)) {
                Logger.LogInformation("Resetting configuration key: {Key}", key);

                var schemaKey = ConfigurationSchema.GetKey(key);
                if (schemaKey == null) {
                    OutputFormatter.WriteError($"Unknown configuration key: {key}");
                    return 400;
                }

                if (!confirm) {
                    if (!AnsiConsole.Confirm($"Reset '{key}' to default value?")) {
                        OutputFormatter.WriteInfo("Operation cancelled");
                        return 0;
                    }
                }

                await _configurationService.SetValueAsync(key, schemaKey.DefaultValue);
                OutputFormatter.WriteSuccess($"Configuration key '{key}' reset to default");
            }
            else {
                Logger.LogInformation("Resetting all configuration to defaults");

                if (!confirm) {
                    OutputFormatter.WriteWarning("This will reset ALL configuration settings to their default values.");
                    if (!AnsiConsole.Confirm("Are you sure you want to continue?")) {
                        OutputFormatter.WriteInfo("Operation cancelled");
                        return 0;
                    }
                }

                // Create backup before reset
                var backupPath = await _configurationService.CreateBackupAsync();
                OutputFormatter.WriteInfo($"Backup created: {backupPath}");

                await _configurationService.ResetToDefaultsAsync();
                OutputFormatter.WriteSuccess("Configuration reset to defaults");
                OutputFormatter.WriteInfo("Use 'mcpm config restore' to restore from backup if needed");
            }

            return 0;
        }
        catch (Exception ex) {
            return HandleError(ex, "config reset");
        }
    }

    private async Task<int> ExecuteEditAsync(bool wizard, string? category) {
        try {
            Logger.LogInformation("Starting interactive configuration edit: Wizard={Wizard}, Category={Category}", wizard, category);

            return wizard ? await RunConfigurationWizardAsync() : await RunInteractiveEditorAsync(category);
        }
        catch (Exception ex) {
            return HandleError(ex, "config edit");
        }
    }

    private async Task<int> ExecuteExportAsync(string path, bool includeSecrets) {
        try {
            Logger.LogInformation("Exporting configuration to: {Path}, IncludeSecrets: {IncludeSecrets}", path, includeSecrets);

            if (includeSecrets) {
                OutputFormatter.WriteWarning("Exporting with secrets - ensure the export file is stored securely");
            }

            await _configurationService.ExportConfigurationAsync(path, includeSecrets);
            OutputFormatter.WriteSuccess($"Configuration exported to: {path}");

            return 0;
        }
        catch (Exception ex) {
            return HandleError(ex, "config export");
        }
    }

    private async Task<int> ExecuteImportAsync(string path, bool overwrite) {
        try {
            Logger.LogInformation("Importing configuration from: {Path}, Overwrite: {Overwrite}", path, overwrite);

            if (overwrite) {
                // Create backup before import
                var backupPath = await _configurationService.CreateBackupAsync();
                OutputFormatter.WriteInfo($"Backup created: {backupPath}");
            }

            await _configurationService.ImportConfigurationAsync(path, overwrite);
            OutputFormatter.WriteSuccess($"Configuration imported from: {path}");

            return 0;
        }
        catch (Exception ex) {
            return HandleError(ex, "config import");
        }
    }

    private async Task<int> ExecuteBackupAsync() {
        try {
            Logger.LogInformation("Creating configuration backup");

            var backupPath = await _configurationService.CreateBackupAsync();
            OutputFormatter.WriteSuccess($"Configuration backup created: {backupPath}");

            return 0;
        }
        catch (Exception ex) {
            return HandleError(ex, "config backup");
        }
    }

    private async Task<int> ExecuteRestoreAsync(string? path) {
        try {
            if (string.IsNullOrEmpty(path)) {
                // Show available backups
                var backups = await _configurationService.ListBackupsAsync();

                if (!backups.Any()) {
                    OutputFormatter.WriteInfo("No backups found");
                    OutputFormatter.WriteInfo("Use 'mcpm config backup' to create a backup");
                    return 0;
                }

                OutputFormatter.WriteInfo("Available backups:");
                var table = new Table()
                    .AddColumn("Backup File")
                    .AddColumn("Created")
                    .AddColumn("Size");

                foreach (var backup in backups) {
                    var fileInfo = new FileInfo(backup);
                    table.AddRow(
                        Path.GetFileName(backup),
                        fileInfo.CreationTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        FormatFileSize(fileInfo.Length));
                }

                AnsiConsole.Write(table);
                OutputFormatter.WriteLine();
                OutputFormatter.WriteInfo("Use 'mcpm config restore <backup-file>' to restore from a specific backup");

                return 0;
            }

            Logger.LogInformation("Restoring configuration from backup: {Path}", path);

            // If path is just a filename, look in the backups directory
            if (!Path.IsPathRooted(path)) {
                var backupDir = Path.Combine(Path.GetDirectoryName(Configuration.Paths.Config)!, "backups");
                path = Path.Combine(backupDir, path);
            }

            if (!File.Exists(path)) {
                OutputFormatter.WriteError($"Backup file not found: {path}");
                return 404;
            }

            OutputFormatter.WriteWarning("This will replace your current configuration with the backup.");
            if (!AnsiConsole.Confirm("Are you sure you want to continue?")) {
                OutputFormatter.WriteInfo("Operation cancelled");
                return 0;
            }

            await _configurationService.RestoreFromBackupAsync(path);
            OutputFormatter.WriteSuccess($"Configuration restored from: {path}");

            return 0;
        }
        catch (Exception ex) {
            return HandleError(ex, "config restore");
        }
    }

    private async Task<int> RunConfigurationWizardAsync() {
        OutputFormatter.WriteLine();
        AnsiConsole.Write(new Rule("[bold green]MCPM Configuration Wizard[/]").Centered());
        OutputFormatter.WriteLine();

        OutputFormatter.WriteInfo("This wizard will help you configure MCPM for your environment.");
        OutputFormatter.WriteLine();

        var config = await _configurationService.GetConfigurationAsync();

        // Registry configuration
        if (AnsiConsole.Confirm("Configure registry settings?", defaultValue: true)) {
            await ConfigureRegistryWizardAsync();
        }

        // Security configuration  
        if (AnsiConsole.Confirm("Configure security settings?", defaultValue: true)) {
            await ConfigureSecurityWizardAsync();
        }

        // UI configuration
        if (AnsiConsole.Confirm("Configure UI preferences?", defaultValue: true)) {
            await ConfigureUiWizardAsync();
        }

        // Path configuration
        if (AnsiConsole.Confirm("Configure directory paths?", defaultValue: false)) {
            await ConfigurePathsWizardAsync();
        }

        OutputFormatter.WriteLine();
        OutputFormatter.WriteSuccess("Configuration wizard completed!");
        OutputFormatter.WriteInfo("Use 'mcpm config list' to view your settings");

        return 0;
    }

    private async Task ConfigureRegistryWizardAsync() {
        OutputFormatter.WriteLine();
        AnsiConsole.Write(new Rule("[cyan]Registry Settings[/]").LeftJustified());

        var currentUrl = await _configurationService.GetValueAsync("registry.url") as string;
        var registryUrl = AnsiConsole.Ask("Registry URL:", currentUrl ?? "https://api.mcphub.dev");

        if (registryUrl != currentUrl) {
            await _configurationService.SetValueAsync("registry.url", registryUrl);
        }

        var timeout = AnsiConsole.Ask("Request timeout (milliseconds):", 30000);
        await _configurationService.SetValueAsync("registry.timeout", timeout);

        var retries = AnsiConsole.Ask("Retry attempts:", 3);
        await _configurationService.SetValueAsync("registry.retries", retries);
    }

    private async Task ConfigureSecurityWizardAsync() {
        OutputFormatter.WriteLine();
        AnsiConsole.Write(new Rule("[yellow]Security Settings[/]").LeftJustified());

        var autoVerify = AnsiConsole.Confirm("Auto-verify packages during installation?", defaultValue: true);
        await _configurationService.SetValueAsync("security.autoVerify", autoVerify);

        var trustTier = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Minimum trust tier for installation:")
                .AddChoices("Unverified", "Community", "Professional", "Enterprise")
                .UseConverter(tier => tier switch {
                    "Unverified" => "Unverified (No restrictions)",
                    "Community" => "Community (Basic verification)",
                    "Professional" => "Professional (Enhanced security)",
                    "Enterprise" => "Enterprise (Maximum security)",
                    _ => tier,
                }));

        await _configurationService.SetValueAsync("security.trustTierMinimum", trustTier);
    }

    private async Task ConfigureUiWizardAsync() {
        OutputFormatter.WriteLine();
        AnsiConsole.Write(new Rule("[magenta]UI Preferences[/]").LeftJustified());

        var colorOutput = AnsiConsole.Confirm("Use colored output?", defaultValue: true);
        await _configurationService.SetValueAsync("ui.colorOutput", colorOutput);

        var progressBars = AnsiConsole.Confirm("Show progress bars?", defaultValue: true);
        await _configurationService.SetValueAsync("ui.progressBars", progressBars);

        var defaultFormat = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Default output format:")
                .AddChoices("table", "json", "detailed"));

        await _configurationService.SetValueAsync("ui.defaultFormat", defaultFormat);
    }

    private async Task ConfigurePathsWizardAsync() {
        OutputFormatter.WriteLine();
        AnsiConsole.Write(new Rule("[blue]Directory Paths[/]").LeftJustified());

        var currentCache = await _configurationService.GetValueAsync("paths.cache") as string;
        var cachePath = AnsiConsole.Ask("Cache directory:", currentCache ?? "");
        await _configurationService.SetValueAsync("paths.cache", cachePath);

        var currentPackages = await _configurationService.GetValueAsync("paths.packages") as string;
        var packagesPath = AnsiConsole.Ask("Packages directory:", currentPackages ?? "");
        await _configurationService.SetValueAsync("paths.packages", packagesPath);
    }

    private async Task<int> RunInteractiveEditorAsync(string? category) {
        // This would implement a more advanced interactive editor
        // For now, redirect to the wizard
        OutputFormatter.WriteInfo("Interactive editor not yet implemented. Using wizard mode...");
        return await RunConfigurationWizardAsync();
    }

    private void SuggestSimilarKeys(string key) {
        var similarKeys = ConfigurationSchema.SearchKeys(key).Take(5);
        if (similarKeys.Any()) {
            OutputFormatter.WriteLine();
            OutputFormatter.WriteInfo("Did you mean one of these?");
            foreach (var similarKey in similarKeys) {
                OutputFormatter.WriteInfo($"  {similarKey.Key} - {similarKey.Description}");
            }
        }
    }

    private static string FormatValueForDisplay(object? value, ConfigurationKey schemaKey, bool showSecrets = false) => value == null
            ? "[dim]null[/]"
            : schemaKey.IsSecret && !showSecrets
            ? "[dim]***[/]"
            : value switch {
                bool b => b ? "[green]true[/]" : "[red]false[/]",
                string s when string.IsNullOrEmpty(s) => "[dim]empty[/]",
                string s => s,
                _ => value.ToString() ?? "[dim]null[/]",
            };

    private static string FormatFileSize(long bytes) {
        string[] suffixes = { "B", "KB", "MB", "GB" };
        var counter = 0;
        decimal number = bytes;

        while (Math.Round(number / 1024) >= 1) {
            number /= 1024;
            counter++;
        }

        return $"{number:n1} {suffixes[counter]}";
    }
}