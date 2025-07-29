using System.Diagnostics;

using MCPHub.CommandLineApp.Configuration;

using Spectre.Console;

namespace MCPHub.CommandLineApp.Services;

/// <summary>
/// Implementation of progress reporting using Spectre.Console
/// </summary>
public class ProgressReporter : IProgressReporter {
    private readonly McpmConfiguration _configuration;

    public ProgressReporter(McpmConfiguration configuration) {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    /// <inheritdoc />
    public IProgressContext CreateProgressBar(string description, double maxValue = 100) {
        if (!_configuration.Ui.ProgressBars) {
            return new NoOpProgressContext();
        }

        return new SimpleProgressContext(description, maxValue);
    }

    /// <inheritdoc />
    public IDownloadProgress CreateDownloadProgress(string fileName, long totalBytes) {
        if (!_configuration.Ui.ProgressBars) {
            return new NoOpDownloadProgress();
        }

        return new SimpleDownloadProgress(fileName, totalBytes);
    }

    /// <inheritdoc />
    public IStepProgress CreateStepProgress(string title, IEnumerable<string> steps) {
        if (!_configuration.Ui.ProgressBars) {
            return new NoOpStepProgress();
        }

        return new SimpleStepProgress(title, steps.ToList());
    }

    /// <inheritdoc />
    public ISpinnerContext CreateSpinner(string message) {
        if (!_configuration.Ui.ProgressBars) {
            return new NoOpSpinnerContext();
        }

        return new SimpleSpinnerContext(message);
    }

    /// <inheritdoc />
    public ILiveDisplayContext CreateLiveDisplay(string title) {
        if (!_configuration.Ui.ProgressBars) {
            return new NoOpLiveDisplayContext();
        }

        return new SimpleLiveDisplayContext(title);
    }

    /// <inheritdoc />
    public IProgressContext CreateProgress(string description) {
        if (!_configuration.Ui.ProgressBars) {
            return new NoOpProgressContext();
        }

        return new SimpleProgressContext(description, 100);
    }
}

// Simple implementations using basic console output and minimal Spectre.Console features
internal class SimpleProgressContext : IProgressContext {
    private readonly string _description;
    private readonly double _maxValue;
    private double _currentValue;
    private bool _disposed;

    public SimpleProgressContext(string description, double maxValue) {
        _description = description;
        _maxValue = maxValue;
        AnsiConsole.MarkupLine($"[blue]Started:[/] {_description}");
    }

    public void UpdateProgress(double value, string? message = null) {
        if (_disposed) return;
        
        _currentValue = value;
        var percentage = (int)((value / _maxValue) * 100);
        var statusText = message ?? _description;
        AnsiConsole.MarkupLine($"[blue]Progress:[/] {statusText} ({percentage}%)");
    }

    public void Complete(string? message = null) {
        if (_disposed) return;
        
        var statusText = message ?? $"{_description} completed";
        AnsiConsole.MarkupLine($"[green]✅ {statusText}[/]");
    }

    public void Fail(string message) {
        if (_disposed) return;
        
        AnsiConsole.MarkupLine($"[red]❌ {message}[/]");
    }

    public void UpdateStatus(string status) {
        if (_disposed) return;
        
        AnsiConsole.MarkupLine($"[blue]Status:[/] {status}");
    }

    public void Increment(double amount = 1.0) {
        if (_disposed) return;
        
        UpdateProgress(_currentValue + amount);
    }

    public void Dispose() {
        if (_disposed) return;
        _disposed = true;
    }
}

internal class SimpleDownloadProgress : IDownloadProgress {
    private readonly string _fileName;
    private readonly long _totalBytes;
    private readonly Stopwatch _stopwatch;
    private long _lastBytes;
    private DateTime _lastUpdate;
    private bool _disposed;

    public SimpleDownloadProgress(string fileName, long totalBytes) {
        _fileName = fileName;
        _totalBytes = totalBytes;
        _stopwatch = Stopwatch.StartNew();
        _lastUpdate = DateTime.UtcNow;
        AnsiConsole.MarkupLine($"[blue]Started downloading:[/] {_fileName} ({FormatBytes(_totalBytes)})");
    }

    public long? CurrentSpeed { get; private set; }
    public TimeSpan? EstimatedTimeRemaining { get; private set; }

    public void UpdateProgress(long bytesDownloaded, long? speed = null) {
        if (_disposed) return;

        // Calculate speed if not provided
        var now = DateTime.UtcNow;
        if (speed.HasValue) {
            CurrentSpeed = speed.Value;
        } else if ((now - _lastUpdate).TotalSeconds >= 1) {
            var bytesThisSecond = bytesDownloaded - _lastBytes;
            var secondsElapsed = (now - _lastUpdate).TotalSeconds;
            CurrentSpeed = (long)(bytesThisSecond / secondsElapsed);
            _lastBytes = bytesDownloaded;
            _lastUpdate = now;
        }

        // Calculate ETA
        if (CurrentSpeed > 0) {
            var remainingBytes = _totalBytes - bytesDownloaded;
            var secondsRemaining = remainingBytes / CurrentSpeed.Value;
            EstimatedTimeRemaining = TimeSpan.FromSeconds(secondsRemaining);
        }

        var percentage = (int)((bytesDownloaded * 100) / _totalBytes);
        var speedText = CurrentSpeed.HasValue ? $"{FormatBytes(CurrentSpeed.Value)}/s" : "";
        var etaText = EstimatedTimeRemaining.HasValue ? $"ETA: {EstimatedTimeRemaining.Value:mm\\:ss}" : "";
        
        AnsiConsole.MarkupLine($"[blue]Downloading:[/] {_fileName} {percentage}% {speedText} {etaText}".Trim());
    }

    public void UpdateStatus(string status) {
        if (_disposed) return;
        
        AnsiConsole.MarkupLine($"[yellow]{status}:[/] {_fileName}");
    }

    public void Complete(string? message = null) {
        if (_disposed) return;
        
        var statusText = message ?? $"Downloaded {_fileName}";
        AnsiConsole.MarkupLine($"[green]✅ {statusText}[/]");
    }

    public void Fail(string message) {
        if (_disposed) return;
        
        AnsiConsole.MarkupLine($"[red]❌ {message}[/]");
    }

    public void Dispose() {
        if (_disposed) return;
        
        _disposed = true;
        _stopwatch?.Stop();
    }

    private static string FormatBytes(long bytes) {
        var units = new[] { "B", "KB", "MB", "GB" };
        var size = (double)bytes;
        var unitIndex = 0;

        while (size >= 1024 && unitIndex < units.Length - 1) {
            size /= 1024;
            unitIndex++;
        }

        return $"{size:F1} {units[unitIndex]}";
    }
}