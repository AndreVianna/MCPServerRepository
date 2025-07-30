using MCPHub.Domain.Contracts.Services;
using MCPHub.Domain.Entities;
using MCPHub.Domain.ValueObjects;

namespace MCPHub.CommandLineApp.Services;

/// <summary>
/// CLI security service implementation
/// </summary>
public class SecurityService(
    ILogger<SecurityService> logger,
    ISecurityScanService securityScanService,
    ISecurityGradeCalculator securityGradeCalculator,
    IMcpHubApiClient apiClient,
    IOfflineSecurityService offlineSecurityService) : ISecurityService {
    private readonly ILogger<SecurityService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly ISecurityScanService _securityScanService = securityScanService ?? throw new ArgumentNullException(nameof(securityScanService));
    private readonly ISecurityGradeCalculator _securityGradeCalculator = securityGradeCalculator ?? throw new ArgumentNullException(nameof(securityGradeCalculator));
    private readonly IMcpHubApiClient _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
    private readonly IOfflineSecurityService _offlineSecurityService = offlineSecurityService ?? throw new ArgumentNullException(nameof(offlineSecurityService));

    /// <inheritdoc />
    public Task<SecurityScanResult> ScanPackageAsync(
        string packageName,
        string version,
        ScanType scanType,
        CancellationToken cancellationToken = default) => throw new NotImplementedException("Package scanning will be implemented when first consumer requires it");

    /// <inheritdoc />
    public Task<SecurityScanResult?> GetLatestScanResultAsync(
        string packageName,
        string version,
        CancellationToken cancellationToken = default) => throw new NotImplementedException("Latest scan result retrieval will be implemented when first consumer requires it");

    /// <inheritdoc />
    public Task<SecurityScanSummary> GetSecuritySummaryAsync(
        string packageName,
        CancellationToken cancellationToken = default) => throw new NotImplementedException("Security summary will be implemented when first consumer requires it");

    /// <inheritdoc />
    public Task<SecurityPolicyValidationResult> ValidateSecurityPolicyAsync(
        string packageName,
        string version,
        SecurityPolicy? policy = null,
        CancellationToken cancellationToken = default) => throw new NotImplementedException("Security policy validation will be implemented when first consumer requires it");

    /// <inheritdoc />
    public Task<string> CalculateSecurityGradeAsync(
        string packageName,
        string version,
        CancellationToken cancellationToken = default) => throw new NotImplementedException("Security grade calculation will be implemented when first consumer requires it");

    /// <inheritdoc />
    public async Task<PackageVerificationResult> VerifyPackageIntegrityAsync(
        string packagePath,
        string? expectedChecksum = null,
        CancellationToken cancellationToken = default) {
        _logger.LogInformation("Verifying package integrity for {PackagePath}", packagePath);

        try {
            // Perform checksum verification using offline service
            var checksumResult = await _offlineSecurityService.VerifyPackageChecksumAsync(
                packagePath, expectedChecksum, "SHA256", cancellationToken);

            // Attempt signature verification using offline service
            var signatureResult = await _offlineSecurityService.VerifyPackageSignatureAsync(
                packagePath, cancellationToken);

            return new PackageVerificationResult {
                IsValid = checksumResult.IsValid && (signatureResult.IsValid || !signatureResult.IsSigned),
                SignatureValid = signatureResult.IsValid,
                ChecksumValid = checksumResult.IsValid,
                CalculatedChecksum = checksumResult.CalculatedChecksum,
                ExpectedChecksum = expectedChecksum,
                Issues = checksumResult.Issues.Concat(signatureResult.ValidationErrors),
            };
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Package integrity verification failed for {PackagePath}", packagePath);
            return new PackageVerificationResult {
                IsValid = false,
                Issues = new[] { $"Verification failed: {ex.Message}" },
            };
        }
    }

    /// <inheritdoc />
    public Task<IEnumerable<SecurityAdvisory>> GetSecurityAdvisoriesAsync(
        string packageName,
        string? version = null,
        SecurityScanSeverity? severity = null,
        CancellationToken cancellationToken = default) => throw new NotImplementedException("Security advisories will be implemented when first consumer requires it");

    /// <inheritdoc />
    public async Task<SecurityDatabaseUpdateResult> UpdateSecurityDatabaseAsync(
        bool forceUpdate = false,
        CancellationToken cancellationToken = default) {
        _logger.LogInformation("Updating security database (force: {ForceUpdate})", forceUpdate);

        try {
            // Use offline security service to update vulnerability database
            var updateResult = await _offlineSecurityService.UpdateVulnerabilityDatabaseAsync(
                sources: null, // Use default sources
                cancellationToken: cancellationToken);

            return new SecurityDatabaseUpdateResult {
                Success = updateResult.Success,
                UpdatedRecords = updateResult.UpdatedRecords,
                LastUpdate = updateResult.UpdatedAt,
                Errors = updateResult.Errors,
            };
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Failed to update security database");
            return new SecurityDatabaseUpdateResult {
                Success = false,
                LastUpdate = DateTimeOffset.UtcNow,
                Errors = new[] { $"Update failed: {ex.Message}" },
            };
        }
    }

    /// <inheritdoc />
    public async Task<SecurityDatabaseStatus> GetSecurityDatabaseStatusAsync() {
        try {
            var dbStatus = await _offlineSecurityService.GetVulnerabilityDatabaseStatusAsync();

            return new SecurityDatabaseStatus {
                IsAvailable = dbStatus.IsAvailable,
                LastUpdate = dbStatus.LastUpdate,
                RecordCount = dbStatus.TotalVulnerabilities,
                IsOutdated = dbStatus.IsOutdated,
                Age = dbStatus.Age,
            };
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Failed to get security database status");
            return new SecurityDatabaseStatus {
                IsAvailable = false,
                RecordCount = 0,
                IsOutdated = true,
            };
        }
    }
}