using MCPHub.CommandLineApp.Models;

namespace MCPHub.CommandLineApp.Services;

/// <summary>
/// Interface for MCP Hub API client operations
/// </summary>
public interface IMcpHubApiClient
{
    /// <summary>
    /// Searches for packages using the advanced search API
    /// </summary>
    /// <param name="request">Search request parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Search results with packages and metadata</returns>
    Task<SearchResultResponse> SearchPackagesAsync(SearchRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets detailed information about a specific package
    /// </summary>
    /// <param name="packageName">Name of the package</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Package information</returns>
    Task<PackageInfoResponse> GetPackageInfoAsync(string packageName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all available packages with pagination
    /// </summary>
    /// <param name="page">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of packages</returns>
    Task<PackageListResponse> GetPackagesAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets version information for a specific package
    /// </summary>
    /// <param name="packageName">Name of the package</param>
    /// <param name="includePrerelease">Whether to include prerelease versions</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Package versions</returns>
    Task<PackageVersionsResponse> GetPackageVersionsAsync(string packageName, bool includePrerelease = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets security summary for a specific package
    /// </summary>
    /// <param name="packageName">Name of the package</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Security summary</returns>
    Task<SecuritySummaryResponse> GetPackageSecuritySummaryAsync(string packageName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets trust tier assessment for a specific package
    /// </summary>
    /// <param name="packageName">Name of the package</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Trust tier assessment</returns>
    Task<TrustTierResponse> GetPackageTrustTierAsync(string packageName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tests connectivity to the API
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if API is reachable</returns>
    Task<bool> TestConnectivityAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates a package manifest
    /// </summary>
    /// <param name="request">Manifest validation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Validation result</returns>
    Task<ValidateManifestResponse> ValidateManifestAsync(ValidateManifestRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Publishes a new package or package version
    /// </summary>
    /// <param name="request">Package publishing request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Publishing result</returns>
    Task<PublishPackageResponse> PublishPackageAsync(PublishPackageRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads a package to get the download URL
    /// </summary>
    /// <param name="packageName">Name of the package</param>
    /// <param name="version">Specific version to download (optional, defaults to latest)</param>
    /// <param name="downloadRequest">Download tracking information</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Download result with pre-signed URL</returns>
    Task<DownloadPackageResponse> DownloadPackageAsync(string packageName, string? version, DownloadPackageRequest downloadRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Records a package installation
    /// </summary>
    /// <param name="packageName">Name of the package</param>
    /// <param name="version">Version that was installed</param>
    /// <param name="installRequest">Installation tracking information</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Installation result</returns>
    Task<InstallPackageResponse> InstallPackageAsync(string packageName, string version, InstallPackageRequest installRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets dependency information for a package
    /// </summary>
    /// <param name="packageName">Name of the package</param>
    /// <param name="version">Package version</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Package dependencies</returns>
    Task<PackageDependenciesResponse> GetPackageDependenciesAsync(string packageName, string version, CancellationToken cancellationToken = default);
}