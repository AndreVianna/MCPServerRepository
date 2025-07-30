namespace MCPHub.Domain.Entities;

/// <summary>
/// Represents the trust tier of an MCP package based on security, community feedback, and publisher reputation
/// </summary>
public enum TrustTier {
    /// <summary>
    /// Initial tier for new packages with minimal verification
    /// </summary>
    Unverified = 0,

    /// <summary>
    /// Packages with community validation and basic security requirements
    /// </summary>
    CommunityTrusted = 1,

    /// <summary>
    /// Publisher-verified packages meeting higher security standards
    /// </summary>
    Verified = 2,

    /// <summary>
    /// Enterprise-grade packages with comprehensive auditing and verification
    /// </summary>
    Enterprise = 3,

    /// <summary>
    /// Packages that have undergone comprehensive security auditing
    /// </summary>
    SecurityAudited = 4,

    /// <summary>
    /// Officially certified packages meeting the highest security and quality standards
    /// </summary>
    Certified = 5,

    /// <summary>
    /// Community trusted packages (alias for CommunityTrusted)
    /// </summary>
    Community = CommunityTrusted,

    /// <summary>
    /// Professional packages (alias for Verified)
    /// </summary>
    Professional = Verified,
}