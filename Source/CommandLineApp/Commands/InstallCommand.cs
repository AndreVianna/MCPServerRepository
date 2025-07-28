using System.CommandLine;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using MCPHub.CommandLineApp.Configuration;
using MCPHub.CommandLineApp.Models;
using MCPHub.CommandLineApp.Services;
using MCPHub.CommandLineApp.Utilities;

namespace MCPHub.CommandLineApp.Commands;

/// <summary>
/// Command for installing MCP packages
/// </summary>
public class InstallCommand : BaseCommand
{
    private readonly PackageManager _packageManager;
    private readonly DependencyResolver _dependencyResolver;

    public InstallCommand(
        ILogger<InstallCommand> logger,
        McpmConfiguration configuration,
        IMcpHubApiClient apiClient,
        IOutputFormatter outputFormatter,
        PackageManager packageManager,
        DependencyResolver dependencyResolver)
        : base(logger, configuration, apiClient, outputFormatter)
    {
        _packageManager = packageManager ?? throw new ArgumentNullException(nameof(packageManager));
        _dependencyResolver = dependencyResolver ?? throw new ArgumentNullException(nameof(dependencyResolver));
    }

    /// <inheritdoc />
    public override Command CreateCommand()
    {
        var command = new Command("install", "Install MCP packages with dependency resolution");

        // Arguments
        var packageArgument = new Argument<string>("package", "Package name with optional version (e.g., 'package-name' or 'package-name@1.0.0')");
        command.AddArgument(packageArgument);

        // Options
        var devOption = new Option<bool>(
            aliases: ["--dev", "-D"],
            description: "Install as development dependency");
        command.AddOption(devOption);

        var forceOption = new Option<bool>(
            aliases: ["--force", "-f"],
            description: "Force reinstallation even if already installed");
        command.AddOption(forceOption);

        var skipVerifyOption = new Option<bool>(
            aliases: ["--skip-verify"],
            description: "Skip security verification and trust tier checks");
        command.AddOption(skipVerifyOption);

        var globalOption = new Option<bool>(
            aliases: ["--global", "-g"],
            description: "Install package globally");
        command.AddOption(globalOption);

        var yesOption = new Option<bool>(
            aliases: ["--yes", "-y"],
            description: "Automatically confirm all prompts");
        command.AddOption(yesOption);

        var dryRunOption = new Option<bool>(
            aliases: ["--dry-run"],
            description: "Show what would be installed without actually installing");
        command.AddOption(dryRunOption);

        // Handler
        command.SetHandler(
            ExecuteAsync,
            packageArgument,
            devOption,
            forceOption,
            skipVerifyOption,
            globalOption,
            yesOption,
            dryRunOption);

        return command;
    }

    private async Task<int> ExecuteAsync(
        string package,
        bool dev,
        bool force,
        bool skipVerify,
        bool global,
        bool yes,
        bool dryRun)
    {
        try
        {
            Logger.LogInformation("Executing install command: Package={Package}, Dev={Dev}, Force={Force}, Global={Global}", 
                package, dev, force, global);

            // Parse package name and version
            var (packageName, version) = ParsePackageSpec(package);
            if (string.IsNullOrWhiteSpace(packageName))
            {
                OutputFormatter.WriteError("Package name is required");
                return 400;
            }

            // Check API connectivity
            if (!await ValidateApiConnectivityAsync())
            {
                return 503; // Service unavailable
            }

            // Get package information
            var packageInfo = await WithProgressAsync(
                $"Getting package information for '{packageName}'...",
                () => ApiClient.GetPackageInfoAsync(packageName));

            // Resolve version if not specified
            if (string.IsNullOrEmpty(version))
            {
                version = packageInfo.Version; // Latest version
                OutputFormatter.WriteInfo($"No version specified, using latest: {version}");
            }

            // Check if already installed (unless force)
            if (!force)
            {
                var installedVersion = await _packageManager.GetInstalledPackageAsync(packageName, version, global);
                if (installedVersion != null)
                {
                    OutputFormatter.WriteInfo($"Package '{packageName}@{version}' is already installed");
                    
                    if (!dev || installedVersion.IsDevelopmentDependency)
                    {
                        OutputFormatter.WriteInfo("Use --force to reinstall");
                        return 0;
                    }
                }
            }

            // Resolve dependencies
            var dependencyResult = await WithProgressAsync(
                "Resolving dependencies...",
                () => _dependencyResolver.ResolveDependenciesAsync(packageName, version, dev, global));

            if (!dependencyResult.Success)
            {
                OutputFormatter.WriteError("Failed to resolve dependencies:");
                foreach (var error in dependencyResult.Errors)
                {
                    OutputFormatter.WriteError($"  - {error}");
                }
                return 1;
            }

            // Show warnings
            foreach (var warning in dependencyResult.Warnings)
            {
                OutputFormatter.WriteWarning(warning);
            }

            // Display installation plan
            DisplayInstallationPlan(dependencyResult.PackagesToInstall, dependencyResult.UntrustedPackages, dev, global);

            if (dryRun)
            {
                OutputFormatter.WriteInfo("Dry run completed. No packages were installed.");
                return 0;
            }

            // Security verification
            if (!skipVerify && dependencyResult.UntrustedPackages.Any())
            {
                if (!await ConfirmUntrustedPackagesAsync(dependencyResult.UntrustedPackages, yes))
                {
                    OutputFormatter.WriteInfo("Installation cancelled by user");
                    return 130; // User cancelled
                }
            }

            // Confirm installation
            if (!yes && !await ConfirmInstallationAsync(dependencyResult.PackagesToInstall))
            {
                OutputFormatter.WriteInfo("Installation cancelled by user");
                return 130; // User cancelled
            }

            // Install packages
            var installationResult = await InstallPackagesAsync(dependencyResult.PackagesToInstall, dev, global);

            if (installationResult.Success)
            {
                OutputFormatter.WriteSuccess($"Successfully installed {installationResult.InstalledCount} package(s)");
                
                if (installationResult.Errors.Any())
                {
                    OutputFormatter.WriteWarning("Some packages had installation warnings:");
                    foreach (var error in installationResult.Errors)
                    {
                        OutputFormatter.WriteWarning($"  - {error}");
                    }
                }

                return 0;
            }
            else
            {
                OutputFormatter.WriteError("Installation failed:");
                foreach (var error in installationResult.Errors)
                {
                    OutputFormatter.WriteError($"  - {error}");
                }
                return 1;
            }
        }
        catch (Exception ex)
        {
            return HandleError(ex, "install");
        }
    }

    private (string packageName, string? version) ParsePackageSpec(string packageSpec)
    {
        var parts = packageSpec.Split('@', 2);
        return parts.Length == 2 ? (parts[0], parts[1]) : (parts[0], null);
    }

    private void DisplayInstallationPlan(List<ResolvedPackage> packages, List<UntrustedPackage> untrustedPackages, bool dev, bool global)
    {
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

        foreach (var package in packages.OrderBy(p => p.IsRootPackage ? 0 : 1).ThenBy(p => p.Name))
        {
            var trustTier = package.PackageInfo.TrustTier;
            var securityGrade = package.PackageInfo.SecurityGrade ?? "N/A";
            var packageType = package.IsRootPackage ? "Main" : "Dependency";

            // Color code trust tier
            var trustTierMarkup = trustTier switch
            {
                "Enterprise" => $"[green]{trustTier}[/]",
                "Professional" => $"[blue]{trustTier}[/]",
                "Community" => $"[yellow]{trustTier}[/]",
                "Unverified" => $"[red]{trustTier}[/]",
                _ => trustTier
            };

            // Color code security grade
            var securityMarkup = securityGrade switch
            {
                "A" or "A+" => $"[green]{securityGrade}[/]",
                "B" or "B+" => $"[yellow]{securityGrade}[/]",
                "C" or "D" or "F" => $"[red]{securityGrade}[/]",
                _ => securityGrade
            };

            table.AddRow(package.Name, package.Version, trustTierMarkup, securityMarkup, packageType);
        }

        AnsiConsole.Write(table);

        if (untrustedPackages.Any())
        {
            OutputFormatter.WriteLine();
            OutputFormatter.WriteWarning($"Warning: {untrustedPackages.Count} package(s) do not meet minimum trust tier requirements:");
            foreach (var untrusted in untrustedPackages)
            {
                OutputFormatter.WriteWarning($"  - {untrusted.Name}@{untrusted.Version} (Trust: {untrusted.TrustTier})");
            }
        }

        OutputFormatter.WriteLine();
    }

    private Task<bool> ConfirmUntrustedPackagesAsync(List<UntrustedPackage> untrustedPackages, bool autoConfirm)
    {
        if (autoConfirm)
            return Task.FromResult(true);

        OutputFormatter.WriteLine();
        OutputFormatter.WriteWarning("Security Warning:");
        OutputFormatter.WriteWarning($"The following {untrustedPackages.Count} package(s) do not meet your minimum trust tier requirement:");
        
        foreach (var package in untrustedPackages)
        {
            OutputFormatter.WriteWarning($"  - {package.Name}@{package.Version} (Trust: {package.TrustTier}, Grade: {package.SecurityGrade ?? "N/A"})");
        }

        OutputFormatter.WriteLine();
        OutputFormatter.WriteWarning("Installing these packages may pose security risks.");
        
        return Task.FromResult(AnsiConsole.Confirm("Do you want to continue with the installation?", false));
    }

    private Task<bool> ConfirmInstallationAsync(List<ResolvedPackage> packages)
    {
        var totalSize = packages.Sum(p => p.VersionInfo.DownloadCount); // Approximate
        OutputFormatter.WriteLine();
        OutputFormatter.WriteInfo($"About to install {packages.Count} package(s)");
        
        return Task.FromResult(AnsiConsole.Confirm("Continue with installation?", true));
    }

    private async Task<InstallationResult> InstallPackagesAsync(List<ResolvedPackage> packages, bool dev, bool global)
    {
        var result = new InstallationResult();
        
        await AnsiConsole.Progress()
            .StartAsync(async ctx =>
            {
                var overallTask = ctx.AddTask("Installing packages", maxValue: packages.Count);
                
                foreach (var package in packages)
                {
                    var packageTask = ctx.AddTask($"Installing {package.Name}@{package.Version}");
                    
                    try
                    {
                        // Get download URL
                        var downloadRequest = new DownloadPackageRequest
                        {
                            UserAgent = "mcpm-cli/1.0.0",
                            DownloadMethod = "CLI",
                            ClientVersion = "1.0.0"
                        };

                        var downloadResponse = await ApiClient.DownloadPackageAsync(package.Name, package.Version, downloadRequest);
                        packageTask.Increment(25);

                        // Download and extract package
                        var progress = new Progress<DownloadProgress>(p =>
                        {
                            packageTask.Value = 25 + (p.ProgressPercentage * 0.5); // 25-75%
                        });

                        var installPath = await _packageManager.DownloadAndExtractPackageAsync(
                            downloadResponse, package.Name, package.Version, global, progress);
                        packageTask.Increment(25);

                        // Register installation
                        await _packageManager.RegisterPackageAsync(
                            package.Name, package.Version, installPath, package.PackageInfo, global, dev);

                        // Record installation in API
                        var installRequest = new InstallPackageRequest
                        {
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
                    catch (Exception ex)
                    {
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
}

/// <summary>
/// Installation result
/// </summary>
public class InstallationResult
{
    public bool Success { get; set; }
    public int InstalledCount { get; set; }
    public List<string> Errors { get; set; } = new();
}