using MCPHub.Domain.Entities;
using MCPHub.Domain.TestUtilities;
using MCPHub.Domain.ValueObjects;

namespace MCPHub.Domain.UnitTests.Entities;

[Collection(DomainTestCategories.Entity)]
public class PackageTests {
    [Fact]
    public void Package_Should_Be_Created_With_Required_Properties() {
        // Arrange
        var name = "TestPackage";
        var description = "Test package description";
        var version = "1.0.0";
        var publisherId = Guid.NewGuid();

        // Act
        var package = new Package(name, description, version, publisherId);

        // Assert
        package.Name.Should().Be(name);
        package.Description.Should().Be(description);
        package.Version.Should().Be(version);
        package.PublisherId.Should().Be(publisherId);
        package.Repository.Should().BeNull();
        package.License.Should().BeNull();
        package.Tags.Should().NotBeNull().And.BeEmpty();
        package.Status.Should().Be(PackageStatus.Pending);
        package.TrustTier.Should().Be(TrustTier.Unverified);
        package.ScanResult.Should().BeNull();
        package.Id.Should().NotBe(Guid.Empty);
        // Verify AuditTrail contains creation entry
        package.AuditTrail.Should().HaveCount(1);
        var createdEntry = package.AuditTrail.First();
        createdEntry.Action.Should().Be("Created");
        createdEntry.UserId.Should().Be(Guid.Empty);
        createdEntry.DateTime.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Package_Should_Be_Created_With_Optional_Properties() {
        // Arrange
        var name = "TestPackage";
        var description = "Test package description";
        var version = "2.1.0";
        var publisherId = Guid.NewGuid();
        var repository = "https://github.com/test/package";
        var license = "MIT";
        var tags = new List<string> { "mcp", "test", "package" };

        // Act
        var package = new Package(name, description, version, publisherId, repository, license, tags);

        // Assert
        package.Name.Should().Be(name);
        package.Description.Should().Be(description);
        package.Version.Should().Be(version);
        package.PublisherId.Should().Be(publisherId);
        package.Repository.Should().Be(repository);
        package.License.Should().Be(license);
        package.Tags.Should().BeEquivalentTo(tags);
        package.Status.Should().Be(PackageStatus.Pending);
        package.TrustTier.Should().Be(TrustTier.Unverified);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Package_Should_Throw_ArgumentException_For_Invalid_Name(string? invalidName) {
        // Arrange
        var description = "Test package description";
        var version = "1.0.0";
        var publisherId = Guid.NewGuid();

        // Act & Assert
        var action = () => new Package(invalidName!, description, version, publisherId);
        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Package_Should_Throw_ArgumentException_For_Invalid_Description(string? invalidDescription) {
        // Arrange
        var name = "TestPackage";
        var version = "1.0.0";
        var publisherId = Guid.NewGuid();

        // Act & Assert
        var action = () => new Package(name, invalidDescription!, version, publisherId);
        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Package_Should_Throw_ArgumentException_For_Invalid_Version(string? invalidVersion) {
        // Arrange
        var name = "TestPackage";
        var description = "Test description";
        var publisherId = Guid.NewGuid();

        // Act & Assert
        var action = () => new Package(name, description, invalidVersion!, publisherId);
        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(PackageStatus.Pending)]
    [InlineData(PackageStatus.Approved)]
    [InlineData(PackageStatus.Rejected)]
    [InlineData(PackageStatus.Suspended)]
    public void Package_Should_Handle_All_Status_Updates(PackageStatus newStatus) {
        // Arrange
        var package = new Package("TestPackage", "Test description", "1.0.0", Guid.NewGuid());
        package.Status.Should().Be(PackageStatus.Pending);

        // Act
        package.UpdateStatus(newStatus);

        // Assert
        package.Status.Should().Be(newStatus);
        // Verify AuditTrail contains status update entry
        package.AuditTrail.Should().HaveCount(2); // Created + Status Update
        var statusUpdateEntry = package.AuditTrail.Last();
        statusUpdateEntry.Action.Should().Be($"Status Updated to {newStatus}");
        statusUpdateEntry.UserId.Should().Be(Guid.Empty); // System action
        statusUpdateEntry.DateTime.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Theory]
    [InlineData(TrustTier.Unverified)]
    [InlineData(TrustTier.CommunityTrusted)]
    [InlineData(TrustTier.SecurityAudited)]
    [InlineData(TrustTier.Certified)]
    public void Package_Should_Handle_All_Trust_Tier_Updates(TrustTier newTrustTier) {
        // Arrange
        var package = new Package("TestPackage", "Test description", "1.0.0", Guid.NewGuid());
        package.TrustTier.Should().Be(TrustTier.Unverified);

        // Act
        package.UpdateTrustTier(newTrustTier);

        // Assert
        package.TrustTier.Should().Be(newTrustTier);
        // Verify AuditTrail contains trust tier update entry
        package.AuditTrail.Should().HaveCount(2); // Created + Trust Tier Update
        var trustTierUpdateEntry = package.AuditTrail.Last();
        trustTierUpdateEntry.Action.Should().Be($"Trust Tier Updated to {newTrustTier}");
        trustTierUpdateEntry.UserId.Should().Be(Guid.Empty); // System action
        trustTierUpdateEntry.DateTime.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Package_Should_Handle_Security_Scan_Update() {
        // Arrange
        var package = new Package("TestPackage", "Test description", "1.0.0", Guid.NewGuid());
        var scanResult = new SecurityScanResult(
            SecurityScanStatus.Passed,
            [],
            "Scanner v1.0"
        );

        // Act
        package.UpdateSecurityScan(scanResult);

        // Assert
        package.ScanResult.Should().Be(scanResult);
        // Verify AuditTrail contains security scan update entry
        package.AuditTrail.Should().HaveCount(2); // Created + Security Scan Update
        var scanUpdateEntry = package.AuditTrail.Last();
        scanUpdateEntry.Action.Should().Be("Security Scan Updated");
        scanUpdateEntry.UserId.Should().Be(Guid.Empty); // System action
        scanUpdateEntry.DateTime.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Package_Should_Handle_Null_Tags_In_Constructor() {
        // Arrange & Act
        var package = new Package("TestPackage", "Test description", "1.0.0", Guid.NewGuid(), tags: null);

        // Assert
        package.Tags.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void Package_Should_Initialize_Collections() {
        // Arrange & Act
        var package = new Package("TestPackage", "Test description", "1.0.0", Guid.NewGuid());

        // Assert
        package.Versions.Should().NotBeNull().And.BeEmpty();
        package.Tags.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void Package_Should_Have_PublisherId_Relationship() {
        // Arrange
        var publisherId = Guid.NewGuid();
        var package = new Package("TestPackage", "Test description", "1.0.0", publisherId);

        // Act & Assert
        package.PublisherId.Should().Be(publisherId);
        package.Publisher.Should().BeNull(); // EF will populate this when loaded from DB
    }

    [Fact]
    public void Package_Should_Handle_Null_Optional_Parameters() {
        // Arrange & Act
        var package = new Package("TestPackage", "Test description", "1.0.0", Guid.NewGuid());

        // Assert
        package.Repository.Should().BeNull();
        package.License.Should().BeNull();
        package.Tags.Should().NotBeNull().And.BeEmpty();
    }
}