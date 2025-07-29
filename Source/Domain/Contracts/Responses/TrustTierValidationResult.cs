using MCPHub.Domain.Entities;

namespace MCPHub.Domain.Contracts.Responses;

/// <summary>
/// Represents the result of trust tier eligibility validation
/// </summary>
public class TrustTierValidationResult {
    /// <summary>
    /// Gets or sets the package identifier being validated
    /// </summary>
    public Guid PackageId { get; set; }

    /// <summary>
    /// Gets or sets the target trust tier being validated for
    /// </summary>
    public TrustTier TargetTier { get; set; }

    /// <summary>
    /// Gets or sets the current trust tier of the package
    /// </summary>
    public TrustTier CurrentTier { get; set; }

    /// <summary>
    /// Gets or sets whether the package is eligible for the target tier
    /// </summary>
    public bool IsEligible { get; set; }

    /// <summary>
    /// Gets or sets the current trust score of the package
    /// </summary>
    public int CurrentScore { get; set; }

    /// <summary>
    /// Gets or sets the minimum score required for the target tier
    /// </summary>
    public int RequiredScore { get; set; }

    /// <summary>
    /// Gets or sets the list of requirements that are currently met
    /// </summary>
    public IEnumerable<string> MetRequirements { get; set; } = Enumerable.Empty<string>();

    /// <summary>
    /// Gets or sets the list of requirements that are not yet met
    /// </summary>
    public IEnumerable<string> UnmetRequirements { get; set; } = Enumerable.Empty<string>();

    /// <summary>
    /// Gets or sets the detailed validation results by category
    /// </summary>
    public Dictionary<string, TrustTierCategoryValidation> CategoryValidations { get; set; } = new();

    /// <summary>
    /// Gets or sets recommendations for achieving the target tier
    /// </summary>
    public IEnumerable<string> Recommendations { get; set; } = Enumerable.Empty<string>();

    /// <summary>
    /// Gets or sets the estimated time to achieve eligibility (in days)
    /// </summary>
    public int? EstimatedDaysToEligibility { get; set; }

    /// <summary>
    /// Gets or sets when this validation was performed
    /// </summary>
    public DateTimeOffset ValidatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets or sets additional validation metadata
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();

    /// <summary>
    /// Gets the percentage of requirements that are met
    /// </summary>
    public decimal CompletionPercentage {
        get {
            var totalRequirements = MetRequirements.Count() + UnmetRequirements.Count();
            return totalRequirements > 0 ? (decimal)MetRequirements.Count() / totalRequirements * 100 : 100;
        }
    }
}

/// <summary>
/// Represents validation results for a specific trust tier category
/// </summary>
public class TrustTierCategoryValidation {
    /// <summary>
    /// Gets or sets the category name (e.g., "Security", "Community", "Publisher")
    /// </summary>
    public string CategoryName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the current score for this category
    /// </summary>
    public int CurrentScore { get; set; }

    /// <summary>
    /// Gets or sets the required score for this category
    /// </summary>
    public int RequiredScore { get; set; }

    /// <summary>
    /// Gets or sets the maximum possible score for this category
    /// </summary>
    public int MaxScore { get; set; }

    /// <summary>
    /// Gets or sets whether this category meets the requirements
    /// </summary>
    public bool Passes { get; set; }

    /// <summary>
    /// Gets or sets the weight of this category in the overall calculation
    /// </summary>
    public decimal Weight { get; set; }

    /// <summary>
    /// Gets or sets specific requirements for this category
    /// </summary>
    public IEnumerable<string> Requirements { get; set; } = Enumerable.Empty<string>();

    /// <summary>
    /// Gets or sets recommendations for improving this category
    /// </summary>
    public IEnumerable<string> Recommendations { get; set; } = Enumerable.Empty<string>();

    /// <summary>
    /// Gets the score as a percentage
    /// </summary>
    public decimal ScorePercentage => MaxScore > 0 ? (decimal)CurrentScore / MaxScore * 100 : 0;

    /// <summary>
    /// Gets the progress towards the required score
    /// </summary>
    public decimal ProgressPercentage => RequiredScore > 0 ? (decimal)CurrentScore / RequiredScore * 100 : 100;
}