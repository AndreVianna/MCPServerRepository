using Common.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;
using MsLogger = Microsoft.Extensions.Logging.ILogger;

namespace Common.Extensions;

public static class SerilogExtensions
{
    /// <summary>
    /// Configures Serilog logging based on configuration options
    /// </summary>
    public static IHostBuilder UseSerilogLogging(this IHostBuilder hostBuilder)
    {
        return hostBuilder.UseSerilog((context, services, configuration) =>
        {
            var observabilityOptions = services.GetService<IOptions<ObservabilityOptions>>()?.Value
                ?? new ObservabilityOptions();

            ConfigureSerilog(configuration, observabilityOptions.Serilog, context.HostingEnvironment.EnvironmentName);
        });
    }

    /// <summary>
    /// Configures Serilog logging with the specified options
    /// </summary>
    public static void ConfigureSerilog(LoggerConfiguration configuration, SerilogOptions options, string environment)
    {
        // Set minimum log level
        configuration.MinimumLevel.Is(ParseLogLevel(options.MinimumLevel));

        // Apply minimum level overrides
        foreach (var kvp in options.MinimumLevelOverrides)
        {
            configuration.MinimumLevel.Override(kvp.Key, ParseLogLevel(kvp.Value));
        }

        // Add enrichers
        configuration
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Environment", environment)
            .Enrich.WithProperty("Application", "MCPHub")
            .Enrich.WithMachineName()
            .Enrich.WithProcessId()
            .Enrich.WithThreadId();

        // Configure console sink
        if (options.EnableConsole)
        {
            configuration.WriteTo.Console(
                outputTemplate: options.LogTemplate,
                restrictedToMinimumLevel: LogEventLevel.Information);
        }

        // Configure file sink
        if (options.EnableFile)
        {
            var logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, options.LogDirectory);
            Directory.CreateDirectory(logDirectory);

            configuration.WriteTo.File(
                path: Path.Combine(logDirectory, "mcphub-.log"),
                outputTemplate: options.LogTemplate,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 30,
                restrictedToMinimumLevel: LogEventLevel.Information);

            // Add error-only file
            configuration.WriteTo.File(
                path: Path.Combine(logDirectory, "mcphub-errors-.log"),
                outputTemplate: options.LogTemplate,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 90,
                restrictedToMinimumLevel: LogEventLevel.Error);
        }
    }

    private static LogEventLevel ParseLogLevel(string level)
    {
        return level.ToLowerInvariant() switch
        {
            "verbose" => LogEventLevel.Verbose,
            "debug" => LogEventLevel.Debug,
            "information" => LogEventLevel.Information,
            "warning" => LogEventLevel.Warning,
            "error" => LogEventLevel.Error,
            "fatal" => LogEventLevel.Fatal,
            _ => LogEventLevel.Information
        };
    }
}

/// <summary>
/// Structured logging helpers
/// </summary>
public static class LoggerExtensions
{
    /// <summary>
    /// Logs an operation with duration timing
    /// </summary>
    public static IDisposable BeginOperation(this MsLogger logger, string operationName, params object[] args)
    {
        return new OperationLogger(logger, operationName, args);
    }

    private class OperationLogger : IDisposable
    {
        private readonly MsLogger _logger;
        private readonly string _operationName;
        private readonly object[] _args;
        private readonly System.Diagnostics.Stopwatch _stopwatch;

        public OperationLogger(MsLogger logger, string operationName, object[] args)
        {
            _logger = logger;
            _operationName = operationName;
            _args = args;
            _stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            _logger.LogDebug("Starting operation {OperationName} with args {Args}", _operationName, _args);
        }

        public void Dispose()
        {
            _stopwatch.Stop();
            _logger.LogDebug("Completed operation {OperationName} in {ElapsedMilliseconds}ms", 
                _operationName, _stopwatch.ElapsedMilliseconds);
        }
    }
}