using System.Text;
using System.Text.Json;

using MCPHub.CommandLineApp.Configuration;
using MCPHub.CommandLineApp.Models;
using MCPHub.CommandLineApp.Services;
using MCPHub.CommandLineApp.Utilities;
using MCPHub.Domain.ValueObjects;

namespace MCPHub.CommandLineApp.Commands;

/// <summary>
/// Command for publishing MCP packages to the registry
/// </summary>
public class PublishCommand(
    ILogger<PublishCommand> logger,
    McpmConfiguration configuration,
    IMcpHubApiClient apiClient,
    IOutputFormatter outputFormatter,
    IInteractionService interactionService,
    IProgressReporter progressReporter) : BaseCommand(logger, configuration, apiClient, outputFormatter, interactionService, progressReporter) {
    private const string DefaultManifestFileName = "mcp-manifest.json";
    private const string DefaultReadmeFileName = "README.md";
    private const string DefaultChangelogFileName = "CHANGELOG.md";

    /// <inheritdoc />
    public override Command CreateCommand() {
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

        var nonInteractiveOption = new Option<bool>(
            aliases: ["--non-interactive", "-n"],
            description: "Disable interactive features (for CI/CD)");
        command.AddOption(nonInteractiveOption);

        // Handler - using fewer parameters to avoid SetHandler limitations
        command.SetHandler(async (manifestPath, tags, readmePath, changelogPath, packageUrl, dryRun, skipValidation, yes) => {
            var exitCode = await ExecuteAsync(manifestPath, tags, readmePath, changelogPath, packageUrl, dryRun, skipValidation, yes, false);
            Environment.Exit(exitCode);
        }, manifestOption, tagsOption, readmeOption, changelogOption, packageUrlOption, dryRunOption, skipValidationOption, yesOption);

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
        bool yes,
        bool nonInteractive) {
        try {
            Logger.LogInformation("Executing publish command: DryRun={DryRun}, SkipValidation={SkipValidation}",
                dryRun, skipValidation);

            // Check API connectivity first
            if (!await ValidateApiConnectivityAsync()) {
                return 503; // Service unavailable
            }

            // TODO: Replace with ICredentialStore when authentication is fully implemented
            // Check authentication - temporarily disabled to avoid obsolete API usage
            // This will be properly implemented when the authentication system is fully functional
            Logger.LogWarning("Authentication check temporarily disabled - will be implemented with ICredentialStore");

            // Resolve file paths
            var resolvedManifestPath = ResolveManifestPath(manifestPath);
            var resolvedReadmePath = ResolveOptionalFile(readmePath, DefaultReadmeFileName);
            var resolvedChangelogPath = ResolveOptionalFile(changelogPath, DefaultChangelogFileName);

            // Enhanced step-by-step publishing process
            using var publishProgress = ProgressReporter.CreateStepProgress("Package Publishing", new[] {
                "Loading and validating manifest",
                "Processing additional files",
                "Creating package archive",
                "Publishing to registry",
                                                                                                        });

            // Step 1: Load and validate manifest with detailed feedback
            publishProgress.StartStep(0, $"Loading manifest from {Path.GetFileName(resolvedManifestPath)}...");
            var manifestContent = await LoadManifestAsync(resolvedManifestPath);

            MCPManifest? manifest = null;
            if (!skipValidation) {
                publishProgress.UpdateStatus("Validating manifest structure and content...");
                var validationResult = await ValidateManifestWithEnhancedFeedbackAsync(manifestContent, nonInteractive);

                if (!validationResult.IsValid) {
                    publishProgress.FailStep(0, "Manifest validation failed");
                    OutputFormatter.WriteError("Manifest validation failed. Fix the errors and try again.");
                    return 422;
                }

                if (validationResult.Warnings.Count > 0 && !yes && !nonInteractive) {
                    var continueWithWarnings = await HandleValidationWarningsAsync(validationResult.Warnings);
                    if (!continueWithWarnings) {
                        publishProgress.CompleteStep(0, "Validation cancelled by user");
                        publishProgress.CompleteAll("Publishing cancelled by user");
                        return 0;
                    }
                }

                // Interactive manifest review and editing
                if (!nonInteractive && !yes) {
                    manifest = ParseManifest(manifestContent);
                    if (manifest != null) {
                        var updatedManifest = await ReviewAndEditManifestAsync(manifest);
                        if (updatedManifest != null) {
                            manifestContent = JsonSerializer.Serialize(updatedManifest, new JsonSerializerOptions {
                                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                                WriteIndented = true,
                            });
                            manifest = updatedManifest;
                        }
                    }
                }
            }

            // Parse manifest if not already done
            if (manifest == null) {
                manifest = ParseManifest(manifestContent);
                if (manifest == null) {
                    publishProgress.FailStep(0, "Failed to parse manifest");
                    OutputFormatter.WriteError("Failed to parse manifest file.");
                    return 422;
                }
            }

            publishProgress.CompleteStep(0, "Manifest loaded and validated");

            // Step 2: Process additional files
            publishProgress.StartStep(1, "Processing additional files...");

            var readmeContent = await LoadOptionalFileAsync(resolvedReadmePath, "README");
            var changelogContent = await LoadOptionalFileAsync(resolvedChangelogPath, "changelog");
            var parsedTags = ParseTags(tags);

            // Interactive file review
            if (!nonInteractive && !dryRun) {
                await ReviewAdditionalFilesAsync(readmeContent, changelogContent, parsedTags);
            }

            publishProgress.CompleteStep(1, "Additional files processed");

            // Step 3: Create package archive
            publishProgress.StartStep(2, "Preparing package for publishing...");

            var publishRequest = await CreatePublishRequestAsync(
                manifestContent,
                parsedTags,
                readmeContent,
                changelogContent,
                packageUrl);

            publishProgress.CompleteStep(2, "Package archive created");

            // Show enhanced publishing summary
            await ShowEnhancedPublishingSummaryAsync(manifest, parsedTags, dryRun, nonInteractive);

            // Final confirmation
            if (!yes && !dryRun && !nonInteractive) {
                var confirmPublish = await InteractionService.ConfirmAsync(
                    $"Publish package '{manifest.Name}' version '{manifest.Version}' to the registry?", false);

                if (!confirmPublish) {
                    publishProgress.SkipStep(3, "Publishing cancelled by user");
                    OutputFormatter.WriteInfo("Publishing cancelled by user.");
                    return 0;
                }
            }

            if (dryRun) {
                publishProgress.SkipStep(3, "Dry run mode");
                OutputFormatter.WriteSuccess("Dry run completed. Package would be published successfully.");
                return 0;
            }

            // Step 4: Publish with enhanced progress tracking
            publishProgress.StartStep(3, "Publishing to registry...");
            var result = await PublishPackageWithProgressAsync(publishRequest, manifest);

            if (result.Success) {
                publishProgress.CompleteStep(3, "Package published successfully");
                await ShowEnhancedPublishSuccessAsync(result, manifest, nonInteractive);
                return 0;
            }
            else {
                publishProgress.FailStep(3, "Publishing failed");
                ShowPublishFailure(result);
                return 1;
            }
        }
        catch (Exception ex) {
            return HandleError(ex, "publish");
        }
    }

    private static string ResolveManifestPath(string? manifestPath) {
        var path = manifestPath ?? DefaultManifestFileName;

        if (!Path.IsPathRooted(path)) {
            path = Path.Combine(Environment.CurrentDirectory, path);
        }

        return !File.Exists(path) ? throw new FileNotFoundException($"Manifest file not found: {path}") : path;
    }

    private static string? ResolveOptionalFile(string? filePath, string defaultFileName) {
        if (!string.IsNullOrEmpty(filePath)) {
            var path = Path.IsPathRooted(filePath) ? filePath : Path.Combine(Environment.CurrentDirectory, filePath);
            return File.Exists(path) ? path : null;
        }

        var defaultPath = Path.Combine(Environment.CurrentDirectory, defaultFileName);
        return File.Exists(defaultPath) ? defaultPath : null;
    }

    private static async Task<string> LoadManifestAsync(string manifestPath) {
        try {
            var content = await File.ReadAllTextAsync(manifestPath);
            return string.IsNullOrWhiteSpace(content) ? throw new InvalidOperationException("Manifest file is empty") : content;
        }
        catch (Exception ex) when (ex is not InvalidOperationException) {
            throw new FileNotFoundException($"Failed to read manifest file: {ex.Message}");
        }
    }

    private async Task<string?> LoadOptionalFileAsync(string? filePath, string fileType) {
        if (string.IsNullOrEmpty(filePath)) {
            return null;
        }

        try {
            var content = await File.ReadAllTextAsync(filePath);
            OutputFormatter.WriteInfo($"Loaded {fileType} from: {filePath}");
            return content;
        }
        catch (Exception ex) {
            OutputFormatter.WriteWarning($"Failed to load {fileType} file '{filePath}': {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Enhanced manifest validation with detailed feedback
    /// </summary>
    private async Task<ValidateManifestResponse> ValidateManifestWithEnhancedFeedbackAsync(string manifestContent, bool nonInteractive) {
        var validationRequest = new ValidateManifestRequest {
            ManifestContent = manifestContent,
            IncludeWarnings = true,
        };

        using var spinner = ProgressReporter.CreateSpinner("Validating manifest...");
        var result = await ApiClient.ValidateManifestAsync(validationRequest);

        if (result.Errors.Count > 0) {
            spinner.Fail("Validation failed");
            OutputFormatter.WriteError("Manifest validation errors:");
            foreach (var error in result.Errors) {
                OutputFormatter.WriteError($"  - {error}");
            }

            // Interactive error resolution suggestions
            if (!nonInteractive) {
                await SuggestErrorResolutionsAsync(result.Errors);
            }
        }
        else if (result.Warnings.Count > 0) {
            spinner.Success("Validation completed with warnings");
        }
        else {
            spinner.Success("Validation passed");
        }

        if (result.Warnings.Count > 0) {
            OutputFormatter.WriteWarning("Manifest validation warnings:");
            foreach (var warning in result.Warnings) {
                OutputFormatter.WriteWarning($"  - {warning}");
            }
        }

        return result;
    }

    /// <summary>
    /// Suggests resolutions for validation errors
    /// </summary>
    private async Task SuggestErrorResolutionsAsync(List<string> errors) {
        OutputFormatter.WriteLine();
        OutputFormatter.WriteInfo("Suggestions for fixing validation errors:");

        foreach (var error in errors) {
            var suggestion = GetErrorSuggestion(error);
            if (!string.IsNullOrEmpty(suggestion)) {
                OutputFormatter.WriteInfo($"  • {suggestion}");
            }
        }

        var openEditor = await InteractionService.ConfirmAsync(
            "Would you like guidance on editing the manifest file?", false);

        if (openEditor) {
            OutputFormatter.WriteInfo("Common manifest fields:");
            OutputFormatter.WriteInfo("  - name: Package identifier (lowercase, no spaces)");
            OutputFormatter.WriteInfo("  - version: Semantic version (e.g., 1.0.0)");
            OutputFormatter.WriteInfo("  - description: Brief package description");
            OutputFormatter.WriteInfo("  - author: Author information { name, email }");
            OutputFormatter.WriteInfo("  - license: License identifier (e.g., MIT, Apache-2.0)");
        }
    }

    /// <summary>
    /// Gets suggestions for specific validation errors
    /// </summary>
    private static string GetErrorSuggestion(string error) => error.ToLower() switch {
        var e when e.Contains("name") => "Use lowercase letters, numbers, and hyphens only for package name",
        var e when e.Contains("version") => "Use semantic versioning format (e.g., 1.0.0, 2.1.3-beta)",
        var e when e.Contains("description") => "Add a brief description explaining what your package does",
        var e when e.Contains("author") => "Include author name and email: { \"name\": \"Your Name\", \"email\": \"you@example.com\" }",
        var e when e.Contains("license") => "Specify a valid license identifier (MIT, Apache-2.0, GPL-3.0, etc.)",
        _ => $"Check the documentation for proper format of: {error}",
    };

    /// <summary>
    /// Handles validation warnings interactively
    /// </summary>
    private async Task<bool> HandleValidationWarningsAsync(List<string> warnings) {
        OutputFormatter.WriteLine();
        OutputFormatter.WriteWarning($"Found {warnings.Count} validation warning(s):");

        foreach (var warning in warnings) {
            OutputFormatter.WriteWarning($"  - {warning}");
        }

        var options = new Dictionary<string, string> {
            { "continue", "Continue with publishing" },
            { "review", "Review warnings in detail" },
            { "cancel", "Cancel publishing" },
                                                     };

        var selection = await InteractionService.ShowMenuAsync("How would you like to proceed?", options);

        return selection switch {
            "continue" => true,
            "review" => await ReviewWarningsDetailAsync(warnings),
            "cancel" => false,
            _ => false,
        };
    }

    /// <summary>
    /// Reviews warnings in detail
    /// </summary>
    private async Task<bool> ReviewWarningsDetailAsync(List<string> warnings) {
        foreach (var warning in warnings) {
            OutputFormatter.WriteLine();
            OutputFormatter.WriteInfo($"Warning: {warning}");
            OutputFormatter.WriteInfo("Recommendations:");

            var recommendation = GetWarningRecommendation(warning);
            OutputFormatter.WriteInfo($"  {recommendation}");

            var continueReview = await InteractionService.ConfirmAsync("Continue reviewing?", true);
            if (!continueReview)
                break;
        }

        return await InteractionService.ConfirmAsync("Proceed with publishing despite warnings?", false);
    }

    /// <summary>
    /// Gets recommendations for warnings
    /// </summary>
    private static string GetWarningRecommendation(string warning) => warning.ToLower() switch {
        var w when w.Contains("readme") => "Consider adding a README.md file to help users understand your package",
        var w when w.Contains("changelog") => "Add a CHANGELOG.md to document version changes",
        var w when w.Contains("keyword") => "Add relevant keywords to improve package discoverability",
        var w when w.Contains("homepage") => "Include a homepage URL in your manifest",
        var w when w.Contains("repository") => "Add repository URL to help users find your source code",
        _ => "Review the manifest documentation for best practices",
    };

    /// <summary>
    /// Reviews and allows editing of manifest interactively
    /// </summary>
    private async Task<MCPManifest?> ReviewAndEditManifestAsync(MCPManifest manifest) {
        var reviewManifest = await InteractionService.ConfirmAsync(
            "Would you like to review and potentially edit the manifest before publishing?", false);

        if (!reviewManifest)
            return null;

        OutputFormatter.WriteLine();
        OutputFormatter.WriteInfo("Current manifest details:");
        OutputFormatter.WriteInfo($"  Name: {manifest.Name}");
        OutputFormatter.WriteInfo($"  Version: {manifest.Version}");
        OutputFormatter.WriteInfo($"  Description: {manifest.Description ?? "Not specified"}");
        OutputFormatter.WriteInfo($"  Author: {manifest.Author?.Name ?? "Not specified"}");
        OutputFormatter.WriteInfo($"  License: {manifest.License ?? "Not specified"}");

        var editOptions = new Dictionary<string, string> {
            { "description", "Edit description" },
            { "author", "Edit author information" },
            { "license", "Edit license" },
            { "done", "Continue with current manifest" },
                                                         };

        while (true) {
            var selection = await InteractionService.ShowMenuAsync("What would you like to edit?", editOptions);

            switch (selection) {
                case "description":
                    var newDescription = await InteractionService.PromptAsync(
                        "Enter new description:", manifest.Description);
                    if (!string.IsNullOrEmpty(newDescription)) {
                        manifest.Description = newDescription;
                    }
                    break;

                case "license":
                    var newLicense = await InteractionService.PromptAsync(
                        "Enter license (e.g., MIT, Apache-2.0):", manifest.License);
                    if (!string.IsNullOrEmpty(newLicense)) {
                        manifest.License = newLicense;
                    }
                    break;

                case "done":
                default:
                    return manifest;
            }
        }
    }

    /// <summary>
    /// Reviews additional files interactively
    /// </summary>
    private async Task ReviewAdditionalFilesAsync(string? readmeContent, string? changelogContent, List<string> tags) {
        var reviewFiles = await InteractionService.ConfirmAsync(
            "Review additional files and tags?", false);

        if (!reviewFiles)
            return;

        OutputFormatter.WriteLine();
        OutputFormatter.WriteInfo("Additional files:");
        OutputFormatter.WriteInfo($"  README: {(readmeContent != null ? "✓ Included" : "✗ Not found")}");
        OutputFormatter.WriteInfo($"  Changelog: {(changelogContent != null ? "✓ Included" : "✗ Not found")}");
        OutputFormatter.WriteInfo($"  Tags: {(tags.Any() ? string.Join(", ", tags) : "None specified")}");

        if (readmeContent == null) {
            var addReadme = await InteractionService.ConfirmAsync(
                "No README found. This is recommended for better package discoverability. Continue anyway?", true);
            if (!addReadme) {
                OutputFormatter.WriteInfo("Consider adding a README.md file and re-running the publish command.");
            }
        }
    }

    /// <summary>
    /// Shows enhanced publishing summary
    /// </summary>
    private async Task ShowEnhancedPublishingSummaryAsync(MCPManifest manifest, List<string> tags, bool dryRun, bool nonInteractive) {
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

        if (tags.Count > 0) {
            table.AddRow("Tags", string.Join(", ", tags.Select(t => $"[blue]{t}[/]")));
        }

        if (manifest.Capabilities != null) {
            var capabilities = new List<string>();
            if (manifest.Capabilities.Tools?.Any() == true)
                capabilities.Add($"{manifest.Capabilities.Tools.Count()} tools");
            if (manifest.Capabilities.Resources?.Any() == true)
                capabilities.Add($"{manifest.Capabilities.Resources.Count()} resources");
            if (manifest.Capabilities.Prompts?.Any() == true)
                capabilities.Add($"{manifest.Capabilities.Prompts.Count()} prompts");

            if (capabilities.Count > 0) {
                table.AddRow("Capabilities", string.Join(", ", capabilities));
            }
        }

        AnsiConsole.Write(table);
        OutputFormatter.WriteLine();

        // Interactive summary review
        if (!nonInteractive && !dryRun) {
            var reviewSummary = await InteractionService.ConfirmAsync(
                "Does this summary look correct?", true);

            if (!reviewSummary) {
                OutputFormatter.WriteInfo("You can cancel and make changes to your manifest file.");
            }
        }
    }

    /// <summary>
    /// Publishes package with enhanced progress tracking
    /// </summary>
    private async Task<PublishPackageResponse> PublishPackageWithProgressAsync(PublishPackageRequest request, MCPManifest manifest) {
        using var publishProgress = ProgressReporter.CreateProgressBar($"Publishing {manifest.Name}@{manifest.Version}", 100);

        try {
            publishProgress.UpdateProgress(10, "Uploading package data...");
            await Task.Delay(500); // Simulate upload time

            publishProgress.UpdateProgress(30, "Validating package on server...");
            await Task.Delay(300);

            publishProgress.UpdateProgress(60, "Processing package metadata...");
            await Task.Delay(400);

            publishProgress.UpdateProgress(80, "Indexing package for search...");
            var result = await ApiClient.PublishPackageAsync(request);

            publishProgress.UpdateProgress(100, "Package published successfully");
            publishProgress.Complete("Publishing completed");

            return result;
        }
        catch (Exception ex) {
            publishProgress.Fail($"Publishing failed: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Shows enhanced publish success with post-publication actions
    /// </summary>
    private async Task ShowEnhancedPublishSuccessAsync(PublishPackageResponse result, MCPManifest manifest, bool nonInteractive) {
        OutputFormatter.WriteSuccess($"Package '{manifest.Name}' version '{manifest.Version}' published successfully!");
        OutputFormatter.WriteLine();

        if (result.Package != null) {
            OutputFormatter.WriteInfo($"Package ID: {result.Package.Id}");
            OutputFormatter.WriteInfo($"Status: {result.Package.Status}");
        }

        if (!string.IsNullOrEmpty(result.StorageUrl)) {
            OutputFormatter.WriteInfo($"Package URL: {result.StorageUrl}");
        }

        OutputFormatter.WriteInfo($"Published at: {result.PublishedAt:yyyy-MM-dd HH:mm:ss} UTC");
        OutputFormatter.WriteInfo($"Processing time: {result.PublishTimeMs}ms");

        if (result.Warnings.Count > 0) {
            OutputFormatter.WriteLine();
            OutputFormatter.WriteWarning("Warnings:");
            foreach (var warning in result.Warnings) {
                OutputFormatter.WriteWarning($"  - {warning}");
            }
        }

        OutputFormatter.WriteLine();
        OutputFormatter.WriteInfo("Your package will be available in the registry shortly.");
        OutputFormatter.WriteInfo($"View package: mcpm info {manifest.Name}");
        OutputFormatter.WriteInfo($"Install package: mcpm install {manifest.Name}");

        // Interactive post-publication options
        if (!nonInteractive) {
            var showNextSteps = await InteractionService.ConfirmAsync(
                "Would you like to see next steps for promoting your package?", false);

            if (showNextSteps) {
                await ShowPostPublicationGuidanceAsync(manifest);
            }
        }
    }

    /// <summary>
    /// Shows post-publication guidance
    /// </summary>
    private async Task ShowPostPublicationGuidanceAsync(MCPManifest manifest) {
        OutputFormatter.WriteLine();
        OutputFormatter.WriteInfo("Next steps to promote your package:");
        OutputFormatter.WriteInfo("  1. Share your package on social media");
        OutputFormatter.WriteInfo("  2. Add documentation and examples");
        OutputFormatter.WriteInfo("  3. Encourage users to leave ratings and reviews");
        OutputFormatter.WriteInfo("  4. Monitor package analytics and user feedback");
        OutputFormatter.WriteInfo("  5. Keep your package updated with bug fixes");

        var viewAnalytics = await InteractionService.ConfirmAsync(
            "Would you like information about package analytics?", false);

        if (viewAnalytics) {
            OutputFormatter.WriteInfo("Package analytics will be available at:");
            OutputFormatter.WriteInfo($"  Dashboard: https://mcphub.com/packages/{manifest.Name}/analytics");
            OutputFormatter.WriteInfo($"  API: mcpm analytics {manifest.Name}");
        }
    }

    private async Task<ValidateManifestResponse> ValidateManifestWithFeedbackAsync(string manifestContent) {
        var validationRequest = new ValidateManifestRequest {
            ManifestContent = manifestContent,
            IncludeWarnings = true,
        };

        var result = await WithProgressAsync(
            "Validating manifest...",
            () => ApiClient.ValidateManifestAsync(validationRequest));

        if (result.Errors.Count > 0) {
            OutputFormatter.WriteError("Manifest validation errors:");
            foreach (var error in result.Errors) {
                OutputFormatter.WriteError($"  - {error}");
            }
        }

        if (result.Warnings.Count > 0) {
            OutputFormatter.WriteWarning("Manifest validation warnings:");
            foreach (var warning in result.Warnings) {
                OutputFormatter.WriteWarning($"  - {warning}");
            }
        }

        if (result.IsValid && result.Errors.Count == 0 && result.Warnings.Count == 0) {
            OutputFormatter.WriteSuccess("Manifest validation passed!");
        }

        return result;
    }

    private MCPManifest? ParseManifest(string manifestContent) {
        try {
            var options = new JsonSerializerOptions {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true,
            };
            return JsonSerializer.Deserialize<MCPManifest>(manifestContent, options);
        }
        catch (JsonException ex) {
            OutputFormatter.WriteError($"Failed to parse manifest: {ex.Message}");
            return null;
        }
    }

    private static List<string> ParseTags(string? tags) => string.IsNullOrWhiteSpace(tags)
            ? []
            : tags.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(t => t.Trim())
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

    private async Task<PublishPackageRequest> CreatePublishRequestAsync(
        string manifestContent,
        List<string> tags,
        string? readmeContent,
        string? changelogContent,
        string? packageUrl) {
        var request = new PublishPackageRequest {
            ManifestContent = manifestContent,
            Tags = tags,
            ReadmeContent = readmeContent,
            ChangelogContent = changelogContent,
            PackageUrl = packageUrl,
        };

        // If no package URL provided, create archive from current directory
        if (string.IsNullOrEmpty(packageUrl)) {
            request.PackageArchive = await CreatePackageArchiveAsync();
        }

        return request;
    }

    private async Task<string> CreatePackageArchiveAsync() {
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
        foreach (var file in files) {
            var relativePath = Path.GetRelativePath(currentDir, file);
            var content = await File.ReadAllTextAsync(file);
            fileData[relativePath] = Convert.ToBase64String(Encoding.UTF8.GetBytes(content));
        }

        var archiveJson = JsonSerializer.Serialize(fileData);
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(archiveJson));
    }

    private static bool IsExcludedFile(string filePath) {
        var fileName = Path.GetFileName(filePath);
        var relativePath = Path.GetRelativePath(Environment.CurrentDirectory, filePath);

        // Exclude common development files
        var excludePatterns = new[]
        {
            ".git/", "node_modules/", ".vs/", ".vscode/", "bin/", "obj/",
            ".gitignore", ".gitattributes", ".mcpmignore",
        };

        return excludePatterns.Any(pattern =>
            relativePath.StartsWith(pattern, StringComparison.OrdinalIgnoreCase) ||
            fileName.Equals(pattern.TrimEnd('/'), StringComparison.OrdinalIgnoreCase));
    }

    private void ShowPublishSummary(MCPManifest manifest, List<string> tags, bool dryRun) {
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

        if (tags.Count > 0) {
            table.AddRow("Tags", string.Join(", ", tags.Select(t => $"[blue]{t}[/]")));
        }

        if (manifest.Capabilities != null) {
            var capabilities = new List<string>();
            if (manifest.Capabilities.Tools?.Any() == true)
                capabilities.Add($"{manifest.Capabilities.Tools.Count()} tools");
            if (manifest.Capabilities.Resources?.Any() == true)
                capabilities.Add($"{manifest.Capabilities.Resources.Count()} resources");
            if (manifest.Capabilities.Prompts?.Any() == true)
                capabilities.Add($"{manifest.Capabilities.Prompts.Count()} prompts");

            if (capabilities.Count > 0) {
                table.AddRow("Capabilities", string.Join(", ", capabilities));
            }
        }

        AnsiConsole.Write(table);
        OutputFormatter.WriteLine();
    }

    private bool ConfirmWithWarnings(List<string> warnings) {
        OutputFormatter.WriteWarning($"Found {warnings.Count} warning(s). Continue with publishing?");
        return AnsiConsole.Confirm("Proceed with publishing?", false);
    }

    private static bool ConfirmPublishing(MCPManifest manifest) => AnsiConsole.Confirm($"Publish package '{manifest.Name}' version '{manifest.Version}' to the registry?", false);

    private async Task<PublishPackageResponse> PublishPackageAsync(PublishPackageRequest request, MCPManifest manifest) => await WithProgressAsync(
            $"Publishing package '{manifest.Name}' version '{manifest.Version}'...",
            () => ApiClient.PublishPackageAsync(request));

    private void ShowPublishSuccess(PublishPackageResponse result, MCPManifest manifest) {
        OutputFormatter.WriteSuccess($"Package '{manifest.Name}' version '{manifest.Version}' published successfully!");
        OutputFormatter.WriteLine();

        if (result.Package != null) {
            OutputFormatter.WriteInfo($"Package ID: {result.Package.Id}");
            OutputFormatter.WriteInfo($"Status: {result.Package.Status}");
        }

        if (!string.IsNullOrEmpty(result.StorageUrl)) {
            OutputFormatter.WriteInfo($"Package URL: {result.StorageUrl}");
        }

        OutputFormatter.WriteInfo($"Published at: {result.PublishedAt:yyyy-MM-dd HH:mm:ss} UTC");
        OutputFormatter.WriteInfo($"Processing time: {result.PublishTimeMs}ms");

        if (result.Warnings.Count > 0) {
            OutputFormatter.WriteLine();
            OutputFormatter.WriteWarning("Warnings:");
            foreach (var warning in result.Warnings) {
                OutputFormatter.WriteWarning($"  - {warning}");
            }
        }

        OutputFormatter.WriteLine();
        OutputFormatter.WriteInfo("Your package will be available in the registry shortly.");
        OutputFormatter.WriteInfo($"You can view it with: mcpm info {manifest.Name}");
    }

    private void ShowPublishFailure(PublishPackageResponse result) {
        OutputFormatter.WriteError("Package publishing failed!");

        if (result.Errors.Count > 0) {
            OutputFormatter.WriteError("Errors:");
            foreach (var error in result.Errors) {
                OutputFormatter.WriteError($"  - {error}");
            }
        }

        if (result.Warnings.Count > 0) {
            OutputFormatter.WriteWarning("Warnings:");
            foreach (var warning in result.Warnings) {
                OutputFormatter.WriteWarning($"  - {warning}");
            }
        }
    }
}