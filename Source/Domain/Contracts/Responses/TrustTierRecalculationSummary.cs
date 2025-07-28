using MCPHub.Domain.Entities;

namespace MCPHub.Domain.Contracts.Responses;

/// <summary>
/// Represents the summary of a trust tier recalculation operation
/// </summary>
public class TrustTierRecalculationSummary
{
    /// <summary>
    /// Gets or sets the total number of packages processed
    /// </summary>
    public int TotalPackagesProcessed { get; set; }

    /// <summary>
    /// Gets or sets the number of packages that had tier changes
    /// </summary>
    public int PackagesWithChanges { get; set; }

    /// <summary>
    /// Gets or sets the number of packages that were promoted
    /// </summary>
    public int Promotions { get; set; }

    /// <summary>
    /// Gets or sets the number of packages that were demoted
    /// </summary>
    public int Demotions { get; set; }

    /// <summary>
    /// Gets or sets the number of packages that had calculation errors
    /// </summary>
    public int Errors { get; set; }

    /// <summary>
    /// Gets or sets when the recalculation started
    /// </summary>
    public DateTimeOffset StartTime { get; set; }

    /// <summary>
    /// Gets or sets when the recalculation completed
    /// </summary>
    public DateTimeOffset EndTime { get; set; }

    /// <summary>
    /// Gets the total duration of the recalculation
    /// </summary>
    public TimeSpan Duration => EndTime - StartTime;

    /// <summary>
    /// Gets or sets the detailed results by trust tier
    /// </summary>
    public Dictionary<TrustTier, TrustTierChangeCount> TierChanges { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of package IDs that had errors during recalculation
    /// </summary>
    public IEnumerable<Guid> FailedPackages { get; set; } = Enumerable.Empty<Guid>();

    /// <summary>
    /// Gets or sets additional performance metrics
    /// </summary>
    public Dictionary<string, object> PerformanceMetrics { get; set; } = new();

    /// <summary>
    /// Gets whether the recalculation was successful overall
    /// </summary>
    public bool IsSuccessful => Errors == 0 || (double)Errors / TotalPackagesProcessed < 0.05; // Less than 5% error rate

    /// <summary>
    /// Gets the success rate as a percentage
    /// </summary>
    public decimal SuccessRate => TotalPackagesProcessed > 0 ? 
        (decimal)(TotalPackagesProcessed - Errors) / TotalPackagesProcessed * 100 : 100;
}

/// <summary>
/// Represents the count of changes for a specific trust tier
/// </summary>
public class TrustTierChangeCount
{
    /// <summary>
    /// Gets or sets the number of packages promoted to this tier
    /// </summary>
    public int PromotedTo { get; set; }

    /// <summary>
    /// Gets or sets the number of packages demoted from this tier
    /// </summary>
    public int DemotedFrom { get; set; }

    /// <summary>
    /// Gets the net change for this tier
    /// </summary>
    public int NetChange => PromotedTo - DemotedFrom;

    /// <summary>
    /// Gets or sets the total number of packages currently in this tier
    /// </summary>
    public int CurrentCount { get; set; }
}