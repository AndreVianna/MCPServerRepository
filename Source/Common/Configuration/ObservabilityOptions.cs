namespace Common.Configuration;

public class ObservabilityOptions {
    public const string SectionName = "Observability";

    public string ServiceName { get; set; } = "MCPHub";
    public string ServiceVersion { get; set; } = "1.0.0";
    public string Environment { get; set; } = "Development";
    public OpenTelemetryOptions OpenTelemetry { get; set; } = new();
    public SerilogOptions Serilog { get; set; } = new();
}

public class OpenTelemetryOptions {
    public bool EnableTracing { get; set; } = true;
    public bool EnableMetrics { get; set; } = true;
    public bool EnableLogging { get; set; } = true;
    public string OtlpEndpoint { get; set; } = "http://localhost:4317";
    public string[] Sources { get; set; } = ["MCPHub.*"];
    public bool EnableConsoleExporter { get; set; } = true;
    public bool EnableOtlpExporter { get; set; } = false;
}

public class SerilogOptions {
    public string MinimumLevel { get; set; } = "Information";
    public bool EnableConsole { get; set; } = true;
    public bool EnableFile { get; set; } = true;
    public string LogDirectory { get; set; } = "logs";
    public string LogTemplate { get; set; } =
        "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] {Message:lj}{NewLine}{Exception}";
    public Dictionary<string, string> MinimumLevelOverrides { get; set; } = new()
    {
        { "Microsoft", "Warning" },
        { "System", "Warning" }
    };
}