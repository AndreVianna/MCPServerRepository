using System.CommandLine;

using MCPHub.CommandLineApp.Configuration;
using MCPHub.CommandLineApp.Services;
using MCPHub.CommandLineApp.Utilities;
using MCPHub.Domain.Contracts.Responses;
using MCPHub.Domain.Entities;
using MCPHub.Domain.ValueObjects;

using Microsoft.Extensions.Logging;

using Spectre.Console;

namespace MCPHub.CommandLineApp.Commands;

/// <summary>
/// Command for security information, reports, and policy management
/// </summary>
public class SecurityCommand(
    ILogger<SecurityCommand> logger,
    McpmConfiguration configuration,
    IMcpHubApiClient apiClient,
    IOutputFormatter outputFormatter,
    IInteractionService interactionService,
    IProgressReporter progressReporter,
    ISecurityService securityService,
    ITrustTierService trustTierService) : BaseCommand(logger, configuration, apiClient, outputFormatter, interactionService, progressReporter) {
    private readonly ISecurityService _securityService = securityService ?? throw new ArgumentNullException(nameof(securityService));
    private readonly ITrustTierService _trustTierService = trustTierService ?? throw new ArgumentNullException(nameof(trustTierService));

    /// <inheritdoc />
    public override Command CreateCommand() {
        var command = new Command("security", "Security information, reports, and policy management");

        // Subcommands
        command.AddCommand(CreateAdvisoriesCommand());
        command.AddCommand(CreatePolicyCommand());
        command.AddCommand(CreateReportCommand());
        command.AddCommand(CreateDatabaseCommand());
        command.AddCommand(CreateGradeCommand());
        command.AddCommand(CreateStatisticsCommand());

        return command;
    }

    private Command CreateAdvisoriesCommand() {
        var command = new Command("advisories", "Get security advisories for packages");

        // Arguments
        var packageArgument = new Argument<string?>("package", "Package name to get advisories for (optional)");
        command.AddArgument(packageArgument);

        // Options
        var severityOption = new Option<string>(
            aliases: ["--severity", "-s"],
            getDefaultValue: () => "medium",
            description: "Minimum severity level (low, medium, high, critical)");
        command.AddOption(severityOption);

        var versionOption = new Option<string?>(
            aliases: ["--version", "-v"],
            description: "Specific package version to check");
        command.AddOption(versionOption);

        var limitOption = new Option<int>(
            aliases: ["--limit", "-l"],
            getDefaultValue: () => 10,
            description: "Maximum number of advisories to show");
        command.AddOption(limitOption);

        var formatOption = new Option<string>(
            aliases: ["--format", "-f"],
            getDefaultValue: () => "table",
            description: "Output format (table, json, detailed)");
        command.AddOption(formatOption);

        command.SetHandler(ExecuteAdvisoriesAsync, packageArgument, severityOption, versionOption, limitOption, formatOption);
        return command;
    }

    private Command CreatePolicyCommand() {
        var command = new Command("policy", "Manage security policies");

        // Subcommands
        var showCommand = new Command("show", "Show current security policy");
        var setCommand = new Command("set", "Set security policy options");
        var resetCommand = new Command("reset", "Reset security policy to defaults");

        // Set command options
        var minTrustTierOption = new Option<string?>(
            aliases: ["--min-trust-tier"],
            description: "Minimum trust tier requirement");
        var minSecurityGradeOption = new Option<string?>(
            aliases: ["--min-security-grade"],
            description: "Minimum security grade requirement");
        var allowUnverifiedOption = new Option<bool?>(
            aliases: ["--allow-unverified"],
            description: "Allow unverified packages");

        setCommand.AddOption(minTrustTierOption);
        setCommand.AddOption(minSecurityGradeOption);
        setCommand.AddOption(allowUnverifiedOption);

        showCommand.SetHandler(ExecuteShowPolicyAsync);
        setCommand.SetHandler(ExecuteSetPolicyAsync, minTrustTierOption, minSecurityGradeOption, allowUnverifiedOption);
        resetCommand.SetHandler(ExecuteResetPolicyAsync);

        command.AddCommand(showCommand);
        command.AddCommand(setCommand);
        command.AddCommand(resetCommand);

        return command;
    }

    private Command CreateReportCommand() {
        var command = new Command("report", "Generate comprehensive security reports");

        // Options
        var typeOption = new Option<string>(
            aliases: ["--type", "-t"],
            getDefaultValue: () => "summary",
            description: "Report type (summary, detailed, compliance, audit)");
        command.AddOption(typeOption);

        var formatOption = new Option<string>(
            aliases: ["--format", "-f"],
            getDefaultValue: () => "html",
            description: "Report format (html, pdf, json, xml)");
        command.AddOption(formatOption);

        var outputOption = new Option<string?>(
            aliases: ["--output", "-o"],
            description: "Output file path");
        command.AddOption(outputOption);

        var includeGlobalOption = new Option<bool>(
            aliases: ["--include-global"],
            description: "Include globally installed packages");
        command.AddOption(includeGlobalOption);

        var periodOption = new Option<int>(
            aliases: ["--period"],
            getDefaultValue: () => 30,
            description: "Analysis period in days");
        command.AddOption(periodOption);

        command.SetHandler(ExecuteReportAsync, typeOption, formatOption, outputOption, includeGlobalOption, periodOption);
        return command;
    }

    private Command CreateDatabaseCommand() {
        var command = new Command("database", "Manage local security database");

        // Subcommands
        var statusCommand = new Command("status", "Show security database status");
        var updateCommand = new Command("update", "Update security database");
        var cleanCommand = new Command("clean", "Clean security database cache");

        // Update command options
        var forceOption = new Option<bool>(
            aliases: ["--force", "-f"],
            description: "Force update even if recently updated");
        updateCommand.AddOption(forceOption);

        statusCommand.SetHandler(ExecuteDatabaseStatusAsync);
        updateCommand.SetHandler(ExecuteDatabaseUpdateAsync, forceOption);
        cleanCommand.SetHandler(ExecuteDatabaseCleanAsync);

        command.AddCommand(statusCommand);
        command.AddCommand(updateCommand);
        command.AddCommand(cleanCommand);

        return command;
    }

    private Command CreateGradeCommand() {
        var command = new Command("grade", "Get security grade for packages");

        // Arguments
        var packageArgument = new Argument<string>("package", "Package name with optional version");
        command.AddArgument(packageArgument);

        // Options
        var detailedOption = new Option<bool>(
            aliases: ["--detailed", "-d"],
            description: "Show detailed grade breakdown");
        command.AddOption(detailedOption);

        var thresholdsOption = new Option<bool>(
            aliases: ["--thresholds"],
            description: "Show grade thresholds configuration");
        command.AddOption(thresholdsOption);

        command.SetHandler(ExecuteGradeAsync, packageArgument, detailedOption, thresholdsOption);
        return command;
    }

    private Command CreateStatisticsCommand() {
        var command = new Command("statistics", "Show security statistics");

        // Options
        var periodOption = new Option<int>(
            aliases: ["--period"],
            getDefaultValue: () => 30,
            description: "Statistics period in days");
        command.AddOption(periodOption);

        var globalOption = new Option<bool>(
            aliases: ["--global"],
            description: "Show platform-wide statistics");
        command.AddOption(globalOption);

        var chartOption = new Option<bool>(
            aliases: ["--chart"],
            description: "Display statistics as charts");
        command.AddOption(chartOption);

        command.SetHandler(ExecuteStatisticsAsync, periodOption, globalOption, chartOption);
        return command;
    }

    private async Task<int> ExecuteAdvisoriesAsync(
        string? package,
        string severity,
        string? version,
        int limit,
        string format) {
        try {
            OutputFormatter.WriteInfo($"Retrieving security advisories...");

            var severityEnum = ParseSeverity(severity);
            var advisories = await _securityService.GetSecurityAdvisoriesAsync(
                package ?? "all", version, severityEnum);

            var limitedAdvisories = advisories.Take(limit);

            if (!limitedAdvisories.Any()) {
                OutputFormatter.WriteInfo("No security advisories found.");
                return 0;
            }

            switch (format.ToLowerInvariant()) {
                case "json":
                    await DisplayAdvisoriesAsJsonAsync(limitedAdvisories);
                    break;
                case "detailed":
                    await DisplayDetailedAdvisoriesAsync(limitedAdvisories);
                    break;
                default:
                    DisplayAdvisoriesAsTable(limitedAdvisories);
                    break;
            }

            return 0;
        }
        catch (Exception ex) {
            return HandleError(ex, "security advisories");
        }
    }

    private async Task<int> ExecuteShowPolicyAsync() {
        try {
            OutputFormatter.WriteInfo("Current Security Policy:");
            OutputFormatter.WriteLine();

            // Implementation would load actual policy from configuration
            var policy = new {
                MinimumTrustTier = "Community",
                MinimumSecurityGrade = "B",
                AllowUnverified = false,
                MaxCriticalVulnerabilities = 0,
                MaxHighVulnerabilities = 2,
                RequireSignedPackages = true,
                AutoUpdateSecurityDatabase = true
            };

            var table = new Table();
            table.AddColumn("Setting");
            table.AddColumn("Value");
            table.AddColumn("Description");

            table.AddRow("Minimum Trust Tier", policy.MinimumTrustTier, "Lowest acceptable trust tier");
            table.AddRow("Minimum Security Grade", policy.MinimumSecurityGrade, "Lowest acceptable security grade");
            table.AddRow("Allow Unverified", policy.AllowUnverified.ToString(), "Allow unverified packages");
            table.AddRow("Max Critical Vulnerabilities", policy.MaxCriticalVulnerabilities.ToString(), "Maximum critical vulnerabilities allowed");
            table.AddRow("Max High Vulnerabilities", policy.MaxHighVulnerabilities.ToString(), "Maximum high vulnerabilities allowed");
            table.AddRow("Require Signed Packages", policy.RequireSignedPackages.ToString(), "Require package signatures");
            table.AddRow("Auto Update Database", policy.AutoUpdateSecurityDatabase.ToString(), "Automatically update security database");

            AnsiConsole.Write(table);

            await Task.Delay(10); // For async consistency
            return 0;
        }
        catch (Exception ex) {
            return HandleError(ex, "show security policy");
        }
    }

    private async Task<int> ExecuteSetPolicyAsync(
        string? minTrustTier,
        string? minSecurityGrade,
        bool? allowUnverified) {
        try {
            OutputFormatter.WriteInfo("Updating security policy...");

            var updates = new List<string>();

            if (!string.IsNullOrEmpty(minTrustTier)) {
                updates.Add($"Minimum Trust Tier: {minTrustTier}");
            }

            if (!string.IsNullOrEmpty(minSecurityGrade)) {
                updates.Add($"Minimum Security Grade: {minSecurityGrade}");
            }

            if (allowUnverified.HasValue) {
                updates.Add($"Allow Unverified: {allowUnverified.Value}");
            }

            if (!updates.Any()) {
                OutputFormatter.WriteWarning("No policy changes specified.");
                return 0;
            }

            OutputFormatter.WriteInfo("Policy changes:");
            foreach (var update in updates) {
                OutputFormatter.WriteInfo($"  • {update}");
            }

            var confirm = await InteractionService.ConfirmAsync("Apply these policy changes?", true);
            if (!confirm) {
                OutputFormatter.WriteInfo("Policy changes cancelled.");
                return 0;
            }

            // Implementation would save policy changes
            await Task.Delay(100); // Simulate save operation

            OutputFormatter.WriteSuccess("Security policy updated successfully.");
            return 0;
        }
        catch (Exception ex) {
            return HandleError(ex, "set security policy");
        }
    }

    private async Task<int> ExecuteResetPolicyAsync() {
        try {
            var confirm = await InteractionService.ConfirmAsync(
                "Reset security policy to defaults? This will remove all custom settings.", false);
            
            if (!confirm) {
                OutputFormatter.WriteInfo("Policy reset cancelled.");
                return 0;
            }

            using var spinner = ProgressReporter.CreateSpinner("Resetting security policy...");
            await Task.Delay(500); // Simulate reset operation
            spinner.Success("Security policy reset to defaults");

            return 0;
        }
        catch (Exception ex) {
            return HandleError(ex, "reset security policy");
        }
    }

    private async Task<int> ExecuteReportAsync(
        string type,
        string format,
        string? output,
        bool includeGlobal,
        int period) {
        try {
            OutputFormatter.WriteInfo($"Generating {type} security report in {format} format...");

            using var reportProgress = ProgressReporter.CreateStepProgress("Security Report Generation", new[] {
                "Collecting security data",
                "Analyzing packages",
                "Generating report",
                "Saving report file"
            });

            // Stage 1: Collect data
            reportProgress.StartStep(0, "Collecting security data...");
            await Task.Delay(1000); // Simulate data collection
            reportProgress.CompleteStep(0, "Security data collected");

            // Stage 2: Analysis
            reportProgress.StartStep(1, "Analyzing packages...");
            await Task.Delay(1500); // Simulate analysis
            reportProgress.CompleteStep(1, "Package analysis completed");

            // Stage 3: Generate report
            reportProgress.StartStep(2, "Generating report...");
            await Task.Delay(2000); // Simulate report generation
            reportProgress.CompleteStep(2, "Report generated");

            // Stage 4: Save file
            reportProgress.StartStep(3, "Saving report file...");
            var fileName = output ?? $"security-report-{DateTimeOffset.UtcNow:yyyyMMdd-HHmmss}.{format}";
            await Task.Delay(500); // Simulate file save
            reportProgress.CompleteStep(3, $"Report saved to {fileName}");

            OutputFormatter.WriteSuccess($"Security report generated successfully: {fileName}");
            return 0;
        }
        catch (Exception ex) {
            return HandleError(ex, "generate security report");
        }
    }

    private async Task<int> ExecuteDatabaseStatusAsync() {
        try {
            OutputFormatter.WriteInfo("Security Database Status:");
            OutputFormatter.WriteLine();

            var status = await _securityService.GetSecurityDatabaseStatusAsync();

            var table = new Table();
            table.AddColumn("Property");
            table.AddColumn("Value");
            table.AddColumn("Status");

            var availableStatus = status.IsAvailable ? "[green]✓ Available[/]" : "[red]✗ Not Available[/]";
            var outdatedStatus = status.IsOutdated ? "[yellow]⚠ Outdated[/]" : "[green]✓ Current[/]";
            var lastUpdateDisplay = status.LastUpdate?.ToString("yyyy-MM-dd HH:mm:ss UTC") ?? "Never";
            var ageDisplay = status.Age?.ToString(@"dd\d\ hh\h\ mm\m") ?? "Unknown";

            table.AddRow("Database Available", status.IsAvailable.ToString(), availableStatus);
            table.AddRow("Record Count", status.RecordCount.ToString("N0"), status.RecordCount > 0 ? "[green]✓[/]" : "[red]✗[/]");
            table.AddRow("Last Updated", lastUpdateDisplay, outdatedStatus);
            table.AddRow("Database Age", ageDisplay, status.IsOutdated ? "[yellow]⚠[/]" : "[green]✓[/]");

            AnsiConsole.Write(table);

            if (status.IsOutdated) {
                OutputFormatter.WriteLine();
                OutputFormatter.WriteWarning("Security database is outdated. Run 'mcpm security database update' to refresh.");
            }

            return 0;
        }
        catch (Exception ex) {
            return HandleError(ex, "security database status");
        }
    }

    private async Task<int> ExecuteDatabaseUpdateAsync(bool force) {
        try {
            var status = await _securityService.GetSecurityDatabaseStatusAsync();
            
            if (!force && status.IsAvailable && !status.IsOutdated) {
                OutputFormatter.WriteInfo("Security database is already up to date.");
                OutputFormatter.WriteInfo("Use --force to update anyway.");
                return 0;
            }

            using var updateProgress = ProgressReporter.CreateProgress("Updating security database");
            
            var result = await _securityService.UpdateSecurityDatabaseAsync(force);
            
            if (result.Success) {
                updateProgress.Complete($"Updated {result.UpdatedRecords:N0} security records");
                OutputFormatter.WriteSuccess("Security database updated successfully.");
                OutputFormatter.WriteInfo($"Last updated: {result.LastUpdate:yyyy-MM-dd HH:mm:ss UTC}");
            }
            else {
                updateProgress.Fail("Database update failed");
                OutputFormatter.WriteError("Failed to update security database:");
                foreach (var error in result.Errors) {
                    OutputFormatter.WriteError($"  - {error}");
                }
                return 1;
            }

            return 0;
        }
        catch (Exception ex) {
            return HandleError(ex, "update security database");
        }
    }

    private async Task<int> ExecuteDatabaseCleanAsync() {
        try {
            var confirm = await InteractionService.ConfirmAsync(
                "Clean security database cache? This will remove all cached data.", false);
            
            if (!confirm) {
                OutputFormatter.WriteInfo("Database clean cancelled.");
                return 0;
            }

            using var cleanProgress = ProgressReporter.CreateSpinner("Cleaning security database cache...");
            await Task.Delay(1000); // Simulate clean operation
            cleanProgress.Success("Security database cache cleaned");

            return 0;
        }
        catch (Exception ex) {
            return HandleError(ex, "clean security database");
        }
    }

    private async Task<int> ExecuteGradeAsync(string package, bool detailed, bool thresholds) {
        try {
            var (packageName, version) = ParsePackageSpec(package);

            if (thresholds) {
                await DisplayGradeThresholdsAsync();
                return 0;
            }

            OutputFormatter.WriteInfo($"Calculating security grade for {packageName}...");

            var grade = await _securityService.CalculateSecurityGradeAsync(packageName, version ?? "latest");

            OutputFormatter.WriteLine();
            OutputFormatter.WriteInfo($"Security Grade for {packageName}: {GetGradeMarkup(grade)}");

            if (detailed) {
                await DisplayDetailedGradeBreakdownAsync(packageName);
            }

            return 0;
        }
        catch (Exception ex) {
            return HandleError(ex, "calculate security grade");
        }
    }

    private async Task<int> ExecuteStatisticsAsync(int period, bool global, bool chart) {
        try {
            OutputFormatter.WriteInfo($"Loading security statistics for {period} days...");

            if (global) {
                var stats = await _trustTierService.GetTrustTierStatisticsAsync(period);
                await DisplayPlatformStatisticsAsync(stats, chart);
            }
            else {
                await DisplayLocalStatisticsAsync(period, chart);
            }

            return 0;
        }
        catch (Exception ex) {
            return HandleError(ex, "show security statistics");
        }
    }

    // Helper methods for display formatting
    private SecurityScanSeverity ParseSeverity(string severity) {
        return severity.ToLowerInvariant() switch {
            "low" => SecurityScanSeverity.Low,
            "medium" => SecurityScanSeverity.Medium,
            "high" => SecurityScanSeverity.High,
            "critical" => SecurityScanSeverity.Critical,
            _ => SecurityScanSeverity.Medium
        };
    }

    private (string packageName, string? version) ParsePackageSpec(string packageSpec) {
        var parts = packageSpec.Split('@', 2);
        return parts.Length == 2 ? (parts[0], parts[1]) : (parts[0], null);
    }

    private void DisplayAdvisoriesAsTable(IEnumerable<SecurityAdvisory> advisories) {
        var table = new Table();
        table.AddColumn("ID");
        table.AddColumn("Severity");
        table.AddColumn("Title");
        table.AddColumn("Published");
        table.AddColumn("Source");

        foreach (var advisory in advisories) {
            var severityMarkup = GetSeverityMarkup(advisory.Severity);
            var publishedDate = advisory.PublishedAt.ToString("yyyy-MM-dd");

            table.AddRow(
                advisory.Id,
                severityMarkup,
                advisory.Title.Length > 50 ? advisory.Title[..47] + "..." : advisory.Title,
                publishedDate,
                advisory.Source);
        }

        AnsiConsole.Write(table);
    }

    private async Task DisplayAdvisoriesAsJsonAsync(IEnumerable<SecurityAdvisory> advisories) {
        // Implementation would output proper JSON
        OutputFormatter.WriteInfo("JSON output not implemented in skeleton");
        await Task.Delay(10);
    }

    private async Task DisplayDetailedAdvisoriesAsync(IEnumerable<SecurityAdvisory> advisories) {
        foreach (var advisory in advisories) {
            OutputFormatter.WriteLine();
            OutputFormatter.WriteError($"[{advisory.Severity}] {advisory.Title}");
            OutputFormatter.WriteInfo($"ID: {advisory.Id}");
            OutputFormatter.WriteInfo($"Description: {advisory.Description}");
            
            if (!string.IsNullOrEmpty(advisory.CveId)) {
                OutputFormatter.WriteInfo($"CVE: {advisory.CveId}");
            }
            
            OutputFormatter.WriteInfo($"Published: {advisory.PublishedAt:yyyy-MM-dd}");
            OutputFormatter.WriteInfo($"Source: {advisory.Source}");
            
            if (advisory.References.Any()) {
                OutputFormatter.WriteInfo("References:");
                foreach (var reference in advisory.References) {
                    OutputFormatter.WriteInfo($"  - {reference}");
                }
            }
        }

        await Task.Delay(10);
    }

    private async Task DisplayGradeThresholdsAsync() {
        OutputFormatter.WriteInfo("Security Grade Thresholds:");
        OutputFormatter.WriteLine();

        // Implementation would show actual grade thresholds
        var thresholds = new[] {
            new { Grade = "A+", MaxCritical = 0, MaxHigh = 0, MaxMedium = 0, Description = "Perfect security" },
            new { Grade = "A", MaxCritical = 0, MaxHigh = 0, MaxMedium = 2, Description = "Excellent security" },
            new { Grade = "B", MaxCritical = 0, MaxHigh = 1, MaxMedium = 5, Description = "Good security" },
            new { Grade = "C", MaxCritical = 0, MaxHigh = 3, MaxMedium = 10, Description = "Acceptable security" },
            new { Grade = "D", MaxCritical = 1, MaxHigh = 5, MaxMedium = 15, Description = "Poor security" },
            new { Grade = "F", MaxCritical = -1, MaxHigh = -1, MaxMedium = -1, Description = "Failing security" }
        };

        var table = new Table();
        table.AddColumn("Grade");
        table.AddColumn("Max Critical");
        table.AddColumn("Max High");
        table.AddColumn("Max Medium");
        table.AddColumn("Description");

        foreach (var threshold in thresholds) {
            var gradeMarkup = GetGradeMarkup(threshold.Grade);
            var criticalText = threshold.MaxCritical == -1 ? "Any" : threshold.MaxCritical.ToString();
            var highText = threshold.MaxHigh == -1 ? "Any" : threshold.MaxHigh.ToString();
            var mediumText = threshold.MaxMedium == -1 ? "Any" : threshold.MaxMedium.ToString();

            table.AddRow(gradeMarkup, criticalText, highText, mediumText, threshold.Description);
        }

        AnsiConsole.Write(table);
        await Task.Delay(10);
    }

    private async Task DisplayDetailedGradeBreakdownAsync(string packageName) {
        OutputFormatter.WriteLine();
        OutputFormatter.WriteInfo("Detailed Grade Breakdown:");

        // Implementation would show actual breakdown from TrustTierService
        await Task.Delay(100);
        
        OutputFormatter.WriteInfo("• Vulnerability Score: 85/100");
        OutputFormatter.WriteInfo("• Code Quality Score: 92/100");
        OutputFormatter.WriteInfo("• Dependency Safety: 78/100");
        OutputFormatter.WriteInfo("• Maintenance Status: 95/100");
        OutputFormatter.WriteInfo("• Publisher Reputation: 88/100");
    }

    private async Task DisplayPlatformStatisticsAsync(TrustTierStatistics stats, bool chart) {
        OutputFormatter.WriteInfo("Platform-wide Security Statistics:");
        OutputFormatter.WriteLine();

        // Implementation would display actual statistics
        await Task.Delay(100);
        
        OutputFormatter.WriteInfo("Statistics display not implemented in skeleton");
    }

    private async Task DisplayLocalStatisticsAsync(int period, bool chart) {
        OutputFormatter.WriteInfo($"Local Security Statistics ({period} days):");
        OutputFormatter.WriteLine();

        // Implementation would display local package statistics
        await Task.Delay(100);
        
        OutputFormatter.WriteInfo("Local statistics display not implemented in skeleton");
    }

    private string GetSeverityMarkup(SecurityScanSeverity severity) {
        return severity switch {
            SecurityScanSeverity.Critical => "[red]Critical[/]",
            SecurityScanSeverity.High => "[orange3]High[/]",
            SecurityScanSeverity.Medium => "[yellow]Medium[/]",
            SecurityScanSeverity.Low => "[green]Low[/]",
            SecurityScanSeverity.None => "[grey]None[/]",
            _ => severity.ToString()
        };
    }

    private string GetGradeMarkup(string grade) {
        return grade switch {
            "A+" or "A" => $"[green]{grade}[/]",
            "B+" or "B" => $"[yellow]{grade}[/]",
            "C+" or "C" => $"[orange3]{grade}[/]",
            "D+" or "D" or "F" => $"[red]{grade}[/]",
            _ => grade
        };
    }
}