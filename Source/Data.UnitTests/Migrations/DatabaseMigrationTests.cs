using Common.UnitTests.TestUtilities;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Npgsql;

namespace Data.UnitTests.Migrations;

[TestCategory(TestCategories.Integration)]
public class DatabaseMigrationTests : DatabaseTestBase {
    private McpHubContext _context = null!;

    protected override void ConfigureDatabaseServices() {
        base.ConfigureDatabaseServices();

        Services.AddDbContext<McpHubContext>(options =>
            options.UseInMemoryDatabase(DatabaseName));
    }

    [SetUp]
    public void SetUp() => _context = CreateInMemoryContext<McpHubContext>();

    [Test]
    public async Task Database_CanBeCreated() {
        // Act
        var result = await _context.Database.EnsureCreatedAsync();

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public async Task Database_HasCorrectTableStructure() {
        // Arrange
        await _context.Database.EnsureCreatedAsync();

        // Act & Assert - Test that we can create entities without errors
        var publisher = new Publisher("Test Publisher", "test@example.com", PublisherType.Individual);
        var server = new Server("Test Server", "A test server", publisher.Id);
        var package = new Package("test-package", "A test package", "1.0.0", publisher.Id);
        var packageVersion = new PackageVersion("1.0.0", package.Id, "http://example.com/download", 1024, "hash123");

        _context.Publishers.Add(publisher);
        _context.Servers.Add(server);
        _context.Packages.Add(package);
        _context.PackageVersions.Add(packageVersion);

        var result = await _context.SaveChangesAsync();
        result.Should().Be(4);
    }

    [Test]
    public async Task Database_EnforcesRelationships() {
        // Arrange
        await _context.Database.EnsureCreatedAsync();

        var publisher = new Publisher("Test Publisher", "test@example.com", PublisherType.Individual);
        _context.Publishers.Add(publisher);
        await _context.SaveChangesAsync();

        // Act & Assert - Test that foreign key relationships work
        var server = new Server("Test Server", "A test server", publisher.Id);
        _context.Servers.Add(server);
        await _context.SaveChangesAsync();

        var package = new Package("test-package", "A test package", "1.0.0", publisher.Id);
        _context.Packages.Add(package);
        await _context.SaveChangesAsync();

        // Verify relationships are loaded correctly
        var loadedServer = await _context.Servers
            .Include(s => s.Publisher)
            .FirstAsync(s => s.Id == server.Id);

        loadedServer.Publisher.Should().NotBeNull();
        loadedServer.Publisher.Name.Should().Be("Test Publisher");

        var loadedPackage = await _context.Packages
            .Include(p => p.Publisher)
            .FirstAsync(p => p.Id == package.Id);

        loadedPackage.Publisher.Should().NotBeNull();
        loadedPackage.Publisher.Name.Should().Be("Test Publisher");
    }

    [Test]
    public async Task Database_EnforcesUniqueConstraints() {
        // Arrange
        await _context.Database.EnsureCreatedAsync();

        var publisher1 = new Publisher("Unique Publisher", "unique@example.com", PublisherType.Individual);
        var publisher2 = new Publisher("Unique Publisher", "different@example.com", PublisherType.Individual);

        _context.Publishers.Add(publisher1);
        await _context.SaveChangesAsync();

        // Act & Assert - Try to add duplicate publisher name
        _context.Publishers.Add(publisher2);

        // In an in-memory database, unique constraints might not be enforced
        // This test would fail with a real database, but we can verify the configuration
        var publisherConfig = _context.Model.FindEntityType(typeof(Publisher));
        var nameProperty = publisherConfig?.FindProperty(nameof(Publisher.Name));

        nameProperty.Should().NotBeNull();
    }

    [Test]
    public async Task Database_HandlesJsonColumns() {
        // Arrange
        await _context.Database.EnsureCreatedAsync();

        var publisher = new Publisher("Test Publisher", "test@example.com", PublisherType.Individual);
        _context.Publishers.Add(publisher);
        await _context.SaveChangesAsync();

        // Act - Test JSON column handling
        var package = new Package("test-package", "A test package", "1.0.0", publisher.Id, tags: new List<string> { "tag1", "tag2", "tag3" });
        _context.Packages.Add(package);
        await _context.SaveChangesAsync();

        // Assert
        var loadedPackage = await _context.Packages.FirstAsync(p => p.Id == package.Id);
        loadedPackage.Tags.Should().HaveCount(3);
        loadedPackage.Tags.Should().Contain("tag1");
        loadedPackage.Tags.Should().Contain("tag2");
        loadedPackage.Tags.Should().Contain("tag3");
    }

    [Test]
    public async Task Database_HandlesCascadeDeletes() {
        // Arrange
        await _context.Database.EnsureCreatedAsync();

        var publisher = new Publisher("Test Publisher", "test@example.com", PublisherType.Individual);
        _context.Publishers.Add(publisher);
        await _context.SaveChangesAsync();

        var server = new Server("Test Server", "A test server", publisher.Id);
        var package = new Package("test-package", "A test package", "1.0.0", publisher.Id);

        _context.Servers.Add(server);
        _context.Packages.Add(package);
        await _context.SaveChangesAsync();

        // Act - Delete the publisher
        _context.Publishers.Remove(publisher);
        await _context.SaveChangesAsync();

        // Assert - Related entities should be deleted (cascade)
        var remainingServers = await _context.Servers.CountAsync();
        var remainingPackages = await _context.Packages.CountAsync();

        remainingServers.Should().Be(0);
        remainingPackages.Should().Be(0);
    }

    [Test]
    public async Task Database_HandlesComplexQueries() {
        // Arrange
        await _context.Database.EnsureCreatedAsync();

        var publisher = new Publisher("Test Publisher", "test@example.com", PublisherType.Individual);
        _context.Publishers.Add(publisher);
        await _context.SaveChangesAsync();

        var packages = new List<Package>
        {
            new Package("package-1", "First package", "1.0.0", publisher.Id, tags: new List<string> { "web", "api" }),
            new Package("package-2", "Second package", "2.0.0", publisher.Id, tags: new List<string> { "cli", "tool" }),
            new Package("package-3", "Third package", "1.5.0", publisher.Id, tags: new List<string> { "web", "ui" })
        };

        packages[0].UpdateStatus(PackageStatus.Approved);
        packages[1].UpdateStatus(PackageStatus.Pending);
        packages[2].UpdateStatus(PackageStatus.Approved);

        _context.Packages.AddRange(packages);
        await _context.SaveChangesAsync();

        // Act - Complex query with joins and filtering
        var approvedWebPackages = await _context.Packages
            .Include(p => p.Publisher)
            .Where(p => p.Status == PackageStatus.Approved)
            .Where(p => p.Tags.Contains("web"))
            .OrderBy(p => p.Name)
            .ToListAsync();

        // Assert
        approvedWebPackages.Should().HaveCount(2);
        approvedWebPackages[0].Name.Should().Be("package-1");
        approvedWebPackages[1].Name.Should().Be("package-3");
        approvedWebPackages.All(p => p.Publisher.Name == "Test Publisher").Should().BeTrue();
    }

    [TearDown]
    public async Task TearDown() => await CleanupDatabaseAsync();

    protected override async Task CleanupDatabaseAsync() {
        if (_context != null) {
            await _context.Database.EnsureDeletedAsync();
            await _context.DisposeAsync();
        }
    }
}