using MCPHub.Domain.Entities;

namespace MCPHub.Domain.Contracts.Responses;

/// <summary>
/// Represents a historical record of trust tier changes for a package
/// </summary>
public class TrustTierHistory {
    /// <summary>
    /// Gets or sets the unique identifier for this history record
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the package identifier this history belongs to
    /// </summary>
    public Guid PackageId { get; set; }

    /// <summary>
    /// Gets or sets the trust tier before the change
    /// </summary>
    public TrustTier FromTier { get; set; }

    /// <summary>
    /// Gets or sets the trust tier after the change
    /// </summary>
    public TrustTier ToTier { get; set; }

    /// <summary>
    /// Gets or sets the reason for the tier change
    /// </summary>
    public string Reason { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets when the tier change occurred
    /// </summary>
    public DateTimeOffset ChangedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets or sets the user who initiated the tier change (null for system changes)
    /// </summary>
    public Guid? ChangedByUserId { get; set; }

    /// <summary>
    /// Gets or sets the type of change that occurred
    /// </summary>
    public TierChangeType ChangeType { get; set; }

    /// <summary>
    /// Gets or sets whether this was an automatic or manual change
    /// </summary>
    public bool IsAutomatic { get; set; } = true;

    /// <summary>
    /// Gets or sets the trust score at the time of change
    /// </summary>
    public int? TrustScoreAtChange { get; set; }

    /// <summary>
    /// Gets or sets additional metadata about the tier change
    /// </summary>
    public Dictionary<string, object>? Metadata { get; set; }

    /// <summary>
    /// Gets or sets the assessment ID that triggered this change
    /// </summary>
    public Guid? AssessmentId { get; set; }

    /// <summary>
    /// Gets whether this represents a promotion (tier increase)
    /// </summary>
    public bool IsPromotion => ToTier > FromTier;

    /// <summary>
    /// Gets whether this represents a demotion (tier decrease)
    /// </summary>
    public bool IsDemotion => ToTier < FromTier;
}

/// <summary>
/// Represents the type of trust tier change
/// </summary>
public enum TierChangeType {
    /// <summary>
    /// Initial tier assignment for a new package
    /// </summary>
    Initial = 0,

    /// <summary>
    /// Promotion to a higher tier
    /// </summary>
    Promotion = 1,

    /// <summary>
    /// Demotion to a lower tier
    /// </summary>
    Demotion = 2,

    /// <summary>
    /// Manual override by administrator
    /// </summary>
    ManualOverride = 3,

    /// <summary>
    /// Automatic recalculation
    /// </summary>
    AutomaticRecalculation = 4,

    /// <summary>
    /// Emergency security-related demotion
    /// </summary>
    SecurityDemotion = 5,

    /// <summary>
    /// Restoration after issue resolution
    /// </summary>
    Restoration = 6,
}