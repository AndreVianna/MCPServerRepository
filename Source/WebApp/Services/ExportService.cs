using System.Globalization;
using System.Text;
using System.Text.Json;

namespace MCPHub.WebApp.Services;

/// <summary>
/// Implementation of the export service that handles multiple export formats
/// </summary>
public class ExportService(ILogger<ExportService> logger) : IExportService {
    private readonly ILogger<ExportService> _logger = logger;

    public async Task<ExportResult> ExportAnalyticsAsync(ExportAnalyticsData data, ExportOptions options, CancellationToken cancellationToken = default) {
        try {
            _logger.LogInformation("Starting analytics export for {Title} in {Format} format", data.Title, options.Format);

            var result = options.Format switch {
                ExportFormat.Csv => await ExportAnalyticsToCsvAsync(data, options, cancellationToken),
                ExportFormat.Json => await ExportAnalyticsToJsonAsync(data, options, cancellationToken),
                ExportFormat.Pdf => await ExportAnalyticsToPdfAsync(data, options, cancellationToken),
                ExportFormat.Excel => await ExportAnalyticsToExcelAsync(data, options, cancellationToken),
                _ => new ExportResult { Success = false, ErrorMessage = $"Unsupported format: {options.Format}" }
            };

            if (result.Success) {
                _logger.LogInformation("Successfully exported analytics data for {Title}", data.Title);
            }
            else {
                _logger.LogWarning("Failed to export analytics data for {Title}: {Error}", data.Title, result.ErrorMessage);
            }

            return result;
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error exporting analytics data for {Title}", data.Title);
            return new ExportResult {
                Success = false,
                ErrorMessage = $"Export failed: {ex.Message}"
            };
        }
    }

    public async Task<ExportResult> ExportChartAsync(ExportChartData chartData, ExportOptions options, CancellationToken cancellationToken = default) {
        try {
            _logger.LogInformation("Starting chart export for {Title} in {Format} format", chartData.Title, options.Format);

            var result = options.Format switch {
                ExportFormat.Csv => await ExportChartToCsvAsync(chartData, options, cancellationToken),
                ExportFormat.Json => await ExportChartToJsonAsync(chartData, options, cancellationToken),
                ExportFormat.Pdf => await ExportChartToPdfAsync(chartData, options, cancellationToken),
                ExportFormat.Excel => await ExportChartToExcelAsync(chartData, options, cancellationToken),
                _ => new ExportResult { Success = false, ErrorMessage = $"Unsupported format: {options.Format}" }
            };

            if (result.Success) {
                _logger.LogInformation("Successfully exported chart data for {Title}", chartData.Title);
            }
            else {
                _logger.LogWarning("Failed to export chart data for {Title}: {Error}", chartData.Title, result.ErrorMessage);
            }

            return result;
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error exporting chart data for {Title}", chartData.Title);
            return new ExportResult {
                Success = false,
                ErrorMessage = $"Chart export failed: {ex.Message}"
            };
        }
    }

    public async Task<ExportResult> ExportTableAsync(List<Dictionary<string, object>> data, ExportOptions options, CancellationToken cancellationToken = default) {
        try {
            _logger.LogInformation("Starting table export with {RowCount} rows in {Format} format", data.Count, options.Format);

            var result = options.Format switch {
                ExportFormat.Csv => await ExportTableToCsvAsync(data, options, cancellationToken),
                ExportFormat.Json => await ExportTableToJsonAsync(data, options, cancellationToken),
                ExportFormat.Pdf => await ExportTableToPdfAsync(data, options, cancellationToken),
                ExportFormat.Excel => await ExportTableToExcelAsync(data, options, cancellationToken),
                _ => new ExportResult { Success = false, ErrorMessage = $"Unsupported format: {options.Format}" }
            };

            if (result.Success) {
                _logger.LogInformation("Successfully exported table data with {RowCount} rows", data.Count);
            }
            else {
                _logger.LogWarning("Failed to export table data: {Error}", result.ErrorMessage);
            }

            return result;
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error exporting table data");
            return new ExportResult {
                Success = false,
                ErrorMessage = $"Table export failed: {ex.Message}"
            };
        }
    }

    public List<ExportFormat> GetSupportedFormats() => Enum.GetValues<ExportFormat>().ToList();

    public string GetContentType(ExportFormat format) => format switch {
        ExportFormat.Csv => "text/csv",
        ExportFormat.Json => "application/json",
        ExportFormat.Pdf => "application/pdf",
        ExportFormat.Excel => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        _ => "application/octet-stream"
    };

    public string GetFileExtension(ExportFormat format) => format switch {
        ExportFormat.Csv => ".csv",
        ExportFormat.Json => ".json",
        ExportFormat.Pdf => ".pdf",
        ExportFormat.Excel => ".xlsx",
        _ => ".txt"
    };

    #region CSV Export Methods

    private async Task<ExportResult> ExportAnalyticsToCsvAsync(ExportAnalyticsData data, ExportOptions options, CancellationToken cancellationToken) {
        var csv = new StringBuilder();

        // Add header information
        if (options.IncludeTimestamp) {
            csv.AppendLine($"# {data.Title}");
            csv.AppendLine($"# Generated: {data.GeneratedAt:yyyy-MM-dd HH:mm:ss}");
            csv.AppendLine($"# Time Range: {data.TimeRange}");
            if (!string.IsNullOrEmpty(data.Description)) {
                csv.AppendLine($"# Description: {data.Description}");
            }
            csv.AppendLine();
        }

        // Export summary statistics
        if (data.Summary.Any()) {
            csv.AppendLine("# Summary Statistics");
            csv.AppendLine("Metric,Value,Change");
            foreach (var stat in data.Summary) {
                var change = stat.Change.HasValue ? stat.Change.Value.ToString("F2", CultureInfo.InvariantCulture) : "";
                csv.AppendLine($"\"{stat.Label}\",\"{stat.Value}\",\"{change}\"");
            }
            csv.AppendLine();
        }

        // Export chart data
        foreach (var chart in data.Charts) {
            csv.AppendLine($"# Chart: {chart.Title}");

            if (chart.XAxisLabels.Any() && chart.Series.Any()) {
                // Build header row
                var headers = new List<string> { "Category" };
                headers.AddRange(chart.Series.Select(s => s.Name));
                csv.AppendLine(string.Join(",", headers.Select(h => $"\"{h}\"")));

                // Build data rows
                for (var i = 0; i < chart.XAxisLabels.Length; i++) {
                    var row = new List<string> { $"\"{chart.XAxisLabels[i]}\"" };
                    foreach (var series in chart.Series) {
                        var value = i < series.Data.Length ? series.Data[i].ToString("F2", CultureInfo.InvariantCulture) : "0";
                        row.Add(value);
                    }
                    csv.AppendLine(string.Join(",", row));
                }
            }
            csv.AppendLine();
        }

        // Export tabular data
        foreach (var table in data.Tables) {
            csv.AppendLine($"# Table: {table.Key}");

            if (table.Value.Any()) {
                var headers = table.Value.First().Keys.ToArray();
                csv.AppendLine(string.Join(",", headers.Select(h => $"\"{h}\"")));

                foreach (var row in table.Value) {
                    var values = headers.Select(h => $"\"{row.GetValueOrDefault(h)?.ToString() ?? ""}\"");
                    csv.AppendLine(string.Join(",", values));
                }
            }
            csv.AppendLine();
        }

        var fileName = GetFileName(options.FileName, options.Format);
        var data_bytes = Encoding.UTF8.GetBytes(csv.ToString());

        return await Task.FromResult(new ExportResult {
            Success = true,
            FileName = fileName,
            Data = data_bytes,
            ContentType = GetContentType(options.Format)
        });
    }

    private async Task<ExportResult> ExportChartToCsvAsync(ExportChartData chartData, ExportOptions options, CancellationToken cancellationToken) {
        var csv = new StringBuilder();

        if (options.IncludeTimestamp) {
            csv.AppendLine($"# {chartData.Title}");
            csv.AppendLine($"# Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");
            if (!string.IsNullOrEmpty(chartData.Description)) {
                csv.AppendLine($"# Description: {chartData.Description}");
            }
            csv.AppendLine();
        }

        if (chartData.XAxisLabels.Any() && chartData.Series.Any()) {
            var headers = new List<string> { "Category" };
            headers.AddRange(chartData.Series.Select(s => s.Name));
            csv.AppendLine(string.Join(",", headers.Select(h => $"\"{h}\"")));

            for (var i = 0; i < chartData.XAxisLabels.Length; i++) {
                var row = new List<string> { $"\"{chartData.XAxisLabels[i]}\"" };
                foreach (var series in chartData.Series) {
                    var value = i < series.Data.Length ? series.Data[i].ToString("F2", CultureInfo.InvariantCulture) : "0";
                    row.Add(value);
                }
                csv.AppendLine(string.Join(",", row));
            }
        }

        var fileName = GetFileName(options.FileName, options.Format);
        var data_bytes = Encoding.UTF8.GetBytes(csv.ToString());

        return await Task.FromResult(new ExportResult {
            Success = true,
            FileName = fileName,
            Data = data_bytes,
            ContentType = GetContentType(options.Format)
        });
    }

    private async Task<ExportResult> ExportTableToCsvAsync(List<Dictionary<string, object>> data, ExportOptions options, CancellationToken cancellationToken) {
        var csv = new StringBuilder();

        if (options.IncludeTimestamp) {
            csv.AppendLine($"# {options.Title}");
            csv.AppendLine($"# Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");
            if (!string.IsNullOrEmpty(options.Description)) {
                csv.AppendLine($"# Description: {options.Description}");
            }
            csv.AppendLine();
        }

        if (data.Any()) {
            var headers = data.First().Keys.ToArray();
            csv.AppendLine(string.Join(",", headers.Select(h => $"\"{h}\"")));

            foreach (var row in data) {
                var values = headers.Select(h => $"\"{row.GetValueOrDefault(h)?.ToString() ?? ""}\"");
                csv.AppendLine(string.Join(",", values));
            }
        }

        var fileName = GetFileName(options.FileName, options.Format);
        var data_bytes = Encoding.UTF8.GetBytes(csv.ToString());

        return await Task.FromResult(new ExportResult {
            Success = true,
            FileName = fileName,
            Data = data_bytes,
            ContentType = GetContentType(options.Format)
        });
    }

    #endregion

    #region JSON Export Methods

    private async Task<ExportResult> ExportAnalyticsToJsonAsync(ExportAnalyticsData data, ExportOptions options, CancellationToken cancellationToken) {
        var exportData = new {
            metadata = new {
                title = data.Title,
                description = data.Description,
                timeRange = data.TimeRange,
                generatedAt = data.GeneratedAt,
                exportedAt = DateTime.UtcNow,
                format = "JSON",
                version = "1.0"
            },
            summary = data.Summary.Select(s => new {
                label = s.Label,
                value = s.Value,
                change = s.Change,
                color = s.Color.ToString()
            }).ToArray(),
            charts = data.Charts.Select(c => new {
                title = c.Title,
                description = c.Description,
                type = c.ChartType.ToString(),
                series = c.Series.Select(s => new {
                    name = s.Name,
                    data = s.Data
                }).ToArray(),
                xAxisLabels = c.XAxisLabels,
                options = c.Options
            }).ToArray(),
            tables = data.Tables,
            rawData = data.RawData
        };

        var jsonOptions = new JsonSerializerOptions {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(exportData, jsonOptions);
        var fileName = GetFileName(options.FileName, options.Format);
        var data_bytes = Encoding.UTF8.GetBytes(json);

        return await Task.FromResult(new ExportResult {
            Success = true,
            FileName = fileName,
            Data = data_bytes,
            ContentType = GetContentType(options.Format)
        });
    }

    private async Task<ExportResult> ExportChartToJsonAsync(ExportChartData chartData, ExportOptions options, CancellationToken cancellationToken) {
        var exportData = new {
            metadata = new {
                title = chartData.Title,
                description = chartData.Description,
                generatedAt = DateTime.UtcNow,
                format = "JSON",
                version = "1.0"
            },
            chart = new {
                title = chartData.Title,
                description = chartData.Description,
                type = chartData.ChartType.ToString(),
                series = chartData.Series.Select(s => new {
                    name = s.Name,
                    data = s.Data
                }).ToArray(),
                xAxisLabels = chartData.XAxisLabels,
                options = chartData.Options
            }
        };

        var jsonOptions = new JsonSerializerOptions {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(exportData, jsonOptions);
        var fileName = GetFileName(options.FileName, options.Format);
        var data_bytes = Encoding.UTF8.GetBytes(json);

        return await Task.FromResult(new ExportResult {
            Success = true,
            FileName = fileName,
            Data = data_bytes,
            ContentType = GetContentType(options.Format)
        });
    }

    private async Task<ExportResult> ExportTableToJsonAsync(List<Dictionary<string, object>> data, ExportOptions options, CancellationToken cancellationToken) {
        var exportData = new {
            metadata = new {
                title = options.Title,
                description = options.Description,
                generatedAt = DateTime.UtcNow,
                format = "JSON",
                version = "1.0",
                rowCount = data.Count
            },
            data
        };

        var jsonOptions = new JsonSerializerOptions {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(exportData, jsonOptions);
        var fileName = GetFileName(options.FileName, options.Format);
        var data_bytes = Encoding.UTF8.GetBytes(json);

        return await Task.FromResult(new ExportResult {
            Success = true,
            FileName = fileName,
            Data = data_bytes,
            ContentType = GetContentType(options.Format)
        });
    }

    #endregion

    #region PDF Export Methods (Basic Implementation)

    private async Task<ExportResult> ExportAnalyticsToPdfAsync(ExportAnalyticsData data, ExportOptions options, CancellationToken cancellationToken) {
        // Note: This is a basic implementation. In a production environment,
        // you would use a proper PDF library like iTextSharp, PdfSharp, or similar
        var textContent = GenerateTextReport(data);
        var fileName = GetFileName(options.FileName, options.Format);
        var data_bytes = Encoding.UTF8.GetBytes(textContent);

        return await Task.FromResult(new ExportResult {
            Success = true,
            FileName = fileName,
            Data = data_bytes,
            ContentType = "text/plain", // Would be "application/pdf" with proper PDF generation
            ErrorMessage = "PDF export implemented as text format. Upgrade to proper PDF library for production use."
        });
    }

    private async Task<ExportResult> ExportChartToPdfAsync(ExportChartData chartData, ExportOptions options, CancellationToken cancellationToken) {
        var textContent = GenerateChartTextReport(chartData);
        var fileName = GetFileName(options.FileName, options.Format);
        var data_bytes = Encoding.UTF8.GetBytes(textContent);

        return await Task.FromResult(new ExportResult {
            Success = true,
            FileName = fileName,
            Data = data_bytes,
            ContentType = "text/plain", // Would be "application/pdf" with proper PDF generation
            ErrorMessage = "PDF export implemented as text format. Upgrade to proper PDF library for production use."
        });
    }

    private async Task<ExportResult> ExportTableToPdfAsync(List<Dictionary<string, object>> data, ExportOptions options, CancellationToken cancellationToken) {
        var textContent = GenerateTableTextReport(data, options);
        var fileName = GetFileName(options.FileName, options.Format);
        var data_bytes = Encoding.UTF8.GetBytes(textContent);

        return await Task.FromResult(new ExportResult {
            Success = true,
            FileName = fileName,
            Data = data_bytes,
            ContentType = "text/plain", // Would be "application/pdf" with proper PDF generation
            ErrorMessage = "PDF export implemented as text format. Upgrade to proper PDF library for production use."
        });
    }

    #endregion

    #region Excel Export Methods (Basic Implementation)

    private async Task<ExportResult> ExportAnalyticsToExcelAsync(ExportAnalyticsData data, ExportOptions options, CancellationToken cancellationToken)
        // Note: This is a basic implementation. In a production environment,
        // you would use a proper Excel library like EPPlus, ClosedXML, or similar
        => await ExportAnalyticsToCsvAsync(data, options, cancellationToken);

    private async Task<ExportResult> ExportChartToExcelAsync(ExportChartData chartData, ExportOptions options, CancellationToken cancellationToken) => await ExportChartToCsvAsync(chartData, options, cancellationToken);

    private async Task<ExportResult> ExportTableToExcelAsync(List<Dictionary<string, object>> data, ExportOptions options, CancellationToken cancellationToken) => await ExportTableToCsvAsync(data, options, cancellationToken);

    #endregion

    #region Helper Methods

    private string GetFileName(string baseName, ExportFormat format) {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
        var extension = GetFileExtension(format);
        return $"{baseName}_{timestamp}{extension}";
    }

    private string GenerateTextReport(ExportAnalyticsData data) {
        var report = new StringBuilder();
        report.AppendLine($"{data.Title}");
        report.AppendLine(new string('=', data.Title.Length));
        report.AppendLine($"Generated: {data.GeneratedAt:yyyy-MM-dd HH:mm:ss}");
        report.AppendLine($"Time Range: {data.TimeRange}");

        if (!string.IsNullOrEmpty(data.Description)) {
            report.AppendLine($"Description: {data.Description}");
        }

        report.AppendLine();

        if (data.Summary.Any()) {
            report.AppendLine("SUMMARY STATISTICS");
            report.AppendLine(new string('-', 20));
            foreach (var stat in data.Summary) {
                var change = stat.Change.HasValue ? $" ({stat.Change.Value:+0.0%;-0.0%})" : "";
                report.AppendLine($"{stat.Label}: {stat.Value}{change}");
            }
            report.AppendLine();
        }

        foreach (var chart in data.Charts) {
            report.AppendLine($"CHART: {chart.Title.ToUpper()}");
            report.AppendLine(new string('-', chart.Title.Length + 7));

            if (!string.IsNullOrEmpty(chart.Description)) {
                report.AppendLine(chart.Description);
                report.AppendLine();
            }

            // Simple text representation of chart data
            for (var i = 0; i < chart.XAxisLabels.Length && i < 10; i++) // Limit to first 10 items
            {
                report.AppendLine($"{chart.XAxisLabels[i]}:");
                foreach (var series in chart.Series) {
                    var value = i < series.Data.Length ? series.Data[i] : 0;
                    report.AppendLine($"  {series.Name}: {value:F2}");
                }
            }
            report.AppendLine();
        }

        return report.ToString();
    }

    private string GenerateChartTextReport(ExportChartData chartData) {
        var report = new StringBuilder();
        report.AppendLine($"{chartData.Title}");
        report.AppendLine(new string('=', chartData.Title.Length));
        report.AppendLine($"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");

        if (!string.IsNullOrEmpty(chartData.Description)) {
            report.AppendLine($"Description: {chartData.Description}");
        }

        report.AppendLine($"Chart Type: {chartData.ChartType}");
        report.AppendLine();

        for (var i = 0; i < chartData.XAxisLabels.Length; i++) {
            report.AppendLine($"{chartData.XAxisLabels[i]}:");
            foreach (var series in chartData.Series) {
                var value = i < series.Data.Length ? series.Data[i] : 0;
                report.AppendLine($"  {series.Name}: {value:F2}");
            }
        }

        return report.ToString();
    }

    private string GenerateTableTextReport(List<Dictionary<string, object>> data, ExportOptions options) {
        var report = new StringBuilder();
        report.AppendLine($"{options.Title}");
        report.AppendLine(new string('=', options.Title.Length));
        report.AppendLine($"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");

        if (!string.IsNullOrEmpty(options.Description)) {
            report.AppendLine($"Description: {options.Description}");
        }

        report.AppendLine($"Total Rows: {data.Count}");
        report.AppendLine();

        if (data.Any()) {
            var headers = data.First().Keys.ToArray();

            // Simple table format
            report.AppendLine(string.Join(" | ", headers));
            report.AppendLine(new string('-', headers.Length * 15));

            foreach (var row in data.Take(100)) // Limit to first 100 rows
            {
                var values = headers.Select(h => (row.GetValueOrDefault(h)?.ToString() ?? "").PadRight(12));
                report.AppendLine(string.Join(" | ", values));
            }

            if (data.Count > 100) {
                report.AppendLine($"... and {data.Count - 100} more rows");
            }
        }

        return report.ToString();
    }

    #endregion
}