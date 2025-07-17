using AwesomeAssertions;

using Common.Middleware;
using Common.Services;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using NSubstitute;

namespace Common.UnitTests.Middleware;

public class RequestMonitoringMiddlewareTests {
    private readonly RequestDelegate _next;
    private readonly IMonitoringService _monitoringService;
    private readonly ILogger<RequestMonitoringMiddleware> _logger;
    private readonly RequestMonitoringMiddleware _middleware;

    public RequestMonitoringMiddlewareTests() {
        _next = Substitute.For<RequestDelegate>();
        _monitoringService = Substitute.For<IMonitoringService>();
        _logger = Substitute.For<ILogger<RequestMonitoringMiddleware>>();
        _middleware = new RequestMonitoringMiddleware(_next, _monitoringService, _logger);
    }

    [Fact]
    public async Task InvokeAsync_CallsNextMiddleware() {
        // Arrange
        var context = CreateHttpContext();

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        await _next.Received(1).Invoke(context);
    }

    [Fact]
    public async Task InvokeAsync_AddsCorrelationIdWhenNotPresent() {
        // Arrange
        var context = CreateHttpContext();

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        context.Request.Headers.Should().ContainKey("X-Correlation-ID");
        context.Response.Headers.Should().ContainKey("X-Correlation-ID");
    }

    [Fact]
    public async Task InvokeAsync_PreservesExistingCorrelationId() {
        // Arrange
        var context = CreateHttpContext();
        var correlationId = Guid.NewGuid().ToString();
        context.Request.Headers.Add("X-Correlation-ID", correlationId);

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        context.Request.Headers["X-Correlation-ID"].ToString().Should().Be(correlationId);
        context.Response.Headers["X-Correlation-ID"].ToString().Should().Be(correlationId);
    }

    [Fact]
    public async Task InvokeAsync_RecordsHttpRequestMetrics() {
        // Arrange
        var context = CreateHttpContext();
        context.Request.Method = "GET";
        context.Request.Path = "/api/test";
        context.Response.StatusCode = 200;

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        _monitoringService.Received(1).RecordHttpRequest(
            "GET",
            "/api/test",
            200,
            Arg.Any<TimeSpan>());
    }

    [Fact]
    public async Task InvokeAsync_LogsRequestStartAndCompletion() {
        // Arrange
        var context = CreateHttpContext();
        context.Request.Method = "GET";
        context.Request.Path = "/api/test";
        context.Response.StatusCode = 200;

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        _logger.Received(1).LogInformation(
            Arg.Is<string>(s => s.Contains("Request started")),
            "GET",
            "/api/test",
            Arg.Any<string>());

        _logger.Received(1).LogInformation(
            Arg.Is<string>(s => s.Contains("Request completed")),
            "GET",
            "/api/test",
            200,
            Arg.Any<long>(),
            Arg.Any<string>());
    }

    [Fact]
    public async Task InvokeAsync_HandlesExceptionsProperly() {
        // Arrange
        var context = CreateHttpContext();
        var exception = new InvalidOperationException("Test exception");
        _next.When(x => x.Invoke(context)).Throw(exception);

        // Act & Assert
        await _middleware.Invoking(m => m.InvokeAsync(context))
            .Should().ThrowAsync<InvalidOperationException>();

        _monitoringService.Received(1).RecordError(
            "HTTP",
            "InvalidOperationException",
            "Test exception");

        _monitoringService.Received(1).RecordHttpRequest(
            "GET",
            "/",
            500,
            Arg.Any<TimeSpan>());
    }

    [Fact]
    public async Task InvokeAsync_LogsErrorWhenExceptionOccurs() {
        // Arrange
        var context = CreateHttpContext();
        var exception = new InvalidOperationException("Test exception");
        _next.When(x => x.Invoke(context)).Throw(exception);

        // Act & Assert
        await _middleware.Invoking(m => m.InvokeAsync(context))
            .Should().ThrowAsync<InvalidOperationException>();

        _logger.Received(1).LogError(
            exception,
            Arg.Is<string>(s => s.Contains("Request failed")),
            "GET",
            "/",
            Arg.Any<long>());
    }

    [Theory]
    [InlineData("GET", "/api/users", 200)]
    [InlineData("POST", "/api/users", 201)]
    [InlineData("PUT", "/api/users/1", 204)]
    [InlineData("DELETE", "/api/users/1", 204)]
    [InlineData("GET", "/api/users/1", 404)]
    [InlineData("POST", "/api/users", 500)]
    public async Task InvokeAsync_HandlesDifferentHttpMethodsAndStatusCodes(string method, string path, int statusCode) {
        // Arrange
        var context = CreateHttpContext();
        context.Request.Method = method;
        context.Request.Path = path;
        context.Response.StatusCode = statusCode;

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        _monitoringService.Received(1).RecordHttpRequest(
            method,
            path,
            statusCode,
            Arg.Any<TimeSpan>());
    }

    [Fact]
    public async Task InvokeAsync_MeasuresRequestDuration() {
        // Arrange
        var context = CreateHttpContext();
        var delay = TimeSpan.FromMilliseconds(100);

        _next.When(x => x.Invoke(context))
            .Do(async _ => await Task.Delay(delay));

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        _monitoringService.Received(1).RecordHttpRequest(
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<int>(),
            Arg.Is<TimeSpan>(ts => ts >= delay));
    }

    private static HttpContext CreateHttpContext() {
        var context = new DefaultHttpContext();
        context.Request.Method = "GET";
        context.Request.Path = "/";
        context.Request.Headers.Clear();
        context.Response.StatusCode = 200;
        return context;
    }
}