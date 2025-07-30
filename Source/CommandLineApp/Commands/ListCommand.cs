using System.CommandLine;

using MCPHub.CommandLineApp.Configuration;
using MCPHub.CommandLineApp.Models;
using MCPHub.CommandLineApp.Services;
using MCPHub.CommandLineApp.Utilities;

namespace MCPHub.CommandLineApp.Commands;

/// <summary>
/// Command for listing packages with optional filtering
/// </summary>
public class ListCommand(
    ILogger<ListCommand> logger,
    McpmConfiguration configuration,
    IMcpHubApiClient apiClient,
    IOutputFormatter outputFormatter,
    IInteractionService interactionService,
    IProgressReporter progressReporter) : BaseCommand(logger, configuration, apiClient, outputFormatter, interactionService, progressReporter) {

    /// <inheritdoc />
    public override Command CreateCommand() {
        var command = new Command("list", "List packages from the registry");

        // Options
        var installedOption = new Option<bool>(
            aliases: ["--installed", "-i"],
            description: "Show only installed packages (requires authentication)");
        command.AddOption(installedOption);

        var categoryOption = new Option<string?>(
            aliases: ["--category", "-c"],
            description: "Filter by category (comma-separated for multiple)");
        command.AddOption(categoryOption);

        var trustTierOption = new Option<string?>(
            aliases: ["--trust-tier", "-t"],
            description: "Minimum trust tier (Unverified, Community, Professional, Enterprise)");
        command.AddOption(trustTierOption);

        var sortOption = new Option<string?>(
            aliases: ["--sort", "-s"],
            description: "Sort by field (name, downloads, rating, created, updated)");
        command.AddOption(sortOption);

        var pageOption = new Option<int>(
            aliases: ["--page", "-p"],
            description: "Page number (1-based)",
            getDefaultValue: () => 1);
        command.AddOption(pageOption);

        var limitOption = new Option<int>(
            aliases: ["--limit", "-l"],
            description: "Number of results per page",
            getDefaultValue: () => 50);
        command.AddOption(limitOption);

        var formatOption = new Option<string?>(
            aliases: ["--format", "-f"],
            description: "Output format (table, json, detailed)");
        command.AddOption(formatOption);

        var allOption = new Option<bool>(
            aliases: ["--all", "-a"],
            description: "Show all available packages (no filtering)");
        command.AddOption(allOption);

        // Handler
        command.SetHandler(
            ExecuteAsync,
            installedOption,
            categoryOption,
            trustTierOption,
            sortOption,
            pageOption,
            limitOption,
            formatOption,
            allOption);

        return command;
    }

    private async Task<int> ExecuteAsync(
        bool installed,
        string? category,
        string? trustTier,
        string? sort,
        int page,
        int limit,
        string? format,
        bool showAll) {
        try {
            Logger.LogInformation("Executing list command: Installed={Installed}, Page={Page}, Limit={Limit}",
                installed, page, limit);

            // Validate input
            if (page < 1) {
                OutputFormatter.WriteError("Page number must be 1 or greater");
                return 400;
            }

            if (limit is < 1 or > 100) {
                OutputFormatter.WriteError("Limit must be between 1 and 100");
                return 400;
            }

            // Validate and normalize options
            var parsedTrustTier = ParseTrustTier(trustTier);
            if (!string.IsNullOrEmpty(trustTier) && parsedTrustTier == null) {
                OutputFormatter.WriteError($"Invalid trust tier: {trustTier}. Valid values are: Unverified, Community, Professional, Enterprise");
                return 400;
            }

            var parsedCategories = ParseCategories(category);
            var parsedSort = ParseSortBy(sort);
            var outputFormat = ValidateOutputFormat(format);

            if (!string.IsNullOrEmpty(sort) && parsedSort == null) {
                OutputFormatter.WriteError($"Invalid sort field: {sort}. Valid values are: name, downloads, rating, created, updated");
                return 400;
            }

            // Check API connectivity
            if (!await ValidateApiConnectivityAsync()) {
                return 503; // Service unavailable
            }

            // Handle installed packages (not implemented in API yet)
            if (installed) {
                OutputFormatter.WriteWarning("Installed packages feature requires authentication and is not yet implemented");
                OutputFormatter.WriteInfo("Showing all available packages instead...");
                OutputFormatter.WriteLine();
            }

            // Determine if we should use search or basic list endpoint
            var hasFilters = parsedCategories?.Any() == true || !string.IsNullOrEmpty(parsedTrustTier);

            if (hasFilters && !showAll) {
                // Use search endpoint with filters
                var searchRequest = new SearchRequest {
                    Query = "", // Empty query to get all packages
                    Categories = parsedCategories,
                    TrustTier = parsedTrustTier,
                    Page = page,
                    PageSize = limit,
                    SortBy = parsedSort,
                    SortDirection = "Ascending"
                };

                var searchResults = await WithProgressAsync(
                    "Loading packages...",
                    () => ApiClient.SearchPackagesAsync(searchRequest));

                // Convert search results to package list format
                var packageList = new PackageListResponse {
                    Packages = searchResults.Packages,
                    TotalCount = searchResults.TotalCount,
                    Page = searchResults.Page,
                    PageSize = searchResults.PageSize,
                    TotalPages = searchResults.TotalPages
                };

                if (packageList.Packages.Count == 0) {
                    OutputFormatter.WriteInfo("No packages found matching the specified criteria");

                    if (parsedCategories?.Any() == true || !string.IsNullOrEmpty(parsedTrustTier)) {
                        OutputFormatter.WriteInfo("Try removing filters or use --all to see all packages");
                    }

                    return 0;
                }

                DisplayPackageList(packageList, outputFormat, hasFilters);
            }
            else {
                // Use basic list endpoint
                var packageList = await WithProgressAsync(
                    "Loading packages...",
                    () => ApiClient.GetPackagesAsync(page, limit));

                if (packageList.Packages.Count == 0) {
                    OutputFormatter.WriteInfo("No packages available");
                    return 0;
                }

                DisplayPackageList(packageList, outputFormat, hasFilters);
            }

            Logger.LogInformation("List command completed successfully");
            return 0;
        }
        catch (Exception ex) {
            return HandleError(ex, "list");
        }
    }

    private void DisplayPackageList(PackageListResponse packageList, string outputFormat, bool hasFilters) {
        OutputFormatter.WritePackageList(packageList, outputFormat);

        // Show pagination info for table format
        if (outputFormat == "table" && packageList.TotalPages > 1) {
            OutputFormatter.WriteLine();
            OutputFormatter.WriteInfo($"Showing page {packageList.Page} of {packageList.TotalPages} ({packageList.TotalCount} total packages)");

            if (packageList.Page < packageList.TotalPages) {
                OutputFormatter.WriteInfo($"Use --page {packageList.Page + 1} to see more results");
            }
        }

        // Show summary for non-JSON format
        if (outputFormat != "json") {
            OutputFormatter.WriteLine();

            if (hasFilters) {
                OutputFormatter.WriteInfo($"Found {packageList.TotalCount} packages matching your criteria");
            }
            else {
                OutputFormatter.WriteInfo($"Showing {packageList.Packages.Count} of {packageList.TotalCount} available packages");
            }

            OutputFormatter.WriteInfo("Use 'mcpm info <package-name>' for detailed information about a package");
            OutputFormatter.WriteInfo("Use 'mcpm search <query>' to search for specific packages");
        }
    }
}