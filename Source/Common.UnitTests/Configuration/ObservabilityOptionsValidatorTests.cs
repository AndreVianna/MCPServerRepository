using MCPHub.Common.Configuration.Validators;

namespace MCPHub.Common.Configuration;

public class ObservabilityOptionsValidatorTests {
    private readonly ObservabilityOptionsValidator _validator;

    public ObservabilityOptionsValidatorTests() {
        _validator = new ObservabilityOptionsValidator();
    }

    [Fact]
    public void Validate_WithValidOptions_ReturnsSuccess() {
        // Arrange
        var options = new ObservabilityOptions {
            ServiceName = "MCPHub",
            ServiceVersion = "1.0.0",
            Environment = "Development"
        };

        // Act
        var result = _validator.Validate(null, options);

        // Assert
        result.Succeeded.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyServiceName_ReturnsFailure() {
        // Arrange
        var options = new ObservabilityOptions {
            ServiceName = "",
            ServiceVersion = "1.0.0",
            Environment = "Development"
        };

        // Act
        var result = _validator.Validate(null, options);

        // Assert
        result.Failed.Should().BeTrue();
        result.FailureMessage.Should().Contain("Service name is required");
    }


}