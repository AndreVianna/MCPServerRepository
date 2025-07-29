using MudBlazor;

namespace MCPHub.WebApp.Components.UI.Models;

/// <summary>
/// Utility class for converting between MudBlazor chart types and custom chart types
/// </summary>
public static class ChartTypeConverter
{
    /// <summary>
    /// Converts MudBlazor ChartType to custom ChartType
    /// </summary>
    /// <param name="mudBlazorType">MudBlazor chart type</param>
    /// <returns>Custom chart type</returns>
    public static ChartType FromMudBlazor(MudBlazor.ChartType mudBlazorType)
    {
        return mudBlazorType switch
        {
            MudBlazor.ChartType.Line => ChartType.Line,
            MudBlazor.ChartType.Bar => ChartType.Bar,
            MudBlazor.ChartType.Donut => ChartType.Donut,
            MudBlazor.ChartType.Pie => ChartType.Pie,
            _ => ChartType.Line
        };
    }

    /// <summary>
    /// Converts custom ChartType to MudBlazor ChartType
    /// </summary>
    /// <param name="customType">Custom chart type</param>
    /// <returns>MudBlazor chart type</returns>
    public static MudBlazor.ChartType ToMudBlazor(ChartType customType)
    {
        return customType switch
        {
            ChartType.Line => MudBlazor.ChartType.Line,
            ChartType.Bar => MudBlazor.ChartType.Bar,
            ChartType.Donut => MudBlazor.ChartType.Donut,
            ChartType.Pie => MudBlazor.ChartType.Pie,
            ChartType.Area => MudBlazor.ChartType.Line, // MudBlazor doesn't have Area, use Line
            _ => MudBlazor.ChartType.Line
        };
    }
}

/// <summary>
/// Utility class for converting between MudBlazor chart series and custom chart series
/// </summary>
public static class ChartSeriesConverter
{
    /// <summary>
    /// Converts MudBlazor ChartSeries list to custom ChartSeries list
    /// </summary>
    /// <param name="mudBlazorSeries">MudBlazor chart series list</param>
    /// <returns>Custom chart series list</returns>
    public static List<ChartSeries> FromMudBlazor(List<MudBlazor.ChartSeries> mudBlazorSeries)
    {
        return mudBlazorSeries.Select(series => new ChartSeries
        {
            Name = series.Name,
            Data = series.Data
        }).ToList();
    }

    /// <summary>
    /// Converts custom ChartSeries list to MudBlazor ChartSeries list
    /// </summary>
    /// <param name="customSeries">Custom chart series list</param>
    /// <returns>MudBlazor chart series list</returns>
    public static List<MudBlazor.ChartSeries> ToMudBlazor(List<ChartSeries> customSeries)
    {
        return customSeries.Select(series => new MudBlazor.ChartSeries
        {
            Name = series.Name,
            Data = series.Data
        }).ToList();
    }

    /// <summary>
    /// Converts custom ChartSeries array to MudBlazor ChartSeries list
    /// </summary>
    /// <param name="customSeries">Custom chart series array</param>
    /// <returns>MudBlazor chart series list</returns>
    public static List<MudBlazor.ChartSeries> ToMudBlazor(ChartSeries[] customSeries)
    {
        return customSeries.Select(series => new MudBlazor.ChartSeries
        {
            Name = series.Name,
            Data = series.Data
        }).ToList();
    }
}