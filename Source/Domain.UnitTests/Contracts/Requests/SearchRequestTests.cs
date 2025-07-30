using MCPHub.Domain.Contracts.Requests;
using MCPHub.Domain.Entities;

namespace MCPHub.Domain.UnitTests.Contracts.Requests;

/// <summary>
/// Unit tests for SearchRequest validation and properties following AAA pattern
/// </summary>
public class SearchRequestTests {
    [Fact]
    [Trait("Category", "Unit")]
    public void SearchRequest_WithDefaultValues_ShouldHaveCorrectDefaults() {
        // Arrange & Act
        var searchRequest = new SearchRequest();

        // Assert
        searchRequest.Query.Should().BeEmpty();
        searchRequest?.Categories.Should().BeNull();
        searchRequest.MinimumTrustTier.Should().BeNull();
        searchRequest.Page.Should().Be(1);
        searchRequest.PageSize.Should().Be(20);
        searchRequest?.SortBy.Should().BeNull();
        searchRequest.SortDirection.Should().Be(SortDirection.Ascending);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void SearchRequest_WithValidQuery_ShouldBeValid() {
        // Arrange
        var searchRequest = new SearchRequest {
            Query = "test package",
        };

        // Act
        var isValid = searchRequest.IsValid(out var errorMessage);

        // Assert
        isValid.Should().BeTrue();
        errorMessage?.Should().BeNull();
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    [InlineData(null)]
    public void SearchRequest_WithInvalidQuery_ShouldBeInvalid(string? invalidQuery) {
        // Arrange
        var searchRequest = new SearchRequest {
            Query = invalidQuery!,
        };

        // Act
        var isValid = searchRequest.IsValid(out var errorMessage);

        // Assert
        isValid.Should().BeFalse();
        errorMessage?.Should().Be("Query is required");
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public void SearchRequest_WithInvalidPage_ShouldBeInvalid(int invalidPage) {
        // Arrange
        var searchRequest = new SearchRequest {
            Query = "test",
            Page = invalidPage,
        };

        // Act
        var isValid = searchRequest.IsValid(out var errorMessage);

        // Assert
        isValid.Should().BeFalse();
        errorMessage?.Should().Be("Page must be 1 or greater");
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(101)]
    [InlineData(1000)]
    public void SearchRequest_WithInvalidPageSize_ShouldBeInvalid(int invalidPageSize) {
        // Arrange
        var searchRequest = new SearchRequest {
            Query = "test",
            PageSize = invalidPageSize,
        };

        // Act
        var isValid = searchRequest.IsValid(out var errorMessage);

        // Assert
        isValid.Should().BeFalse();
        errorMessage?.Should().Be("Page size must be between 1 and 100");
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(50)]
    [InlineData(100)]
    public void SearchRequest_WithValidPageSize_ShouldBeValid(int validPageSize) {
        // Arrange
        var searchRequest = new SearchRequest {
            Query = "test",
            PageSize = validPageSize,
        };

        // Act
        var isValid = searchRequest.IsValid(out var errorMessage);

        // Assert
        isValid.Should().BeTrue();
        errorMessage?.Should().BeNull();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void SearchRequest_WithAllValidProperties_ShouldBeValid() {
        // Arrange
        var searchRequest = new SearchRequest {
            Query = "test package search",
            Categories = new[] { "cli", "web", "api" },
            MinimumTrustTier = TrustTier.CommunityTrusted,
            Page = 2,
            PageSize = 50,
            SortBy = "name",
            SortDirection = SortDirection.Descending,
        };

        // Act
        var isValid = searchRequest.IsValid(out var errorMessage);

        // Assert
        isValid.Should().BeTrue();
        errorMessage?.Should().BeNull();
        searchRequest.Categories.Should().HaveCount(3);
        searchRequest.Categories.Should().Contain(new[] { "cli", "web", "api" });
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void SearchRequest_WithNullCategories_ShouldBeValid() {
        // Arrange
        var searchRequest = new SearchRequest {
            Query = "test",
            Categories = null,
        };

        // Act
        var isValid = searchRequest.IsValid(out var errorMessage);

        // Assert
        isValid.Should().BeTrue();
        errorMessage?.Should().BeNull();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void SearchRequest_WithEmptyCategories_ShouldBeValid() {
        // Arrange
        var searchRequest = new SearchRequest {
            Query = "test",
            Categories = [],
        };

        // Act
        var isValid = searchRequest.IsValid(out var errorMessage);

        // Assert
        isValid.Should().BeTrue();
        errorMessage?.Should().BeNull();
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData(TrustTier.Unverified)]
    [InlineData(TrustTier.CommunityTrusted)]
    [InlineData(TrustTier.SecurityAudited)]
    [InlineData(TrustTier.Certified)]
    public void SearchRequest_WithValidTrustTiers_ShouldBeValid(TrustTier trustTier) {
        // Arrange
        var searchRequest = new SearchRequest {
            Query = "test",
            MinimumTrustTier = trustTier,
        };

        // Act
        var isValid = searchRequest.IsValid(out var errorMessage);

        // Assert
        isValid.Should().BeTrue();
        errorMessage?.Should().BeNull();
        searchRequest.MinimumTrustTier.Should().Be(trustTier);
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData(SortDirection.Ascending)]
    [InlineData(SortDirection.Descending)]
    public void SearchRequest_WithValidSortDirections_ShouldBeValid(SortDirection sortDirection) {
        // Arrange
        var searchRequest = new SearchRequest {
            Query = "test",
            SortDirection = sortDirection,
        };

        // Act
        var isValid = searchRequest.IsValid(out var errorMessage);

        // Assert
        isValid.Should().BeTrue();
        errorMessage?.Should().BeNull();
        searchRequest.SortDirection.Should().Be(sortDirection);
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData("name")]
    [InlineData("downloads")]
    [InlineData("rating")]
    [InlineData("created")]
    [InlineData("NAME")]
    [InlineData(null)]
    [InlineData("")]
    public void SearchRequest_WithVariousSortByValues_ShouldBeValid(string? sortBy) {
        // Arrange
        var searchRequest = new SearchRequest {
            Query = "test",
            SortBy = sortBy,
        };

        // Act
        var isValid = searchRequest.IsValid(out var errorMessage);

        // Assert
        isValid.Should().BeTrue();
        errorMessage?.Should().BeNull();
    }
}