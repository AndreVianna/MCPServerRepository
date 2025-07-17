using Microsoft.Extensions.Options;

namespace Common.Configuration;

public class RabbitMqOptionsValidator : IValidateOptions<RabbitMqOptions>
{
    public ValidateOptionsResult Validate(string? name, RabbitMqOptions options)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(options.ConnectionString))
        {
            errors.Add("RabbitMQ connection string is required");
        }

        if (string.IsNullOrWhiteSpace(options.VirtualHost))
        {
            errors.Add("RabbitMQ VirtualHost is required");
        }

        if (options.Port <= 0 || options.Port > 65535)
        {
            errors.Add("RabbitMQ Port must be between 1 and 65535");
        }

        if (string.IsNullOrWhiteSpace(options.Username))
        {
            errors.Add("RabbitMQ Username is required");
        }

        if (string.IsNullOrWhiteSpace(options.Password))
        {
            errors.Add("RabbitMQ Password is required");
        }

        if (options.RequestTimeout <= TimeSpan.Zero)
        {
            errors.Add("RabbitMQ RequestTimeout must be positive");
        }

        if (options.MaxRetryCount < 0)
        {
            errors.Add("RabbitMQ MaxRetryCount must be non-negative");
        }

        if (options.HealthCheckTimeout <= TimeSpan.Zero)
        {
            errors.Add("RabbitMQ HealthCheckTimeout must be positive");
        }

        return errors.Count > 0 
            ? ValidateOptionsResult.Fail(errors)
            : ValidateOptionsResult.Success;
    }
}