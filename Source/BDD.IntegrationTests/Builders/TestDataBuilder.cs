namespace MCPHub.BDD.IntegrationTests.Builders;

/// <summary>
/// Builder for creating test data using Bogus library
/// </summary>
public class TestDataBuilder
{
    private readonly Faker _faker = new();

    public List<Package> CreateTestPackages(int count = 10)
    {
        var publishers = CreateTestPublishers(3);
        var packageFaker = new Faker<Package>()
            .RuleFor(p => p.Id, _ => Guid.CreateVersion7())
            .RuleFor(p => p.Name, f => $"mcp-{f.Hacker.Noun()}-{f.Random.Word()}")
            .RuleFor(p => p.Description, f => f.Lorem.Sentence())
            .RuleFor(p => p.Publisher, f => f.PickRandom(publishers))
            .RuleFor(p => p.PublisherId, (f, p) => p.Publisher.Id)
            .RuleFor(p => p.Status, f => f.PickRandom<PackageStatus>())
            .RuleFor(p => p.TrustTier, f => f.PickRandom<TrustTier>())
            .RuleFor(p => p.DownloadCount, f => f.Random.Long(0, 100000))
            .RuleFor(p => p.Categories, f => f.Make(f.Random.Int(1, 3), () => f.PickRandom("ai", "data", "files", "web", "tools", "security")))
            .RuleFor(p => p.Tags, f => f.Make(f.Random.Int(2, 5), () => f.Hacker.Noun()))
            .FinishWith((f, p) =>
            {
                // Add audit trail entries
                p.AddAuditEntry("Created", Guid.Empty);
                
                // Add versions
                var versions = CreateTestVersionsForPackage(p, f.Random.Int(1, 3));
                foreach (var version in versions)
                {
                    p.Versions.Add(version);
                }

                // Add security scans
                var scans = CreateTestSecurityScans(p, f.Random.Int(0, 2));
                foreach (var scan in scans)
                {
                    p.SecurityScans.Add(scan);
                }
            });

        return packageFaker.Generate(count);
    }

    public List<Publisher> CreateTestPublishers(int count = 3)
    {
        var publisherFaker = new Faker<Publisher>()
            .RuleFor(p => p.Id, _ => Guid.CreateVersion7())
            .RuleFor(p => p.Name, f => f.Company.CompanyName())
            .RuleFor(p => p.DisplayName, (f, p) => p.Name)
            .RuleFor(p => p.Email, f => f.Internet.Email())
            .RuleFor(p => p.Website, f => f.Internet.Url())
            .RuleFor(p => p.Type, f => f.PickRandom<PublisherType>())
            .RuleFor(p => p.Verified, f => f.Random.Bool(0.7f))
            .FinishWith((f, p) => p.AddAuditEntry("Created", Guid.Empty));

        return publisherFaker.Generate(count);
    }

    public List<PackageVersion> CreateTestVersionsForPackage(Package package, int count = 3)
    {
        var versionFaker = new Faker<PackageVersion>()
            .RuleFor(v => v.Id, _ => Guid.CreateVersion7())
            .RuleFor(v => v.PackageId, _ => package.Id)
            .RuleFor(v => v.Version, f => f.System.Version().ToString())
            .RuleFor(v => v.ReleaseNotes, f => f.Lorem.Paragraph())
            .RuleFor(v => v.Status, f => f.PickRandom<VersionStatus>())
            .RuleFor(v => v.Downloads, f => f.Random.Long(0, 10000))
            .RuleFor(v => v.PackageSize, f => f.Random.Long(1024, 10485760)) // 1KB to 10MB
            .RuleFor(v => v.ManifestHash, f => f.Random.Hash(64))
            .RuleFor(v => v.PackageHash, f => f.Random.Hash(64))
            .FinishWith((f, v) => v.AddAuditEntry("Created", Guid.Empty));

        return versionFaker.Generate(count);
    }

    public List<SecurityScan> CreateTestSecurityScans(Package package, int count = 1)
    {
        var scanFaker = new Faker<SecurityScan>()
            .RuleFor(s => s.Id, _ => Guid.CreateVersion7())
            .RuleFor(s => s.PackageId, _ => package.Id)
            .RuleFor(s => s.Type, f => f.PickRandom<ScanType>())
            .RuleFor(s => s.Status, f => f.PickRandom<ScanStatus>())
            .RuleFor(s => s.Result, f => CreateTestSecurityScanResult())
            .FinishWith((f, s) => s.AddAuditEntry("Created", Guid.Empty));

        return scanFaker.Generate(count);
    }

    public SecurityScanResult CreateTestSecurityScanResult()
    {
        var vulnerabilities = CreateTestVulnerabilities(_faker.Random.Int(0, 5));
        var highestSeverity = vulnerabilities.Any() 
            ? vulnerabilities.Max(v => v.Severity) 
            : SecurityScanSeverity.None;

        return new SecurityScanResult
        {
            Status = _faker.PickRandom<SecurityScanStatus>(),
            HighestSeverity = highestSeverity,
            Vulnerabilities = vulnerabilities,
            SecurityScore = _faker.Random.Double(1.0, 10.0),
            ScannedAt = _faker.Date.Recent(30)
        };
    }

    public List<SecurityVulnerability> CreateTestVulnerabilities(int count = 3)
    {
        var vulnerabilityFaker = new Faker<SecurityVulnerability>()
            .RuleFor(v => v.Id, f => f.Random.AlphaNumeric(10))
            .RuleFor(v => v.Title, f => f.Hacker.Phrase())
            .RuleFor(v => v.Description, f => f.Lorem.Sentence())
            .RuleFor(v => v.Severity, f => f.PickRandom<SecurityScanSeverity>())
            .RuleFor(v => v.Score, f => f.Random.Double(0.1, 10.0))
            .RuleFor(v => v.DiscoveredAt, f => f.Date.Recent(30));

        return vulnerabilityFaker.Generate(count);
    }

    public ApplicationUser CreateTestUser(string? email = null, string? userId = null)
    {
        var userFaker = new Faker<ApplicationUser>()
            .RuleFor(u => u.Id, _ => Guid.Parse(userId ?? Guid.CreateVersion7().ToString()))
            .RuleFor(u => u.Email, f => email ?? f.Internet.Email())
            .RuleFor(u => u.UserName, (f, u) => u.Email)
            .RuleFor(u => u.NormalizedEmail, (f, u) => u.Email?.ToUpperInvariant())
            .RuleFor(u => u.NormalizedUserName, (f, u) => u.Email?.ToUpperInvariant())
            .RuleFor(u => u.FirstName, f => f.Name.FirstName())
            .RuleFor(u => u.LastName, f => f.Name.LastName())
            .RuleFor(u => u.DisplayName, (f, u) => $"{u.FirstName} {u.LastName}")
            .RuleFor(u => u.EmailConfirmed, f => f.Random.Bool(0.8f))
            .RuleFor(u => u.SecurityStamp, f => f.Random.Guid().ToString())
            .RuleFor(u => u.ConcurrencyStamp, f => f.Random.Guid().ToString())
            .FinishWith((f, u) => u.AddAuditEntry("Created", Guid.Empty));

        return userFaker.Generate();
    }

    public SearchRequest CreateSearchRequest(
        string query = "test",
        IEnumerable<string>? categories = null,
        TrustTier? minimumTrustTier = null,
        int page = 1,
        int pageSize = 20)
    {
        return new SearchRequest
        {
            Query = query,
            Categories = categories,
            MinimumTrustTier = minimumTrustTier,
            Page = page,
            PageSize = pageSize,
            SortBy = "relevance",
            SortDirection = SortDirection.Descending
        };
    }

    public PublishRequest CreatePublishRequest(string packageName = "test-package")
    {
        var manifest = CreateTestMCPManifest(packageName);
        
        return new PublishRequest
        {
            Manifest = manifest,
            PackageData = CreateTestPackageStream(),
            PublisherId = Guid.CreateVersion7()
        };
    }

    public MCPManifest CreateTestMCPManifest(string name = "test-package")
    {
        return new MCPManifest
        {
            Name = name,
            Version = "1.0.0",
            Description = _faker.Lorem.Sentence(),
            Author = _faker.Name.FullName(),
            License = "MIT",
            Repository = _faker.Internet.Url(),
            Tools = CreateTestToolDefinitions(2),
            Resources = CreateTestResourceDefinitions(1),
            Prompts = CreateTestPromptDefinitions(1)
        };
    }

    private Stream CreateTestPackageStream()
    {
        var testData = _faker.Lorem.Paragraphs(10);
        var bytes = Encoding.UTF8.GetBytes(string.Join("\n", testData));
        return new MemoryStream(bytes);
    }

    private List<ToolDefinition> CreateTestToolDefinitions(int count)
    {
        var toolFaker = new Faker<ToolDefinition>()
            .RuleFor(t => t.Name, f => f.Hacker.Noun())
            .RuleFor(t => t.Description, f => f.Hacker.Phrase())
            .RuleFor(t => t.Parameters, f => new Dictionary<string, object>
            {
                ["type"] = "object",
                ["properties"] = new Dictionary<string, object>
                {
                    ["query"] = new Dictionary<string, object>
                    {
                        ["type"] = "string",
                        ["description"] = f.Lorem.Sentence()
                    }
                }
            });

        return toolFaker.Generate(count);
    }

    private List<ResourceDefinition> CreateTestResourceDefinitions(int count)
    {
        var resourceFaker = new Faker<ResourceDefinition>()
            .RuleFor(r => r.Name, f => f.System.FileName())
            .RuleFor(r => r.Description, f => f.Hacker.Phrase())
            .RuleFor(r => r.Uri, f => f.Internet.Url())
            .RuleFor(r => r.MimeType, f => f.PickRandom("text/plain", "application/json", "text/html"));

        return resourceFaker.Generate(count);
    }

    private List<PromptDefinition> CreateTestPromptDefinitions(int count)
    {
        var promptFaker = new Faker<PromptDefinition>()
            .RuleFor(p => p.Name, f => f.Hacker.Noun())
            .RuleFor(p => p.Description, f => f.Hacker.Phrase())
            .RuleFor(p => p.Template, f => f.Lorem.Sentence())
            .RuleFor(p => p.Parameters, f => new List<string> { "input", "context" });

        return promptFaker.Generate(count);
    }
}

/// <summary>
/// Fluent builder for creating test scenarios
/// </summary>
public class TestScenarioBuilder
{
    private readonly TestDataBuilder _dataBuilder = new();
    private readonly List<Package> _packages = new();
    private readonly List<ApplicationUser> _users = new();

    public TestScenarioBuilder WithPackages(int count = 5)
    {
        _packages.AddRange(_dataBuilder.CreateTestPackages(count));
        return this;
    }

    public TestScenarioBuilder WithPackage(Package package)
    {
        _packages.Add(package);
        return this;
    }

    public TestScenarioBuilder WithUsers(int count = 3)
    {
        for (int i = 0; i < count; i++)
        {
            _users.Add(_dataBuilder.CreateTestUser());
        }
        return this;
    }

    public TestScenarioBuilder WithUser(ApplicationUser user)
    {
        _users.Add(user);
        return this;
    }

    public TestScenario Build()
    {
        return new TestScenario
        {
            Packages = _packages,
            Users = _users,
            DataBuilder = _dataBuilder
        };
    }
}

/// <summary>
/// Contains test scenario data for BDD tests
/// </summary>
public class TestScenario
{
    public List<Package> Packages { get; set; } = new();
    public List<ApplicationUser> Users { get; set; } = new();
    public TestDataBuilder DataBuilder { get; set; } = new();
}