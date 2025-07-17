namespace Common.UnitTests.TestUtilities;

/// <summary>
/// Attribute to mark tests as unit tests.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class UnitTestAttribute : TraitAttribute {
    public UnitTestAttribute() : base("Category", TestCategories.Unit) { }
}

/// <summary>
/// Attribute to mark tests as integration tests.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class IntegrationTestAttribute : TraitAttribute {
    public IntegrationTestAttribute() : base("Category", TestCategories.Integration) { }
}

/// <summary>
/// Attribute to mark tests as end-to-end tests.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class EndToEndTestAttribute : TraitAttribute {
    public EndToEndTestAttribute() : base("Category", TestCategories.EndToEnd) { }
}

/// <summary>
/// Attribute to mark tests as performance tests.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class PerformanceTestAttribute : TraitAttribute {
    public PerformanceTestAttribute() : base("Category", TestCategories.Performance) { }
}

/// <summary>
/// Attribute to mark tests as security tests.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class SecurityTestAttribute : TraitAttribute {
    public SecurityTestAttribute() : base("Category", TestCategories.Security) { }
}

/// <summary>
/// Attribute to mark tests as database tests.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class DatabaseTestAttribute : TraitAttribute {
    public DatabaseTestAttribute() : base("Category", TestCategories.Database) { }
}

/// <summary>
/// Attribute to mark tests as external API tests.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class ExternalApiTestAttribute : TraitAttribute {
    public ExternalApiTestAttribute() : base("Category", TestCategories.ExternalApi) { }
}

/// <summary>
/// Attribute to mark tests as long-running tests.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class LongRunningTestAttribute : TraitAttribute {
    public LongRunningTestAttribute() : base("Category", TestCategories.LongRunning) { }
}