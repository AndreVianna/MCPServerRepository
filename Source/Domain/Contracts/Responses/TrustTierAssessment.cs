using MCPHub.Domain.Entities;

namespace MCPHub.Domain.Contracts.Responses;

/// <summary>
/// Represents a comprehensive trust tier assessment for a package
/// </summary>
public class TrustTierAssessment
{
    /// <summary>
    /// Gets or sets the current trust tier of the package
    /// </summary>
    public TrustTier CurrentTier { get; set; } = TrustTier.Unverified;

    /// <summary>
    /// Gets or sets the recommended trust tier based on current metrics
    /// </summary>
    public TrustTier RecommendedTier { get; set; } = TrustTier.Unverified;

    /// <summary>
    /// Gets or sets the total calculated trust score
    /// </summary>
    public int TotalScore { get; set; }

    /// <summary>
    /// Gets or sets the maximum possible trust score
    /// </summary>
    public int MaxScore { get; set; }

    /// <summary>
    /// Gets or sets the trust score as a percentage (0-100)
    /// </summary>
    public decimal ScorePercentage => MaxScore > 0 ? (decimal)TotalScore / MaxScore * 100 : 0;

    /// <summary>
    /// Gets or sets the detailed factors that contribute to the trust score
    /// </summary>
    public Dictionary<string, TrustTierFactor> Factors { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of positive factors that improve trust
    /// </summary>
    public IEnumerable<string> PositiveFactors { get; set; } = Enumerable.Empty<string>();

    /// <summary>
    /// Gets or sets the list of negative factors that reduce trust
    /// </summary>
    public IEnumerable<string> NegativeFactors { get; set; } = Enumerable.Empty<string>();

    /// <summary>
    /// Gets or sets when this assessment was last calculated
    /// </summary>
    public DateTimeOffset LastAssessment { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets or sets the previous trust tier before the current one
    /// </summary>
    public TrustTier? PreviousTier { get; set; }

    /// <summary>
    /// Gets or sets when the next automatic reassessment should occur
    /// </summary>
    public DateTimeOffset? NextAssessment { get; set; }

    /// <summary>
    /// Gets or sets whether the package is eligible for tier promotion
    /// </summary>
    public bool EligibleForPromotion { get; set; }

    /// <summary>
    /// Gets or sets whether the package is at risk for tier demotion
    /// </summary>
    public bool AtRiskForDemotion { get; set; }

    /// <summary>
    /// Gets or sets additional metadata about the assessment
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}