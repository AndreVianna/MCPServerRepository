using MCPHub.CommandLineApp.Configuration;
using MCPHub.CommandLineApp.Services;
using MCPHub.CommandLineApp.Utilities;

namespace MCPHub.CommandLineApp.Commands;

/// <summary>
/// Base class for all CLI commands
/// </summary>
public abstract class BaseCommand(
    ILogger logger,
    McpmConfiguration configuration,
    IMcpHubApiClient apiClient,
    IOutputFormatter outputFormatter,
    IInteractionService interactionService,
    IProgressReporter progressReporter) {
    protected readonly ILogger Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    protected readonly McpmConfiguration Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    protected readonly IMcpHubApiClient ApiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
    protected readonly IOutputFormatter OutputFormatter = outputFormatter ?? throw new ArgumentNullException(nameof(outputFormatter));
    protected readonly IInteractionService InteractionService = interactionService ?? throw new ArgumentNullException(nameof(interactionService));
    protected readonly IProgressReporter ProgressReporter = progressReporter ?? throw new ArgumentNullException(nameof(progressReporter));

    /// <summary>
    /// Creates the command definition
    /// </summary>
    /// <returns>The configured command</returns>
    public abstract Command CreateCommand();

    /// <summary>
    /// Handles errors consistently across all commands
    /// </summary>
    /// <param name="ex">The exception that occurred</param>
    /// <param name="context">The command context</param>
    /// <returns>Exit code</returns>
    protected int HandleError(Exception ex, string context) {
        Logger.LogError(ex, "Error in {Context}", context);

        var errorMessage = ex switch {
            PackageNotFoundException => ex.Message,
            ManifestValidationException mvEx => $"Manifest validation failed: {string.Join(", ", mvEx.ValidationErrors)}",
            PackageAlreadyExistsException => ex.Message,
            UnauthorizedAccessException => "Authentication required. Please run 'mcpm login' first.",
            HttpRequestException httpEx when httpEx.Message.Contains("Rate limit") =>
                "Rate limit exceeded. Please try again later.",
            HttpRequestException httpEx when httpEx.Message.Contains("timeout") =>
                "Request timed out. Please check your internet connection and try again.",
            HttpRequestException => "Network error. Please check your internet connection and try again.",
            TaskCanceledException => "Operation was cancelled or timed out.",
            _ => Configuration.Ui.VerboseErrors ? ex.ToString() : "An unexpected error occurred.",
        };

        OutputFormatter.WriteError(errorMessage);

        if (Configuration.Ui.VerboseErrors && ex is not PackageNotFoundException and not ManifestValidationException and not PackageAlreadyExistsException) {
            OutputFormatter.WriteError($"Error details: {ex}");
        }

        // Show detailed validation errors for manifest validation failures
        if (ex is ManifestValidationException manifestEx && manifestEx.ValidationWarnings.Count > 0) {
            OutputFormatter.WriteWarning("Validation warnings:");
            foreach (var warning in manifestEx.ValidationWarnings) {
                OutputFormatter.WriteWarning($"  - {warning}");
            }
        }

        return GetExitCode(ex);
    }

    /// <summary>
    /// Gets the appropriate exit code for an exception
    /// </summary>
    /// <param name="ex">The exception</param>
    /// <returns>Exit code</returns>
    protected static int GetExitCode(Exception ex) => ex switch {
        PackageNotFoundException => 404,
        ManifestValidationException => 422,
        PackageAlreadyExistsException => 409,
        UnauthorizedAccessException => 401,
        ArgumentException => 400,
        TaskCanceledException => 130, // SIGINT
        HttpRequestException httpEx when httpEx.Message.Contains("Rate limit") => 429,
        _ => 1,
    };

    /// <summary>
    /// Shows a progress spinner for long-running operations
    /// </summary>
    /// <param name="message">The progress message</param>
    /// <param name="task">The task to execute</param>
    /// <returns>The result of the task</returns>
    protected async Task<T> WithProgressAsync<T>(string message, Func<Task<T>> task) => !Configuration.Ui.ProgressBars
            ? await task()
            : await AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots)
            .StartAsync(message, async _ => await task());

    /// <summary>
    /// Shows a progress spinner for long-running operations without return value
    /// </summary>
    /// <param name="message">The progress message</param>
    /// <param name="task">The task to execute</param>
    protected async Task WithProgressAsync(string message, Func<Task> task) {
        if (!Configuration.Ui.ProgressBars) {
            await task();
            return;
        }

        await AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots)
            .StartAsync(message, async _ => await task());
    }

    /// <summary>
    /// Validates that the API is reachable
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if API is reachable</returns>
    protected async Task<bool> ValidateApiConnectivityAsync(CancellationToken cancellationToken = default) {
        try {
            var isReachable = await WithProgressAsync(
                "Testing API connectivity...",
                () => ApiClient.TestConnectivityAsync(cancellationToken));

            if (!isReachable) {
                OutputFormatter.WriteError($"Unable to connect to MCP Hub API at {Configuration.Registry.Url}");
                OutputFormatter.WriteInfo("Please check your internet connection and try again.");
                OutputFormatter.WriteInfo($"If the problem persists, you can configure a different API URL with:");
                OutputFormatter.WriteInfo($"  mcpm config set registry.url <new-url>");
                return false;
            }

            return true;
        }
        catch (Exception ex) {
            Logger.LogWarning(ex, "API connectivity check failed");
            OutputFormatter.WriteWarning($"Unable to verify API connectivity: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Parses trust tier string to enum
    /// </summary>
    /// <param name="trustTierString">Trust tier as string</param>
    /// <returns>Trust tier value or null if invalid</returns>
    protected static string? ParseTrustTier(string? trustTierString) {
        if (string.IsNullOrWhiteSpace(trustTierString))
            return null;

        var validTiers = new[] { "Unverified", "Community", "Professional", "Enterprise" };
        var tier = validTiers.FirstOrDefault(t =>
            string.Equals(t, trustTierString, StringComparison.OrdinalIgnoreCase));

        return tier;
    }

    /// <summary>
    /// Validates and normalizes categories
    /// </summary>
    /// <param name="categories">Comma-separated categories</param>
    /// <returns>Normalized category list</returns>
    protected static IEnumerable<string>? ParseCategories(string? categories) => string.IsNullOrWhiteSpace(categories)
            ? null
            : categories.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(c => c.Trim())
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .Distinct(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Validates sort field
    /// </summary>
    /// <param name="sortBy">Sort field</param>
    /// <returns>Valid sort field or null</returns>
    protected static string? ParseSortBy(string? sortBy) {
        if (string.IsNullOrWhiteSpace(sortBy))
            return null;

        var validSortFields = new[] { "name", "downloads", "rating", "created", "updated" };
        return validSortFields.FirstOrDefault(f =>
            string.Equals(f, sortBy, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Validates output format
    /// </summary>
    /// <param name="format">Output format</param>
    /// <returns>Valid format or default</returns>
    protected string ValidateOutputFormat(string? format) {
        if (string.IsNullOrWhiteSpace(format))
            return Configuration.Ui.DefaultFormat;

        var validFormats = new[] { "table", "json", "detailed" };
        return validFormats.FirstOrDefault(f =>
            string.Equals(f, format, StringComparison.OrdinalIgnoreCase)) ?? Configuration.Ui.DefaultFormat;
    }
}