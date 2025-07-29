using MCPHub.Domain.ValueObjects;

namespace MCPHub.Domain.Contracts.Services;

/// <summary>
/// Interface for static security analyzers
/// </summary>
public interface IStaticAnalyzer {
    /// <summary>
    /// Name of the analyzer
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Version of the analyzer
    /// </summary>
    string Version { get; }

    /// <summary>
    /// Types of scans this analyzer supports
    /// </summary>
    IReadOnlyList<string> SupportedScanTypes { get; }

    /// <summary>
    /// Performs static analysis on the provided content
    /// </summary>
    /// <param name="content">Content to analyze</param>
    /// <param name="contentType">Type of content (manifest, archive, etc.)</param>
    /// <param name="options">Analysis options</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Analysis result</returns>
    Task<SecurityAnalysisResult> AnalyzeAsync(
        string content,
        string contentType,
        Dictionary<string, object>? options = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs static analysis on a file
    /// </summary>
    /// <param name="filePath">Path to the file to analyze</param>
    /// <param name="options">Analysis options</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Analysis result</returns>
    Task<SecurityAnalysisResult> AnalyzeFileAsync(
        string filePath,
        Dictionary<string, object>? options = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates the analyzer configuration
    /// </summary>
    /// <returns>True if the analyzer is properly configured</returns>
    Task<bool> ValidateConfigurationAsync();

    /// <summary>
    /// Gets the health status of the analyzer
    /// </summary>
    /// <returns>Health status information</returns>
    Task<Dictionary<string, object>> GetHealthStatusAsync();
}