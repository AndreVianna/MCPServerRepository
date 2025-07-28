using FluentAssertions;
using MCPHub.Domain.Contracts.Responses;
using MCPHub.Domain.Entities;

namespace MCPHub.Domain.UnitTests.Contracts.Responses;

/// <summary>
/// Unit tests for the TrustTierAssessment response model
/// </summary>
public class TrustTierAssessmentTests
{
    [Fact]
    public void ScorePercentage_ShouldReturnZero_WhenMaxScoreIsZero()
    {
        // Arrange
        var assessment = new TrustTierAssessment
        {
            TotalScore = 50,
            MaxScore = 0
        };

        // Act & Assert
        assessment.ScorePercentage.Should().Be(0);
    }

    [Fact]
    public void ScorePercentage_ShouldReturnCorrectPercentage_WhenMaxScoreIsGreaterThanZero()
    {
        // Arrange
        var assessment = new TrustTierAssessment
        {
            TotalScore = 75,
            MaxScore = 100
        };

        // Act & Assert
        assessment.ScorePercentage.Should().Be(75.0m);
    }

    [Fact]
    public void ScorePercentage_ShouldHandleDecimalCalculation_WhenScoresDoNotDivideEvenly()
    {
        // Arrange
        var assessment = new TrustTierAssessment
        {
            TotalScore = 33,
            MaxScore = 100
        };

        // Act & Assert
        assessment.ScorePercentage.Should().Be(33.0m);
    }

    [Fact]
    public void DefaultValues_ShouldBeSetCorrectly()
    {
        // Arrange & Act
        var assessment = new TrustTierAssessment();

        // Assert
        assessment.CurrentTier.Should().Be(TrustTier.Unverified);
        assessment.RecommendedTier.Should().Be(TrustTier.Unverified);
        assessment.TotalScore.Should().Be(0);
        assessment.MaxScore.Should().Be(0);
        assessment.Factors.Should().NotBeNull().And.BeEmpty();
        assessment.PositiveFactors.Should().NotBeNull().And.BeEmpty();
        assessment.NegativeFactors.Should().NotBeNull().And.BeEmpty();
        assessment.LastAssessment.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
        assessment.PreviousTier.Should().BeNull();
        assessment.NextAssessment.Should().BeNull();
        assessment.EligibleForPromotion.Should().BeFalse();
        assessment.AtRiskForDemotion.Should().BeFalse();
        assessment.Metadata.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void Properties_ShouldBeSettableAndGettable()
    {
        // Arrange
        var assessment = new TrustTierAssessment();
        var factors = new Dictionary<string, TrustTierFactor>
        {
            ["security"] = new TrustTierFactor { Name = "Security", Score = 85, MaxScore = 100 }
        };
        var positiveFactors = new[] { "High download count", "Good user ratings" };
        var negativeFactors = new[] { "Recent security issues" };
        var metadata = new Dictionary<string, object> { ["calculated_at"] = DateTimeOffset.UtcNow };
        var lastAssessment = DateTimeOffset.UtcNow.AddDays(-1);
        var nextAssessment = DateTimeOffset.UtcNow.AddDays(7);

        // Act
        assessment.CurrentTier = TrustTier.CommunityTrusted;
        assessment.RecommendedTier = TrustTier.Verified;
        assessment.TotalScore = 850;
        assessment.MaxScore = 1000;
        assessment.Factors = factors;
        assessment.PositiveFactors = positiveFactors;
        assessment.NegativeFactors = negativeFactors;
        assessment.LastAssessment = lastAssessment;
        assessment.PreviousTier = TrustTier.Unverified;
        assessment.NextAssessment = nextAssessment;
        assessment.EligibleForPromotion = true;
        assessment.AtRiskForDemotion = false;
        assessment.Metadata = metadata;

        // Assert
        assessment.CurrentTier.Should().Be(TrustTier.CommunityTrusted);
        assessment.RecommendedTier.Should().Be(TrustTier.Verified);
        assessment.TotalScore.Should().Be(850);
        assessment.MaxScore.Should().Be(1000);
        assessment.ScorePercentage.Should().Be(85.0m);
        assessment.Factors.Should().BeEquivalentTo(factors);
        assessment.PositiveFactors.Should().BeEquivalentTo(positiveFactors);
        assessment.NegativeFactors.Should().BeEquivalentTo(negativeFactors);
        assessment.LastAssessment.Should().Be(lastAssessment);
        assessment.PreviousTier.Should().Be(TrustTier.Unverified);
        assessment.NextAssessment.Should().Be(nextAssessment);
        assessment.EligibleForPromotion.Should().BeTrue();
        assessment.AtRiskForDemotion.Should().BeFalse();
        assessment.Metadata.Should().BeEquivalentTo(metadata);
    }
}