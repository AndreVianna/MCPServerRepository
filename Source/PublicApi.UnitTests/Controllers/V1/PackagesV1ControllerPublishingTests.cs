using System.Security.Claims;
using MCPHub.Domain.Contracts.Requests;
using MCPHub.Domain.Contracts.Responses;
using MCPHub.Domain.Contracts.Services;
using MCPHub.Domain.Entities;
using MCPHub.Domain.ValueObjects;
using MCPHub.PublicApi.Controllers.V1;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace MCPHub.PublicApi.UnitTests.Controllers.V1;

/// <summary>
/// Unit tests for publishing functionality in PackagesV1Controller
/// </summary>
[TestClass]
public class PackagesV1ControllerPublishingTests
{
    private Mock<IPackageService> _mockPackageService;
    private Mock<IPackagePublishingService> _mockPublishingService;
    private Mock<ILogger<PackagesV1Controller>> _mockLogger;
    private PackagesV1Controller _controller;
    private readonly Guid _testUserId = Guid.NewGuid();

    [TestInitialize]
    public void Initialize()
    {
        _mockPackageService = new Mock<IPackageService>();
        _mockPublishingService = new Mock<IPackagePublishingService>();
        _mockLogger = new Mock<ILogger<PackagesV1Controller>>();

        _controller = new PackagesV1Controller(
            _mockPackageService.Object,
            _mockPublishingService.Object,
            _mockLogger.Object);

        // Setup user claims
        var claims = new List<Claim>
        {
            new("sub", _testUserId.ToString()),
            new("userId", _testUserId.ToString())
        };
        var identity = new ClaimsIdentity(claims, "test");
        var principal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = principal
            }
        };
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("PackagesV1Controller")]
    [TestCategory("Publishing")]
    public async Task PublishPackage_WithValidRequest_ShouldReturnCreated()
    {
        // Arrange
        var request = new PublishRequest
        {
            ManifestContent = """
            {
                "name": "test-package",
                "version": "1.0.0",
                "description": "Test package",
                "author": { "name": "Test Author" },
                "license": "MIT"
            }
            """,
            PackageUrl = "https://example.com/package.zip",
            Tags = ["testing", "mcp"]
        };

        var package = new Package("test-package", "Test package", "1.0.0", _testUserId);
        var publishResult = PublishResult.CreateSuccess(package);

        _mockPublishingService
            .Setup(x => x.PublishPackageAsync(It.IsAny<PublishRequest>(), _testUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(publishResult);

        // Act
        var result = await _controller.PublishPackage(request, CancellationToken.None);

        // Assert
        Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));
        var createdResult = (CreatedAtActionResult)result;
        Assert.AreEqual(201, createdResult.StatusCode);

        _mockPublishingService.Verify(
            x => x.PublishPackageAsync(request, _testUserId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("PackagesV1Controller")]
    [TestCategory("Publishing")]
    public async Task PublishPackage_WithInvalidRequest_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new PublishRequest
        {
            ManifestContent = """
            {
                "name": "test-package",
                "version": "1.0.0",
                "description": "Test package",
                "author": { "name": "Test Author" },
                "license": "MIT"
            }
            """
            // Missing both PackageArchive and PackageUrl
        };

        // Act
        var result = await _controller.PublishPackage(request, CancellationToken.None);

        // Assert
        Assert.IsInstanceOfType(result, typeof(ObjectResult));
        var objectResult = (ObjectResult)result;
        Assert.AreEqual(400, objectResult.StatusCode);

        _mockPublishingService.Verify(
            x => x.PublishPackageAsync(It.IsAny<PublishRequest>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("PackagesV1Controller")]
    [TestCategory("Publishing")]
    public async Task PublishPackage_WithPublishingFailure_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new PublishRequest
        {
            ManifestContent = """
            {
                "name": "invalid-package",
                "version": "1.0.0",
                "description": "Test package",
                "author": { "name": "Test Author" },
                "license": "MIT"
            }
            """,
            PackageUrl = "https://example.com/package.zip"
        };

        var publishResult = PublishResult.CreateFailure(["Invalid package name", "Missing required capabilities"]);

        _mockPublishingService
            .Setup(x => x.PublishPackageAsync(It.IsAny<PublishRequest>(), _testUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(publishResult);

        // Act
        var result = await _controller.PublishPackage(request, CancellationToken.None);

        // Assert
        Assert.IsInstanceOfType(result, typeof(ObjectResult));
        var objectResult = (ObjectResult)result;
        Assert.AreEqual(400, objectResult.StatusCode);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("PackagesV1Controller")]
    [TestCategory("Publishing")]
    public async Task PublishPackageVersion_WithValidRequest_ShouldReturnCreated()
    {
        // Arrange
        var packageName = "test-package";
        var request = new PublishVersionRequest
        {
            Version = "1.1.0",
            ManifestContent = """
            {
                "name": "test-package",
                "version": "1.1.0",
                "description": "Test package updated",
                "author": { "name": "Test Author" },
                "license": "MIT"
            }
            """,
            PackageUrl = "https://example.com/package-1.1.0.zip"
        };

        var packageVersion = new PackageVersion(Guid.NewGuid(), "1.1.0", "https://example.com/package-1.1.0.zip", "checksum", 1024);
        var publishResult = PublishResult.CreateSuccess(null, packageVersion);

        _mockPublishingService
            .Setup(x => x.PublishPackageVersionAsync(packageName, It.IsAny<PublishVersionRequest>(), _testUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(publishResult);

        // Act
        var result = await _controller.PublishPackageVersion(packageName, request, CancellationToken.None);

        // Assert
        Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));
        var createdResult = (CreatedAtActionResult)result;
        Assert.AreEqual(201, createdResult.StatusCode);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("PackagesV1Controller")]
    [TestCategory("Publishing")]
    public async Task PublishPackageVersion_WithEmptyPackageName_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new PublishVersionRequest
        {
            Version = "1.1.0",
            ManifestContent = "{}",
            PackageUrl = "https://example.com/package.zip"
        };

        // Act
        var result = await _controller.PublishPackageVersion("", request, CancellationToken.None);

        // Assert
        Assert.IsInstanceOfType(result, typeof(ObjectResult));
        var objectResult = (ObjectResult)result;
        Assert.AreEqual(400, objectResult.StatusCode);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("PackagesV1Controller")]
    [TestCategory("Publishing")]
    public async Task ValidateManifest_WithValidManifest_ShouldReturnOk()
    {
        // Arrange
        var manifestContent = """
        {
            "name": "test-package",
            "version": "1.0.0",
            "description": "Test package",
            "author": { "name": "Test Author" },
            "license": "MIT"
        }
        """;

        var validationSummary = ValidationSummary.Success("manifest", 50);
        var publishResult = PublishResult.CreateSuccess(validationSummary: validationSummary);

        _mockPublishingService
            .Setup(x => x.ValidateManifestAsync(manifestContent, It.IsAny<CancellationToken>()))
            .ReturnsAsync(publishResult);

        // Act
        var result = await _controller.ValidateManifest(manifestContent, CancellationToken.None);

        // Assert
        Assert.IsInstanceOfType(result, typeof(ObjectResult));
        var objectResult = (ObjectResult)result;
        Assert.AreEqual(200, objectResult.StatusCode);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("PackagesV1Controller")]
    [TestCategory("Publishing")]
    public async Task ValidateManifest_WithEmptyManifest_ShouldReturnBadRequest()
    {
        // Act
        var result = await _controller.ValidateManifest("", CancellationToken.None);

        // Assert
        Assert.IsInstanceOfType(result, typeof(ObjectResult));
        var objectResult = (ObjectResult)result;
        Assert.AreEqual(400, objectResult.StatusCode);

        _mockPublishingService.Verify(
            x => x.ValidateManifestAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("PackagesV1Controller")]
    [TestCategory("Publishing")]
    public async Task GetPackageVersions_WithValidPackageName_ShouldReturnOk()
    {
        // Arrange
        var packageName = "test-package";
        var versions = new List<PackageVersionInfo>
        {
            new() { Version = "1.0.0", PublishedAt = DateTimeOffset.UtcNow, IsPrerelease = false },
            new() { Version = "1.1.0", PublishedAt = DateTimeOffset.UtcNow.AddDays(1), IsPrerelease = false }
        };

        _mockPublishingService
            .Setup(x => x.GetPackageVersionsAsync(packageName, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(versions);

        // Act
        var result = await _controller.GetPackageVersions(packageName, false, CancellationToken.None);

        // Assert
        Assert.IsInstanceOfType(result, typeof(ObjectResult));
        var objectResult = (ObjectResult)result;
        Assert.AreEqual(200, objectResult.StatusCode);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("PackagesV1Controller")]
    [TestCategory("Publishing")]
    public async Task GetPackageVersions_WithNoVersions_ShouldReturnNotFound()
    {
        // Arrange
        var packageName = "nonexistent-package";
        var versions = new List<PackageVersionInfo>();

        _mockPublishingService
            .Setup(x => x.GetPackageVersionsAsync(packageName, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(versions);

        // Act
        var result = await _controller.GetPackageVersions(packageName, false, CancellationToken.None);

        // Assert
        Assert.IsInstanceOfType(result, typeof(ObjectResult));
        var objectResult = (ObjectResult)result;
        Assert.AreEqual(404, objectResult.StatusCode);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("PackagesV1Controller")]
    [TestCategory("Publishing")]
    public async Task CheckPackageNameAvailability_WithAvailableName_ShouldReturnOk()
    {
        // Arrange
        var packageName = "available-package";

        _mockPublishingService
            .Setup(x => x.IsPackageNameAvailableAsync(packageName, _testUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.CheckPackageNameAvailability(packageName, CancellationToken.None);

        // Assert
        Assert.IsInstanceOfType(result, typeof(ObjectResult));
        var objectResult = (ObjectResult)result;
        Assert.AreEqual(200, objectResult.StatusCode);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("PackagesV1Controller")]
    [TestCategory("Publishing")]
    public async Task CheckPackageNameAvailability_WithUnavailableName_ShouldReturnOk()
    {
        // Arrange
        var packageName = "taken-package";

        _mockPublishingService
            .Setup(x => x.IsPackageNameAvailableAsync(packageName, _testUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.CheckPackageNameAvailability(packageName, CancellationToken.None);

        // Assert
        Assert.IsInstanceOfType(result, typeof(ObjectResult));
        var objectResult = (ObjectResult)result;
        Assert.AreEqual(200, objectResult.StatusCode);
        // Should still return 200 but with isAvailable = false in the response
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("PackagesV1Controller")]
    [TestCategory("Publishing")]
    public async Task CheckPackageNameAvailability_WithEmptyPackageName_ShouldReturnBadRequest()
    {
        // Act
        var result = await _controller.CheckPackageNameAvailability("", CancellationToken.None);

        // Assert
        Assert.IsInstanceOfType(result, typeof(ObjectResult));
        var objectResult = (ObjectResult)result;
        Assert.AreEqual(400, objectResult.StatusCode);

        _mockPublishingService.Verify(
            x => x.IsPackageNameAvailableAsync(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("PackagesV1Controller")]
    [TestCategory("Publishing")]
    public async Task PublishPackage_WithUnauthorizedException_ShouldReturnForbidden()
    {
        // Arrange
        var request = new PublishRequest
        {
            ManifestContent = "{}",
            PackageUrl = "https://example.com/package.zip"
        };

        _mockPublishingService
            .Setup(x => x.PublishPackageAsync(It.IsAny<PublishRequest>(), _testUserId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UnauthorizedAccessException());

        // Act
        var result = await _controller.PublishPackage(request, CancellationToken.None);

        // Assert
        Assert.IsInstanceOfType(result, typeof(ObjectResult));
        var objectResult = (ObjectResult)result;
        Assert.AreEqual(403, objectResult.StatusCode);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("PackagesV1Controller")]
    [TestCategory("Publishing")]
    public async Task PublishPackage_WithArgumentException_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new PublishRequest
        {
            ManifestContent = "{}",
            PackageUrl = "https://example.com/package.zip"
        };

        _mockPublishingService
            .Setup(x => x.PublishPackageAsync(It.IsAny<PublishRequest>(), _testUserId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException("Invalid manifest format"));

        // Act
        var result = await _controller.PublishPackage(request, CancellationToken.None);

        // Assert
        Assert.IsInstanceOfType(result, typeof(ObjectResult));
        var objectResult = (ObjectResult)result;
        Assert.AreEqual(400, objectResult.StatusCode);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("PackagesV1Controller")]
    [TestCategory("Publishing")]
    public async Task PublishPackage_WithGenericException_ShouldReturnInternalServerError()
    {
        // Arrange
        var request = new PublishRequest
        {
            ManifestContent = "{}",
            PackageUrl = "https://example.com/package.zip"
        };

        _mockPublishingService
            .Setup(x => x.PublishPackageAsync(It.IsAny<PublishRequest>(), _testUserId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Database connection failed"));

        // Act
        var result = await _controller.PublishPackage(request, CancellationToken.None);

        // Assert
        Assert.IsInstanceOfType(result, typeof(ObjectResult));
        var objectResult = (ObjectResult)result;
        Assert.AreEqual(500, objectResult.StatusCode);
    }
}