using System.CommandLine;

using MCPHub.CommandLineApp.Configuration;
using MCPHub.CommandLineApp.Models;
using MCPHub.CommandLineApp.Services;
using MCPHub.CommandLineApp.Utilities;

namespace MCPHub.CommandLineApp.Commands;

/// <summary>
/// Command for searching MCP packages
/// </summary>
public class SearchCommand(
    ILogger<SearchCommand> logger,
    McpmConfiguration configuration,
    IMcpHubApiClient apiClient,
    IOutputFormatter outputFormatter,
    IInteractionService interactionService,
    IProgressReporter progressReporter) : BaseCommand(logger, configuration, apiClient, outputFormatter, interactionService, progressReporter) {

    /// <inheritdoc />
    public override Command CreateCommand() {
        var command = new Command("search", "Search for MCP packages in the registry");

        // Arguments
        var queryArgument = new Argument<string>("query", "Search query string");
        command.AddArgument(queryArgument);

        // Options
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
            getDefaultValue: () => 20);
        command.AddOption(limitOption);

        var formatOption = new Option<string?>(
            aliases: ["--format", "-f"],
            description: "Output format (table, json, detailed)");
        command.AddOption(formatOption);

        var nonInteractiveOption = new Option<bool>(
            aliases: ["--non-interactive", "-n"],
            description: "Disable interactive features (for CI/CD)");
        command.AddOption(nonInteractiveOption);

        var installFromSearchOption = new Option<bool>(
            aliases: ["--install", "-i"],
            description: "Allow installation directly from search results");
        command.AddOption(installFromSearchOption);

        // Handler - using fewer parameters to avoid SetHandler limitations
        command.SetHandler(async (query, category, trustTier, sort, page, limit, format) => {
            var exitCode = await ExecuteAsync(query, category, trustTier, sort, page, limit, format, false, false);
            Environment.Exit(exitCode);
        }, queryArgument, categoryOption, trustTierOption, sortOption, pageOption, limitOption, formatOption);

        return command;
    }

    private async Task<int> ExecuteAsync(
        string query,
        string? category,
        string? trustTier,
        string? sort,
        int page,
        int limit,
        string? format,
        bool nonInteractive,
        bool installFromSearch) {
        try {
            Logger.LogInformation("Executing search command: Query={Query}, Page={Page}, Limit={Limit}",
                query, page, limit);

            // Validate input
            if (string.IsNullOrWhiteSpace(query)) {
                OutputFormatter.WriteError("Search query is required");
                return 400;
            }

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

            // Create search request
            var searchRequest = new SearchRequest {
                Query = query,
                Categories = parsedCategories,
                TrustTier = parsedTrustTier,
                Page = page,
                PageSize = limit,
                SortBy = parsedSort,
                SortDirection = "Ascending"
            };

            // Execute search with enhanced progress
            using var spinner = ProgressReporter.CreateSpinner($"Searching for '{query}'...");
            var results = await ApiClient.SearchPackagesAsync(searchRequest);
            spinner.Success($"Found {results.TotalCount} packages");

            // Handle no results with enhanced suggestions
            if (results.Packages.Count == 0) {
                await HandleNoResultsAsync(query, parsedCategories, parsedTrustTier, nonInteractive);
                return 0;
            }

            // Display results
            OutputFormatter.WriteSearchResults(results, outputFormat);

            // Interactive features (if not in non-interactive mode)
            if (!nonInteractive) {
                await HandleInteractiveSearchResultsAsync(results, installFromSearch, outputFormat);
            }
            else {
                // Show pagination info for non-interactive mode
                if (outputFormat == "table" && results.TotalPages > 1) {
                    OutputFormatter.WriteLine();
                    OutputFormatter.WriteInfo($"Showing page {results.Page} of {results.TotalPages} ({results.TotalCount} total packages)");

                    if (results.Page < results.TotalPages) {
                        OutputFormatter.WriteInfo($"Use --page {results.Page + 1} to see more results");
                    }
                }
            }

            Logger.LogInformation("Search completed successfully: Found {PackageCount} packages", results.Packages.Count);
            return 0;
        }
        catch (Exception ex) {
            return HandleError(ex, "search");
        }
    }

    /// <summary>
    /// Handles no search results with enhanced suggestions
    /// </summary>
    private async Task HandleNoResultsAsync(string query, IEnumerable<string>? categories, string? trustTier, bool nonInteractive) {
        OutputFormatter.WriteInfo($"No packages found matching '{query}'");

        if (categories?.Any() == true || !string.IsNullOrEmpty(trustTier)) {
            OutputFormatter.WriteInfo("Try removing filters or broadening your search criteria.");
        }

        // Interactive "Did you mean?" suggestions
        if (!nonInteractive) {
            await SuggestAlternativeSearchAsync(query);
        }
    }

    /// <summary>
    /// Suggests alternative search terms based on typos or similar terms
    /// </summary>
    private async Task SuggestAlternativeSearchAsync(string query) {
        try {
            // Get suggested terms from API (if available)
            var suggestions = await GetSearchSuggestionsAsync(query);

            if (suggestions.Any()) {
                OutputFormatter.WriteLine();
                OutputFormatter.WriteInfo("Did you mean:");

                var selectedSuggestion = await InteractionService.SelectFromListAsync(
                    "Select a suggestion to search:",
                    suggestions.Concat(new[] { "None of the above" }),
                    s => s);

                if (selectedSuggestion is not null and not "None of the above") {
                    OutputFormatter.WriteInfo($"Searching for '{selectedSuggestion}'...");
                    // Note: In a real implementation, this would trigger a new search
                }
            }
        }
        catch (Exception ex) {
            Logger.LogWarning(ex, "Failed to get search suggestions");
        }
    }

    /// <summary>
    /// Gets search suggestions from the API
    /// </summary>
    private static Task<IEnumerable<string>> GetSearchSuggestionsAsync(string query) {
        // Placeholder implementation - would call API for suggestions
        var commonSuggestions = new List<string>();

        // Simple typo corrections for demonstration
        if (query.Contains("managment"))
            commonSuggestions.Add(query.Replace("managment", "management"));
        if (query.Contains("analitcs"))
            commonSuggestions.Add(query.Replace("analitcs", "analytics"));
        if (query.Contains("authentiction"))
            commonSuggestions.Add(query.Replace("authentiction", "authentication"));

        return Task.FromResult<IEnumerable<string>>(commonSuggestions);
    }

    /// <summary>
    /// Handles interactive features for search results
    /// </summary>
    private async Task HandleInteractiveSearchResultsAsync(SearchResultResponse results, bool allowInstall, string outputFormat) {
        try {
            OutputFormatter.WriteLine();

            var options = new Dictionary<string, string> {
                { "details", "View package details" },
                { "filter", "Apply additional filters" },
                { "sort", "Change sorting" }
            };

            if (allowInstall) {
                options.Add("install", "Install a package");
            }

            if (results.TotalPages > 1) {
                options.Add("navigate", "Navigate pages");
            }

            options.Add("exit", "Exit search");

            var selection = await InteractionService.ShowMenuAsync("What would you like to do with these results?", options);

            switch (selection) {
                case "details":
                    await ShowPackageDetailsAsync(results.Packages);
                    break;
                case "filter":
                    await HandleInteractiveFilteringAsync();
                    break;
                case "sort":
                    await HandleInteractiveSortingAsync();
                    break;
                case "install" when allowInstall:
                    await HandleInstallFromSearchAsync(results.Packages);
                    break;
                case "navigate":
                    await HandlePaginationAsync(results);
                    break;
                case "exit":
                default:
                    return;
            }
        }
        catch (Exception ex) {
            Logger.LogWarning(ex, "Error in interactive search handling");
        }
    }

    /// <summary>
    /// Shows detailed information for selected packages
    /// </summary>
    private async Task ShowPackageDetailsAsync(IEnumerable<object> packages) {
        var selectedPackage = await InteractionService.SelectFromListAsync(
            "Select a package to view details:",
            packages,
            p => $"{((dynamic)p).Name} - {((dynamic)p).Description ?? "No description"}");

        if (selectedPackage != null) {
            var packageName = ((dynamic)selectedPackage).Name;
            using var spinner = ProgressReporter.CreateSpinner($"Getting details for {packageName}...");

            try {
                var packageInfo = await ApiClient.GetPackageInfoAsync(packageName);
                spinner.Success("Package details retrieved");

                OutputFormatter.WriteLine();
                OutputFormatter.WriteInfo($"Package: {packageInfo.Name}");
                OutputFormatter.WriteInfo($"Version: {packageInfo.Version}");
                OutputFormatter.WriteInfo($"Description: {packageInfo.Description ?? "No description"}");
                OutputFormatter.WriteInfo($"Downloads: {packageInfo.DownloadCount:N0}");
                OutputFormatter.WriteInfo($"Trust Tier: {packageInfo.TrustTier}");
                if (packageInfo.SecurityGrade != null) {
                    OutputFormatter.WriteInfo($"Security Grade: {packageInfo.SecurityGrade}");
                }
            }
            catch (Exception ex) {
                spinner.Fail($"Failed to get package details: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Handles interactive filtering
    /// </summary>
    private Task HandleInteractiveFilteringAsync() {
        OutputFormatter.WriteInfo("Interactive filtering would be implemented here");
        // Implementation would allow users to interactively set filters
        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles interactive sorting
    /// </summary>
    private async Task HandleInteractiveSortingAsync() {
        var sortOptions = new[] { "name", "downloads", "rating", "created", "updated" };
        var selectedSort = await InteractionService.SelectFromListAsync(
            "Select sort field:",
            sortOptions,
            s => s);

        if (selectedSort != null) {
            OutputFormatter.WriteInfo($"Would re-sort by {selectedSort}");
            // Implementation would re-run search with new sort
        }
    }

    /// <summary>
    /// Handles installation directly from search results
    /// </summary>
    private async Task HandleInstallFromSearchAsync(IEnumerable<object> packages) {
        var selectedPackage = await InteractionService.SelectFromListAsync(
            "Select a package to install:",
            packages,
            p => $"{((dynamic)p).Name} - {((dynamic)p).Description ?? "No description"}");

        if (selectedPackage != null) {
            var packageName = ((dynamic)selectedPackage).Name;
            var confirm = await InteractionService.ConfirmAsync($"Install {packageName}?", true);

            if (confirm) {
                OutputFormatter.WriteInfo($"To install {packageName}, run: mcpm install {packageName}");
                // In a real implementation, this could trigger the install command directly
            }
        }
    }

    /// <summary>
    /// Handles interactive pagination
    /// </summary>
    private async Task HandlePaginationAsync(SearchResultResponse results) {
        var options = new Dictionary<string, string>();

        if (results.Page > 1) {
            options.Add("prev", "Previous page");
        }
        if (results.Page < results.TotalPages) {
            options.Add("next", "Next page");
        }
        options.Add("goto", "Go to specific page");
        options.Add("back", "Back to results");

        var selection = await InteractionService.ShowMenuAsync("Navigation options:", options);

        switch (selection) {
            case "prev":
                OutputFormatter.WriteInfo($"Would navigate to page {results.Page - 1}");
                break;
            case "next":
                OutputFormatter.WriteInfo($"Would navigate to page {results.Page + 1}");
                break;
            case "goto":
                var pageInput = await InteractionService.PromptAsync(
                    $"Enter page number (1-{results.TotalPages}):",
                    validator: input => {
                        if (int.TryParse(input, out var page) && page >= 1 && page <= results.TotalPages) {
                            return null; // Valid
                        }
                        return $"Please enter a number between 1 and {results.TotalPages}";
                    });

                if (int.TryParse(pageInput, out var targetPage)) {
                    OutputFormatter.WriteInfo($"Would navigate to page {targetPage}");
                }
                break;
        }
    }
}