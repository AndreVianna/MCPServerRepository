using System.CommandLine;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using MCPHub.CommandLineApp.Configuration;
using MCPHub.CommandLineApp.Models;
using MCPHub.CommandLineApp.Services;
using MCPHub.CommandLineApp.Utilities;
using MCPHub.Domain.ValueObjects;

namespace MCPHub.CommandLineApp.Commands;

/// <summary>
/// Command for publishing MCP packages to the registry
/// </summary>
public class PublishCommand : BaseCommand
{
    private const string DefaultManifestFileName = "mcp-manifest.json";
    private const string DefaultReadmeFileName = "README.md";
    private const string DefaultChangelogFileName = "CHANGELOG.md";
    
    public PublishCommand(
        ILogger<PublishCommand> logger,
        McpmConfiguration configuration,
        IMcpHubApiClient apiClient,
        IOutputFormatter outputFormatter)
        : base(logger, configuration, apiClient, outputFormatter)
    {
    }

    /// <inheritdoc />
    public override Command CreateCommand()
    {
        var command = new Command("publish", "Publish an MCP package to the registry");

        // Options
        var manifestOption = new Option<string?>(
            aliases: ["--manifest", "-m"],
            description: $"Path to manifest file (default: {DefaultManifestFileName})");
        command.AddOption(manifestOption);

        var tagsOption = new Option<string?>(
            aliases: ["--tags", "-t"],
            description: "Comma-separated tags for the package");
        command.AddOption(tagsOption);

        var readmeOption = new Option<string?>(
            aliases: ["--readme", "-r"],
            description: $"Path to README file (default: {DefaultReadmeFileName})");
        command.AddOption(readmeOption);

        var changelogOption = new Option<string?>(
            aliases: ["--changelog", "-c"],
            description: $"Path to changelog file (default: {DefaultChangelogFileName})");
        command.AddOption(changelogOption);

        var packageUrlOption = new Option<string?>(
            aliases: ["--package-url", "-u"],
            description: "URL to package archive (instead of local files)");
        command.AddOption(packageUrlOption);

        var dryRunOption = new Option<bool>(
            aliases: ["--dry-run", "-n"],
            description: "Validate and show what would be published without actually publishing");
        command.AddOption(dryRunOption);

        var skipValidationOption = new Option<bool>(
            aliases: ["--skip-validation"],
            description: "Skip interactive manifest validation");
        command.AddOption(skipValidationOption);

        var yesOption = new Option<bool>(
            aliases: ["--yes", "-y"],
            description: "Skip confirmation prompts");
        command.AddOption(yesOption);

        // Handler
        command.SetHandler(
            ExecuteAsync,
            manifestOption,
            tagsOption,
            readmeOption,
            changelogOption,
            packageUrlOption,
            dryRunOption,
            skipValidationOption,
            yesOption);

        return command;
    }

    private async Task<int> ExecuteAsync(
        string? manifestPath,
        string? tags,
        string? readmePath,
        string? changelogPath,
        string? packageUrl,
        bool dryRun,
        bool skipValidation,
        bool yes)
    {
        try
        {
            Logger.LogInformation("Executing publish command: DryRun={DryRun}, SkipValidation={SkipValidation}", 
                dryRun, skipValidation);

            // Check API connectivity first
            if (!await ValidateApiConnectivityAsync())
            {
                return 503; // Service unavailable
            }

            // Check authentication
            if (string.IsNullOrEmpty(Configuration.Auth.Token))
            {
                OutputFormatter.WriteError("Authentication required. Please run 'mcpm login' first.");
                return 401;
            }

            // Resolve file paths
            var resolvedManifestPath = ResolveManifestPath(manifestPath);
            var resolvedReadmePath = ResolveOptionalFile(readmePath, DefaultReadmeFileName);
            var resolvedChangelogPath = ResolveOptionalFile(changelogPath, DefaultChangelogFileName);

            // Load and validate manifest
            OutputFormatter.WriteInfo($"Loading manifest from: {resolvedManifestPath}");
            var manifestContent = await LoadManifestAsync(resolvedManifestPath);
            
            if (!skipValidation)
            {
                var validationResult = await ValidateManifestWithFeedbackAsync(manifestContent);
                if (!validationResult.IsValid)
                {
                    OutputFormatter.WriteError("Manifest validation failed. Fix the errors and try again.");
                    return 422;
                }

                if (validationResult.Warnings.Count > 0 && !yes)
                {
                    if (!ConfirmWithWarnings(validationResult.Warnings))
                    {
                        OutputFormatter.WriteInfo("Publishing cancelled by user.");
                        return 0;
                    }
                }
            }

            // Parse manifest to get package info
            var manifest = ParseManifest(manifestContent);
            if (manifest == null)
            {
                OutputFormatter.WriteError("Failed to parse manifest file.");
                return 422;
            }

            // Load additional files
            var readmeContent = await LoadOptionalFileAsync(resolvedReadmePath, "README");
            var changelogContent = await LoadOptionalFileAsync(resolvedChangelogPath, "changelog");

            // Parse tags
            var parsedTags = ParseTags(tags);

            // Create publish request
            var publishRequest = await CreatePublishRequestAsync(
                manifestContent, 
                parsedTags, 
                readmeContent, 
                changelogContent, 
                packageUrl);

            // Show summary
            ShowPublishSummary(manifest, parsedTags, dryRun);

            // Confirm publishing
            if (!yes && !dryRun)
            {
                if (!ConfirmPublishing(manifest))
                {
                    OutputFormatter.WriteInfo("Publishing cancelled by user.");
                    return 0;
                }
            }

            if (dryRun)
            {
                OutputFormatter.WriteSuccess("Dry run completed. Package would be published successfully.");
                return 0;
            }

            // Publish package
            var result = await PublishPackageAsync(publishRequest, manifest);
            
            if (result.Success)
            {
                ShowPublishSuccess(result, manifest);
                return 0;
            }
            else
            {
                ShowPublishFailure(result);
                return 1;
            }
        }
        catch (Exception ex)
        {
            return HandleError(ex, "publish");
        }
    }

    private string ResolveManifestPath(string? manifestPath)
    {
        var path = manifestPath ?? DefaultManifestFileName;
        
        if (!Path.IsPathRooted(path))
        {
            path = Path.Combine(Environment.CurrentDirectory, path);
        }

        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Manifest file not found: {path}");
        }

        return path;
    }

    private string? ResolveOptionalFile(string? filePath, string defaultFileName)
    {
        if (!string.IsNullOrEmpty(filePath))
        {
            var path = Path.IsPathRooted(filePath) ? filePath : Path.Combine(Environment.CurrentDirectory, filePath);
            return File.Exists(path) ? path : null;
        }

        var defaultPath = Path.Combine(Environment.CurrentDirectory, defaultFileName);
        return File.Exists(defaultPath) ? defaultPath : null;
    }

    private async Task<string> LoadManifestAsync(string manifestPath)
    {
        try
        {
            var content = await File.ReadAllTextAsync(manifestPath);
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new InvalidOperationException("Manifest file is empty");
            }
            return content;
        }
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            throw new FileNotFoundException($"Failed to read manifest file: {ex.Message}");
        }
    }

    private async Task<string?> LoadOptionalFileAsync(string? filePath, string fileType)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            return null;
        }

        try
        {
            var content = await File.ReadAllTextAsync(filePath);
            OutputFormatter.WriteInfo($"Loaded {fileType} from: {filePath}");
            return content;
        }
        catch (Exception ex)
        {
            OutputFormatter.WriteWarning($"Failed to load {fileType} file '{filePath}': {ex.Message}");
            return null;
        }
    }

    private async Task<ValidateManifestResponse> ValidateManifestWithFeedbackAsync(string manifestContent)
    {
        var validationRequest = new ValidateManifestRequest
        {
            ManifestContent = manifestContent,
            IncludeWarnings = true
        };

        var result = await WithProgressAsync(
            "Validating manifest...",
            () => ApiClient.ValidateManifestAsync(validationRequest));

        if (result.Errors.Count > 0)
        {
            OutputFormatter.WriteError("Manifest validation errors:");
            foreach (var error in result.Errors)
            {
                OutputFormatter.WriteError($"  - {error}");
            }
        }

        if (result.Warnings.Count > 0)
        {
            OutputFormatter.WriteWarning("Manifest validation warnings:");
            foreach (var warning in result.Warnings)
            {
                OutputFormatter.WriteWarning($"  - {warning}");
            }
        }

        if (result.IsValid && result.Errors.Count == 0 && result.Warnings.Count == 0)
        {
            OutputFormatter.WriteSuccess("Manifest validation passed!");
        }

        return result;
    }

    private MCPManifest? ParseManifest(string manifestContent)
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<MCPManifest>(manifestContent, options);
        }
        catch (JsonException ex)
        {
            OutputFormatter.WriteError($"Failed to parse manifest: {ex.Message}");
            return null;
        }
    }

    private List<string> ParseTags(string? tags)
    {
        if (string.IsNullOrWhiteSpace(tags))
        {
            return new List<string>();
        }

        return tags.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(t => t.Trim())
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private async Task<PublishPackageRequest> CreatePublishRequestAsync(
        string manifestContent,
        List<string> tags,
        string? readmeContent,
        string? changelogContent,
        string? packageUrl)
    {
        var request = new PublishPackageRequest
        {
            ManifestContent = manifestContent,
            Tags = tags,
            ReadmeContent = readmeContent,
            ChangelogContent = changelogContent,
            PackageUrl = packageUrl
        };

        // If no package URL provided, create archive from current directory
        if (string.IsNullOrEmpty(packageUrl))
        {
            request.PackageArchive = await CreatePackageArchiveAsync();
        }

        return request;
    }

    private async Task<string> CreatePackageArchiveAsync()
    {
        // For now, create a simple tar.gz archive of the current directory
        // In a real implementation, this would create a proper package archive
        OutputFormatter.WriteInfo("Creating package archive from current directory...");
        
        // This is a placeholder - in real implementation, you'd create a tar.gz or zip
        var currentDir = Environment.CurrentDirectory;
        var files = Directory.GetFiles(currentDir, "*", SearchOption.AllDirectories)
            .Where(f => !IsExcludedFile(f))
            .ToList();

        // Create a simple base64 encoded representation
        var fileData = new Dictionary<string, string>();
        foreach (var file in files)
        {
            var relativePath = Path.GetRelativePath(currentDir, file);
            var content = await File.ReadAllTextAsync(file);
            fileData[relativePath] = Convert.ToBase64String(Encoding.UTF8.GetBytes(content));
        }

        var archiveJson = JsonSerializer.Serialize(fileData);
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(archiveJson));
    }

    private bool IsExcludedFile(string filePath)
    {
        var fileName = Path.GetFileName(filePath);
        var relativePath = Path.GetRelativePath(Environment.CurrentDirectory, filePath);
        
        // Exclude common development files
        var excludePatterns = new[]
        {
            ".git/", "node_modules/", ".vs/", ".vscode/", "bin/", "obj/",
            ".gitignore", ".gitattributes", ".mcpmignore"
        };

        return excludePatterns.Any(pattern => 
            relativePath.StartsWith(pattern, StringComparison.OrdinalIgnoreCase) ||
            fileName.Equals(pattern.TrimEnd('/'), StringComparison.OrdinalIgnoreCase));
    }

    private void ShowPublishSummary(MCPManifest manifest, List<string> tags, bool dryRun)
    {
        var table = new Table()
            .Title(dryRun ? "[yellow]Package Publishing Summary (Dry Run)[/]" : "[green]Package Publishing Summary[/]")
            .Border(TableBorder.Rounded)
            .AddColumn("Property")
            .AddColumn("Value");

        table.AddRow("Package Name", $"[cyan]{manifest.Name}[/]");
        table.AddRow("Version", $"[yellow]{manifest.Version}[/]");
        table.AddRow("Description", manifest.Description ?? "N/A");
        table.AddRow("Author", manifest.Author?.Name ?? "N/A");
        table.AddRow("License", manifest.License ?? "N/A");
        
        if (tags.Count > 0)
        {
            table.AddRow("Tags", string.Join(", ", tags.Select(t => $"[blue]{t}[/]")));
        }

        if (manifest.Capabilities != null)
        {
            var capabilities = new List<string>();
            if (manifest.Capabilities.Tools?.Any() == true)
                capabilities.Add($"{manifest.Capabilities.Tools.Count()} tools");
            if (manifest.Capabilities.Resources?.Any() == true)
                capabilities.Add($"{manifest.Capabilities.Resources.Count()} resources");
            if (manifest.Capabilities.Prompts?.Any() == true)
                capabilities.Add($"{manifest.Capabilities.Prompts.Count()} prompts");
            
            if (capabilities.Count > 0)
            {
                table.AddRow("Capabilities", string.Join(", ", capabilities));
            }
        }

        AnsiConsole.Write(table);
        OutputFormatter.WriteLine();
    }

    private bool ConfirmWithWarnings(List<string> warnings)
    {
        OutputFormatter.WriteWarning($"Found {warnings.Count} warning(s). Continue with publishing?");
        return AnsiConsole.Confirm("Proceed with publishing?", false);
    }

    private bool ConfirmPublishing(MCPManifest manifest)
    {
        return AnsiConsole.Confirm($"Publish package '{manifest.Name}' version '{manifest.Version}' to the registry?", false);
    }

    private async Task<PublishPackageResponse> PublishPackageAsync(PublishPackageRequest request, MCPManifest manifest)
    {
        return await WithProgressAsync(
            $"Publishing package '{manifest.Name}' version '{manifest.Version}'...",
            () => ApiClient.PublishPackageAsync(request));
    }

    private void ShowPublishSuccess(PublishPackageResponse result, MCPManifest manifest)
    {
        OutputFormatter.WriteSuccess($"Package '{manifest.Name}' version '{manifest.Version}' published successfully!");
        OutputFormatter.WriteLine();
        
        if (result.Package != null)
        {
            OutputFormatter.WriteInfo($"Package ID: {result.Package.Id}");
            OutputFormatter.WriteInfo($"Status: {result.Package.Status}");
        }
        
        if (!string.IsNullOrEmpty(result.StorageUrl))
        {
            OutputFormatter.WriteInfo($"Package URL: {result.StorageUrl}");
        }
        
        OutputFormatter.WriteInfo($"Published at: {result.PublishedAt:yyyy-MM-dd HH:mm:ss} UTC");
        OutputFormatter.WriteInfo($"Processing time: {result.PublishTimeMs}ms");

        if (result.Warnings.Count > 0)
        {
            OutputFormatter.WriteLine();
            OutputFormatter.WriteWarning("Warnings:");
            foreach (var warning in result.Warnings)
            {
                OutputFormatter.WriteWarning($"  - {warning}");
            }
        }

        OutputFormatter.WriteLine();
        OutputFormatter.WriteInfo("Your package will be available in the registry shortly.");
        OutputFormatter.WriteInfo($"You can view it with: mcpm info {manifest.Name}");
    }

    private void ShowPublishFailure(PublishPackageResponse result)
    {
        OutputFormatter.WriteError("Package publishing failed!");
        
        if (result.Errors.Count > 0)
        {
            OutputFormatter.WriteError("Errors:");
            foreach (var error in result.Errors)
            {
                OutputFormatter.WriteError($"  - {error}");
            }
        }

        if (result.Warnings.Count > 0)
        {
            OutputFormatter.WriteWarning("Warnings:");
            foreach (var warning in result.Warnings)
            {
                OutputFormatter.WriteWarning($"  - {warning}");
            }
        }
    }
}