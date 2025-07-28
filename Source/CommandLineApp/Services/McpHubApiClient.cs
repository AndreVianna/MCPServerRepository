using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using MCPHub.CommandLineApp.Configuration;
using MCPHub.CommandLineApp.Models;

namespace MCPHub.CommandLineApp.Services;

/// <summary>
/// HTTP client for communicating with the MCP Hub API
/// </summary>
public class McpHubApiClient : IMcpHubApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<McpHubApiClient> _logger;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly McpmConfiguration _configuration;

    public McpHubApiClient(
        HttpClient httpClient, 
        ILogger<McpHubApiClient> logger, 
        McpmConfiguration configuration)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        ConfigureHttpClient();
    }

    /// <inheritdoc />
    public async Task<SearchResultResponse> SearchPackagesAsync(SearchRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Searching packages: Query={Query}, Page={Page}, PageSize={PageSize}", 
                request.Query, request.Page, request.PageSize);

            var queryParams = new List<string>
            {
                $"q={Uri.EscapeDataString(request.Query)}"
            };

            if (request.Categories?.Any() == true)
            {
                queryParams.Add($"categories={Uri.EscapeDataString(string.Join(",", request.Categories))}");
            }

            if (!string.IsNullOrEmpty(request.TrustTier))
            {
                queryParams.Add($"trustTier={Uri.EscapeDataString(request.TrustTier)}");
            }

            if (request.Page > 1)
            {
                queryParams.Add($"page={request.Page}");
            }

            if (request.PageSize != 20)
            {
                queryParams.Add($"pageSize={request.PageSize}");
            }

            if (!string.IsNullOrEmpty(request.SortBy))
            {
                queryParams.Add($"sortBy={Uri.EscapeDataString(request.SortBy)}");
            }

            if (request.SortDirection != "Ascending")
            {
                queryParams.Add($"sortDirection={request.SortDirection}");
            }

            var queryString = string.Join("&", queryParams);
            var endpoint = $"/api/v{_configuration.Registry.ApiVersion}/packages/search/advanced?{queryString}";

            var response = await _httpClient.GetAsync(endpoint, cancellationToken);
            await EnsureSuccessStatusCodeAsync(response);

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<SearchResultResponse>>(responseContent, _jsonOptions);

            if (apiResponse?.Data == null)
            {
                throw new InvalidOperationException("Invalid response format from search API");
            }

            _logger.LogInformation("Search completed: Found {TotalCount} packages in {SearchTime}ms", 
                apiResponse.Data.TotalCount, apiResponse.Data.SearchTimeMs);

            return apiResponse.Data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to search packages");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<PackageInfoResponse> GetPackageInfoAsync(string packageName, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting package info: {PackageName}", packageName);

            var endpoint = $"/api/v{_configuration.Registry.ApiVersion}/packages/by-name/{Uri.EscapeDataString(packageName)}";
            var response = await _httpClient.GetAsync(endpoint, cancellationToken);
            
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new PackageNotFoundException($"Package '{packageName}' not found");
            }

            await EnsureSuccessStatusCodeAsync(response);

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<PackageInfoResponse>>(responseContent, _jsonOptions);

            if (apiResponse?.Data == null)
            {
                throw new InvalidOperationException("Invalid response format from package info API");
            }

            _logger.LogInformation("Retrieved package info: {PackageName}", packageName);
            return apiResponse.Data;
        }
        catch (Exception ex) when (ex is not PackageNotFoundException)
        {
            _logger.LogError(ex, "Failed to get package info for: {PackageName}", packageName);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<PackageListResponse> GetPackagesAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting packages: Page={Page}, PageSize={PageSize}", page, pageSize);

            var queryParams = new List<string>();
            
            if (page > 1)
            {
                queryParams.Add($"pageIndex={page - 1}"); // API uses 0-based indexing
            }

            if (pageSize != 20)
            {
                queryParams.Add($"pageSize={pageSize}");
            }

            var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
            var endpoint = $"/api/v{_configuration.Registry.ApiVersion}/packages{queryString}";

            var response = await _httpClient.GetAsync(endpoint, cancellationToken);
            await EnsureSuccessStatusCodeAsync(response);

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<List<PackageSearchResult>>>(responseContent, _jsonOptions);

            if (apiResponse?.Data == null)
            {
                throw new InvalidOperationException("Invalid response format from packages API");
            }

            // Extract pagination info from headers if available
            var totalCount = GetHeaderValue(response, "X-Total-Count", apiResponse.Data.Count.ToString());
            var totalPages = GetHeaderValue(response, "X-Total-Pages", "1");

            var result = new PackageListResponse
            {
                Packages = apiResponse.Data,
                TotalCount = int.TryParse(totalCount, out var count) ? count : apiResponse.Data.Count,
                Page = page,
                PageSize = pageSize,
                TotalPages = int.TryParse(totalPages, out var pages) ? pages : 1
            };

            _logger.LogInformation("Retrieved {PackageCount} packages (page {Page})", result.Packages.Count, page);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get packages");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<PackageVersionsResponse> GetPackageVersionsAsync(string packageName, bool includePrerelease = false, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting package versions: {PackageName}, IncludePrerelease={IncludePrerelease}", 
                packageName, includePrerelease);

            var queryString = includePrerelease ? "?includePrerelease=true" : "";
            var endpoint = $"/api/v{_configuration.Registry.ApiVersion}/packages/{Uri.EscapeDataString(packageName)}/versions{queryString}";

            var response = await _httpClient.GetAsync(endpoint, cancellationToken);
            
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new PackageNotFoundException($"Package '{packageName}' not found");
            }

            await EnsureSuccessStatusCodeAsync(response);

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<List<PackageVersionInfo>>>(responseContent, _jsonOptions);

            if (apiResponse?.Data == null)
            {
                throw new InvalidOperationException("Invalid response format from package versions API");
            }

            var result = new PackageVersionsResponse
            {
                Versions = apiResponse.Data
            };

            _logger.LogInformation("Retrieved {VersionCount} versions for package: {PackageName}", 
                result.Versions.Count, packageName);

            return result;
        }
        catch (Exception ex) when (ex is not PackageNotFoundException)
        {
            _logger.LogError(ex, "Failed to get package versions for: {PackageName}", packageName);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<SecuritySummaryResponse> GetPackageSecuritySummaryAsync(string packageName, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting package security summary: {PackageName}", packageName);

            var endpoint = $"/api/v{_configuration.Registry.ApiVersion}/packages/{Uri.EscapeDataString(packageName)}/security/summary";
            var response = await _httpClient.GetAsync(endpoint, cancellationToken);
            
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new PackageNotFoundException($"Package '{packageName}' not found");
            }

            await EnsureSuccessStatusCodeAsync(response);

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<SecuritySummaryResponse>>(responseContent, _jsonOptions);

            if (apiResponse?.Data == null)
            {
                throw new InvalidOperationException("Invalid response format from security summary API");
            }

            _logger.LogInformation("Retrieved security summary for package: {PackageName}, Grade: {Grade}", 
                packageName, apiResponse.Data.Grade);

            return apiResponse.Data;
        }
        catch (Exception ex) when (ex is not PackageNotFoundException)
        {
            _logger.LogError(ex, "Failed to get security summary for: {PackageName}", packageName);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<TrustTierResponse> GetPackageTrustTierAsync(string packageName, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting package trust tier: {PackageName}", packageName);

            var endpoint = $"/api/v{_configuration.Registry.ApiVersion}/packages/{Uri.EscapeDataString(packageName)}/trust-tier";
            var response = await _httpClient.GetAsync(endpoint, cancellationToken);
            
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new PackageNotFoundException($"Package '{packageName}' not found");
            }

            await EnsureSuccessStatusCodeAsync(response);

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<TrustTierResponse>>(responseContent, _jsonOptions);

            if (apiResponse?.Data == null)
            {
                throw new InvalidOperationException("Invalid response format from trust tier API");
            }

            _logger.LogInformation("Retrieved trust tier for package: {PackageName}, Tier: {Tier}", 
                packageName, apiResponse.Data.CurrentTier);

            return apiResponse.Data;
        }
        catch (Exception ex) when (ex is not PackageNotFoundException)
        {
            _logger.LogError(ex, "Failed to get trust tier for: {PackageName}", packageName);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<bool> TestConnectivityAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Testing API connectivity");

            // Test with a simple packages endpoint
            var endpoint = $"/api/v{_configuration.Registry.ApiVersion}/packages?pageSize=1";
            var response = await _httpClient.GetAsync(endpoint, cancellationToken);

            var isReachable = response.IsSuccessStatusCode;
            _logger.LogInformation("API connectivity test: {Result}", isReachable ? "Success" : "Failed");

            return isReachable;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "API connectivity test failed");
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<ValidateManifestResponse> ValidateManifestAsync(ValidateManifestRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Validating manifest");

            var endpoint = $"/api/v{_configuration.Registry.ApiVersion}/packages/validate";
            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(endpoint, content, cancellationToken);
            await EnsureSuccessStatusCodeAsync(response);

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<ValidateManifestResponse>>(responseContent, _jsonOptions);

            if (apiResponse?.Data == null)
            {
                throw new InvalidOperationException("Invalid response format from validation API");
            }

            _logger.LogInformation("Manifest validation completed: Valid={IsValid}, Errors={ErrorCount}, Warnings={WarningCount}", 
                apiResponse.Data.IsValid, apiResponse.Data.Errors.Count, apiResponse.Data.Warnings.Count);

            return apiResponse.Data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to validate manifest");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<PublishPackageResponse> PublishPackageAsync(PublishPackageRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Publishing package");

            var endpoint = $"/api/v{_configuration.Registry.ApiVersion}/packages";
            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(endpoint, content, cancellationToken);

            // Handle specific error cases for publishing
            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new PackageAlreadyExistsException("Package with this name and version already exists");
            }

            if (response.StatusCode == HttpStatusCode.UnprocessableEntity)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                try
                {
                    var errorResponse = JsonSerializer.Deserialize<ApiResponse<object>>(errorContent, _jsonOptions);
                    throw new ManifestValidationException(
                        "Manifest validation failed", 
                        errorResponse?.Errors?.ToList() ?? new List<string> { "Unknown validation error" });
                }
                catch (JsonException)
                {
                    throw new ManifestValidationException("Manifest validation failed", new List<string> { "Invalid manifest format" });
                }
            }

            await EnsureSuccessStatusCodeAsync(response);

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<PublishPackageResponse>>(responseContent, _jsonOptions);

            if (apiResponse?.Data == null)
            {
                throw new InvalidOperationException("Invalid response format from publishing API");
            }

            _logger.LogInformation("Package publishing completed: Success={Success}, PackageId={PackageId}", 
                apiResponse.Data.Success, apiResponse.Data.Package?.Id);

            return apiResponse.Data;
        }
        catch (Exception ex) when (ex is not PackageAlreadyExistsException and not ManifestValidationException)
        {
            _logger.LogError(ex, "Failed to publish package");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<DownloadPackageResponse> DownloadPackageAsync(string packageName, string? version, DownloadPackageRequest downloadRequest, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting download URL for package: {PackageName}@{Version}", packageName, version ?? "latest");

            var versionPart = string.IsNullOrEmpty(version) ? "" : $"/{Uri.EscapeDataString(version)}";
            var endpoint = $"/api/v{_configuration.Registry.ApiVersion}/packages/{Uri.EscapeDataString(packageName)}/download{versionPart}";
            
            var json = JsonSerializer.Serialize(downloadRequest, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(endpoint, content, cancellationToken);
            
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new PackageNotFoundException($"Package '{packageName}' {(version != null ? $"version '{version}'" : "")} not found");
            }

            await EnsureSuccessStatusCodeAsync(response);

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<DownloadPackageResponse>>(responseContent, _jsonOptions);

            if (apiResponse?.Data == null)
            {
                throw new InvalidOperationException("Invalid response format from download API");
            }

            _logger.LogInformation("Download URL obtained for package: {PackageName}@{Version}, DownloadId: {DownloadId}", 
                packageName, version ?? "latest", apiResponse.Data.DownloadId);

            return apiResponse.Data;
        }
        catch (Exception ex) when (ex is not PackageNotFoundException)
        {
            _logger.LogError(ex, "Failed to get download URL for package: {PackageName}@{Version}", packageName, version);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<InstallPackageResponse> InstallPackageAsync(string packageName, string version, InstallPackageRequest installRequest, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Recording package installation: {PackageName}@{Version}", packageName, version);

            var endpoint = $"/api/v{_configuration.Registry.ApiVersion}/packages/{Uri.EscapeDataString(packageName)}/versions/{Uri.EscapeDataString(version)}/install";
            
            var json = JsonSerializer.Serialize(installRequest, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(endpoint, content, cancellationToken);
            
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new PackageNotFoundException($"Package '{packageName}' version '{version}' not found");
            }

            await EnsureSuccessStatusCodeAsync(response);

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<InstallPackageResponse>>(responseContent, _jsonOptions);

            if (apiResponse?.Data == null)
            {
                throw new InvalidOperationException("Invalid response format from install API");
            }

            _logger.LogInformation("Package installation recorded: {PackageName}@{Version}, InstallationId: {InstallationId}", 
                packageName, version, apiResponse.Data.InstallationId);

            return apiResponse.Data;
        }
        catch (Exception ex) when (ex is not PackageNotFoundException)
        {
            _logger.LogError(ex, "Failed to record package installation: {PackageName}@{Version}", packageName, version);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<PackageDependenciesResponse> GetPackageDependenciesAsync(string packageName, string version, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting package dependencies: {PackageName}@{Version}", packageName, version);

            var endpoint = $"/api/v{_configuration.Registry.ApiVersion}/packages/{Uri.EscapeDataString(packageName)}/versions/{Uri.EscapeDataString(version)}/dependencies";
            
            var response = await _httpClient.GetAsync(endpoint, cancellationToken);
            
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new PackageNotFoundException($"Package '{packageName}' version '{version}' not found");
            }

            await EnsureSuccessStatusCodeAsync(response);

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<PackageDependenciesResponse>>(responseContent, _jsonOptions);

            if (apiResponse?.Data == null)
            {
                throw new InvalidOperationException("Invalid response format from dependencies API");
            }

            _logger.LogInformation("Retrieved dependencies for package: {PackageName}@{Version}, Dependencies: {DependencyCount}, DevDependencies: {DevDependencyCount}", 
                packageName, version, apiResponse.Data.Dependencies.Count, apiResponse.Data.DevDependencies.Count);

            return apiResponse.Data;
        }
        catch (Exception ex) when (ex is not PackageNotFoundException)
        {
            _logger.LogError(ex, "Failed to get package dependencies: {PackageName}@{Version}", packageName, version);
            throw;
        }
    }

    private void ConfigureHttpClient()
    {
        _httpClient.BaseAddress = new Uri(_configuration.Registry.Url);
        _httpClient.Timeout = TimeSpan.FromMilliseconds(_configuration.Registry.Timeout);

        // Set default headers
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "mcpm-cli/1.0.0");
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

        // Add authentication if available
        if (!string.IsNullOrEmpty(_configuration.Auth.Token))
        {
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_configuration.Auth.Token}");
        }

        _logger.LogDebug("HTTP client configured: BaseUrl={BaseUrl}, Timeout={Timeout}ms", 
            _configuration.Registry.Url, _configuration.Registry.Timeout);
    }

    private async Task EnsureSuccessStatusCodeAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
            return;

        var content = await response.Content.ReadAsStringAsync();
        var errorMessage = $"API request failed with status {response.StatusCode}";

        // Try to extract error message from response
        try
        {
            var errorResponse = JsonSerializer.Deserialize<ApiResponse<object>>(content, _jsonOptions);
            if (errorResponse?.Errors?.Any() == true)
            {
                errorMessage += $": {string.Join(", ", errorResponse.Errors)}";
            }
            else if (!string.IsNullOrEmpty(errorResponse?.Message))
            {
                errorMessage += $": {errorResponse.Message}";
            }
        }
        catch
        {
            // Ignore JSON parsing errors, use default message
        }

        throw response.StatusCode switch
        {
            HttpStatusCode.Unauthorized => new UnauthorizedAccessException("Authentication required or token invalid"),
            HttpStatusCode.Forbidden => new UnauthorizedAccessException("Access forbidden"),
            HttpStatusCode.NotFound => new PackageNotFoundException("Resource not found"),
            HttpStatusCode.TooManyRequests => new HttpRequestException("Rate limit exceeded"),
            _ => new HttpRequestException(errorMessage)
        };
    }

    private static string GetHeaderValue(HttpResponseMessage response, string headerName, string defaultValue)
    {
        return response.Headers.TryGetValues(headerName, out var values) 
            ? values.FirstOrDefault() ?? defaultValue 
            : defaultValue;
    }
}

/// <summary>
/// Exception thrown when a package is not found
/// </summary>
public class PackageNotFoundException : Exception
{
    public PackageNotFoundException(string message) : base(message) { }
    public PackageNotFoundException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Exception thrown when manifest validation fails
/// </summary>
public class ManifestValidationException : Exception
{
    public List<string> ValidationErrors { get; }
    public List<string> ValidationWarnings { get; }

    public ManifestValidationException(string message, List<string> errors, List<string>? warnings = null) : base(message)
    {
        ValidationErrors = errors ?? new List<string>();
        ValidationWarnings = warnings ?? new List<string>();
    }

    public ManifestValidationException(string message, Exception innerException) : base(message, innerException)
    {
        ValidationErrors = new List<string>();
        ValidationWarnings = new List<string>();
    }
}

/// <summary>
/// Exception thrown when a package already exists
/// </summary>
public class PackageAlreadyExistsException : Exception
{
    public PackageAlreadyExistsException(string message) : base(message) { }
    public PackageAlreadyExistsException(string message, Exception innerException) : base(message, innerException) { }
}