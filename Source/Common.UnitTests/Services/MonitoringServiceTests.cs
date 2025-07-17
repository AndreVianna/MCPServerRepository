using Common.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;
using AwesomeAssertions;

namespace Common.UnitTests.Services;

public class MonitoringServiceTests
{
    private readonly ILogger<MonitoringService> _logger;
    private readonly MonitoringService _monitoringService;

    public MonitoringServiceTests()
    {
        _logger = Substitute.For<ILogger<MonitoringService>>();
        _monitoringService = new MonitoringService(_logger);
    }

    [Fact]
    public void RecordHttpRequest_DoesNotThrow()
    {
        // Arrange
        var method = "GET";
        var path = "/api/test";
        var statusCode = 200;
        var duration = TimeSpan.FromMilliseconds(100);

        // Act & Assert
        Action act = () => _monitoringService.RecordHttpRequest(method, path, statusCode, duration);
        act.Should().NotThrow();
    }

    [Fact]
    public void RecordDatabaseQuery_DoesNotThrow()
    {
        // Arrange
        var operation = "SELECT";
        var table = "Users";
        var duration = TimeSpan.FromMilliseconds(50);
        var success = true;

        // Act & Assert
        Action act = () => _monitoringService.RecordDatabaseQuery(operation, table, duration, success);
        act.Should().NotThrow();
    }

    [Fact]
    public void RecordCacheOperation_DoesNotThrow()
    {
        // Arrange
        var operation = "GET";
        var duration = TimeSpan.FromMilliseconds(10);
        var hit = true;

        // Act & Assert
        Action act = () => _monitoringService.RecordCacheOperation(operation, duration, hit);
        act.Should().NotThrow();
    }

    [Fact]
    public void RecordSearchOperation_DoesNotThrow()
    {
        // Arrange
        var operation = "search";
        var duration = TimeSpan.FromMilliseconds(200);
        var resultCount = 42;

        // Act & Assert
        Action act = () => _monitoringService.RecordSearchOperation(operation, duration, resultCount);
        act.Should().NotThrow();
    }

    [Fact]
    public void RecordMessageProcessing_DoesNotThrow()
    {
        // Arrange
        var messageType = "UserCreated";
        var duration = TimeSpan.FromMilliseconds(75);
        var success = true;

        // Act & Assert
        Action act = () => _monitoringService.RecordMessageProcessing(messageType, duration, success);
        act.Should().NotThrow();
    }

    [Fact]
    public void RecordSecurityScan_DoesNotThrow()
    {
        // Arrange
        var scanType = "vulnerability";
        var duration = TimeSpan.FromSeconds(5);
        var result = "clean";

        // Act & Assert
        Action act = () => _monitoringService.RecordSecurityScan(scanType, duration, result);
        act.Should().NotThrow();
    }

    [Fact]
    public void RecordError_LogsError()
    {
        // Arrange
        var component = "Database";
        var errorType = "ConnectionTimeout";
        var message = "Connection timed out after 30 seconds";

        // Act
        _monitoringService.RecordError(component, errorType, message);

        // Assert
        _logger.Received(1).LogError(
            Arg.Is<string>(s => s.Contains("Error recorded")),
            component,
            errorType,
            message);
    }

    [Fact]
    public void StartActivity_ReturnsDisposable()
    {
        // Arrange
        var operationName = "TestOperation";

        // Act
        var activity = _monitoringService.StartActivity(operationName);

        // Assert
        activity.Should().NotBeNull();
        activity.Should().BeAssignableTo<IDisposable>();
    }

    [Fact]
    public void StartActivity_DisposableCanBeDisposed()
    {
        // Arrange
        var operationName = "TestOperation";

        // Act
        using var activity = _monitoringService.StartActivity(operationName);

        // Assert
        activity.Should().NotBeNull();
        // If we get here without exception, disposal worked
    }

    [Fact]
    public void Constructor_DoesNotThrow()
    {
        // Act & Assert
        Action act = () => new MonitoringService(_logger);
        act.Should().NotThrow();
    }

    [Theory]
    [InlineData("GET", "/api/users", 200)]
    [InlineData("POST", "/api/users", 201)]
    [InlineData("PUT", "/api/users/1", 204)]
    [InlineData("DELETE", "/api/users/1", 204)]
    [InlineData("GET", "/api/users/1", 404)]
    [InlineData("POST", "/api/users", 500)]
    public void RecordHttpRequest_HandlesDifferentHttpMethods(string method, string path, int statusCode)
    {
        // Arrange
        var duration = TimeSpan.FromMilliseconds(100);

        // Act & Assert
        Action act = () => _monitoringService.RecordHttpRequest(method, path, statusCode, duration);
        act.Should().NotThrow();
    }

    [Theory]
    [InlineData("SELECT", true)]
    [InlineData("INSERT", true)]
    [InlineData("UPDATE", true)]
    [InlineData("DELETE", true)]
    [InlineData("SELECT", false)]
    [InlineData("INSERT", false)]
    public void RecordDatabaseQuery_HandlesDifferentOperations(string operation, bool success)
    {
        // Arrange
        var table = "TestTable";
        var duration = TimeSpan.FromMilliseconds(50);

        // Act & Assert
        Action act = () => _monitoringService.RecordDatabaseQuery(operation, table, duration, success);
        act.Should().NotThrow();
    }
}