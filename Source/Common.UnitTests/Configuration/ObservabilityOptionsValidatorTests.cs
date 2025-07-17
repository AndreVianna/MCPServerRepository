using Common.Configuration;
using Microsoft.Extensions.Options;
using AwesomeAssertions;

namespace Common.UnitTests.Configuration;

public class ObservabilityOptionsValidatorTests
{
    private readonly ObservabilityOptionsValidator _validator;

    public ObservabilityOptionsValidatorTests()
    {
        _validator = new ObservabilityOptionsValidator();
    }

    [Fact]
    public void Validate_WithValidOptions_ReturnsSuccess()
    {
        // Arrange
        var options = new ObservabilityOptions
        {
            ServiceName = "MCPHub",
            ServiceVersion = "1.0.0",
            Environment = "Development",
            OpenTelemetry = new OpenTelemetryOptions
            {
                EnableTracing = true,
                EnableMetrics = true,
                EnableLogging = true,
                OtlpEndpoint = "http://localhost:4317",
                Sources = ["MCPHub.*"],
                EnableConsoleExporter = true,
                EnableOtlpExporter = true
            },
            Serilog = new SerilogOptions
            {
                MinimumLevel = "Information",
                EnableConsole = true,
                EnableFile = true,
                LogDirectory = "logs",
                LogTemplate = "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                MinimumLevelOverrides = new Dictionary<string, string>
                {
                    { "Microsoft", "Warning" },
                    { "System", "Warning" }
                }
            }
        };

        // Act
        var result = _validator.Validate(null, options);

        // Assert
        result.Succeeded.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyServiceName_ReturnsFailure()
    {
        // Arrange
        var options = new ObservabilityOptions
        {
            ServiceName = "",
            ServiceVersion = "1.0.0",
            Environment = "Development",
            OpenTelemetry = new OpenTelemetryOptions
            {
                EnableOtlpExporter = false,
                Sources = ["MCPHub.*"]
            },
            Serilog = new SerilogOptions
            {
                MinimumLevel = "Information",
                EnableFile = false,
                LogTemplate = "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                MinimumLevelOverrides = new Dictionary<string, string>()
            }
        };

        // Act
        var result = _validator.Validate(null, options);

        // Assert
        result.Failed.Should().BeTrue();
        result.FailureMessage.Should().Contain("Observability ServiceName is required");
    }

    [Fact]
    public void Validate_WithOtlpExporterEnabledButNoEndpoint_ReturnsFailure()
    {
        // Arrange
        var options = new ObservabilityOptions
        {
            ServiceName = "MCPHub",
            ServiceVersion = "1.0.0",
            Environment = "Development",
            OpenTelemetry = new OpenTelemetryOptions
            {
                EnableOtlpExporter = true,
                OtlpEndpoint = "",
                Sources = ["MCPHub.*"]
            },
            Serilog = new SerilogOptions
            {
                MinimumLevel = "Information",
                EnableFile = false,
                LogTemplate = "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                MinimumLevelOverrides = new Dictionary<string, string>()
            }
        };

        // Act
        var result = _validator.Validate(null, options);

        // Assert
        result.Failed.Should().BeTrue();
        result.FailureMessage.Should().Contain("OpenTelemetry OtlpEndpoint is required when OtlpExporter is enabled");
    }

    [Fact]
    public void Validate_WithEmptyOpenTelemetrySources_ReturnsFailure()
    {
        // Arrange
        var options = new ObservabilityOptions
        {
            ServiceName = "MCPHub",
            ServiceVersion = "1.0.0",
            Environment = "Development",
            OpenTelemetry = new OpenTelemetryOptions
            {
                EnableOtlpExporter = false,
                Sources = []
            },
            Serilog = new SerilogOptions
            {
                MinimumLevel = "Information",
                EnableFile = false,
                LogTemplate = "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                MinimumLevelOverrides = new Dictionary<string, string>()
            }
        };

        // Act
        var result = _validator.Validate(null, options);

        // Assert
        result.Failed.Should().BeTrue();
        result.FailureMessage.Should().Contain("OpenTelemetry Sources must contain at least one source");
    }

    [Fact]
    public void Validate_WithFileLoggingEnabledButNoDirectory_ReturnsFailure()
    {
        // Arrange
        var options = new ObservabilityOptions
        {
            ServiceName = "MCPHub",
            ServiceVersion = "1.0.0",
            Environment = "Development",
            OpenTelemetry = new OpenTelemetryOptions
            {
                EnableOtlpExporter = false,
                Sources = ["MCPHub.*"]
            },
            Serilog = new SerilogOptions
            {
                MinimumLevel = "Information",
                EnableFile = true,
                LogDirectory = "",
                LogTemplate = "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                MinimumLevelOverrides = new Dictionary<string, string>()
            }
        };

        // Act
        var result = _validator.Validate(null, options);

        // Assert
        result.Failed.Should().BeTrue();
        result.FailureMessage.Should().Contain("Serilog LogDirectory is required when file logging is enabled");
    }
}