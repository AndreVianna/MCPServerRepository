namespace Common.UnitTests.TestUtilities;

/// <summary>
/// Base class for all unit tests providing common setup and teardown functionality.
/// </summary>
public abstract class TestBase : IDisposable
{
    private bool _disposed = false;
    
    /// <summary>
    /// Service provider for dependency injection in tests.
    /// </summary>
    protected IServiceProvider ServiceProvider { get; private set; }
    
    /// <summary>
    /// Service collection for configuring dependencies.
    /// </summary>
    protected IServiceCollection Services { get; private set; }
    
    /// <summary>
    /// Logger factory for creating test loggers.
    /// </summary>
    protected ILoggerFactory LoggerFactory { get; private set; }
    
    /// <summary>
    /// Configuration for tests.
    /// </summary>
    protected IConfiguration Configuration { get; private set; }

    protected TestBase()
    {
        Services = new ServiceCollection();
        SetupConfiguration();
        SetupLogging();
        ConfigureServices();
        ServiceProvider = Services.BuildServiceProvider();
    }

    /// <summary>
    /// Override this method to configure additional services for testing.
    /// </summary>
    protected virtual void ConfigureServices()
    {
        // Base configuration - can be overridden
    }

    /// <summary>
    /// Creates a mock of the specified type using NSubstitute.
    /// </summary>
    /// <typeparam name="T">The type to mock.</typeparam>
    /// <returns>A mock instance of the specified type.</returns>
    protected T CreateMock<T>() where T : class
    {
        return Substitute.For<T>();
    }

    /// <summary>
    /// Creates a logger for the specified type.
    /// </summary>
    /// <typeparam name="T">The type to create a logger for.</typeparam>
    /// <returns>A logger instance.</returns>
    protected ILogger<T> CreateLogger<T>()
    {
        return LoggerFactory.CreateLogger<T>();
    }

    /// <summary>
    /// Gets a service from the service provider.
    /// </summary>
    /// <typeparam name="T">The service type.</typeparam>
    /// <returns>The service instance.</returns>
    protected T GetService<T>() where T : notnull
    {
        return ServiceProvider.GetRequiredService<T>();
    }

    /// <summary>
    /// Gets a service from the service provider or null if not found.
    /// </summary>
    /// <typeparam name="T">The service type.</typeparam>
    /// <returns>The service instance or null.</returns>
    protected T? GetServiceOrNull<T>() where T : class
    {
        return ServiceProvider.GetService<T>();
    }

    private void SetupConfiguration()
    {
        var configurationBuilder = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Environment"] = "Test",
                ["ConnectionStrings:Default"] = "Data Source=:memory:",
                ["Logging:LogLevel:Default"] = "Information"
            });

        Configuration = configurationBuilder.Build();
        Services.AddSingleton(Configuration);
    }

    private void SetupLogging()
    {
        LoggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });

        Services.AddSingleton(LoggerFactory);
        Services.AddLogging(builder => builder.AddConsole());
    }

    /// <summary>
    /// Disposes the test base and cleans up resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes the test base and cleans up resources.
    /// </summary>
    /// <param name="disposing">Whether the method is called from Dispose().</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            ServiceProvider?.Dispose();
            LoggerFactory?.Dispose();
            _disposed = true;
        }
    }
}