using Microsoft.EntityFrameworkCore.Design;

namespace MCPHub.Data;

/// <summary>
/// Factory for creating DbContext instances at design time for EF migrations
/// </summary>
public class McpHubContextFactory : IDesignTimeDbContextFactory<McpHubContext> {
    public McpHubContext CreateDbContext(string[] args) {
        var optionsBuilder = new DbContextOptionsBuilder<McpHubContext>();

        // Use a default connection string for migrations
        // In production, this will be overridden by configuration
        optionsBuilder.UseNpgsql("Host=localhost;Database=mcphub_design;Username=postgres;Password=postgres");

        return new McpHubContext(optionsBuilder.Options);
    }
}