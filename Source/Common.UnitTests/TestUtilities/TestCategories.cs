namespace Common.UnitTests.TestUtilities;

/// <summary>
/// Test categories for organizing and running different types of tests.
/// </summary>
public static class TestCategories {
    /// <summary>
    /// Unit tests that test individual components in isolation.
    /// </summary>
    public const string Unit = "Unit";

    /// <summary>
    /// Integration tests that test multiple components working together.
    /// </summary>
    public const string Integration = "Integration";

    /// <summary>
    /// End-to-end tests that test the complete application flow.
    /// </summary>
    public const string EndToEnd = "EndToEnd";

    /// <summary>
    /// Performance tests that measure system performance.
    /// </summary>
    public const string Performance = "Performance";

    /// <summary>
    /// Security tests that verify security constraints.
    /// </summary>
    public const string Security = "Security";

    /// <summary>
    /// Database tests that require database access.
    /// </summary>
    public const string Database = "Database";

    /// <summary>
    /// External API tests that require external service calls.
    /// </summary>
    public const string ExternalApi = "ExternalApi";

    /// <summary>
    /// Long-running tests that may take significant time to complete.
    /// </summary>
    public const string LongRunning = "LongRunning";
}