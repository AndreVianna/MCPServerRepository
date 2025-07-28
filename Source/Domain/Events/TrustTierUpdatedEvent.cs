using MCPHub.Domain.Contracts.Responses;
using MCPHub.Domain.Entities;
using MCPHub.Domain.Messaging;

namespace MCPHub.Domain.Events;

/// <summary>
/// Event raised when a package's trust tier is updated
/// </summary>
public record TrustTierUpdatedEvent : BaseMessage
{
    /// <summary>
    /// Gets or sets the package identifier
    /// </summary>
    public Guid PackageId { get; init; }

    /// <summary>
    /// Gets or sets the package name
    /// </summary>
    public string PackageName { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the previous trust tier
    /// </summary>
    public TrustTier PreviousTier { get; init; }

    /// <summary>
    /// Gets or sets the new trust tier
    /// </summary>
    public TrustTier NewTier { get; init; }

    /// <summary>
    /// Gets or sets the reason for the tier change
    /// </summary>
    public string Reason { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the type of change that occurred
    /// </summary>
    public TierChangeType ChangeType { get; init; }

    /// <summary>
    /// Gets or sets whether this was an automatic change
    /// </summary>
    public bool IsAutomatic { get; init; } = true;

    /// <summary>
    /// Gets or sets the user who initiated the change (null for system changes)
    /// </summary>
    public Guid? ChangedByUserId { get; init; }

    /// <summary>
    /// Gets or sets the trust score at the time of change
    /// </summary>
    public int? TrustScoreAtChange { get; init; }

    /// <summary>
    /// Gets or sets the publisher identifier
    /// </summary>
    public Guid PublisherId { get; init; }

    /// <summary>
    /// Gets or sets when the tier change occurred
    /// </summary>
    public DateTimeOffset ChangedAt { get; init; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets or sets additional metadata about the change
    /// </summary>
    public new Dictionary<string, object> TrustTierMetadata { get; init; } = new();

    /// <summary>
    /// Gets whether this represents a promotion (tier increase)
    /// </summary>
    public bool IsPromotion => NewTier > PreviousTier;

    /// <summary>
    /// Gets whether this represents a demotion (tier decrease)
    /// </summary>
    public bool IsDemotion => NewTier < PreviousTier;

    /// <summary>
    /// Gets the tier change magnitude (absolute difference)
    /// </summary>
    public int ChangeMagnitude => Math.Abs((int)NewTier - (int)PreviousTier);
}