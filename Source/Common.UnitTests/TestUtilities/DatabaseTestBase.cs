namespace Common.UnitTests.TestUtilities;

/// <summary>
/// Base class for database-related tests providing in-memory database setup.
/// </summary>
public abstract class DatabaseTestBase : TestBase {
    /// <summary>
    /// Gets the database name for the test.
    /// </summary>
    protected virtual string DatabaseName => $"TestDb_{Guid.NewGuid():N}";

    /// <summary>
    /// Override this method to configure Entity Framework services for testing.
    /// </summary>
    protected override void ConfigureServices() {
        base.ConfigureServices();
        ConfigureDatabaseServices();
    }

    /// <summary>
    /// Configures database services for testing.
    /// Override this method to add specific DbContext configurations.
    /// </summary>
    protected virtual void ConfigureDatabaseServices() {
        // Base implementation - override in derived classes to add specific DbContext
    }

    /// <summary>
    /// Creates a test database context with in-memory database.
    /// </summary>
    /// <typeparam name="TContext">The DbContext type.</typeparam>
    /// <returns>A configured DbContext instance.</returns>
    protected TContext CreateInMemoryContext<TContext>() where TContext : class => GetService<TContext>();

    /// <summary>
    /// Seeds the database with test data.
    /// Override this method to add specific test data seeding.
    /// </summary>
    protected virtual async Task SeedDatabaseAsync()
        // Base implementation - override in derived classes
        => await Task.CompletedTask;

    /// <summary>
    /// Cleans up the test database.
    /// Override this method to add specific cleanup logic.
    /// </summary>
    protected virtual async Task CleanupDatabaseAsync()
        // Base implementation - override in derived classes
        => await Task.CompletedTask;

    /// <summary>
    /// Disposes the database test base and cleans up database resources.
    /// </summary>
    /// <param name="disposing">Whether the method is called from Dispose().</param>
    protected override void Dispose(bool disposing) {
        if (disposing) {
            // Cleanup database resources
            CleanupDatabaseAsync().GetAwaiter().GetResult();
        }

        base.Dispose(disposing);
    }
}