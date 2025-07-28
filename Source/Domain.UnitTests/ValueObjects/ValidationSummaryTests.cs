using MCPHub.Domain.ValueObjects;

namespace MCPHub.Domain.UnitTests.ValueObjects;

/// <summary>
/// Unit tests for ValidationSummary value object
/// </summary>
[TestClass]
public class ValidationSummaryTests
{
    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("ValidationSummary")]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Arrange
        var errors = new[] { "Error 1", "Error 2" };
        var warnings = new[] { "Warning 1" };
        var context = "test-context";
        var validationTime = 100L;

        // Act
        var summary = new ValidationSummary(false, errors, warnings, context, validationTime);

        // Assert
        Assert.IsFalse(summary.IsValid);
        Assert.AreEqual(2, summary.Errors.Count);
        Assert.AreEqual(1, summary.Warnings.Count);
        Assert.AreEqual(context, summary.Context);
        Assert.AreEqual(validationTime, summary.ValidationTimeMs);
        CollectionAssert.AreEqual(errors.ToList(), summary.Errors.ToList());
        CollectionAssert.AreEqual(warnings.ToList(), summary.Warnings.ToList());
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("ValidationSummary")]
    public void Success_WithDefaultParameters_ShouldCreateValidSummary()
    {
        // Act
        var summary = ValidationSummary.Success();

        // Assert
        Assert.IsTrue(summary.IsValid);
        Assert.AreEqual(0, summary.Errors.Count);
        Assert.AreEqual(0, summary.Warnings.Count);
        Assert.AreEqual(string.Empty, summary.Context);
        Assert.AreEqual(0L, summary.ValidationTimeMs);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("ValidationSummary")]
    public void Success_WithParameters_ShouldCreateValidSummary()
    {
        // Arrange
        var context = "manifest";
        var validationTime = 50L;
        var warnings = new[] { "Minor issue" };

        // Act
        var summary = ValidationSummary.Success(context, validationTime, warnings);

        // Assert
        Assert.IsTrue(summary.IsValid);
        Assert.AreEqual(0, summary.Errors.Count);
        Assert.AreEqual(1, summary.Warnings.Count);
        Assert.AreEqual(context, summary.Context);
        Assert.AreEqual(validationTime, summary.ValidationTimeMs);
        Assert.AreEqual("Minor issue", summary.Warnings.First());
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("ValidationSummary")]
    public void Failure_WithMultipleErrors_ShouldCreateInvalidSummary()
    {
        // Arrange
        var errors = new[] { "Error 1", "Error 2", "Error 3" };
        var context = "security";
        var validationTime = 75L;

        // Act
        var summary = ValidationSummary.Failure(errors, context, validationTime);

        // Assert
        Assert.IsFalse(summary.IsValid);
        Assert.AreEqual(3, summary.Errors.Count);
        Assert.AreEqual(0, summary.Warnings.Count);
        Assert.AreEqual(context, summary.Context);
        Assert.AreEqual(validationTime, summary.ValidationTimeMs);
        CollectionAssert.AreEqual(errors.ToList(), summary.Errors.ToList());
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("ValidationSummary")]
    public void Failure_WithSingleError_ShouldCreateInvalidSummary()
    {
        // Arrange
        var error = "Critical validation error";
        var context = "manifest";

        // Act
        var summary = ValidationSummary.Failure(error, context);

        // Assert
        Assert.IsFalse(summary.IsValid);
        Assert.AreEqual(1, summary.Errors.Count);
        Assert.AreEqual(error, summary.Errors.First());
        Assert.AreEqual(context, summary.Context);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("ValidationSummary")]
    public void Failure_WithErrorsAndWarnings_ShouldCreateInvalidSummary()
    {
        // Arrange
        var errors = new[] { "Error 1" };
        var warnings = new[] { "Warning 1", "Warning 2" };
        var context = "dependencies";

        // Act
        var summary = ValidationSummary.Failure(errors, context, 0, warnings);

        // Assert
        Assert.IsFalse(summary.IsValid);
        Assert.AreEqual(1, summary.Errors.Count);
        Assert.AreEqual(2, summary.Warnings.Count);
        Assert.AreEqual(context, summary.Context);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("ValidationSummary")]
    public void Combine_WithNoSummaries_ShouldReturnSuccess()
    {
        // Act
        var combined = ValidationSummary.Combine();

        // Assert
        Assert.IsTrue(combined.IsValid);
        Assert.AreEqual(0, combined.Errors.Count);
        Assert.AreEqual(0, combined.Warnings.Count);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("ValidationSummary")]
    public void Combine_WithAllValidSummaries_ShouldReturnValid()
    {
        // Arrange
        var summary1 = ValidationSummary.Success("manifest", 10, ["Warning 1"]);
        var summary2 = ValidationSummary.Success("security", 20, ["Warning 2"]);
        var summary3 = ValidationSummary.Success("dependencies", 30);

        // Act
        var combined = ValidationSummary.Combine(summary1, summary2, summary3);

        // Assert
        Assert.IsTrue(combined.IsValid);
        Assert.AreEqual(0, combined.Errors.Count);
        Assert.AreEqual(2, combined.Warnings.Count);
        Assert.AreEqual(60L, combined.ValidationTimeMs);
        Assert.AreEqual("manifest, security, dependencies", combined.Context);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("ValidationSummary")]
    public void Combine_WithSomeInvalidSummaries_ShouldReturnInvalid()
    {
        // Arrange
        var summary1 = ValidationSummary.Success("manifest", 10, ["Warning 1"]);
        var summary2 = ValidationSummary.Failure(["Error 1", "Error 2"], "security", 20, ["Warning 2"]);
        var summary3 = ValidationSummary.Failure("Error 3", "dependencies", 30);

        // Act
        var combined = ValidationSummary.Combine(summary1, summary2, summary3);

        // Assert
        Assert.IsFalse(combined.IsValid);
        Assert.AreEqual(3, combined.Errors.Count);
        Assert.AreEqual(2, combined.Warnings.Count);
        Assert.AreEqual(60L, combined.ValidationTimeMs);
        Assert.AreEqual("manifest, security, dependencies", combined.Context);
        Assert.IsTrue(combined.Errors.Contains("Error 1"));
        Assert.IsTrue(combined.Errors.Contains("Error 2"));
        Assert.IsTrue(combined.Errors.Contains("Error 3"));
        Assert.IsTrue(combined.Warnings.Contains("Warning 1"));
        Assert.IsTrue(combined.Warnings.Contains("Warning 2"));
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("ValidationSummary")]
    public void Combine_WithEmptyContextSummaries_ShouldIgnoreEmptyContexts()
    {
        // Arrange
        var summary1 = ValidationSummary.Success("manifest", 10);
        var summary2 = ValidationSummary.Success("", 20); // Empty context
        var summary3 = ValidationSummary.Success("security", 30);

        // Act
        var combined = ValidationSummary.Combine(summary1, summary2, summary3);

        // Assert
        Assert.IsTrue(combined.IsValid);
        Assert.AreEqual("manifest, security", combined.Context);
        Assert.AreEqual(60L, combined.ValidationTimeMs);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("ValidationSummary")]
    public void Constructor_WithNullCollections_ShouldUseEmptyCollections()
    {
        // Act
        var summary = new ValidationSummary(true, null, null, "test", 0);

        // Assert
        Assert.IsTrue(summary.IsValid);
        Assert.AreEqual(0, summary.Errors.Count);
        Assert.AreEqual(0, summary.Warnings.Count);
        Assert.AreEqual("test", summary.Context);
    }
}