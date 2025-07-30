namespace MCPHub.WebApp.Components.UI.Models;

/// <summary>
/// Represents a package information model for UI components
/// </summary>
public class PackageInfo {
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string LatestVersion { get; set; }
    public string Version { get; set; } = string.Empty;
    public string? Description { get; set; }
    public required PublisherInfo Publisher { get; set; }
    public string SecurityGrade { get; set; } = "N/A";
    public double SecurityScore { get; set; }
    public TrustTier TrustTier { get; set; } = TrustTier.Unverified;
    public long WeeklyDownloads { get; set; }
    public double Rating { get; set; }
    public int RatingCount { get; set; }
    public DateTime LastUpdated { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<string> Tags { get; set; } = [];
    public string Category { get; set; } = string.Empty;
    public string License { get; set; } = string.Empty;
    public double TrustScore { get; set; }
    public bool IsTrending { get; set; }
    public bool IsStarred { get; set; }
    public SecurityScanResult? SecurityReport { get; set; }
    public TrustTierProgress? TrustProgress { get; set; }
    public double? AIRelevanceScore { get; set; }
    public bool IsFeatured { get; set; }
    public string? RepositoryUrl { get; set; }
    public string? DocumentationUrl { get; set; }
    public List<DependencyInfo> Dependencies { get; set; } = [];
    public List<VersionInfo> Versions { get; set; } = [];
    public string ReadmeContent { get; set; } = string.Empty;
    public string InstallCommand { get; set; } = string.Empty;
}

/// <summary>
/// Represents publisher information for UI components
/// </summary>
public class PublisherInfo {
    public required string Name { get; set; }
    public string? DisplayName { get; set; }
    public PublisherType Type { get; set; } = PublisherType.Individual;
    public bool IsVerified { get; set; }
    public int PackageCount { get; set; }
}

/// <summary>
/// Represents trust tier progression information
/// </summary>
public class TrustTierProgress {
    public bool CanAdvance { get; set; }
    public double CompletionPercentage { get; set; }
    public List<TrustTierRequirement> Requirements { get; set; } = [];
    public List<TrustTierRequirement> CompletedFactors { get; set; } = [];
    public List<TrustTierRequirement> PendingFactors { get; set; } = [];
}

/// <summary>
/// Represents a trust tier requirement
/// </summary>
public class TrustTierRequirement {
    public required string Name { get; set; }
    public string? Description { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsRequired { get; set; } = true;
    public double Weight { get; set; } = 1.0;
}

/// <summary>
/// Represents search suggestion data
/// </summary>
public class SearchSuggestion {
    public required string DisplayText { get; set; }
    public string Description { get; set; } = "";
    public SuggestionType Type { get; set; }
    public string? Url { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = [];
}

/// <summary>
/// Represents a search query with filtering options
/// </summary>
public class SearchQuery {
    public string Text { get; set; } = "";
    public string Category { get; set; } = "";
    public TrustTier? TrustTier { get; set; }
    public string SecurityGrade { get; set; } = "";
    public string Timeframe { get; set; } = "";
    public List<string> Tags { get; set; } = [];
    public bool IncludePrerelease { get; set; }
    public int PageSize { get; set; } = 20;
    public int PageNumber { get; set; } = 1;
}

/// <summary>
/// Represents chart data for analytics components
/// </summary>
public class ChartDataRow {
    public required string Period { get; set; }
    public double[] Values { get; set; } = Array.Empty<double>();
}

/// <summary>
/// Represents summary statistics for analytics
/// </summary>
public class SummaryStat {
    public required string Label { get; set; }
    public required string Value { get; set; }
    public Color Color { get; set; } = Color.Default;
    public double? Change { get; set; }
}

/// <summary>
/// Represents responsive layout configuration
/// </summary>
public class ResponsiveConfig {
    public bool ShowSidebar { get; set; } = true;
    public bool ShowHeader { get; set; } = true;
    public bool ShowFooter { get; set; } = true;
    public SidebarPosition SidebarPosition { get; set; } = SidebarPosition.Right;
    public int SidebarWidth { get; set; } = 300;
    public ContainerMaxWidth MaxWidth { get; set; } = ContainerMaxWidth.Large;
}

/// <summary>
/// Extension methods for UI components
/// </summary>
public static class ComponentExtensions {
    /// <summary>
    /// Truncates a string to the specified length and adds ellipsis if needed
    /// </summary>
    /// <param name="value">The string to truncate</param>
    /// <param name="maxLength">Maximum length of the string</param>
    /// <returns>Truncated string with ellipsis if needed</returns>
    public static string Truncate(this string? value, int maxLength) => string.IsNullOrEmpty(value) ? string.Empty : value.Length <= maxLength ? value : value[..(maxLength - 3)] + "...";

    /// <summary>
    /// Converts custom ChartSeries to MudBlazor ChartSeries
    /// </summary>
    /// <param name="series">Custom chart series list</param>
    /// <returns>MudBlazor chart series list</returns>
    public static List<MudBlazor.ChartSeries> ToMudBlazorChartSeries(this List<ChartSeries> series) => series.Select(s => new MudBlazor.ChartSeries {
        Name = s.Name,
        Data = s.Data,
                                                                                                                                                    }).ToList();

    /// <summary>
    /// Converts MudBlazor ChartSeries to custom ChartSeries
    /// </summary>
    /// <param name="series">MudBlazor chart series list</param>
    /// <returns>Custom chart series list</returns>
    public static List<ChartSeries> ToCustomChartSeries(this List<MudBlazor.ChartSeries> series) => series.Select(s => new ChartSeries {
        Name = s.Name,
        Data = s.Data,
                                                                                                                                       }).ToList();
}

/// <summary>
/// Represents chart series data for MudBlazor compatibility
/// </summary>
public class ChartSeries {
    public string Name { get; set; } = string.Empty;
    public double[] Data { get; set; } = Array.Empty<double>();
}

/// <summary>
/// Extension methods for export dialog functionality
/// </summary>
public static class ExportDialogExtensions {
    /// <summary>
    /// Shows an export dialog for analytics data
    /// </summary>
    /// <param name="dialogService">The dialog service</param>
    /// <param name="data">Analytics data to export</param>
    /// <param name="fileName">Default file name</param>
    /// <returns>Dialog result with export information</returns>
    public static Task<IDialogReference> ShowExportDialogAsync(
        this IDialogService dialogService,
        ExportAnalyticsData data,
        string fileName = "analytics_export") {
        var parameters = new DialogParameters {
            ["AnalyticsData"] = data,
            ["DefaultFileName"] = fileName,
            ["HasChartData"] = data.Charts.Any(),
            ["EstimatedSize"] = EstimateDataSize(data),
                                              };

        var options = new DialogOptions {
            MaxWidth = MaxWidth.Large,
            FullWidth = true,
            CloseButton = true,
            CloseOnEscapeKey = true,
                                        };

        return Task.FromResult(dialogService.Show<Analytics.ExportDialog>("Export Analytics Data", parameters, options));
    }

    /// <summary>
    /// Shows an export dialog for chart data
    /// </summary>
    /// <param name="dialogService">The dialog service</param>
    /// <param name="chartData">Chart data to export</param>
    /// <param name="fileName">Default file name</param>
    /// <returns>Dialog result with export information</returns>
    public static Task<IDialogReference> ShowExportDialogAsync(
        this IDialogService dialogService,
        ExportChartData chartData,
        string fileName = "chart_export") {
        var parameters = new DialogParameters {
            ["ChartData"] = chartData,
            ["DefaultFileName"] = fileName,
            ["HasChartData"] = true,
            ["EstimatedSize"] = EstimateChartSize(chartData),
                                              };

        var options = new DialogOptions {
            MaxWidth = MaxWidth.Large,
            FullWidth = true,
            CloseButton = true,
            CloseOnEscapeKey = true,
                                        };

        return Task.FromResult(dialogService.Show<Analytics.ExportDialog>("Export Chart Data", parameters, options));
    }

    /// <summary>
    /// Shows an export dialog for table data
    /// </summary>
    /// <param name="dialogService">The dialog service</param>
    /// <param name="tableData">Table data to export</param>
    /// <param name="title">Export title</param>
    /// <param name="fileName">Default file name</param>
    /// <returns>Dialog result with export information</returns>
    public static Task<IDialogReference> ShowExportDialogAsync(
        this IDialogService dialogService,
        List<Dictionary<string, object>> tableData,
        string title = "Table Export",
        string fileName = "table_export") {
        var parameters = new DialogParameters {
            ["TableData"] = tableData,
            ["DefaultFileName"] = fileName,
            ["HasChartData"] = false,
            ["EstimatedSize"] = EstimateTableSize(tableData),
                                              };

        var options = new DialogOptions {
            MaxWidth = MaxWidth.Large,
            FullWidth = true,
            CloseButton = true,
            CloseOnEscapeKey = true,
                                        };

        return Task.FromResult(dialogService.Show<Analytics.ExportDialog>(title, parameters, options));
    }

    private static long EstimateDataSize(ExportAnalyticsData data) {
        // Rough estimation based on data structure
        long size = 0;

        // Summary stats
        size += data.Summary.Count * 50;

        // Charts
        foreach (var chart in data.Charts) {
            size += chart.XAxisLabels.Length * 20;
            size += chart.Series.Sum(s => s.Data.Length * 8);
        }

        // Tables
        foreach (var table in data.Tables) {
            size += table.Value.Count * table.Value.FirstOrDefault()?.Count * 20 ?? 0;
        }

        return Math.Max(size, 1024); // Minimum 1KB
    }

    private static long EstimateChartSize(ExportChartData chartData) {
        long size = chartData.XAxisLabels.Length * 20;
        size += chartData.Series.Sum(s => s.Data.Length * 8);
        return Math.Max(size, 512);
    }

    private static long EstimateTableSize(List<Dictionary<string, object>> tableData) {
        if (!tableData.Any())
            return 512;

        var avgRowSize = tableData.First().Count * 20;
        return Math.Max(tableData.Count * avgRowSize, 512);
    }
}

/// <summary>
/// Chart drill-down event arguments
/// </summary>
public class ChartDrillDownEventArgs {
    public int SelectedIndex { get; set; }
    public string XAxisLabel { get; set; } = string.Empty;
    public double[] SeriesData { get; set; } = Array.Empty<double>();
    public string[] SeriesNames { get; set; } = Array.Empty<string>();
    public Dictionary<string, object> Metadata { get; set; } = [];
}

/// <summary>
/// Version information for package installations (placeholder)
/// </summary>
public class VersionInfo {
    public string Version { get; set; } = string.Empty;
    public DateTime ReleaseDate { get; set; }
    public bool IsStable { get; set; }
    public bool IsLatest { get; set; }
    public bool IsPrerelease { get; set; }
    public string Notes { get; set; } = string.Empty;
    public string SecurityGrade { get; set; } = string.Empty;
    public string[] Changes { get; set; } = Array.Empty<string>();
    public long Downloads { get; set; }
    public string Description { get; set; } = string.Empty;
    public double SecurityScore { get; set; }
    public int VulnerabilityCount { get; set; }
    public int DependencyCount { get; set; }
    public DateTime LastScanDate { get; set; }
    public long Size { get; set; }
    public List<DependencyInfo> Dependencies { get; set; } = [];
}

/// <summary>
/// Dependency information for packages (placeholder)
/// </summary>
public class DependencyInfo {
    public string Name { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsRequired { get; set; } = true;
    public string SecurityGrade { get; set; } = string.Empty;
    public double SecurityScore { get; set; }
    public string License { get; set; } = string.Empty;
    public string Publisher { get; set; } = string.Empty;
}

/// <summary>
/// Step change direction for wizard components (placeholder)
/// </summary>
public enum StepChangeDirection {
    Next,
    Previous,
}