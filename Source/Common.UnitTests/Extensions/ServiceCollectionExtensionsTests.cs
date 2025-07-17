using AwesomeAssertions;

using Common.Configuration;
using Common.Extensions;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Common.UnitTests.Extensions;

public class ServiceCollectionExtensionsTests {
    private readonly IServiceCollection _services;
    private readonly IConfiguration _configuration;

    public ServiceCollectionExtensionsTests() {
        _services = new ServiceCollection();

        var configurationBuilder = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> {
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
                ["Observability:ServiceName"] = "MCPHub.Test",
                ["Observability:ServiceVersion"] = "1.0.0",
                ["Observability:Environment"] = "Test",
                ["Observability:OpenTelemetry:Sources:0"] = "MCPHub.*",
                ["Observability:Serilog:MinimumLevel"] = "Information",
                ["Observability:Serilog:LogTemplate"] = "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] {Message:lj}{NewLine}{Exception}"
            });

        _configuration = configurationBuilder.Build();
    }

    [Fact]
    public void AddConfigurationOptions_RegistersAllOptions() {
        // Act
        _services.AddConfigurationOptions(_configuration);
        var serviceProvider = _services.BuildServiceProvider();

        // Assert
        var databaseOptions = serviceProvider.GetService<IOptions<DatabaseOptions>>();
        databaseOptions.Should().NotBeNull();
        databaseOptions!.Value.ConnectionString.Should().Be("Host=localhost;Database=test;Username=user;Password=pass");

        var cacheOptions = serviceProvider.GetService<IOptions<CacheOptions>>();
        cacheOptions.Should().NotBeNull();
        cacheOptions!.Value.ConnectionString.Should().Be("localhost:6379");

        var observabilityOptions = serviceProvider.GetService<IOptions<ObservabilityOptions>>();
        observabilityOptions.Should().NotBeNull();
        observabilityOptions!.Value.ServiceName.Should().Be("MCPHub.Test");
    }

    [Fact]
    public void AddConfigurationOptions_RegistersValidators() {
        // Act
        _services.AddConfigurationOptions(_configuration);
        var serviceProvider = _services.BuildServiceProvider();

        // Assert
        var databaseValidator = serviceProvider.GetService<IValidateOptions<DatabaseOptions>>();
        databaseValidator.Should().NotBeNull();
        databaseValidator.Should().BeOfType<DatabaseOptionsValidator>();

        var cacheValidator = serviceProvider.GetService<IValidateOptions<CacheOptions>>();
        cacheValidator.Should().NotBeNull();
        cacheValidator.Should().BeOfType<CacheOptionsValidator>();

        var observabilityValidator = serviceProvider.GetService<IValidateOptions<ObservabilityOptions>>();
        observabilityValidator.Should().NotBeNull();
        observabilityValidator.Should().BeOfType<ObservabilityOptionsValidator>();
    }

    [Fact]
    public void AddServicesFromAssembly_RegistersServicesCorrectly() {
        // Arrange
        var assembly = typeof(ServiceCollectionExtensions).Assembly;

        // Act
        _services.AddServicesFromAssembly(assembly);
        var serviceProvider = _services.BuildServiceProvider();

        // Assert
        var registrations = _services.Where(s => s.ServiceType.Name.EndsWith("Service")).ToList();
        registrations.Should().NotBeEmpty();
    }

    [Fact]
    public void AddMediatRFromAssembly_RegistersMediatR() {
        // Arrange
        var assembly = typeof(ServiceCollectionExtensionsTests).Assembly;

        // Act
        _services.AddMediatRFromAssembly(assembly);
        var serviceProvider = _services.BuildServiceProvider();

        // Assert
        var mediator = serviceProvider.GetService<MediatR.IMediator>();
        mediator.Should().NotBeNull();
    }

    [Fact]
    public void AddAutoMapperFromAssembly_RegistersAutoMapper() {
        // Arrange
        var assembly = typeof(ServiceCollectionExtensionsTests).Assembly;

        // Act
        _services.AddAutoMapperFromAssembly(assembly);
        var serviceProvider = _services.BuildServiceProvider();

        // Assert
        var mapper = serviceProvider.GetService<AutoMapper.IMapper>();
        mapper.Should().NotBeNull();
    }

    [Fact]
    public void AddFluentValidationFromAssembly_RegistersValidators() {
        // Arrange
        var assembly = typeof(ServiceCollectionExtensionsTests).Assembly;

        // Act
        _services.AddFluentValidationFromAssembly(assembly);
        var serviceProvider = _services.BuildServiceProvider();

        // Assert
        var validatorFactory = serviceProvider.GetService<FluentValidation.IValidatorFactory>();
        validatorFactory.Should().NotBeNull();
    }
}