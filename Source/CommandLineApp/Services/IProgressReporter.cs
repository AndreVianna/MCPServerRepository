namespace MCPHub.CommandLineApp.Services;

/// <summary>
/// Service interface for reporting progress and visual feedback
/// </summary>
public interface IProgressReporter {
    /// <summary>
    /// Creates a progress bar for tracking operation completion
    /// </summary>
    /// <param name="description">Description of the operation</param>
    /// <param name="maxValue">Maximum value for progress (100 for percentage)</param>
    /// <returns>Progress context for updating the progress bar</returns>
    IProgressContext CreateProgressBar(string description, double maxValue = 100);

    /// <summary>
    /// Creates a download progress bar with advanced features
    /// </summary>
    /// <param name="fileName">Name of the file being downloaded</param>
    /// <param name="totalBytes">Total size in bytes</param>
    /// <returns>Download progress context</returns>
    IDownloadProgress CreateDownloadProgress(string fileName, long totalBytes);

    /// <summary>
    /// Creates a step-by-step progress tracker
    /// </summary>
    /// <param name="title">Title of the operation</param>
    /// <param name="steps">List of step descriptions</param>
    /// <returns>Step progress context</returns>
    IStepProgress CreateStepProgress(string title, IEnumerable<string> steps);

    /// <summary>
    /// Shows a simple spinner for indeterminate operations
    /// </summary>
    /// <param name="message">Message to display with the spinner</param>
    /// <returns>Spinner context</returns>
    ISpinnerContext CreateSpinner(string message);

    /// <summary>
    /// Creates a live display for real-time updates
    /// </summary>
    /// <param name="title">Title of the live display</param>
    /// <returns>Live display context</returns>
    ILiveDisplayContext CreateLiveDisplay(string title);

    /// <summary>
    /// Creates a simple progress context for tracking operation completion
    /// </summary>
    /// <param name="description">Description of the operation</param>
    /// <returns>Progress context for updating the progress</returns>
    IProgressContext CreateProgress(string description);
}

/// <summary>
/// Context for updating a progress bar
/// </summary>
public interface IProgressContext : IDisposable {
    /// <summary>
    /// Updates the progress value
    /// </summary>
    /// <param name="value">Current progress value</param>
    /// <param name="message">Optional status message</param>
    void UpdateProgress(double value, string? message = null);

    /// <summary>
    /// Marks the operation as complete
    /// </summary>
    /// <param name="message">Completion message</param>
    void Complete(string? message = null);

    /// <summary>
    /// Marks the operation as failed
    /// </summary>
    /// <param name="message">Error message</param>
    void Fail(string message);

    /// <summary>
    /// Updates the status message of the progress
    /// </summary>
    /// <param name="status">Status message</param>
    void UpdateStatus(string status);

    /// <summary>
    /// Increments the progress by a specified amount
    /// </summary>
    /// <param name="amount">Amount to increment by (default: 1)</param>
    void Increment(double amount = 1.0);
}

/// <summary>
/// Context for download progress tracking
/// </summary>
public interface IDownloadProgress : IDisposable {
    /// <summary>
    /// Updates download progress
    /// </summary>
    /// <param name="bytesDownloaded">Number of bytes downloaded</param>
    /// <param name="speed">Download speed in bytes per second</param>
    void UpdateProgress(long bytesDownloaded, long? speed = null);

    /// <summary>
    /// Updates the download status
    /// </summary>
    /// <param name="status">Status message (e.g., "Verifying", "Extracting")</param>
    void UpdateStatus(string status);

    /// <summary>
    /// Marks the download as complete
    /// </summary>
    /// <param name="message">Completion message</param>
    void Complete(string? message = null);

    /// <summary>
    /// Marks the download as failed
    /// </summary>
    /// <param name="message">Error message</param>
    void Fail(string message);

    /// <summary>
    /// Gets the estimated time remaining
    /// </summary>
    TimeSpan? EstimatedTimeRemaining { get; }

    /// <summary>
    /// Gets the current download speed in bytes per second
    /// </summary>
    long? CurrentSpeed { get; }
}

/// <summary>
/// Context for step-by-step progress tracking
/// </summary>
public interface IStepProgress : IDisposable {
    /// <summary>
    /// Marks a step as starting
    /// </summary>
    /// <param name="stepIndex">Index of the step (0-based)</param>
    /// <param name="message">Optional status message</param>
    void StartStep(int stepIndex, string? message = null);

    /// <summary>
    /// Marks a step as complete
    /// </summary>
    /// <param name="stepIndex">Index of the step (0-based)</param>
    /// <param name="message">Optional completion message</param>
    void CompleteStep(int stepIndex, string? message = null);

    /// <summary>
    /// Marks a step as failed
    /// </summary>
    /// <param name="stepIndex">Index of the step (0-based)</param>
    /// <param name="message">Error message</param>
    void FailStep(int stepIndex, string message);

    /// <summary>
    /// Skips a step
    /// </summary>
    /// <param name="stepIndex">Index of the step (0-based)</param>
    /// <param name="reason">Reason for skipping</param>
    void SkipStep(int stepIndex, string reason);

    /// <summary>
    /// Updates the status of the current step
    /// </summary>
    /// <param name="message">Status message</param>
    void UpdateStatus(string message);

    /// <summary>
    /// Marks all remaining steps as complete
    /// </summary>
    /// <param name="message">Completion message</param>
    void CompleteAll(string? message = null);
}

/// <summary>
/// Context for spinner display
/// </summary>
public interface ISpinnerContext : IDisposable {
    /// <summary>
    /// Updates the spinner message
    /// </summary>
    /// <param name="message">New message to display</param>
    void UpdateMessage(string message);

    /// <summary>
    /// Stops the spinner and shows success
    /// </summary>
    /// <param name="message">Success message</param>
    void Success(string? message = null);

    /// <summary>
    /// Stops the spinner and shows failure
    /// </summary>
    /// <param name="message">Error message</param>
    void Fail(string message);

    /// <summary>
    /// Stops the spinner and shows warning
    /// </summary>
    /// <param name="message">Warning message</param>
    void Warning(string message);
}

/// <summary>
/// Context for live display updates
/// </summary>
public interface ILiveDisplayContext : IDisposable {
    /// <summary>
    /// Updates the live display content
    /// </summary>
    /// <param name="content">New content to display</param>
    void UpdateContent(string content);

    /// <summary>
    /// Adds a line to the live display
    /// </summary>
    /// <param name="line">Line to add</param>
    void AddLine(string line);

    /// <summary>
    /// Clears the live display
    /// </summary>
    void Clear();

    /// <summary>
    /// Stops the live display
    /// </summary>
    void Stop();
}