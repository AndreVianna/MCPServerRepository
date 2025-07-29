using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;

namespace MCPHub.Common.Configuration;

public class ObservabilityOptions {
    public const string SectionName = "Observability";

    [Required(ErrorMessage = "Service name is required")]
    public string ServiceName { get; set; } = "MCPHub";
    [Required(ErrorMessage = "Service version is required")]
    public string ServiceVersion { get; set; } = "1.0.0";
    [Required(ErrorMessage = "Environment is required")]
    public string Environment { get; set; } = "Development";
    
    // OpenTelemetry Configuration
    public OpenTelemetryOptions OpenTelemetry { get; set; } = new();
    
    // Metrics Configuration
    public MetricsOptions Metrics { get; set; } = new();
    
    // Tracing Configuration
    public TracingOptions Tracing { get; set; } = new();
    
    // Logging Configuration
    public LoggingOptions Logging { get; set; } = new();
    
    // Alerting Configuration
    public AlertingOptions Alerting { get; set; } = new();
}

public class OpenTelemetryOptions
{
    public bool Enabled { get; set; } = true;
    public string? OtlpEndpoint { get; set; }
    public string? ApiKey { get; set; }
    public Dictionary<string, string> Headers { get; set; } = new();
    public TimeSpan ExportTimeout { get; set; } = TimeSpan.FromSeconds(30);
    public int MaxExportBatchSize { get; set; } = 512;
    public TimeSpan ExportInterval { get; set; } = TimeSpan.FromSeconds(5);
}

public class MetricsOptions
{
    public bool Enabled { get; set; } = true;
    public string PrometheusEndpoint { get; set; } = "/metrics";
    public TimeSpan CollectionInterval { get; set; } = TimeSpan.FromSeconds(15);
    public bool EnableCustomMetrics { get; set; } = true;
    public bool EnableBusinessMetrics { get; set; } = true;
    public bool EnableInfrastructureMetrics { get; set; } = true;
}

public class TracingOptions
{
    public bool Enabled { get; set; } = true;
    public double SamplingRatio { get; set; } = 1.0; // 100% sampling for development
    public bool EnableSqlInstrumentation { get; set; } = true;
    public bool EnableHttpInstrumentation { get; set; } = true;
    public bool EnableRedisInstrumentation { get; set; } = true;
    public bool EnableElasticsearchInstrumentation { get; set; } = true;
    public List<string> IgnoredPaths { get; set; } = new() { "/health", "/metrics" };
}

public class LoggingOptions
{
    public bool EnableStructuredLogging { get; set; } = true;
    public bool EnableCorrelationIds { get; set; } = true;
    public bool EnableElasticSearch { get; set; } = false; // Disabled by default for development
    public string ElasticSearchUrl { get; set; } = "http://localhost:9200";
    public string LogstashUrl { get; set; } = "http://localhost:5044";
    public TimeSpan LogRetentionPeriod { get; set; } = TimeSpan.FromDays(30);
    public LogLevel MinimumLogLevel { get; set; } = LogLevel.Information;
}

public class AlertingOptions
{
    public bool Enabled { get; set; } = false; // Disabled by default for development
    public string? PagerDutyApiKey { get; set; }
    public string? SlackWebhookUrl { get; set; }
    public string? EmailSmtpServer { get; set; }
    public string? EmailFromAddress { get; set; }
    public List<string> DefaultRecipients { get; set; } = new();
    public TimeSpan AlertCooldownPeriod { get; set; } = TimeSpan.FromMinutes(15);
}