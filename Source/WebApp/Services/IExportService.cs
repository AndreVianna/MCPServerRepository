using MCPHub.WebApp.Components.UI.Models;

namespace MCPHub.WebApp.Services;

/// <summary>
/// Supported export formats
/// </summary>
public enum ExportFormat
{
    Csv,
    Json,
    Pdf,
    Excel
}

/// <summary>
/// Export configuration options
/// </summary>
public class ExportOptions
{
    public required string FileName { get; set; }
    public ExportFormat Format { get; set; } = ExportFormat.Csv;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Dictionary<string, object> Metadata { get; set; } = new();
    public bool IncludeTimestamp { get; set; } = true;
    public bool IncludeCharts { get; set; } = false;
    public string? CustomTemplate { get; set; }
}

/// <summary>
/// Result of an export operation
/// </summary>
public class ExportResult
{
    public bool Success { get; set; }
    public string? FileName { get; set; }
    public byte[]? Data { get; set; }
    public string? ContentType { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime ExportedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Chart data for export
/// </summary>
public class ExportChartData
{
    public required string Title { get; set; }
    public string Description { get; set; } = string.Empty;
    public ChartType ChartType { get; set; }
    public List<ChartSeries> Series { get; set; } = new();
    public string[] XAxisLabels { get; set; } = Array.Empty<string>();
    public Dictionary<string, object> Options { get; set; } = new();
}

/// <summary>
/// Analytics data for export
/// </summary>
public class ExportAnalyticsData
{
    public required string Title { get; set; }
    public string Description { get; set; } = string.Empty;
    public string TimeRange { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public List<SummaryStat> Summary { get; set; } = new();
    public List<ExportChartData> Charts { get; set; } = new();
    public Dictionary<string, List<Dictionary<string, object>>> Tables { get; set; } = new();
    public Dictionary<string, object> RawData { get; set; } = new();
}

/// <summary>
/// Service for exporting analytics data in various formats
/// </summary>
public interface IExportService
{
    /// <summary>
    /// Exports analytics data in the specified format
    /// </summary>
    /// <param name="data">The analytics data to export</param>
    /// <param name="options">Export configuration options</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Export result with file data</returns>
    Task<ExportResult> ExportAnalyticsAsync(ExportAnalyticsData data, ExportOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Exports chart data in the specified format
    /// </summary>
    /// <param name="chartData">The chart data to export</param>
    /// <param name="options">Export configuration options</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Export result with file data</returns>
    Task<ExportResult> ExportChartAsync(ExportChartData chartData, ExportOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Exports tabular data in the specified format
    /// </summary>
    /// <param name="data">The tabular data to export</param>
    /// <param name="options">Export configuration options</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Export result with file data</returns>
    Task<ExportResult> ExportTableAsync(List<Dictionary<string, object>> data, ExportOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the supported formats for export
    /// </summary>
    /// <returns>List of supported export formats</returns>
    List<ExportFormat> GetSupportedFormats();

    /// <summary>
    /// Gets the content type for a given export format
    /// </summary>
    /// <param name="format">The export format</param>
    /// <returns>MIME content type</returns>
    string GetContentType(ExportFormat format);

    /// <summary>
    /// Gets the file extension for a given export format
    /// </summary>
    /// <param name="format">The export format</param>
    /// <returns>File extension with dot</returns>
    string GetFileExtension(ExportFormat format);
}