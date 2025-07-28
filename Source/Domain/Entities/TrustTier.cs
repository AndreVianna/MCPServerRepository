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
    Enterprise = 3
}