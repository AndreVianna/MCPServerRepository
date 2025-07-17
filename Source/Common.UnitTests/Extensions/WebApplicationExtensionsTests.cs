using Common.Extensions;
using Common.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using AwesomeAssertions;

namespace Common.UnitTests.Extensions;

public class WebApplicationExtensionsTests
{
    private readonly IServiceCollection _services;
    private readonly IConfiguration _configuration;

    public WebApplicationExtensionsTests()
    {
        _services = new ServiceCollection();
        
        var configurationBuilder = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Database:ConnectionString"] = "Host=localhost;Database=test;Username=user;Password=pass",
                ["Database:MaxRetryCount"] = "3",
                ["Database:CommandTimeout"] = "00:00:30",
                ["Database:MaxPoolSize"] = "100",
                ["Database:HealthCheckTimeout"] = "00:00:05",
                ["Cache:ConnectionString"] = "localhost:6379",
                ["Cache:DefaultExpiration"] = "00:30:00",
                ["Cache:SlidingExpiration"] = "00:05:00",
                ["Cache:KeyPrefix"] = "test:",
                ["Cache:HealthCheckTimeout"] = "00:00:05",
                ["Elasticsearch:ConnectionString"] = "http://localhost:9200",
                ["Elasticsearch:IndexPrefix"] = "test-",
                ["Elasticsearch:HealthCheckTimeout"] = "00:00:05",
                ["RabbitMQ:ConnectionString"] = "amqp://guest:guest@localhost:5672/",
                ["RabbitMQ:HealthCheckTimeout"] = "00:00:05",
                ["Observability:ServiceName"] = "MCPHub.Test",
                ["Observability:ServiceVersion"] = "1.0.0",
                ["Observability:Environment"] = "Test",
                ["Observability:OpenTelemetry:Sources:0"] = "MCPHub.*",
                ["Observability:Serilog:MinimumLevel"] = "Information",
                ["Observability:Serilog:LogTemplate"] = "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] {Message:lj}{NewLine}{Exception}"
            });
        
        _configuration = configurationBuilder.Build();
        
        // Add required services for testing
        _services.AddLogging();
    }

    [Fact]
    public void AddObservabilityServices_RegistersAllServices()
    {
        // Act
        _services.AddObservabilityServices();
        var serviceProvider = _services.BuildServiceProvider();

        // Assert
        var monitoringService = serviceProvider.GetService<IMonitoringService>();
        monitoringService.Should().NotBeNull();
        monitoringService.Should().BeOfType<MonitoringService>();

        var tracingService = serviceProvider.GetService<ITracingService>();
        tracingService.Should().NotBeNull();
        tracingService.Should().BeOfType<TracingService>();

        var healthCheckService = serviceProvider.GetService<HealthCheckService>();
        healthCheckService.Should().NotBeNull();
    }

    [Fact]
    public void AddObservabilityServices_RegistersHealthChecks()
    {
        // Act
        _services.AddObservabilityServices();
        var serviceProvider = _services.BuildServiceProvider();

        // Assert
        var healthCheckService = serviceProvider.GetRequiredService<HealthCheckService>();
        var healthReport = healthCheckService.CheckHealthAsync().Result;

        healthReport.Entries.Should().ContainKey("database");
        healthReport.Entries.Should().ContainKey("cache");
        healthReport.Entries.Should().ContainKey("search");
        healthReport.Entries.Should().ContainKey("messagequeue");
        healthReport.Entries.Should().ContainKey("application");
    }

    [Fact]
    public void AddApplicationServices_RegistersAllServices()
    {
        // Act
        _services.AddApplicationServices(_configuration);
        var serviceProvider = _services.BuildServiceProvider();

        // Assert
        var monitoringService = serviceProvider.GetService<IMonitoringService>();
        monitoringService.Should().NotBeNull();

        var tracingService = serviceProvider.GetService<ITracingService>();
        tracingService.Should().NotBeNull();

        var healthCheckService = serviceProvider.GetService<HealthCheckService>();
        healthCheckService.Should().NotBeNull();

        var mediator = serviceProvider.GetService<MediatR.IMediator>();
        mediator.Should().NotBeNull();

        var mapper = serviceProvider.GetService<AutoMapper.IMapper>();
        mapper.Should().NotBeNull();

        var validatorFactory = serviceProvider.GetService<FluentValidation.IValidatorFactory>();
        validatorFactory.Should().NotBeNull();
    }

    [Fact]
    public void AddApplicationServices_RegistersConfigurationOptions()
    {
        // Act
        _services.AddApplicationServices(_configuration);
        var serviceProvider = _services.BuildServiceProvider();

        // Assert
        var databaseOptions = serviceProvider.GetService<Microsoft.Extensions.Options.IOptions<Common.Configuration.DatabaseOptions>>();
        databaseOptions.Should().NotBeNull();
        databaseOptions!.Value.ConnectionString.Should().Be("Host=localhost;Database=test;Username=user;Password=pass");

        var cacheOptions = serviceProvider.GetService<Microsoft.Extensions.Options.IOptions<Common.Configuration.CacheOptions>>();
        cacheOptions.Should().NotBeNull();
        cacheOptions!.Value.ConnectionString.Should().Be("localhost:6379");

        var observabilityOptions = serviceProvider.GetService<Microsoft.Extensions.Options.IOptions<Common.Configuration.ObservabilityOptions>>();
        observabilityOptions.Should().NotBeNull();
        observabilityOptions!.Value.ServiceName.Should().Be("MCPHub.Test");
    }

    [Fact]
    public void AddObservabilityServices_RegistersServicesAsSingleton()
    {
        // Act
        _services.AddObservabilityServices();
        var serviceProvider = _services.BuildServiceProvider();

        // Assert
        var monitoringService1 = serviceProvider.GetService<IMonitoringService>();
        var monitoringService2 = serviceProvider.GetService<IMonitoringService>();

        monitoringService1.Should().BeSameAs(monitoringService2);
    }

    [Fact]
    public void AddObservabilityServices_RegistersTracingAsScoped()
    {
        // Act
        _services.AddObservabilityServices();
        var serviceProvider = _services.BuildServiceProvider();

        // Assert
        using var scope1 = serviceProvider.CreateScope();
        using var scope2 = serviceProvider.CreateScope();

        var tracingService1 = scope1.ServiceProvider.GetService<ITracingService>();
        var tracingService2 = scope1.ServiceProvider.GetService<ITracingService>();
        var tracingService3 = scope2.ServiceProvider.GetService<ITracingService>();

        tracingService1.Should().BeSameAs(tracingService2);
        tracingService1.Should().NotBeSameAs(tracingService3);
    }

    [Fact]
    public void AddObservabilityServices_DoesNotThrow()
    {
        // Act & Assert
        Action act = () => _services.AddObservabilityServices();
        act.Should().NotThrow();
    }

    [Fact]
    public void AddApplicationServices_DoesNotThrow()
    {
        // Act & Assert
        Action act = () => _services.AddApplicationServices(_configuration);
        act.Should().NotThrow();
    }

    [Fact]
    public void AddObservabilityServices_CanBeCalledMultipleTimes()
    {
        // Act & Assert
        Action act = () =>
        {
            _services.AddObservabilityServices();
            _services.AddObservabilityServices();
        };
        act.Should().NotThrow();
    }

    [Fact]
    public void AddApplicationServices_CanBeCalledMultipleTimes()
    {
        // Act & Assert
        Action act = () =>
        {
            _services.AddApplicationServices(_configuration);
            _services.AddApplicationServices(_configuration);
        };
        act.Should().NotThrow();
    }
}