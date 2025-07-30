namespace MCPHub.IntegrationTests.Support;

/// <summary>
/// Extended scenario context for BDD tests with strongly-typed data storage
/// </summary>
public class SolutionScenarioContext {
    private readonly Dictionary<string, object> _data = [];

    public TestDataBuilder DataBuilder { get; } = new();

    // API Response Storage
    public HttpResponseMessage? LastApiResponse { get; set; }
    public string? LastApiResponseContent { get; set; }
    public HttpStatusCode? LastApiStatusCode { get; set; }

    // Authentication Context
    public string? CurrentUserId { get; set; }
    public string? CurrentUserEmail { get; set; }
    public List<string> CurrentUserRoles { get; set; } = [];
    public string? AuthToken { get; set; }

    // Package Context
    public Package? CurrentPackage { get; set; }
    public List<Package> PackageSearchResults { get; set; } = [];
    public SearchRequest? LastSearchRequest { get; set; }
    public PublishRequest? LastPublishRequest { get; set; }

    // Web Context
    public IWebDriver? WebDriver { get; set; }
    public string? CurrentPageUrl { get; set; }
    public Dictionary<string, string> FormData { get; set; } = [];

    // CLI Context
    public string? LastCliCommand { get; set; }
    public string? LastCliOutput { get; set; }
    public int? LastCliExitCode { get; set; }
    public Dictionary<string, string> CliConfiguration { get; set; } = [];

    // Test Data
    public TestScenario? TestScenario { get; set; }
    public List<ApplicationUser> TestUsers { get; set; } = [];

    // Generic storage for custom data
    public T? Get<T>(string key) where T : class => _data.TryGetValue(key, out var value) ? value as T : null;

    public void Set<T>(string key, T value) where T : class => _data[key] = value;

    public void Set(string key, object value) => _data[key] = value;

    public bool ContainsKey(string key) => _data.ContainsKey(key);

    public void Clear() => _data.Clear();

    // Helper methods for common operations
    public void SetCurrentUser(string userId, string email, params string[] roles) {
        CurrentUserId = userId;
        CurrentUserEmail = email;
        CurrentUserRoles = roles.ToList();
    }

    public ApplicationUser GetOrCreateTestUser(string email = "test@example.com") {
        var existingUser = TestUsers.FirstOrDefault(u => u.Email == email);
        if (existingUser != null)
            return existingUser;

        var newUser = TestDataBuilder.CreateTestUser(email);
        TestUsers.Add(newUser);
        return newUser;
    }

    public void StoreApiResponse(HttpResponseMessage response, string content) {
        LastApiResponse = response;
        LastApiResponseContent = content;
        LastApiStatusCode = response.StatusCode;
    }

    public void SetTestScenario(TestScenario scenario) {
        TestScenario = scenario;
        TestUsers.AddRange(scenario.Users);
    }
}

/// <summary>
/// Context hooks for SpecFlow scenarios
/// </summary>
[Binding]
public class ContextHooks(SolutionScenarioContext scenarioContext) {
    private readonly SolutionScenarioContext _scenarioContext = scenarioContext ?? throw new ArgumentNullException(nameof(scenarioContext));

    [BeforeScenario]
    public void BeforeScenario() {
        // Initialize scenario context
        _scenarioContext.Clear();

        // Set up default test user
        _scenarioContext.SetCurrentUser(
            Guid.CreateVersion7().ToString(),
            "test@example.com",
            "User"
        );
    }

    [AfterScenario]
    public void AfterScenario() {
        // Clean up web driver
        _scenarioContext.WebDriver?.Dispose();
        _scenarioContext.WebDriver = null;

        // Clear context
        _scenarioContext.Clear();
    }
}

/// <summary>
/// Step argument transformations for common BDD patterns
/// </summary>
[Binding]
public static class StepArgumentTransformations {
    [StepArgumentTransformation]
    public static TrustTier TransformTrustTier(string trustTier) => trustTier.ToLowerInvariant() switch {
        "unverified" => TrustTier.Unverified,
        "community" or "community trusted" => TrustTier.CommunityTrusted,
        "security audited" or "audited" => TrustTier.SecurityAudited,
        "certified" => TrustTier.Certified,
        _ => throw new ArgumentException($"Unknown trust tier: {trustTier}"),
    };

    [StepArgumentTransformation]
    public static PackageStatus TransformPackageStatus(string status) => status.ToLowerInvariant() switch {
        "active" => PackageStatus.Published,
        "deprecated" => PackageStatus.Deprecated,
        "archived" => PackageStatus.Deprecated,
        "suspended" => PackageStatus.Suspended,
        _ => throw new ArgumentException($"Unknown package status: {status}"),
    };

    [StepArgumentTransformation]
    public static HttpStatusCode TransformHttpStatusCode(string statusCode) => statusCode.ToLowerInvariant() switch {
        "200" or "ok" => HttpStatusCode.OK,
        "201" or "created" => HttpStatusCode.Created,
        "400" or "bad request" => HttpStatusCode.BadRequest,
        "401" or "unauthorized" => HttpStatusCode.Unauthorized,
        "403" or "forbidden" => HttpStatusCode.Forbidden,
        "404" or "not found" => HttpStatusCode.NotFound,
        "500" or "internal server error" => HttpStatusCode.InternalServerError,
        _ when int.TryParse(statusCode, out var code) => (HttpStatusCode)code,
        _ => throw new ArgumentException($"Unknown status code: {statusCode}"),
    };

    [StepArgumentTransformation]
    public static Dictionary<string, string> TransformTable(Table table) {
        var dictionary = new Dictionary<string, string>();
        foreach (var row in table.Rows) {
            dictionary[row[0]] = row[1];
        }
        return dictionary;
    }

    [StepArgumentTransformation]
    public static List<string> TransformStringList(string commaSeparatedValues) => commaSeparatedValues
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Trim())
            .Where(s => !string.IsNullOrEmpty(s))
            .ToList();
}