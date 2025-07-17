namespace Common.Configuration;

public class CacheOptions
{
    public const string SectionName = "Cache";

    public string ConnectionString { get; set; } = string.Empty;
    public TimeSpan DefaultExpiration { get; set; } = TimeSpan.FromMinutes(30);
    public TimeSpan SlidingExpiration { get; set; } = TimeSpan.FromMinutes(5);
    public int Database { get; set; } = 0;
    public string KeyPrefix { get; set; } = "mcphub:";
    public bool EnableCompression { get; set; } = true;
    public TimeSpan HealthCheckTimeout { get; set; } = TimeSpan.FromSeconds(5);
}