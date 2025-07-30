using System.Security.Claims;

using MCPHub.PublicApi.Controllers.V1;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Moq;

namespace MCPHub.PublicApi.UnitTests.Controllers.V1;

public class SimpleBaseApiV1ControllerTests {
    private readonly TestController _controller;
    private readonly Mock<ILogger<TestController>> _mockLogger;

    public SimpleBaseApiV1ControllerTests() {
        _mockLogger = new Mock<ILogger<TestController>>();
        _controller = new TestController(_mockLogger.Object) {
            // Setup controller context
            ControllerContext = new ControllerContext {
                HttpContext = new DefaultHttpContext(),
            },
        };
    }

    [Fact]
    public void CreateErrorResponse_ShouldReturnCorrectFormat() {
        // Arrange
        var message = "Test error message";
        var statusCode = 400;

        // Act
        var result = _controller.TestCreateErrorResponse(message, statusCode) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(statusCode, result.StatusCode);

        dynamic value = result.Value!;
        Assert.Equal(message, value.Error.Message);
        Assert.Equal("1.0", value.Error.Version);
        Assert.NotNull(value.Error.Timestamp);
        Assert.NotNull(value.Error.TraceId);
    }

    [Fact]
    public void CreateSuccessResponse_ShouldReturnCorrectFormat() {
        // Arrange
        var data = new { Id = 1, Name = "Test" };
        var message = "Success message";

        // Act
        var result = _controller.TestCreateSuccessResponse(data, message) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);

        dynamic value = result.Value!;
        Assert.Equal(data, value.Data);
        Assert.Equal(message, value.Message);
        Assert.Equal("1.0", value.Version);
        Assert.NotNull(value.Timestamp);
    }

    [Fact]
    public void GetCurrentUserId_WhenNotAuthenticated_ShouldReturnNull() {
        // Act
        var userId = _controller.TestGetCurrentUserId();

        // Assert
        Assert.Null(userId);
    }

    [Fact]
    public void GetCurrentUserId_WhenAuthenticated_ShouldReturnUserId() {
        // Arrange
        var expectedUserId = Guid.NewGuid();
        var claims = new List<Claim>
        {
            new("sub", expectedUserId.ToString()),
        };
        var identity = new ClaimsIdentity(claims, "test");
        _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(identity);

        // Act
        var userId = _controller.TestGetCurrentUserId();

        // Assert
        Assert.Equal(expectedUserId, userId);
    }

    [Fact]
    public void GetCurrentUserRoles_WhenNotAuthenticated_ShouldReturnEmpty() {
        // Act
        var roles = _controller.TestGetCurrentUserRoles();

        // Assert
        Assert.NotNull(roles);
        Assert.Empty(roles);
    }

    [Fact]
    public void HasRole_WhenUserHasRole_ShouldReturnTrue() {
        // Arrange
        var claims = new List<Claim>
        {
            new(ClaimTypes.Role, "Admin"),
            new(ClaimTypes.Role, "User"),
        };
        var identity = new ClaimsIdentity(claims, "test");
        _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(identity);

        // Act
        var hasAdminRole = _controller.TestHasRole("Admin");
        var hasUserRole = _controller.TestHasRole("User");
        var hasManagerRole = _controller.TestHasRole("Manager");

        // Assert
        Assert.True(hasAdminRole);
        Assert.True(hasUserRole);
        Assert.False(hasManagerRole);
    }

    // Test controller to expose protected methods
    private class TestController(ILogger<TestController> logger) : BaseApiV1Controller(logger) {
        public IActionResult TestCreateErrorResponse(string message, int statusCode = 400)
            => CreateErrorResponse(message, statusCode);

        public IActionResult TestCreateSuccessResponse<T>(T data, string? message = null)
            => CreateSuccessResponse(data, message);

        public Guid? TestGetCurrentUserId()
            => GetCurrentUserId();

        public IEnumerable<string> TestGetCurrentUserRoles()
            => GetCurrentUserRoles();

        public bool TestHasRole(string role)
            => HasRole(role);
    }
}