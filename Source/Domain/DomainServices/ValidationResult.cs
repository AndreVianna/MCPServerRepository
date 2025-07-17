namespace Domain.DomainServices;

public sealed record ValidationResult {
    public bool IsValid { get; init; }
    public string? ErrorMessage { get; init; }

    private ValidationResult(bool isValid, string? errorMessage = null) {
        IsValid = isValid;
        ErrorMessage = errorMessage;
    }

    public static ValidationResult Success() => new(true);
    public static ValidationResult Failure(string errorMessage) => new(false, errorMessage);
}