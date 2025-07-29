using MCPHub.Domain.Contracts.Responses;
using MCPHub.Domain.Entities;

namespace MCPHub.Domain.Contracts.Services;

/// <summary>
/// Service interface for trust tier calculation and management
/// </summary>
public interface ITrustTierCalculationService {
    /// <summary>
    /// Calculates the trust tier for a specific package based on current metrics
    /// </summary>
    /// <param name="packageId">The package identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The calculated trust tier</returns>
    Task<TrustTier> CalculatePackageTrustTierAsync(Guid packageId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a comprehensive trust tier assessment for a package
    /// </summary>
    /// <param name="packageId">The package identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Detailed trust tier assessment</returns>
    Task<TrustTierAssessment> GetTrustTierAssessmentAsync(Guid packageId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the trust tier for a package and records the change
    /// </summary>
    /// <param name="packageId">The package identifier</param>
    /// <param name="newTier">The new trust tier</param>
    /// <param name="reason">Reason for the tier change</param>
    /// <param name="changedByUserId">User who initiated the change (null for system changes)</param>
    /// <param name="isManual">Whether this is a manual override</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the operation</returns>
    Task UpdatePackageTrustTierAsync(
        Guid packageId,
        TrustTier newTier,
        string reason,
        Guid? changedByUserId = null,
        bool isManual = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the trust tier change history for a package
    /// </summary>
    /// <param name="packageId">The package identifier</param>
    /// <param name="limit">Maximum number of history records to return</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Chronological history of trust tier changes</returns>
    Task<IEnumerable<TrustTierHistory>> GetTrustTierHistoryAsync(
        Guid packageId,
        int limit = 50,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets platform-wide trust tier statistics
    /// </summary>
    /// <param name="periodDays">Number of days to include in recent statistics</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Platform trust tier statistics</returns>
    Task<TrustTierStatistics> GetTrustTierStatisticsAsync(
        int periodDays = 30,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Recalculates trust tiers for multiple packages in batch
    /// </summary>
    /// <param name="packageIds">Package identifiers to recalculate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Dictionary of package IDs to their new trust tiers</returns>
    Task<Dictionary<Guid, TrustTier>> BatchRecalculateTrustTiersAsync(
        IEnumerable<Guid> packageIds,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Recalculates trust tiers for all packages in the system
    /// </summary>
    /// <param name="maxConcurrency">Maximum number of concurrent calculations</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Summary of recalculation results</returns>
    Task<TrustTierRecalculationSummary> RecalculateAllTrustTiersAsync(
        int maxConcurrency = 10,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates whether a package meets the requirements for a specific trust tier
    /// </summary>
    /// <param name="packageId">The package identifier</param>
    /// <param name="targetTier">The target trust tier to validate against</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Validation result indicating eligibility and missing requirements</returns>
    Task<TrustTierValidationResult> ValidateTrustTierEligibilityAsync(
        Guid packageId,
        TrustTier targetTier,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets packages that are eligible for trust tier promotion
    /// </summary>
    /// <param name="fromTier">Current tier to promote from</param>
    /// <param name="limit">Maximum number of packages to return</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of packages eligible for promotion</returns>
    Task<IEnumerable<Guid>> GetEligibleForPromotionAsync(
        TrustTier? fromTier = null,
        int limit = 100,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets packages that are at risk for trust tier demotion
    /// </summary>
    /// <param name="fromTier">Current tier to check for demotion risk</param>
    /// <param name="limit">Maximum number of packages to return</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of packages at risk for demotion</returns>
    Task<IEnumerable<Guid>> GetAtRiskForDemotionAsync(
        TrustTier? fromTier = null,
        int limit = 100,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Triggers emergency demotion for packages with critical security issues
    /// </summary>
    /// <param name="packageIds">Package identifiers to demote</param>
    /// <param name="reason">Reason for emergency demotion</param>
    /// <param name="demoteToTier">Target tier for demotion</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Results of emergency demotion operations</returns>
    Task<Dictionary<Guid, bool>> EmergencyDemotePackagesAsync(
        IEnumerable<Guid> packageIds,
        string reason,
        TrustTier demoteToTier = TrustTier.Unverified,
        CancellationToken cancellationToken = default);
}