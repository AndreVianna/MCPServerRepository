using System.Text.Json;

using MCPHub.Domain.Contracts.Services;

namespace MCPHub.WebApp.Services;

/// <summary>
/// API client service implementation
/// </summary>
public class ApiClientService : IApiClientService {
    private readonly HttpClient _httpClient;
    private readonly CustomAuthenticationStateProvider _authStateProvider;
    private readonly ILogger<ApiClientService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public ApiClientService(
        HttpClient httpClient,
        AuthenticationStateProvider authStateProvider,
        ILogger<ApiClientService> logger) {
        _httpClient = httpClient;
        _authStateProvider = (CustomAuthenticationStateProvider)authStateProvider;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    /// <inheritdoc />
    public async Task<SearchResult<PackageSearchResultItem>> SearchPackagesAsync(SearchRequest request, CancellationToken cancellationToken = default) {
        try {
            await SetAuthorizationHeaderAsync();

            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(request.Query))
                queryParams.Add($"query={Uri.EscapeDataString(request.Query)}");
            if (request.PageSize > 0)
                queryParams.Add($"pageSize={request.PageSize}");
            if (request.Page > 0)
                queryParams.Add($"pageIndex={request.Page - 1}"); // Convert to 0-based

            var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
            var response = await _httpClient.GetAsync($"/api/packages/search{queryString}", cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            if (response.IsSuccessStatusCode) {
                var result = JsonSerializer.Deserialize<SearchResult<PackageSearchResultItem>>(responseContent, _jsonOptions);
                return result ?? new SearchResult<PackageSearchResultItem> { Items = [], TotalCount = 0 };
            }

            _logger.LogWarning("Package search failed with status {StatusCode}: {Content}", response.StatusCode, responseContent);
            return new SearchResult<PackageSearchResultItem> { Items = [], TotalCount = 0 };
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error searching packages");
            return new SearchResult<PackageSearchResultItem> { Items = [], TotalCount = 0 };
        }
    }

    /// <inheritdoc />
    public async Task<PackageDetailsResult> GetPackageAsync(Guid packageId, CancellationToken cancellationToken = default) {
        try {
            await SetAuthorizationHeaderAsync();

            var response = await _httpClient.GetAsync($"/api/packages/{packageId}", cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            if (response.IsSuccessStatusCode) {
                var result = JsonSerializer.Deserialize<PackageDetailsResult>(responseContent, _jsonOptions);
                return result ?? new PackageDetailsResult { IsSuccess = false, Message = "Invalid response format" };
            }

            var errorResult = JsonSerializer.Deserialize<PackageDetailsResult>(responseContent, _jsonOptions);
            return errorResult ?? new PackageDetailsResult { IsSuccess = false, Message = "Failed to get package details" };
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error getting package details for {PackageId}", packageId);
            return new PackageDetailsResult { IsSuccess = false, Message = "An error occurred getting package details" };
        }
    }

    /// <inheritdoc />
    public async Task<ServerDetailsResult> GetServerAsync(Guid serverId, CancellationToken cancellationToken = default) {
        try {
            await SetAuthorizationHeaderAsync();

            var response = await _httpClient.GetAsync($"/api/servers/{serverId}", cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            if (response.IsSuccessStatusCode) {
                var result = JsonSerializer.Deserialize<ServerDetailsResult>(responseContent, _jsonOptions);
                return result ?? new ServerDetailsResult { IsSuccess = false, Message = "Invalid response format" };
            }

            var errorResult = JsonSerializer.Deserialize<ServerDetailsResult>(responseContent, _jsonOptions);
            return errorResult ?? new ServerDetailsResult { IsSuccess = false, Message = "Failed to get server details" };
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error getting server details for {ServerId}", serverId);
            return new ServerDetailsResult { IsSuccess = false, Message = "An error occurred getting server details" };
        }
    }

    /// <inheritdoc />
    public async Task<RegisterServerResponse> RegisterServerAsync(RegisterServerRequest request, CancellationToken cancellationToken = default) {
        try {
            await SetAuthorizationHeaderAsync();

            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/servers/register", content, cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            var result = JsonSerializer.Deserialize<RegisterServerResponse>(responseContent, _jsonOptions);
            return result ?? new RegisterServerResponse { Status = "Failed", Message = "Server registration failed" };
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error registering server");
            return new RegisterServerResponse { Status = "Failed", Message = "An error occurred during server registration" };
        }
    }

    /// <inheritdoc />
    public async Task<PackageDownloadStats> GetPackageStatsAsync(Guid packageId, CancellationToken cancellationToken = default) {
        try {
            await SetAuthorizationHeaderAsync();

            var response = await _httpClient.GetAsync($"/api/packages/{packageId}/stats", cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            if (response.IsSuccessStatusCode) {
                var result = JsonSerializer.Deserialize<PackageDownloadStats>(responseContent, _jsonOptions);
                return result ?? PackageDownloadStats.Empty(packageId);
            }

            _logger.LogWarning("Package stats request failed with status {StatusCode}: {Content}", response.StatusCode, responseContent);
            return PackageDownloadStats.Empty(packageId);
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error getting package stats for {PackageId}", packageId);
            return PackageDownloadStats.Empty(packageId);
        }
    }

    /// <inheritdoc />
    public async Task<SearchResult<PackageSearchResultItem>> SearchPackagesAdvancedAsync(AdvancedSearchRequest request, CancellationToken cancellationToken = default) {
        try {
            await SetAuthorizationHeaderAsync();

            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(request.Query))
                queryParams.Add($"q={Uri.EscapeDataString(request.Query)}");
            if (request.Categories?.Any() == true)
                queryParams.Add($"categories={string.Join(",", request.Categories.Select(Uri.EscapeDataString))}");
            if (!string.IsNullOrEmpty(request.TrustTier))
                queryParams.Add($"trustTier={Uri.EscapeDataString(request.TrustTier)}");
            if (request.Page > 0)
                queryParams.Add($"page={request.Page}");
            if (request.PageSize > 0)
                queryParams.Add($"pageSize={request.PageSize}");
            if (!string.IsNullOrEmpty(request.SortBy))
                queryParams.Add($"sortBy={Uri.EscapeDataString(request.SortBy)}");
            if (!string.IsNullOrEmpty(request.SortDirection))
                queryParams.Add($"sortDirection={Uri.EscapeDataString(request.SortDirection)}");

            var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
            var response = await _httpClient.GetAsync($"/api/packages/search/advanced{queryString}", cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            if (response.IsSuccessStatusCode) {
                var result = JsonSerializer.Deserialize<SearchResult<PackageSearchResultItem>>(responseContent, _jsonOptions);
                return result ?? new SearchResult<PackageSearchResultItem> { Items = [], TotalCount = 0 };
            }

            _logger.LogWarning("Advanced package search failed with status {StatusCode}: {Content}", response.StatusCode, responseContent);
            return new SearchResult<PackageSearchResultItem> { Items = [], TotalCount = 0 };
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error performing advanced package search");
            return new SearchResult<PackageSearchResultItem> { Items = [], TotalCount = 0 };
        }
    }

    /// <inheritdoc />
    public async Task<SearchResult<PackageSearchResultItem>> GetAllPackagesAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default) {
        try {
            await SetAuthorizationHeaderAsync();

            var queryParams = new List<string>
            {
                $"pageIndex={page - 1}", // Convert to 0-based
                $"pageSize={pageSize}"
            };

            var queryString = "?" + string.Join("&", queryParams);
            var response = await _httpClient.GetAsync($"/api/packages{queryString}", cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            if (response.IsSuccessStatusCode) {
                var packages = JsonSerializer.Deserialize<List<PackageSearchResultItem>>(responseContent, _jsonOptions);
                // For now, create a simple SearchResult wrapper since the API doesn't return paginated results
                return new SearchResult<PackageSearchResultItem> {
                    Items = packages ?? [],
                    TotalCount = packages?.Count ?? 0,
                    Page = page,
                    PageSize = pageSize
                };
            }

            _logger.LogWarning("Get all packages failed with status {StatusCode}: {Content}", response.StatusCode, responseContent);
            return new SearchResult<PackageSearchResultItem> { Items = [], TotalCount = 0 };
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error getting all packages");
            return new SearchResult<PackageSearchResultItem> { Items = [], TotalCount = 0 };
        }
    }

    /// <inheritdoc />
    public async Task<PackageDetailsResult> GetPackageByNameAsync(string publisherName, string packageName, string? version = null, CancellationToken cancellationToken = default) {
        try {
            await SetAuthorizationHeaderAsync();

            // For now, use the existing by-name endpoint
            var response = await _httpClient.GetAsync($"/api/packages/by-name/{Uri.EscapeDataString(packageName)}", cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            if (response.IsSuccessStatusCode) {
                var package = JsonSerializer.Deserialize<PackageDetails>(responseContent, _jsonOptions);
                return new PackageDetailsResult { IsSuccess = true, Package = package };
            }

            var errorResult = JsonSerializer.Deserialize<PackageDetailsResult>(responseContent, _jsonOptions);
            return errorResult ?? new PackageDetailsResult { IsSuccess = false, Message = "Failed to get package details" };
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error getting package by name {PublisherName}/{PackageName}", publisherName, packageName);
            return new PackageDetailsResult { IsSuccess = false, Message = "An error occurred getting package details" };
        }
    }

    /// <inheritdoc />
    public async Task<SearchResult<PackageSearchResultItem>> GetPackagesByCategoryAsync(string category, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default) {
        try {
            await SetAuthorizationHeaderAsync();

            // Use advanced search with category filter
            var request = new AdvancedSearchRequest {
                Query = "*", // Search all packages
                Categories = [category],
                Page = page,
                PageSize = pageSize
            };

            return await SearchPackagesAdvancedAsync(request, cancellationToken);
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error getting packages by category {Category}", category);
            return new SearchResult<PackageSearchResultItem> { Items = [], TotalCount = 0 };
        }
    }

    /// <inheritdoc />
    public async Task<List<PackageSearchResultItem>> GetTrendingPackagesAsync(string timeframe = "week", int limit = 10, CancellationToken cancellationToken = default) {
        try {
            await SetAuthorizationHeaderAsync();

            // Use advanced search with trending sort
            var request = new AdvancedSearchRequest {
                Query = "*", // Search all packages
                SortBy = "downloads",
                SortDirection = "descending",
                Page = 1,
                PageSize = limit,
                Timeframe = timeframe
            };

            var result = await SearchPackagesAdvancedAsync(request, cancellationToken);
            return result.Items.ToList();
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error getting trending packages");
            return [];
        }
    }

    /// <inheritdoc />
    public async Task<List<PackageCollection>> GetFeaturedCollectionsAsync(CancellationToken cancellationToken = default) {
        try {
            await SetAuthorizationHeaderAsync();

            // For now, return mock data since the API endpoint doesn't exist yet
            // TODO: Implement actual API endpoint for featured collections
            _logger.LogInformation("Getting featured collections - using mock data");
            return [];
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error getting featured collections");
            return [];
        }
    }

    /// <inheritdoc />
    public async Task<PlatformStatistics> GetPlatformStatisticsAsync(CancellationToken cancellationToken = default) {
        try {
            await SetAuthorizationHeaderAsync();

            // For now, return mock data since the API endpoint doesn't exist yet
            // TODO: Implement actual API endpoint for platform statistics
            _logger.LogInformation("Getting platform statistics - using mock data");
            return new PlatformStatistics();
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error getting platform statistics");
            return new PlatformStatistics();
        }
    }

    private async Task SetAuthorizationHeaderAsync() {
        try {
            var token = await _authStateProvider.GetTokenAsync();
            if (!string.IsNullOrEmpty(token)) {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }
        catch (Exception ex) {
            _logger.LogWarning(ex, "Failed to set authorization header");
        }
    }
}