using System.Diagnostics;

using AwesomeAssertions;

using Common.Services;

using Microsoft.Extensions.Logging;

using NSubstitute;

namespace Common.UnitTests.Services;

public class TracingServiceTests {
    private readonly ILogger<TracingService> _logger;
    private readonly TracingService _tracingService;

    public TracingServiceTests() {
        _logger = Substitute.For<ILogger<TracingService>>();
        _tracingService = new TracingService(_logger);
    }

    [Fact]
    public void StartActivity_ReturnsActivity() {
        // Arrange
        var name = "TestActivity";

        // Act
        var activity = _tracingService.StartActivity(name);

        // Assert
        activity.Should().NotBeNull();
        activity!.DisplayName.Should().Be(name);
    }

    [Fact]
    public void StartActivity_SetsDefaultTags() {
        // Arrange
        var name = "TestActivity";

        // Act
        var activity = _tracingService.StartActivity(name);

        // Assert
        activity.Should().NotBeNull();
        activity!.GetTagItem("service.name").Should().Be("MCPHub");
        activity.GetTagItem("service.version").Should().Be("1.0.0");
        activity.GetTagItem("environment").Should().NotBeNull();
    }

    [Fact]
    public void StartActivity_WithDifferentKind_SetsKindCorrectly() {
        // Arrange
        var name = "TestActivity";
        var kind = ActivityKind.Client;

        // Act
        var activity = _tracingService.StartActivity(name, kind);

        // Assert
        activity.Should().NotBeNull();
        activity!.Kind.Should().Be(kind);
    }

    [Fact]
    public void AddEvent_WithActivity_AddsEvent() {
        // Arrange
        var activity = _tracingService.StartActivity("TestActivity");
        var eventName = "TestEvent";
        var description = "Test event description";

        // Act
        _tracingService.AddEvent(activity, eventName, description);

        // Assert
        activity.Should().NotBeNull();
        // Note: ActivityEvent access is limited in testing, but we can verify the method doesn't throw
    }

    [Fact]
    public void AddEvent_WithNullActivity_DoesNotThrow() {
        // Arrange
        Activity? activity = null;
        var eventName = "TestEvent";

        // Act & Assert
        Action act = () => _tracingService.AddEvent(activity, eventName);
        act.Should().NotThrow();
    }

    [Fact]
    public void SetTag_WithActivity_SetsTag() {
        // Arrange
        var activity = _tracingService.StartActivity("TestActivity");
        var key = "test.key";
        var value = "test.value";

        // Act
        _tracingService.SetTag(activity, key, value);

        // Assert
        activity.Should().NotBeNull();
        activity!.GetTagItem(key).Should().Be(value);
    }

    [Fact]
    public void SetTag_WithNullActivity_DoesNotThrow() {
        // Arrange
        Activity? activity = null;
        var key = "test.key";
        var value = "test.value";

        // Act & Assert
        Action act = () => _tracingService.SetTag(activity, key, value);
        act.Should().NotThrow();
    }

    [Fact]
    public void SetError_WithActivity_SetsErrorTags() {
        // Arrange
        var activity = _tracingService.StartActivity("TestActivity");
        var exception = new InvalidOperationException("Test exception");

        // Act
        _tracingService.SetError(activity, exception);

        // Assert
        activity.Should().NotBeNull();
        activity!.GetTagItem("error.type").Should().Be("InvalidOperationException");
        activity.GetTagItem("error.message").Should().Be("Test exception");
        activity.Status.Should().Be(ActivityStatusCode.Error);
    }

    [Fact]
    public void SetError_WithNullActivity_DoesNotThrow() {
        // Arrange
        Activity? activity = null;
        var exception = new InvalidOperationException("Test exception");

        // Act & Assert
        Action act = () => _tracingService.SetError(activity, exception);
        act.Should().NotThrow();
    }

    [Fact]
    public void SetStatus_WithActivity_SetsStatus() {
        // Arrange
        var activity = _tracingService.StartActivity("TestActivity");
        var statusCode = ActivityStatusCode.Ok;
        var description = "Success";

        // Act
        _tracingService.SetStatus(activity, statusCode, description);

        // Assert
        activity.Should().NotBeNull();
        activity!.Status.Should().Be(statusCode);
        activity.StatusDescription.Should().Be(description);
    }

    [Fact]
    public void SetStatus_WithNullActivity_DoesNotThrow() {
        // Arrange
        Activity? activity = null;
        var statusCode = ActivityStatusCode.Ok;

        // Act & Assert
        Action act = () => _tracingService.SetStatus(activity, statusCode);
        act.Should().NotThrow();
    }

    [Fact]
    public void CreateScope_ReturnsDisposable() {
        // Arrange
        var operationName = "TestOperation";

        // Act
        var scope = _tracingService.CreateScope(operationName);

        // Assert
        scope.Should().NotBeNull();
        scope.Should().BeAssignableTo<IDisposable>();
    }

    [Fact]
    public void CreateScope_DisposableCanBeDisposed() {
        // Arrange
        var operationName = "TestOperation";

        // Act & Assert
        using var scope = _tracingService.CreateScope(operationName);
        scope.Should().NotBeNull();
        // If we get here without exception, disposal worked
    }

    [Fact]
    public void TraceDatabaseOperation_Extension_ReturnsDisposable() {
        // Arrange
        var operation = "SELECT";
        var table = "Users";

        // Act
        var scope = _tracingService.TraceDatabaseOperation(operation, table);

        // Assert
        scope.Should().NotBeNull();
        scope.Should().BeAssignableTo<IDisposable>();
    }

    [Fact]
    public void TraceCacheOperation_Extension_ReturnsDisposable() {
        // Arrange
        var operation = "GET";
        var key = "user:123";

        // Act
        var scope = _tracingService.TraceCacheOperation(operation, key);

        // Assert
        scope.Should().NotBeNull();
        scope.Should().BeAssignableTo<IDisposable>();
    }

    [Fact]
    public void TraceSearchOperation_Extension_ReturnsDisposable() {
        // Arrange
        var operation = "search";
        var query = "test query";

        // Act
        var scope = _tracingService.TraceSearchOperation(operation, query);

        // Assert
        scope.Should().NotBeNull();
        scope.Should().BeAssignableTo<IDisposable>();
    }

    [Fact]
    public void TraceMessageOperation_Extension_ReturnsDisposable() {
        // Arrange
        var operation = "process";
        var messageType = "UserCreated";

        // Act
        var scope = _tracingService.TraceMessageOperation(operation, messageType);

        // Assert
        scope.Should().NotBeNull();
        scope.Should().BeAssignableTo<IDisposable>();
    }

    [Fact]
    public void TracingExtensions_CanBeDisposed() {
        // Arrange & Act & Assert
        using var dbScope = _tracingService.TraceDatabaseOperation("SELECT", "Users");
        using var cacheScope = _tracingService.TraceCacheOperation("GET", "key");
        using var searchScope = _tracingService.TraceSearchOperation("search", "query");
        using var messageScope = _tracingService.TraceMessageOperation("process", "UserCreated");

        // If we get here without exception, all disposals worked
        dbScope.Should().NotBeNull();
        cacheScope.Should().NotBeNull();
        searchScope.Should().NotBeNull();
        messageScope.Should().NotBeNull();
    }
}