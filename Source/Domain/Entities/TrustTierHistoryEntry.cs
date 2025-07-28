using System.Diagnostics.CodeAnalysis;
using MCPHub.Domain.Common;
using MCPHub.Domain.Contracts.Responses;

namespace MCPHub.Domain.Entities;

/// <summary>
/// Represents a historical record of trust tier changes for a package
/// </summary>
public class TrustTierHistoryEntry : BaseEntity
{
    /// <summary>
    /// Gets or sets the package identifier this history belongs to
    /// </summary>
    public Guid PackageId { get; set; }

    /// <summary>
    /// Gets or sets the navigation property to the package
    /// </summary>
    public Package Package { get; set; } = null!;

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
    public Dictionary<string, object> Metadata { get; set; } = new();

    /// <summary>
    /// Gets or sets the assessment ID that triggered this change
    /// </summary>
    public Guid? AssessmentId { get; set; }

    private TrustTierHistoryEntry() { } // For EF Core

    [SetsRequiredMembers]
    public TrustTierHistoryEntry(
        Guid packageId,
        TrustTier fromTier,
        TrustTier toTier,
        string reason,
        TierChangeType changeType,
        Guid? changedByUserId = null,
        bool isAutomatic = true,
        int? trustScoreAtChange = null,
        Dictionary<string, object>? metadata = null,
        Guid? assessmentId = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);

        PackageId = packageId;
        FromTier = fromTier;
        ToTier = toTier;
        Reason = reason;
        ChangeType = changeType;
        ChangedByUserId = changedByUserId;
        IsAutomatic = isAutomatic;
        ChangedAt = DateTimeOffset.UtcNow;
        TrustScoreAtChange = trustScoreAtChange;
        Metadata = metadata ?? new Dictionary<string, object>();
        AssessmentId = assessmentId;

        AuditTrail.Add(new AuditEntry
        {
            Action = $"Trust Tier Changed from {fromTier} to {toTier}",
            UserId = changedByUserId ?? Guid.Empty,
            DateTime = DateTimeOffset.UtcNow
        });
    }

    /// <summary>
    /// Gets whether this represents a promotion (tier increase)
    /// </summary>
    public bool IsPromotion => ToTier > FromTier;

    /// <summary>
    /// Gets whether this represents a demotion (tier decrease)
    /// </summary>
    public bool IsDemotion => ToTier < FromTier;

    /// <summary>
    /// Gets the tier change magnitude (absolute difference)
    /// </summary>
    public int ChangeMagnitude => Math.Abs((int)ToTier - (int)FromTier);

    /// <summary>
    /// Updates the metadata for this history entry
    /// </summary>
    /// <param name="newMetadata">New metadata to add or update</param>
    /// <param name="userId">User performing the update</param>
    public void UpdateMetadata(Dictionary<string, object> newMetadata, Guid? userId = null)
    {
        ArgumentNullException.ThrowIfNull(newMetadata);

        foreach (var kvp in newMetadata)
        {
            Metadata[kvp.Key] = kvp.Value;
        }

        AuditTrail.Add(new AuditEntry
        {
            Action = "Trust Tier History Metadata Updated",
            UserId = userId ?? Guid.Empty,
            DateTime = DateTimeOffset.UtcNow
        });
    }
}