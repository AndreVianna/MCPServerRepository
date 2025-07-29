using MCPHub.Domain.Contracts.Requests;
using MCPHub.Domain.Contracts.Responses;
using MCPHub.Domain.Contracts.Services;
using MCPHub.Domain.ValueObjects;

namespace MCPHub.Domain.Services;

/// <summary>
/// Implementation of package publishing service
/// </summary>
public class PackagePublishingService : IPackagePublishingService {
    /// <inheritdoc />
    public Task<PublishResult> ValidateManifestAsync(
        string manifestContent,
        CancellationToken cancellationToken = default) => throw new NotImplementedException("Manifest validation logic will be implemented when first consumer requires it");

    /// <inheritdoc />
    public Task<PublishResult> PublishPackageAsync(
        PublishRequest request,
        Guid userId,
        CancellationToken cancellationToken = default) => throw new NotImplementedException("Package publishing logic will be implemented when first consumer requires it");

    /// <inheritdoc />
    public Task<PublishResult> PublishPackageVersionAsync(
        string packageName,
        PublishVersionRequest request,
        Guid userId,
        CancellationToken cancellationToken = default) => throw new NotImplementedException("Package version publishing logic will be implemented when first consumer requires it");

    /// <inheritdoc />
    public Task<ValidationSummary> PrePublishValidationAsync(
        PublishRequest request,
        Guid userId,
        CancellationToken cancellationToken = default) => throw new NotImplementedException("Pre-publish validation logic will be implemented when first consumer requires it");

    /// <inheritdoc />
    public Task<IEnumerable<PackageVersionInfo>> GetPackageVersionsAsync(
        string packageName,
        bool includePrerelease = false,
        CancellationToken cancellationToken = default) => throw new NotImplementedException("Package version retrieval logic will be implemented when first consumer requires it");

    /// <inheritdoc />
    public Task<bool> IsPackageNameAvailableAsync(
        string packageName,
        Guid userId,
        CancellationToken cancellationToken = default) => throw new NotImplementedException("Package name availability logic will be implemented when first consumer requires it");

    /// <inheritdoc />
    public Task<string> GenerateDownloadUrlAsync(
        string packageName,
        string version,
        int expirationMinutes = 60,
        CancellationToken cancellationToken = default) => throw new NotImplementedException("Download URL generation logic will be implemented when first consumer requires it");

    /// <inheritdoc />
    public Task<bool> UnpublishPackageVersionAsync(
        string packageName,
        string version,
        Guid userId,
        string reason,
        CancellationToken cancellationToken = default) => throw new NotImplementedException("Package unpublishing logic will be implemented when first consumer requires it");
}