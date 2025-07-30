using System.CommandLine;
using System.CommandLine.Invocation;

using MCPHub.CommandLineApp.Configuration;
using MCPHub.CommandLineApp.Services;
using MCPHub.CommandLineApp.Utilities;

using Microsoft.Extensions.Logging;

using Spectre.Console;

namespace MCPHub.CommandLineApp.Commands;

/// <summary>
/// Command for updating MCP packages with dependency checking and rollback capability
/// </summary>
public class UpdateCommand(
    ILogger<UpdateCommand> logger,
    McpmConfiguration configuration,
    IMcpHubApiClient apiClient,
    IOutputFormatter outputFormatter,
    IInteractionService interactionService,
    IProgressReporter progressReporter,
    IUpdateService updateService) : BaseCommand(logger, configuration, apiClient, outputFormatter, interactionService, progressReporter) {
    private readonly IUpdateService _updateService = updateService ?? throw new ArgumentNullException(nameof(updateService));

    // Store references to command line elements for value extraction
    private Argument<string?>? _packageArgument;
    private Option<string?>? _versionOption;
    private Option<bool>? _globalOption;
    private Option<bool>? _prereleaseOption;
    private Option<bool>? _forceOption;
    private Option<bool>? _skipDependencyCheckOption;
    private Option<bool>? _noBackupOption;
    private Option<bool>? _showChangelogOption;
    private Option<bool>? _dryRunOption;
    private Option<bool>? _yesOption;
    private Option<bool>? _nonInteractiveOption;
    private Option<bool>? _verboseOption;

    /// <inheritdoc />
    public override Command CreateCommand() {
        var command = new Command("update", "Update MCP packages to their latest versions with dependency checking");

        // Arguments
        _packageArgument = new Argument<string?>("package", "Package name to update (optional - if not specified, checks all packages)") {
            Arity = ArgumentArity.ZeroOrOne
        };
        command.AddArgument(_packageArgument);

        // Options
        _versionOption = new Option<string?>(
            aliases: ["--version", "-v"],
            description: "Target version to update to (default: latest)");
        command.AddOption(_versionOption);

        _globalOption = new Option<bool>(
            aliases: ["--global", "-g"],
            description: "Update global packages");
        command.AddOption(_globalOption);

        _prereleaseOption = new Option<bool>(
            aliases: ["--prerelease", "--pre"],
            description: "Include prerelease versions");
        command.AddOption(_prereleaseOption);

        _forceOption = new Option<bool>(
            aliases: ["--force", "-f"],
            description: "Force update even if target version is older");
        command.AddOption(_forceOption);

        _skipDependencyCheckOption = new Option<bool>(
            aliases: ["--skip-dependency-check"],
            description: "Skip dependency compatibility checks (dangerous)");
        command.AddOption(_skipDependencyCheckOption);

        _noBackupOption = new Option<bool>(
            aliases: ["--no-backup"],
            description: "Skip backup creation before update");
        command.AddOption(_noBackupOption);

        _showChangelogOption = new Option<bool>(
            aliases: ["--changelog", "-c"],
            getDefaultValue: () => true,
            description: "Show changelog before updating");
        command.AddOption(_showChangelogOption);

        _dryRunOption = new Option<bool>(
            aliases: ["--dry-run"],
            description: "Show what would be updated without actually updating");
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
            description: "Show detailed update information");
        command.AddOption(_verboseOption);

        // Handler
        command.SetHandler(ExecuteAsync);

        return command;
    }

    private async Task<int> ExecuteAsync(InvocationContext context) {
        // Extract option values from the context using ParseResult
        var package = context.ParseResult.GetValueForArgument(_packageArgument!);
        var version = context.ParseResult.GetValueForOption(_versionOption!);
        var global = context.ParseResult.GetValueForOption(_globalOption!);
        var prerelease = context.ParseResult.GetValueForOption(_prereleaseOption!);
        var force = context.ParseResult.GetValueForOption(_forceOption!);
        var skipDependencyCheck = context.ParseResult.GetValueForOption(_skipDependencyCheckOption!);
        var noBackup = context.ParseResult.GetValueForOption(_noBackupOption!);
        var showChangelog = context.ParseResult.GetValueForOption(_showChangelogOption!);
        var dryRun = context.ParseResult.GetValueForOption(_dryRunOption!);
        var yes = context.ParseResult.GetValueForOption(_yesOption!);
        var nonInteractive = context.ParseResult.GetValueForOption(_nonInteractiveOption!);
        var verbose = context.ParseResult.GetValueForOption(_verboseOption!);

        try {
            Logger.LogInformation("Executing update command: Package={Package}, Version={Version}, Global={Global}",
                package, version, global);

            // Check API connectivity
            if (!await ValidateApiConnectivityAsync()) {
                return 503; // Service unavailable
            }

            // Determine update strategy
            if (string.IsNullOrWhiteSpace(package)) {
                // Update all packages
                return await UpdateAllPackagesAsync(global, prerelease, skipDependencyCheck, !noBackup, showChangelog, dryRun, yes, nonInteractive, verbose);
            }
            else {
                // Update specific package
                return await UpdateSpecificPackageAsync(package, version, global, prerelease, force, skipDependencyCheck, !noBackup, showChangelog, dryRun, yes, nonInteractive, verbose);
            }
        }
        catch (Exception ex) {
            return HandleError(ex, "update");
        }
    }

    private async Task<int> UpdateAllPackagesAsync(
        bool global,
        bool prerelease,
        bool skipDependencyCheck,
        bool createBackup,
        bool showChangelog,
        bool dryRun,
        bool yes,
        bool nonInteractive,
        bool verbose) {

        using var progress = ProgressReporter.CreateStepProgress("Package Updates", new[] {
            "Checking for updates",
            "Analyzing dependencies",
            "Updating packages"
        });

        // Stage 1: Check for updates
        progress.StartStep(0, "Scanning installed packages for updates...");

        try {
            var updateInfo = await _updateService.CheckForUpdatesAsync(global, prerelease);

            if (!updateInfo.Any()) {
                progress.CompleteStep(0, "All packages are up to date");
                progress.SkipStep(1, "No updates available");
                progress.SkipStep(2, "No updates available");

                OutputFormatter.WriteSuccess("All packages are already up to date!");
                return 0;
            }

            progress.CompleteStep(0, $"Found {updateInfo.Count} package(s) with updates available");

            // Display available updates
            await DisplayAvailableUpdatesAsync(updateInfo, verbose);

            if (dryRun) {
                progress.SkipStep(1, "Dry run mode");
                progress.SkipStep(2, "Dry run mode");
                OutputFormatter.WriteSuccess("Dry run completed. No packages were updated.");
                return 0;
            }

            // Confirmation
            if (!yes && !nonInteractive) {
                var confirmUpdate = await InteractionService.ConfirmAsync(
                    $"Update {updateInfo.Count} package(s)?", true);
                if (!confirmUpdate) {
                    progress.SkipStep(1, "User cancelled");
                    progress.SkipStep(2, "User cancelled");
                    OutputFormatter.WriteInfo("Update cancelled by user");
                    return 130;
                }
            }

            // Stage 2: Dependency analysis (if not skipped)
            if (!skipDependencyCheck) {
                progress.StartStep(1, "Analyzing dependency impacts...");

                var dependencyIssues = new List<string>();

                foreach (var update in updateInfo) {
                    var validation = await _updateService.ValidateUpdateDependenciesAsync(
                        update.PackageName, update.LatestVersion, global);

                    if (!validation.IsValid) {
                        dependencyIssues.AddRange(validation.Conflicts);
                    }
                }

                if (dependencyIssues.Any()) {
                    progress.FailStep(1, "Dependency conflicts detected");

                    OutputFormatter.WriteError("Dependency conflicts detected:");
                    foreach (var issue in dependencyIssues) {
                        OutputFormatter.WriteError($"  - {issue}");
                    }

                    if (!nonInteractive && !yes) {
                        var continueAnyway = await InteractionService.ConfirmAsync(
                            "Continue with updates despite conflicts? (may break dependencies)", false);
                        if (!continueAnyway) {
                            progress.SkipStep(2, "Cancelled due to conflicts");
                            return 130;
                        }
                    }
                    else {
                        OutputFormatter.WriteError("Update cancelled due to dependency conflicts");
                        return 1;
                    }
                }

                progress.CompleteStep(1, "Dependency analysis completed");
            }
            else {
                progress.SkipStep(1, "Dependency check skipped");
            }

            // Stage 3: Perform updates
            progress.StartStep(2, "Updating packages...");

            var result = await _updateService.UpdateAllPackagesAsync(global, prerelease, skipDependencyCheck, createBackup);

            if (result.Success) {
                progress.CompleteStep(2, $"Successfully updated {result.UpdatedPackages} packages");
                return await HandleBatchUpdateResultAsync(result, verbose);
            }
            else {
                progress.FailStep(2, "Some updates failed");
                return await HandleBatchUpdateResultAsync(result, verbose);
            }
        }
        catch (Exception ex) {
            progress.FailStep(0, $"Update check failed: {ex.Message}");
            throw;
        }
    }

    private async Task<int> UpdateSpecificPackageAsync(
        string packageName,
        string? targetVersion,
        bool global,
        bool prerelease,
        bool force,
        bool skipDependencyCheck,
        bool createBackup,
        bool showChangelog,
        bool dryRun,
        bool yes,
        bool nonInteractive,
        bool verbose) {

        using var progress = ProgressReporter.CreateStepProgress("Package Update", new[] {
            "Checking package updates",
            "Analyzing dependencies",
            "Displaying changelog",
            "Updating package"
        });

        // Stage 1: Check for updates
        progress.StartStep(0, $"Checking updates for '{packageName}'...");

        try {
            var updateInfo = await _updateService.CheckPackageUpdatesAsync(packageName, global, prerelease);

            if (updateInfo == null) {
                progress.CompleteStep(0, "Package is up to date");
                progress.SkipStep(1, "No updates available");
                progress.SkipStep(2, "No updates available");
                progress.SkipStep(3, "No updates available");

                OutputFormatter.WriteSuccess($"Package '{packageName}' is already up to date!");
                return 0;
            }

            progress.CompleteStep(0, $"Update available: {updateInfo.CurrentVersion} → {updateInfo.LatestVersion}");

            // Display update information
            await DisplayPackageUpdateInfoAsync(updateInfo, verbose);

            // Stage 2: Dependency validation (if not skipped)
            if (!skipDependencyCheck) {
                progress.StartStep(1, "Validating dependencies...");

                var validation = await _updateService.ValidateUpdateDependenciesAsync(
                    packageName, targetVersion ?? updateInfo.LatestVersion, global);

                if (!validation.IsValid) {
                    progress.FailStep(1, "Dependency conflicts detected");

                    OutputFormatter.WriteError("Dependency conflicts detected:");
                    foreach (var conflict in validation.Conflicts) {
                        OutputFormatter.WriteError($"  - {conflict}");
                    }

                    if (!force && !nonInteractive && !yes) {
                        var continueAnyway = await InteractionService.ConfirmAsync(
                            "Continue with update despite conflicts?", false);
                        if (!continueAnyway) {
                            progress.SkipStep(2, "Cancelled due to conflicts");
                            progress.SkipStep(3, "Cancelled due to conflicts");
                            return 130;
                        }
                    }
                    else if (!force) {
                        OutputFormatter.WriteError("Update cancelled due to dependency conflicts");
                        return 1;
                    }
                }

                progress.CompleteStep(1, "Dependencies validated");
            }
            else {
                progress.SkipStep(1, "Dependency check skipped");
            }

            // Stage 3: Show changelog (if requested)
            if (showChangelog) {
                progress.StartStep(2, "Retrieving changelog...");

                var changelog = await _updateService.GetChangelogAsync(
                    packageName, updateInfo.CurrentVersion, targetVersion ?? updateInfo.LatestVersion);

                if (changelog != null) {
                    await DisplayChangelogAsync(changelog, nonInteractive);
                    progress.CompleteStep(2, "Changelog displayed");
                }
                else {
                    progress.CompleteStep(2, "No changelog available");
                }
            }
            else {
                progress.SkipStep(2, "Changelog display disabled");
            }

            if (dryRun) {
                progress.SkipStep(3, "Dry run mode");
                OutputFormatter.WriteSuccess("Dry run completed. Package was not updated.");
                return 0;
            }

            // Final confirmation
            if (!yes && !nonInteractive) {
                var confirmUpdate = await InteractionService.ConfirmAsync(
                    $"Update '{packageName}' from {updateInfo.CurrentVersion} to {targetVersion ?? updateInfo.LatestVersion}?", true);
                if (!confirmUpdate) {
                    progress.SkipStep(3, "User cancelled");
                    OutputFormatter.WriteInfo("Update cancelled by user");
                    return 130;
                }
            }

            // Stage 4: Perform update
            progress.StartStep(3, $"Updating {packageName}...");

            var result = await _updateService.UpdatePackageAsync(
                packageName, targetVersion, global, force, skipDependencyCheck, createBackup);

            if (result.Success) {
                progress.CompleteStep(3, $"Successfully updated to {result.NewVersion}");
                return await HandleUpdateResultAsync(result, verbose);
            }
            else {
                progress.FailStep(3, "Update failed");
                return await HandleUpdateResultAsync(result, verbose);
            }
        }
        catch (Exception ex) {
            progress.FailStep(0, $"Update check failed: {ex.Message}");
            throw;
        }
    }

    private Task DisplayAvailableUpdatesAsync(List<PackageUpdateInfo> updates, bool verbose) {
        OutputFormatter.WriteLine();
        OutputFormatter.WriteInfo($"Found {updates.Count} package(s) with updates available:");
        OutputFormatter.WriteLine();

        var table = new Table();
        table.AddColumn("Package");
        table.AddColumn("Current");
        table.AddColumn("Latest");
        table.AddColumn("Type");

        if (verbose) {
            table.AddColumn("Release Date");
            table.AddColumn("Changes");
        }

        foreach (var update in updates.OrderBy(u => u.PackageName)) {
            var changeType = GetUpdateChangeType(update);
            var changeMarkup = GetChangeTypeMarkup(changeType);

            if (verbose) {
                var releaseDate = update.ReleaseDate.ToString("yyyy-MM-dd");
                var changes = GetUpdateSummary(update);

                table.AddRow(
                    update.PackageName,
                    update.CurrentVersion,
                    update.LatestVersion,
                    changeMarkup,
                    releaseDate,
                    changes);
            }
            else {
                table.AddRow(
                    update.PackageName,
                    update.CurrentVersion,
                    update.LatestVersion,
                    changeMarkup);
            }
        }

        AnsiConsole.Write(table);
        OutputFormatter.WriteLine();
        return Task.CompletedTask;
    }

    private Task DisplayPackageUpdateInfoAsync(PackageUpdateInfo updateInfo, bool verbose) {
        OutputFormatter.WriteLine();
        OutputFormatter.WriteInfo($"Update available for '{updateInfo.PackageName}':");
        OutputFormatter.WriteInfo($"  Current version: {updateInfo.CurrentVersion}");
        OutputFormatter.WriteInfo($"  Latest version:  {updateInfo.LatestVersion}");

        if (updateInfo.IsPrerelease) {
            OutputFormatter.WriteWarning("  Note: This is a prerelease version");
        }

        if (updateInfo.HasBreakingChanges) {
            OutputFormatter.WriteWarning("  ⚠️  Contains breaking changes");
        }

        if (updateInfo.HasSecurityFixes) {
            OutputFormatter.WriteSuccess("  🛡️  Contains security fixes");
        }

        if (verbose && !string.IsNullOrEmpty(updateInfo.ReleaseNotes)) {
            OutputFormatter.WriteLine();
            OutputFormatter.WriteInfo("Release Notes:");
            OutputFormatter.WriteInfo(updateInfo.ReleaseNotes);
        }

        if (updateInfo.DependencyImpacts.Any()) {
            OutputFormatter.WriteLine();
            OutputFormatter.WriteWarning("Dependency impacts:");
            foreach (var impact in updateInfo.DependencyImpacts) {
                OutputFormatter.WriteWarning($"  - {impact}");
            }
        }

        OutputFormatter.WriteLine();
        return Task.CompletedTask;
    }

    private Task DisplayChangelogAsync(PackageChangelog changelog, bool nonInteractive) {
        if (nonInteractive)
            return Task.CompletedTask;

        OutputFormatter.WriteLine();
        OutputFormatter.WriteInfo($"Changelog for {changelog.PackageName} ({changelog.FromVersion} → {changelog.ToVersion}):");
        OutputFormatter.WriteLine();

        if (changelog.HasBreakingChanges) {
            OutputFormatter.WriteWarning("⚠️  This update contains BREAKING CHANGES");
        }

        if (changelog.HasSecurityFixes) {
            OutputFormatter.WriteSuccess("🛡️  This update contains security fixes");
        }

        if (changelog.Entries.Any()) {
            var groupedEntries = changelog.Entries.GroupBy(e => e.Type);

            foreach (var group in groupedEntries.OrderBy(g => g.Key)) {
                var sectionTitle = GetChangelogSectionTitle(group.Key);
                OutputFormatter.WriteInfo(sectionTitle);

                foreach (var entry in group.OrderByDescending(e => e.Date)) {
                    var prefix = GetChangelogEntryPrefix(entry.Type);
                    OutputFormatter.WriteInfo($"  {prefix} {entry.Description}");

                    if (!string.IsNullOrEmpty(entry.Author)) {
                        OutputFormatter.WriteInfo($"    by {entry.Author}");
                    }
                }

                OutputFormatter.WriteLine();
            }
        }
        else {
            OutputFormatter.WriteInfo("No detailed changelog available");
        }

        return Task.CompletedTask;
    }

    private Task<int> HandleUpdateResultAsync(PackageUpdateResult result, bool verbose) {
        if (result.Success) {
            OutputFormatter.WriteSuccess($"Successfully updated {result.PackageName}!");
            OutputFormatter.WriteInfo($"  Previous version: {result.PreviousVersion}");
            OutputFormatter.WriteInfo($"  New version: {result.NewVersion}");

            if (!string.IsNullOrEmpty(result.BackupPath)) {
                OutputFormatter.WriteInfo($"  Backup created: {result.BackupPath}");
            }

            if (verbose) {
                OutputFormatter.WriteInfo($"  Update duration: {result.Duration.TotalSeconds:F1}s");
            }

            foreach (var message in result.Messages) {
                OutputFormatter.WriteInfo($"  {message}");
            }

            foreach (var warning in result.Warnings) {
                OutputFormatter.WriteWarning($"  ⚠️  {warning}");
            }

            return Task.FromResult(0);
        }
        else {
            OutputFormatter.WriteError($"Failed to update {result.PackageName}:");

            foreach (var error in result.Errors) {
                OutputFormatter.WriteError($"  - {error}");
            }

            if (!string.IsNullOrEmpty(result.BackupPath)) {
                OutputFormatter.WriteInfo($"Backup available at: {result.BackupPath}");
                OutputFormatter.WriteInfo("You can restore using: mcpm update --rollback");
            }

            return Task.FromResult(1);
        }
    }

    private Task<int> HandleBatchUpdateResultAsync(BatchUpdateResult result, bool verbose) {
        OutputFormatter.WriteLine();

        if (result.Success) {
            OutputFormatter.WriteSuccess($"Batch update completed successfully!");
        }
        else {
            OutputFormatter.WriteWarning("Batch update completed with some failures");
        }

        OutputFormatter.WriteInfo($"Summary:");
        OutputFormatter.WriteInfo($"  Total packages: {result.TotalPackages}");
        OutputFormatter.WriteInfo($"  Updated: {result.UpdatedPackages}");

        if (result.FailedPackages > 0) {
            OutputFormatter.WriteError($"  Failed: {result.FailedPackages}");
        }

        if (result.SkippedPackages > 0) {
            OutputFormatter.WriteWarning($"  Skipped: {result.SkippedPackages}");
        }

        if (verbose) {
            OutputFormatter.WriteInfo($"  Total duration: {result.Duration.TotalSeconds:F1}s");
        }

        if (!string.IsNullOrEmpty(result.BackupPath)) {
            OutputFormatter.WriteInfo($"  Batch backup: {result.BackupPath}");
        }

        // Show detailed results if requested
        if (verbose && result.Results.Any()) {
            OutputFormatter.WriteLine();
            OutputFormatter.WriteInfo("Detailed results:");

            foreach (var packageResult in result.Results.OrderBy(r => r.PackageName)) {
                var status = packageResult.Success ? "✅" : "❌";
                OutputFormatter.WriteInfo($"  {status} {packageResult.PackageName}: {packageResult.PreviousVersion} → {packageResult.NewVersion}");

                if (!packageResult.Success) {
                    foreach (var error in packageResult.Errors) {
                        OutputFormatter.WriteError($"    - {error}");
                    }
                }
            }
        }

        return Task.FromResult(result.Success ? 0 : 1);
    }

    private string GetUpdateChangeType(PackageUpdateInfo update) {
        if (update.HasBreakingChanges)
            return "Major";
        if (update.HasSecurityFixes)
            return "Security";
        if (update.IsPrerelease)
            return "Prerelease";
        return "Minor";
    }

    private string GetChangeTypeMarkup(string changeType) => changeType switch {
        "Major" => "[red]Major[/]",
        "Security" => "[green]Security[/]",
        "Prerelease" => "[yellow]Prerelease[/]",
        "Minor" => "[blue]Minor[/]",
        _ => changeType
    };

    private string GetUpdateSummary(PackageUpdateInfo update) {
        var summaries = new List<string>();

        if (update.HasSecurityFixes)
            summaries.Add("Security fixes");
        if (update.HasBreakingChanges)
            summaries.Add("Breaking changes");
        if (update.DependencyImpacts.Any())
            summaries.Add("Dependency changes");

        return summaries.Any() ? string.Join(", ", summaries) : "Updates";
    }

    private string GetChangelogSectionTitle(ChangelogEntryType type) => type switch {
        ChangelogEntryType.Feature => "🆕 New Features:",
        ChangelogEntryType.BugFix => "🐛 Bug Fixes:",
        ChangelogEntryType.SecurityFix => "🛡️ Security Fixes:",
        ChangelogEntryType.BreakingChange => "💥 Breaking Changes:",
        ChangelogEntryType.Performance => "⚡ Performance Improvements:",
        ChangelogEntryType.Documentation => "📚 Documentation:",
        ChangelogEntryType.Dependency => "📦 Dependencies:",
        _ => "📝 Other Changes:"
    };

    private string GetChangelogEntryPrefix(ChangelogEntryType type) => type switch {
        ChangelogEntryType.Feature => "✨",
        ChangelogEntryType.BugFix => "🔧",
        ChangelogEntryType.SecurityFix => "🛡️",
        ChangelogEntryType.BreakingChange => "💥",
        ChangelogEntryType.Performance => "⚡",
        ChangelogEntryType.Documentation => "📖",
        ChangelogEntryType.Dependency => "📦",
        _ => "•"
    };
}