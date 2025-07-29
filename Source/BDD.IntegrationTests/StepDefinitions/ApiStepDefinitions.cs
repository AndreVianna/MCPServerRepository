namespace MCPHub.BDD.IntegrationTests.StepDefinitions;

[Binding]
[Collection("TestContainer")]
public class ApiStepDefinitions
{
    private readonly TestContainerFixture _fixture;
    private readonly BddScenarioContext _scenarioContext;
    private HttpClient _apiClient = null!;

    public ApiStepDefinitions(TestContainerFixture fixture, BddScenarioContext scenarioContext)
    {
        _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
        _scenarioContext = scenarioContext ?? throw new ArgumentNullException(nameof(scenarioContext));
    }

    [Given(@"the MCP Hub API is running")]
    public void GivenTheMCPHubAPIIsRunning()
    {
        _apiClient = _fixture.WebApplicationFactory.CreateClient();
        _apiClient.Should().NotBeNull();
    }

    [Given(@"the database is initialized with test data")]
    public async Task GivenTheDatabaseIsInitializedWithTestData()
    {
        // Database is already initialized in the fixture
        // Add any additional test data if needed
        using var scope = _fixture.WebApplicationFactory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<McpHubContext>();
        
        // Verify database is accessible
        var canConnect = await context.Database.CanConnectAsync();
        canConnect.Should().BeTrue();
    }

    [Given(@"there are (\d+) packages in the registry")]
    public async Task GivenThereArePackagesInTheRegistry(int packageCount)
    {
        var scenario = new TestScenarioBuilder()
            .WithPackages(packageCount)
            .Build();

        await SeedTestData(scenario);
        _scenarioContext.SetTestScenario(scenario);
    }

    [Given(@"there are packages in the following categories:")]
    public async Task GivenThereArePackagesInTheFollowingCategories(Table table)
    {
        var packages = new List<Package>();
        var dataBuilder = new TestDataBuilder();

        foreach (var row in table.Rows)
        {
            var category = row["Category"];
            var count = int.Parse(row["Count"]);

            for (int i = 0; i < count; i++)
            {
                var package = dataBuilder.CreateTestPackages(1).First();
                package.Categories = new[] { category };
                packages.Add(package);
            }
        }

        var scenario = new TestScenario { Packages = packages };
        await SeedTestData(scenario);
        _scenarioContext.SetTestScenario(scenario);
    }

    [Given(@"there are packages with the following trust tiers:")]
    public async Task GivenThereArePackagesWithTheFollowingTrustTiers(Table table)
    {
        var packages = new List<Package>();
        var dataBuilder = new TestDataBuilder();

        foreach (var row in table.Rows)
        {
            var trustTierText = row["Trust Tier"];
            var count = int.Parse(row["Count"]);
            var trustTier = ParseTrustTier(trustTierText);

            for (int i = 0; i < count; i++)
            {
                var package = dataBuilder.CreateTestPackages(1).First();
                package.TrustTier = trustTier;
                packages.Add(package);
            }
        }

        var scenario = new TestScenario { Packages = packages };
        await SeedTestData(scenario);
        _scenarioContext.SetTestScenario(scenario);
    }

    [Given(@"there is a package with name ""(.*)""")]
    public async Task GivenThereIsAPackageWithName(string packageName)
    {
        var dataBuilder = new TestDataBuilder();
        var package = dataBuilder.CreateTestPackages(1).First();
        package.Name = packageName;

        var scenario = new TestScenario { Packages = new[] { package }.ToList() };
        await SeedTestData(scenario);
        _scenarioContext.SetTestScenario(scenario);
        _scenarioContext.CurrentPackage = package;
    }

    [Given(@"there are publishers with packages:")]
    public async Task GivenThereArePublishersWithPackages(Table table)
    {
        var packages = new List<Package>();
        var dataBuilder = new TestDataBuilder();

        foreach (var row in table.Rows)
        {
            var publisherName = row["Publisher"];
            var packageCount = int.Parse(row["Package Count"]);

            var publisher = dataBuilder.CreateTestPublishers(1).First();
            publisher.Name = publisherName;

            for (int i = 0; i < packageCount; i++)
            {
                var package = dataBuilder.CreateTestPackages(1).First();
                package.Publisher = publisher;
                package.PublisherId = publisher.Id;
                packages.Add(package);
            }
        }

        var scenario = new TestScenario { Packages = packages };
        await SeedTestData(scenario);
        _scenarioContext.SetTestScenario(scenario);
    }

    [Given(@"I am authenticated as a publisher")]
    public void GivenIAmAuthenticatedAsAPublisher()
    {
        var userId = Guid.CreateVersion7().ToString();
        var userEmail = "publisher@example.com";
        
        _apiClient?.Dispose();
        _apiClient = _fixture.WebApplicationFactory.CreateClientWithAuth(userId, userEmail, new[] { "Publisher" });
        
        _scenarioContext.SetCurrentUser(userId, userEmail, "Publisher");
    }

    [Given(@"I am authenticated as user ""(.*)""")]
    public void GivenIAmAuthenticatedAsUser(string email)
    {
        var userId = Guid.CreateVersion7().ToString();
        
        _apiClient?.Dispose();
        _apiClient = _fixture.WebApplicationFactory.CreateClientWithAuth(userId, email, new[] { "User" });
        
        _scenarioContext.SetCurrentUser(userId, email, "User");
    }

    [Given(@"I am not authenticated")]
    public void GivenIAmNotAuthenticated()
    {
        _apiClient?.Dispose();
        _apiClient = _fixture.WebApplicationFactory.CreateClient();
        _scenarioContext.CurrentUserId = null;
        _scenarioContext.CurrentUserEmail = null;
        _scenarioContext.CurrentUserRoles.Clear();
    }

    [When(@"I search for packages with query ""(.*)""")]
    public async Task WhenISearchForPackagesWithQuery(string query)
    {
        var response = await _apiClient.GetAsync($"/api/packages/search?query={Uri.EscapeDataString(query)}");
        var content = await response.Content.ReadAsStringAsync();

        _scenarioContext.StoreApiResponse(response, content);
    }

    [When(@"I search for packages with query ""(.*)"" and category ""(.*)""")]
    public async Task WhenISearchForPackagesWithQueryAndCategory(string query, string category)
    {
        var url = $"/api/packages/search/advanced?q={Uri.EscapeDataString(query)}&categories={Uri.EscapeDataString(category)}";
        var response = await _apiClient.GetAsync(url);
        var content = await response.Content.ReadAsStringAsync();

        _scenarioContext.StoreApiResponse(response, content);
    }

    [When(@"I search for packages with minimum trust tier ""(.*)""")]
    public async Task WhenISearchForPackagesWithMinimumTrustTier(string trustTierText)
    {
        var trustTier = ParseTrustTier(trustTierText);
        var url = $"/api/packages/search/advanced?q=*&trustTier={trustTier}";
        var response = await _apiClient.GetAsync(url);
        var content = await response.Content.ReadAsStringAsync();

        _scenarioContext.StoreApiResponse(response, content);
    }

    [When(@"I search for packages with query ""(.*)"" and page size (\d+) and page (\d+)")]
    public async Task WhenISearchForPackagesWithQueryAndPagination(string query, int pageSize, int page)
    {
        var url = $"/api/packages/search/advanced?q={Uri.EscapeDataString(query)}&pageSize={pageSize}&page={page}";
        var response = await _apiClient.GetAsync(url);
        var content = await response.Content.ReadAsStringAsync();

        _scenarioContext.StoreApiResponse(response, content);
    }

    [When(@"I request the package by its ID")]
    public async Task WhenIRequestThePackageByItsID()
    {
        var package = _scenarioContext.CurrentPackage;
        package.Should().NotBeNull("A current package should be set in the scenario context");

        var response = await _apiClient.GetAsync($"/api/packages/{package!.Id}");
        var content = await response.Content.ReadAsStringAsync();

        _scenarioContext.StoreApiResponse(response, content);
    }

    [When(@"I request the package by name ""(.*)""")]
    public async Task WhenIRequestThePackageByName(string packageName)
    {
        var response = await _apiClient.GetAsync($"/api/packages/by-name/{Uri.EscapeDataString(packageName)}");
        var content = await response.Content.ReadAsStringAsync();

        _scenarioContext.StoreApiResponse(response, content);
    }

    [When(@"I request a package with non-existent ID")]
    public async Task WhenIRequestAPackageWithNonExistentID()
    {
        var nonExistentId = Guid.CreateVersion7();
        var response = await _apiClient.GetAsync($"/api/packages/{nonExistentId}");
        var content = await response.Content.ReadAsStringAsync();

        _scenarioContext.StoreApiResponse(response, content);
    }

    [When(@"I request packages for publisher ""(.*)""")]
    public async Task WhenIRequestPackagesForPublisher(string publisherName)
    {
        // Find the publisher ID from test data
        var publisher = _scenarioContext.TestScenario?.Packages
            .FirstOrDefault(p => p.Publisher.Name == publisherName)?.Publisher;
        
        publisher.Should().NotBeNull($"Publisher '{publisherName}' should exist in test data");

        var response = await _apiClient.GetAsync($"/api/packages/by-publisher/{publisher!.Id}");
        var content = await response.Content.ReadAsStringAsync();

        _scenarioContext.StoreApiResponse(response, content);
    }

    [Then(@"I should receive a successful response")]
    public void ThenIShouldReceiveASuccessfulResponse()
    {
        _scenarioContext.LastApiResponse.Should().NotBeNull();
        _scenarioContext.LastApiResponse!.IsSuccessStatusCode.Should().BeTrue(
            $"Expected success status code, but got {_scenarioContext.LastApiStatusCode}. Response: {_scenarioContext.LastApiResponseContent}");
    }

    [Then(@"I should receive a ""(.*)"" response")]
    public void ThenIShouldReceiveAResponse(HttpStatusCode expectedStatusCode)
    {
        _scenarioContext.LastApiResponse.Should().NotBeNull();
        _scenarioContext.LastApiStatusCode.Should().Be(expectedStatusCode,
            $"Expected {expectedStatusCode}, but got {_scenarioContext.LastApiStatusCode}. Response: {_scenarioContext.LastApiResponseContent}");
    }

    [Then(@"the response should contain packages matching ""(.*)""")]
    public void ThenTheResponseShouldContainPackagesMatching(string searchTerm)
    {
        _scenarioContext.LastApiResponseContent.Should().NotBeNull();
        
        var packages = JsonSerializer.Deserialize<SearchResult<Package>>(
            _scenarioContext.LastApiResponseContent!,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        packages.Should().NotBeNull();
        packages!.Items.Should().NotBeEmpty();
        
        // Verify that packages contain the search term in name or description
        packages.Items.Should().OnlyContain(p => 
            p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
            p.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
    }

    [Then(@"each package should have required fields populated")]
    public void ThenEachPackageShouldHaveRequiredFieldsPopulated()
    {
        _scenarioContext.LastApiResponseContent.Should().NotBeNull();
        
        var packages = JsonSerializer.Deserialize<SearchResult<Package>>(
            _scenarioContext.LastApiResponseContent!,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        packages.Should().NotBeNull();
        packages!.Items.Should().NotBeEmpty();

        foreach (var package in packages.Items)
        {
            package.Id.Should().NotBeEmpty();
            package.Name.Should().NotBeNullOrEmpty();
            package.Description.Should().NotBeNullOrEmpty();
            package.PublisherId.Should().NotBeEmpty();
            package.TrustTier.Should().BeDefined();
        }
    }

    [Then(@"all returned packages should be in category ""(.*)""")]
    public void ThenAllReturnedPackagesShouldBeInCategory(string category)
    {
        _scenarioContext.LastApiResponseContent.Should().NotBeNull();
        
        var searchResult = JsonSerializer.Deserialize<SearchResult<Package>>(
            _scenarioContext.LastApiResponseContent!,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        searchResult.Should().NotBeNull();
        searchResult!.Items.Should().NotBeEmpty();
        
        searchResult.Items.Should().OnlyContain(p => p.Categories.Contains(category));
    }

    [Then(@"the total count should be (\d+)")]
    public void ThenTheTotalCountShouldBe(int expectedCount)
    {
        _scenarioContext.LastApiResponseContent.Should().NotBeNull();
        
        var searchResult = JsonSerializer.Deserialize<SearchResult<Package>>(
            _scenarioContext.LastApiResponseContent!,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        searchResult.Should().NotBeNull();
        searchResult!.TotalCount.Should().Be(expectedCount);
    }

    [Then(@"all returned packages should have trust tier ""(.*)"" or higher")]
    public void ThenAllReturnedPackagesShouldHaveTrustTierOrHigher(string minimumTrustTierText)
    {
        var minimumTrustTier = ParseTrustTier(minimumTrustTierText);
        
        _scenarioContext.LastApiResponseContent.Should().NotBeNull();
        
        var searchResult = JsonSerializer.Deserialize<SearchResult<Package>>(
            _scenarioContext.LastApiResponseContent!,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        searchResult.Should().NotBeNull();
        searchResult!.Items.Should().NotBeEmpty();
        
        searchResult.Items.Should().OnlyContain(p => p.TrustTier >= minimumTrustTier);
    }

    private async Task SeedTestData(TestScenario scenario)
    {
        using var scope = _fixture.WebApplicationFactory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<McpHubContext>();

        // Clear existing test data
        context.Packages.RemoveRange(context.Packages);
        context.Publishers.RemoveRange(context.Publishers);
        await context.SaveChangesAsync();

        // Add test data
        if (scenario.Packages.Any())
        {
            var publishers = scenario.Packages.Select(p => p.Publisher).Distinct().ToList();
            context.Publishers.AddRange(publishers);
            await context.SaveChangesAsync();

            context.Packages.AddRange(scenario.Packages);
            await context.SaveChangesAsync();
        }
    }

    private static TrustTier ParseTrustTier(string trustTierText)
    {
        return trustTierText.ToLowerInvariant() switch
        {
            "unverified" => TrustTier.Unverified,
            "community" or "community trusted" => TrustTier.CommunityTrusted,
            "security audited" or "audited" => TrustTier.SecurityAudited,
            "certified" => TrustTier.Certified,
            _ => throw new ArgumentException($"Unknown trust tier: {trustTierText}")
        };
    }
}

/// <summary>
/// Search result wrapper for API responses
/// </summary>
public class SearchResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public double SearchTimeMs { get; set; }
}