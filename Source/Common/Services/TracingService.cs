using Common.Extensions;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Common.Services;

/// <summary>
/// Service for managing distributed tracing with OpenTelemetry
/// </summary>
public interface ITracingService
{
    Activity? StartActivity(string name, ActivityKind kind = ActivityKind.Internal);
    void AddEvent(Activity? activity, string name, string? description = null);
    void SetTag(Activity? activity, string key, string value);
    void SetError(Activity? activity, Exception exception);
    void SetStatus(Activity? activity, ActivityStatusCode statusCode, string? description = null);
    IDisposable CreateScope(string operationName, [CallerMemberName] string? callerName = null);
}

public class TracingService : ITracingService
{
    private readonly ILogger<TracingService> _logger;

    public TracingService(ILogger<TracingService> logger)
    {
        _logger = logger;
    }

    public Activity? StartActivity(string name, ActivityKind kind = ActivityKind.Internal)
    {
        var activity = ActivitySources.Main.StartActivity(name, kind);
        
        if (activity != null)
        {
            activity.SetTag("service.name", "MCPHub");
            activity.SetTag("service.version", "1.0.0");
            activity.SetTag("environment", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development");
        }

        return activity;
    }

    public void AddEvent(Activity? activity, string name, string? description = null)
    {
        if (activity == null) return;

        var tags = new ActivityTagsCollection();
        if (!string.IsNullOrEmpty(description))
        {
            tags.Add("description", description);
        }

        activity.AddEvent(new ActivityEvent(name, DateTime.UtcNow, tags));
    }

    public void SetTag(Activity? activity, string key, string value)
    {
        activity?.SetTag(key, value);
    }

    public void SetError(Activity? activity, Exception exception)
    {
        if (activity == null) return;

        activity.SetStatus(ActivityStatusCode.Error, exception.Message);
        activity.SetTag("error.type", exception.GetType().Name);
        activity.SetTag("error.message", exception.Message);
        activity.SetTag("error.stack", exception.StackTrace);

        AddEvent(activity, "exception", exception.ToString());
    }

    public void SetStatus(Activity? activity, ActivityStatusCode statusCode, string? description = null)
    {
        activity?.SetStatus(statusCode, description);
    }

    public IDisposable CreateScope(string operationName, [CallerMemberName] string? callerName = null)
    {
        var fullName = callerName != null ? $"{callerName}.{operationName}" : operationName;
        var activity = StartActivity(fullName);
        
        return new TracingScope(activity, _logger);
    }

    private class TracingScope : IDisposable
    {
        private readonly Activity? _activity;
        private readonly ILogger _logger;
        private readonly Stopwatch _stopwatch;

        public TracingScope(Activity? activity, ILogger logger)
        {
            _activity = activity;
            _logger = logger;
            _stopwatch = Stopwatch.StartNew();
        }

        public void Dispose()
        {
            _stopwatch.Stop();
            
            if (_activity != null)
            {
                _activity.SetTag("duration_ms", _stopwatch.ElapsedMilliseconds);
                _activity.Dispose();
            }
        }
    }
}

/// <summary>
/// Extension methods for easier tracing
/// </summary>
public static class TracingExtensions
{
    /// <summary>
    /// Traces a database operation
    /// </summary>
    public static IDisposable TraceDatabaseOperation(this ITracingService tracingService, string operation, string? table = null)
    {
        var activity = tracingService.StartActivity($"db.{operation}", ActivityKind.Client);
        
        if (activity != null)
        {
            activity.SetTag("db.system", "postgresql");
            activity.SetTag("db.operation", operation);
            if (!string.IsNullOrEmpty(table))
            {
                activity.SetTag("db.table", table);
            }
        }

        return new TracingScope(activity);
    }

    /// <summary>
    /// Traces a cache operation
    /// </summary>
    public static IDisposable TraceCacheOperation(this ITracingService tracingService, string operation, string? key = null)
    {
        var activity = tracingService.StartActivity($"cache.{operation}", ActivityKind.Client);
        
        if (activity != null)
        {
            activity.SetTag("cache.system", "redis");
            activity.SetTag("cache.operation", operation);
            if (!string.IsNullOrEmpty(key))
            {
                activity.SetTag("cache.key", key);
            }
        }

        return new TracingScope(activity);
    }

    /// <summary>
    /// Traces a search operation
    /// </summary>
    public static IDisposable TraceSearchOperation(this ITracingService tracingService, string operation, string? query = null)
    {
        var activity = tracingService.StartActivity($"search.{operation}", ActivityKind.Client);
        
        if (activity != null)
        {
            activity.SetTag("search.system", "elasticsearch");
            activity.SetTag("search.operation", operation);
            if (!string.IsNullOrEmpty(query))
            {
                activity.SetTag("search.query", query);
            }
        }

        return new TracingScope(activity);
    }

    /// <summary>
    /// Traces a message processing operation
    /// </summary>
    public static IDisposable TraceMessageOperation(this ITracingService tracingService, string operation, string? messageType = null)
    {
        var activity = tracingService.StartActivity($"message.{operation}", ActivityKind.Consumer);
        
        if (activity != null)
        {
            activity.SetTag("messaging.system", "rabbitmq");
            activity.SetTag("messaging.operation", operation);
            if (!string.IsNullOrEmpty(messageType))
            {
                activity.SetTag("messaging.message_type", messageType);
            }
        }

        return new TracingScope(activity);
    }

    private class TracingScope : IDisposable
    {
        private readonly Activity? _activity;

        public TracingScope(Activity? activity)
        {
            _activity = activity;
        }

        public void Dispose()
        {
            _activity?.Dispose();
        }
    }
}