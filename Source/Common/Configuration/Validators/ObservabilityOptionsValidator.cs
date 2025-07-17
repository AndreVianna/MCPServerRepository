using Microsoft.Extensions.Options;

namespace Common.Configuration;

public class ObservabilityOptionsValidator : IValidateOptions<ObservabilityOptions>
{
    public ValidateOptionsResult Validate(string? name, ObservabilityOptions options)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(options.ServiceName))
        {
            errors.Add("Observability ServiceName is required");
        }

        if (string.IsNullOrWhiteSpace(options.ServiceVersion))
        {
            errors.Add("Observability ServiceVersion is required");
        }

        if (string.IsNullOrWhiteSpace(options.Environment))
        {
            errors.Add("Observability Environment is required");
        }

        // Validate OpenTelemetry options
        if (options.OpenTelemetry.EnableOtlpExporter && string.IsNullOrWhiteSpace(options.OpenTelemetry.OtlpEndpoint))
        {
            errors.Add("OpenTelemetry OtlpEndpoint is required when OtlpExporter is enabled");
        }

        if (options.OpenTelemetry.Sources.Length == 0)
        {
            errors.Add("OpenTelemetry Sources must contain at least one source");
        }

        // Validate Serilog options
        if (string.IsNullOrWhiteSpace(options.Serilog.MinimumLevel))
        {
            errors.Add("Serilog MinimumLevel is required");
        }

        if (options.Serilog.EnableFile && string.IsNullOrWhiteSpace(options.Serilog.LogDirectory))
        {
            errors.Add("Serilog LogDirectory is required when file logging is enabled");
        }

        if (string.IsNullOrWhiteSpace(options.Serilog.LogTemplate))
        {
            errors.Add("Serilog LogTemplate is required");
        }

        return errors.Count > 0 
            ? ValidateOptionsResult.Fail(errors)
            : ValidateOptionsResult.Success;
    }
}