using System.ComponentModel.DataAnnotations;

using Microsoft.Extensions.Options;

namespace MCPHub.Data.Configuration.Validators;

public class DatabaseOptionsValidator : IValidateOptions<DatabaseOptions> {
    public ValidateOptionsResult Validate(string? name, DatabaseOptions options) {
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