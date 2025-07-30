using Microsoft.Extensions.Caching.Distributed;

using Moq;

namespace MCPHub.PublicApi.UnitTests.Services;

public class SimpleRateLimitingServiceTests : IDisposable {
    private readonly Mock<IOptions<RateLimitingOptions>> _mockOptions;
    private readonly Mock<IDistributedCache> _mockCache;
    private readonly Mock<ILogger<RateLimitingService>> _mockLogger;
    private readonly RateLimitingOptions _options;
    private readonly RateLimitingService _service;

    public SimpleRateLimitingServiceTests() {
        _mockOptions = new Mock<IOptions<RateLimitingOptions>>();
        _mockCache = new Mock<IDistributedCache>();
        _mockLogger = new Mock<ILogger<RateLimitingService>>();

        _options = new RateLimitingOptions {
            Enabled = true,
            UseInMemory = true,
            Authentication = new EndpointRateLimitConfig {
                PermitLimit = 5,
                WindowMinutes = 1,
                QueueLimit = 0,
                PerIpLimiting = true,
                PerUserLimiting = false,
                Strategy = "FixedWindow",
            },
        };

        _mockOptions.Setup(x => x.Value).Returns(_options);
        _service = new RateLimitingService(_mockOptions.Object, _mockCache.Object, _mockLogger.Object);
    }

    public void Dispose() => _service?.Dispose();

    [Fact]
    public async Task CheckRateLimitAsync_WhenDisabled_ShouldAlwaysAllow() {
        // Arrange - Create a new service with disabled options
        var disabledOptions = new RateLimitingOptions { Enabled = false };
        var mockDisabledOptions = new Mock<IOptions<RateLimitingOptions>>();
        mockDisabledOptions.Setup(x => x.Value).Returns(disabledOptions);

        var disabledService = new RateLimitingService(mockDisabledOptions.Object, _mockCache.Object, _mockLogger.Object);
        var identifier = "192.168.1.1";
        var policy = "Authentication";

        try {
            // Act
            var result = await disabledService.CheckRateLimitAsync(identifier, policy);

            // Assert
            Assert.True(result.IsAllowed);
            Assert.Equal(long.MaxValue, result.RequestsRemaining);
        }
        finally {
            disabledService.Dispose();
        }
    }

    [Fact]
    public async Task CheckRateLimitAsync_FirstRequest_ShouldAllow() {
        // Arrange
        var identifier = "192.168.1.1";
        var policy = "Authentication";

        // Act
        var result = await _service.CheckRateLimitAsync(identifier, policy);

        // Assert
        Assert.True(result.IsAllowed);
        Assert.Equal(5, result.RequestsRemaining); // Permit limit is 5
    }

    [Fact]
    public async Task RecordRequestAsync_ShouldUpdateUsage() {
        // Arrange
        var identifier = "192.168.1.1";
        var policy = "Authentication";

        // Act
        await _service.RecordRequestAsync(identifier, policy);
        var usage = await _service.GetUsageAsync(identifier, policy);

        // Assert
        Assert.Equal(1, usage.RequestCount);
    }

    [Fact]
    public async Task GetUsageAsync_NoRequests_ShouldReturnZero() {
        // Arrange
        var identifier = "192.168.1.1";
        var policy = "Authentication";

        // Act
        var usage = await _service.GetUsageAsync(identifier, policy);

        // Assert
        Assert.Equal(0, usage.RequestCount);
    }

    [Fact]
    public async Task ResetAsync_ShouldClearUsage() {
        // Arrange
        var identifier = "192.168.1.1";
        var policy = "Authentication";

        // Act
        await _service.RecordRequestAsync(identifier, policy);
        await _service.ResetAsync(identifier, policy);
        var usage = await _service.GetUsageAsync(identifier, policy);

        // Assert
        Assert.Equal(0, usage.RequestCount);
    }

    [Fact]
    public async Task GetStatisticsAsync_ShouldReturnStatistics() {
        // Arrange
        var identifier = "192.168.1.1";
        var policy = "Authentication";

        // Act
        await _service.RecordRequestAsync(identifier, policy);
        var stats = await _service.GetStatisticsAsync();

        // Assert
        Assert.NotNull(stats);
        Assert.True(stats.TotalRequests > 0);
    }
}