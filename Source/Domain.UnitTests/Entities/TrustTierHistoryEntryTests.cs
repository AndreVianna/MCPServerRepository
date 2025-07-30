using MCPHub.Domain.Contracts.Responses;
using MCPHub.Domain.Entities;

namespace MCPHub.Domain.UnitTests.Entities;

/// <summary>
/// Unit tests for the TrustTierHistoryEntry entity
/// </summary>
public class TrustTierHistoryEntryTests {
    [Fact]
    public void Constructor_ShouldCreateValidHistoryEntry_WhenValidParametersProvided() {
        // Arrange
        var packageId = Guid.CreateVersion7();
        var fromTier = TrustTier.Unverified;
        var toTier = TrustTier.CommunityTrusted;
        var reason = "Met community requirements";
        var changeType = TierChangeType.Promotion;
        var changedByUserId = Guid.CreateVersion7();
        var isAutomatic = false;
        var trustScore = 85;
        var metadata = new Dictionary<string, object> { { "criteria", "download_count" } };
        var assessmentId = Guid.CreateVersion7();

        // Act
        var historyEntry = new TrustTierHistoryEntry(
            packageId,
            fromTier,
            toTier,
            reason,
            changeType,
            changedByUserId,
            isAutomatic,
            trustScore,
            metadata,
            assessmentId);

        // Assert
        historyEntry.Should().NotBeNull();
        historyEntry.Id.Should().NotBe(Guid.Empty);
        historyEntry.PackageId.Should().Be(packageId);
        historyEntry.FromTier.Should().Be(fromTier);
        historyEntry.ToTier.Should().Be(toTier);
        historyEntry.Reason.Should().Be(reason);
        historyEntry.ChangeType.Should().Be(changeType);
        historyEntry.ChangedByUserId.Should().Be(changedByUserId);
        historyEntry.IsAutomatic.Should().Be(isAutomatic);
        historyEntry.TrustScoreAtChange.Should().Be(trustScore);
        historyEntry.Metadata.Should().BeEquivalentTo(metadata);
        historyEntry.AssessmentId.Should().Be(assessmentId);
        historyEntry.ChangedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
        historyEntry.AuditTrail.Should().HaveCount(1);
        historyEntry.AuditTrail.First().Action.Should().Contain("Trust Tier Changed");
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentException_WhenReasonIsNullOrWhiteSpace() {
        // Arrange
        var packageId = Guid.CreateVersion7();
        var fromTier = TrustTier.Unverified;
        var toTier = TrustTier.CommunityTrusted;
        var changeType = TierChangeType.Promotion;

        // Act & Assert
        var act = () => new TrustTierHistoryEntry(
            packageId,
            fromTier,
            toTier,
            string.Empty,
            changeType);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void IsPromotion_ShouldReturnTrue_WhenToTierIsHigherThanFromTier() {
        // Arrange
        var historyEntry = new TrustTierHistoryEntry(
            Guid.CreateVersion7(),
            TrustTier.Unverified,
            TrustTier.CommunityTrusted,
            "Promotion test",
            TierChangeType.Promotion);

        // Act & Assert
        historyEntry.IsPromotion.Should().BeTrue();
        historyEntry.IsDemotion.Should().BeFalse();
    }

    [Fact]
    public void IsDemotion_ShouldReturnTrue_WhenToTierIsLowerThanFromTier() {
        // Arrange
        var historyEntry = new TrustTierHistoryEntry(
            Guid.CreateVersion7(),
            TrustTier.CommunityTrusted,
            TrustTier.Unverified,
            "Demotion test",
            TierChangeType.Demotion);

        // Act & Assert
        historyEntry.IsDemotion.Should().BeTrue();
        historyEntry.IsPromotion.Should().BeFalse();
    }

    [Fact]
    public void ChangeMagnitude_ShouldReturnCorrectAbsoluteDifference() {
        // Arrange
        var historyEntry = new TrustTierHistoryEntry(
            Guid.CreateVersion7(),
            TrustTier.Unverified,
            TrustTier.Enterprise,
            "Large promotion test",
            TierChangeType.Promotion);

        // Act & Assert
        historyEntry.ChangeMagnitude.Should().Be(3); // Enterprise(3) - Unverified(0) = 3
    }

    [Fact]
    public void UpdateMetadata_ShouldAddOrUpdateMetadata_WhenValidMetadataProvided() {
        // Arrange
        var historyEntry = new TrustTierHistoryEntry(
            Guid.CreateVersion7(),
            TrustTier.Unverified,
            TrustTier.CommunityTrusted,
            "Metadata test",
            TierChangeType.Promotion);

        var initialMetadata = new Dictionary<string, object> { { "key1", "value1" } };
        var updateMetadata = new Dictionary<string, object>
        {
            { "key1", "updated_value1" },
            { "key2", "value2" }
        };

        historyEntry.UpdateMetadata(initialMetadata);
        var initialAuditCount = historyEntry.AuditTrail.Count;

        // Act
        historyEntry.UpdateMetadata(updateMetadata, Guid.CreateVersion7());

        // Assert
        historyEntry.Metadata.Should().HaveCount(2);
        historyEntry.Metadata["key1"].Should().Be("updated_value1");
        historyEntry.Metadata["key2"].Should().Be("value2");
        historyEntry.AuditTrail.Should().HaveCount(initialAuditCount + 1);
        historyEntry.AuditTrail.Last().Action.Should().Contain("Trust Tier History Metadata Updated");
    }

    [Fact]
    public void UpdateMetadata_ShouldThrowArgumentNullException_WhenMetadataIsNull() {
        // Arrange
        var historyEntry = new TrustTierHistoryEntry(
            Guid.CreateVersion7(),
            TrustTier.Unverified,
            TrustTier.CommunityTrusted,
            "Null metadata test",
            TierChangeType.Promotion);

        // Act & Assert
        var act = () => historyEntry.UpdateMetadata(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InlineData(TrustTier.Unverified, TrustTier.Unverified, false, false)]
    [InlineData(TrustTier.Unverified, TrustTier.CommunityTrusted, true, false)]
    [InlineData(TrustTier.CommunityTrusted, TrustTier.Verified, true, false)]
    [InlineData(TrustTier.Verified, TrustTier.Enterprise, true, false)]
    [InlineData(TrustTier.Enterprise, TrustTier.Verified, false, true)]
    [InlineData(TrustTier.Verified, TrustTier.CommunityTrusted, false, true)]
    [InlineData(TrustTier.CommunityTrusted, TrustTier.Unverified, false, true)]
    public void PromotionAndDemotionProperties_ShouldReturnCorrectValues_ForDifferentTierTransitions(
        TrustTier fromTier,
        TrustTier toTier,
        bool expectedIsPromotion,
        bool expectedIsDemotion) {
        // Arrange
        var historyEntry = new TrustTierHistoryEntry(
            Guid.CreateVersion7(),
            fromTier,
            toTier,
            "Tier transition test",
            expectedIsPromotion ? TierChangeType.Promotion :
            expectedIsDemotion ? TierChangeType.Demotion : TierChangeType.AutomaticRecalculation);

        // Act & Assert
        historyEntry.IsPromotion.Should().Be(expectedIsPromotion);
        historyEntry.IsDemotion.Should().Be(expectedIsDemotion);
    }
}