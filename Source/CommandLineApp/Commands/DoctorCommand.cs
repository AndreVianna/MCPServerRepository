using MCPHub.CommandLineApp.Configuration;
using MCPHub.CommandLineApp.Services;
using MCPHub.CommandLineApp.Utilities;

namespace MCPHub.CommandLineApp.Commands;

/// <summary>
/// Command for comprehensive system health checking and diagnostics
/// </summary>
public class DoctorCommand(
    ILogger<DoctorCommand> logger,
    McpmConfiguration configuration,
    IMcpHubApiClient apiClient,
    IOutputFormatter outputFormatter,
    IInteractionService interactionService,
    IProgressReporter progressReporter,
    IDoctorService doctorService) : BaseCommand(logger, configuration, apiClient, outputFormatter, interactionService, progressReporter) {
    private readonly IDoctorService _doctorService = doctorService ?? throw new ArgumentNullException(nameof(doctorService));

    /// <inheritdoc />
    public override Command CreateCommand() {
        var command = new Command("doctor", "Perform comprehensive system health checks and diagnostics");

        // Add alias
        command.AddAlias("health");

        // Options
        var skipPackageChecksOption = new Option<bool>(
            aliases: ["--skip-packages"],
            description: "Skip individual package integrity checks");
        command.AddOption(skipPackageChecksOption);

        var skipSecurityScanOption = new Option<bool>(
            aliases: ["--skip-security"],
            description: "Skip security vulnerability scanning");
        command.AddOption(skipSecurityScanOption);

        var includePerformanceOption = new Option<bool>(
            aliases: ["--performance", "--perf"],
            description: "Include performance analysis (slower)");
        command.AddOption(includePerformanceOption);

        var autoFixOption = new Option<bool>(
            aliases: ["--fix", "--auto-fix"],
            description: "Automatically attempt to fix found issues");
        command.AddOption(autoFixOption);

        var verboseOption = new Option<bool>(
            aliases: ["--verbose", "-v"],
            description: "Show detailed diagnostic information");
        command.AddOption(verboseOption);

        var jsonOutputOption = new Option<bool>(
            aliases: ["--json"],
            description: "Output results in JSON format");
        command.AddOption(jsonOutputOption);

        var nonInteractiveOption = new Option<bool>(
            aliases: ["--non-interactive", "-n"],
            description: "Disable interactive prompts (for CI/CD)");
        command.AddOption(nonInteractiveOption);

        // Subcommands
        var integrityCommand = CreateIntegrityCommand();
        command.AddCommand(integrityCommand);

        var dependenciesCommand = CreateDependenciesCommand();
        command.AddCommand(dependenciesCommand);

        var cacheCommand = CreateCacheCommand();
        command.AddCommand(cacheCommand);

        var configCommand = CreateConfigCommand();
        command.AddCommand(configCommand);

        var securityCommand = CreateSecurityCommand();
        command.AddCommand(securityCommand);

        var performanceCommand = CreatePerformanceCommand();
        command.AddCommand(performanceCommand);

        var repairCommand = CreateRepairCommand();
        command.AddCommand(repairCommand);

        var envCommand = CreateEnvironmentCommand();
        command.AddCommand(envCommand);

        // Handler
        command.SetHandler(
            ExecuteAsync,
            skipPackageChecksOption,
            skipSecurityScanOption,
            includePerformanceOption,
            autoFixOption,
            verboseOption,
            jsonOutputOption,
            nonInteractiveOption);

        return command;
    }

    private Command CreateIntegrityCommand() {
        var command = new Command("integrity", "Check package integrity and file consistency");

        var packageArgument = new Argument<string?>("package", "Specific package to check (optional)") {
            Arity = ArgumentArity.ZeroOrOne,
        };
        command.AddArgument(packageArgument);

        var globalOption = new Option<bool>(
            aliases: ["--global", "-g"],
            description: "Check global packages");
        command.AddOption(globalOption);

        var deepOption = new Option<bool>(
            aliases: ["--deep"],
            description: "Perform deep integrity checks (slower)");
        command.AddOption(deepOption);

        var verboseOption = new Option<bool>(
            aliases: ["--verbose", "-v"],
            description: "Show detailed integrity information");
        command.AddOption(verboseOption);

        command.SetHandler(
            ExecuteIntegrityAsync,
            packageArgument,
            globalOption,
            deepOption,
            verboseOption);

        return command;
    }

    private Command CreateDependenciesCommand() {
        var command = new Command("dependencies", "Validate dependency consistency");
        command.AddAlias("deps");

        var globalOption = new Option<bool>(
            aliases: ["--global", "-g"],
            description: "Check global dependencies");
        command.AddOption(globalOption);

        var autoResolveOption = new Option<bool>(
            aliases: ["--auto-resolve"],
            description: "Automatically resolve conflicts where possible");
        command.AddOption(autoResolveOption);

        var verboseOption = new Option<bool>(
            aliases: ["--verbose", "-v"],
            description: "Show detailed dependency information");
        command.AddOption(verboseOption);

        command.SetHandler(
            ExecuteDependenciesAsync,
            globalOption,
            autoResolveOption,
            verboseOption);

        return command;
    }

    private Command CreateCacheCommand() {
        var command = new Command("cache", "Check and optimize cache health");

        var clearCorruptedOption = new Option<bool>(
            aliases: ["--clear-corrupted"],
            getDefaultValue: () => true,
            description: "Clear corrupted cache entries");
        command.AddOption(clearCorruptedOption);

        var compactOption = new Option<bool>(
            aliases: ["--compact"],
            description: "Compact cache files to save space");
        command.AddOption(compactOption);

        var verboseOption = new Option<bool>(
            aliases: ["--verbose", "-v"],
            description: "Show detailed cache information");
        command.AddOption(verboseOption);

        command.SetHandler(
            ExecuteCacheAsync,
            clearCorruptedOption,
            compactOption,
            verboseOption);

        return command;
    }

    private Command CreateConfigCommand() {
        var command = new Command("config", "Validate configuration and settings");

        var checkPermissionsOption = new Option<bool>(
            aliases: ["--check-permissions"],
            getDefaultValue: () => true,
            description: "Check file permissions");
        command.AddOption(checkPermissionsOption);

        var validatePathsOption = new Option<bool>(
            aliases: ["--validate-paths"],
            getDefaultValue: () => true,
            description: "Validate configured paths");
        command.AddOption(validatePathsOption);

        var verboseOption = new Option<bool>(
            aliases: ["--verbose", "-v"],
            description: "Show detailed configuration information");
        command.AddOption(verboseOption);

        command.SetHandler(
            ExecuteConfigAsync,
            checkPermissionsOption,
            validatePathsOption,
            verboseOption);

        return command;
    }

    private Command CreateSecurityCommand() {
        var command = new Command("security", "Perform security audit and vulnerability scan");

        var scanVulnerabilitiesOption = new Option<bool>(
            aliases: ["--scan-vulnerabilities"],
            getDefaultValue: () => true,
            description: "Scan for known vulnerabilities");
        command.AddOption(scanVulnerabilitiesOption);

        var checkTrustTiersOption = new Option<bool>(
            aliases: ["--check-trust-tiers"],
            getDefaultValue: () => true,
            description: "Validate package trust tiers");
        command.AddOption(checkTrustTiersOption);

        var analyzePermissionsOption = new Option<bool>(
            aliases: ["--analyze-permissions"],
            description: "Analyze package permissions (advanced)");
        command.AddOption(analyzePermissionsOption);

        var verboseOption = new Option<bool>(
            aliases: ["--verbose", "-v"],
            description: "Show detailed security information");
        command.AddOption(verboseOption);

        command.SetHandler(
            ExecuteSecurityAsync,
            scanVulnerabilitiesOption,
            checkTrustTiersOption,
            analyzePermissionsOption,
            verboseOption);

        return command;
    }

    private Command CreatePerformanceCommand() {
        var command = new Command("performance", "Analyze system performance and optimization opportunities");
        command.AddAlias("perf");

        var measureStartupOption = new Option<bool>(
            aliases: ["--startup"],
            getDefaultValue: () => true,
            description: "Measure CLI startup performance");
        command.AddOption(measureStartupOption);

        var analyzeDiskUsageOption = new Option<bool>(
            aliases: ["--disk"],
            getDefaultValue: () => true,
            description: "Analyze disk usage patterns");
        command.AddOption(analyzeDiskUsageOption);

        var checkNetworkLatencyOption = new Option<bool>(
            aliases: ["--network"],
            getDefaultValue: () => true,
            description: "Check network connectivity performance");
        command.AddOption(checkNetworkLatencyOption);

        var verboseOption = new Option<bool>(
            aliases: ["--verbose", "-v"],
            description: "Show detailed performance metrics");
        command.AddOption(verboseOption);

        command.SetHandler(
            ExecutePerformanceAsync,
            measureStartupOption,
            analyzeDiskUsageOption,
            checkNetworkLatencyOption,
            verboseOption);

        return command;
    }

    private Command CreateRepairCommand() {
        var command = new Command("repair", "Attempt to repair identified issues");

        var issueIdsOption = new Option<string[]>(
            aliases: ["--issues"],
            description: "Specific issue IDs to repair (comma-separated)") {
            Arity = ArgumentArity.ZeroOrMore,
            AllowMultipleArgumentsPerToken = true,
        };
        command.AddOption(issueIdsOption);

        var createBackupOption = new Option<bool>(
            aliases: ["--backup"],
            getDefaultValue: () => true,
            description: "Create backup before repairs");
        command.AddOption(createBackupOption);

        var yesOption = new Option<bool>(
            aliases: ["--yes", "-y"],
            description: "Automatically confirm all repair actions");
        command.AddOption(yesOption);

        var verboseOption = new Option<bool>(
            aliases: ["--verbose", "-v"],
            description: "Show detailed repair information");
        command.AddOption(verboseOption);

        command.SetHandler(
            ExecuteRepairAsync,
            issueIdsOption,
            createBackupOption,
            yesOption,
            verboseOption);

        return command;
    }

    private Command CreateEnvironmentCommand() {
        var command = new Command("environment", "Display system environment information");
        command.AddAlias("env");

        var includeSystemInfoOption = new Option<bool>(
            aliases: ["--system"],
            getDefaultValue: () => true,
            description: "Include system information");
        command.AddOption(includeSystemInfoOption);

        var includeEnvironmentVarsOption = new Option<bool>(
            aliases: ["--env-vars"],
            description: "Include environment variables");
        command.AddOption(includeEnvironmentVarsOption);

        var includePathsOption = new Option<bool>(
            aliases: ["--paths"],
            getDefaultValue: () => true,
            description: "Include path information");
        command.AddOption(includePathsOption);

        var verboseOption = new Option<bool>(
            aliases: ["--verbose", "-v"],
            description: "Show detailed environment information");
        command.AddOption(verboseOption);

        command.SetHandler(
            ExecuteEnvironmentAsync,
            includeSystemInfoOption,
            includeEnvironmentVarsOption,
            includePathsOption,
            verboseOption);

        return command;
    }

    private async Task<int> ExecuteAsync(
        bool skipPackageChecks,
        bool skipSecurityScan,
        bool includePerformance,
        bool autoFix,
        bool verbose,
        bool jsonOutput,
        bool nonInteractive) {
        try {
            Logger.LogInformation("Executing doctor command: SkipPackages={SkipPackages}, SkipSecurity={SkipSecurity}, Performance={Performance}",
                skipPackageChecks, skipSecurityScan, includePerformance);

            if (!jsonOutput) {
                OutputFormatter.WriteInfo("🩺 MCP Hub System Health Check");
                OutputFormatter.WriteInfo("Running comprehensive system diagnostics...");
                OutputFormatter.WriteLine();
            }

            var healthCheck = await _doctorService.PerformHealthCheckAsync(
                includePackageChecks: !skipPackageChecks,
                includeSecurityScan: !skipSecurityScan,
                includePerformanceAnalysis: includePerformance,
                autoFix: autoFix);

            if (jsonOutput) {
                await OutputHealthCheckAsJsonAsync(healthCheck);
            }
            else {
                await DisplayHealthCheckResultAsync(healthCheck, verbose, nonInteractive, autoFix);
            }

            // Return appropriate exit code based on health
            return healthCheck.OverallScore switch {
                HealthScore.Critical => 2,
                HealthScore.Poor => 1,
                _ => 0,
            };
        }
        catch (Exception ex) {
            return HandleError(ex, "doctor");
        }
    }

    private async Task<int> ExecuteIntegrityAsync(string? package, bool global, bool deep, bool verbose) {
        try {
            Logger.LogInformation("Executing integrity check: Package={Package}, Global={Global}, Deep={Deep}",
                package, global, deep);

            using var spinner = ProgressReporter.CreateSpinner(
                string.IsNullOrEmpty(package) ? "Checking package integrity..." : $"Checking integrity of '{package}'...");

            var result = await _doctorService.VerifyPackageIntegrityAsync(package, global, deep);

            if (result.IsValid) {
                spinner.Success($"Integrity check completed: {result.ValidPackages}/{result.TotalPackagesChecked} packages valid");
            }
            else {
                spinner.Fail($"Integrity issues found: {result.CorruptedPackages} corrupted packages");
            }

            await DisplayIntegrityResultAsync(result, verbose);

            return result.IsValid ? 0 : 1;
        }
        catch (Exception ex) {
            return HandleError(ex, "integrity");
        }
    }

    private async Task<int> ExecuteDependenciesAsync(bool global, bool autoResolve, bool verbose) {
        try {
            Logger.LogInformation("Executing dependency validation: Global={Global}, AutoResolve={AutoResolve}",
                global, autoResolve);

            using var spinner = ProgressReporter.CreateSpinner("Validating dependency consistency...");

            var result = await _doctorService.ValidateDependencyConsistencyAsync(global, autoResolve);

            if (result.IsConsistent) {
                spinner.Success("Dependencies are consistent");
            }
            else {
                spinner.Fail($"Dependency conflicts found: {result.Conflicts.Count} conflicts");
            }

            await DisplayDependencyResultAsync(result, verbose);

            return result.IsConsistent ? 0 : 1;
        }
        catch (Exception ex) {
            return HandleError(ex, "dependencies");
        }
    }

    private async Task<int> ExecuteCacheAsync(bool clearCorrupted, bool compact, bool verbose) {
        try {
            Logger.LogInformation("Executing cache health check: ClearCorrupted={ClearCorrupted}, Compact={Compact}",
                clearCorrupted, compact);

            using var spinner = ProgressReporter.CreateSpinner("Checking cache health...");

            var result = await _doctorService.CheckCacheHealthAsync(clearCorrupted, compact);

            if (result.IsHealthy) {
                spinner.Success("Cache is healthy");
            }
            else {
                spinner.Warning($"Cache issues found: {result.CorruptedEntries} corrupted entries");
            }

            await DisplayCacheResultAsync(result, verbose);

            return result.IsHealthy ? 0 : 1;
        }
        catch (Exception ex) {
            return HandleError(ex, "cache");
        }
    }

    private async Task<int> ExecuteConfigAsync(bool checkPermissions, bool validatePaths, bool verbose) {
        try {
            Logger.LogInformation("Executing configuration validation: CheckPermissions={CheckPermissions}, ValidatePaths={ValidatePaths}",
                checkPermissions, validatePaths);

            using var spinner = ProgressReporter.CreateSpinner("Validating configuration...");

            var result = await _doctorService.ValidateConfigurationAsync(checkPermissions, validatePaths);

            if (result.IsValid) {
                spinner.Success("Configuration is valid");
            }
            else {
                spinner.Fail($"Configuration issues found: {result.Issues.Count} issues");
            }

            await DisplayConfigurationResultAsync(result, verbose);

            return result.IsValid ? 0 : 1;
        }
        catch (Exception ex) {
            return HandleError(ex, "config");
        }
    }

    private async Task<int> ExecuteSecurityAsync(bool scanVulnerabilities, bool checkTrustTiers, bool analyzePermissions, bool verbose) {
        try {
            Logger.LogInformation("Executing security audit: ScanVulnerabilities={ScanVulnerabilities}, CheckTrustTiers={CheckTrustTiers}",
                scanVulnerabilities, checkTrustTiers);

            using var spinner = ProgressReporter.CreateSpinner("Performing security audit...");

            var result = await _doctorService.PerformSecurityAuditAsync(scanVulnerabilities, checkTrustTiers, analyzePermissions);

            if (result.IsSecure) {
                spinner.Success("Security audit passed");
            }
            else {
                spinner.Fail($"Security issues found: {result.CriticalVulnerabilities} critical, {result.HighVulnerabilities} high severity");
            }

            await DisplaySecurityResultAsync(result, verbose);

            return result.IsSecure ? 0 : (result.CriticalVulnerabilities > 0 ? 2 : 1);
        }
        catch (Exception ex) {
            return HandleError(ex, "security");
        }
    }

    private async Task<int> ExecutePerformanceAsync(bool measureStartup, bool analyzeDiskUsage, bool checkNetworkLatency, bool verbose) {
        try {
            Logger.LogInformation("Executing performance analysis: Startup={Startup}, DiskUsage={DiskUsage}, Network={Network}",
                measureStartup, analyzeDiskUsage, checkNetworkLatency);

            using var spinner = ProgressReporter.CreateSpinner("Analyzing performance...");

            var result = await _doctorService.AnalyzePerformanceAsync(measureStartup, analyzeDiskUsage, checkNetworkLatency);

            var scoreText = GetHealthScoreText(result.PerformanceScore);
            spinner.Success($"Performance analysis completed: {scoreText}");

            await DisplayPerformanceResultAsync(result, verbose);

            return result.PerformanceScore == HealthScore.Critical ? 2 : 0;
        }
        catch (Exception ex) {
            return HandleError(ex, "performance");
        }
    }

    private Task<int> ExecuteRepairAsync(string[] issueIds, bool createBackup, bool yes, bool verbose) {
        try {
            Logger.LogInformation("Executing repair: IssueIds={IssueIds}, Backup={Backup}",
                string.Join(",", issueIds), createBackup);

            // For now, this is a placeholder - real implementation would first check for issues
            OutputFormatter.WriteInfo("Repair functionality requires identified issues from a health check.");
            OutputFormatter.WriteInfo("Run 'mcpm doctor' first to identify issues that can be repaired.");

            return Task.FromResult(0);
        }
        catch (Exception ex) {
            return Task.FromResult(HandleError(ex, "repair"));
        }
    }

    private async Task<int> ExecuteEnvironmentAsync(bool includeSystemInfo, bool includeEnvironmentVars, bool includePaths, bool verbose) {
        try {
            Logger.LogInformation("Executing environment info: SystemInfo={SystemInfo}, EnvVars={EnvVars}, Paths={Paths}",
                includeSystemInfo, includeEnvironmentVars, includePaths);

            using var spinner = ProgressReporter.CreateSpinner("Gathering environment information...");

            var result = await _doctorService.GetSystemEnvironmentAsync(includeSystemInfo, includeEnvironmentVars, includePaths);

            spinner.Success("Environment information gathered");

            await DisplayEnvironmentInfoAsync(result, verbose);

            return 0;
        }
        catch (Exception ex) {
            return HandleError(ex, "environment");
        }
    }

    private async Task DisplayHealthCheckResultAsync(SystemHealthCheckResult result, bool verbose, bool nonInteractive, bool autoFix) {
        // Overall status
        var scoreText = GetHealthScoreText(result.OverallScore);
        var scoreColor = GetHealthScoreColor(result.OverallScore);

        OutputFormatter.WriteInfo($"[{scoreColor}]Overall Health: {scoreText}[/]");
        OutputFormatter.WriteInfo($"Check completed in {result.CheckDuration.TotalSeconds:F1}s");
        OutputFormatter.WriteLine();

        // Category results
        if (result.Categories.Any()) {
            OutputFormatter.WriteInfo("Health Check Categories:");
            OutputFormatter.WriteLine();

            var table = new Table();
            table.AddColumn("Category");
            table.AddColumn("Status");
            table.AddColumn("Score");
            table.AddColumn("Issues");

            if (verbose) {
                table.AddColumn("Duration");
            }

            foreach (var category in result.Categories.OrderBy(c => c.Name)) {
                var categoryScoreText = GetHealthScoreText(category.Score);
                var categoryScoreColor = GetHealthScoreColor(category.Score);
                var issueCount = category.Issues.Count.ToString();

                if (verbose) {
                    table.AddRow(
                        category.Name,
                        category.Status,
                        $"[{categoryScoreColor}]{categoryScoreText}[/]",
                        issueCount,
                        $"{category.CheckDuration.TotalSeconds:F1}s");
                }
                else {
                    table.AddRow(
                        category.Name,
                        category.Status,
                        $"[{categoryScoreColor}]{categoryScoreText}[/]",
                        issueCount);
                }
            }

            AnsiConsole.Write(table);
            OutputFormatter.WriteLine();
        }

        // Issues summary
        if (result.Issues.Any()) {
            await DisplayIssuesSummaryAsync(result.Issues, verbose);
        }

        // Recommendations
        if (result.Recommendations.Any()) {
            OutputFormatter.WriteInfo("Recommendations:");
            foreach (var recommendation in result.Recommendations) {
                OutputFormatter.WriteInfo($"  • {recommendation}");
            }
            OutputFormatter.WriteLine();
        }

        // Environment summary (if verbose)
        if (verbose) {
            await DisplayEnvironmentSummaryAsync(result.Environment);
        }
    }

    private Task DisplayIssuesSummaryAsync(List<SystemIssue> issues, bool verbose) {
        var criticalIssues = issues.Where(i => i.Severity == IssueSeverity.Critical).ToList();
        var errorIssues = issues.Where(i => i.Severity == IssueSeverity.Error).ToList();
        var warningIssues = issues.Where(i => i.Severity == IssueSeverity.Warning).ToList();
        var infoIssues = issues.Where(i => i.Severity == IssueSeverity.Info).ToList();

        OutputFormatter.WriteInfo($"Issues Found: {issues.Count} total");

        if (criticalIssues.Any()) {
            OutputFormatter.WriteError($"  🔴 Critical: {criticalIssues.Count}");
        }
        if (errorIssues.Any()) {
            OutputFormatter.WriteError($"  🟠 Errors: {errorIssues.Count}");
        }
        if (warningIssues.Any()) {
            OutputFormatter.WriteWarning($"  🟡 Warnings: {warningIssues.Count}");
        }
        if (infoIssues.Any()) {
            OutputFormatter.WriteInfo($"  🔵 Info: {infoIssues.Count}");
        }

        OutputFormatter.WriteLine();

        if (verbose) {
            // Show detailed issues
            foreach (var severityGroup in issues.GroupBy(i => i.Severity).OrderBy(g => g.Key)) {
                var severityText = GetSeverityText(severityGroup.Key);
                OutputFormatter.WriteInfo($"{severityText} Issues:");

                foreach (var issue in severityGroup.OrderBy(i => i.Category).ThenBy(i => i.Title)) {
                    OutputFormatter.WriteInfo($"  [{issue.Category}] {issue.Title}");
                    if (!string.IsNullOrEmpty(issue.Description)) {
                        OutputFormatter.WriteInfo($"    {issue.Description}");
                    }
                    if (issue.CanAutoFix && !string.IsNullOrEmpty(issue.FixDescription)) {
                        OutputFormatter.WriteInfo($"    Fix: {issue.FixDescription}");
                    }
                }
                OutputFormatter.WriteLine();
            }
        }
        return Task.CompletedTask;
    }

    private Task DisplayIntegrityResultAsync(PackageIntegrityResult result, bool verbose) {
        OutputFormatter.WriteLine();
        OutputFormatter.WriteInfo("Package Integrity Check Results:");
        OutputFormatter.WriteInfo($"  Total packages checked: {result.TotalPackagesChecked}");
        OutputFormatter.WriteInfo($"  Valid packages: {result.ValidPackages}");

        if (result.CorruptedPackages > 0) {
            OutputFormatter.WriteError($"  Corrupted packages: {result.CorruptedPackages}");
        }

        OutputFormatter.WriteInfo($"  Check duration: {result.CheckDuration.TotalSeconds:F1}s");

        if (result.Issues.Any()) {
            OutputFormatter.WriteLine();
            OutputFormatter.WriteWarning("Integrity Issues:");

            foreach (var issue in result.Issues) {
                OutputFormatter.WriteWarning($"  {issue.PackageName}@{issue.Version}: {issue.Description}");
                if (issue.CanRepair && !string.IsNullOrEmpty(issue.RepairAction)) {
                    OutputFormatter.WriteInfo($"    Repair: {issue.RepairAction}");
                }
            }
        }
        return Task.CompletedTask;
    }

    private Task DisplayDependencyResultAsync(DependencyConsistencyResult result, bool verbose) {
        OutputFormatter.WriteLine();
        OutputFormatter.WriteInfo("Dependency Consistency Results:");

        if (result.IsConsistent) {
            OutputFormatter.WriteSuccess("  All dependencies are consistent");
        }
        else {
            OutputFormatter.WriteError($"  Found {result.Conflicts.Count} dependency conflicts");
        }

        if (result.AutoResolved && result.ResolvedConflicts.Any()) {
            OutputFormatter.WriteSuccess($"  Auto-resolved {result.ResolvedConflicts.Count} conflicts:");
            foreach (var resolved in result.ResolvedConflicts) {
                OutputFormatter.WriteSuccess($"    ✓ {resolved}");
            }
        }

        if (result.Conflicts.Any()) {
            OutputFormatter.WriteLine();
            OutputFormatter.WriteWarning("Dependency Conflicts:");

            foreach (var conflict in result.Conflicts) {
                var severityColor = GetConflictSeverityColor(conflict.Severity);
                OutputFormatter.WriteInfo($"  [{severityColor}]{conflict.Severity}[/] {conflict.DependencyName}:");
                OutputFormatter.WriteInfo($"    Required by: {conflict.RequiredByPackage}");
                OutputFormatter.WriteInfo($"    Required version: {conflict.RequiredVersion}");
                OutputFormatter.WriteInfo($"    Actual version: {conflict.ActualVersion}");
                if (!string.IsNullOrEmpty(conflict.Resolution)) {
                    OutputFormatter.WriteInfo($"    Resolution: {conflict.Resolution}");
                }
            }
        }

        foreach (var warning in result.Warnings) {
            OutputFormatter.WriteWarning($"  ⚠️  {warning}");
        }

        foreach (var suggestion in result.Suggestions) {
            OutputFormatter.WriteInfo($"  💡 {suggestion}");
        }

        return Task.CompletedTask;
    }

    private Task DisplayCacheResultAsync(CacheHealthResult result, bool verbose) {
        OutputFormatter.WriteLine();
        OutputFormatter.WriteInfo("Cache Health Results:");

        if (result.IsHealthy) {
            OutputFormatter.WriteSuccess("  Cache is healthy");
        }
        else {
            OutputFormatter.WriteWarning("  Cache has issues");
        }

        var totalSizeMB = result.TotalCacheSize / (1024.0 * 1024.0);
        OutputFormatter.WriteInfo($"  Total cache size: {totalSizeMB:F1} MB");
        OutputFormatter.WriteInfo($"  Total entries: {result.TotalEntries:N0}");

        if (result.CorruptedEntries > 0) {
            var corruptedSizeMB = result.CorruptedCacheSize / (1024.0 * 1024.0);
            OutputFormatter.WriteWarning($"  Corrupted entries: {result.CorruptedEntries} ({corruptedSizeMB:F1} MB)");
        }

        if (result.ClearedEntries > 0) {
            OutputFormatter.WriteSuccess($"  Cleared entries: {result.ClearedEntries}");
        }

        if (result.SpaceFreed > 0) {
            var freedSpaceMB = result.SpaceFreed / (1024.0 * 1024.0);
            OutputFormatter.WriteSuccess($"  Space freed: {freedSpaceMB:F1} MB");
        }

        if (result.WasOptimized) {
            OutputFormatter.WriteSuccess("  Cache was optimized");
        }

        foreach (var issue in result.Issues) {
            OutputFormatter.WriteWarning($"  ❌ {issue}");
        }

        foreach (var optimization in result.Optimizations) {
            OutputFormatter.WriteInfo($"  ⚡ {optimization}");
        }

        return Task.CompletedTask;
    }

    private Task DisplayConfigurationResultAsync(ConfigurationValidationResult result, bool verbose) {
        OutputFormatter.WriteLine();
        OutputFormatter.WriteInfo("Configuration Validation Results:");

        if (result.IsValid) {
            OutputFormatter.WriteSuccess("  Configuration is valid");
        }
        else {
            OutputFormatter.WriteError($"  Configuration has {result.Issues.Count} issues");
        }

        if (result.HasPermissionIssues) {
            OutputFormatter.WriteWarning("  Permission issues detected");
        }

        if (result.HasPathIssues) {
            OutputFormatter.WriteWarning("  Path issues detected");
        }

        if (result.Issues.Any()) {
            OutputFormatter.WriteLine();
            OutputFormatter.WriteWarning("Configuration Issues:");

            foreach (var issue in result.Issues.OrderBy(i => i.Severity)) {
                var severityIcon = GetSeverityIcon(issue.Severity);
                OutputFormatter.WriteInfo($"  {severityIcon} [{issue.ConfigKey}] {issue.Description}");

                if (!string.IsNullOrEmpty(issue.SuggestedValue)) {
                    OutputFormatter.WriteInfo($"    Suggested: {issue.SuggestedValue}");
                }

                if (!string.IsNullOrEmpty(issue.FixCommand)) {
                    OutputFormatter.WriteInfo($"    Fix: {issue.FixCommand}");
                }
            }
        }

        foreach (var recommendation in result.Recommendations) {
            OutputFormatter.WriteInfo($"  💡 {recommendation}");
        }

        foreach (var optimization in result.OptimizationSuggestions) {
            OutputFormatter.WriteInfo($"  ⚡ {optimization}");
        }

        return Task.CompletedTask;
    }

    private Task DisplaySecurityResultAsync(SecurityAuditResult result, bool verbose) {
        OutputFormatter.WriteLine();
        OutputFormatter.WriteInfo("Security Audit Results:");

        var scoreText = GetHealthScoreText(result.SecurityScore);
        var scoreColor = GetHealthScoreColor(result.SecurityScore);
        OutputFormatter.WriteInfo($"  Security Score: [{scoreColor}]{scoreText}[/]");

        OutputFormatter.WriteInfo($"  Packages scanned: {result.TotalPackagesScanned}");

        if (result.VulnerablePackages > 0) {
            OutputFormatter.WriteWarning($"  Vulnerable packages: {result.VulnerablePackages}");
        }

        if (result.CriticalVulnerabilities > 0) {
            OutputFormatter.WriteError($"  🔴 Critical vulnerabilities: {result.CriticalVulnerabilities}");
        }
        if (result.HighVulnerabilities > 0) {
            OutputFormatter.WriteError($"  🟠 High vulnerabilities: {result.HighVulnerabilities}");
        }
        if (result.MediumVulnerabilities > 0) {
            OutputFormatter.WriteWarning($"  🟡 Medium vulnerabilities: {result.MediumVulnerabilities}");
        }
        if (result.LowVulnerabilities > 0) {
            OutputFormatter.WriteInfo($"  🟢 Low vulnerabilities: {result.LowVulnerabilities}");
        }

        if (result.Issues.Any() && verbose) {
            OutputFormatter.WriteLine();
            OutputFormatter.WriteWarning("Security Issues:");

            foreach (var issue in result.Issues.OrderByDescending(i => i.Severity)) {
                var severityIcon = GetSecuritySeverityIcon(issue.Severity);
                OutputFormatter.WriteInfo($"  {severityIcon} {issue.PackageName}@{issue.Version}: {issue.Title}");

                if (!string.IsNullOrEmpty(issue.Description)) {
                    OutputFormatter.WriteInfo($"    {issue.Description}");
                }

                if (!string.IsNullOrEmpty(issue.FixVersion)) {
                    OutputFormatter.WriteInfo($"    Fix available in: {issue.FixVersion}");
                }

                if (!string.IsNullOrEmpty(issue.CvssScore)) {
                    OutputFormatter.WriteInfo($"    CVSS Score: {issue.CvssScore}");
                }
            }
        }

        foreach (var recommendation in result.Recommendations) {
            OutputFormatter.WriteInfo($"  🛡️  {recommendation}");
        }

        return Task.CompletedTask;
    }

    private Task DisplayPerformanceResultAsync(PerformanceAnalysisResult result, bool verbose) {
        OutputFormatter.WriteLine();
        OutputFormatter.WriteInfo("Performance Analysis Results:");

        var scoreText = GetHealthScoreText(result.PerformanceScore);
        var scoreColor = GetHealthScoreColor(result.PerformanceScore);
        OutputFormatter.WriteInfo($"  Performance Score: [{scoreColor}]{scoreText}[/]");

        OutputFormatter.WriteInfo($"  Startup time: {result.StartupTime.TotalMilliseconds:F0}ms");

        // Disk usage
        var packageSizeMB = result.DiskUsage.TotalPackageSize / (1024.0 * 1024.0);
        var cacheSizeMB = result.DiskUsage.CacheSize / (1024.0 * 1024.0);
        var tempSizeMB = result.DiskUsage.TempSize / (1024.0 * 1024.0);

        OutputFormatter.WriteInfo($"  Disk usage:");
        OutputFormatter.WriteInfo($"    Packages: {packageSizeMB:F1} MB ({result.DiskUsage.PackageCount} packages)");
        OutputFormatter.WriteInfo($"    Cache: {cacheSizeMB:F1} MB");
        OutputFormatter.WriteInfo($"    Temp: {tempSizeMB:F1} MB");

        if (!string.IsNullOrEmpty(result.DiskUsage.LargestPackage)) {
            var largestSizeMB = result.DiskUsage.LargestPackageSize / (1024.0 * 1024.0);
            OutputFormatter.WriteInfo($"    Largest package: {result.DiskUsage.LargestPackage} ({largestSizeMB:F1} MB)");
        }

        // Network
        if (result.NetworkLatency.IsReachable) {
            OutputFormatter.WriteInfo($"  Network latency: {result.NetworkLatency.RegistryLatency.TotalMilliseconds:F0}ms");
            OutputFormatter.WriteInfo($"  Connection quality: {result.NetworkLatency.ConnectionQuality}");
        }
        else {
            OutputFormatter.WriteWarning("  Network: Not reachable");
        }

        foreach (var optimization in result.Optimizations) {
            OutputFormatter.WriteInfo($"  ⚡ {optimization}");
        }

        foreach (var warning in result.Warnings) {
            OutputFormatter.WriteWarning($"  ⚠️  {warning}");
        }

        return Task.CompletedTask;
    }

    private Task DisplayEnvironmentInfoAsync(SystemEnvironmentInfo result, bool verbose) {
        OutputFormatter.WriteLine();
        OutputFormatter.WriteInfo("System Environment Information:");
        OutputFormatter.WriteLine();

        var table = new Table();
        table.AddColumn("Property");
        table.AddColumn("Value");

        table.AddRow("Operating System", result.OperatingSystem);
        table.AddRow("Architecture", result.Architecture);
        table.AddRow(".NET Version", result.DotNetVersion);
        table.AddRow("CLI Version", result.CliVersion);
        table.AddRow("Config Path", result.ConfigPath);
        table.AddRow("Cache Path", result.CachePath);
        table.AddRow("Packages Path", result.PackagesPath);
        table.AddRow("Temp Path", result.TempPath);

        AnsiConsole.Write(table);

        if (verbose && result.EnvironmentVariables.Any()) {
            OutputFormatter.WriteLine();
            OutputFormatter.WriteInfo("Relevant Environment Variables:");

            foreach (var envVar in result.EnvironmentVariables.OrderBy(kv => kv.Key)) {
                OutputFormatter.WriteInfo($"  {envVar.Key}={envVar.Value}");
            }
        }

        if (verbose && result.SystemInfo.Any()) {
            OutputFormatter.WriteLine();
            OutputFormatter.WriteInfo("System Information:");

            foreach (var info in result.SystemInfo.OrderBy(kv => kv.Key)) {
                OutputFormatter.WriteInfo($"  {info.Key}: {info.Value}");
            }
        }

        return Task.CompletedTask;
    }

    private Task DisplayEnvironmentSummaryAsync(SystemEnvironmentInfo environment) {
        OutputFormatter.WriteInfo("Environment Summary:");
        OutputFormatter.WriteInfo($"  OS: {environment.OperatingSystem} ({environment.Architecture})");
        OutputFormatter.WriteInfo($"  .NET: {environment.DotNetVersion}");
        OutputFormatter.WriteInfo($"  CLI: {environment.CliVersion}");
        OutputFormatter.WriteLine();

        return Task.CompletedTask;
    }

    private Task OutputHealthCheckAsJsonAsync(SystemHealthCheckResult result) {
        // This would serialize the result to JSON
        // For now, just output a placeholder
        OutputFormatter.WriteInfo("{ \"health_check\": \"json_output_placeholder\" }");

        return Task.CompletedTask;
    }

    private static string GetHealthScoreText(HealthScore score) => score switch {
        HealthScore.Excellent => "Excellent",
        HealthScore.Good => "Good",
        HealthScore.Fair => "Fair",
        HealthScore.Poor => "Poor",
        HealthScore.Critical => "Critical",
        _ => score.ToString(),
    };

    private static string GetHealthScoreColor(HealthScore score) => score switch {
        HealthScore.Excellent => "green",
        HealthScore.Good => "lime",
        HealthScore.Fair => "yellow",
        HealthScore.Poor => "orange",
        HealthScore.Critical => "red",
        _ => "white",
    };

    private static string GetSeverityText(IssueSeverity severity) => severity switch {
        IssueSeverity.Critical => "🔴 Critical",
        IssueSeverity.Error => "🟠 Error",
        IssueSeverity.Warning => "🟡 Warning",
        IssueSeverity.Info => "🔵 Info",
        _ => severity.ToString(),
    };

    private static string GetSeverityIcon(IssueSeverity severity) => severity switch {
        IssueSeverity.Critical => "🔴",
        IssueSeverity.Error => "🟠",
        IssueSeverity.Warning => "🟡",
        IssueSeverity.Info => "🔵",
        _ => "•",
    };

    private static string GetSecuritySeverityIcon(SecuritySeverity severity) => severity switch {
        SecuritySeverity.Critical => "🔴",
        SecuritySeverity.High => "🟠",
        SecuritySeverity.Medium => "🟡",
        SecuritySeverity.Low => "🟢",
        _ => "•",
    };

    private static string GetConflictSeverityColor(ConflictSeverity severity) => severity switch {
        ConflictSeverity.Critical => "red",
        ConflictSeverity.High => "orange",
        ConflictSeverity.Medium => "yellow",
        ConflictSeverity.Low => "green",
        _ => "white",
    };
}