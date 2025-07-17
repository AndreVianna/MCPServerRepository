using Common.UnitTests.TestUtilities;

using Data.Repositories;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Data.UnitTests.Repositories;

[TestCategory(TestCategories.Integration)]
public class PackageRepositoryTests : DatabaseTestBase {
    private McpHubContext _context = null!;
    private IPackageRepository _packageRepository = null!;
    private IPublisherRepository _publisherRepository = null!;

    protected override void ConfigureDatabaseServices() {
        base.ConfigureDatabaseServices();

        Services.AddDbContext<McpHubContext>(options =>
            options.UseInMemoryDatabase(DatabaseName));

        Services.AddScoped<IPackageRepository, PackageRepository>();
        Services.AddScoped<IPublisherRepository, PublisherRepository>();
    }

    [SetUp]
    public async Task SetUp() {
        _context = CreateInMemoryContext<McpHubContext>();
        _packageRepository = GetService<IPackageRepository>();
        _publisherRepository = GetService<IPublisherRepository>();

        await SeedDatabaseAsync();
    }

    protected override async Task SeedDatabaseAsync() {
        var publisher = new Publisher("Test Publisher", "test@example.com", PublisherType.Individual);
        await _publisherRepository.AddAsync(publisher);
        await _context.SaveChangesAsync();
    }

    [Test]
    public async Task GetByNameAsync_WhenPackageExists_ReturnsPackage() {
        // Arrange
        var publisher = await _publisherRepository.FirstOrDefaultAsync(p => p.Name == "Test Publisher");
        var package = new Package("test-package", "A test package", "1.0.0", publisher!.Id);
        await _packageRepository.AddAsync(package);
        await _context.SaveChangesAsync();

        // Act
        var result = await _packageRepository.GetByNameAsync("test-package");

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("test-package");
        result.Description.Should().Be("A test package");
        result.Version.Should().Be("1.0.0");
    }

    [Test]
    public async Task GetByNameAsync_WhenPackageDoesNotExist_ReturnsNull() {
        // Act
        var result = await _packageRepository.GetByNameAsync("non-existent-package");

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task GetByPublisherIdAsync_WhenPackagesExist_ReturnsPackages() {
        // Arrange
        var publisher = await _publisherRepository.FirstOrDefaultAsync(p => p.Name == "Test Publisher");
        var package1 = new Package("package-1", "First package", "1.0.0", publisher!.Id);
        var package2 = new Package("package-2", "Second package", "1.0.0", publisher.Id);

        await _packageRepository.AddAsync(package1);
        await _packageRepository.AddAsync(package2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _packageRepository.GetByPublisherIdAsync(publisher.Id);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(p => p.Name == "package-1");
        result.Should().Contain(p => p.Name == "package-2");
    }

    [Test]
    public async Task GetByStatusAsync_WhenPackagesWithStatusExist_ReturnsFilteredPackages() {
        // Arrange
        var publisher = await _publisherRepository.FirstOrDefaultAsync(p => p.Name == "Test Publisher");
        var pendingPackage = new Package("pending-package", "Pending package", "1.0.0", publisher!.Id);
        var approvedPackage = new Package("approved-package", "Approved package", "1.0.0", publisher.Id);
        approvedPackage.UpdateStatus(PackageStatus.Approved);

        await _packageRepository.AddAsync(pendingPackage);
        await _packageRepository.AddAsync(approvedPackage);
        await _context.SaveChangesAsync();

        // Act
        var pendingResults = await _packageRepository.GetByStatusAsync(PackageStatus.Pending);
        var approvedResults = await _packageRepository.GetByStatusAsync(PackageStatus.Approved);

        // Assert
        pendingResults.Should().HaveCount(1);
        pendingResults.First().Name.Should().Be("pending-package");

        approvedResults.Should().HaveCount(1);
        approvedResults.First().Name.Should().Be("approved-package");
    }

    [Test]
    public async Task SearchAsync_WhenQueryMatchesName_ReturnsMatchingPackages() {
        // Arrange
        var publisher = await _publisherRepository.FirstOrDefaultAsync(p => p.Name == "Test Publisher");
        var package1 = new Package("search-test", "Test package", "1.0.0", publisher!.Id);
        var package2 = new Package("other-package", "Other package", "1.0.0", publisher.Id);

        await _packageRepository.AddAsync(package1);
        await _packageRepository.AddAsync(package2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _packageRepository.SearchAsync("search");

        // Assert
        result.Should().HaveCount(1);
        result.First().Name.Should().Be("search-test");
    }

    [Test]
    public async Task SearchAsync_WithPagination_ReturnsCorrectPage() {
        // Arrange
        var publisher = await _publisherRepository.FirstOrDefaultAsync(p => p.Name == "Test Publisher");

        for (var i = 1; i <= 25; i++) {
            var package = new Package($"package-{i:D2}", $"Package {i}", "1.0.0", publisher!.Id);
            await _packageRepository.AddAsync(package);
        }
        await _context.SaveChangesAsync();

        // Act
        var firstPage = await _packageRepository.SearchAsync("package", page: 0, pageSize: 10);
        var secondPage = await _packageRepository.SearchAsync("package", page: 1, pageSize: 10);

        // Assert
        firstPage.Should().HaveCount(10);
        secondPage.Should().HaveCount(10);

        // Ensure no overlap
        var firstPageNames = firstPage.Select(p => p.Name).ToList();
        var secondPageNames = secondPage.Select(p => p.Name).ToList();
        firstPageNames.Should().NotIntersectWith(secondPageNames);
    }

    [Test]
    public async Task GetRecentlyUpdatedAsync_ReturnsPackagesOrderedByUpdateTime() {
        // Arrange
        var publisher = await _publisherRepository.FirstOrDefaultAsync(p => p.Name == "Test Publisher");
        var package1 = new Package("package-1", "First package", "1.0.0", publisher!.Id);
        var package2 = new Package("package-2", "Second package", "1.0.0", publisher.Id);

        await _packageRepository.AddAsync(package1);
        await _packageRepository.AddAsync(package2);
        await _context.SaveChangesAsync();

        // Update package1 to be more recent
        package1.UpdateStatus(PackageStatus.Approved);
        await _packageRepository.UpdateAsync(package1);
        await _context.SaveChangesAsync();

        // Act
        var result = await _packageRepository.GetRecentlyUpdatedAsync(10);

        // Assert
        result.Should().HaveCount(2);
        result.First().Name.Should().Be("package-1"); // Most recently updated
        result.Last().Name.Should().Be("package-2");
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