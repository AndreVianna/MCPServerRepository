using AwesomeAssertions;

using Common.Configuration;

using Microsoft.Extensions.Options;

namespace Common.UnitTests.Configuration;

public class DatabaseOptionsValidatorTests {
    private readonly DatabaseOptionsValidator _validator;

    public DatabaseOptionsValidatorTests() {
        _validator = new DatabaseOptionsValidator();
    }

    [Fact]
    public void Validate_WithValidOptions_ReturnsSuccess() {
        // Arrange
        var options = new DatabaseOptions {
            ConnectionString = "Host=localhost;Database=test;Username=user;Password=pass",
            MaxRetryCount = 3,
            CommandTimeout = TimeSpan.FromSeconds(30),
            MaxPoolSize = 100,
            HealthCheckTimeout = TimeSpan.FromSeconds(5)
        };

        // Act
        var result = _validator.Validate(null, options);

        // Assert
        result.Succeeded.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyConnectionString_ReturnsFailure() {
        // Arrange
        var options = new DatabaseOptions {
            ConnectionString = "",
            MaxRetryCount = 3,
            CommandTimeout = TimeSpan.FromSeconds(30),
            MaxPoolSize = 100,
            HealthCheckTimeout = TimeSpan.FromSeconds(5)
        };

        // Act
        var result = _validator.Validate(null, options);

        // Assert
        result.Failed.Should().BeTrue();
        result.FailureMessage.Should().Contain("Database connection string is required");
    }

    [Fact]
    public void Validate_WithNegativeMaxRetryCount_ReturnsFailure() {
        // Arrange
        var options = new DatabaseOptions {
            ConnectionString = "Host=localhost;Database=test;Username=user;Password=pass",
            MaxRetryCount = -1,
            CommandTimeout = TimeSpan.FromSeconds(30),
            MaxPoolSize = 100,
            HealthCheckTimeout = TimeSpan.FromSeconds(5)
        };

        // Act
        var result = _validator.Validate(null, options);

        // Assert
        result.Failed.Should().BeTrue();
        result.FailureMessage.Should().Contain("Database MaxRetryCount must be non-negative");
    }

    [Fact]
    public void Validate_WithZeroCommandTimeout_ReturnsFailure() {
        // Arrange
        var options = new DatabaseOptions {
            ConnectionString = "Host=localhost;Database=test;Username=user;Password=pass",
            MaxRetryCount = 3,
            CommandTimeout = TimeSpan.Zero,
            MaxPoolSize = 100,
            HealthCheckTimeout = TimeSpan.FromSeconds(5)
        };

        // Act
        var result = _validator.Validate(null, options);

        // Assert
        result.Failed.Should().BeTrue();
        result.FailureMessage.Should().Contain("Database CommandTimeout must be positive");
    }

    [Fact]
    public void Validate_WithZeroMaxPoolSize_ReturnsFailure() {
        // Arrange
        var options = new DatabaseOptions {
            ConnectionString = "Host=localhost;Database=test;Username=user;Password=pass",
            MaxRetryCount = 3,
            CommandTimeout = TimeSpan.FromSeconds(30),
            MaxPoolSize = 0,
            HealthCheckTimeout = TimeSpan.FromSeconds(5)
        };

        // Act
        var result = _validator.Validate(null, options);

        // Assert
        result.Failed.Should().BeTrue();
        result.FailureMessage.Should().Contain("Database MaxPoolSize must be positive");
    }

    [Fact]
    public void Validate_WithZeroHealthCheckTimeout_ReturnsFailure() {
        // Arrange
        var options = new DatabaseOptions {
            ConnectionString = "Host=localhost;Database=test;Username=user;Password=pass",
            MaxRetryCount = 3,
            CommandTimeout = TimeSpan.FromSeconds(30),
            MaxPoolSize = 100,
            HealthCheckTimeout = TimeSpan.Zero
        };

        // Act
        var result = _validator.Validate(null, options);

        // Assert
        result.Failed.Should().BeTrue();
        result.FailureMessage.Should().Contain("Database HealthCheckTimeout must be positive");
    }
}