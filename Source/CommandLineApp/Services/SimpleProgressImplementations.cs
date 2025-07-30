using MCPHub.CommandLineApp.Configuration;

using Spectre.Console;

namespace MCPHub.CommandLineApp.Services;

/// <summary>
/// Simple implementations of progress interfaces using basic console output
/// </summary>
internal class SimpleStepProgress : IStepProgress {
    private readonly string _title;
    private readonly List<string> _steps;
    private int _currentStep = -1;
    private bool _disposed;

    public SimpleStepProgress(string title, List<string> steps) {
        _title = title;
        _steps = steps;

        AnsiConsole.MarkupLine($"[blue]📋 {title}[/]");
        for (var i = 0; i < _steps.Count; i++) {
            AnsiConsole.MarkupLine($"  {i + 1}. [gray]⏳ {_steps[i]}[/]");
        }
    }

    public void StartStep(int stepIndex, string? message = null) {
        if (_disposed || stepIndex < 0 || stepIndex >= _steps.Count)
            return;

        _currentStep = stepIndex;
        var stepText = message ?? _steps[stepIndex];
        AnsiConsole.MarkupLine($"[yellow]🔄 Step {stepIndex + 1}: {stepText}[/]");
    }

    public void CompleteStep(int stepIndex, string? message = null) {
        if (_disposed || stepIndex < 0 || stepIndex >= _steps.Count)
            return;

        var stepText = message ?? _steps[stepIndex];
        AnsiConsole.MarkupLine($"[green]✅ Step {stepIndex + 1}: {stepText}[/]");
    }

    public void FailStep(int stepIndex, string message) {
        if (_disposed || stepIndex < 0 || stepIndex >= _steps.Count)
            return;

        AnsiConsole.MarkupLine($"[red]❌ Step {stepIndex + 1}: {message}[/]");
    }

    public void SkipStep(int stepIndex, string reason) {
        if (_disposed || stepIndex < 0 || stepIndex >= _steps.Count)
            return;

        AnsiConsole.MarkupLine($"[yellow]⏭️ Step {stepIndex + 1}: Skipped - {reason}[/]");
    }

    public void UpdateStatus(string message) {
        if (_disposed || _currentStep < 0)
            return;

        AnsiConsole.MarkupLine($"[blue]  💬 {message}[/]");
    }

    public void CompleteAll(string? message = null) {
        if (_disposed)
            return;

        var statusText = message ?? $"{_title} - All steps completed";
        AnsiConsole.MarkupLine($"[green]🎉 {statusText}[/]");
    }

    public void Dispose() {
        if (_disposed)
            return;
        _disposed = true;
    }
}

internal class SimpleSpinnerContext : ISpinnerContext {
    private readonly string _initialMessage;
    private bool _disposed;

    public SimpleSpinnerContext(string message) {
        _initialMessage = message;
        AnsiConsole.MarkupLine($"[blue]🔄 {message}[/]");
    }

    public void UpdateMessage(string message) {
        if (_disposed)
            return;
        AnsiConsole.MarkupLine($"[blue]🔄 {message}[/]");
    }

    public void Success(string? message = null) {
        if (_disposed)
            return;

        var statusText = message ?? "Operation completed successfully";
        AnsiConsole.MarkupLine($"[green]✅ {statusText}[/]");
        Dispose();
    }

    public void Fail(string message) {
        if (_disposed)
            return;

        AnsiConsole.MarkupLine($"[red]❌ {message}[/]");
        Dispose();
    }

    public void Warning(string message) {
        if (_disposed)
            return;

        AnsiConsole.MarkupLine($"[yellow]⚠️ {message}[/]");
        Dispose();
    }

    public void Dispose() {
        if (_disposed)
            return;
        _disposed = true;
    }
}

internal class SimpleLiveDisplayContext : ILiveDisplayContext {
    private readonly List<string> _lines = [];
    private readonly string _title;
    private bool _disposed;

    public SimpleLiveDisplayContext(string title) {
        _title = title;
        AnsiConsole.MarkupLine($"[blue]📺 {title}[/]");
    }

    public void UpdateContent(string content) {
        if (_disposed)
            return;
        AnsiConsole.MarkupLine($"[gray]{content}[/]");
    }

    public void AddLine(string line) {
        if (_disposed)
            return;

        _lines.Add(line);
        AnsiConsole.MarkupLine($"[gray]  {line}[/]");
    }

    public void Clear() {
        if (_disposed)
            return;

        _lines.Clear();
        AnsiConsole.MarkupLine($"[blue]📺 {_title} - Cleared[/]");
    }

    public void Stop() => Dispose();

    public void Dispose() {
        if (_disposed)
            return;
        _disposed = true;
    }
}

// No-op implementations for non-interactive mode
internal class NoOpProgressContext : IProgressContext {
    public void UpdateProgress(double value, string? message = null) { }
    public void Complete(string? message = null) { }
    public void Fail(string message) { }
    public void UpdateStatus(string status) { }
    public void Increment(double amount = 1.0) { }
    public void Dispose() { }
}

internal class NoOpDownloadProgress : IDownloadProgress {
    public TimeSpan? EstimatedTimeRemaining => null;
    public long? CurrentSpeed => null;
    public void UpdateProgress(long bytesDownloaded, long? speed = null) { }
    public void UpdateStatus(string status) { }
    public void Complete(string? message = null) { }
    public void Fail(string message) { }
    public void Dispose() { }
}

internal class NoOpStepProgress : IStepProgress {
    public void StartStep(int stepIndex, string? message = null) { }
    public void CompleteStep(int stepIndex, string? message = null) { }
    public void FailStep(int stepIndex, string message) { }
    public void SkipStep(int stepIndex, string reason) { }
    public void UpdateStatus(string message) { }
    public void CompleteAll(string? message = null) { }
    public void Dispose() { }
}

internal class NoOpSpinnerContext : ISpinnerContext {
    public void UpdateMessage(string message) { }
    public void Success(string? message = null) { }
    public void Fail(string message) { }
    public void Warning(string message) { }
    public void Dispose() { }
}

internal class NoOpLiveDisplayContext : ILiveDisplayContext {
    public void UpdateContent(string content) { }
    public void AddLine(string line) { }
    public void Clear() { }
    public void Stop() { }
    public void Dispose() { }
}