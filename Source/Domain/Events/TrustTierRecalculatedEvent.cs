using MCPHub.Domain.Contracts.Responses;
using MCPHub.Domain.Entities;
using MCPHub.Domain.Messaging;

namespace MCPHub.Domain.Events;

/// <summary>
/// Event raised when trust tier recalculation is completed
/// </summary>
public record TrustTierRecalculatedEvent : BaseMessage {
    /// <summary>
    /// Gets or sets the package identifier
    /// </summary>
    public Guid PackageId { get; init; }

    /// <summary>
    /// Gets or sets the package name
    /// </summary>
    public string PackageName { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the calculated trust tier
    /// </summary>
    public TrustTier CalculatedTier { get; init; }

    /// <summary>
    /// Gets or sets the current trust tier before recalculation
    /// </summary>
    public TrustTier CurrentTier { get; init; }

    /// <summary>
    /// Gets or sets the total trust score calculated
    /// </summary>
    public int TotalScore { get; init; }

    /// <summary>
    /// Gets or sets the maximum possible score
    /// </summary>
    public int MaxScore { get; init; }

    /// <summary>
    /// Gets or sets whether the package is eligible for tier promotion
    /// </summary>
    public bool EligibleForPromotion { get; init; }

    /// <summary>
    /// Gets or sets whether the package is at risk for tier demotion
    /// </summary>
    public bool AtRiskForDemotion { get; init; }

    /// <summary>
    /// Gets or sets the assessment that was performed
    /// </summary>
    public TrustTierAssessment Assessment { get; init; } = null!;

    /// <summary>
    /// Gets or sets the assessment identifier
    /// </summary>
    public Guid AssessmentId { get; init; }

    /// <summary>
    /// Gets or sets when the recalculation was performed
    /// </summary>
    public DateTimeOffset RecalculatedAt { get; init; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets or sets the duration of the recalculation process
    /// </summary>
    public TimeSpan CalculationDuration { get; init; }

    /// <summary>
    /// Gets or sets whether this was triggered by an automatic process
    /// </summary>
    public bool IsAutomaticRecalculation { get; init; } = true;

    /// <summary>
    /// Gets or sets the user who triggered the recalculation (null for automatic)
    /// </summary>
    public Guid? TriggeredByUserId { get; init; }

    /// <summary>
    /// Gets or sets additional recalculation metadata
    /// </summary>
    public new Dictionary<string, object> RecalculationMetadata { get; init; } = new();

    /// <summary>
    /// Gets whether the calculated tier differs from current tier
    /// </summary>
    public bool TierShouldChange => CalculatedTier != CurrentTier;

    /// <summary>
    /// Gets the score as a percentage
    /// </summary>
    public decimal ScorePercentage => MaxScore > 0 ? (decimal)TotalScore / MaxScore * 100 : 0;
}