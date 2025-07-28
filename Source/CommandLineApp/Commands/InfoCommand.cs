using System.CommandLine;
using Microsoft.Extensions.Logging;
using MCPHub.CommandLineApp.Configuration;
using MCPHub.CommandLineApp.Services;
using MCPHub.CommandLineApp.Utilities;

namespace MCPHub.CommandLineApp.Commands;

/// <summary>
/// Command for displaying detailed information about a specific package
/// </summary>
public class InfoCommand : BaseCommand
{
    public InfoCommand(
        ILogger<InfoCommand> logger,
        McpmConfiguration configuration,
        IMcpHubApiClient apiClient,
        IOutputFormatter outputFormatter)
        : base(logger, configuration, apiClient, outputFormatter)
    {
    }

    /// <inheritdoc />
    public override Command CreateCommand()
    {
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
        bool showVersions)
    {
        try
        {
            Logger.LogInformation("Executing info command: PackageName={PackageName}, Version={Version}", 
                packageName, version);

            // Validate input
            if (string.IsNullOrWhiteSpace(packageName))
            {
                OutputFormatter.WriteError("Package name is required");
                return 400;
            }

            var outputFormat = ValidateOutputFormat(format);

            // Check API connectivity
            if (!await ValidateApiConnectivityAsync())
            {
                return 503; // Service unavailable
            }

            // Get basic package information
            var packageInfo = await WithProgressAsync(
                $"Getting information for '{packageName}'...",
                () => ApiClient.GetPackageInfoAsync(packageName));

            // Display basic package info
            OutputFormatter.WritePackageInfo(packageInfo, outputFormat);

            // Show additional information if requested
            if (showSecurity || showTrust || showVersions)
            {
                OutputFormatter.WriteLine();

                if (showSecurity)
                {
                    try
                    {
                        var securitySummary = await WithProgressAsync(
                            "Getting security information...",
                            () => ApiClient.GetPackageSecuritySummaryAsync(packageName));

                        OutputFormatter.WriteSecuritySummary(securitySummary, outputFormat);
                        OutputFormatter.WriteLine();
                    }
                    catch (Exception ex)
                    {
                        Logger.LogWarning(ex, "Failed to get security information for package: {PackageName}", packageName);
                        OutputFormatter.WriteWarning("Security information not available");
                    }
                }

                if (showTrust)
                {
                    try
                    {
                        var trustTier = await WithProgressAsync(
                            "Getting trust tier assessment...",
                            () => ApiClient.GetPackageTrustTierAsync(packageName));

                        OutputFormatter.WriteTrustTier(trustTier, outputFormat);
                        OutputFormatter.WriteLine();
                    }
                    catch (Exception ex)
                    {
                        Logger.LogWarning(ex, "Failed to get trust tier information for package: {PackageName}", packageName);
                        OutputFormatter.WriteWarning("Trust tier information not available");
                    }
                }

                if (showVersions)
                {
                    try
                    {
                        var versions = await WithProgressAsync(
                            "Getting version history...",
                            () => ApiClient.GetPackageVersionsAsync(packageName, includePrerelease: true));

                        if (versions.Versions.Any())
                        {
                            OutputFormatter.WritePackageVersions(versions, outputFormat);
                        }
                        else
                        {
                            OutputFormatter.WriteInfo("No version history available");
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogWarning(ex, "Failed to get version history for package: {PackageName}", packageName);
                        OutputFormatter.WriteWarning("Version history not available");
                    }
                }
            }

            // Show usage information
            if (outputFormat != "json")
            {
                OutputFormatter.WriteLine();
                OutputFormatter.WriteInfo($"Use 'mcpm install {packageName}' to install this package");
                
                if (showSecurity || showTrust)
                {
                    OutputFormatter.WriteInfo($"Use 'mcpm verify {packageName}' to run security verification");
                }
            }

            Logger.LogInformation("Info command completed successfully for package: {PackageName}", packageName);
            return 0;
        }
        catch (PackageNotFoundException)
        {
            OutputFormatter.WriteError($"Package '{packageName}' not found");
            OutputFormatter.WriteInfo("Use 'mcpm search' to find available packages");
            return 404;
        }
        catch (Exception ex)
        {
            return HandleError(ex, "info");
        }
    }
}