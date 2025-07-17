using Common.Models;
using Common.Services;

using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using NSubstitute;

namespace Common.UnitTests.Services;

[TestClass]
[TestCategory(TestCategories.Unit)]
public class StorageMonitoringServiceTests {
    private IStorageMonitoringService _monitoringService = null!;
    private IStorageService _storageService = null!;
    private ICacheService _cacheService = null!;
    private StorageConfiguration _configuration = null!;
    private ILogger<StorageMonitoringService> _logger = null!;

    [TestInitialize]
    public void Setup() {
        _configuration = new StorageConfiguration {
            Monitoring = new StorageMonitoringSettings {
                EnableMetrics = true,
                EnableHealthChecks = true,
                MetricsInterval = TimeSpan.FromMinutes(5),
                HealthCheckInterval = TimeSpan.FromMinutes(1),
                Thresholds = new StorageThresholds {
                    HighUsagePercentage = 80.0,
                    CriticalUsagePercentage = 95.0,
                    MaxFailedOperationsPerMinute = 10,
                    MaxResponseTime = TimeSpan.FromSeconds(30)
                }
            }
        };

        var options = Options.Create(_configuration);
        _storageService = Substitute.For<IStorageService>();
        _cacheService = Substitute.For<ICacheService>();
        _logger = Substitute.For<ILogger<StorageMonitoringService>>();

        _monitoringService = new StorageMonitoringService(_storageService, _cacheService, options, _logger);
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public async Task RecordOperationAsync_ValidMetric_ShouldStoreMetric() {
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

        // Assert
        await _cacheService.Received(1).SetAsync(
            Arg.Is<string>(key => key.StartsWith("storage_metric:")),
            metric,
            TimeSpan.FromHours(24),
            Arg.Any<CancellationToken>());
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public async Task GetMetricsAsync_WithMultipleOperations_ShouldReturnAggregatedMetrics() {
        // Arrange
        var period = TimeSpan.FromMinutes(10);
        var now = DateTimeOffset.UtcNow;

        var metrics = new List<StorageOperationMetric>
        {
            new() {
                OperationName = "upload",
                OperationType = StorageOperationType.Upload,
                IsSuccess = true,
                ResponseTime = TimeSpan.FromMilliseconds(100),
                BytesTransferred = 1024,
                Timestamp = now.AddMinutes(-5)
            },
            new() {
                OperationName = "download",
                OperationType = StorageOperationType.Download,
                IsSuccess = true,
                ResponseTime = TimeSpan.FromMilliseconds(200),
                BytesTransferred = 2048,
                Timestamp = now.AddMinutes(-3)
            },
            new() {
                OperationName = "delete",
                OperationType = StorageOperationType.Delete,
                IsSuccess = false,
                ResponseTime = TimeSpan.FromMilliseconds(50),
                BytesTransferred = 0,
                Timestamp = now.AddMinutes(-1),
                ErrorType = "FileNotFoundException"
            }
        };

        // Simulate metrics being stored
        foreach (var metric in metrics) {
            await _monitoringService.RecordOperationAsync(metric);
        }

        // Act
        var result = await _monitoringService.GetMetricsAsync(period);

        // Assert
        result.Should().NotBeNull();
        result.Period.Should().Be(period);
        result.TotalOperations.Should().BeGreaterThan(0);
        result.SuccessfulOperations.Should().BeGreaterThan(0);
        result.FailedOperations.Should().BeGreaterThan(0);
        result.TotalBytesTransferred.Should().BeGreaterThan(0);
        result.SuccessRate.Should().BeGreaterThan(0);
        result.ErrorRate.Should().BeGreaterThan(0);
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public async Task GetHealthStatusAsync_HealthyOperations_ShouldReturnHealthy() {
        // Arrange
        var now = DateTimeOffset.UtcNow;

        // Record successful operations
        var successfulMetrics = new List<StorageOperationMetric>
        {
            new() {
                OperationName = "upload",
                OperationType = StorageOperationType.Upload,
                IsSuccess = true,
                ResponseTime = TimeSpan.FromMilliseconds(100),
                Timestamp = now.AddMinutes(-2)
            },
            new() {
                OperationName = "download",
                OperationType = StorageOperationType.Download,
                IsSuccess = true,
                ResponseTime = TimeSpan.FromMilliseconds(150),
                Timestamp = now.AddMinutes(-1)
            }
        };

        foreach (var metric in successfulMetrics) {
            await _monitoringService.RecordOperationAsync(metric);
        }

        // Act
        var result = await _monitoringService.GetHealthStatusAsync();

        // Assert
        result.Should().NotBeNull();
        result.OverallStatus.Should().Be(HealthStatus.Healthy);
        result.SuccessRate.Should().BeGreaterThan(0.9);
        result.ErrorRate.Should().BeLessThan(0.1);
        result.CheckedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(10));
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public async Task GetHealthStatusAsync_HighErrorRate_ShouldReturnUnhealthy() {
        // Arrange
        var now = DateTimeOffset.UtcNow;

        // Record mostly failed operations
        var failedMetrics = new List<StorageOperationMetric>
        {
            new() {
                OperationName = "upload",
                OperationType = StorageOperationType.Upload,
                IsSuccess = false,
                ResponseTime = TimeSpan.FromMilliseconds(100),
                Timestamp = now.AddMinutes(-4),
                ErrorType = "ConnectionException"
            },
            new() {
                OperationName = "download",
                OperationType = StorageOperationType.Download,
                IsSuccess = false,
                ResponseTime = TimeSpan.FromMilliseconds(150),
                Timestamp = now.AddMinutes(-3),
                ErrorType = "TimeoutException"
            },
            new() {
                OperationName = "delete",
                OperationType = StorageOperationType.Delete,
                IsSuccess = false,
                ResponseTime = TimeSpan.FromMilliseconds(200),
                Timestamp = now.AddMinutes(-2),
                ErrorType = "AuthenticationException"
            },
            new() {
                OperationName = "upload",
                OperationType = StorageOperationType.Upload,
                IsSuccess = false,
                ResponseTime = TimeSpan.FromMilliseconds(250),
                Timestamp = now.AddMinutes(-1),
                ErrorType = "StorageException"
            }
        };

        foreach (var metric in failedMetrics) {
            await _monitoringService.RecordOperationAsync(metric);
        }

        // Act
        var result = await _monitoringService.GetHealthStatusAsync();

        // Assert
        result.Should().NotBeNull();
        result.OverallStatus.Should().Be(HealthStatus.Unhealthy);
        result.SuccessRate.Should().BeLessThan(0.8);
        result.ErrorRate.Should().BeGreaterThan(0.2);
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public async Task GetHealthStatusAsync_SlowOperations_ShouldReturnDegraded() {
        // Arrange
        var now = DateTimeOffset.UtcNow;

        // Record slow but successful operations
        var slowMetrics = new List<StorageOperationMetric>
        {
            new() {
                OperationName = "upload",
                OperationType = StorageOperationType.Upload,
                IsSuccess = true,
                ResponseTime = TimeSpan.FromSeconds(35), // Exceeds MaxResponseTime threshold
                Timestamp = now.AddMinutes(-3)
            },
            new() {
                OperationName = "download",
                OperationType = StorageOperationType.Download,
                IsSuccess = true,
                ResponseTime = TimeSpan.FromSeconds(40), // Exceeds MaxResponseTime threshold
                Timestamp = now.AddMinutes(-2)
            }
        };

        foreach (var metric in slowMetrics) {
            await _monitoringService.RecordOperationAsync(metric);
        }

        // Act
        var result = await _monitoringService.GetHealthStatusAsync();

        // Assert
        result.Should().NotBeNull();
        result.OverallStatus.Should().Be(HealthStatus.Degraded);
        result.AverageResponseTime.Should().BeGreaterThan(_configuration.Monitoring.Thresholds.MaxResponseTime);
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public void StartOperationMonitoring_ShouldReturnMonitor() {
        // Arrange
        var operationName = "upload";
        var containerName = "test-container";
        var fileName = "test-file.txt";

        // Act
        using var monitor = _monitoringService.StartOperationMonitoring(operationName, containerName, fileName);

        // Assert
        monitor.Should().NotBeNull();
        monitor.Should().BeAssignableTo<IStorageOperationMonitor>();
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public async Task CheckThresholdsAsync_HighErrorRate_ShouldReturnAlert() {
        // Arrange
        var now = DateTimeOffset.UtcNow;

        // Record high error rate operations
        var metrics = new List<StorageOperationMetric>
        {
            new() { IsSuccess = false, ResponseTime = TimeSpan.FromMilliseconds(100), Timestamp = now.AddMinutes(-2) },
            new() { IsSuccess = false, ResponseTime = TimeSpan.FromMilliseconds(150), Timestamp = now.AddMinutes(-2) },
            new() { IsSuccess = false, ResponseTime = TimeSpan.FromMilliseconds(200), Timestamp = now.AddMinutes(-1) },
            new() { IsSuccess = true, ResponseTime = TimeSpan.FromMilliseconds(100), Timestamp = now.AddMinutes(-1) }
        };

        foreach (var metric in metrics) {
            await _monitoringService.RecordOperationAsync(metric);
        }

        // Act
        var alerts = await _monitoringService.CheckThresholdsAsync();

        // Assert
        alerts.Should().NotBeNull();
        alerts.Should().ContainSingle(a => a.AlertType == StorageAlertType.HighErrorRate);
        alerts.First(a => a.AlertType == StorageAlertType.HighErrorRate).Severity.Should().Be(AlertSeverity.Critical);
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public async Task CheckThresholdsAsync_HighResponseTime_ShouldReturnAlert() {
        // Arrange
        var now = DateTimeOffset.UtcNow;

        // Record slow operations
        var metrics = new List<StorageOperationMetric>
        {
            new() { IsSuccess = true, ResponseTime = TimeSpan.FromSeconds(35), Timestamp = now.AddMinutes(-2) },
            new() { IsSuccess = true, ResponseTime = TimeSpan.FromSeconds(40), Timestamp = now.AddMinutes(-1) }
        };

        foreach (var metric in metrics) {
            await _monitoringService.RecordOperationAsync(metric);
        }

        // Act
        var alerts = await _monitoringService.CheckThresholdsAsync();

        // Assert
        alerts.Should().NotBeNull();
        alerts.Should().ContainSingle(a => a.AlertType == StorageAlertType.HighResponseTime);
        alerts.First(a => a.AlertType == StorageAlertType.HighResponseTime).Severity.Should().Be(AlertSeverity.Warning);
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public async Task CheckThresholdsAsync_LowSuccessRate_ShouldReturnAlert() {
        // Arrange
        var now = DateTimeOffset.UtcNow;

        // Record operations with low success rate
        var metrics = new List<StorageOperationMetric>
        {
            new() { IsSuccess = false, ResponseTime = TimeSpan.FromMilliseconds(100), Timestamp = now.AddMinutes(-3) },
            new() { IsSuccess = false, ResponseTime = TimeSpan.FromMilliseconds(150), Timestamp = now.AddMinutes(-2) },
            new() { IsSuccess = false, ResponseTime = TimeSpan.FromMilliseconds(200), Timestamp = now.AddMinutes(-2) },
            new() { IsSuccess = true, ResponseTime = TimeSpan.FromMilliseconds(100), Timestamp = now.AddMinutes(-1) }
        };

        foreach (var metric in metrics) {
            await _monitoringService.RecordOperationAsync(metric);
        }

        // Act
        var alerts = await _monitoringService.CheckThresholdsAsync();

        // Assert
        alerts.Should().NotBeNull();
        alerts.Should().ContainSingle(a => a.AlertType == StorageAlertType.LowSuccessRate);
        alerts.First(a => a.AlertType == StorageAlertType.LowSuccessRate).Severity.Should().Be(AlertSeverity.Warning);
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public async Task GetUsageStatisticsAsync_ShouldReturnStatistics() {
        // Act
        var statistics = await _monitoringService.GetUsageStatisticsAsync();

        // Assert
        statistics.Should().NotBeNull();
        statistics.GeneratedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(10));
        statistics.ContainerStatistics.Should().NotBeNull();
    }
}

[TestClass]
[TestCategory(TestCategories.Unit)]
public class StorageOperationMonitorTests {
    private IStorageMonitoringService _monitoringService = null!;
    private IStorageOperationMonitor _monitor = null!;

    [TestInitialize]
    public void Setup() {
        _monitoringService = Substitute.For<IStorageMonitoringService>();
        _monitor = new StorageOperationMonitor(_monitoringService, "test-operation", "test-container", "test-file.txt");
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public void RecordSuccess_ShouldCallMonitoringService() {
        // Arrange
        var bytesTransferred = 1024L;

        // Act
        _monitor.RecordSuccess(bytesTransferred);

        // Assert
        _monitoringService.Received(1).RecordOperationAsync(
            Arg.Is<StorageOperationMetric>(m => m.IsSuccess && m.BytesTransferred == bytesTransferred),
            Arg.Any<CancellationToken>());
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public void RecordFailure_ShouldCallMonitoringService() {
        // Arrange
        var exception = new InvalidOperationException("Test error");

        // Act
        _monitor.RecordFailure(exception);

        // Assert
        _monitoringService.Received(1).RecordOperationAsync(
            Arg.Is<StorageOperationMetric>(m => !m.IsSuccess && m.ErrorType == "InvalidOperationException"),
            Arg.Any<CancellationToken>());
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public void RecordCompletion_Success_ShouldCallMonitoringService() {
        // Arrange
        var bytesTransferred = 2048L;

        // Act
        _monitor.RecordCompletion(true, bytesTransferred);

        // Assert
        _monitoringService.Received(1).RecordOperationAsync(
            Arg.Is<StorageOperationMetric>(m => m.IsSuccess && m.BytesTransferred == bytesTransferred),
            Arg.Any<CancellationToken>());
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public void RecordCompletion_Failure_ShouldCallMonitoringService() {
        // Arrange
        var exception = new TimeoutException("Operation timed out");

        // Act
        _monitor.RecordCompletion(false, null, exception);

        // Assert
        _monitoringService.Received(1).RecordOperationAsync(
            Arg.Is<StorageOperationMetric>(m => !m.IsSuccess && m.ErrorType == "TimeoutException"),
            Arg.Any<CancellationToken>());
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public void Dispose_ShouldNotThrow() {
        // Act & Assert
        var action = _monitor.Dispose;
        action.Should().NotThrow();
    }

    [TestCleanup]
    public void Cleanup() => _monitor?.Dispose();
}