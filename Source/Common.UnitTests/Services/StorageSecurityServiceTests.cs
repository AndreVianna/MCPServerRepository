using System.Text;

using Common.Models;
using Common.Services;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using NSubstitute;

namespace Common.UnitTests.Services;

[TestClass]
[TestCategory(TestCategories.Unit)]
public class StorageSecurityServiceTests {
    private IStorageSecurityService _securityService = null!;
    private ICacheService _cacheService = null!;
    private StorageConfiguration _configuration = null!;
    private ILogger<StorageSecurityService> _logger = null!;

    [TestInitialize]
    public void Setup() {
        _configuration = new StorageConfiguration {
            Options = new StorageOptions {
                MaxFileSize = 1024 * 1024, // 1MB
                AllowedFileExtensions = [".txt", ".json", ".xml"],
                BlockedFileExtensions = [".exe", ".bat", ".cmd", ".scr"]
            },
            Security = new StorageSecuritySettings {
                EnableEncryptionAtRest = true,
                EnableEncryptionInTransit = true,
                EnableAccessLogging = true,
                EnableVirusScanning = true,
                MaxDownloadAttemptsPerHour = 100,
                AllowedIpAddresses = ["192.168.1.0/24", "10.0.0.0/8"],
                BlockedIpAddresses = ["192.168.1.100", "10.0.0.50"]
            }
        };

        var options = Options.Create(_configuration);
        _cacheService = Substitute.For<ICacheService>();
        _logger = Substitute.For<ILogger<StorageSecurityService>>();

        _securityService = new StorageSecurityService(options, _cacheService, _logger);
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public async Task ValidateFileUploadAsync_ValidFile_ShouldReturnValid() {
        // Arrange
        var fileName = "test-file.txt";
        var content = "This is a valid test file";
        var contentType = "text/plain";
        var clientIp = "192.168.1.10";

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

        // Act
        var result = await _securityService.ValidateFileUploadAsync(fileName, stream, contentType, clientIp);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public async Task ValidateFileUploadAsync_InvalidExtension_ShouldReturnInvalid() {
        // Arrange
        var fileName = "malicious-file.exe";
        var content = "This is a malicious file";
        var contentType = "application/octet-stream";

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

        // Act
        var result = await _securityService.ValidateFileUploadAsync(fileName, stream, contentType);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("File extension not allowed"));
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public async Task ValidateFileUploadAsync_FileTooLarge_ShouldReturnInvalid() {
        // Arrange
        var fileName = "large-file.txt";
        var content = new string('A', 2 * 1024 * 1024); // 2MB file, exceeds 1MB limit
        var contentType = "text/plain";

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

        // Act
        var result = await _securityService.ValidateFileUploadAsync(fileName, stream, contentType);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("File size exceeds maximum"));
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public async Task ScanFileAsync_CleanFile_ShouldReturnClean() {
        // Arrange
        var fileName = "clean-file.txt";
        var content = "This is a clean file with no malware";

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

        // Act
        var result = await _securityService.ScanFileAsync(stream, fileName);

        // Assert
        result.Should().NotBeNull();
        result.IsClean.Should().BeTrue();
        result.FileName.Should().Be(fileName);
        result.ScanTimestamp.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(10));
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public async Task ScanFileAsync_MaliciousFile_ShouldReturnInfected() {
        // Arrange
        var fileName = "infected-file.txt";
        var content = "This file contains EICAR-STANDARD-ANTIVIRUS-TEST-FILE signature";

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

        // Act
        var result = await _securityService.ScanFileAsync(stream, fileName);

        // Assert
        result.Should().NotBeNull();
        result.IsClean.Should().BeFalse();
        result.ThreatName.Should().NotBeNullOrEmpty();
        result.ThreatType.Should().NotBeNullOrEmpty();
        result.FileName.Should().Be(fileName);
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public async Task EncryptContentAsync_ValidContent_ShouldReturnEncryptedStream() {
        // Arrange
        var originalContent = "This is content to encrypt";
        using var originalStream = new MemoryStream(Encoding.UTF8.GetBytes(originalContent));

        // Act
        using var encryptedStream = await _securityService.EncryptContentAsync(originalStream);

        // Assert
        encryptedStream.Should().NotBeNull();
        encryptedStream.Length.Should().BeGreaterThan(0);
        encryptedStream.Length.Should().NotBe(originalContent.Length); // Should be different due to encryption
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public async Task DecryptContentAsync_EncryptedContent_ShouldReturnOriginalContent() {
        // Arrange
        var originalContent = "This is content to encrypt and decrypt";
        using var originalStream = new MemoryStream(Encoding.UTF8.GetBytes(originalContent));

        // First encrypt
        using var encryptedStream = await _securityService.EncryptContentAsync(originalStream);

        // Act
        using var decryptedStream = await _securityService.DecryptContentAsync(encryptedStream);
        var decryptedContent = await new StreamReader(decryptedStream).ReadToEndAsync();

        // Assert
        decryptedContent.Should().Be(originalContent);
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public async Task IsIpAddressAllowedAsync_AllowedIp_ShouldReturnTrue() {
        // Arrange
        var ipAddress = "192.168.1.10";

        // Act
        var result = await _securityService.IsIpAddressAllowedAsync(ipAddress);

        // Assert
        result.Should().BeTrue();
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public async Task IsIpAddressAllowedAsync_BlockedIp_ShouldReturnFalse() {
        // Arrange
        var ipAddress = "192.168.1.100";

        // Act
        var result = await _securityService.IsIpAddressAllowedAsync(ipAddress);

        // Assert
        result.Should().BeFalse();
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public async Task GetRateLimitStatusAsync_NewClient_ShouldReturnAllowed() {
        // Arrange
        var clientIdentifier = "test-client-123";
        _cacheService.GetAsync<int>($"rate_limit:{clientIdentifier}", Arg.Any<CancellationToken>())
                    .Returns(Task.FromResult<int?>(null));

        // Act
        var result = await _securityService.GetRateLimitStatusAsync(clientIdentifier);

        // Assert
        result.Should().NotBeNull();
        result.IsAllowed.Should().BeTrue();
        result.CurrentCount.Should().Be(0);
        result.MaxAllowed.Should().Be(_configuration.Security.MaxDownloadAttemptsPerHour);
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public async Task GetRateLimitStatusAsync_ExceededLimit_ShouldReturnNotAllowed() {
        // Arrange
        var clientIdentifier = "test-client-456";
        var currentCount = _configuration.Security.MaxDownloadAttemptsPerHour + 1;

        _cacheService.GetAsync<int>($"rate_limit:{clientIdentifier}", Arg.Any<CancellationToken>())
                    .Returns(Task.FromResult<int?>(currentCount));

        // Act
        var result = await _securityService.GetRateLimitStatusAsync(clientIdentifier);

        // Assert
        result.Should().NotBeNull();
        result.IsAllowed.Should().BeFalse();
        result.CurrentCount.Should().Be(currentCount);
        result.MaxAllowed.Should().Be(_configuration.Security.MaxDownloadAttemptsPerHour);
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public async Task UpdateRateLimitAsync_ShouldIncrementCount() {
        // Arrange
        var clientIdentifier = "test-client-789";
        var initialCount = 5;

        _cacheService.GetAsync<int>($"rate_limit:{clientIdentifier}", Arg.Any<CancellationToken>())
                    .Returns(Task.FromResult<int?>(initialCount));

        // Act
        await _securityService.UpdateRateLimitAsync(clientIdentifier, StorageOperation.Download);

        // Assert
        await _cacheService.Received(1).SetAsync(
            $"rate_limit:{clientIdentifier}",
            initialCount + 1,
            TimeSpan.FromHours(1),
            Arg.Any<CancellationToken>());
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public async Task LogSecurityEventAsync_ShouldLogEvent() {
        // Arrange
        var securityEvent = new StorageSecurityEvent {
            EventType = StorageSecurityEventType.FileUploadValidation,
            FileName = "test-file.txt",
            ContainerName = "test-container",
            ClientIpAddress = "192.168.1.10",
            IsSuccess = true,
            Timestamp = DateTimeOffset.UtcNow,
            Details = "File upload validation passed"
        };

        // Act
        await _securityService.LogSecurityEventAsync(securityEvent);

        // Assert
        // Verify that the logger was called (this depends on the specific logger implementation)
        _logger.Received(1).LogInformation(Arg.Any<string>(), Arg.Any<object[]>());
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public async Task ValidateFileDownloadAsync_ValidRequest_ShouldReturnValid() {
        // Arrange
        var containerName = "test-container";
        var fileName = "test-file.txt";
        var clientIp = "192.168.1.10";

        _cacheService.GetAsync<int>($"rate_limit:{clientIp}", Arg.Any<CancellationToken>())
                    .Returns(Task.FromResult<int?>(10)); // Well within limit

        // Act
        var result = await _securityService.ValidateFileDownloadAsync(containerName, fileName, clientIp);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public async Task ValidateFileDownloadAsync_RateLimitExceeded_ShouldReturnInvalid() {
        // Arrange
        var containerName = "test-container";
        var fileName = "test-file.txt";
        var clientIp = "192.168.1.10";

        _cacheService.GetAsync<int>($"rate_limit:{clientIp}", Arg.Any<CancellationToken>())
                    .Returns(Task.FromResult<int?>(_configuration.Security.MaxDownloadAttemptsPerHour + 1));

        // Act
        var result = await _securityService.ValidateFileDownloadAsync(containerName, fileName, clientIp);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("Rate limit exceeded"));
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public async Task ValidateFileDownloadAsync_BlockedIp_ShouldReturnInvalid() {
        // Arrange
        var containerName = "test-container";
        var fileName = "test-file.txt";
        var clientIp = "192.168.1.100"; // This IP is in the blocked list

        // Act
        var result = await _securityService.ValidateFileDownloadAsync(containerName, fileName, clientIp);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("IP address not allowed"));
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public async Task ScanFileAsync_ScriptContent_ShouldDetectThreat() {
        // Arrange
        var fileName = "script-file.txt";
        var content = "This file contains <script>alert('xss')</script> content";

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

        // Act
        var result = await _securityService.ScanFileAsync(stream, fileName);

        // Assert
        result.Should().NotBeNull();
        result.IsClean.Should().BeFalse();
        result.ThreatName.Should().Contain("Suspicious content");
        result.FileName.Should().Be(fileName);
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public async Task ScanFileAsync_JavaScriptContent_ShouldDetectThreat() {
        // Arrange
        var fileName = "js-file.txt";
        var content = "This file contains eval('malicious code') content";

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

        // Act
        var result = await _securityService.ScanFileAsync(stream, fileName);

        // Assert
        result.Should().NotBeNull();
        result.IsClean.Should().BeFalse();
        result.ThreatName.Should().Contain("Suspicious content");
        result.FileName.Should().Be(fileName);
    }
}