using System.Linq.Expressions;

namespace MCPHub.Common.Services;

/// <summary>
/// Background job processing service
/// Supports: In-Process → Hangfire → Azure Service Bus
/// </summary>
public interface IBackgroundJobService {
    /// <summary>
    /// Enqueues a job for immediate processing
    /// </summary>
    Task<string> EnqueueAsync<T>(Expression<Func<T, Task>> methodCall, CancellationToken cancellationToken = default);

    /// <summary>
    /// Schedules a job for future execution
    /// </summary>
    Task<string> ScheduleAsync<T>(Expression<Func<T, Task>> methodCall, DateTimeOffset scheduleAt, CancellationToken cancellationToken = default);

    /// <summary>
    /// Schedules a recurring job
    /// </summary>
    Task<string> RecurringAsync<T>(string jobId, Expression<Func<T, Task>> methodCall, string cronExpression, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cancels a scheduled job
    /// </summary>
    Task<bool> CancelAsync(string jobId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets job status and details
    /// </summary>
    Task<JobInfo> GetJobAsync(string jobId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists jobs with filtering
    /// </summary>
    Task<IEnumerable<JobInfo>> ListJobsAsync(JobStatus? status = null, int maxCount = 100, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets processing statistics
    /// </summary>
    Task<JobProcessingStatistics> GetStatisticsAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Job information record
/// </summary>
public record JobInfo(
    string JobId,
    string JobType,
    JobStatus Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset? ScheduledAt,
    DateTimeOffset? StartedAt,
    DateTimeOffset? CompletedAt,
    string? ErrorMessage,
    int RetryCount,
    IDictionary<string, object> Parameters);

/// <summary>
/// Job status enumeration
/// </summary>
public enum JobStatus {
    Pending,
    Processing,
    Completed,
    Failed,
    Cancelled,
    Scheduled,
}

/// <summary>
/// Job processing statistics
/// </summary>
public record JobProcessingStatistics(
    long QueuedCount,
    long ProcessingCount,
    long CompletedCount,
    long FailedCount,
    double AverageProcessingTime,
    double ThroughputPerMinute,
    DateTime Timestamp);