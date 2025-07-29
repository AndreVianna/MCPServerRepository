using MCPHub.Domain.ValueObjects;

namespace MCPHub.Domain.UnitTests.ValueObjects;

/// <summary>
/// Unit tests for ValidationSummary value object
/// </summary>
public class ValidationSummaryTests {
    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "ValidationSummary")]
    public void Constructor_WithValidParameters_ShouldCreateInstance() {
        // Arrange
        var errors = new[] { "Error 1", "Error 2" };
        var warnings = new[] { "Warning 1" };
        var context = "test-context";
        var validationTime = 100L;

        // Act
        var summary = new ValidationSummary(false, errors, warnings, context, validationTime);

        // Assert
        summary.IsValid.Should().BeFalse();
        summary.Errors.Should().HaveCount(2);
        summary.Warnings.Should().HaveCount(1);
        summary.Context.Should().Be(context);
        summary.ValidationTimeMs.Should().Be(validationTime);
        summary.Errors.Should().BeEquivalentTo(errors);
        summary.Warnings.Should().BeEquivalentTo(warnings);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "ValidationSummary")]
    public void Success_WithDefaultParameters_ShouldCreateValidSummary() {
        // Act
        var summary = ValidationSummary.Success();

        // Assert
        summary.IsValid.Should().BeTrue();
        summary.Errors.Should().BeEmpty();
        summary.Warnings.Should().BeEmpty();
        summary.Context.Should().Be(string.Empty);
        summary.ValidationTimeMs.Should().Be(0L);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "ValidationSummary")]
    public void Success_WithParameters_ShouldCreateValidSummary() {
        // Arrange
        var context = "manifest";
        var validationTime = 50L;
        var warnings = new[] { "Minor issue" };

        // Act
        var summary = ValidationSummary.Success(context, validationTime, warnings);

        // Assert
        summary.IsValid.Should().BeTrue();
        summary.Errors.Should().BeEmpty();
        summary.Warnings.Should().HaveCount(1);
        summary.Context.Should().Be(context);
        summary.ValidationTimeMs.Should().Be(validationTime);
        summary.Warnings.First().Should().Be("Minor issue");
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "ValidationSummary")]
    public void Failure_WithMultipleErrors_ShouldCreateInvalidSummary() {
        // Arrange
        var errors = new[] { "Error 1", "Error 2", "Error 3" };
        var context = "security";
        var validationTime = 75L;

        // Act
        var summary = ValidationSummary.Failure(errors, context, validationTime);

        // Assert
        Assert.False(summary.IsValid);
        Assert.Equal(3, summary.Errors.Count);
        Assert.Equal(0, summary.Warnings.Count);
        Assert.Equal(context, summary.Context);
        Assert.Equal(validationTime, summary.ValidationTimeMs);
        Assert.Equal(errors.ToList(), summary.Errors.ToList());
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "ValidationSummary")]
    public void Failure_WithSingleError_ShouldCreateInvalidSummary() {
        // Arrange
        var error = "Critical validation error";
        var context = "manifest";

        // Act
        var summary = ValidationSummary.Failure(error, context);

        // Assert
        Assert.False(summary.IsValid);
        Assert.Equal(1, summary.Errors.Count);
        Assert.Equal(error, summary.Errors.First());
        Assert.Equal(context, summary.Context);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "ValidationSummary")]
    public void Failure_WithErrorsAndWarnings_ShouldCreateInvalidSummary() {
        // Arrange
        var errors = new[] { "Error 1" };
        var warnings = new[] { "Warning 1", "Warning 2" };
        var context = "dependencies";

        // Act
        var summary = ValidationSummary.Failure(errors, context, 0, warnings);

        // Assert
        Assert.False(summary.IsValid);
        Assert.Equal(1, summary.Errors.Count);
        Assert.Equal(2, summary.Warnings.Count);
        Assert.Equal(context, summary.Context);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "ValidationSummary")]
    public void Combine_WithNoSummaries_ShouldReturnSuccess() {
        // Act
        var combined = ValidationSummary.Combine();

        // Assert
        Assert.True(combined.IsValid);
        Assert.Equal(0, combined.Errors.Count);
        Assert.Equal(0, combined.Warnings.Count);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "ValidationSummary")]
    public void Combine_WithAllValidSummaries_ShouldReturnValid() {
        // Arrange
        var summary1 = ValidationSummary.Success("manifest", 10, ["Warning 1"]);
        var summary2 = ValidationSummary.Success("security", 20, ["Warning 2"]);
        var summary3 = ValidationSummary.Success("dependencies", 30);

        // Act
        var combined = ValidationSummary.Combine(summary1, summary2, summary3);

        // Assert
        Assert.True(combined.IsValid);
        Assert.Equal(0, combined.Errors.Count);
        Assert.Equal(2, combined.Warnings.Count);
        Assert.Equal(60L, combined.ValidationTimeMs);
        Assert.Equal("manifest, security, dependencies", combined.Context);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "ValidationSummary")]
    public void Combine_WithSomeInvalidSummaries_ShouldReturnInvalid() {
        // Arrange
        var summary1 = ValidationSummary.Success("manifest", 10, ["Warning 1"]);
        var summary2 = ValidationSummary.Failure(["Error 1", "Error 2"], "security", 20, ["Warning 2"]);
        var summary3 = ValidationSummary.Failure("Error 3", "dependencies", 30);

        // Act
        var combined = ValidationSummary.Combine(summary1, summary2, summary3);

        // Assert
        Assert.False(combined.IsValid);
        Assert.Equal(3, combined.Errors.Count);
        Assert.Equal(2, combined.Warnings.Count);
        Assert.Equal(60L, combined.ValidationTimeMs);
        Assert.Equal("manifest, security, dependencies", combined.Context);
        Assert.True(combined.Errors.Contains("Error 1"));
        Assert.True(combined.Errors.Contains("Error 2"));
        Assert.True(combined.Errors.Contains("Error 3"));
        Assert.True(combined.Warnings.Contains("Warning 1"));
        Assert.True(combined.Warnings.Contains("Warning 2"));
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "ValidationSummary")]
    public void Combine_WithEmptyContextSummaries_ShouldIgnoreEmptyContexts() {
        // Arrange
        var summary1 = ValidationSummary.Success("manifest", 10);
        var summary2 = ValidationSummary.Success("", 20); // Empty context
        var summary3 = ValidationSummary.Success("security", 30);

        // Act
        var combined = ValidationSummary.Combine(summary1, summary2, summary3);

        // Assert
        Assert.True(combined.IsValid);
        Assert.Equal("manifest, security", combined.Context);
        Assert.Equal(60L, combined.ValidationTimeMs);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "ValidationSummary")]
    public void Constructor_WithNullCollections_ShouldUseEmptyCollections() {
        // Act
        var summary = new ValidationSummary(true, null, null, "test", 0);

        // Assert
        Assert.True(summary.IsValid);
        Assert.Equal(0, summary.Errors.Count);
        Assert.Equal(0, summary.Warnings.Count);
        Assert.Equal("test", summary.Context);
    }
}