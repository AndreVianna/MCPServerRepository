using Common.Services;
using Common.UnitTests.TestUtilities;
using Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace Data.UnitTests.Repositories;

[TestCategory(TestCategories.Integration)]
public class CachedPackageRepositoryTests : DatabaseTestBase
{
    private McpHubContext _context = null!;
    private IPackageRepository _baseRepository = null!;
    private ICacheService _cacheService = null!;
    private CachedPackageRepository _cachedRepository = null!;
    private IPublisherRepository _publisherRepository = null!;

    protected override void ConfigureDatabaseServices()
    {
        base.ConfigureDatabaseServices();
        
        Services.AddDbContext<McpHubContext>(options =>
            options.UseInMemoryDatabase(DatabaseName));
        
        Services.AddScoped<IPackageRepository, PackageRepository>();
        Services.AddScoped<IPublisherRepository, PublisherRepository>();
        Services.AddSingleton(_cacheService = Substitute.For<ICacheService>());
    }

    [SetUp]
    public async Task SetUp()
    {
        _context = CreateInMemoryContext<McpHubContext>();
        _baseRepository = GetService<IPackageRepository>();
        _publisherRepository = GetService<IPublisherRepository>();
        _cacheService = GetService<ICacheService>();
        _cachedRepository = new CachedPackageRepository(_baseRepository, _cacheService);
        
        await SeedDatabaseAsync();
    }

    protected override async Task SeedDatabaseAsync()
    {
        var publisher = new Publisher("Test Publisher", "test@example.com", PublisherType.Individual);
        await _publisherRepository.AddAsync(publisher);
        await _context.SaveChangesAsync();
    }

    [Test]
    public async Task GetByNameAsync_WhenCacheHit_ReturnsCachedValue()
    {
        // Arrange
        var cachedPackage = new Package("cached-package", "Cached package", "1.0.0", Guid.NewGuid());
        _cacheService.GetAsync<Package>("package:name:test-package", Arg.Any<CancellationToken>())
            .Returns(cachedPackage);

        // Act
        var result = await _cachedRepository.GetByNameAsync("test-package");

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("cached-package");
        
        // Verify cache was checked
        await _cacheService.Received(1).GetAsync<Package>("package:name:test-package", Arg.Any<CancellationToken>());
        
        // Verify database was not accessed (no SetAsync call for setting cache)
        await _cacheService.DidNotReceive().SetAsync(Arg.Any<string>(), Arg.Any<Package>(), Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task GetByNameAsync_WhenCacheMiss_FetchesFromDatabaseAndCaches()
    {
        // Arrange
        var publisher = await _publisherRepository.FirstOrDefaultAsync(p => p.Name == "Test Publisher");
        var package = new Package("test-package", "Test package", "1.0.0", publisher!.Id);
        await _baseRepository.AddAsync(package);
        await _context.SaveChangesAsync();

        _cacheService.GetAsync<Package>("package:name:test-package", Arg.Any<CancellationToken>())
            .Returns((Package?)null);

        // Act
        var result = await _cachedRepository.GetByNameAsync("test-package");

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("test-package");
        
        // Verify cache was checked
        await _cacheService.Received(1).GetAsync<Package>("package:name:test-package", Arg.Any<CancellationToken>());
        
        // Verify result was cached
        await _cacheService.Received(1).SetAsync("package:name:test-package", Arg.Any<Package>(), TimeSpan.FromMinutes(30), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task GetByPublisherIdAsync_WhenCacheHit_ReturnsCachedValue()
    {
        // Arrange
        var publisherId = Guid.NewGuid();
        var cachedPackages = new List<Package>
        {
            new Package("package-1", "First package", "1.0.0", publisherId),
            new Package("package-2", "Second package", "1.0.0", publisherId)
        };
        
        _cacheService.GetAsync<List<Package>>($"package:publisher:{publisherId}", Arg.Any<CancellationToken>())
            .Returns(cachedPackages);

        // Act
        var result = await _cachedRepository.GetByPublisherIdAsync(publisherId);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(p => p.Name == "package-1");
        result.Should().Contain(p => p.Name == "package-2");
        
        // Verify cache was checked
        await _cacheService.Received(1).GetAsync<List<Package>>($"package:publisher:{publisherId}", Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task AddAsync_InvalidatesCacheAfterOperation()
    {
        // Arrange
        var publisher = await _publisherRepository.FirstOrDefaultAsync(p => p.Name == "Test Publisher");
        var package = new Package("new-package", "New package", "1.0.0", publisher!.Id);

        // Act
        await _cachedRepository.AddAsync(package);
        await _context.SaveChangesAsync();

        // Assert
        await _cacheService.Received(1).RemovePatternAsync("package:*", Arg.Any<CancellationToken>());
        await _cacheService.Received(1).RemovePatternAsync("packageversion:*", Arg.Any<CancellationToken>());
        await _cacheService.Received(1).RemovePatternAsync("publisher:*", Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task UpdateAsync_InvalidatesCacheAfterOperation()
    {
        // Arrange
        var publisher = await _publisherRepository.FirstOrDefaultAsync(p => p.Name == "Test Publisher");
        var package = new Package("update-package", "Package to update", "1.0.0", publisher!.Id);
        await _baseRepository.AddAsync(package);
        await _context.SaveChangesAsync();

        // Act
        package.UpdateStatus(PackageStatus.Approved);
        await _cachedRepository.UpdateAsync(package);
        await _context.SaveChangesAsync();

        // Assert
        await _cacheService.Received(1).RemovePatternAsync("package:*", Arg.Any<CancellationToken>());
        await _cacheService.Received(1).RemovePatternAsync("packageversion:*", Arg.Any<CancellationToken>());
        await _cacheService.Received(1).RemovePatternAsync("publisher:*", Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task DeleteAsync_InvalidatesCacheAfterOperation()
    {
        // Arrange
        var publisher = await _publisherRepository.FirstOrDefaultAsync(p => p.Name == "Test Publisher");
        var package = new Package("delete-package", "Package to delete", "1.0.0", publisher!.Id);
        await _baseRepository.AddAsync(package);
        await _context.SaveChangesAsync();

        // Act
        await _cachedRepository.DeleteAsync(package);
        await _context.SaveChangesAsync();

        // Assert
        await _cacheService.Received(1).RemovePatternAsync("package:*", Arg.Any<CancellationToken>());
        await _cacheService.Received(1).RemovePatternAsync("packageversion:*", Arg.Any<CancellationToken>());
        await _cacheService.Received(1).RemovePatternAsync("publisher:*", Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task SearchAsync_CachesSearchResults()
    {
        // Arrange
        var publisher = await _publisherRepository.FirstOrDefaultAsync(p => p.Name == "Test Publisher");
        var package = new Package("search-package", "Searchable package", "1.0.0", publisher!.Id);
        await _baseRepository.AddAsync(package);
        await _context.SaveChangesAsync();

        _cacheService.GetAsync<List<Package>>("package:search:test:0:20", Arg.Any<CancellationToken>())
            .Returns((List<Package>?)null);

        // Act
        var result = await _cachedRepository.SearchAsync("test", page: 0, pageSize: 20);

        // Assert
        result.Should().HaveCount(1);
        result.First().Name.Should().Be("search-package");
        
        // Verify cache was checked and result was cached
        await _cacheService.Received(1).GetAsync<List<Package>>("package:search:test:0:20", Arg.Any<CancellationToken>());
        await _cacheService.Received(1).SetAsync("package:search:test:0:20", Arg.Any<List<Package>>(), TimeSpan.FromMinutes(10), Arg.Any<CancellationToken>());
    }

    [TearDown]
    public async Task TearDown()
    {
        await CleanupDatabaseAsync();
    }

    protected override async Task CleanupDatabaseAsync()
    {
        if (_context != null)
        {
            await _context.Database.EnsureDeletedAsync();
            await _context.DisposeAsync();
        }
    }
}