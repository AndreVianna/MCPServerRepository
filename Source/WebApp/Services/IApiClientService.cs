using MCPHub.Domain.Contracts.Requests;
using MCPHub.Domain.Contracts.Responses;
using MCPHub.Domain.Contracts.Services;

namespace MCPHub.WebApp.Services;

/// <summary>
/// Interface for API client operations
/// </summary>
public interface IApiClientService {
    /// <summary>
    /// Searches for packages
    /// </summary>
    /// <param name="request">Search request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Search results</returns>
    Task<SearchResult<PackageSearchResultItem>> SearchPackagesAsync(SearchRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets package details by ID
    /// </summary>
    /// <param name="packageId">Package identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Package details</returns>
    Task<PackageDetailsResult> GetPackageAsync(Guid packageId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets server details by ID
    /// </summary>
    /// <param name="serverId">Server identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Server details</returns>
    Task<ServerDetailsResult> GetServerAsync(Guid serverId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers a new server
    /// </summary>
    /// <param name="request">Server registration request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Registration response</returns>
    Task<RegisterServerResponse> RegisterServerAsync(RegisterServerRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets package download statistics
    /// </summary>
    /// <param name="packageId">Package identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Download statistics</returns>
    Task<PackageDownloadStats> GetPackageStatsAsync(Guid packageId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs advanced search with filtering and pagination
    /// </summary>
    /// <param name="request">Advanced search request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Search results with pagination info</returns>
    Task<SearchResult<PackageSearchResultItem>> SearchPackagesAdvancedAsync(AdvancedSearchRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all packages with optional pagination
    /// </summary>
    /// <param name="page">Page number (1-based)</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>All packages</returns>
    Task<SearchResult<PackageSearchResultItem>> GetAllPackagesAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets package by name and publisher
    /// </summary>
    /// <param name="publisherName">Publisher name</param>
    /// <param name="packageName">Package name</param>
    /// <param name="version">Optional version</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Package details</returns>
    Task<PackageDetailsResult> GetPackageByNameAsync(string publisherName, string packageName, string? version = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets packages by category
    /// </summary>
    /// <param name="category">Category name</param>
    /// <param name="page">Page number (1-based)</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Packages in category</returns>
    Task<SearchResult<PackageSearchResultItem>> GetPackagesByCategoryAsync(string category, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets trending packages
    /// </summary>
    /// <param name="timeframe">Timeframe for trending calculation</param>
    /// <param name="limit">Maximum number of packages to return</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Trending packages</returns>
    Task<List<PackageSearchResultItem>> GetTrendingPackagesAsync(string timeframe = "week", int limit = 10, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets featured collections
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Featured collections</returns>
    Task<List<PackageCollection>> GetFeaturedCollectionsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets platform statistics
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Platform statistics</returns>
    Task<PlatformStatistics> GetPlatformStatisticsAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Package search result item
/// </summary>
public class PackageSearchResultItem {
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Publisher { get; set; } = string.Empty;
    public DateTime LastUpdated { get; set; }
    public int DownloadCount { get; set; }
    public string SecurityGrade { get; set; } = string.Empty;
    public string TrustTier { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public string Category { get; set; } = string.Empty;
    public string License { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsTrending { get; set; }
    public double Rating { get; set; }
    public int RatingCount { get; set; }
}

/// <summary>
/// Package details result
/// </summary>
public class PackageDetailsResult {
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public PackageDetails? Package { get; set; }
}

/// <summary>
/// Package details
/// </summary>
public class PackageDetails {
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Publisher { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime LastUpdated { get; set; }
    public int DownloadCount { get; set; }
    public string SecurityGrade { get; set; } = string.Empty;
    public string TrustTier { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public string Repository { get; set; } = string.Empty;
    public string Documentation { get; set; } = string.Empty;
    public string License { get; set; } = string.Empty;
    public string ReadmeContent { get; set; } = string.Empty;
    public List<PackageVersion> Versions { get; set; } = new();
    public List<string> Dependencies { get; set; } = new();
    public List<string> Categories { get; set; } = new();
    public SecurityAnalysisDetails? SecurityAnalysis { get; set; }
    public double Rating { get; set; }
    public int RatingCount { get; set; }
    public List<string> Maintainers { get; set; } = new();
    public string InstallCommand { get; set; } = string.Empty;
}

/// <summary>
/// Server details result
/// </summary>
public class ServerDetailsResult {
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public ServerDetails? Server { get; set; }
}

/// <summary>
/// Server details
/// </summary>
public class ServerDetails {
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Publisher { get; set; } = string.Empty;
    public DateTime RegisteredAt { get; set; }
    public DateTime LastUpdated { get; set; }
    public string Status { get; set; } = string.Empty;
    public string SecurityGrade { get; set; } = string.Empty;
    public string TrustTier { get; set; } = string.Empty;
}

/// <summary>
/// Advanced search request
/// </summary>
public class AdvancedSearchRequest {
    public string Query { get; set; } = string.Empty;
    public List<string>? Categories { get; set; }
    public string? TrustTier { get; set; }
    public string? SecurityGrade { get; set; }
    public string? Timeframe { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; }
    public string SortDirection { get; set; } = "ascending";
}

/// <summary>
/// Package version information
/// </summary>
public class PackageVersion {
    public string Version { get; set; } = string.Empty;
    public DateTime ReleaseDate { get; set; }
    public List<string> Changes { get; set; } = new();
    public string SecurityGrade { get; set; } = string.Empty;
    public bool IsLatest { get; set; }
    public bool IsPrerelease { get; set; }
}

/// <summary>
/// Security analysis details
/// </summary>
public class SecurityAnalysisDetails {
    public string OverallGrade { get; set; } = string.Empty;
    public List<SecurityCategory> Categories { get; set; } = new();
    public List<Vulnerability> Vulnerabilities { get; set; } = new();
    public DateTime LastScanDate { get; set; }
}

/// <summary>
/// Security category assessment
/// </summary>
public class SecurityCategory {
    public string Name { get; set; } = string.Empty;
    public string Grade { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> Features { get; set; } = new();
}

/// <summary>
/// Security vulnerability
/// </summary>
public class Vulnerability {
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime DiscoveredDate { get; set; }
    public DateTime? FixedDate { get; set; }
}

/// <summary>
/// Package collection
/// </summary>
public class PackageCollection {
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public List<PackageSearchResultItem> Packages { get; set; } = new();
    public string CuratorName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Platform statistics
/// </summary>
public class PlatformStatistics {
    public int TotalPackages { get; set; }
    public int TotalDownloads { get; set; }
    public int TotalPublishers { get; set; }
    public int PackagesThisMonth { get; set; }
    public List<CategoryStats> TopCategories { get; set; } = new();
    public List<PublisherStats> TopPublishers { get; set; } = new();
}

/// <summary>
/// Category statistics
/// </summary>
public class CategoryStats {
    public string Name { get; set; } = string.Empty;
    public int PackageCount { get; set; }
    public int DownloadCount { get; set; }
}

/// <summary>
/// Publisher statistics
/// </summary>
public class PublisherStats {
    public string Name { get; set; } = string.Empty;
    public int PackageCount { get; set; }
    public int TotalDownloads { get; set; }
    public string TrustTier { get; set; } = string.Empty;
}