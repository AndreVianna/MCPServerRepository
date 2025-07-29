using MCPHub.Domain.Contracts.Responses;
using MCPHub.Domain.Entities;

namespace MCPHub.Domain.Repositories;

/// <summary>
/// Repository interface for trust tier history operations
/// </summary>
public interface ITrustTierHistoryRepository : IRepository<TrustTierHistoryEntry> {
    /// <summary>
    /// Gets the trust tier change history for a specific package
    /// </summary>
    /// <param name="packageId">The package identifier</param>
    /// <param name="limit">Maximum number of history entries to return</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Chronological history of trust tier changes</returns>
    Task<IEnumerable<TrustTierHistoryEntry>> GetPackageHistoryAsync(
        Guid packageId,
        int limit = 50,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the most recent trust tier change for a package
    /// </summary>
    /// <param name="packageId">The package identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The most recent trust tier change or null if none exists</returns>
    Task<TrustTierHistoryEntry?> GetLatestHistoryEntryAsync(
        Guid packageId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets trust tier changes within a specific time period
    /// </summary>
    /// <param name="fromDate">Start date for the period</param>
    /// <param name="toDate">End date for the period</param>
    /// <param name="changeType">Optional filter by change type</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Trust tier changes within the specified period</returns>
    Task<IEnumerable<TrustTierHistoryEntry>> GetHistoryByPeriodAsync(
        DateTimeOffset fromDate,
        DateTimeOffset toDate,
        TierChangeType? changeType = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets trust tier changes for a specific tier transition
    /// </summary>
    /// <param name="fromTier">The source tier</param>
    /// <param name="toTier">The destination tier</param>
    /// <param name="limit">Maximum number of entries to return</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Trust tier changes matching the tier transition</returns>
    Task<IEnumerable<TrustTierHistoryEntry>> GetHistoryByTierTransitionAsync(
        TrustTier fromTier,
        TrustTier toTier,
        int limit = 100,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the count of tier changes by type within a period
    /// </summary>
    /// <param name="fromDate">Start date for the period</param>
    /// <param name="toDate">End date for the period</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Dictionary of change types to their counts</returns>
    Task<Dictionary<TierChangeType, int>> GetChangeCountsByTypeAsync(
        DateTimeOffset fromDate,
        DateTimeOffset toDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the count of tier changes by trust tier within a period
    /// </summary>
    /// <param name="fromDate">Start date for the period</param>
    /// <param name="toDate">End date for the period</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Dictionary of trust tiers to promotion/demotion counts</returns>
    Task<Dictionary<TrustTier, TrustTierChangeCount>> GetChangeCountsByTierAsync(
        DateTimeOffset fromDate,
        DateTimeOffset toDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets packages that have had tier changes initiated by a specific user
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="limit">Maximum number of entries to return</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Trust tier changes initiated by the user</returns>
    Task<IEnumerable<TrustTierHistoryEntry>> GetHistoryByUserAsync(
        Guid userId,
        int limit = 100,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets packages that have experienced frequent tier changes (potential instability)
    /// </summary>
    /// <param name="periodDays">Number of days to look back</param>
    /// <param name="minChanges">Minimum number of changes to be considered frequent</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Package IDs with frequent tier changes</returns>
    Task<IEnumerable<Guid>> GetPackagesWithFrequentChangesAsync(
        int periodDays = 30,
        int minChanges = 3,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the average time between tier promotions for packages
    /// </summary>
    /// <param name="fromTier">Source tier for promotions</param>
    /// <param name="toTier">Destination tier for promotions</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Average time between promotions or null if insufficient data</returns>
    Task<TimeSpan?> GetAveragePromotionTimeAsync(
        TrustTier fromTier,
        TrustTier toTier,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Records a new trust tier change
    /// </summary>
    /// <param name="packageId">The package identifier</param>
    /// <param name="fromTier">The previous tier</param>
    /// <param name="toTier">The new tier</param>
    /// <param name="reason">Reason for the change</param>
    /// <param name="changeType">Type of change</param>
    /// <param name="changedByUserId">User who initiated the change</param>
    /// <param name="isAutomatic">Whether this was an automatic change</param>
    /// <param name="trustScoreAtChange">Trust score at time of change</param>
    /// <param name="metadata">Additional metadata</param>
    /// <param name="assessmentId">Assessment that triggered the change</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created history entry</returns>
    Task<TrustTierHistoryEntry> RecordTierChangeAsync(
        Guid packageId,
        TrustTier fromTier,
        TrustTier toTier,
        string reason,
        TierChangeType changeType,
        Guid? changedByUserId = null,
        bool isAutomatic = true,
        int? trustScoreAtChange = null,
        Dictionary<string, object>? metadata = null,
        Guid? assessmentId = null,
        CancellationToken cancellationToken = default);
}