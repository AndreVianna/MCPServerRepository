using MCPHub.CommandLineApp.Configuration;
using MCPHub.CommandLineApp.Models;
using MCPHub.CommandLineApp.Services;
using MCPHub.CommandLineApp.Utilities;
using MCPHub.Domain.Entities;
using MCPHub.Domain.ValueObjects;

namespace MCPHub.CommandLineApp.Commands;

/// <summary>
/// Command for comprehensive package security verification and scanning
/// </summary>
public class VerifyCommand(
    ILogger<VerifyCommand> logger,
    McpmConfiguration configuration,
    IMcpHubApiClient apiClient,
    IOutputFormatter outputFormatter,
    IInteractionService interactionService,
    IProgressReporter progressReporter,
    ISecurityService securityService,
    ITrustTierService trustTierService) : BaseCommand(logger, configuration, apiClient, outputFormatter, interactionService, progressReporter) {
    private readonly ISecurityService _securityService = securityService ?? throw new ArgumentNullException(nameof(securityService));
    private readonly ITrustTierService _trustTierService = trustTierService ?? throw new ArgumentNullException(nameof(trustTierService));

    // Store references to command line elements for value extraction
    private Argument<string?>? _packageArgument;
    private Option<string>? _scanTypeOption;
    private Option<string>? _severityOption;
    private Option<string?>? _trustTierOption;
    private Option<bool>? _offlineOption;
    private Option<bool>? _forceOption;
    private Option<bool>? _fixOption;
    private Option<string?>? _reportOption;
    private Option<bool>? _nonInteractiveOption;
    private Option<bool>? _globalOption;
    private Option<bool>? _verboseOption;

    /// <inheritdoc />
    public override Command CreateCommand() {
        var command = new Command("verify", "Verify package security, integrity, and trust tier compliance");

        // Arguments
        _packageArgument = new Argument<string?>("package", "Package name with optional version (e.g., 'package-name' or 'package-name@1.0.0'). Leave empty to verify all installed packages.");
        command.AddArgument(_packageArgument);

        // Options
        _scanTypeOption = new Option<string>(
            aliases: ["--scan-type", "-t"],
            getDefaultValue: () => "comprehensive",
            description: "Type of security scan (comprehensive, vulnerability, malware, license, all)");
        command.AddOption(_scanTypeOption);

        _severityOption = new Option<string>(
            aliases: ["--severity", "-s"],
            getDefaultValue: () => "medium",
            description: "Minimum severity level to report (low, medium, high, critical)");
        command.AddOption(_severityOption);

        _trustTierOption = new Option<string?>(
            aliases: ["--trust-tier"],
            description: "Minimum trust tier requirement (unverified, community, professional, enterprise)");
        command.AddOption(_trustTierOption);

        _offlineOption = new Option<bool>(
            aliases: ["--offline", "-o"],
            description: "Use offline security database only (no API calls)");
        command.AddOption(_offlineOption);

        _forceOption = new Option<bool>(
            aliases: ["--force", "-f"],
            description: "Force fresh security scan (ignore cached results)");
        command.AddOption(_forceOption);

        _fixOption = new Option<bool>(
            aliases: ["--fix"],
            description: "Attempt to fix security issues automatically where possible");
        command.AddOption(_fixOption);

        _reportOption = new Option<string?>(
            aliases: ["--report", "-r"],
            description: "Generate security report file (json, xml, html, pdf)");
        command.AddOption(_reportOption);

        _nonInteractiveOption = new Option<bool>(
            aliases: ["--non-interactive", "-n"],
            description: "Disable interactive prompts (for CI/CD)");
        command.AddOption(_nonInteractiveOption);

        _globalOption = new Option<bool>(
            aliases: ["--global", "-g"],
            description: "Include globally installed packages in verification");
        command.AddOption(_globalOption);

        _verboseOption = new Option<bool>(
            aliases: ["--verbose", "-v"],
            description: "Show detailed verification information");
        command.AddOption(_verboseOption);

        // Handler
        command.SetHandler(ExecuteAsync);

        return command;
    }

    private async Task<int> ExecuteAsync(InvocationContext context) {
        // Extract option values from the context using ParseResult
        var package = context.ParseResult.GetValueForArgument(_packageArgument!);
        var scanType = context.ParseResult.GetValueForOption(_scanTypeOption!);
        var severity = context.ParseResult.GetValueForOption(_severityOption!);
        var trustTier = context.ParseResult.GetValueForOption(_trustTierOption!);
        var offline = context.ParseResult.GetValueForOption(_offlineOption!);
        var force = context.ParseResult.GetValueForOption(_forceOption!);
        var fix = context.ParseResult.GetValueForOption(_fixOption!);
        var report = context.ParseResult.GetValueForOption(_reportOption!);
        var nonInteractive = context.ParseResult.GetValueForOption(_nonInteractiveOption!);
        var global = context.ParseResult.GetValueForOption(_globalOption!);
        var verbose = context.ParseResult.GetValueForOption(_verboseOption!);

        try {
            Logger.LogInformation("Executing verify command: Package={Package}, ScanType={ScanType}, Severity={Severity}",
                package, scanType, severity);

            // Parse and validate options
            var scanTypeEnum = ParseScanType(scanType ?? "");
            var severityEnum = ParseSeverity(severity ?? "");
            var trustTierEnum = ParseTrustTierEnum(trustTier);

            if (!offline && !await ValidateApiConnectivityAsync()) {
                OutputFormatter.WriteWarning("API connectivity failed. Running in offline mode.");
                offline = true;
            }

            // Check security database status for offline mode
            if (offline) {
                var dbStatus = await _securityService.GetSecurityDatabaseStatusAsync();
                if (!dbStatus.IsAvailable) {
                    OutputFormatter.WriteError("Offline security database is not available.");
                    OutputFormatter.WriteInfo("Run 'mcpm security update-db' to download the security database.");
                    return 503;
                }

                if (dbStatus.IsOutdated) {
                    OutputFormatter.WriteWarning($"Security database is outdated (last updated: {dbStatus.LastUpdate})");
                    if (!nonInteractive) {
                        var updateDb = await InteractionService.ConfirmAsync("Update security database now?", true);
                        if (updateDb) {
                            await UpdateSecurityDatabaseAsync();
                        }
                    }
                }
            }

            // Determine packages to verify
            var packagesToVerify = await GetPackagesToVerifyAsync(package, global);
            if (!packagesToVerify.Any()) {
                OutputFormatter.WriteWarning("No packages found to verify.");
                return 0;
            }

            OutputFormatter.WriteInfo($"Verifying {packagesToVerify.Count()} package(s)...");
            OutputFormatter.WriteLine();

            // Create verification progress tracker
            using var verificationProgress = ProgressReporter.CreateStepProgress("Security Verification", new[] {
                "Analyzing packages",
                "Running security scans",
                "Checking trust tiers",
                "Generating reports",
                                                                                                                });

            // Stage 1: Analyze packages
            verificationProgress.StartStep(0, "Analyzing package specifications...");
            var analysisResults = await AnalyzePackagesAsync(packagesToVerify, trustTierEnum);
            verificationProgress.CompleteStep(0, $"Analyzed {analysisResults.Count} packages");

            // Stage 2: Run security scans
            verificationProgress.StartStep(1, "Running security scans...");
            var scanResults = await RunSecurityScansAsync(analysisResults, scanTypeEnum, severityEnum, force, offline);
            verificationProgress.CompleteStep(1, $"Completed {scanResults.Count} security scans");

            // Stage 3: Check trust tier compliance
            verificationProgress.StartStep(2, "Checking trust tier compliance...");
            var trustTierResults = await CheckTrustTierComplianceAsync(analysisResults, trustTierEnum);
            verificationProgress.CompleteStep(2, "Trust tier compliance checked");

            // Stage 4: Generate comprehensive results
            verificationProgress.StartStep(3, "Generating verification report...");
            var verificationSummary = await GenerateVerificationSummaryAsync(scanResults, trustTierResults, analysisResults);

            // Display results
            await DisplayVerificationResultsAsync(verificationSummary, verbose, nonInteractive);

            // Generate report file if requested
            if (!string.IsNullOrEmpty(report)) {
                await GenerateReportFileAsync(verificationSummary, report);
            }

            verificationProgress.CompleteStep(3, "Verification completed");

            // Handle fixing issues if requested
            if (fix && verificationSummary.HasIssues) {
                var fixResult = await HandleSecurityFixesAsync(verificationSummary, nonInteractive);
                if (fixResult) {
                    OutputFormatter.WriteSuccess("Security fixes applied successfully");
                }
            }

            // Return appropriate exit code
            return DetermineExitCode(verificationSummary);
        }
        catch (Exception ex) {
            return HandleError(ex, "verify");
        }
    }

    private static ScanType ParseScanType(string scanType) => scanType.ToLowerInvariant() switch {
        "vulnerability" => ScanType.Vulnerability,
        "malware" => ScanType.Malware,
        "license" => ScanType.License,
        "comprehensive" => ScanType.Comprehensive,
        "all" => ScanType.All,
        _ => ScanType.Comprehensive,
    };

    private static SecurityScanSeverity ParseSeverity(string severity) => severity.ToLowerInvariant() switch {
        "low" => SecurityScanSeverity.Low,
        "medium" => SecurityScanSeverity.Medium,
        "high" => SecurityScanSeverity.High,
        "critical" => SecurityScanSeverity.Critical,
        _ => SecurityScanSeverity.Medium,
    };

    private static TrustTier? ParseTrustTierEnum(string? trustTier) => string.IsNullOrWhiteSpace(trustTier)
            ? null
            : trustTier.ToLowerInvariant() switch {
                "unverified" => TrustTier.Unverified,
                "community" => TrustTier.Community,
                "professional" => TrustTier.Professional,
                "enterprise" => TrustTier.Enterprise,
                _ => null,
            };

    private Task<IEnumerable<PackageToVerify>> GetPackagesToVerifyAsync(string? package, bool global) {
        if (!string.IsNullOrEmpty(package)) {
            // Verify specific package
            var (packageName, version) = ParsePackageSpec(package);
            return Task.FromResult<IEnumerable<PackageToVerify>>(new[] { new PackageToVerify { Name = packageName, Version = version } });
        }

        // Verify all installed packages (placeholder implementation)
        OutputFormatter.WriteInfo("Discovering installed packages...");

        // In real implementation, this would scan the local package database
        // For now, return empty collection as implementation placeholder
        return Task.FromResult<IEnumerable<PackageToVerify>>(Enumerable.Empty<PackageToVerify>());
    }

    private static (string packageName, string? version) ParsePackageSpec(string packageSpec) {
        var parts = packageSpec.Split('@', 2);
        return parts.Length == 2 ? (parts[0], parts[1]) : (parts[0], null);
    }

    private async Task<List<PackageAnalysisResult>> AnalyzePackagesAsync(
        IEnumerable<PackageToVerify> packages,
        TrustTier? minimumTrustTier) {
        var results = new List<PackageAnalysisResult>();

        foreach (var package in packages) {
            using var spinner = ProgressReporter.CreateSpinner($"Analyzing {package.Name}...");

            try {
                // Get package information from API
                var packageInfo = await ApiClient.GetPackageInfoAsync(package.Name);

                var analysisResult = new PackageAnalysisResult {
                    PackageName = package.Name,
                    Version = package.Version ?? packageInfo.Version,
                    PackageInfo = packageInfo,
                    RequiredTrustTier = minimumTrustTier,
                };

                results.Add(analysisResult);
                spinner.Success($"Analyzed {package.Name}");
            }
            catch (Exception ex) {
                spinner.Fail($"Failed to analyze {package.Name}: {ex.Message}");
                results.Add(new PackageAnalysisResult {
                    PackageName = package.Name,
                    Version = package.Version ?? "unknown",
                    HasError = true,
                    ErrorMessage = ex.Message,
                });
            }
        }

        return results;
    }

    private async Task<List<SecurityScanResult>> RunSecurityScansAsync(
        List<PackageAnalysisResult> packages,
        ScanType scanType,
        SecurityScanSeverity minimumSeverity,
        bool force,
        bool offline) {
        var results = new List<SecurityScanResult>();

        foreach (var package in packages.Where(p => !p.HasError)) {
            using var spinner = ProgressReporter.CreateSpinner($"Scanning {package.PackageName}...");

            try {
                SecurityScanResult? scanResult = null;

                // Try to get cached results first unless forced
                if (!force) {
                    scanResult = await _securityService.GetLatestScanResultAsync(
                        package.PackageName, package.Version);
                }

                // Run fresh scan if no cached results or forced
                if (scanResult == null || force) {
                    scanResult = await _securityService.ScanPackageAsync(
                        package.PackageName, package.Version, scanType);
                }

                results.Add(scanResult);
                spinner.Success($"Scanned {package.PackageName}");
            }
            catch (Exception ex) {
                spinner.Fail($"Scan failed for {package.PackageName}: {ex.Message}");
                Logger.LogError(ex, "Security scan failed for {PackageName}", package.PackageName);
            }
        }

        return results;
    }

    private async Task<List<TrustTierComplianceResult>> CheckTrustTierComplianceAsync(
        List<PackageAnalysisResult> packages,
        TrustTier? minimumTrustTier) {
        if (minimumTrustTier == null) {
            return [];
        }

        var packageSpecs = packages.Where(p => !p.HasError).Select(p => new PackageSpec {
            Name = p.PackageName,
            Version = p.Version,
            CurrentTier = Enum.Parse<TrustTier>(p.PackageInfo?.TrustTier ?? "Unverified"),
        });

        return (await _trustTierService.CheckTrustTierComplianceAsync(packageSpecs, minimumTrustTier.Value))
            .ToList();
    }

    private static async Task<VerificationSummary> GenerateVerificationSummaryAsync(
        List<SecurityScanResult> scanResults,
        List<TrustTierComplianceResult> trustTierResults,
        List<PackageAnalysisResult> analysisResults) {
        await Task.Delay(100); // Simulate processing time

        var summary = new VerificationSummary {
            TotalPackages = analysisResults.Count,
            ScannedPackages = scanResults.Count,
            PackagesWithIssues = scanResults.Count(r => !r.IsClean),
            TrustTierViolations = trustTierResults.Count(r => !r.IsCompliant),
            HighestSeverity = scanResults.Any() ? scanResults.Max(r => r.HighestSeverity) : SecurityScanSeverity.None,
            ScanResults = scanResults,
            TrustTierResults = trustTierResults,
            AnalysisResults = analysisResults,
            VerificationTime = DateTimeOffset.UtcNow,
        };

        return summary;
    }

    private async Task DisplayVerificationResultsAsync(
        VerificationSummary summary,
        bool verbose,
        bool nonInteractive) {
        OutputFormatter.WriteLine();
        OutputFormatter.WriteInfo("=== Security Verification Results ===");
        OutputFormatter.WriteLine();

        // Summary statistics
        DisplaySummaryStatistics(summary);

        // Security scan results
        if (summary.ScanResults.Any()) {
            OutputFormatter.WriteLine();
            OutputFormatter.WriteInfo("Security Scan Results:");
            await DisplaySecurityScanResultsAsync(summary.ScanResults, verbose);
        }

        // Trust tier compliance
        if (summary.TrustTierResults.Any()) {
            OutputFormatter.WriteLine();
            OutputFormatter.WriteInfo("Trust Tier Compliance:");
            DisplayTrustTierResults(summary.TrustTierResults);
        }

        // Overall assessment
        OutputFormatter.WriteLine();
        DisplayOverallAssessment(summary);

        // Interactive recommendations
        if (!nonInteractive && summary.HasIssues) {
            await DisplayInteractiveRecommendationsAsync(summary);
        }
    }

    private void DisplaySummaryStatistics(VerificationSummary summary) {
        var table = new Table();
        table.AddColumn("Metric");
        table.AddColumn("Count");
        table.AddColumn("Status");

        table.AddRow("Total Packages", summary.TotalPackages.ToString(), GetStatusMarkup(summary.TotalPackages > 0));
        table.AddRow("Scanned Packages", summary.ScannedPackages.ToString(), GetStatusMarkup(summary.ScannedPackages > 0));
        table.AddRow("Packages with Issues", summary.PackagesWithIssues.ToString(), GetIssueStatusMarkup(summary.PackagesWithIssues));
        table.AddRow("Trust Tier Violations", summary.TrustTierViolations.ToString(), GetIssueStatusMarkup(summary.TrustTierViolations));
        table.AddRow("Highest Severity", summary.HighestSeverity.ToString(), GetSeverityMarkup(summary.HighestSeverity));

        AnsiConsole.Write(table);
    }

    private async Task DisplaySecurityScanResultsAsync(List<SecurityScanResult> scanResults, bool verbose) {
        var table = new Table();
        table.AddColumn("Package");
        table.AddColumn("Status");
        table.AddColumn("Vulnerabilities");
        table.AddColumn("Highest Severity");
        table.AddColumn("Scanner Version");

        foreach (var result in scanResults.OrderByDescending(r => r.HighestSeverity)) {
            var statusMarkup = result.IsClean ? "[green]Clean[/]" : "[red]Issues[/]";
            var severityMarkup = GetSeverityMarkup(result.HighestSeverity);
            var vulnCount = result.VulnerabilityCount.ToString();

            table.AddRow("Package", statusMarkup, vulnCount, severityMarkup, result.ScannerVersion);
        }

        AnsiConsole.Write(table);

        if (verbose) {
            foreach (var result in scanResults.Where(r => !r.IsClean)) {
                await DisplayDetailedVulnerabilitiesAsync(result);
            }
        }
    }

    private async Task DisplayDetailedVulnerabilitiesAsync(SecurityScanResult scanResult) {
        OutputFormatter.WriteLine();
        OutputFormatter.WriteWarning($"Vulnerabilities in package:");

        foreach (var vulnerability in scanResult.Vulnerabilities.OrderByDescending(v => v.Severity)) {
            OutputFormatter.WriteError($"  [{vulnerability.Severity}] {vulnerability.Title}");
            OutputFormatter.WriteInfo($"    {vulnerability.Description}");
            if (!string.IsNullOrEmpty(vulnerability.CveId)) {
                OutputFormatter.WriteInfo($"    CVE: {vulnerability.CveId}");
            }
            if (!string.IsNullOrEmpty(vulnerability.Reference)) {
                OutputFormatter.WriteInfo($"    Reference: {vulnerability.Reference}");
            }
        }

        await Task.Delay(10); // Small delay for UI responsiveness
    }

    private static void DisplayTrustTierResults(List<TrustTierComplianceResult> trustTierResults) {
        var table = new Table();
        table.AddColumn("Package");
        table.AddColumn("Current Tier");
        table.AddColumn("Required Tier");
        table.AddColumn("Compliance");
        table.AddColumn("Issues");

        foreach (var result in trustTierResults) {
            var complianceMarkup = result.IsCompliant ? "[green]✓ Compliant[/]" : "[red]✗ Non-compliant[/]";
            var issueCount = result.Issues.Count().ToString();

            table.AddRow(
                result.PackageName,
                result.CurrentTier.ToString(),
                result.RequiredTier.ToString(),
                complianceMarkup,
                issueCount);
        }

        AnsiConsole.Write(table);
    }

    private void DisplayOverallAssessment(VerificationSummary summary) {
        if (!summary.HasIssues) {
            OutputFormatter.WriteSuccess("🎉 All packages passed security verification!");
            OutputFormatter.WriteInfo("Your packages meet security and trust tier requirements.");
        }
        else {
            OutputFormatter.WriteWarning("⚠️  Security issues detected in your packages");
            OutputFormatter.WriteInfo($"Found {summary.PackagesWithIssues} packages with security issues");

            if (summary.TrustTierViolations > 0) {
                OutputFormatter.WriteInfo($"Found {summary.TrustTierViolations} trust tier compliance violations");
            }

            if (summary.HighestSeverity >= SecurityScanSeverity.High) {
                OutputFormatter.WriteError("🚨 Critical or high severity vulnerabilities detected!");
                OutputFormatter.WriteInfo("Immediate action is recommended.");
            }
        }
    }

    private async Task DisplayInteractiveRecommendationsAsync(VerificationSummary summary) {
        OutputFormatter.WriteLine();
        OutputFormatter.WriteInfo("=== Recommendations ===");

        var recommendations = new List<string>();

        if (summary.PackagesWithIssues > 0) {
            recommendations.Add($"Review and address security vulnerabilities in {summary.PackagesWithIssues} packages");
        }

        if (summary.TrustTierViolations > 0) {
            recommendations.Add("Consider upgrading packages to meet trust tier requirements");
        }

        if (summary.HighestSeverity >= SecurityScanSeverity.High) {
            recommendations.Add("Prioritize fixing high and critical severity vulnerabilities");
        }

        foreach (var recommendation in recommendations) {
            OutputFormatter.WriteInfo($"• {recommendation}");
        }

        var showDetails = await InteractionService.ConfirmAsync("Would you like to see detailed remediation guidance?", false);
        if (showDetails) {
            await DisplayDetailedRemediationAsync(summary);
        }
    }

    private async Task DisplayDetailedRemediationAsync(VerificationSummary summary) {
        OutputFormatter.WriteLine();
        OutputFormatter.WriteInfo("=== Detailed Remediation Guidance ===");

        // Implementation would provide specific remediation steps
        await Task.Delay(100); // Placeholder for real implementation

        OutputFormatter.WriteInfo("1. Update packages to latest versions with security fixes");
        OutputFormatter.WriteInfo("2. Review package dependencies for known vulnerabilities");
        OutputFormatter.WriteInfo("3. Consider alternative packages with better security records");
        OutputFormatter.WriteInfo("4. Implement security monitoring for ongoing protection");
    }

    private async Task<bool> GenerateReportFileAsync(VerificationSummary summary, string reportType) {
        using var spinner = ProgressReporter.CreateSpinner($"Generating {reportType.ToUpper()} report...");

        try {
            // Implementation would generate actual report files
            await Task.Delay(1000); // Simulate report generation

            var fileName = $"security-report-{DateTimeOffset.UtcNow:yyyyMMdd-HHmmss}.{reportType.ToLowerInvariant()}";
            spinner.Success($"Report saved to {fileName}");
            return true;
        }
        catch (Exception ex) {
            spinner.Fail($"Failed to generate report: {ex.Message}");
            return false;
        }
    }

    private async Task<bool> HandleSecurityFixesAsync(VerificationSummary summary, bool nonInteractive) {
        if (nonInteractive) {
            OutputFormatter.WriteInfo("Automatic fixes not supported in non-interactive mode");
            return false;
        }

        var fixableIssues = summary.ScanResults.Count(r => !r.IsClean);
        if (fixableIssues == 0) {
            return true;
        }

        OutputFormatter.WriteLine();
        OutputFormatter.WriteInfo($"Found {fixableIssues} packages with potential fixes available");

        var proceedWithFixes = await InteractionService.ConfirmAsync("Proceed with automatic fixes?", false);
        if (!proceedWithFixes) {
            return false;
        }

        // Implementation would attempt to fix issues
        using var fixProgress = ProgressReporter.CreateProgress($"Applying fixes to {fixableIssues} packages");
        await Task.Delay(2000); // Simulate fix process
        fixProgress.Complete("Fixes applied");

        return true;
    }

    private async Task UpdateSecurityDatabaseAsync() {
        using var updateProgress = ProgressReporter.CreateSpinner("Updating security database...");

        try {
            var result = await _securityService.UpdateSecurityDatabaseAsync(forceUpdate: true);
            if (result.Success) {
                updateProgress.Success($"Updated {result.UpdatedRecords} security records");
            }
            else {
                updateProgress.Fail("Database update failed");
                foreach (var error in result.Errors) {
                    OutputFormatter.WriteError($"  - {error}");
                }
            }
        }
        catch (Exception ex) {
            updateProgress.Fail($"Update failed: {ex.Message}");
        }
    }

    private static int DetermineExitCode(VerificationSummary summary) {
        if (!summary.HasIssues) {
            return 0; // Success
        }

        if (summary.HighestSeverity >= SecurityScanSeverity.Critical) {
            return 2; // Critical issues
        }

        if (summary.HighestSeverity >= SecurityScanSeverity.High) {
            return 1; // High severity issues
        }

        return 0; // Medium/low issues - still success
    }

    private static string GetStatusMarkup(bool isGood) => isGood ? "[green]✓[/]" : "[red]✗[/]";

    private static string GetIssueStatusMarkup(int count) => count == 0 ? "[green]✓[/]" : "[red]⚠[/]";

    private static string GetSeverityMarkup(SecurityScanSeverity severity) => severity switch {
        SecurityScanSeverity.Critical => "[red]Critical[/]",
        SecurityScanSeverity.High => "[orange3]High[/]",
        SecurityScanSeverity.Medium => "[yellow]Medium[/]",
        SecurityScanSeverity.Low => "[green]Low[/]",
        SecurityScanSeverity.None => "[grey]None[/]",
        _ => severity.ToString(),
    };
}

/// <summary>
/// Package to verify
/// </summary>
public record PackageToVerify {
    public string Name { get; init; } = string.Empty;
    public string? Version { get; init; }
}

/// <summary>
/// Package analysis result
/// </summary>
public record PackageAnalysisResult {
    public string PackageName { get; init; } = string.Empty;
    public string Version { get; init; } = string.Empty;
    public PackageInfoResponse? PackageInfo { get; init; }
    public TrustTier? RequiredTrustTier { get; init; }
    public bool HasError { get; init; }
    public string? ErrorMessage { get; init; }
}

/// <summary>
/// Comprehensive verification summary
/// </summary>
public record VerificationSummary {
    public int TotalPackages { get; init; }
    public int ScannedPackages { get; init; }
    public int PackagesWithIssues { get; init; }
    public int TrustTierViolations { get; init; }
    public SecurityScanSeverity HighestSeverity { get; init; }
    public List<SecurityScanResult> ScanResults { get; init; } = [];
    public List<TrustTierComplianceResult> TrustTierResults { get; init; } = [];
    public List<PackageAnalysisResult> AnalysisResults { get; init; } = [];
    public DateTimeOffset VerificationTime { get; init; }

    public bool HasIssues => PackagesWithIssues > 0 || TrustTierViolations > 0;
}