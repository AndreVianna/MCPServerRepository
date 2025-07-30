namespace MCPHub.IntegrationTests.Builders;

/// <summary>
/// Builder for creating test data using Bogus library
/// </summary>
public class TestDataBuilder {
    private readonly Faker _faker = new();

    public List<Package> CreateTestPackages(int count = 10) {
        var publishers = CreateTestPublishers(3);
        var packageFaker = new Faker<Package>()
            .RuleFor(p => p.Id, _ => Guid.CreateVersion7())
            .RuleFor(p => p.Name, f => $"mcp-{f.Hacker.Noun()}-{f.Random.Word()}")
            .RuleFor(p => p.Description, f => f.Lorem.Sentence())
            .RuleFor(p => p.Publisher, f => f.PickRandom(publishers))
            .RuleFor(p => p.PublisherId, (f, p) => p.Publisher.Id)
            .RuleFor(p => p.Status, f => f.PickRandom<PackageStatus>())
            .RuleFor(p => p.TrustTier, f => f.PickRandom<TrustTier>())
            .RuleFor(p => p.Tags, f => f.Make(f.Random.Int(2, 5), () => f.Hacker.Noun()))
            .FinishWith((f, p) => {
                // Add audit trail entries
                p.AuditTrail.Add(new AuditEntry { Action = "Created", UserId = Guid.Empty, DateTime = DateTimeOffset.UtcNow });

                // Add versions
                var versions = CreateTestVersionsForPackage(p, f.Random.Int(1, 3));
                foreach (var version in versions) {
                    p.Versions.Add(version);
                }

                // Note: Security scans are handled separately as they reference PackageVersion
            });

        return packageFaker.Generate(count);
    }

    public List<Publisher> CreateTestPublishers(int count = 3) {
        var publisherFaker = new Faker<Publisher>()
            .RuleFor(p => p.Id, _ => Guid.CreateVersion7())
            .RuleFor(p => p.Name, f => f.Company.CompanyName())
            .RuleFor(p => p.Email, f => f.Internet.Email())
            .RuleFor(p => p.Website, f => f.Internet.Url())
            .RuleFor(p => p.Type, f => f.PickRandom<PublisherType>())
            .RuleFor(p => p.Verified, f => f.Random.Bool(0.7f))
            .FinishWith((f, p) => p.AuditTrail.Add(new AuditEntry { Action = "Created", UserId = Guid.Empty, DateTime = DateTimeOffset.UtcNow }));

        return publisherFaker.Generate(count);
    }

    public List<PackageVersion> CreateTestVersionsForPackage(Package package, int count = 3) {
        var versionFaker = new Faker<PackageVersion>()
            .RuleFor(v => v.Id, _ => Guid.CreateVersion7())
            .RuleFor(v => v.PackageId, _ => package.Id)
            .RuleFor(v => v.Version, f => f.System.Version().ToString())
            .RuleFor(v => v.ReleaseNotes, f => f.Lorem.Paragraph())
            .FinishWith((f, v) => v.AuditTrail.Add(new AuditEntry { Action = "Created", UserId = Guid.Empty, DateTime = DateTimeOffset.UtcNow }));

        return versionFaker.Generate(count);
    }

    public List<SecurityScan> CreateTestSecurityScans(Package package, int count = 1) {
        var scanFaker = new Faker<SecurityScan>()
            .CustomInstantiator(f => new SecurityScan(
                package.Versions.FirstOrDefault()?.Id ?? Guid.CreateVersion7(), // Use first version or create dummy
                f.PickRandom<ScanType>(),
                f.System.Version().ToString(),
                true // isPackageScan
            ))
            .RuleFor(s => s.Result, f => CreateTestSecurityScanResult());

        return scanFaker.Generate(count);
    }

    public SecurityScanResult CreateTestSecurityScanResult() {
        var vulnerabilities = CreateTestVulnerabilities(_faker.Random.Int(0, 5));

        return new SecurityScanResult(
            _faker.PickRandom<SecurityScanStatus>(),
            vulnerabilities,
            _faker.System.Version().ToString(),
            _faker.Lorem.Paragraph()
        );
    }

    public List<SecurityVulnerability> CreateTestVulnerabilities(int count = 3) {
        var vulnerabilityFaker = new Faker<SecurityVulnerability>()
            .CustomInstantiator(f => new SecurityVulnerability(
                f.Random.AlphaNumeric(10),
                f.Hacker.Phrase(),
                f.Lorem.Sentence(),
                f.PickRandom<DomainSecurityScanSeverity>(),
                f.Random.Bool() ? f.Random.AlphaNumeric(15) : null,
                f.Internet.Url(),
                f.Hacker.Noun(),
                f.Random.Double(0.1, 10.0),
                f.Lorem.Sentence(),
                f.Make(f.Random.Int(1, 3), () => f.System.Version().ToString())
            ));

        return vulnerabilityFaker.Generate(count);
    }

    public ApplicationUser CreateTestUser(string? email = null, string? userId = null) {
        var userFaker = new Faker<ApplicationUser>()
            .RuleFor(u => u.Id, _ => Guid.Parse(userId ?? Guid.CreateVersion7().ToString()))
            .RuleFor(u => u.Email, f => email ?? f.Internet.Email())
            .RuleFor(u => u.UserName, (f, u) => u.Email)
            .RuleFor(u => u.NormalizedEmail, (f, u) => u.Email?.ToUpperInvariant())
            .RuleFor(u => u.NormalizedUserName, (f, u) => u.Email?.ToUpperInvariant())
            .RuleFor(u => u.DisplayName, f => f.Name.FullName())
            .RuleFor(u => u.EmailConfirmed, f => f.Random.Bool(0.8f))
            .RuleFor(u => u.SecurityStamp, f => f.Random.Guid().ToString())
            .RuleFor(u => u.ConcurrencyStamp, f => f.Random.Guid().ToString())
            .FinishWith((f, u) => u.AuditTrail.Add(new AuditEntry { Action = "Created", UserId = Guid.Empty, DateTime = DateTimeOffset.UtcNow }));

        return userFaker.Generate();
    }

    public SearchRequest CreateSearchRequest(
        string query = "test",
        IEnumerable<string>? categories = null,
        TrustTier? minimumTrustTier = null,
        int page = 1,
        int pageSize = 20) => new() {
            Query = query,
            Categories = categories,
            MinimumTrustTier = minimumTrustTier,
            Page = page,
            PageSize = pageSize,
            SortBy = "relevance",
            SortDirection = DomainSortDirection.Descending
        };

    public PublishRequest CreatePublishRequest(string packageName = "test-package") {
        var manifest = CreateTestMCPManifest(packageName);

        return new PublishRequest {
            ManifestContent = JsonSerializer.Serialize(manifest),
            PackageArchive = CreateTestPackageBytes(),
            Tags = new[] { "test", "mcp", "automation" },
            ReadmeContent = _faker.Lorem.Paragraphs(5),
            ChangelogContent = _faker.Lorem.Paragraph()
        };
    }

    public MCPManifest CreateTestMCPManifest(string name = "test-package") => new() {
        Name = name,
        Version = "1.0.0",
        Description = _faker.Lorem.Sentence(),
        Author = new MCPAuthor { Name = _faker.Name.FullName(), Email = _faker.Internet.Email() },
        License = "MIT",
        Repository = _faker.Internet.Url(),
        Capabilities = new MCPCapabilities {
            Tools = CreateTestToolDefinitions(2),
            Resources = CreateTestResourceDefinitions(1),
            Prompts = CreateTestPromptDefinitions(1)
        }
    };

    private byte[] CreateTestPackageBytes() {
        var testData = _faker.Lorem.Paragraphs(10);
        return Encoding.UTF8.GetBytes(string.Join("\n", testData));
    }

    private List<MCPTool> CreateTestToolDefinitions(int count) {
        var toolFaker = new Faker<MCPTool>()
            .RuleFor(t => t.Name, f => f.Hacker.Noun())
            .RuleFor(t => t.Description, f => f.Hacker.Phrase())
            .RuleFor(t => t.InputSchema, f => new Dictionary<string, object> {
                ["type"] = "object",
                ["properties"] = new Dictionary<string, object> {
                    ["query"] = new Dictionary<string, object> {
                        ["type"] = "string",
                        ["description"] = f.Lorem.Sentence()
                    }
                }
            });

        return toolFaker.Generate(count);
    }

    private List<MCPResource> CreateTestResourceDefinitions(int count) {
        var resourceFaker = new Faker<MCPResource>()
            .RuleFor(r => r.Name, f => f.System.FileName())
            .RuleFor(r => r.Description, f => f.Hacker.Phrase())
            .RuleFor(r => r.Uri, f => f.Internet.Url())
            .RuleFor(r => r.MimeType, f => f.PickRandom("text/plain", "application/json", "text/html"));

        return resourceFaker.Generate(count);
    }

    private List<MCPPrompt> CreateTestPromptDefinitions(int count) {
        var promptFaker = new Faker<MCPPrompt>()
            .RuleFor(p => p.Name, f => f.Hacker.Noun())
            .RuleFor(p => p.Description, f => f.Hacker.Phrase())
            .RuleFor(p => p.Arguments, f => new List<MCPPromptArgument>
            {
                new() { Name = "input", Description = "Input parameter" },
                new() { Name = "context", Description = "Context parameter" }
            });

        return promptFaker.Generate(count);
    }
}

/// <summary>
/// Fluent builder for creating test scenarios
/// </summary>
public class TestScenarioBuilder {
    private readonly TestDataBuilder _dataBuilder = new();
    private readonly List<Package> _packages = new();
    private readonly List<ApplicationUser> _users = new();

    public TestScenarioBuilder WithPackages(int count = 5) {
        _packages.AddRange(_dataBuilder.CreateTestPackages(count));
        return this;
    }

    public TestScenarioBuilder WithPackage(Package package) {
        _packages.Add(package);
        return this;
    }

    public TestScenarioBuilder WithUsers(int count = 3) {
        for (var i = 0; i < count; i++) {
            _users.Add(_dataBuilder.CreateTestUser());
        }
        return this;
    }

    public TestScenarioBuilder WithUser(ApplicationUser user) {
        _users.Add(user);
        return this;
    }

    public TestScenario Build() => new() {
        Packages = _packages,
        Users = _users,
        DataBuilder = _dataBuilder
    };
}

/// <summary>
/// Contains test scenario data for BDD tests
/// </summary>
public class TestScenario {
    public List<Package> Packages { get; set; } = new();
    public List<ApplicationUser> Users { get; set; } = new();
    public TestDataBuilder DataBuilder { get; set; } = new();
}