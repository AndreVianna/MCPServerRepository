using System.Security.Claims;

using MCPHub.Domain.Contracts.Services;
using MCPHub.Domain.ValueObjects;
using MCPHub.PublicApi.Controllers.V1;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Moq;

namespace MCPHub.PublicApi.UnitTests.Controllers.V1;

/// <summary>
/// Unit tests for publishing functionality in PackagesV1Controller
/// </summary>
public class PackagesV1ControllerPublishingTests {
    private readonly Mock<IPackageService> _mockPackageService;
    private readonly Mock<IPackagePublishingService> _mockPublishingService;
    private readonly Mock<IPackageInstallationService> _mockInstallationService;
    private readonly Mock<ISecurityScanService> _mockSecurityScanService;
    private readonly Mock<ITrustTierCalculationService> _mockTrustTierService;
    private readonly Mock<ILogger<PackagesV1Controller>> _mockLogger;
    private readonly PackagesV1Controller _controller;
    private readonly Guid _testUserId = Guid.NewGuid();

    public PackagesV1ControllerPublishingTests() {
        _mockPackageService = new Mock<IPackageService>();
        _mockPublishingService = new Mock<IPackagePublishingService>();
        _mockInstallationService = new Mock<IPackageInstallationService>();
        _mockSecurityScanService = new Mock<ISecurityScanService>();
        _mockTrustTierService = new Mock<ITrustTierCalculationService>();
        _mockLogger = new Mock<ILogger<PackagesV1Controller>>();

        _controller = new PackagesV1Controller(
            _mockPackageService.Object,
            _mockPublishingService.Object,
            _mockInstallationService.Object,
            _mockSecurityScanService.Object,
            _mockTrustTierService.Object,
            _mockLogger.Object);

        // Setup user claims
        var claims = new List<Claim>
        {
            new("sub", _testUserId.ToString()),
            new("userId", _testUserId.ToString()),
        };
        var identity = new ClaimsIdentity(claims, "test");
        var principal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext {
            HttpContext = new DefaultHttpContext {
                User = principal,
            },
        };
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "PackagesV1Controller")]
    [Trait("Category", "Publishing")]
    public async Task PublishPackage_WithValidRequest_ShouldReturnCreated() {
        // Arrange
        var request = new PublishRequest {
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
            Tags = ["testing", "mcp"],
        };

        var package = new Package("test-package", "Test package", "1.0.0", _testUserId);
        var publishResult = PublishResult.CreateSuccess(package);

        _mockPublishingService
            .Setup(x => x.PublishPackageAsync(It.IsAny<PublishRequest>(), _testUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(publishResult);

        // Act
        var result = await _controller.PublishPackage(request, CancellationToken.None);

        // Assert
        Assert.IsType<CreatedAtActionResult>(result);
        var createdResult = (CreatedAtActionResult)result;
        Assert.Equal(201, createdResult.StatusCode);

        _mockPublishingService.Verify(
            x => x.PublishPackageAsync(request, _testUserId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "PackagesV1Controller")]
    [Trait("Category", "Publishing")]
    public async Task PublishPackage_WithInvalidRequest_ShouldReturnBadRequest() {
        // Arrange
        var request = new PublishRequest {
            ManifestContent = """
            {
                "name": "test-package",
                "version": "1.0.0",
                "description": "Test package",
                "author": { "name": "Test Author" },
                "license": "MIT"
            }
            """,
            // Missing both PackageArchive and PackageUrl
        };

        // Act
        var result = await _controller.PublishPackage(request, CancellationToken.None);

        // Assert
        Assert.IsType<ObjectResult>(result);
        var objectResult = (ObjectResult)result;
        Assert.Equal(400, objectResult.StatusCode);

        _mockPublishingService.Verify(
            x => x.PublishPackageAsync(It.IsAny<PublishRequest>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "PackagesV1Controller")]
    [Trait("Category", "Publishing")]
    public async Task PublishPackage_WithPublishingFailure_ShouldReturnBadRequest() {
        // Arrange
        var request = new PublishRequest {
            ManifestContent = """
            {
                "name": "invalid-package",
                "version": "1.0.0",
                "description": "Test package",
                "author": { "name": "Test Author" },
                "license": "MIT"
            }
            """,
            PackageUrl = "https://example.com/package.zip",
        };

        var publishResult = PublishResult.CreateFailure(["Invalid package name", "Missing required capabilities"]);

        _mockPublishingService
            .Setup(x => x.PublishPackageAsync(It.IsAny<PublishRequest>(), _testUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(publishResult);

        // Act
        var result = await _controller.PublishPackage(request, CancellationToken.None);

        // Assert
        Assert.IsType<ObjectResult>(result);
        var objectResult = (ObjectResult)result;
        Assert.Equal(400, objectResult.StatusCode);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "PackagesV1Controller")]
    [Trait("Category", "Publishing")]
    public async Task PublishPackageVersion_WithValidRequest_ShouldReturnCreated() {
        // Arrange
        var packageName = "test-package";
        var request = new PublishVersionRequest {
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
            PackageUrl = "https://example.com/package-1.1.0.zip",
        };

        var packageVersion = new PackageVersion(Guid.NewGuid(), "1.1.0", "https://example.com/package-1.1.0.zip", "checksum", 1024);
        var publishResult = PublishResult.CreateSuccess(null, packageVersion);

        _mockPublishingService
            .Setup(x => x.PublishPackageVersionAsync(packageName, It.IsAny<PublishVersionRequest>(), _testUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(publishResult);

        // Act
        var result = await _controller.PublishPackageVersion(packageName, request, CancellationToken.None);

        // Assert
        Assert.IsType<CreatedAtActionResult>(result);
        var createdResult = (CreatedAtActionResult)result;
        Assert.Equal(201, createdResult.StatusCode);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "PackagesV1Controller")]
    [Trait("Category", "Publishing")]
    public async Task PublishPackageVersion_WithEmptyPackageName_ShouldReturnBadRequest() {
        // Arrange
        var request = new PublishVersionRequest {
            Version = "1.1.0",
            ManifestContent = "{}",
            PackageUrl = "https://example.com/package.zip",
        };

        // Act
        var result = await _controller.PublishPackageVersion("", request, CancellationToken.None);

        // Assert
        Assert.IsType<ObjectResult>(result);
        var objectResult = (ObjectResult)result;
        Assert.Equal(400, objectResult.StatusCode);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "PackagesV1Controller")]
    [Trait("Category", "Publishing")]
    public async Task ValidateManifest_WithValidManifest_ShouldReturnOk() {
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
        Assert.IsType<ObjectResult>(result);
        var objectResult = (ObjectResult)result;
        Assert.Equal(200, objectResult.StatusCode);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "PackagesV1Controller")]
    [Trait("Category", "Publishing")]
    public async Task ValidateManifest_WithEmptyManifest_ShouldReturnBadRequest() {
        // Act
        var result = await _controller.ValidateManifest("", CancellationToken.None);

        // Assert
        Assert.IsType<ObjectResult>(result);
        var objectResult = (ObjectResult)result;
        Assert.Equal(400, objectResult.StatusCode);

        _mockPublishingService.Verify(
            x => x.ValidateManifestAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "PackagesV1Controller")]
    [Trait("Category", "Publishing")]
    public async Task GetPackageVersions_WithValidPackageName_ShouldReturnOk() {
        // Arrange
        var packageName = "test-package";
        var versions = new List<PackageVersionInfo>
        {
            new() { Version = "1.0.0", PublishedAt = DateTimeOffset.UtcNow, IsPrerelease = false },
            new() { Version = "1.1.0", PublishedAt = DateTimeOffset.UtcNow.AddDays(1), IsPrerelease = false },
        };

        _mockPublishingService
            .Setup(x => x.GetPackageVersionsAsync(packageName, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(versions);

        // Act
        var result = await _controller.GetPackageVersions(packageName, false, CancellationToken.None);

        // Assert
        Assert.IsType<ObjectResult>(result);
        var objectResult = (ObjectResult)result;
        Assert.Equal(200, objectResult.StatusCode);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "PackagesV1Controller")]
    [Trait("Category", "Publishing")]
    public async Task GetPackageVersions_WithNoVersions_ShouldReturnNotFound() {
        // Arrange
        var packageName = "nonexistent-package";
        var versions = new List<PackageVersionInfo>();

        _mockPublishingService
            .Setup(x => x.GetPackageVersionsAsync(packageName, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(versions);

        // Act
        var result = await _controller.GetPackageVersions(packageName, false, CancellationToken.None);

        // Assert
        Assert.IsType<ObjectResult>(result);
        var objectResult = (ObjectResult)result;
        Assert.Equal(404, objectResult.StatusCode);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "PackagesV1Controller")]
    [Trait("Category", "Publishing")]
    public async Task CheckPackageNameAvailability_WithAvailableName_ShouldReturnOk() {
        // Arrange
        var packageName = "available-package";

        _mockPublishingService
            .Setup(x => x.IsPackageNameAvailableAsync(packageName, _testUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.CheckPackageNameAvailability(packageName, CancellationToken.None);

        // Assert
        Assert.IsType<ObjectResult>(result);
        var objectResult = (ObjectResult)result;
        Assert.Equal(200, objectResult.StatusCode);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "PackagesV1Controller")]
    [Trait("Category", "Publishing")]
    public async Task CheckPackageNameAvailability_WithUnavailableName_ShouldReturnOk() {
        // Arrange
        var packageName = "taken-package";

        _mockPublishingService
            .Setup(x => x.IsPackageNameAvailableAsync(packageName, _testUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.CheckPackageNameAvailability(packageName, CancellationToken.None);

        // Assert
        Assert.IsType<ObjectResult>(result);
        var objectResult = (ObjectResult)result;
        Assert.Equal(200, objectResult.StatusCode);
        // Should still return 200 but with isAvailable = false in the response
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "PackagesV1Controller")]
    [Trait("Category", "Publishing")]
    public async Task CheckPackageNameAvailability_WithEmptyPackageName_ShouldReturnBadRequest() {
        // Act
        var result = await _controller.CheckPackageNameAvailability("", CancellationToken.None);

        // Assert
        Assert.IsType<ObjectResult>(result);
        var objectResult = (ObjectResult)result;
        Assert.Equal(400, objectResult.StatusCode);

        _mockPublishingService.Verify(
            x => x.IsPackageNameAvailableAsync(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "PackagesV1Controller")]
    [Trait("Category", "Publishing")]
    public async Task PublishPackage_WithUnauthorizedException_ShouldReturnForbidden() {
        // Arrange
        var request = new PublishRequest {
            ManifestContent = "{}",
            PackageUrl = "https://example.com/package.zip",
        };

        _mockPublishingService
            .Setup(x => x.PublishPackageAsync(It.IsAny<PublishRequest>(), _testUserId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UnauthorizedAccessException());

        // Act
        var result = await _controller.PublishPackage(request, CancellationToken.None);

        // Assert
        Assert.IsType<ObjectResult>(result);
        var objectResult = (ObjectResult)result;
        Assert.Equal(403, objectResult.StatusCode);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "PackagesV1Controller")]
    [Trait("Category", "Publishing")]
    public async Task PublishPackage_WithArgumentException_ShouldReturnBadRequest() {
        // Arrange
        var request = new PublishRequest {
            ManifestContent = "{}",
            PackageUrl = "https://example.com/package.zip",
        };

        _mockPublishingService
            .Setup(x => x.PublishPackageAsync(It.IsAny<PublishRequest>(), _testUserId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException("Invalid manifest format"));

        // Act
        var result = await _controller.PublishPackage(request, CancellationToken.None);

        // Assert
        Assert.IsType<ObjectResult>(result);
        var objectResult = (ObjectResult)result;
        Assert.Equal(400, objectResult.StatusCode);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [Trait("Category", "PackagesV1Controller")]
    [Trait("Category", "Publishing")]
    public async Task PublishPackage_WithGenericException_ShouldReturnInternalServerError() {
        // Arrange
        var request = new PublishRequest {
            ManifestContent = "{}",
            PackageUrl = "https://example.com/package.zip",
        };

        _mockPublishingService
            .Setup(x => x.PublishPackageAsync(It.IsAny<PublishRequest>(), _testUserId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Database connection failed"));

        // Act
        var result = await _controller.PublishPackage(request, CancellationToken.None);

        // Assert
        Assert.IsType<ObjectResult>(result);
        var objectResult = (ObjectResult)result;
        Assert.Equal(500, objectResult.StatusCode);
    }
}