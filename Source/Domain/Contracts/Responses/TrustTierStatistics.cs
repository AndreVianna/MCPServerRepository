using MCPHub.Domain.Entities;

namespace MCPHub.Domain.Contracts.Responses;

/// <summary>
/// Represents overall trust tier statistics across the platform
/// </summary>
public class TrustTierStatistics
{
    /// <summary>
    /// Gets or sets the total number of packages in the system
    /// </summary>
    public int TotalPackages { get; set; }

    /// <summary>
    /// Gets or sets the distribution of packages by trust tier
    /// </summary>
    public Dictionary<TrustTier, int> TierDistribution { get; set; } = new();

    /// <summary>
    /// Gets or sets the distribution of packages by trust tier as percentages
    /// </summary>
    public Dictionary<TrustTier, decimal> TierPercentages { get; set; } = new();

    /// <summary>
    /// Gets or sets the average trust score across all packages
    /// </summary>
    public decimal AverageTrustScore { get; set; }

    /// <summary>
    /// Gets or sets the number of tier changes in the last period
    /// </summary>
    public int RecentTierChanges { get; set; }

    /// <summary>
    /// Gets or sets the number of promotions in the last period
    /// </summary>
    public int RecentPromotions { get; set; }

    /// <summary>
    /// Gets or sets the number of demotions in the last period
    /// </summary>
    public int RecentDemotions { get; set; }

    /// <summary>
    /// Gets or sets the time period for recent statistics (in days)
    /// </summary>
    public int StatisticsPeriodDays { get; set; } = 30;

    /// <summary>
    /// Gets or sets when these statistics were last calculated
    /// </summary>
    public DateTimeOffset LastCalculated { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets or sets the top trust tier factors contributing to promotions
    /// </summary>
    public IEnumerable<string> TopPromotionFactors { get; set; } = Enumerable.Empty<string>();

    /// <summary>
    /// Gets or sets the top trust tier factors contributing to demotions
    /// </summary>
    public IEnumerable<string> TopDemotionFactors { get; set; } = Enumerable.Empty<string>();

    /// <summary>
    /// Gets or sets trending statistics for trust tiers
    /// </summary>
    public Dictionary<TrustTier, TrustTierTrend> TierTrends { get; set; } = new();

    /// <summary>
    /// Gets or sets additional platform-wide trust metrics
    /// </summary>
    public Dictionary<string, object> AdditionalMetrics { get; set; } = new();
}

/// <summary>
/// Represents trend data for a specific trust tier
/// </summary>
public class TrustTierTrend
{
    /// <summary>
    /// Gets or sets the current count of packages in this tier
    /// </summary>
    public int CurrentCount { get; set; }

    /// <summary>
    /// Gets or sets the count from the previous period
    /// </summary>
    public int PreviousCount { get; set; }

    /// <summary>
    /// Gets or sets the change in package count
    /// </summary>
    public int Change => CurrentCount - PreviousCount;

    /// <summary>
    /// Gets or sets the percentage change
    /// </summary>
    public decimal ChangePercentage => PreviousCount > 0 ? (decimal)Change / PreviousCount * 100 : 0;

    /// <summary>
    /// Gets whether the tier is trending upward
    /// </summary>
    public bool IsTrendingUp => Change > 0;

    /// <summary>
    /// Gets whether the tier is trending downward
    /// </summary>
    public bool IsTrendingDown => Change < 0;

    /// <summary>
    /// Gets or sets the trend direction
    /// </summary>
    public TrendDirection Direction => Change > 0 ? TrendDirection.Up : 
                                     Change < 0 ? TrendDirection.Down : TrendDirection.Stable;
}

/// <summary>
/// Represents the direction of a trend
/// </summary>
public enum TrendDirection
{
    /// <summary>
    /// Trend is stable (no significant change)
    /// </summary>
    Stable = 0,

    /// <summary>
    /// Trend is upward (increasing)
    /// </summary>
    Up = 1,

    /// <summary>
    /// Trend is downward (decreasing)
    /// </summary>
    Down = 2
}