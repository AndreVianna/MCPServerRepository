using System.CommandLine;
using Microsoft.Extensions.Logging;
using MCPHub.CommandLineApp.Configuration;
using MCPHub.CommandLineApp.Models;
using MCPHub.CommandLineApp.Services;
using MCPHub.CommandLineApp.Utilities;

namespace MCPHub.CommandLineApp.Commands;

/// <summary>
/// Command for searching MCP packages
/// </summary>
public class SearchCommand : BaseCommand
{
    public SearchCommand(
        ILogger<SearchCommand> logger,
        McpmConfiguration configuration,
        IMcpHubApiClient apiClient,
        IOutputFormatter outputFormatter)
        : base(logger, configuration, apiClient, outputFormatter)
    {
    }

    /// <inheritdoc />
    public override Command CreateCommand()
    {
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

        // Handler
        command.SetHandler(
            ExecuteAsync,
            queryArgument,
            categoryOption,
            trustTierOption,
            sortOption,
            pageOption,
            limitOption,
            formatOption);

        return command;
    }

    private async Task<int> ExecuteAsync(
        string query,
        string? category,
        string? trustTier,
        string? sort,
        int page,
        int limit,
        string? format)
    {
        try
        {
            Logger.LogInformation("Executing search command: Query={Query}, Page={Page}, Limit={Limit}", 
                query, page, limit);

            // Validate input
            if (string.IsNullOrWhiteSpace(query))
            {
                OutputFormatter.WriteError("Search query is required");
                return 400;
            }

            if (page < 1)
            {
                OutputFormatter.WriteError("Page number must be 1 or greater");
                return 400;
            }

            if (limit < 1 || limit > 100)
            {
                OutputFormatter.WriteError("Limit must be between 1 and 100");
                return 400;
            }

            // Validate and normalize options
            var parsedTrustTier = ParseTrustTier(trustTier);
            if (!string.IsNullOrEmpty(trustTier) && parsedTrustTier == null)
            {
                OutputFormatter.WriteError($"Invalid trust tier: {trustTier}. Valid values are: Unverified, Community, Professional, Enterprise");
                return 400;
            }

            var parsedCategories = ParseCategories(category);
            var parsedSort = ParseSortBy(sort);
            var outputFormat = ValidateOutputFormat(format);

            if (!string.IsNullOrEmpty(sort) && parsedSort == null)
            {
                OutputFormatter.WriteError($"Invalid sort field: {sort}. Valid values are: name, downloads, rating, created, updated");
                return 400;
            }

            // Check API connectivity
            if (!await ValidateApiConnectivityAsync())
            {
                return 503; // Service unavailable
            }

            // Create search request
            var searchRequest = new SearchRequest
            {
                Query = query,
                Categories = parsedCategories,
                TrustTier = parsedTrustTier,
                Page = page,
                PageSize = limit,
                SortBy = parsedSort,
                SortDirection = "Ascending"
            };

            // Execute search
            var results = await WithProgressAsync(
                $"Searching for '{query}'...",
                () => ApiClient.SearchPackagesAsync(searchRequest));

            // Display results
            if (results.Packages.Count == 0)
            {
                OutputFormatter.WriteInfo($"No packages found matching '{query}'");
                
                if (parsedCategories?.Any() == true || !string.IsNullOrEmpty(parsedTrustTier))
                {
                    OutputFormatter.WriteInfo("Try removing filters or broadening your search criteria.");
                }
                
                return 0;
            }

            OutputFormatter.WriteSearchResults(results, outputFormat);

            // Show pagination info for table format
            if (outputFormat == "table" && results.TotalPages > 1)
            {
                OutputFormatter.WriteLine();
                OutputFormatter.WriteInfo($"Showing page {results.Page} of {results.TotalPages} ({results.TotalCount} total packages)");
                
                if (results.Page < results.TotalPages)
                {
                    OutputFormatter.WriteInfo($"Use --page {results.Page + 1} to see more results");
                }
            }

            Logger.LogInformation("Search completed successfully: Found {PackageCount} packages", results.Packages.Count);
            return 0;
        }
        catch (Exception ex)
        {
            return HandleError(ex, "search");
        }
    }
}