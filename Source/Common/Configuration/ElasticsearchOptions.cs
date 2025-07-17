namespace Common.Configuration;

public class ElasticsearchOptions
{
    public const string SectionName = "Elasticsearch";

    public string ConnectionString { get; set; } = string.Empty;
    public string IndexPrefix { get; set; } = "mcphub-";
    public int MaxRetryCount { get; set; } = 3;
    public TimeSpan RequestTimeout { get; set; } = TimeSpan.FromSeconds(30);
    public int MaxDegreeOfParallelism { get; set; } = 10;
    public bool EnableDebugMode { get; set; } = false;
    public TimeSpan HealthCheckTimeout { get; set; } = TimeSpan.FromSeconds(5);
}