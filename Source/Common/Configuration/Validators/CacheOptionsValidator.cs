using Microsoft.Extensions.Options;

namespace Common.Configuration;

public class CacheOptionsValidator : IValidateOptions<CacheOptions>
{
    public ValidateOptionsResult Validate(string? name, CacheOptions options)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(options.ConnectionString))
        {
            errors.Add("Cache connection string is required");
        }

        if (options.DefaultExpiration <= TimeSpan.Zero)
        {
            errors.Add("Cache DefaultExpiration must be positive");
        }

        if (options.SlidingExpiration <= TimeSpan.Zero)
        {
            errors.Add("Cache SlidingExpiration must be positive");
        }

        if (options.Database < 0)
        {
            errors.Add("Cache Database must be non-negative");
        }

        if (string.IsNullOrWhiteSpace(options.KeyPrefix))
        {
            errors.Add("Cache KeyPrefix is required");
        }

        if (options.HealthCheckTimeout <= TimeSpan.Zero)
        {
            errors.Add("Cache HealthCheckTimeout must be positive");
        }

        return errors.Count > 0 
            ? ValidateOptionsResult.Fail(errors)
            : ValidateOptionsResult.Success;
    }
}