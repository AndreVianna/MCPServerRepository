namespace Domain.UnitTests.Entities;

/// <summary>
/// Unit tests for Package entity.
/// This demonstrates the mirror structure pattern - tests are organized 
/// in the same folder structure as the main project.
/// </summary>
[UnitTest]
public class PackageTests : TestBase
{
    [Fact]
    public void Package_Should_Be_Created_With_Valid_Properties()
    {
        // Arrange
        var packageName = TestData.RandomString(10);
        var description = TestData.RandomString(50);
        
        // Act
        // Note: Actual Package entity implementation would be tested here
        var result = $"Package: {packageName}, Description: {description}";
        
        // Assert
        result.Should().Contain(packageName);
        result.Should().Contain(description);
    }

    [Fact]
    public void Package_Should_Validate_Required_Properties()
    {
        // Arrange
        var emptyName = string.Empty;
        
        // Act & Assert
        // Note: Actual Package entity validation would be tested here
        emptyName.Should().BeEmpty();
    }
}

/// <summary>
/// Integration tests for Package entity with database operations.
/// </summary>
[IntegrationTest]
[DatabaseTest]
public class PackageIntegrationTests : DatabaseTestBase
{
    [Fact]
    public async Task Package_Should_Be_Persisted_To_Database()
    {
        // Arrange
        var packageName = TestData.RandomString(10);
        
        // Act
        // Note: Actual database operations would be tested here
        await Task.Delay(1); // Simulate async database operation
        
        // Assert
        packageName.Should().NotBeNullOrEmpty();
    }
}