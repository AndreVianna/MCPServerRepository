using MCPHub.Domain.Entities;
using MCPHub.Domain.TestUtilities;

namespace MCPHub.Domain.UnitTests.Entities;

[Collection(DomainTestCategories.Entity)]
public class ServerTests {
    [Fact]
    public void Server_Should_Be_Created_With_Required_Properties() {
        // Arrange
        var name = "TestServer";
        var description = "Test server description";
        var publisherId = Guid.NewGuid();

        // Act
        var server = new Server(name, description, publisherId);

        // Assert
        server.Name.Should().Be(name);
        server.Description.Should().Be(description);
        server.PublisherId.Should().Be(publisherId);
        server.Repository.Should().BeNull();
        server.License.Should().BeNull();
        server.Tags.Should().NotBeNull().And.BeEmpty();
        server.Status.Should().Be(ServerStatus.Pending);
        server.TrustTier.Should().Be(TrustTier.Unverified);
        server.Id.Should().NotBe(Guid.Empty);
        // Verify AuditTrail contains creation entry
        server.AuditTrail.Should().HaveCount(1);
        var createdEntry = server.AuditTrail.First();
        createdEntry.Action.Should().Be("Created");
        createdEntry.UserId.Should().Be(Guid.Empty); // System action
        createdEntry.DateTime.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Server_Should_Be_Created_With_Optional_Properties() {
        // Arrange
        var name = "TestServer";
        var description = "Test server description";
        var publisherId = Guid.NewGuid();
        var repository = "https://github.com/test/server";
        var license = "MIT";
        var tags = new List<string> { "test", "server", "mcp" };

        // Act
        var server = new Server(name, description, publisherId, repository, license, tags);

        // Assert
        server.Name.Should().Be(name);
        server.Description.Should().Be(description);
        server.PublisherId.Should().Be(publisherId);
        server.Repository.Should().Be(repository);
        server.License.Should().Be(license);
        server.Tags.Should().BeEquivalentTo(tags);
        server.Status.Should().Be(ServerStatus.Pending);
        server.TrustTier.Should().Be(TrustTier.Unverified);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Server_Should_Throw_ArgumentException_For_Invalid_Name(string? invalidName) {
        // Arrange
        var description = "Test server description";
        var publisherId = Guid.NewGuid();

        // Act & Assert
        var action = () => new Server(invalidName!, description, publisherId);
        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Server_Should_Throw_ArgumentException_For_Invalid_Description(string? invalidDescription) {
        // Arrange
        var name = "TestServer";
        var publisherId = Guid.NewGuid();

        // Act & Assert
        var action = () => new Server(name, invalidDescription!, publisherId);
        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(ServerStatus.Pending)]
    [InlineData(ServerStatus.Approved)]
    [InlineData(ServerStatus.Rejected)]
    [InlineData(ServerStatus.Suspended)]
    public void Server_Should_Handle_All_Status_Updates(ServerStatus newStatus) {
        // Arrange
        var server = new Server("TestServer", "Test description", Guid.NewGuid());
        server.Status.Should().Be(ServerStatus.Pending);

        // Act
        server.UpdateStatus(newStatus);

        // Assert
        server.Status.Should().Be(newStatus);
        // Verify AuditTrail contains status update entry
        server.AuditTrail.Should().HaveCount(2); // Created + Status Update
        var statusUpdateEntry = server.AuditTrail.Last();
        statusUpdateEntry.Action.Should().Be($"Status Updated to {newStatus}");
        statusUpdateEntry.UserId.Should().Be(Guid.Empty); // System action
        statusUpdateEntry.DateTime.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Theory]
    [InlineData(TrustTier.Unverified)]
    [InlineData(TrustTier.CommunityTrusted)]
    [InlineData(TrustTier.SecurityAudited)]
    [InlineData(TrustTier.Certified)]
    public void Server_Should_Handle_All_Trust_Tier_Updates(TrustTier newTrustTier) {
        // Arrange
        var server = new Server("TestServer", "Test description", Guid.NewGuid());
        server.TrustTier.Should().Be(TrustTier.Unverified);

        // Act
        server.UpdateTrustTier(newTrustTier);

        // Assert
        server.TrustTier.Should().Be(newTrustTier);
        // Verify AuditTrail contains trust tier update entry
        server.AuditTrail.Should().HaveCount(2); // Created + Trust Tier Update
        var trustTierUpdateEntry = server.AuditTrail.Last();
        trustTierUpdateEntry.Action.Should().Be($"Trust Tier Updated to {newTrustTier}");
        trustTierUpdateEntry.UserId.Should().Be(Guid.Empty); // System action
        trustTierUpdateEntry.DateTime.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Server_Should_Handle_Details_Update() {
        // Arrange
        var server = new Server("TestServer", "Original description", Guid.NewGuid());
        var newDescription = "Updated description";
        var newRepository = "https://github.com/updated/server";
        var newLicense = "Apache-2.0";
        var newTags = new List<string> { "updated", "tags" };

        // Act
        server.UpdateDetails(newDescription, newRepository, newLicense, newTags);

        // Assert
        server.Description.Should().Be(newDescription);
        server.Repository.Should().Be(newRepository);
        server.License.Should().Be(newLicense);
        server.Tags.Should().BeEquivalentTo(newTags);
        // Verify AuditTrail contains details update entry
        server.AuditTrail.Should().HaveCount(2); // Created + Details Update
        var detailsUpdateEntry = server.AuditTrail.Last();
        detailsUpdateEntry.Action.Should().Be("Details Updated");
        detailsUpdateEntry.UserId.Should().Be(Guid.Empty); // System action
        detailsUpdateEntry.DateTime.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Server_UpdateDetails_Should_Throw_ArgumentException_For_Invalid_Description(string? invalidDescription) {
        // Arrange
        var server = new Server("TestServer", "Original description", Guid.NewGuid());

        // Act & Assert
        var action = () => server.UpdateDetails(invalidDescription!, null, null, null);
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Server_Should_Handle_Null_Tags_In_Constructor() {
        // Arrange & Act
        var server = new Server("TestServer", "Test description", Guid.NewGuid(), tags: null);

        // Assert
        server.Tags.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void Server_Should_Handle_Null_Tags_In_UpdateDetails() {
        // Arrange
        var server = new Server("TestServer", "Original description", Guid.NewGuid());

        // Act
        server.UpdateDetails("New description", null, null, null);

        // Assert
        server.Tags.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void Server_Should_Initialize_Collections() {
        // Arrange & Act
        var server = new Server("TestServer", "Test description", Guid.NewGuid());

        // Assert
        server.Versions.Should().NotBeNull().And.BeEmpty();
        server.Tags.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void Server_Should_Have_PublisherId_Relationship() {
        // Arrange
        var publisherId = Guid.NewGuid();
        var server = new Server("TestServer", "Test description", publisherId);

        // Act & Assert
        server.PublisherId.Should().Be(publisherId);
        server.Publisher.Should().BeNull(); // EF will populate this when loaded from DB
    }
}