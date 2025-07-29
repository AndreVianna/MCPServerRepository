using System.ComponentModel.DataAnnotations;

namespace MCPHub.Common.Configuration.Validators;

public class ObservabilityOptionsValidator : IValidateOptions<ObservabilityOptions> {
    public ValidateOptionsResult Validate(string? name, ObservabilityOptions options) {
        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(options);

        // Use DataAnnotations validation first
        var isValid = Validator.TryValidateObject(options, context, validationResults, true);

        List<string> errors = [];

        if (!isValid) {
            errors.AddRange(validationResults.Select(vr => vr.ErrorMessage ?? "Validation error"));
        }

        // Add custom validation logic that can't be expressed with DataAnnotations
        // Note: OpenTelemetry configuration removed - no custom validation needed

        return errors.Count > 0
            ? ValidateOptionsResult.Fail(errors)
            : ValidateOptionsResult.Success;
    }
}