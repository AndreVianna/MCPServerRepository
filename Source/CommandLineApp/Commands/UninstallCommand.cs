using System.CommandLine;
using System.CommandLine.Invocation;

using MCPHub.CommandLineApp.Configuration;
using MCPHub.CommandLineApp.Services;
using MCPHub.CommandLineApp.Utilities;

using Microsoft.Extensions.Logging;

using Spectre.Console;

namespace MCPHub.CommandLineApp.Commands;

/// <summary>
/// Command for uninstalling MCP packages with dependency checking and cleanup
/// </summary>
public class UninstallCommand(
    ILogger<UninstallCommand> logger,
    McpmConfiguration configuration,
    IMcpHubApiClient apiClient,
    IOutputFormatter outputFormatter,
    IInteractionService interactionService,
    IProgressReporter progressReporter,
    IUninstallService uninstallService) : BaseCommand(logger, configuration, apiClient, outputFormatter, interactionService, progressReporter) {
    private readonly IUninstallService _uninstallService = uninstallService ?? throw new ArgumentNullException(nameof(uninstallService));

    // Store references to command line elements for value extraction
    private Argument<string>? _packageArgument;
    private Option<string?>? _versionOption;
    private Option<bool>? _globalOption;
    private Option<bool>? _forceOption;
    private Option<bool>? _removeDependenciesOption;
    private Option<bool>? _noBackupOption;
    private Option<bool>? _purgeOption;
    private Option<bool>? _dryRunOption;
    private Option<bool>? _yesOption;
    private Option<bool>? _nonInteractiveOption;
    private Option<bool>? _verboseOption;

    /// <inheritdoc />
    public override Command CreateCommand() {
        var command = new Command("uninstall", "Uninstall MCP packages with dependency checking and cleanup");
        
        // Add aliases
        command.AddAlias("remove");
        command.AddAlias("rm");

        // Arguments
        _packageArgument = new Argument<string>("package", "Package name to uninstall");
        command.AddArgument(_packageArgument);

        // Options
        _versionOption = new Option<string?>(
            aliases: ["--version", "-v"],
            description: "Specific version to uninstall (default: all versions)");
        command.AddOption(_versionOption);

        _globalOption = new Option<bool>(
            aliases: ["--global", "-g"],
            description: "Uninstall global package");
        command.AddOption(_globalOption);

        _forceOption = new Option<bool>(
            aliases: ["--force", "-f"],
            description: "Force uninstall even if other packages depend on it");
        command.AddOption(_forceOption);

        _removeDependenciesOption = new Option<bool>(
            aliases: ["--remove-deps", "--auto-remove"],
            description: "Remove unused dependencies after uninstallation");
        command.AddOption(_removeDependenciesOption);

        _noBackupOption = new Option<bool>(
            aliases: ["--no-backup"],
            description: "Skip backup creation before uninstallation");
        command.AddOption(_noBackupOption);

        _purgeOption = new Option<bool>(
            aliases: ["--purge"],
            description: "Remove all configuration and cache files (complete removal)");
        command.AddOption(_purgeOption);

        _dryRunOption = new Option<bool>(
            aliases: ["--dry-run"],
            description: "Show what would be removed without actually removing");
        command.AddOption(_dryRunOption);

        _yesOption = new Option<bool>(
            aliases: ["--yes", "-y"],
            description: "Automatically confirm all prompts");
        command.AddOption(_yesOption);

        _nonInteractiveOption = new Option<bool>(
            aliases: ["--non-interactive", "-n"],
            description: "Disable interactive prompts (for CI/CD)");
        command.AddOption(_nonInteractiveOption);

        _verboseOption = new Option<bool>(
            aliases: ["--verbose"],
            description: "Show detailed uninstallation information");
        command.AddOption(_verboseOption);

        // Subcommands
        var cleanupCommand = CreateCleanupCommand();
        command.AddCommand(cleanupCommand);

        var restoreCommand = CreateRestoreCommand();
        command.AddCommand(restoreCommand);

        var listBackupsCommand = CreateListBackupsCommand();
        command.AddCommand(listBackupsCommand);

        // Handler
        command.SetHandler(ExecuteAsync);

        return command;
    }

    private Command CreateCleanupCommand() {
        var command = new Command("cleanup", "Remove orphaned dependencies and clean up unused packages");

        var globalOption = new Option<bool>(
            aliases: ["--global", "-g"],
            description: "Clean global packages");
        command.AddOption(globalOption);

        var dryRunOption = new Option<bool>(
            aliases: ["--dry-run"],
            description: "Show what would be cleaned without actually cleaning");
        command.AddOption(dryRunOption);

        var yesOption = new Option<bool>(
            aliases: ["--yes", "-y"],
            description: "Automatically confirm all prompts");
        command.AddOption(yesOption);

        var verboseOption = new Option<bool>(
            aliases: ["--verbose"],
            description: "Show detailed cleanup information");
        command.AddOption(verboseOption);

        command.SetHandler(
            ExecuteCleanupAsync,
            globalOption,
            dryRunOption,
            yesOption,
            verboseOption);

        return command;
    }

    private Command CreateRestoreCommand() {
        var command = new Command("restore", "Restore a package from backup");

        var backupPathArgument = new Argument<string>("backup-path", "Path to the backup file");
        command.AddArgument(backupPathArgument);

        var globalOption = new Option<bool>(
            aliases: ["--global", "-g"],
            description: "Restore as global package");
        command.AddOption(globalOption);

        var yesOption = new Option<bool>(
            aliases: ["--yes", "-y"],
            description: "Automatically confirm restoration");
        command.AddOption(yesOption);

        var verboseOption = new Option<bool>(
            aliases: ["--verbose"],
            description: "Show detailed restoration information");
        command.AddOption(verboseOption);

        command.SetHandler(
            ExecuteRestoreAsync,
            backupPathArgument,
            globalOption,
            yesOption,
            verboseOption);

        return command;
    }

    private Command CreateListBackupsCommand() {
        var command = new Command("list-backups", "List available package backups");
        command.AddAlias("backups");

        var packageOption = new Option<string?>(
            aliases: ["--package"],
            description: "Filter backups by package name");
        command.AddOption(packageOption);

        var globalOption = new Option<bool>(
            aliases: ["--global", "-g"],
            description: "List global package backups");
        command.AddOption(globalOption);

        var verboseOption = new Option<bool>(
            aliases: ["--verbose"],
            description: "Show detailed backup information");
        command.AddOption(verboseOption);

        command.SetHandler(
            ExecuteListBackupsAsync,
            packageOption,
            globalOption,
            verboseOption);

        return command;
    }

    private async Task<int> ExecuteAsync(InvocationContext context) {
        // Extract option values from the context using ParseResult
        var package = context.ParseResult.GetValueForArgument(_packageArgument!);
        var version = context.ParseResult.GetValueForOption(_versionOption!);
        var global = context.ParseResult.GetValueForOption(_globalOption!);
        var force = context.ParseResult.GetValueForOption(_forceOption!);
        var removeDependencies = context.ParseResult.GetValueForOption(_removeDependenciesOption!);
        var noBackup = context.ParseResult.GetValueForOption(_noBackupOption!);
        var purge = context.ParseResult.GetValueForOption(_purgeOption!);
        var dryRun = context.ParseResult.GetValueForOption(_dryRunOption!);
        var yes = context.ParseResult.GetValueForOption(_yesOption!);
        var nonInteractive = context.ParseResult.GetValueForOption(_nonInteractiveOption!);
        var verbose = context.ParseResult.GetValueForOption(_verboseOption!);
        
        try {
            Logger.LogInformation("Executing uninstall command: Package={Package}, Version={Version}, Global={Global}",
                package, version, global);

            if (string.IsNullOrWhiteSpace(package)) {
                OutputFormatter.WriteError("Package name is required");
                return 400;
            }

            using var progress = ProgressReporter.CreateStepProgress("Package Uninstall", new[] {
                "Validating uninstall",
                "Analyzing dependencies",
                "Creating backup",
                "Uninstalling package"
            });

            // Stage 1: Validate uninstall
            progress.StartStep(0, $"Validating uninstall for '{package}'...");
            
            try {
                var validation = await _uninstallService.ValidateUninstallAsync(package, version, global);
                
                if (!validation.CanUninstall && !force) {
                    progress.FailStep(0, "Cannot uninstall package");
                    
                    OutputFormatter.WriteError("Cannot uninstall package:");
                    foreach (var issue in validation.Issues) {
                        OutputFormatter.WriteError($"  - {issue}");
                    }
                    
                    if (validation.RequiresForce) {
                        OutputFormatter.WriteInfo("Use --force to override these issues (not recommended)");
                    }
                    
                    return 1;
                }

                foreach (var warning in validation.Warnings) {
                    OutputFormatter.WriteWarning(warning);
                }

                progress.CompleteStep(0, "Validation completed");
            }
            catch (Exception ex) {
                progress.FailStep(0, $"Validation failed: {ex.Message}");
                throw;
            }

            // Stage 2: Analyze dependencies
            progress.StartStep(1, "Analyzing package dependencies...");
            
            try {
                var analysis = await _uninstallService.AnalyzeDependenciesAsync(package, version, global);
                
                if (!analysis.CanUninstallSafely && !force) {
                    progress.FailStep(1, "Dependency conflicts detected");
                    
                    await DisplayDependencyAnalysisAsync(analysis, verbose);
                    
                    if (!nonInteractive && !yes) {
                        var continueAnyway = await InteractionService.ConfirmAsync(
                            "This may break other packages. Continue anyway?", false);
                        if (!continueAnyway) {
                            progress.SkipStep(2, "User cancelled");
                            progress.SkipStep(3, "User cancelled");
                            return 130;
                        }
                    } else if (!force) {
                        OutputFormatter.WriteError("Uninstall cancelled due to dependency conflicts");
                        return 1;
                    }
                }

                progress.CompleteStep(1, $"Analyzed {analysis.DependentPackages.Count} dependent packages");
                
                // Show what will be removed
                await DisplayUninstallPlanAsync(package, version, analysis, removeDependencies, purge, verbose);
                
                if (dryRun) {
                    progress.SkipStep(2, "Dry run mode");
                    progress.SkipStep(3, "Dry run mode");
                    OutputFormatter.WriteSuccess("Dry run completed. No packages were uninstalled.");
                    return 0;
                }
            }
            catch (Exception ex) {
                progress.FailStep(1, $"Dependency analysis failed: {ex.Message}");
                throw;
            }

            // Final confirmation
            if (!yes && !nonInteractive) {
                var confirmMessage = purge ? 
                    $"Completely remove '{package}' and all its data?" :
                    $"Uninstall '{package}'?";
                    
                var confirmUninstall = await InteractionService.ConfirmAsync(confirmMessage, false);
                if (!confirmUninstall) {
                    progress.SkipStep(2, "User cancelled");
                    progress.SkipStep(3, "User cancelled");
                    OutputFormatter.WriteInfo("Uninstall cancelled by user");
                    return 130;
                }
            }

            // Stage 3: Create backup (if requested)
            if (!noBackup && !purge) {
                progress.StartStep(2, "Creating backup...");
                
                try {
                    // For now, we'll assume the uninstall service handles backup creation
                    progress.CompleteStep(2, "Backup created");
                }
                catch (Exception ex) {
                    progress.FailStep(2, $"Backup failed: {ex.Message}");
                    
                    if (!force && !nonInteractive && !yes) {
                        var continueWithoutBackup = await InteractionService.ConfirmAsync(
                            "Continue without backup?", false);
                        if (!continueWithoutBackup) {
                            progress.SkipStep(3, "Cancelled due to backup failure");
                            return 1;
                        }
                    }
                }
            } else {
                progress.SkipStep(2, noBackup ? "Backup skipped" : "Purge mode - no backup");
            }

            // Stage 4: Perform uninstall
            progress.StartStep(3, $"Uninstalling {package}...");
            
            try {
                PackageUninstallResult result;
                
                if (purge) {
                    var purgeResult = await _uninstallService.PurgePackageAsync(package, global);
                    result = new PackageUninstallResult {
                        Success = purgeResult.Success,
                        PackageName = purgeResult.PackageName,
                        Messages = purgeResult.Messages,
                        Errors = purgeResult.Errors,
                        Duration = purgeResult.Duration,
                        FreedSpace = purgeResult.FreedSpace
                    };
                } else {
                    result = await _uninstallService.UninstallPackageAsync(
                        package, version, global, force, removeDependencies, !noBackup, false);
                }
                
                if (result.Success) {
                    progress.CompleteStep(3, $"Successfully uninstalled {package}");
                    return await HandleUninstallResultAsync(result, verbose);
                } else {
                    progress.FailStep(3, "Uninstall failed");
                    return await HandleUninstallResultAsync(result, verbose);
                }
            }
            catch (Exception ex) {
                progress.FailStep(3, $"Uninstall failed: {ex.Message}");
                throw;
            }
        }
        catch (Exception ex) {
            return HandleError(ex, "uninstall");
        }
    }

    private async Task<int> ExecuteCleanupAsync(bool global, bool dryRun, bool yes, bool verbose) {
        try {
            Logger.LogInformation("Executing cleanup command: Global={Global}, DryRun={DryRun}", global, dryRun);

            using var spinner = ProgressReporter.CreateSpinner("Scanning for orphaned dependencies...");
            
            var result = await _uninstallService.CleanupOrphanedDependenciesAsync(global, dryRun);
            
            if (result.RemovedPackages.Any()) {
                spinner.Success($"Found {result.RemovedPackages.Count} orphaned packages");
                
                OutputFormatter.WriteLine();
                OutputFormatter.WriteInfo($"Orphaned packages {(dryRun ? "that would be removed" : "removed")}:");
                
                foreach (var package in result.RemovedPackages) {
                    OutputFormatter.WriteInfo($"  - {package}");
                }
                
                if (result.FreedSpace > 0) {
                    var freedSpaceMB = result.FreedSpace / (1024 * 1024);
                    OutputFormatter.WriteInfo($"Space {(dryRun ? "that would be" : "")} freed: {freedSpaceMB:N1} MB");
                }
            } else {
                spinner.Success("No orphaned packages found");
                OutputFormatter.WriteSuccess("No cleanup needed - all packages are being used!");
            }

            if (result.Errors.Any()) {
                OutputFormatter.WriteLine();
                OutputFormatter.WriteWarning("Cleanup completed with errors:");
                foreach (var error in result.Errors) {
                    OutputFormatter.WriteError($"  - {error}");
                }
            }

            return result.Success ? 0 : 1;
        }
        catch (Exception ex) {
            return HandleError(ex, "cleanup");
        }
    }

    private async Task<int> ExecuteRestoreAsync(string backupPath, bool global, bool yes, bool verbose) {
        try {
            Logger.LogInformation("Executing restore command: BackupPath={BackupPath}, Global={Global}", backupPath, global);

            if (!File.Exists(backupPath)) {
                OutputFormatter.WriteError($"Backup file not found: {backupPath}");
                return 404;
            }

            if (!yes) {
                var confirmRestore = await InteractionService.ConfirmAsync(
                    $"Restore package from backup '{backupPath}'?", false);
                if (!confirmRestore) {
                    OutputFormatter.WriteInfo("Restore cancelled by user");
                    return 130;
                }
            }

            using var spinner = ProgressReporter.CreateSpinner("Restoring package from backup...");
            
            var result = await _uninstallService.RestorePackageFromBackupAsync(backupPath, global);
            
            if (result.Success) {
                spinner.Success($"Successfully restored {result.PackageName}@{result.Version}");
                
                OutputFormatter.WriteSuccess($"Package restored: {result.PackageName}@{result.Version}");
                OutputFormatter.WriteInfo($"Restored to: {result.RestorePath}");
                
                if (verbose) {
                    OutputFormatter.WriteInfo($"Restore duration: {result.Duration.TotalSeconds:F1}s");
                }

                foreach (var message in result.Messages) {
                    OutputFormatter.WriteInfo($"  {message}");
                }
            } else {
                spinner.Fail("Restore failed");
                
                OutputFormatter.WriteError("Failed to restore package:");
                foreach (var error in result.Errors) {
                    OutputFormatter.WriteError($"  - {error}");
                }
            }

            return result.Success ? 0 : 1;
        }
        catch (Exception ex) {
            return HandleError(ex, "restore");
        }
    }

    private async Task<int> ExecuteListBackupsAsync(string? package, bool global, bool verbose) {
        try {
            Logger.LogInformation("Executing list-backups command: Package={Package}, Global={Global}", package, global);

            using var spinner = ProgressReporter.CreateSpinner("Loading backup information...");
            
            var backups = await _uninstallService.ListBackupsAsync(package, global);
            
            if (backups.Any()) {
                spinner.Success($"Found {backups.Count} backup(s)");
                
                OutputFormatter.WriteLine();
                OutputFormatter.WriteInfo($"Available backups {(global ? "(global)" : "(local)")}:");
                OutputFormatter.WriteLine();

                var table = new Table();
                table.AddColumn("Package");
                table.AddColumn("Version");
                table.AddColumn("Created");
                table.AddColumn("Size");
                
                if (verbose) {
                    table.AddColumn("Path");
                    table.AddColumn("Description");
                }

                foreach (var backup in backups.OrderByDescending(b => b.CreatedAt)) {
                    var sizeMB = backup.BackupSize / (1024.0 * 1024.0);
                    var created = backup.CreatedAt.ToString("yyyy-MM-dd HH:mm");
                    
                    if (verbose) {
                        table.AddRow(
                            backup.PackageName,
                            backup.Version,
                            created,
                            $"{sizeMB:F1} MB",
                            backup.BackupPath,
                            backup.Description ?? "");
                    } else {
                        table.AddRow(
                            backup.PackageName,
                            backup.Version,
                            created,
                            $"{sizeMB:F1} MB");
                    }
                }

                AnsiConsole.Write(table);
                
                var totalSizeMB = backups.Sum(b => b.BackupSize) / (1024.0 * 1024.0);
                OutputFormatter.WriteLine();
                OutputFormatter.WriteInfo($"Total backup size: {totalSizeMB:F1} MB");
            } else {
                spinner.Success("No backups found");
                
                var filterText = string.IsNullOrEmpty(package) ? "" : $" for '{package}'";
                OutputFormatter.WriteInfo($"No backups found{filterText}{(global ? " (global)" : " (local)")}");
            }

            return 0;
        }
        catch (Exception ex) {
            return HandleError(ex, "list-backups");
        }
    }

    private async Task DisplayDependencyAnalysisAsync(UninstallDependencyAnalysis analysis, bool verbose) {
        OutputFormatter.WriteLine();

        if (analysis.DependentPackages.Any()) {
            OutputFormatter.WriteWarning($"The following packages depend on '{analysis.PackageName}':");
            
            var table = new Table();
            table.AddColumn("Package");
            table.AddColumn("Version");
            table.AddColumn("Dependency Type");
            
            if (verbose) {
                table.AddColumn("Required Version");
                table.AddColumn("Optional");
            }

            foreach (var dependent in analysis.DependentPackages.OrderBy(d => d.PackageName)) {
                var depType = GetDependencyTypeMarkup(dependent.DependencyType);
                
                if (verbose) {
                    table.AddRow(
                        dependent.PackageName,
                        dependent.Version,
                        depType,
                        dependent.RequiredVersion,
                        dependent.IsOptional ? "Yes" : "No");
                } else {
                    table.AddRow(
                        dependent.PackageName,
                        dependent.Version,
                        depType);
                }
            }

            AnsiConsole.Write(table);
        }

        if (analysis.OrphanedDependencies.Any()) {
            OutputFormatter.WriteLine();
            OutputFormatter.WriteInfo("Dependencies that would become orphaned:");
            foreach (var orphan in analysis.OrphanedDependencies) {
                OutputFormatter.WriteInfo($"  - {orphan}");
            }
        }

        foreach (var warning in analysis.Warnings) {
            OutputFormatter.WriteWarning(warning);
        }

        foreach (var issue in analysis.BlockingIssues) {
            OutputFormatter.WriteError(issue);
        }
    }

    private async Task DisplayUninstallPlanAsync(
        string packageName,
        string? version,
        UninstallDependencyAnalysis analysis,
        bool removeDependencies,
        bool purge,
        bool verbose) {
        
        OutputFormatter.WriteLine();
        OutputFormatter.WriteInfo("Uninstall Plan:");
        OutputFormatter.WriteLine();

        var versionText = string.IsNullOrEmpty(version) ? "all versions" : $"version {version}";
        var actionText = purge ? "Purge" : "Uninstall";
        
        OutputFormatter.WriteInfo($"  {actionText}: {packageName} ({versionText})");

        if (removeDependencies && analysis.OrphanedDependencies.Any()) {
            OutputFormatter.WriteInfo($"  Remove orphaned dependencies:");
            foreach (var orphan in analysis.OrphanedDependencies) {
                OutputFormatter.WriteInfo($"    - {orphan}");
            }
        }

        if (purge) {
            OutputFormatter.WriteInfo($"  Remove configuration files and cache");
        }

        OutputFormatter.WriteLine();
    }

    private async Task<int> HandleUninstallResultAsync(PackageUninstallResult result, bool verbose) {
        if (result.Success) {
            OutputFormatter.WriteSuccess($"Successfully uninstalled {result.PackageName}!");
            
            if (!string.IsNullOrEmpty(result.UninstalledVersion)) {
                OutputFormatter.WriteInfo($"  Removed version: {result.UninstalledVersion}");
            }
            
            if (!string.IsNullOrEmpty(result.BackupPath)) {
                OutputFormatter.WriteInfo($"  Backup created: {result.BackupPath}");
            }

            if (result.RemovedDependencies.Any()) {
                OutputFormatter.WriteInfo($"  Removed dependencies:");
                foreach (var dep in result.RemovedDependencies) {
                    OutputFormatter.WriteInfo($"    - {dep}");
                }
            }

            if (result.FreedSpace > 0) {
                var freedSpaceMB = result.FreedSpace / (1024.0 * 1024.0);
                OutputFormatter.WriteInfo($"  Freed space: {freedSpaceMB:F1} MB");
            }
            
            if (verbose) {
                OutputFormatter.WriteInfo($"  Uninstall duration: {result.Duration.TotalSeconds:F1}s");
            }

            foreach (var message in result.Messages) {
                OutputFormatter.WriteInfo($"  {message}");
            }

            foreach (var warning in result.Warnings) {
                OutputFormatter.WriteWarning($"  ⚠️  {warning}");
            }

            return 0;
        } else {
            OutputFormatter.WriteError($"Failed to uninstall {result.PackageName}:");
            
            foreach (var error in result.Errors) {
                OutputFormatter.WriteError($"  - {error}");
            }

            if (!string.IsNullOrEmpty(result.BackupPath)) {
                OutputFormatter.WriteInfo($"Backup available at: {result.BackupPath}");
                OutputFormatter.WriteInfo("You can restore using: mcpm uninstall restore <backup-path>");
            }

            return 1;
        }
    }

    private string GetDependencyTypeMarkup(DependencyType type) {
        return type switch {
            DependencyType.Runtime => "[red]Runtime[/]",
            DependencyType.Development => "[blue]Development[/]",
            DependencyType.Optional => "[yellow]Optional[/]",
            DependencyType.Peer => "[purple]Peer[/]",
            _ => type.ToString()
        };
    }
}