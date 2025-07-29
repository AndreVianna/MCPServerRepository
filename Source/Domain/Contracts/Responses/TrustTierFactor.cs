namespace MCPHub.Domain.Contracts.Responses;

/// <summary>
/// Represents a specific factor that contributes to trust tier calculation
/// </summary>
public class TrustTierFactor {
    /// <summary>
    /// Gets or sets the name of the trust factor
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the current score for this factor
    /// </summary>
    public int Score { get; set; }

    /// <summary>
    /// Gets or sets the maximum possible score for this factor
    /// </summary>
    public int MaxScore { get; set; }

    /// <summary>
    /// Gets or sets the weight of this factor in the overall calculation (0.0 to 1.0)
    /// </summary>
    public decimal Weight { get; set; }

    /// <summary>
    /// Gets or sets the weighted score (Score * Weight)
    /// </summary>
    public decimal WeightedScore => Score * Weight;

    /// <summary>
    /// Gets or sets a human-readable description of this factor
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the status of this factor
    /// </summary>
    public FactorStatus Status { get; set; } = FactorStatus.Unknown;

    /// <summary>
    /// Gets or sets the impact level of this factor
    /// </summary>
    public FactorImpact Impact { get; set; } = FactorImpact.Low;

    /// <summary>
    /// Gets or sets when this factor was last evaluated
    /// </summary>
    public DateTimeOffset LastEvaluated { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets or sets additional details about this factor
    /// </summary>
    public Dictionary<string, object> Details { get; set; } = new();

    /// <summary>
    /// Gets or sets recommendations for improving this factor
    /// </summary>
    public IEnumerable<string> Recommendations { get; set; } = Enumerable.Empty<string>();
}

/// <summary>
/// Represents the status of a trust tier factor
/// </summary>
public enum FactorStatus {
    /// <summary>
    /// Factor status is unknown or not yet evaluated
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// Factor has a positive impact on trust
    /// </summary>
    Positive = 1,

    /// <summary>
    /// Factor has a negative impact on trust
    /// </summary>
    Negative = 2,

    /// <summary>
    /// Factor has a neutral impact on trust
    /// </summary>
    Neutral = 3,

    /// <summary>
    /// Factor requires attention or improvement
    /// </summary>
    NeedsAttention = 4
}

/// <summary>
/// Represents the impact level of a trust tier factor
/// </summary>
public enum FactorImpact {
    /// <summary>
    /// Low impact on overall trust score
    /// </summary>
    Low = 0,

    /// <summary>
    /// Medium impact on overall trust score
    /// </summary>
    Medium = 1,

    /// <summary>
    /// High impact on overall trust score
    /// </summary>
    High = 2,

    /// <summary>
    /// Critical impact on overall trust score
    /// </summary>
    Critical = 3
}