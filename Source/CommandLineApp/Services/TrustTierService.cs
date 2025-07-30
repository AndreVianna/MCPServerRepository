using MCPHub.Domain.Contracts.Responses;
using MCPHub.Domain.Contracts.Services;
using MCPHub.Domain.Entities;

using Microsoft.Extensions.Logging;

namespace MCPHub.CommandLineApp.Services;

/// <summary>
/// CLI trust tier service implementation
/// </summary>
public class TrustTierService(
    ILogger<TrustTierService> logger,
    ITrustTierCalculationService trustTierCalculationService,
    IMcpHubApiClient apiClient) : ITrustTierService {
    private readonly ILogger<TrustTierService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly ITrustTierCalculationService _trustTierCalculationService = trustTierCalculationService ?? throw new ArgumentNullException(nameof(trustTierCalculationService));
    private readonly IMcpHubApiClient _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));

    /// <inheritdoc />
    public Task<TrustTierAssessment> GetTrustTierAssessmentAsync(
        string packageName,
        CancellationToken cancellationToken = default) => throw new NotImplementedException("Trust tier assessment will be implemented when first consumer requires it");

    /// <inheritdoc />
    public Task<IEnumerable<TrustTierHistory>> GetTrustTierHistoryAsync(
        string packageName,
        int limit = 10,
        CancellationToken cancellationToken = default) => throw new NotImplementedException("Trust tier history will be implemented when first consumer requires it");

    /// <inheritdoc />
    public Task<TrustTierValidationResult> ValidateTrustTierEligibilityAsync(
        string packageName,
        TrustTier targetTier,
        CancellationToken cancellationToken = default) => throw new NotImplementedException("Trust tier eligibility validation will be implemented when first consumer requires it");

    /// <inheritdoc />
    public Task<TrustTierStatistics> GetTrustTierStatisticsAsync(
        int periodDays = 30,
        CancellationToken cancellationToken = default) => throw new NotImplementedException("Trust tier statistics will be implemented when first consumer requires it");

    /// <inheritdoc />
    public Task<IEnumerable<TrustTierComplianceResult>> CheckTrustTierComplianceAsync(
        IEnumerable<PackageSpec> packages,
        TrustTier minimumTier,
        CancellationToken cancellationToken = default) => throw new NotImplementedException("Trust tier compliance checking will be implemented when first consumer requires it");

    /// <inheritdoc />
    public Task<IEnumerable<TrustTierRecommendation>> GetTrustTierRecommendationsAsync(
        bool includeGlobal = true,
        CancellationToken cancellationToken = default) => throw new NotImplementedException("Trust tier recommendations will be implemented when first consumer requires it");

    /// <inheritdoc />
    public Task<TrustScoreBreakdown> GetTrustScoreBreakdownAsync(
        string packageName,
        CancellationToken cancellationToken = default) => throw new NotImplementedException("Trust score breakdown will be implemented when first consumer requires it");
}