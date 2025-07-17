using Common.Services;
using Common.UnitTests.TestUtilities;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using StackExchange.Redis;

namespace Common.UnitTests.Services;

[TestCategory(TestCategories.Integration)]
public class RedisCacheServiceTests : TestBase
{
    private IDistributedCache _distributedCache = null!;
    private IConnectionMultiplexer _connectionMultiplexer = null!;
    private IDatabase _database = null!;
    private ILogger<RedisCacheService> _logger = null!;
    private RedisCacheService _cacheService = null!;

    protected override void ConfigureServices()
    {
        base.ConfigureServices();
        
        _distributedCache = Substitute.For<IDistributedCache>();
        _connectionMultiplexer = Substitute.For<IConnectionMultiplexer>();
        _database = Substitute.For<IDatabase>();
        _logger = Substitute.For<ILogger<RedisCacheService>>();
        
        _connectionMultiplexer.GetDatabase(Arg.Any<int>(), Arg.Any<object>()).Returns(_database);
        
        Services.AddSingleton(_distributedCache);
        Services.AddSingleton(_connectionMultiplexer);
        Services.AddSingleton(_logger);
        Services.AddSingleton<ICacheService, RedisCacheService>();
    }

    [SetUp]
    public void SetUp()
    {
        _cacheService = GetService<ICacheService>() as RedisCacheService;
    }

    [Test]
    public async Task GetAsync_WhenValueExists_ReturnsDeserializedValue()
    {
        // Arrange
        var testObject = new TestData { Name = "Test", Value = 42 };
        var serializedValue = """{"name":"Test","value":42}""";
        
        _distributedCache.GetStringAsync("test-key", Arg.Any<CancellationToken>())
            .Returns(serializedValue);

        // Act
        var result = await _cacheService.GetAsync<TestData>("test-key");

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Test");
        result.Value.Should().Be(42);
    }

    [Test]
    public async Task GetAsync_WhenValueDoesNotExist_ReturnsDefault()
    {
        // Arrange
        _distributedCache.GetStringAsync("non-existent-key", Arg.Any<CancellationToken>())
            .Returns((string?)null);

        // Act
        var result = await _cacheService.GetAsync<TestData>("non-existent-key");

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task GetAsync_WhenExceptionOccurs_ReturnsDefaultAndLogsError()
    {
        // Arrange
        _distributedCache.GetStringAsync("error-key", Arg.Any<CancellationToken>())
            .Throws(new InvalidOperationException("Test exception"));

        // Act
        var result = await _cacheService.GetAsync<TestData>("error-key");

        // Assert
        result.Should().BeNull();
        
        _logger.Received(1).LogError(
            Arg.Any<Exception>(),
            Arg.Is<string>(s => s.Contains("Error getting cache value for key: {Key}")),
            "error-key");
    }

    [Test]
    public async Task SetAsync_WithTimeSpan_SetsValueWithSlidingExpiration()
    {
        // Arrange
        var testObject = new TestData { Name = "Test", Value = 42 };
        var expiration = TimeSpan.FromMinutes(15);

        // Act
        await _cacheService.SetAsync("test-key", testObject, expiration);

        // Assert
        await _distributedCache.Received(1).SetStringAsync(
            "test-key",
            Arg.Is<string>(s => s.Contains("Test") && s.Contains("42")),
            Arg.Is<DistributedCacheEntryOptions>(options => options.AbsoluteExpiration.HasValue),
            Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task SetAsync_WithDateTimeOffset_SetsValueWithAbsoluteExpiration()
    {
        // Arrange
        var testObject = new TestData { Name = "Test", Value = 42 };
        var expiration = DateTimeOffset.UtcNow.AddMinutes(30);

        // Act
        await _cacheService.SetAsync("test-key", testObject, expiration);

        // Assert
        await _distributedCache.Received(1).SetStringAsync(
            "test-key",
            Arg.Is<string>(s => s.Contains("Test") && s.Contains("42")),
            Arg.Is<DistributedCacheEntryOptions>(options => options.AbsoluteExpiration == expiration),
            Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task SetAsync_WithoutExpiration_SetsValueWithDefaultSlidingExpiration()
    {
        // Arrange
        var testObject = new TestData { Name = "Test", Value = 42 };

        // Act
        await _cacheService.SetAsync("test-key", testObject);

        // Assert
        await _distributedCache.Received(1).SetStringAsync(
            "test-key",
            Arg.Is<string>(s => s.Contains("Test") && s.Contains("42")),
            Arg.Is<DistributedCacheEntryOptions>(options => options.SlidingExpiration.HasValue),
            Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task SetAsync_WhenExceptionOccurs_LogsErrorAndContinues()
    {
        // Arrange
        var testObject = new TestData { Name = "Test", Value = 42 };
        
        _distributedCache.SetStringAsync(
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<DistributedCacheEntryOptions>(),
            Arg.Any<CancellationToken>())
            .Throws(new InvalidOperationException("Test exception"));

        // Act
        var action = async () => await _cacheService.SetAsync("test-key", testObject);

        // Assert
        await action.Should().NotThrowAsync();
        
        _logger.Received(1).LogError(
            Arg.Any<Exception>(),
            Arg.Is<string>(s => s.Contains("Error setting cache value for key: {Key}")),
            "test-key");
    }

    [Test]
    public async Task RemoveAsync_CallsDistributedCacheRemove()
    {
        // Act
        await _cacheService.RemoveAsync("test-key");

        // Assert
        await _distributedCache.Received(1).RemoveAsync("test-key", Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task RemoveAsync_WhenExceptionOccurs_LogsErrorAndContinues()
    {
        // Arrange
        _distributedCache.RemoveAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Throws(new InvalidOperationException("Test exception"));

        // Act
        var action = async () => await _cacheService.RemoveAsync("test-key");

        // Assert
        await action.Should().NotThrowAsync();
        
        _logger.Received(1).LogError(
            Arg.Any<Exception>(),
            Arg.Is<string>(s => s.Contains("Error removing cache value for key: {Key}")),
            "test-key");
    }

    [Test]
    public async Task ExistsAsync_WhenKeyExists_ReturnsTrue()
    {
        // Arrange
        _database.KeyExistsAsync("test-key").Returns(true);

        // Act
        var result = await _cacheService.ExistsAsync("test-key");

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public async Task ExistsAsync_WhenKeyDoesNotExist_ReturnsFalse()
    {
        // Arrange
        _database.KeyExistsAsync("non-existent-key").Returns(false);

        // Act
        var result = await _cacheService.ExistsAsync("non-existent-key");

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public async Task ExistsAsync_WhenExceptionOccurs_ReturnsFalseAndLogsError()
    {
        // Arrange
        _database.KeyExistsAsync("error-key").Throws(new InvalidOperationException("Test exception"));

        // Act
        var result = await _cacheService.ExistsAsync("error-key");

        // Assert
        result.Should().BeFalse();
        
        _logger.Received(1).LogError(
            Arg.Any<Exception>(),
            Arg.Is<string>(s => s.Contains("Error checking cache existence for key: {Key}")),
            "error-key");
    }

    [Test]
    public async Task RemovePatternAsync_CallsDatabaseKeyDeleteWithMatchingKeys()
    {
        // Arrange
        var server = Substitute.For<IServer>();
        var endPoints = new EndPoint[] { new DnsEndPoint("localhost", 6379) };
        var keys = new RedisKey[] { "test:key1", "test:key2", "test:key3" };
        
        _connectionMultiplexer.GetEndPoints().Returns(endPoints);
        _connectionMultiplexer.GetServer(endPoints[0]).Returns(server);
        server.Keys(pattern: "test:*").Returns(keys);

        // Act
        await _cacheService.RemovePatternAsync("test:*");

        // Assert
        await _database.Received(1).KeyDeleteAsync(Arg.Is<RedisKey[]>(k => k.Length == 3), Arg.Any<CommandFlags>());
    }

    private class TestData
    {
        public string Name { get; set; } = string.Empty;
        public int Value { get; set; }
    }
}