using MCPHub.CommandLineApp.Models;

namespace MCPHub.CommandLineApp.Utilities;

/// <summary>
/// Interface for formatting and displaying output in the CLI
/// </summary>
public interface IOutputFormatter
{
    /// <summary>
    /// Writes a success message
    /// </summary>
    /// <param name="message">The message to write</param>
    void WriteSuccess(string message);

    /// <summary>
    /// Writes an error message
    /// </summary>
    /// <param name="message">The error message to write</param>
    void WriteError(string message);

    /// <summary>
    /// Writes a warning message
    /// </summary>
    /// <param name="message">The warning message to write</param>
    void WriteWarning(string message);

    /// <summary>
    /// Writes an informational message
    /// </summary>
    /// <param name="message">The info message to write</param>
    void WriteInfo(string message);

    /// <summary>
    /// Writes a debug message (only if verbose mode is enabled)
    /// </summary>
    /// <param name="message">The debug message to write</param>
    void WriteDebug(string message);

    /// <summary>
    /// Writes search results in the specified format
    /// </summary>
    /// <param name="results">Search results to display</param>
    /// <param name="format">Output format (table, json, detailed)</param>
    void WriteSearchResults(SearchResultResponse results, string format);

    /// <summary>
    /// Writes package information in the specified format
    /// </summary>
    /// <param name="packageInfo">Package information to display</param>
    /// <param name="format">Output format (table, json, detailed)</param>
    void WritePackageInfo(PackageInfoResponse packageInfo, string format);

    /// <summary>
    /// Writes package list in the specified format
    /// </summary>
    /// <param name="packages">Package list to display</param>
    /// <param name="format">Output format (table, json, detailed)</param>
    void WritePackageList(PackageListResponse packages, string format);

    /// <summary>
    /// Writes package versions in the specified format
    /// </summary>
    /// <param name="versions">Package versions to display</param>
    /// <param name="format">Output format (table, json, detailed)</param>
    void WritePackageVersions(PackageVersionsResponse versions, string format);

    /// <summary>
    /// Writes security summary information
    /// </summary>
    /// <param name="security">Security summary to display</param>
    /// <param name="format">Output format (table, json, detailed)</param>
    void WriteSecuritySummary(SecuritySummaryResponse security, string format);

    /// <summary>
    /// Writes trust tier information
    /// </summary>
    /// <param name="trustTier">Trust tier information to display</param>
    /// <param name="format">Output format (table, json, detailed)</param>
    void WriteTrustTier(TrustTierResponse trustTier, string format);

    /// <summary>
    /// Writes JSON output
    /// </summary>
    /// <param name="data">Data to serialize as JSON</param>
    void WriteJson(object data);

    /// <summary>
    /// Writes a blank line
    /// </summary>
    void WriteLine();

    /// <summary>
    /// Gets the trust tier color for display
    /// </summary>
    /// <param name="trustTier">Trust tier name</param>
    /// <returns>Color for the trust tier</returns>
    string GetTrustTierColor(string trustTier);

    /// <summary>
    /// Gets the security grade color for display
    /// </summary>
    /// <param name="grade">Security grade</param>
    /// <returns>Color for the security grade</returns>
    string GetSecurityGradeColor(string grade);

    /// <summary>
    /// Formats a file size for display
    /// </summary>
    /// <param name="bytes">Size in bytes</param>
    /// <returns>Formatted size string</returns>
    string FormatFileSize(long bytes);

    /// <summary>
    /// Formats a download count for display
    /// </summary>
    /// <param name="count">Download count</param>
    /// <returns>Formatted count string</returns>
    string FormatDownloadCount(long count);

    /// <summary>
    /// Formats a date for display
    /// </summary>
    /// <param name="date">Date to format</param>
    /// <returns>Formatted date string</returns>
    string FormatDate(DateTimeOffset date);

    /// <summary>
    /// Formats a relative date for display (e.g., "2 days ago")
    /// </summary>
    /// <param name="date">Date to format</param>
    /// <returns>Relative date string</returns>
    string FormatRelativeDate(DateTimeOffset date);

    /// <summary>
    /// Truncates text to fit within specified width
    /// </summary>
    /// <param name="text">Text to truncate</param>
    /// <param name="maxWidth">Maximum width</param>
    /// <returns>Truncated text</returns>
    string TruncateText(string text, int maxWidth);

    /// <summary>
    /// Writes publishing result information
    /// </summary>
    /// <param name="result">Publishing result to display</param>
    /// <param name="format">Output format (table, json, detailed)</param>
    void WritePublishResult(PublishPackageResponse result, string format);

    /// <summary>
    /// Writes manifest validation result information
    /// </summary>
    /// <param name="result">Validation result to display</param>
    /// <param name="format">Output format (table, json, detailed)</param>
    void WriteValidationResult(ValidateManifestResponse result, string format);
}