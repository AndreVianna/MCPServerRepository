namespace Domain.UnitTests;

/// <summary>
/// Sample unit test class demonstrating the testing framework setup.
/// </summary>
[UnitTest]
public class SampleUnitTests : TestBase {
    [Fact]
    public void SampleTest_Should_Pass() {
        // Arrange
        var expected = "Hello World";

        // Act
        var actual = "Hello World";

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void SampleMockTest_Should_Use_NSubstitute() {
        // Arrange
        var mockLogger = CreateMock<ILogger<SampleUnitTests>>();
        var testMessage = "Test message";

        // Act
        mockLogger.LogInformation(testMessage);

        // Assert
        mockLogger.Received(1).LogInformation(testMessage);
    }

    [Fact]
    public void SampleDataTest_Should_Generate_Random_Data() {
        // Arrange & Act
        var randomString = TestData.RandomString(10);
        var randomEmail = TestData.RandomEmail();
        var randomInt = TestData.RandomInt(1, 100);

        // Assert
        randomString.Length.Should().Be(10);
        randomEmail.Should().Contain("@");
        randomInt.Should().BeGreaterThan(0);
        randomInt.Should().BeLessThan(100);
    }
}

/// <summary>
/// Sample integration test class demonstrating test categories.
/// </summary>
[IntegrationTest]
public class SampleIntegrationTests : TestBase {
    [Fact]
    public void SampleIntegrationTest_Should_Pass() {
        // Arrange
        var configuration = GetService<IConfiguration>();

        // Act
        var environment = configuration["Environment"];

        // Assert
        environment.Should().Be("Test");
    }
}

/// <summary>
/// Sample database test class demonstrating database testing.
/// </summary>
[DatabaseTest]
public class SampleDatabaseTests : DatabaseTestBase {
    [Fact]
    public void SampleDatabaseTest_Should_Pass() {
        // Arrange
        var testData = TestData.RandomString();

        // Act & Assert
        testData.Should().NotBeNullOrEmpty();
    }
}