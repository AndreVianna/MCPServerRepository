namespace Domain.ValueObjects;

public enum SecurityScanStatus {
    Pending,
    InProgress,
    Passed,
    Failed,
    Error
}