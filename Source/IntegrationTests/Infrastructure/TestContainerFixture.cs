namespace MCPHub.IntegrationTests.Infrastructure;

/// <summary>
/// Test container fixture for managing database and cache containers across test runs
/// </summary>
public class TestContainerFixture : IAsyncLifetime {
    public PostgreSqlContainer PostgreSqlContainer { get; private set; } = null!;
    public RedisContainer RedisContainer { get; private set; } = null!;
    public TestWebApplicationFactory<Program> WebApplicationFactory { get; private set; } = null!;
    public TestWebApplicationFactory<WebApp.Program> WebAppFactory { get; private set; } = null!;

    public async Task InitializeAsync() {
        // Start PostgreSQL container
        PostgreSqlContainer = new PostgreSqlBuilder()
            .WithDatabase("mcphub_test")
            .WithUsername("testuser")
            .WithPassword("testpass")
            .WithPortBinding(0, true) // Use random port
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5432))
            .WithCleanUp(true)
            .Build();

        await PostgreSqlContainer.StartAsync();

        // Start Redis container
        RedisContainer = new RedisBuilder()
            .WithPortBinding(0, true) // Use random port
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(6379))
            .WithCleanUp(true)
            .Build();

        await RedisContainer.StartAsync();

        // Create test application factories
        WebApplicationFactory = new TestWebApplicationFactory<Program>(PostgreSqlContainer, RedisContainer);
        WebAppFactory = new TestWebApplicationFactory<WebApp.Program>(PostgreSqlContainer, RedisContainer);

        // Initialize databases
        await WebApplicationFactory.InitializeDatabaseAsync();
        await WebAppFactory.InitializeDatabaseAsync();
    }

    public async Task DisposeAsync() {
        WebApplicationFactory?.Dispose();
        WebAppFactory?.Dispose();

        if (PostgreSqlContainer != null)
            await PostgreSqlContainer.DisposeAsync();

        if (RedisContainer != null)
            await RedisContainer.DisposeAsync();
    }
}

/// <summary>
/// Collection definition for sharing test containers across test classes
/// </summary>
[CollectionDefinition("TestContainer")]
public class TestContainerCollection : ICollectionFixture<TestContainerFixture> {
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
