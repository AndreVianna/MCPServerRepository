using System.Text;

using Common.Models;
using Common.Services;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using NSubstitute;

namespace Common.UnitTests.Services;

[TestClass]
[TestCategory(TestCategories.Integration)]
public class StorageServiceTests {
    private IStorageService _storageService = null!;
    private IStorageSecurityService _securityService = null!;
    private IStorageMonitoringService _monitoringService = null!;
    private IStorageLifecycleService _lifecycleService = null!;
    private IStorageBackupService _backupService = null!;
    private StorageConfiguration _configuration = null!;
    private ILogger<AzureBlobStorageService> _logger = null!;

    [TestInitialize]
    public void Setup() {
        // Setup configuration for testing
        _configuration = new StorageConfiguration {
            Provider = StorageProviderType.AzureBlob,
            AzureBlob = new AzureBlobStorageConfiguration {
                ConnectionString = "UseDevelopmentStorage=true", // Use Azure Storage Emulator
                DefaultContainer = "test-container",
                MaxRetryAttempts = 3,
                RetryDelay = TimeSpan.FromMilliseconds(100)
            },
            Options = new StorageOptions {
                MaxFileSize = 10 * 1024 * 1024, // 10MB
                MaxConcurrentOperations = 5,
                EnableCompression = true,
                AllowedFileExtensions = [".txt", ".json", ".xml"],
                BlockedFileExtensions = [".exe", ".bat", ".cmd"],
                DefaultPresignedUrlExpiration = TimeSpan.FromHours(1),
                EnableVersioning = true,
                MaxVersionsToKeep = 5
            },
            Security = new StorageSecuritySettings {
                EnableEncryptionAtRest = true,
                EnableEncryptionInTransit = true,
                EnableAccessLogging = true,
                EnableVirusScanning = true,
                MaxDownloadAttemptsPerHour = 100
            },
            Monitoring = new StorageMonitoringSettings {
                EnableMetrics = true,
                EnableHealthChecks = true,
                MetricsInterval = TimeSpan.FromMinutes(1),
                HealthCheckInterval = TimeSpan.FromSeconds(30)
            }
        };

        var options = Options.Create(_configuration);
        _logger = Substitute.For<ILogger<AzureBlobStorageService>>();

        // Create storage service implementation
        _storageService = new AzureBlobStorageService(options, _logger);

        // Create supporting services
        var cacheService = Substitute.For<ICacheService>();
        var securityLogger = Substitute.For<ILogger<StorageSecurityService>>();
        _securityService = new StorageSecurityService(options, cacheService, securityLogger);

        var monitoringLogger = Substitute.For<ILogger<StorageMonitoringService>>();
        _monitoringService = new StorageMonitoringService(_storageService, cacheService, options, monitoringLogger);

        var lifecycleLogger = Substitute.For<ILogger<StorageLifecycleService>>();
        _lifecycleService = new StorageLifecycleService(_storageService, options, lifecycleLogger);

        var backupLogger = Substitute.For<ILogger<StorageBackupService>>();
        _backupService = new StorageBackupService(_storageService, _storageService, options, backupLogger);
    }

    [TestMethod]
    [TestCategory(TestCategories.Integration)]
    public async Task UploadAsync_ValidFile_ShouldSucceed() {
        // Arrange
        var containerName = "test-container";
        var fileName = "test-file.txt";
        var content = "This is a test file content";
        var contentType = "text/plain";
        var metadata = new Dictionary<string, string> { ["test"] = "value" };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

        // Act
        var result = await _storageService.UploadAsync(containerName, fileName, stream, contentType, metadata);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Contain(fileName);
    }

    [TestMethod]
    [TestCategory(TestCategories.Integration)]
    public async Task DownloadAsync_ExistingFile_ShouldReturnContent() {
        // Arrange
        var containerName = "test-container";
        var fileName = "test-download.txt";
        var originalContent = "This is test content for download";
        var contentType = "text/plain";

        // First upload a file
        using var uploadStream = new MemoryStream(Encoding.UTF8.GetBytes(originalContent));
        await _storageService.UploadAsync(containerName, fileName, uploadStream, contentType);

        // Act
        using var downloadStream = await _storageService.DownloadAsync(containerName, fileName);
        var downloadedContent = await new StreamReader(downloadStream).ReadToEndAsync();

        // Assert
        downloadedContent.Should().Be(originalContent);
    }

    [TestMethod]
    [TestCategory(TestCategories.Integration)]
    public async Task ExistsAsync_ExistingFile_ShouldReturnTrue() {
        // Arrange
        var containerName = "test-container";
        var fileName = "test-exists.txt";
        var content = "Test content";
        var contentType = "text/plain";

        // First upload a file
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await _storageService.UploadAsync(containerName, fileName, stream, contentType);

        // Act
        var exists = await _storageService.ExistsAsync(containerName, fileName);

        // Assert
        exists.Should().BeTrue();
    }

    [TestMethod]
    [TestCategory(TestCategories.Integration)]
    public async Task ExistsAsync_NonExistingFile_ShouldReturnFalse() {
        // Arrange
        var containerName = "test-container";
        var fileName = "non-existing-file.txt";

        // Act
        var exists = await _storageService.ExistsAsync(containerName, fileName);

        // Assert
        exists.Should().BeFalse();
    }

    [TestMethod]
    [TestCategory(TestCategories.Integration)]
    public async Task DeleteAsync_ExistingFile_ShouldSucceed() {
        // Arrange
        var containerName = "test-container";
        var fileName = "test-delete.txt";
        var content = "Test content";
        var contentType = "text/plain";

        // First upload a file
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await _storageService.UploadAsync(containerName, fileName, stream, contentType);

        // Act
        await _storageService.DeleteAsync(containerName, fileName);

        // Assert
        var exists = await _storageService.ExistsAsync(containerName, fileName);
        exists.Should().BeFalse();
    }

    [TestMethod]
    [TestCategory(TestCategories.Integration)]
    public async Task ListFilesAsync_ContainerWithFiles_ShouldReturnFiles() {
        // Arrange
        var containerName = "test-container";
        var fileNames = new[] { "file1.txt", "file2.txt", "file3.txt" };
        var content = "Test content";
        var contentType = "text/plain";

        // Upload multiple files
        foreach (var fileName in fileNames) {
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            await _storageService.UploadAsync(containerName, fileName, stream, contentType);
        }

        // Act
        var files = await _storageService.ListFilesAsync(containerName);

        // Assert
        files.Should().NotBeNull();
        files.Should().HaveCountGreaterOrEqualTo(fileNames.Length);
        fileNames.All(fn => files.Any(f => f.FileName == fn)).Should().BeTrue();
    }

    [TestMethod]
    [TestCategory(TestCategories.Integration)]
    public async Task GetMetadataAsync_ExistingFile_ShouldReturnMetadata() {
        // Arrange
        var containerName = "test-container";
        var fileName = "test-metadata.txt";
        var content = "Test content";
        var contentType = "text/plain";
        var metadata = new Dictionary<string, string> { ["test"] = "value", ["env"] = "test" };

        // First upload a file
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await _storageService.UploadAsync(containerName, fileName, stream, contentType, metadata);

        // Act
        var fileMetadata = await _storageService.GetMetadataAsync(containerName, fileName);

        // Assert
        fileMetadata.Should().NotBeNull();
        fileMetadata.FileName.Should().Be(fileName);
        fileMetadata.Size.Should().Be(content.Length);
        fileMetadata.ContentType.Should().Be(contentType);
        fileMetadata.Metadata.Should().ContainKey("test");
        fileMetadata.Metadata["test"].Should().Be("value");
    }

    [TestMethod]
    [TestCategory(TestCategories.Integration)]
    public async Task GetPresignedUrlAsync_ValidFile_ShouldReturnUrl() {
        // Arrange
        var containerName = "test-container";
        var fileName = "test-presigned.txt";
        var content = "Test content";
        var contentType = "text/plain";
        var expiration = TimeSpan.FromHours(1);

        // First upload a file
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await _storageService.UploadAsync(containerName, fileName, stream, contentType);

        // Act
        var presignedUrl = await _storageService.GetPresignedUrlAsync(containerName, fileName, expiration);

        // Assert
        presignedUrl.Should().NotBeNullOrEmpty();
        presignedUrl.Should().StartWith("https://");
    }

    [TestMethod]
    [TestCategory(TestCategories.Integration)]
    public async Task CopyAsync_ExistingFile_ShouldCopySuccessfully() {
        // Arrange
        var sourceContainer = "test-container";
        var sourceFileName = "source-file.txt";
        var destinationContainer = "test-container";
        var destinationFileName = "destination-file.txt";
        var content = "Test content for copy";
        var contentType = "text/plain";

        // First upload a source file
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await _storageService.UploadAsync(sourceContainer, sourceFileName, stream, contentType);

        // Act
        await _storageService.CopyAsync(sourceContainer, sourceFileName, destinationContainer, destinationFileName);

        // Assert
        var destinationExists = await _storageService.ExistsAsync(destinationContainer, destinationFileName);
        destinationExists.Should().BeTrue();

        // Verify content is the same
        using var downloadStream = await _storageService.DownloadAsync(destinationContainer, destinationFileName);
        var downloadedContent = await new StreamReader(downloadStream).ReadToEndAsync();
        downloadedContent.Should().Be(content);
    }

    [TestMethod]
    [TestCategory(TestCategories.Integration)]
    public async Task SecurityService_ValidateFileUpload_ShouldValidateCorrectly() {
        // Arrange
        var fileName = "test-file.txt";
        var content = "This is a test file content";
        var contentType = "text/plain";
        var clientIp = "192.168.1.1";

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

        // Act
        var result = await _securityService.ValidateFileUploadAsync(fileName, stream, contentType, clientIp);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [TestMethod]
    [TestCategory(TestCategories.Integration)]
    public async Task SecurityService_ValidateFileUpload_InvalidExtension_ShouldFail() {
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
    [TestCategory(TestCategories.Integration)]
    public async Task SecurityService_ScanFile_CleanFile_ShouldPassScan() {
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
    }

    [TestMethod]
    [TestCategory(TestCategories.Integration)]
    public async Task SecurityService_ScanFile_MaliciousFile_ShouldFailScan() {
        // Arrange
        var fileName = "malicious-file.txt";
        var content = "This file contains EICAR-STANDARD-ANTIVIRUS-TEST-FILE signature";

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

        // Act
        var result = await _securityService.ScanFileAsync(stream, fileName);

        // Assert
        result.Should().NotBeNull();
        result.IsClean.Should().BeFalse();
        result.ThreatName.Should().NotBeNullOrEmpty();
    }

    [TestMethod]
    [TestCategory(TestCategories.Integration)]
    public async Task MonitoringService_RecordOperation_ShouldRecordMetric() {
        // Arrange
        var metric = new StorageOperationMetric {
            OperationName = "upload",
            OperationType = StorageOperationType.Upload,
            ContainerName = "test-container",
            FileName = "test-file.txt",
            IsSuccess = true,
            ResponseTime = TimeSpan.FromMilliseconds(100),
            BytesTransferred = 1024,
            Timestamp = DateTimeOffset.UtcNow
        };

        // Act
        await _monitoringService.RecordOperationAsync(metric);

        // Assert - Check that metrics are recorded
        var metrics = await _monitoringService.GetMetricsAsync(TimeSpan.FromMinutes(1));
        metrics.Should().NotBeNull();
        metrics.TotalOperations.Should().BeGreaterThan(0);
    }

    [TestMethod]
    [TestCategory(TestCategories.Integration)]
    public async Task MonitoringService_GetHealthStatus_ShouldReturnStatus() {
        // Act
        var healthStatus = await _monitoringService.GetHealthStatusAsync();

        // Assert
        healthStatus.Should().NotBeNull();
        healthStatus.CheckedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(10));
        healthStatus.OverallStatus.Should().BeOneOf(HealthStatus.Healthy, HealthStatus.Degraded, HealthStatus.Unhealthy);
    }

    [TestMethod]
    [TestCategory(TestCategories.Integration)]
    public async Task LifecycleService_ValidatePolicy_ValidPolicy_ShouldReturnTrue() {
        // Arrange
        var policy = new StorageLifecyclePolicy {
            Name = "Test Policy",
            ContainerPattern = "test-.*",
            FilePattern = ".*\\.txt$",
            IsEnabled = true,
            Rules =
            [
                new StorageLifecycleRule
                {
                    Action = StorageLifecycleAction.Delete,
                    DaysAfterCreation = 30,
                    DaysAfterModification = 0
                }
            ]
        };

        // Act
        var isValid = await _lifecycleService.ValidateLifecyclePolicyAsync(policy);

        // Assert
        isValid.Should().BeTrue();
    }

    [TestMethod]
    [TestCategory(TestCategories.Integration)]
    public async Task LifecycleService_ValidatePolicy_InvalidPolicy_ShouldReturnFalse() {
        // Arrange
        var policy = new StorageLifecyclePolicy {
            Name = "", // Invalid: empty name
            ContainerPattern = "test-.*",
            IsEnabled = true,
            Rules = []
        };

        // Act
        var isValid = await _lifecycleService.ValidateLifecyclePolicyAsync(policy);

        // Assert
        isValid.Should().BeFalse();
    }

    [TestMethod]
    [TestCategory(TestCategories.Integration)]
    public async Task BackupService_CreateBackup_ShouldCreateSuccessfully() {
        // Arrange
        var containerName = "test-backup-container";
        var fileName = "backup-test-file.txt";
        var content = "This is content for backup testing";
        var contentType = "text/plain";

        // First upload a file to backup
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await _storageService.UploadAsync(containerName, fileName, stream, contentType);

        // Act
        var result = await _backupService.CreateBackupAsync(containerName);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.BackupId.Should().NotBeNullOrEmpty();
        result.ContainerName.Should().Be(containerName);
        result.FileCount.Should().BeGreaterThan(0);
    }

    [TestMethod]
    [TestCategory(TestCategories.Integration)]
    public async Task BackupService_RestoreBackup_ShouldRestoreSuccessfully() {
        // Arrange
        var containerName = "test-restore-container";
        var fileName = "restore-test-file.txt";
        var content = "This is content for restore testing";
        var contentType = "text/plain";

        // First upload a file
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await _storageService.UploadAsync(containerName, fileName, stream, contentType);

        // Create backup
        var backupResult = await _backupService.CreateBackupAsync(containerName);
        backupResult.IsSuccess.Should().BeTrue();

        // Delete original file
        await _storageService.DeleteAsync(containerName, fileName);

        // Act
        var restoreResult = await _backupService.RestoreBackupAsync(backupResult.BackupId, containerName);

        // Assert
        restoreResult.Should().NotBeNull();
        restoreResult.IsSuccess.Should().BeTrue();
        restoreResult.BackupId.Should().Be(backupResult.BackupId);
        restoreResult.RestoredFileCount.Should().BeGreaterThan(0);

        // Verify file was restored
        var exists = await _storageService.ExistsAsync(containerName, fileName);
        exists.Should().BeTrue();
    }

    [TestMethod]
    [TestCategory(TestCategories.Integration)]
    public async Task BackupService_ValidateBackup_ShouldValidateCorrectly() {
        // Arrange
        var containerName = "test-validation-container";
        var fileName = "validation-test-file.txt";
        var content = "This is content for validation testing";
        var contentType = "text/plain";

        // First upload a file
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await _storageService.UploadAsync(containerName, fileName, stream, contentType);

        // Create backup
        var backupResult = await _backupService.CreateBackupAsync(containerName);
        backupResult.IsSuccess.Should().BeTrue();

        // Act
        var validationResult = await _backupService.ValidateBackupAsync(backupResult.BackupId);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeTrue();
        validationResult.BackupId.Should().Be(backupResult.BackupId);
        validationResult.FileCount.Should().BeGreaterThan(0);
        validationResult.ValidationErrors.Should().BeEmpty();
    }

    [TestMethod]
    [TestCategory(TestCategories.Performance)]
    public async Task StorageService_ConcurrentOperations_ShouldHandleLoad() {
        // Arrange
        var containerName = "test-performance-container";
        var numberOfOperations = 50;
        var content = "Performance test content";
        var contentType = "text/plain";

        // Act
        var tasks = new List<Task>();
        for (var i = 0; i < numberOfOperations; i++) {
            var fileName = $"perf-test-{i}.txt";
            var task = Task.Run(async () => {
                using var stream = new MemoryStream(Encoding.UTF8.GetBytes($"{content}-{i}"));
                await _storageService.UploadAsync(containerName, fileName, stream, contentType);
            });
            tasks.Add(task);
        }

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await Task.WhenAll(tasks);
        stopwatch.Stop();

        // Assert
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(30000); // Should complete within 30 seconds

        // Verify all files were uploaded
        var files = await _storageService.ListFilesAsync(containerName);
        files.Count().Should().BeGreaterOrEqualTo(numberOfOperations);
    }

    [TestMethod]
    [TestCategory(TestCategories.Integration)]
    public async Task StorageService_LargeFile_ShouldHandleWithinLimits() {
        // Arrange
        var containerName = "test-large-file-container";
        var fileName = "large-test-file.txt";
        var contentType = "text/plain";
        var largeContent = new string('A', 1024 * 1024); // 1MB file

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(largeContent));

        // Act
        var uploadResult = await _storageService.UploadAsync(containerName, fileName, stream, contentType);

        // Assert
        uploadResult.Should().NotBeNullOrEmpty();

        // Verify file was uploaded correctly
        var exists = await _storageService.ExistsAsync(containerName, fileName);
        exists.Should().BeTrue();

        var metadata = await _storageService.GetMetadataAsync(containerName, fileName);
        metadata.Size.Should().Be(largeContent.Length);
    }

    [TestCleanup]
    public async Task Cleanup() {
        // Clean up test containers and files
        try {
            var testContainers = new[]
            {
                "test-container",
                "test-backup-container",
                "test-restore-container",
                "test-validation-container",
                "test-performance-container",
                "test-large-file-container"
            };

            foreach (var container in testContainers) {
                try {
                    await _storageService.DeleteContainerAsync(container);
                }
                catch {
                    // Ignore cleanup errors
                }
            }
        }
        catch {
            // Ignore cleanup errors
        }
    }
}