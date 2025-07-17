using Microsoft.Extensions.Options;

namespace Common.Configuration;

public class ElasticsearchOptionsValidator : IValidateOptions<ElasticsearchOptions> {
    public ValidateOptionsResult Validate(string? name, ElasticsearchOptions options) {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(options.ConnectionString)) {
            errors.Add("Elasticsearch connection string is required");
        }

        if (string.IsNullOrWhiteSpace(options.IndexPrefix)) {
            errors.Add("Elasticsearch IndexPrefix is required");
        }

        if (options.MaxRetryCount < 0) {
            errors.Add("Elasticsearch MaxRetryCount must be non-negative");
        }

        if (options.RequestTimeout <= TimeSpan.Zero) {
            errors.Add("Elasticsearch RequestTimeout must be positive");
        }

        if (options.MaxDegreeOfParallelism <= 0) {
            errors.Add("Elasticsearch MaxDegreeOfParallelism must be positive");
        }

        if (options.HealthCheckTimeout <= TimeSpan.Zero) {
            errors.Add("Elasticsearch HealthCheckTimeout must be positive");
        }

        return errors.Count > 0
            ? ValidateOptionsResult.Fail(errors)
            : ValidateOptionsResult.Success;
    }
}