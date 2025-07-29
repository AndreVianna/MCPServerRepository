namespace MCPHub.Domain.Entities;

/// <summary>
/// Types of security scans that can be performed
/// </summary>
public enum ScanType {
    /// <summary>
    /// Static code analysis scan
    /// </summary>
    StaticAnalysis,

    /// <summary>
    /// Dynamic security testing
    /// </summary>
    DynamicTesting,

    /// <summary>
    /// Dependency vulnerability scan
    /// </summary>
    DependencyVulnerability,

    /// <summary>
    /// Configuration security scan
    /// </summary>
    Configuration,

    /// <summary>
    /// Compliance verification scan
    /// </summary>
    Compliance,

    /// <summary>
    /// Container security scan
    /// </summary>
    Container,

    /// <summary>
    /// Network security scan
    /// </summary>
    Network,

    /// <summary>
    /// MCP manifest security scan
    /// </summary>
    Manifest,

    /// <summary>
    /// Package content security scan
    /// </summary>
    Content,

    /// <summary>
    /// Full comprehensive scan (all scan types)
    /// </summary>
    Full,

    /// <summary>
    /// Malware detection scan
    /// </summary>
    Malware,

    /// <summary>
    /// License compliance scan
    /// </summary>
    License,

    /// <summary>
    /// Comprehensive security analysis (combination of multiple scan types)
    /// </summary>
    Comprehensive,

    /// <summary>
    /// All available scan types
    /// </summary>
    All,

    /// <summary>
    /// Vulnerability scan (alias for DependencyVulnerability)
    /// </summary>
    Vulnerability
}