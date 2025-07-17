namespace Data;

public class McpHubContext : DbContext
{
    public McpHubContext(DbContextOptions<McpHubContext> options) : base(options)
    {
    }
    
    public DbSet<Publisher> Publishers => Set<Publisher>();
    public DbSet<Server> Servers => Set<Server>();
    public DbSet<ServerVersion> ServerVersions => Set<ServerVersion>();
    public DbSet<Package> Packages => Set<Package>();
    public DbSet<PackageVersion> PackageVersions => Set<PackageVersion>();
    public DbSet<SecurityScan> SecurityScans => Set<SecurityScan>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(McpHubContext).Assembly);
    }
}