namespace MCPHub.CommandLineApp.Services;

/// <summary>
/// CLI service interface for trust tier operations and assessment
/// </summary>
public interface ITrustTierService {
    /// <summary>
    /// Gets trust tier assessment for a package
    /// </summary>
    /// <param name="packageName">Package name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Trust tier assessment</returns>
    Task<TrustTierAssessment> GetTrustTierAssessmentAsync(
        string packageName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets trust tier history for a package
    /// </summary>
    /// <param name="packageName">Package name</param>
    /// <param name="limit">Maximum number of history records</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Trust tier history</returns>
    Task<IEnumerable<TrustTierHistory>> GetTrustTierHistoryAsync(
        string packageName,
        int limit = 10,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates if package meets trust tier requirements
    /// </summary>
    /// <param name="packageName">Package name</param>
    /// <param name="targetTier">Target trust tier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Validation result</returns>
    Task<TrustTierValidationResult> ValidateTrustTierEligibilityAsync(
        string packageName,
        TrustTier targetTier,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets platform-wide trust tier statistics
    /// </summary>
    /// <param name="periodDays">Period in days for statistics</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Trust tier statistics</returns>
    Task<TrustTierStatistics> GetTrustTierStatisticsAsync(
        int periodDays = 30,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if packages meet minimum trust tier requirements
    /// </summary>
    /// <param name="packages">Package specifications to check</param>
    /// <param name="minimumTier">Minimum trust tier required</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Trust tier compliance results</returns>
    Task<IEnumerable<TrustTierComplianceResult>> CheckTrustTierComplianceAsync(
        IEnumerable<PackageSpec> packages,
        TrustTier minimumTier,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets recommended trust tier actions for user's packages
    /// </summary>
    /// <param name="includeGlobal">Include globally installed packages</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Recommended trust tier actions</returns>
    Task<IEnumerable<TrustTierRecommendation>> GetTrustTierRecommendationsAsync(
        bool includeGlobal = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Calculates trust score breakdown for educational purposes
    /// </summary>
    /// <param name="packageName">Package name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Trust score breakdown</returns>
    Task<TrustScoreBreakdown> GetTrustScoreBreakdownAsync(
        string packageName,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Package specification for trust tier checking
/// </summary>
public record PackageSpec {
    public string Name { get; init; } = string.Empty;
    public string Version { get; init; } = string.Empty;
    public TrustTier CurrentTier { get; init; }
}

/// <summary>
/// Result of trust tier compliance check
/// </summary>
public record TrustTierComplianceResult {
    public string PackageName { get; init; } = string.Empty;
    public string Version { get; init; } = string.Empty;
    public TrustTier CurrentTier { get; init; }
    public TrustTier RequiredTier { get; init; }
    public bool IsCompliant { get; init; }
    public string ComplianceStatus { get; init; } = string.Empty;
    public IEnumerable<string> Issues { get; init; } = Enumerable.Empty<string>();
    public IEnumerable<string> Recommendations { get; init; } = Enumerable.Empty<string>();
}

/// <summary>
/// Trust tier recommendation
/// </summary>
public record TrustTierRecommendation {
    public string PackageName { get; init; } = string.Empty;
    public TrustTier CurrentTier { get; init; }
    public TrustTier RecommendedTier { get; init; }
    public string Action { get; init; } = string.Empty;
    public string Reason { get; init; } = string.Empty;
    public int Priority { get; init; }
    public DateTimeOffset? Deadline { get; init; }
}

/// <summary>
/// Detailed trust score breakdown for educational purposes
/// </summary>
public record TrustScoreBreakdown {
    public string PackageName { get; init; } = string.Empty;
    public TrustTier CurrentTier { get; init; }
    public int TotalScore { get; init; }
    public int MaxScore { get; init; }
    public decimal ScorePercentage { get; init; }
    public Dictionary<string, TrustScoreFactor> Factors { get; init; } = new();
    public IEnumerable<string> PositiveFactors { get; init; } = Enumerable.Empty<string>();
    public IEnumerable<string> NegativeFactors { get; init; } = Enumerable.Empty<string>();
    public IEnumerable<string> ImprovementSuggestions { get; init; } = Enumerable.Empty<string>();
}

/// <summary>
/// Individual trust score factor
/// </summary>
public record TrustScoreFactor {
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public int Score { get; init; }
    public int MaxScore { get; init; }
    public decimal Weight { get; init; }
    public string Impact { get; init; } = string.Empty;
}