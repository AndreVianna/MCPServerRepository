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
/// Command for displaying detailed information about a specific package
/// </summary>
public class InfoCommand(
    ILogger<InfoCommand> logger,
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
        var command = new Command("info", "Show detailed information about a specific package");

        // Arguments
        var packageNameArgument = new Argument<string>("package-name", "Name of the package (e.g., @anthropic/file-organizer)");
        command.AddArgument(packageNameArgument);

        // Options
        var versionOption = new Option<string?>(
            aliases: ["--version", "-v"],
            description: "Show information for a specific version");
        command.AddOption(versionOption);

        var formatOption = new Option<string?>(
            aliases: ["--format", "-f"],
            description: "Output format (table, json, detailed)");
        command.AddOption(formatOption);

        var securityOption = new Option<bool>(
            aliases: ["--security", "-s"],
            description: "Show detailed security information");
        command.AddOption(securityOption);

        var trustOption = new Option<bool>(
            aliases: ["--trust", "-t"],
            description: "Show trust tier assessment");
        command.AddOption(trustOption);

        var versionsOption = new Option<bool>(
            aliases: ["--versions"],
            description: "Show version history");
        command.AddOption(versionsOption);

        // Handler
        command.SetHandler(
            ExecuteAsync,
            packageNameArgument,
            versionOption,
            formatOption,
            securityOption,
            trustOption,
            versionsOption);

        return command;
    }

    private async Task<int> ExecuteAsync(
        string packageName,
        string? version,
        string? format,
        bool showSecurity,
        bool showTrust,
        bool showVersions) {
        try {
            Logger.LogInformation("Executing info command: PackageName={PackageName}, Version={Version}",
                packageName, version);

            // Validate input
            if (string.IsNullOrWhiteSpace(packageName)) {
                OutputFormatter.WriteError("Package name is required");
                return 400;
            }

            var outputFormat = ValidateOutputFormat(format);

            // Check API connectivity
            if (!await ValidateApiConnectivityAsync()) {
                return 503; // Service unavailable
            }

            // Get basic package information
            var packageInfo = await WithProgressAsync(
                $"Getting information for '{packageName}'...",
                () => ApiClient.GetPackageInfoAsync(packageName));

            // Display basic package info
            OutputFormatter.WritePackageInfo(packageInfo, outputFormat);

            // Show additional information if requested
            if (showSecurity || showTrust || showVersions) {
                OutputFormatter.WriteLine();

                if (showSecurity) {
                    try {
                        var securitySummary = await WithProgressAsync(
                            "Getting comprehensive security information...",
                            () => _securityService.GetSecuritySummaryAsync(packageName));

                        await DisplaySecurityInformationAsync(packageName, version, securitySummary, outputFormat);
                        OutputFormatter.WriteLine();
                    }
                    catch (Exception ex) {
                        Logger.LogWarning(ex, "Failed to get security information for package: {PackageName}", packageName);
                        OutputFormatter.WriteWarning("Security information not available");
                    }
                }

                if (showTrust) {
                    try {
                        var trustTierAssessment = await WithProgressAsync(
                            "Getting comprehensive trust tier assessment...",
                            () => _trustTierService.GetTrustTierAssessmentAsync(packageName));

                        await DisplayTrustTierInformationAsync(packageName, trustTierAssessment, outputFormat);
                        OutputFormatter.WriteLine();
                    }
                    catch (Exception ex) {
                        Logger.LogWarning(ex, "Failed to get trust tier information for package: {PackageName}", packageName);
                        OutputFormatter.WriteWarning("Trust tier information not available");
                    }
                }

                if (showVersions) {
                    try {
                        var versions = await WithProgressAsync(
                            "Getting version history...",
                            () => ApiClient.GetPackageVersionsAsync(packageName, includePrerelease: true));

                        if (versions.Versions.Any()) {
                            OutputFormatter.WritePackageVersions(versions, outputFormat);
                        }
                        else {
                            OutputFormatter.WriteInfo("No version history available");
                        }
                    }
                    catch (Exception ex) {
                        Logger.LogWarning(ex, "Failed to get version history for package: {PackageName}", packageName);
                        OutputFormatter.WriteWarning("Version history not available");
                    }
                }
            }

            // Show usage information
            if (outputFormat != "json") {
                OutputFormatter.WriteLine();
                OutputFormatter.WriteInfo($"Use 'mcpm install {packageName}' to install this package");

                if (showSecurity || showTrust) {
                    OutputFormatter.WriteInfo($"Use 'mcpm verify {packageName}' to run security verification");
                }
            }

            Logger.LogInformation("Info command completed successfully for package: {PackageName}", packageName);
            return 0;
        }
        catch (PackageNotFoundException) {
            OutputFormatter.WriteError($"Package '{packageName}' not found");
            OutputFormatter.WriteInfo("Use 'mcpm search' to find available packages");
            return 404;
        }
        catch (Exception ex) {
            return HandleError(ex, "info");
        }
    }

    /// <summary>
    /// Displays comprehensive security information for a package
    /// </summary>
    private async Task DisplaySecurityInformationAsync(
        string packageName,
        string? version,
        SecurityScanSummary securitySummary,
        string outputFormat) {
        
        OutputFormatter.WriteInfo("=== Security Information ===");
        OutputFormatter.WriteLine();

        // Get security grade
        try {
            var securityGrade = await _securityService.CalculateSecurityGradeAsync(
                packageName, version ?? "latest");
            OutputFormatter.WriteInfo($"Security Grade: {GetSecurityGradeDisplay(securityGrade)}");
        }
        catch (Exception ex) {
            Logger.LogWarning(ex, "Failed to calculate security grade");
            OutputFormatter.WriteWarning("Security grade calculation unavailable");
        }

        // Display security scan summary
        if (outputFormat == "json") {
            // Implementation would output JSON format
            OutputFormatter.WriteInfo("JSON security summary not implemented in skeleton");
        }
        else {
            DisplaySecuritySummaryTable(securitySummary);
        }

        // Get security advisories
        try {
            var advisories = await _securityService.GetSecurityAdvisoriesAsync(packageName, version);
            if (advisories.Any()) {
                OutputFormatter.WriteLine();
                OutputFormatter.WriteWarning($"Found {advisories.Count()} security advisories:");
                
                foreach (var advisory in advisories.Take(5)) {
                    var severityColor = advisory.Severity switch {
                        SecurityScanSeverity.Critical => "red",
                        SecurityScanSeverity.High => "orange3",
                        SecurityScanSeverity.Medium => "yellow",
                        _ => "green"
                    };
                    
                    OutputFormatter.WriteWarning($"  [{severityColor}]{advisory.Severity}[/] {advisory.Title}");
                }

                if (advisories.Count() > 5) {
                    OutputFormatter.WriteInfo($"  ... and {advisories.Count() - 5} more. Use 'mcpm security advisories {packageName}' for full list");
                }
            }
        }
        catch (Exception ex) {
            Logger.LogWarning(ex, "Failed to get security advisories");
        }
    }

    /// <summary>
    /// Displays comprehensive trust tier information for a package
    /// </summary>
    private async Task DisplayTrustTierInformationAsync(
        string packageName,
        TrustTierAssessment assessment,
        string outputFormat) {
        
        OutputFormatter.WriteInfo("=== Trust Tier Assessment ===");
        OutputFormatter.WriteLine();

        if (outputFormat == "json") {
            // Implementation would output JSON format
            OutputFormatter.WriteInfo("JSON trust tier assessment not implemented in skeleton");
        }
        else {
            DisplayTrustTierAssessmentTable(assessment);
        }

        // Get trust tier history
        try {
            var history = await _trustTierService.GetTrustTierHistoryAsync(packageName, 5);
            if (history.Any()) {
                OutputFormatter.WriteLine();
                OutputFormatter.WriteInfo("Recent Trust Tier History:");
                
                foreach (var entry in history) {
                    var changeDate = entry.ChangedAt.ToString("yyyy-MM-dd");
                    var changeType = entry.ToTier > entry.FromTier ? "↗️ Promoted" : "↘️ Demoted";
                    OutputFormatter.WriteInfo($"  {changeDate}: {changeType} to {entry.ToTier}");
                    
                    if (!string.IsNullOrEmpty(entry.Reason)) {
                        OutputFormatter.WriteInfo($"    Reason: {entry.Reason}");
                    }
                }
            }
        }
        catch (Exception ex) {
            Logger.LogWarning(ex, "Failed to get trust tier history");
        }

        // Get trust score breakdown for educational purposes
        try {
            var scoreBreakdown = await _trustTierService.GetTrustScoreBreakdownAsync(packageName);
            OutputFormatter.WriteLine();
            OutputFormatter.WriteInfo("Trust Score Breakdown:");
            OutputFormatter.WriteInfo($"  Overall Score: {scoreBreakdown.TotalScore}/{scoreBreakdown.MaxScore} ({scoreBreakdown.ScorePercentage:F1}%)");
            
            if (scoreBreakdown.Factors.Any()) {
                OutputFormatter.WriteInfo("  Key Factors:");
                foreach (var factor in scoreBreakdown.Factors.Take(3)) {
                    var percentage = factor.Value.MaxScore > 0 ? (decimal)factor.Value.Score / factor.Value.MaxScore * 100 : 0;
                    OutputFormatter.WriteInfo($"    {factor.Key}: {factor.Value.Score}/{factor.Value.MaxScore} ({percentage:F1}%)");
                }
            }

            if (scoreBreakdown.ImprovementSuggestions.Any()) {
                OutputFormatter.WriteLine();
                OutputFormatter.WriteInfo("Improvement Suggestions:");
                foreach (var suggestion in scoreBreakdown.ImprovementSuggestions.Take(3)) {
                    OutputFormatter.WriteInfo($"  • {suggestion}");
                }
            }
        }
        catch (Exception ex) {
            Logger.LogWarning(ex, "Failed to get trust score breakdown");
        }
    }

    private void DisplaySecuritySummaryTable(SecurityScanSummary summary) {
        // Implementation would display actual security summary
        OutputFormatter.WriteInfo("Security summary display not implemented in skeleton");
    }

    private void DisplayTrustTierAssessmentTable(TrustTierAssessment assessment) {
        var table = new Table();
        table.AddColumn("Metric");
        table.AddColumn("Value");
        table.AddColumn("Status");

        var currentTierColor = assessment.CurrentTier switch {
            TrustTier.Enterprise => "green",
            TrustTier.Professional => "blue", 
            TrustTier.Community => "yellow",
            _ => "red"
        };

        var recommendedTierColor = assessment.RecommendedTier switch {
            TrustTier.Enterprise => "green",
            TrustTier.Professional => "blue",
            TrustTier.Community => "yellow", 
            _ => "red"
        };

        table.AddRow("Current Tier", $"[{currentTierColor}]{assessment.CurrentTier}[/]", 
            assessment.CurrentTier == assessment.RecommendedTier ? "[green]✓[/]" : "[yellow]⚠[/]");
        
        table.AddRow("Recommended Tier", $"[{recommendedTierColor}]{assessment.RecommendedTier}[/]",
            assessment.EligibleForPromotion ? "[green]Eligible[/]" : "[grey]Not Eligible[/]");
        
        table.AddRow("Trust Score", $"{assessment.TotalScore}/{assessment.MaxScore}", 
            $"{assessment.ScorePercentage:F1}%");
        
        table.AddRow("Last Assessment", assessment.LastAssessment.ToString("yyyy-MM-dd"), 
            assessment.AtRiskForDemotion ? "[red]At Risk[/]" : "[green]Stable[/]");

        AnsiConsole.Write(table);

        if (assessment.PositiveFactors.Any()) {
            OutputFormatter.WriteLine();
            OutputFormatter.WriteInfo("Positive Factors:");
            foreach (var factor in assessment.PositiveFactors.Take(3)) {
                OutputFormatter.WriteInfo($"  ✅ {factor}");
            }
        }

        if (assessment.NegativeFactors.Any()) {
            OutputFormatter.WriteLine();
            OutputFormatter.WriteWarning("Areas for Improvement:");
            foreach (var factor in assessment.NegativeFactors.Take(3)) {
                OutputFormatter.WriteWarning($"  ⚠️ {factor}");
            }
        }
    }

    private string GetSecurityGradeDisplay(string grade) {
        return grade switch {
            "A+" or "A" => $"[green]{grade}[/] (Excellent)",
            "B" => $"[yellow]{grade}[/] (Good)",
            "C" => $"[orange3]{grade}[/] (Acceptable)",
            "D" or "F" => $"[red]{grade}[/] (Poor)",
            _ => $"{grade} (Unknown)"
        };
    }
}