using MCPHub.Domain.Contracts.Requests;
using MCPHub.Domain.Contracts.Services;
using MCPHub.Domain.Entities;
using MCPHub.Domain.Repositories;
using MCPHub.Domain.Services;
using MCPHub.Domain.TestUtilities;

namespace MCPHub.Domain.UnitTests.Services;

/// <summary>
/// Unit tests for PackageService following AAA pattern
/// </summary>
public class PackageServiceTests {
    private readonly IUnitOfWork _mockUnitOfWork;
    private readonly IPackageRepository _mockPackageRepository;
    private readonly IPackageService _packageService;

    public PackageServiceTests() {
        _mockUnitOfWork = Substitute.For<IUnitOfWork>();
        _mockPackageRepository = Substitute.For<IPackageRepository>();
        _mockUnitOfWork.Packages.Returns(_mockPackageRepository);
        _packageService = new PackageService(_mockUnitOfWork);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Constructor_WithNullUnitOfWork_ThrowsArgumentNullException() {
        // Arrange, Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => new PackageService(null!));
        exception.ParamName.Should().Be("unitOfWork");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GetAllPackagesAsync_WhenCalled_ReturnsAllPackages() {
        // Arrange
        var packages = new List<Package> {
            DomainTestData.CreateValidPackage(),
            DomainTestData.CreateValidPackage(),
            DomainTestData.CreateValidPackage(),
                                         };
        _mockPackageRepository.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(packages.AsReadOnly());

        // Act
        var result = await _packageService.GetAllPackagesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().BeEquivalentTo(packages);
        await _mockPackageRepository.Received(1).GetAllAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GetAllPackagesAsync_WithCancellationToken_PassesCancellationToken() {
        // Arrange
        var cancellationToken = new CancellationToken();
        var packages = new List<Package>().AsReadOnly();
        _mockPackageRepository.GetAllAsync(cancellationToken).Returns(packages);

        // Act
        await _packageService.GetAllPackagesAsync(cancellationToken);

        // Assert
        await _mockPackageRepository.Received(1).GetAllAsync(cancellationToken);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GetPackageByIdAsync_WithValidId_ReturnsPackage() {
        // Arrange
        var packageId = Guid.NewGuid();
        var package = DomainTestData.CreateValidPackage();
        _mockPackageRepository.GetByIdAsync(packageId, Arg.Any<CancellationToken>())
            .Returns(package);

        // Act
        var result = await _packageService.GetPackageByIdAsync(packageId);

        // Assert
        result?.Should().NotBeNull();
        result?.Should().Be(package);
        await _mockPackageRepository.Received(1).GetByIdAsync(packageId, Arg.Any<CancellationToken>());
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GetPackageByIdAsync_WithNonExistentId_ReturnsNull() {
        // Arrange
        var packageId = Guid.NewGuid();
        _mockPackageRepository.GetByIdAsync(packageId, Arg.Any<CancellationToken>())
            .Returns((Package?)null);

        // Act
        var result = await _packageService.GetPackageByIdAsync(packageId);

        // Assert
        result?.Should().BeNull();
        await _mockPackageRepository.Received(1).GetByIdAsync(packageId, Arg.Any<CancellationToken>());
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GetPackageByNameAsync_WithValidName_ReturnsPackage() {
        // Arrange
        var packageName = "test-package";
        var package = DomainTestData.CreateValidPackage();
        _mockPackageRepository.GetByNameAsync(packageName, Arg.Any<CancellationToken>())
            .Returns(package);

        // Act
        var result = await _packageService.GetPackageByNameAsync(packageName);

        // Assert
        result?.Should().NotBeNull();
        result?.Should().Be(package);
        await _mockPackageRepository.Received(1).GetByNameAsync(packageName, Arg.Any<CancellationToken>());
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GetPackageByNameAsync_WithNonExistentName_ReturnsNull() {
        // Arrange
        var packageName = "non-existent-package";
        _mockPackageRepository.GetByNameAsync(packageName, Arg.Any<CancellationToken>())
            .Returns((Package?)null);

        // Act
        var result = await _packageService.GetPackageByNameAsync(packageName);

        // Assert
        result?.Should().BeNull();
        await _mockPackageRepository.Received(1).GetByNameAsync(packageName, Arg.Any<CancellationToken>());
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GetPackagesByPublisherAsync_WithValidPublisherId_ReturnsPackages() {
        // Arrange
        var publisherId = Guid.NewGuid();
        var packages = new List<Package> {
            DomainTestData.CreateValidPackage(),
            DomainTestData.CreateValidPackage(),
                                         };
        _mockPackageRepository.GetByPublisherIdAsync(publisherId, Arg.Any<CancellationToken>())
            .Returns(packages);

        // Act
        var result = await _packageService.GetPackagesByPublisherAsync(publisherId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(packages);
        await _mockPackageRepository.Received(1).GetByPublisherIdAsync(publisherId, Arg.Any<CancellationToken>());
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task SearchPackagesAsync_WithQuery_ReturnsMatchingPackages() {
        // Arrange
        var query = "search-term";
        var pageSize = 10;
        var pageIndex = 0;
        var packages = new List<Package> { DomainTestData.CreateValidPackage() };
        _mockPackageRepository.SearchAsync(query, pageIndex, pageSize, Arg.Any<CancellationToken>())
            .Returns(packages);

        // Act
        var result = await _packageService.SearchPackagesAsync(query, pageSize, pageIndex);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.Should().BeEquivalentTo(packages);
        await _mockPackageRepository.Received(1).SearchAsync(query, pageIndex, pageSize, Arg.Any<CancellationToken>());
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task CreatePackageAsync_WithValidPackage_CreatesPackageAndReturnsResult() {
        // Arrange
        var package = DomainTestData.CreateValidPackage();
        var createdPackage = DomainTestData.CreateValidPackage();
        _mockPackageRepository.AddAsync(package, Arg.Any<CancellationToken>())
            .Returns(createdPackage);

        // Act
        var result = await _packageService.CreatePackageAsync(package);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(createdPackage);
        await _mockPackageRepository.Received(1).AddAsync(package, Arg.Any<CancellationToken>());
        await _mockUnitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task CreatePackageAsync_WithValidPackage_AddsAuditTrailEntry() {
        // Arrange
        var package = DomainTestData.CreateValidPackage();
        var initialAuditCount = package.AuditTrail.Count;
        _mockPackageRepository.AddAsync(package, Arg.Any<CancellationToken>())
            .Returns(package);

        // Act
        await _packageService.CreatePackageAsync(package);

        // Assert
        // The package should have additional audit trail entry for creation action
        package.AuditTrail.Should().HaveCountGreaterThan(initialAuditCount);
        var lastAuditEntry = package.AuditTrail.Last();
        lastAuditEntry.Action.Should().Contain("Created");
        lastAuditEntry.DateTime.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task UpdatePackageAsync_WithValidPackage_UpdatesPackageAndReturnsResult() {
        // Arrange
        var package = DomainTestData.CreateValidPackage();
        var updatedPackage = DomainTestData.CreateValidPackage();
        _mockPackageRepository.UpdateAsync(package, Arg.Any<CancellationToken>())
            .Returns(updatedPackage);

        // Act
        var result = await _packageService.UpdatePackageAsync(package);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(updatedPackage);
        await _mockPackageRepository.Received(1).UpdateAsync(package, Arg.Any<CancellationToken>());
        await _mockUnitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task UpdatePackageAsync_WithValidPackage_AddsAuditTrailEntry() {
        // Arrange
        var package = DomainTestData.CreateValidPackage();
        var initialAuditCount = package.AuditTrail.Count;
        _mockPackageRepository.UpdateAsync(package, Arg.Any<CancellationToken>())
            .Returns(package);

        // Act
        await _packageService.UpdatePackageAsync(package);

        // Assert
        // The package should have additional audit trail entry for update action
        package.AuditTrail.Should().HaveCountGreaterThan(initialAuditCount);
        var lastAuditEntry = package.AuditTrail.Last();
        lastAuditEntry.Action.Should().Contain("Updated");
        lastAuditEntry.DateTime.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task DeletePackageAsync_WithExistingPackage_DeletesPackageAndReturnsTrue() {
        // Arrange
        var packageId = Guid.NewGuid();
        var package = DomainTestData.CreateValidPackage();
        _mockPackageRepository.GetByIdAsync(packageId, Arg.Any<CancellationToken>())
            .Returns(package);

        // Act
        var result = await _packageService.DeletePackageAsync(packageId);

        // Assert
        result.Should().BeTrue();
        await _mockPackageRepository.Received(1).GetByIdAsync(packageId, Arg.Any<CancellationToken>());
        await _mockPackageRepository.Received(1).DeleteAsync(package, Arg.Any<CancellationToken>());
        await _mockUnitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task DeletePackageAsync_WithNonExistentPackage_ReturnsFalse() {
        // Arrange
        var packageId = Guid.NewGuid();
        _mockPackageRepository.GetByIdAsync(packageId, Arg.Any<CancellationToken>())
            .Returns((Package?)null);

        // Act
        var result = await _packageService.DeletePackageAsync(packageId);

        // Assert
        result.Should().BeFalse();
        await _mockPackageRepository.Received(1).GetByIdAsync(packageId, Arg.Any<CancellationToken>());
        await _mockPackageRepository.DidNotReceive().DeleteAsync(Arg.Any<Package>(), Arg.Any<CancellationToken>());
        await _mockUnitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task DeletePackageAsync_WithExistingPackage_AddsAuditTrailEntry() {
        // Arrange
        var packageId = Guid.NewGuid();
        var package = DomainTestData.CreateValidPackage();
        var initialAuditCount = package.AuditTrail.Count;
        _mockPackageRepository.GetByIdAsync(packageId, Arg.Any<CancellationToken>())
            .Returns(package);

        // Act
        await _packageService.DeletePackageAsync(packageId);

        // Assert
        // The package should have additional audit trail entry for deletion action
        package.AuditTrail.Should().HaveCountGreaterThan(initialAuditCount);
        var lastAuditEntry = package.AuditTrail.Last();
        lastAuditEntry.Action.Should().Contain("Deleted");
        lastAuditEntry.DateTime.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null!)]
    public async Task GetPackageByNameAsync_WithInvalidName_ThrowsArgumentException(string? invalidName) {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            _packageService.GetPackageByNameAsync(invalidName!));
        exception.Message.Should().Contain("name");
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null!)]
    public async Task SearchPackagesAsync_WithInvalidQuery_ThrowsArgumentException(string? invalidQuery) {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            _packageService.SearchPackagesAsync(invalidQuery!));
        exception.Message.Should().Contain("query");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task CreatePackageAsync_WithNullPackage_ThrowsArgumentNullException() {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _packageService.CreatePackageAsync(null!));
        exception.ParamName.Should().Be("package");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task UpdatePackageAsync_WithNullPackage_ThrowsArgumentNullException() {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _packageService.UpdatePackageAsync(null!));
        exception.ParamName.Should().Be("package");
    }

    #region Enhanced Search Tests

    [Fact]
    [Trait("Category", "Unit")]
    public async Task SearchPackagesAsync_WithValidSearchRequest_ReturnsSearchResult() {
        // Arrange
        var searchRequest = new SearchRequest {
            Query = "test package",
            Page = 1,
            PageSize = 10,
        };
        var packages = new List<Package> { DomainTestData.CreateValidPackage() };
        var searchResult = new SearchResult<Package> {
            Items = packages,
            TotalCount = 1,
            Page = 1,
            PageSize = 10,
            Query = "test package",
            SearchTimeMs = 50,
        };
        _mockPackageRepository.SearchAsync(searchRequest, Arg.Any<CancellationToken>())
            .Returns(searchResult);

        // Act
        var result = await _packageService.SearchPackagesAsync(searchRequest);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(searchResult);
        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.Query.Should().Be("test package");
        await _mockPackageRepository.Received(1).SearchAsync(searchRequest, Arg.Any<CancellationToken>());
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task SearchPackagesAsync_WithValidSearchRequestAndCancellationToken_PassesCancellationToken() {
        // Arrange
        var cancellationToken = new CancellationToken();
        var searchRequest = new SearchRequest { Query = "test" };
        var searchResult = new SearchResult<Package> {
            Items = [],
            TotalCount = 0,
            Page = 1,
            PageSize = 20,
            Query = "test",
            SearchTimeMs = 25,
        };
        _mockPackageRepository.SearchAsync(searchRequest, cancellationToken).Returns(searchResult);

        // Act
        await _packageService.SearchPackagesAsync(searchRequest, cancellationToken);

        // Assert
        await _mockPackageRepository.Received(1).SearchAsync(searchRequest, cancellationToken);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task SearchPackagesAsync_WithNullSearchRequest_ThrowsArgumentNullException() {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _packageService.SearchPackagesAsync((SearchRequest)null!));
        exception.ParamName.Should().Be("request");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task SearchPackagesAsync_WithInvalidSearchRequest_ThrowsArgumentException() {
        // Arrange
        var invalidRequest = new SearchRequest {
            Query = "", // Invalid: empty query
            Page = 1,
            PageSize = 20,
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            _packageService.SearchPackagesAsync(invalidRequest));
        exception.ParamName.Should().Be("request");
        exception.Message.Should().Contain("Query is required");
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task SearchPackagesAsync_WithInvalidPage_ThrowsArgumentException(int invalidPage) {
        // Arrange
        var invalidRequest = new SearchRequest {
            Query = "test",
            Page = invalidPage,
            PageSize = 20,
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            _packageService.SearchPackagesAsync(invalidRequest));
        exception.ParamName.Should().Be("request");
        exception.Message.Should().Contain("Page must be 1 or greater");
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData(0)]
    [InlineData(101)]
    public async Task SearchPackagesAsync_WithInvalidPageSize_ThrowsArgumentException(int invalidPageSize) {
        // Arrange
        var invalidRequest = new SearchRequest {
            Query = "test",
            Page = 1,
            PageSize = invalidPageSize,
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            _packageService.SearchPackagesAsync(invalidRequest));
        exception.ParamName.Should().Be("request");
        exception.Message.Should().Contain("Page size must be between 1 and 100");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task SearchPackagesAsync_WithCategoriesAndTrustTier_CallsRepositoryCorrectly() {
        // Arrange
        var searchRequest = new SearchRequest {
            Query = "api package",
            Categories = ["api", "web"],
            MinimumTrustTier = TrustTier.CommunityTrusted,
            Page = 2,
            PageSize = 15,
            SortBy = "name",
            SortDirection = SortDirection.Descending,
        };
        var packages = new List<Package> {
            DomainTestData.CreateValidPackage(),
            DomainTestData.CreateValidPackage(),
                                         };
        var searchResult = new SearchResult<Package> {
            Items = packages,
            TotalCount = 25,
            Page = 2,
            PageSize = 15,
            Query = "api package",
            SearchTimeMs = 75,
        };
        _mockPackageRepository.SearchAsync(searchRequest, Arg.Any<CancellationToken>())
            .Returns(searchResult);

        // Act
        var result = await _packageService.SearchPackagesAsync(searchRequest);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(25);
        result.Page.Should().Be(2);
        result.PageSize.Should().Be(15);
        result.TotalPages.Should().Be(2); // 25 total / 15 per page = 2 pages
        await _mockPackageRepository.Received(1).SearchAsync(searchRequest, Arg.Any<CancellationToken>());
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task SearchPackagesAsync_WithSearchResult_ReturnsCorrectTotalPages() {
        // Arrange
        var searchRequest = new SearchRequest {
            Query = "test",
            Page = 1,
            PageSize = 10,
        };
        var searchResult = new SearchResult<Package> {
            Items = [],
            TotalCount = 55, // Should result in 6 pages (55/10 = 5.5 -> 6)
            Page = 1,
            PageSize = 10,
            Query = "test",
            SearchTimeMs = 30,
        };
        _mockPackageRepository.SearchAsync(searchRequest, Arg.Any<CancellationToken>())
            .Returns(searchResult);

        // Act
        var result = await _packageService.SearchPackagesAsync(searchRequest);

        // Assert
        result.TotalPages.Should().Be(6);
    }

    #endregion
}