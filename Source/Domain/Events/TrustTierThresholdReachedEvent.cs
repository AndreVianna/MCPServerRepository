using MCPHub.Domain.Entities;
using MCPHub.Domain.Messaging;

namespace MCPHub.Domain.Events;

/// <summary>
/// Event raised when a package reaches a trust tier progression threshold
/// </summary>
public record TrustTierThresholdReachedEvent : BaseMessage {
    /// <summary>
    /// Gets or sets the package identifier
    /// </summary>
    public Guid PackageId { get; init; }

    /// <summary>
    /// Gets or sets the package name
    /// </summary>
    public string PackageName { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the current trust tier
    /// </summary>
    public TrustTier CurrentTier { get; init; }

    /// <summary>
    /// Gets or sets the tier the package is eligible for
    /// </summary>
    public TrustTier EligibleTier { get; init; }

    /// <summary>
    /// Gets or sets the type of threshold reached
    /// </summary>
    public ThresholdType ThresholdType { get; init; }

    /// <summary>
    /// Gets or sets the current trust score
    /// </summary>
    public int CurrentScore { get; init; }

    /// <summary>
    /// Gets or sets the threshold score that was reached
    /// </summary>
    public int ThresholdScore { get; init; }

    /// <summary>
    /// Gets or sets the specific criteria that triggered the threshold
    /// </summary>
    public IEnumerable<string> TriggeredCriteria { get; init; } = Enumerable.Empty<string>();

    /// <summary>
    /// Gets or sets the publisher identifier
    /// </summary>
    public Guid PublisherId { get; init; }

    /// <summary>
    /// Gets or sets when the threshold was reached
    /// </summary>
    public DateTimeOffset ThresholdReachedAt { get; init; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets or sets whether automatic promotion is enabled
    /// </summary>
    public bool AutoPromotionEnabled { get; init; } = false;

    /// <summary>
    /// Gets or sets whether manual review is required
    /// </summary>
    public bool RequiresManualReview { get; init; } = true;

    /// <summary>
    /// Gets or sets additional threshold metadata
    /// </summary>
    public Dictionary<string, object> ThresholdMetadata { get; init; } = [];

    /// <summary>
    /// Gets or sets the assessment ID that detected the threshold
    /// </summary>
    public Guid? AssessmentId { get; init; }

    /// <summary>
    /// Gets whether this represents eligibility for promotion
    /// </summary>
    public bool IsPromotionEligibility => EligibleTier > CurrentTier;

    /// <summary>
    /// Gets whether this represents risk for demotion
    /// </summary>
    public bool IsDemotionRisk => EligibleTier < CurrentTier;
}

/// <summary>
/// Represents the type of trust tier threshold that was reached
/// </summary>
public enum ThresholdType {
    /// <summary>
    /// Package has met criteria for promotion to next tier
    /// </summary>
    PromotionEligibility = 0,

    /// <summary>
    /// Package is at risk for demotion due to declining metrics
    /// </summary>
    DemotionRisk = 1,

    /// <summary>
    /// Package has critical issues requiring immediate attention
    /// </summary>
    CriticalIssue = 2,

    /// <summary>
    /// Package has achieved exceptional performance metrics
    /// </summary>
    ExceptionalPerformance = 3,

    /// <summary>
    /// Package security status has changed significantly
    /// </summary>
    SecurityStatusChange = 4,

    /// <summary>
    /// Package community metrics have changed significantly
    /// </summary>
    CommunityStatusChange = 5,

    /// <summary>
    /// Publisher verification status has changed
    /// </summary>
    PublisherStatusChange = 6,
}