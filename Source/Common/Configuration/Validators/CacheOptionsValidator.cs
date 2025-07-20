using System.ComponentModel.DataAnnotations;

namespace MCPHub.Common.Configuration.Validators;

public class CacheOptionsValidator : IValidateOptions<CacheOptions> {
    public ValidateOptionsResult Validate(string? name, CacheOptions options) {
        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(options);

        // Use DataAnnotations validation
        var isValid = Validator.TryValidateObject(options, context, validationResults, true);

        if (!isValid) {
            var errors = validationResults.Select(vr => vr.ErrorMessage ?? "Validation error").ToList();
            return ValidateOptionsResult.Fail(errors);
        }

        return ValidateOptionsResult.Success;
    }
}