namespace Common.Configuration;

public class DatabaseOptions {
    public const string SectionName = "Database";

    public string ConnectionString { get; set; } = string.Empty;
    public int MaxRetryCount { get; set; } = 3;
    public TimeSpan CommandTimeout { get; set; } = TimeSpan.FromSeconds(30);
    public bool EnableSensitiveDataLogging { get; set; } = false;
    public bool EnableDetailedErrors { get; set; } = false;
    public int MaxPoolSize { get; set; } = 100;
    public TimeSpan HealthCheckTimeout { get; set; } = TimeSpan.FromSeconds(5);
}