using System.Security.Claims;
using System.Text.Encodings.Web;

using MCPHub.Common.Services;

namespace MCPHub.IntegrationTests.Infrastructure;

/// <summary>
/// Custom WebApplicationFactory for integration testing with test database and authentication
/// </summary>
/// <typeparam name="TProgram">The program type to test</typeparam>
public class TestWebApplicationFactory<TProgram>(PostgreSqlContainer dbContainer, RedisContainer redisContainer) : WebApplicationFactory<TProgram> where TProgram : class {
    private readonly PostgreSqlContainer _dbContainer = dbContainer ?? throw new ArgumentNullException(nameof(dbContainer));
    private readonly RedisContainer _redisContainer = redisContainer ?? throw new ArgumentNullException(nameof(redisContainer));

    public HttpClient CreateClientWithAuth(string userId = "test-user-id", string email = "test@example.com", IEnumerable<string>? roles = null) {
        var client = WithWebHostBuilder(builder => builder.ConfigureServices(services => {
            // Add test authentication
            services.AddAuthentication("Test")
                .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>("Test", options => { });

            services.AddScoped<IAuthorizationHandler, TestAuthorizationHandler>();
        })).CreateClient();

        // Add test user claims to headers
        client.DefaultRequestHeaders.Add("test-user-id", userId);
        client.DefaultRequestHeaders.Add("test-user-email", email);
        client.DefaultRequestHeaders.Add("test-user-roles", string.Join(",", roles ?? new[] { "User" }));

        return client;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder) {
        builder.ConfigureServices(services => {
            // Remove existing DbContext registration
            var dbContextDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<McpHubContext>));
            if (dbContextDescriptor != null)
                services.Remove(dbContextDescriptor);

            var dbContextServiceDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(McpHubContext));
            if (dbContextServiceDescriptor != null)
                services.Remove(dbContextServiceDescriptor);

            // Add test database
            services.AddDbContext<McpHubContext>(options => {
                options.UseNpgsql(_dbContainer.GetConnectionString());
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            });

            // Configure test Redis cache
            services.AddStackExchangeRedisCache(options => options.Configuration = _redisContainer.GetConnectionString());

            // Add test logging
            services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LoggingLogLevel.Debug));

            // Override external services with test doubles
            services.AddSingleton<IEmailService>(provider => Substitute.For<IEmailService>());
            services.AddSingleton<IStorageService>(provider => Substitute.For<IStorageService>());
        });

        builder.UseEnvironment("Testing");
    }

    public async Task InitializeDatabaseAsync() {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<McpHubContext>();

        await context.Database.EnsureCreatedAsync();

        // Seed test data if needed
        await SeedTestDataAsync(context);
    }

    private static async Task SeedTestDataAsync(McpHubContext context) {
        // Only seed if no data exists
        if (await context.Packages.AnyAsync())
            return;

        var testDataBuilder = new TestDataBuilder();
        var testData = testDataBuilder.CreateTestPackages(10);

        context.Packages.AddRange(testData);
        await context.SaveChangesAsync();
    }

    protected override void Dispose(bool disposing) {
        if (disposing) {
            // Containers are managed by the test collection fixture
        }
        base.Dispose(disposing);
    }
}

/// <summary>
/// Test authentication handler for simulating authenticated users
/// </summary>
public class TestAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger, UrlEncoder encoder) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder) {
    protected override Task<AuthenticateResult> HandleAuthenticateAsync() {
        var userId = Context.Request.Headers["test-user-id"].FirstOrDefault();
        var userEmail = Context.Request.Headers["test-user-email"].FirstOrDefault();
        var userRoles = Context.Request.Headers["test-user-roles"].FirstOrDefault();

        if (string.IsNullOrEmpty(userId)) {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId),
            new(ClaimTypes.Email, userEmail ?? "test@example.com"),
            new("sub", userId),
            new("id", userId),
        };

        if (!string.IsNullOrEmpty(userRoles)) {
            foreach (var role in userRoles.Split(',')) {
                claims.Add(new Claim(ClaimTypes.Role, role.Trim()));
            }
        }

        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "Test");

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}

/// <summary>
/// Test authorization handler that allows all operations for testing
/// </summary>
public class TestAuthorizationHandler : IAuthorizationHandler {
    public Task HandleAsync(AuthorizationHandlerContext context) {
        foreach (var requirement in context.Requirements) {
            context.Succeed(requirement);
        }
        return Task.CompletedTask;
    }
}