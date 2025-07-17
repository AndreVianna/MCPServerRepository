using Microsoft.Extensions.Options;

namespace Common.Configuration;

public class DatabaseOptionsValidator : IValidateOptions<DatabaseOptions>
{
    public ValidateOptionsResult Validate(string? name, DatabaseOptions options)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(options.ConnectionString))
        {
            errors.Add("Database connection string is required");
        }

        if (options.MaxRetryCount < 0)
        {
            errors.Add("Database MaxRetryCount must be non-negative");
        }

        if (options.CommandTimeout <= TimeSpan.Zero)
        {
            errors.Add("Database CommandTimeout must be positive");
        }

        if (options.MaxPoolSize <= 0)
        {
            errors.Add("Database MaxPoolSize must be positive");
        }

        if (options.HealthCheckTimeout <= TimeSpan.Zero)
        {
            errors.Add("Database HealthCheckTimeout must be positive");
        }

        return errors.Count > 0 
            ? ValidateOptionsResult.Fail(errors)
            : ValidateOptionsResult.Success;
    }
}