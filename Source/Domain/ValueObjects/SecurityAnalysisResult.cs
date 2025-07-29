using System.Diagnostics.CodeAnalysis;

namespace MCPHub.Domain.ValueObjects;

/// <summary>
/// Result of a specific security analyzer run
/// </summary>
public record SecurityAnalysisResult {
    /// <summary>
    /// Name of the analyzer that produced this result
    /// </summary>
    [MaxLength(128)]
    public string AnalyzerName { get; init; } = string.Empty;

    /// <summary>
    /// Version of the analyzer used
    /// </summary>
    [MaxLength(32)]
    public string AnalyzerVersion { get; init; } = string.Empty;

    /// <summary>
    /// Vulnerabilities found by this analyzer
    /// </summary>
    public IReadOnlyList<SecurityVulnerability> Vulnerabilities { get; init; } = [];

    /// <summary>
    /// Warning messages from the analyzer
    /// </summary>
    public IReadOnlyList<string> Warnings { get; init; } = [];

    /// <summary>
    /// Additional metadata from the analyzer
    /// </summary>
    public IReadOnlyDictionary<string, object> Metadata { get; init; } = new Dictionary<string, object>();

    /// <summary>
    /// Time taken to complete the analysis
    /// </summary>
    public TimeSpan AnalysisTime { get; init; }

    /// <summary>
    /// Whether the analysis completed successfully
    /// </summary>
    public bool Success { get; init; }

    /// <summary>
    /// Error message if the analysis failed
    /// </summary>
    [MaxLength(1024)]
    public string? ErrorMessage { get; init; }

    private SecurityAnalysisResult() { } // For EF Core

    [SetsRequiredMembers]
    public SecurityAnalysisResult(
        string analyzerName,
        string analyzerVersion,
        IEnumerable<SecurityVulnerability> vulnerabilities,
        IEnumerable<string> warnings,
        IDictionary<string, object> metadata,
        TimeSpan analysisTime,
        bool success,
        string? errorMessage = null) {
        if (string.IsNullOrWhiteSpace(analyzerName))
            throw new ArgumentException("Analyzer name cannot be null or empty", nameof(analyzerName));
        if (string.IsNullOrWhiteSpace(analyzerVersion))
            throw new ArgumentException("Analyzer version cannot be null or empty", nameof(analyzerVersion));

        AnalyzerName = analyzerName;
        AnalyzerVersion = analyzerVersion;
        Vulnerabilities = vulnerabilities.ToList().AsReadOnly();
        Warnings = warnings.ToList().AsReadOnly();
        Metadata = metadata.ToDictionary(kv => kv.Key, kv => kv.Value);
        AnalysisTime = analysisTime;
        Success = success;
        ErrorMessage = errorMessage;
    }
}