using System.CommandLine;
using System.CommandLine.Invocation;

using MCPHub.CommandLineApp.Configuration;
using MCPHub.CommandLineApp.Models;
using MCPHub.CommandLineApp.Services;
using MCPHub.CommandLineApp.Utilities;
using MCPHub.Domain.Contracts.Responses;
using MCPHub.Domain.Entities;
using MCPHub.Domain.ValueObjects;

using Spectre.Console;

namespace MCPHub.CommandLineApp.Commands;

/// <summary>
/// Command for installing MCP packages
/// </summary>
public class InstallCommand(
    ILogger<InstallCommand> logger,
    McpmConfiguration configuration,
    IMcpHubApiClient apiClient,
    IOutputFormatter outputFormatter,
    IInteractionService interactionService,
    IProgressReporter progressReporter,
    PackageManager packageManager,
    DependencyResolver dependencyResolver,
    ISecurityService securityService,
    ITrustTierService trustTierService) : BaseCommand(logger, configuration, apiClient, outputFormatter, interactionService, progressReporter) {
    private readonly PackageManager _packageManager = packageManager ?? throw new ArgumentNullException(nameof(packageManager));
    private readonly DependencyResolver _dependencyResolver = dependencyResolver ?? throw new ArgumentNullException(nameof(dependencyResolver));
    private readonly ISecurityService _securityService = securityService ?? throw new ArgumentNullException(nameof(securityService));
    private readonly ITrustTierService _trustTierService = trustTierService ?? throw new ArgumentNullException(nameof(trustTierService));

    // Store references to command line elements for value extraction
    private Argument<string>? _packageArgument;
    private Option<bool>? _devOption;
    private Option<bool>? _forceOption;
    private Option<bool>? _skipVerifyOption;
    private Option<string?>? _minTrustTierOption;
    private Option<string?>? _minSecurityGradeOption;
    private Option<bool>? _allowVulnerabilitiesOption;
    private Option<bool>? _securityScanOption;
    private Option<bool>? _globalOption;
    private Option<bool>? _yesOption;
    private Option<bool>? _dryRunOption;
    private Option<bool>? _nonInteractiveOption;

    /// <inheritdoc />
    public override Command CreateCommand() {
        var command = new Command("install", "Install MCP packages with dependency resolution");

        // Arguments
        _packageArgument = new Argument<string>("package", "Package name with optional version (e.g., 'package-name' or 'package-name@1.0.0')");
        command.AddArgument(_packageArgument);

        // Options
        _devOption = new Option<bool>(
            aliases: ["--dev", "-D"],
            description: "Install as development dependency");
        command.AddOption(_devOption);

        _forceOption = new Option<bool>(
            aliases: ["--force", "-f"],
            description: "Force reinstallation even if already installed");
        command.AddOption(_forceOption);

        _skipVerifyOption = new Option<bool>(
            aliases: ["--skip-verify"],
            description: "Skip security verification and trust tier checks");
        command.AddOption(_skipVerifyOption);

        _minTrustTierOption = new Option<string?>(
            aliases: ["--min-trust-tier"],
            description: "Minimum trust tier requirement (unverified, community, professional, enterprise)");
        command.AddOption(_minTrustTierOption);

        _minSecurityGradeOption = new Option<string?>(
            aliases: ["--min-security-grade"],
            description: "Minimum security grade requirement (A+, A, B, C, D, F)");
        command.AddOption(_minSecurityGradeOption);

        _allowVulnerabilitiesOption = new Option<bool>(
            aliases: ["--allow-vulnerabilities"],
            description: "Allow packages with known vulnerabilities (not recommended)");
        command.AddOption(_allowVulnerabilitiesOption);

        _securityScanOption = new Option<bool>(
            aliases: ["--security-scan"],
            getDefaultValue: () => true,
            description: "Perform comprehensive security scan during installation");
        command.AddOption(_securityScanOption);

        _globalOption = new Option<bool>(
            aliases: ["--global", "-g"],
            description: "Install package globally");
        command.AddOption(_globalOption);

        _yesOption = new Option<bool>(
            aliases: ["--yes", "-y"],
            description: "Automatically confirm all prompts");
        command.AddOption(_yesOption);

        _dryRunOption = new Option<bool>(
            aliases: ["--dry-run"],
            description: "Show what would be installed without actually installing");
        command.AddOption(_dryRunOption);

        _nonInteractiveOption = new Option<bool>(
            aliases: ["--non-interactive", "-n"],
            description: "Disable interactive prompts (for CI/CD)");
        command.AddOption(_nonInteractiveOption);

        // Handler
        command.SetHandler(ExecuteAsync);

        return command;
    }

    private async Task<int> ExecuteAsync(InvocationContext context) {
        // Extract option values from the context using ParseResult
        var package = context.ParseResult.GetValueForArgument(_packageArgument!);
        var dev = context.ParseResult.GetValueForOption(_devOption!);
        var force = context.ParseResult.GetValueForOption(_forceOption!);
        var skipVerify = context.ParseResult.GetValueForOption(_skipVerifyOption!);
        var minTrustTier = context.ParseResult.GetValueForOption(_minTrustTierOption!);
        var minSecurityGrade = context.ParseResult.GetValueForOption(_minSecurityGradeOption!);
        var allowVulnerabilities = context.ParseResult.GetValueForOption(_allowVulnerabilitiesOption!);
        var securityScan = context.ParseResult.GetValueForOption(_securityScanOption!);
        var global = context.ParseResult.GetValueForOption(_globalOption!);
        var yes = context.ParseResult.GetValueForOption(_yesOption!);
        var dryRun = context.ParseResult.GetValueForOption(_dryRunOption!);
        var nonInteractive = context.ParseResult.GetValueForOption(_nonInteractiveOption!);

        try {
            Logger.LogInformation("Executing install command: Package={Package}, Dev={Dev}, Force={Force}, Global={Global}",
                package, dev, force, global);

            // Parse package name and version
            var (packageName, version) = ParsePackageSpec(package);
            if (string.IsNullOrWhiteSpace(packageName)) {
                OutputFormatter.WriteError("Package name is required");
                return 400;
            }

            // Check API connectivity
            if (!await ValidateApiConnectivityAsync()) {
                return 503; // Service unavailable
            }

            // Stage 1: Fetch - Get package information with enhanced progress
            PackageInfoResponse packageInfo;
            using var fetchProgress = ProgressReporter.CreateStepProgress("Package Installation", new[] {
                "Fetching package information",
                "Resolving dependencies",
                "Verifying security",
                "Installing packages"
            });
            fetchProgress.StartStep(0, $"Getting information for '{packageName}'...");

            try {
                packageInfo = await ApiClient.GetPackageInfoAsync(packageName);
                fetchProgress.CompleteStep(0, $"Retrieved {packageName} information");
            }
            catch (Exception ex) {
                fetchProgress.FailStep(0, $"Failed to get package information: {ex.Message}");
                throw;
            }

            // Resolve version if not specified
            if (string.IsNullOrEmpty(version)) {
                version = packageInfo.Version; // Latest version
                if (!nonInteractive) {
                    OutputFormatter.WriteInfo($"No version specified, using latest: {version}");
                }
            }

            // Check if already installed (unless force)
            if (!force) {
                var installedVersion = await _packageManager.GetInstalledPackageAsync(packageName, version, global);
                if (installedVersion != null) {
                    if (!nonInteractive && !yes) {
                        var reinstall = await InteractionService.ConfirmAsync(
                            $"Package '{packageName}@{version}' is already installed. Reinstall?", false);
                        if (!reinstall) {
                            fetchProgress.CompleteAll("Installation cancelled by user");
                            return 0;
                        }
                    }
                    else if (!nonInteractive) {
                        OutputFormatter.WriteInfo($"Package '{packageName}@{version}' is already installed");
                        OutputFormatter.WriteInfo("Use --force to reinstall");
                        return 0;
                    }
                }
            }

            // Stage 2: Resolve dependencies with interactive choices
            fetchProgress.StartStep(1, "Analyzing dependencies...");
            var dependencyResult = await _dependencyResolver.ResolveDependenciesAsync(packageName, version, dev, global);

            if (!dependencyResult.Success) {
                fetchProgress.FailStep(1, "Dependency resolution failed");
                OutputFormatter.WriteError("Failed to resolve dependencies:");
                foreach (var error in dependencyResult.Errors) {
                    OutputFormatter.WriteError($"  - {error}");
                }
                return 1;
            }

            fetchProgress.CompleteStep(1, $"Resolved {dependencyResult.PackagesToInstall.Count} packages");

            // Interactive dependency resolution (mock implementation for conflicts)
            var mockConflicts = new List<object>(); // In real implementation, get from dependencyResult
            if (!nonInteractive && mockConflicts.Any()) {
                var resolveConflicts = await HandleDependencyConflictsAsync(mockConflicts);
                if (!resolveConflicts) {
                    fetchProgress.CompleteAll("Installation cancelled due to dependency conflicts");
                    return 130;
                }
            }

            // Show warnings
            foreach (var warning in dependencyResult.Warnings) {
                OutputFormatter.WriteWarning(warning);
            }

            // Stage 3: Comprehensive security verification
            fetchProgress.StartStep(2, "Performing comprehensive security verification...");

            // Parse security requirements
            var minTrustTierEnum = ParseTrustTierEnum(minTrustTier);
            var securityRequirements = new SecurityRequirements {
                MinimumTrustTier = minTrustTierEnum,
                MinimumSecurityGrade = minSecurityGrade,
                AllowVulnerabilities = allowVulnerabilities,
                PerformSecurityScan = securityScan && !skipVerify
            };

            // Perform comprehensive security assessment
            var securityAssessment = await PerformSecurityAssessmentAsync(
                dependencyResult.PackagesToInstall, securityRequirements, nonInteractive);

            if (!securityAssessment.IsApproved) {
                fetchProgress.FailStep(2, "Security verification failed");
                fetchProgress.SkipStep(3, "Security issues detected");

                DisplaySecurityIssues(securityAssessment);

                if (!nonInteractive && !yes) {
                    var proceedAnyway = await InteractionService.ConfirmAsync(
                        "Security issues detected. Do you want to proceed anyway? (NOT RECOMMENDED)", false);
                    if (!proceedAnyway) {
                        OutputFormatter.WriteInfo("Installation cancelled due to security concerns");
                        return 130;
                    }
                }
                else if (nonInteractive) {
                    OutputFormatter.WriteError("Installation blocked due to security policy violations");
                    return 130;
                }
            }

            // Display enhanced installation plan with security information
            await DisplayEnhancedInstallationPlanWithSecurityAsync(
                dependencyResult.PackagesToInstall, securityAssessment, dev, global, nonInteractive);

            if (dryRun) {
                fetchProgress.CompleteStep(2, "Security verification complete (dry run)");
                fetchProgress.SkipStep(3, "Dry run mode");
                OutputFormatter.WriteSuccess("Dry run completed. No packages were installed.");
                DisplaySecuritySummary(securityAssessment);
                return 0;
            }

            fetchProgress.CompleteStep(2, "Security verification completed");

            // Final installation confirmation
            if (!yes && !nonInteractive) {
                var confirmInstall = await ConfirmEnhancedInstallationAsync(dependencyResult.PackagesToInstall);
                if (!confirmInstall) {
                    fetchProgress.SkipStep(3, "User cancelled installation");
                    OutputFormatter.WriteInfo("Installation cancelled by user");
                    return 130;
                }
            }

            // Stage 4: Install packages with detailed progress
            fetchProgress.StartStep(3, "Installing packages...");
            var installationResult = await InstallPackagesWithEnhancedProgressAsync(dependencyResult.PackagesToInstall, dev, global);

            if (installationResult.Success) {
                fetchProgress.CompleteStep(3, $"Successfully installed {installationResult.InstalledCount} packages");
            }
            else {
                fetchProgress.FailStep(3, "Installation failed");
            }

            return await HandleInstallationResultAsync(installationResult);
        }
        catch (Exception ex) {
            return HandleError(ex, "install");
        }
    }

    private static (string packageName, string? version) ParsePackageSpec(string packageSpec) {
        var parts = packageSpec.Split('@', 2);
        return parts.Length == 2 ? (parts[0], parts[1]) : (parts[0], null);
    }

    /// <summary>
    /// Handles dependency conflicts interactively
    /// </summary>
    private async Task<bool> HandleDependencyConflictsAsync(IEnumerable<object> conflicts) {
        OutputFormatter.WriteWarning("Dependency conflicts detected:");

        foreach (var conflict in conflicts) {
            OutputFormatter.WriteWarning($"  - {conflict}");
        }

        return await InteractionService.ConfirmAsync(
            "Do you want to continue with automatic conflict resolution?", true);
    }

    /// <summary>
    /// Displays enhanced installation plan with interactive features
    /// </summary>
    private async Task DisplayEnhancedInstallationPlanAsync(List<ResolvedPackage> packages, List<UntrustedPackage> untrustedPackages, bool dev, bool global, bool nonInteractive) {
        DisplayInstallationPlan(packages, untrustedPackages, dev, global);

        // Interactive package review
        if (!nonInteractive && packages.Count > 1) {
            var reviewPackages = await InteractionService.ConfirmAsync(
                "Would you like to review individual packages before installation?", false);

            if (reviewPackages) {
                await ReviewPackagesInteractivelyAsync(packages);
            }
        }
    }

    /// <summary>
    /// Reviews packages interactively
    /// </summary>
    private async Task ReviewPackagesInteractivelyAsync(List<ResolvedPackage> packages) {
        foreach (var package in packages) {
            using var spinner = ProgressReporter.CreateSpinner($"Getting details for {package.Name}...");

            try {
                // In a real implementation, this would get detailed package info
                spinner.Success($"Loaded details for {package.Name}");

                OutputFormatter.WriteLine();
                OutputFormatter.WriteInfo($"Package: {package.Name}@{package.Version}");
                OutputFormatter.WriteInfo($"Trust Tier: {package.PackageInfo.TrustTier}");
                OutputFormatter.WriteInfo($"Security Grade: {package.PackageInfo.SecurityGrade ?? "N/A"}");

                var continueReview = await InteractionService.ConfirmAsync("Continue with this package?", true);
                if (!continueReview) {
                    throw new OperationCanceledException("Package review cancelled by user");
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException) {
                spinner.Fail($"Failed to get details: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Handles security consent with detailed information
    /// </summary>
    private async Task<bool> HandleSecurityConsentAsync(List<UntrustedPackage> untrustedPackages, bool yes, bool nonInteractive) {
        if (yes || nonInteractive) {
            return true;
        }

        var permissions = new List<string> {
            "Install packages with lower trust tiers",
            "Execute package code during installation",
            "Modify local package configuration"
        };

        var risks = untrustedPackages.Select(p =>
            $"{p.Name}@{p.Version}: Trust tier '{p.TrustTier}', Security grade '{p.SecurityGrade ?? "Unknown"}'").ToList();

        return await InteractionService.ShowConsentFlowAsync(
            "Security Warning: Untrusted Packages",
            $"You are about to install {untrustedPackages.Count} package(s) that do not meet your minimum trust tier requirements. " +
            "These packages may pose security risks to your system.",
            permissions,
            risks.Cast<string>());
    }

    /// <summary>
    /// Enhanced installation confirmation
    /// </summary>
    private async Task<bool> ConfirmEnhancedInstallationAsync(List<ResolvedPackage> packages) {
        var totalSize = packages.Sum(p => p.VersionInfo?.DownloadCount ?? 0);

        OutputFormatter.WriteLine();
        OutputFormatter.WriteInfo($"Ready to install {packages.Count} package(s)");
        if (totalSize > 0) {
            OutputFormatter.WriteInfo($"Estimated download: {totalSize:N0} bytes");
        }

        return await InteractionService.ConfirmAsync("Proceed with installation?", true);
    }

    /// <summary>
    /// Installs packages with enhanced progress tracking
    /// </summary>
    private async Task<InstallationResult> InstallPackagesWithEnhancedProgressAsync(List<ResolvedPackage> packages, bool dev, bool global) {
        var result = new InstallationResult();

        foreach (var package in packages) {
            using var downloadProgress = ProgressReporter.CreateDownloadProgress(
                $"{package.Name}@{package.Version}",
                package.VersionInfo?.DownloadCount ?? 1000000);

            try {
                // Get download URL
                downloadProgress.UpdateStatus("Requesting download...");
                var downloadRequest = new DownloadPackageRequest {
                    UserAgent = "mcpm-cli/1.0.0",
                    DownloadMethod = "CLI",
                    ClientVersion = "1.0.0"
                };

                var downloadResponse = await ApiClient.DownloadPackageAsync(package.Name, package.Version, downloadRequest);

                // Simulate download progress
                downloadProgress.UpdateStatus("Downloading...");
                for (var i = 0; i <= 100; i += 10) {
                    var bytesDownloaded = (package.VersionInfo?.DownloadCount ?? 1000000) * i / 100;
                    downloadProgress.UpdateProgress(bytesDownloaded, 50000); // 50KB/s speed simulation
                    await Task.Delay(100); // Simulate download time
                }

                downloadProgress.UpdateStatus("Extracting...");
                var installPath = await _packageManager.DownloadAndExtractPackageAsync(
                    downloadResponse, package.Name, package.Version, global, null);

                downloadProgress.UpdateStatus("Registering...");
                await _packageManager.RegisterPackageAsync(
                    package.Name, package.Version, installPath, package.PackageInfo, global, dev);

                // Record installation in API
                var installRequest = new InstallPackageRequest {
                    InstallationPath = installPath,
                    ClientVersion = "1.0.0",
                    IsGlobal = global,
                    IsDevelopmentDependency = dev
                };

                await ApiClient.InstallPackageAsync(package.Name, package.Version, installRequest);

                downloadProgress.Complete($"Installed {package.Name}@{package.Version}");
                result.InstalledCount++;

                Logger.LogInformation("Successfully installed {PackageName}@{Version}", package.Name, package.Version);
            }
            catch (Exception ex) {
                Logger.LogError(ex, "Failed to install {PackageName}@{Version}", package.Name, package.Version);
                result.Errors.Add($"Failed to install {package.Name}@{package.Version}: {ex.Message}");
                downloadProgress.Fail($"Installation failed: {ex.Message}");
            }
        }

        result.Success = result.InstalledCount > 0;
        return result;
    }

    /// <summary>
    /// Handles installation result with enhanced feedback
    /// </summary>
    private Task<int> HandleInstallationResultAsync(InstallationResult result) {
        if (result.Success) {
            OutputFormatter.WriteSuccess($"Successfully installed {result.InstalledCount} package(s)");

            if (result.Errors.Any()) {
                OutputFormatter.WriteWarning("Installation completed with some warnings:");
                foreach (var error in result.Errors) {
                    OutputFormatter.WriteWarning($"  - {error}");
                }
            }

            // Show post-installation summary
            OutputFormatter.WriteLine();
            OutputFormatter.WriteInfo("Installation Summary:");
            OutputFormatter.WriteInfo($"  ✓ {result.InstalledCount} packages installed successfully");
            if (result.Errors.Any()) {
                OutputFormatter.WriteInfo($"  ⚠ {result.Errors.Count} packages had warnings");
            }

            return Task.FromResult(0);
        }
        else {
            OutputFormatter.WriteError("Installation failed:");
            foreach (var error in result.Errors) {
                OutputFormatter.WriteError($"  - {error}");
            }
            return Task.FromResult(1);
        }
    }

    private void DisplayInstallationPlan(List<ResolvedPackage> packages, List<UntrustedPackage> untrustedPackages, bool dev, bool global) {
        OutputFormatter.WriteLine();

        var scopeText = global ? "globally" : "locally";
        var depTypeText = dev ? "as development dependencies" : "";

        OutputFormatter.WriteInfo($"Installation plan ({scopeText} {depTypeText}):".Trim());
        OutputFormatter.WriteLine();

        var table = new Table();
        table.AddColumn("Package");
        table.AddColumn("Version");
        table.AddColumn("Trust Tier");
        table.AddColumn("Security");
        table.AddColumn("Type");

        foreach (var package in packages.OrderBy(p => p.IsRootPackage ? 0 : 1).ThenBy(p => p.Name)) {
            var trustTier = package.PackageInfo.TrustTier;
            var securityGrade = package.PackageInfo.SecurityGrade ?? "N/A";
            var packageType = package.IsRootPackage ? "Main" : "Dependency";

            // Color code trust tier
            var trustTierMarkup = trustTier switch {
                "Enterprise" => $"[green]{trustTier}[/]",
                "Professional" => $"[blue]{trustTier}[/]",
                "Community" => $"[yellow]{trustTier}[/]",
                "Unverified" => $"[red]{trustTier}[/]",
                _ => trustTier
            };

            // Color code security grade
            var securityMarkup = securityGrade switch {
                "A" or "A+" => $"[green]{securityGrade}[/]",
                "B" or "B+" => $"[yellow]{securityGrade}[/]",
                "C" or "D" or "F" => $"[red]{securityGrade}[/]",
                _ => securityGrade
            };

            table.AddRow(package.Name, package.Version, trustTierMarkup, securityMarkup, packageType);
        }

        AnsiConsole.Write(table);

        if (untrustedPackages.Any()) {
            OutputFormatter.WriteLine();
            OutputFormatter.WriteWarning($"Warning: {untrustedPackages.Count} package(s) do not meet minimum trust tier requirements:");
            foreach (var untrusted in untrustedPackages) {
                OutputFormatter.WriteWarning($"  - {untrusted.Name}@{untrusted.Version} (Trust: {untrusted.TrustTier})");
            }
        }

        OutputFormatter.WriteLine();
    }

    private Task<bool> ConfirmUntrustedPackagesAsync(List<UntrustedPackage> untrustedPackages, bool autoConfirm) {
        if (autoConfirm)
            return Task.FromResult(true);

        OutputFormatter.WriteLine();
        OutputFormatter.WriteWarning("Security Warning:");
        OutputFormatter.WriteWarning($"The following {untrustedPackages.Count} package(s) do not meet your minimum trust tier requirement:");

        foreach (var package in untrustedPackages) {
            OutputFormatter.WriteWarning($"  - {package.Name}@{package.Version} (Trust: {package.TrustTier}, Grade: {package.SecurityGrade ?? "N/A"})");
        }

        OutputFormatter.WriteLine();
        OutputFormatter.WriteWarning("Installing these packages may pose security risks.");

        return Task.FromResult(AnsiConsole.Confirm("Do you want to continue with the installation?", false));
    }

    private Task<bool> ConfirmInstallationAsync(List<ResolvedPackage> packages) {
        var totalSize = packages.Sum(p => p.VersionInfo.DownloadCount); // Approximate
        OutputFormatter.WriteLine();
        OutputFormatter.WriteInfo($"About to install {packages.Count} package(s)");

        return Task.FromResult(AnsiConsole.Confirm("Continue with installation?", true));
    }

    /// <summary>
    /// Performs comprehensive security assessment for packages to be installed
    /// </summary>
    private async Task<SecurityAssessment> PerformSecurityAssessmentAsync(
        List<ResolvedPackage> packages,
        SecurityRequirements requirements,
        bool nonInteractive) {
        var assessment = new SecurityAssessment();

        foreach (var package in packages) {
            using var packageSpinner = ProgressReporter.CreateSpinner($"Assessing {package.Name}...");

            try {
                var packageSecurity = new PackageSecurityInfo {
                    PackageName = package.Name,
                    Version = package.Version
                };

                // Get trust tier assessment
                if (requirements.MinimumTrustTier.HasValue) {
                    var trustTierAssessment = await _trustTierService.GetTrustTierAssessmentAsync(package.Name);
                    packageSecurity.TrustTierAssessment = trustTierAssessment;

                    if (trustTierAssessment.CurrentTier < requirements.MinimumTrustTier.Value) {
                        packageSecurity.Issues.Add($"Trust tier {trustTierAssessment.CurrentTier} is below required minimum {requirements.MinimumTrustTier.Value}");
                    }
                }

                // Perform security scan if requested
                if (requirements.PerformSecurityScan) {
                    var scanResult = await _securityService.GetLatestScanResultAsync(package.Name, package.Version);
                    if (scanResult != null) {
                        packageSecurity.SecurityScanResult = scanResult;

                        if (!requirements.AllowVulnerabilities && !scanResult.IsClean) {
                            packageSecurity.Issues.Add($"Package has {scanResult.VulnerabilityCount} security vulnerabilities");
                        }

                        if (scanResult.HasCriticalVulnerabilities) {
                            packageSecurity.CriticalIssues.Add("Critical security vulnerabilities detected");
                        }

                        if (scanResult.HasHighVulnerabilities) {
                            packageSecurity.HighIssues.Add("High severity vulnerabilities detected");
                        }
                    }
                    else {
                        // No scan results available - trigger fresh scan
                        packageSecurity.Issues.Add("No recent security scan results available");
                    }
                }

                // Check security grade if required
                if (!string.IsNullOrEmpty(requirements.MinimumSecurityGrade)) {
                    var securityGrade = await _securityService.CalculateSecurityGradeAsync(package.Name, package.Version);
                    packageSecurity.SecurityGrade = securityGrade;

                    if (!MeetsMinimumGrade(securityGrade, requirements.MinimumSecurityGrade)) {
                        packageSecurity.Issues.Add($"Security grade {securityGrade} is below required minimum {requirements.MinimumSecurityGrade}");
                    }
                }

                assessment.PackageSecurityInfo.Add(packageSecurity);
                packageSpinner.Success($"Assessed {package.Name}");
            }
            catch (Exception ex) {
                packageSpinner.Fail($"Assessment failed for {package.Name}: {ex.Message}");
                assessment.Errors.Add($"Security assessment failed for {package.Name}: {ex.Message}");
            }
        }

        // Determine overall approval status
        assessment.IsApproved = assessment.PackageSecurityInfo.All(p => !p.Issues.Any() && !p.CriticalIssues.Any()) &&
                               !assessment.Errors.Any();

        return assessment;
    }

    /// <summary>
    /// Checks if a security grade meets the minimum requirement
    /// </summary>
    private static bool MeetsMinimumGrade(string actualGrade, string minimumGrade) {
        var gradeOrder = new[] { "F", "D", "C", "B", "A", "A+" };
        var actualIndex = Array.IndexOf(gradeOrder, actualGrade);
        var minimumIndex = Array.IndexOf(gradeOrder, minimumGrade);

        return actualIndex >= minimumIndex;
    }

    /// <summary>
    /// Displays security issues found during assessment
    /// </summary>
    private void DisplaySecurityIssues(SecurityAssessment assessment) {
        OutputFormatter.WriteLine();
        OutputFormatter.WriteError("🚨 Security Issues Detected:");
        OutputFormatter.WriteLine();

        foreach (var packageInfo in assessment.PackageSecurityInfo.Where(p => p.Issues.Any() || p.CriticalIssues.Any())) {
            OutputFormatter.WriteWarning($"Package: {packageInfo.PackageName}@{packageInfo.Version}");

            foreach (var criticalIssue in packageInfo.CriticalIssues) {
                OutputFormatter.WriteError($"  🔴 CRITICAL: {criticalIssue}");
            }

            foreach (var highIssue in packageInfo.HighIssues) {
                OutputFormatter.WriteError($"  🟠 HIGH: {highIssue}");
            }

            foreach (var issue in packageInfo.Issues) {
                OutputFormatter.WriteWarning($"  ⚠️  {issue}");
            }

            OutputFormatter.WriteLine();
        }

        foreach (var error in assessment.Errors) {
            OutputFormatter.WriteError($"❌ {error}");
        }
    }

    /// <summary>
    /// Displays enhanced installation plan with security information
    /// </summary>
    private async Task DisplayEnhancedInstallationPlanWithSecurityAsync(
        List<ResolvedPackage> packages,
        SecurityAssessment securityAssessment,
        bool dev,
        bool global,
        bool nonInteractive) {

        await DisplayEnhancedInstallationPlanAsync(packages, [], dev, global, nonInteractive);

        if (securityAssessment.PackageSecurityInfo.Any()) {
            OutputFormatter.WriteLine();
            OutputFormatter.WriteInfo("Security Assessment Summary:");

            var securityTable = new Table();
            securityTable.AddColumn("Package");
            securityTable.AddColumn("Trust Tier");
            securityTable.AddColumn("Security Grade");
            securityTable.AddColumn("Vulnerabilities");
            securityTable.AddColumn("Status");

            foreach (var packageInfo in securityAssessment.PackageSecurityInfo) {
                var trustTier = packageInfo.TrustTierAssessment?.CurrentTier.ToString() ?? "Unknown";
                var securityGrade = packageInfo.SecurityGrade ?? "N/A";
                var vulnCount = packageInfo.SecurityScanResult?.VulnerabilityCount.ToString() ?? "N/A";
                var status = packageInfo.Issues.Any() || packageInfo.CriticalIssues.Any() ? "[red]Issues[/]" : "[green]Clean[/]";

                securityTable.AddRow(
                    packageInfo.PackageName,
                    GetTrustTierMarkup(trustTier),
                    GetSecurityGradeMarkup(securityGrade),
                    vulnCount,
                    status);
            }

            AnsiConsole.Write(securityTable);
        }
    }

    /// <summary>
    /// Displays security summary for dry run
    /// </summary>
    private void DisplaySecuritySummary(SecurityAssessment assessment) {
        OutputFormatter.WriteLine();
        OutputFormatter.WriteInfo("=== Security Summary ===");

        var totalPackages = assessment.PackageSecurityInfo.Count;
        var packagesWithIssues = assessment.PackageSecurityInfo.Count(p => p.Issues.Any() || p.CriticalIssues.Any());
        var criticalIssues = assessment.PackageSecurityInfo.Sum(p => p.CriticalIssues.Count);
        var highIssues = assessment.PackageSecurityInfo.Sum(p => p.HighIssues.Count);

        OutputFormatter.WriteInfo($"Total packages assessed: {totalPackages}");
        OutputFormatter.WriteInfo($"Packages with issues: {packagesWithIssues}");

        if (criticalIssues > 0) {
            OutputFormatter.WriteError($"Critical security issues: {criticalIssues}");
        }

        if (highIssues > 0) {
            OutputFormatter.WriteWarning($"High severity issues: {highIssues}");
        }

        if (assessment.IsApproved) {
            OutputFormatter.WriteSuccess("✅ Security assessment: APPROVED");
        }
        else {
            OutputFormatter.WriteError("❌ Security assessment: BLOCKED");
        }
    }

    private static string GetTrustTierMarkup(string trustTier) => trustTier switch {
        "Enterprise" => $"[green]{trustTier}[/]",
        "Professional" => $"[blue]{trustTier}[/]",
        "Community" => $"[yellow]{trustTier}[/]",
        "Unverified" => $"[red]{trustTier}[/]",
        _ => trustTier
    };

    private static string GetSecurityGradeMarkup(string grade) => grade switch {
        "A+" or "A" => $"[green]{grade}[/]",
        "B" => $"[yellow]{grade}[/]",
        "C" or "D" or "F" => $"[red]{grade}[/]",
        _ => grade
    };

    private async Task<InstallationResult> InstallPackagesAsync(List<ResolvedPackage> packages, bool dev, bool global) {
        var result = new InstallationResult();

        await AnsiConsole.Progress()
            .StartAsync(async ctx => {
                var overallTask = ctx.AddTask("Installing packages", maxValue: packages.Count);

                foreach (var package in packages) {
                    var packageTask = ctx.AddTask($"Installing {package.Name}@{package.Version}");

                    try {
                        // Get download URL
                        var downloadRequest = new DownloadPackageRequest {
                            UserAgent = "mcpm-cli/1.0.0",
                            DownloadMethod = "CLI",
                            ClientVersion = "1.0.0"
                        };

                        var downloadResponse = await ApiClient.DownloadPackageAsync(package.Name, package.Version, downloadRequest);
                        packageTask.Increment(25);

                        // Download and extract package
                        var progress = new Progress<DownloadProgress>(p => packageTask.Value = 25 + (p.ProgressPercentage * 0.5));

                        var installPath = await _packageManager.DownloadAndExtractPackageAsync(
                            downloadResponse, package.Name, package.Version, global, progress);
                        packageTask.Increment(25);

                        // Register installation
                        await _packageManager.RegisterPackageAsync(
                            package.Name, package.Version, installPath, package.PackageInfo, global, dev);

                        // Record installation in API
                        var installRequest = new InstallPackageRequest {
                            InstallationPath = installPath,
                            ClientVersion = "1.0.0",
                            IsGlobal = global,
                            IsDevelopmentDependency = dev
                        };

                        await ApiClient.InstallPackageAsync(package.Name, package.Version, installRequest);

                        packageTask.Increment(25);
                        result.InstalledCount++;

                        Logger.LogInformation("Successfully installed {PackageName}@{Version}", package.Name, package.Version);
                    }
                    catch (Exception ex) {
                        Logger.LogError(ex, "Failed to install {PackageName}@{Version}", package.Name, package.Version);
                        result.Errors.Add($"Failed to install {package.Name}@{package.Version}: {ex.Message}");
                        packageTask.StopTask();
                    }

                    overallTask.Increment(1);
                }
            });

        result.Success = result.InstalledCount > 0;
        return result;
    }

    private static TrustTier? ParseTrustTierEnum(string? trustTier) => string.IsNullOrWhiteSpace(trustTier)
            ? null
            : trustTier.ToLowerInvariant() switch {
                "unverified" => TrustTier.Unverified,
                "community" => TrustTier.Community,
                "professional" => TrustTier.Professional,
                "enterprise" => TrustTier.Enterprise,
                _ => null
            };
}

/// <summary>
/// Installation result
/// </summary>
public class InstallationResult {
    public bool Success { get; set; }
    public int InstalledCount { get; set; }
    public List<string> Errors { get; set; } = [];
}

/// <summary>
/// Security requirements for package installation
/// </summary>
public record SecurityRequirements {
    public TrustTier? MinimumTrustTier { get; init; }
    public string? MinimumSecurityGrade { get; init; }
    public bool AllowVulnerabilities { get; init; }
    public bool PerformSecurityScan { get; init; }
}

/// <summary>
/// Comprehensive security assessment for installation
/// </summary>
public class SecurityAssessment {
    public bool IsApproved { get; set; }
    public List<PackageSecurityInfo> PackageSecurityInfo { get; set; } = [];
    public List<string> Errors { get; set; } = [];
}

/// <summary>
/// Security information for a specific package
/// </summary>
public class PackageSecurityInfo {
    public string PackageName { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public TrustTierAssessment? TrustTierAssessment { get; set; }
    public SecurityScanResult? SecurityScanResult { get; set; }
    public string? SecurityGrade { get; set; }
    public List<string> Issues { get; set; } = [];
    public List<string> CriticalIssues { get; set; } = [];
    public List<string> HighIssues { get; set; } = [];
}