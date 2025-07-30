using MCPHub.Domain.Entities;
using MCPHub.Domain.TestUtilities;

namespace MCPHub.Domain.UnitTests.Entities;

[Collection(DomainTestCategories.Entity)]
public class PublisherTests {
    [Fact]
    public void Publisher_Should_Be_Created_With_Required_Properties() {
        // Arrange
        var name = "Test Publisher";
        var email = "test@example.com";
        var type = PublisherType.Individual;

        // Act
        var publisher = new Publisher(name, email, type);

        // Assert
        publisher.Name.Should().Be(name);
        publisher.Email.Should().Be(email);
        publisher.Type.Should().Be(type);
        publisher.OrganizationName.Should().BeNull();
        publisher.Website.Should().BeNull();
        publisher.User.Should().BeNull();
        publisher.UserId.Should().BeNull();
        publisher.Verified.Should().BeFalse();
        publisher.Id.Should().NotBe(Guid.Empty);
        // Verify AuditTrail contains creation entry
        publisher.AuditTrail.Should().HaveCount(1);
        var createdEntry = publisher.AuditTrail.First();
        createdEntry.Action.Should().Be("Created");
        createdEntry.UserId.Should().Be(Guid.Empty); // No user provided
        createdEntry.DateTime.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Publisher_Should_Be_Created_With_Optional_Properties() {
        // Arrange
        var name = "Test Organization";
        var email = "test@organization.com";
        var type = PublisherType.Organization;
        var organizationName = "Test Organization Inc.";
        var website = "https://test.com";

        // Act
        var publisher = new Publisher(name, email, type, organizationName, website);

        // Assert
        publisher.Name.Should().Be(name);
        publisher.Email.Should().Be(email);
        publisher.Type.Should().Be(type);
        publisher.OrganizationName.Should().Be(organizationName);
        publisher.Website.Should().Be(website);
        publisher.Verified.Should().BeFalse();
    }

    [Fact]
    public void Publisher_Should_Be_Created_With_ApplicationUser() {
        // Arrange
        var user = new ApplicationUser("testuser", "test@example.com");
        var name = "Test Publisher";
        var email = "test@example.com";
        var type = PublisherType.Individual;

        // Act
        var publisher = new Publisher(name, email, type, user: user);

        // Assert
        publisher.Name.Should().Be(name);
        publisher.Email.Should().Be(email);
        publisher.Type.Should().Be(type);
        publisher.User.Should().Be(user);
        publisher.UserId.Should().Be(user.Id);
        publisher.Verified.Should().BeFalse();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Publisher_Should_Throw_ArgumentException_For_Invalid_Name(string? invalidName) {
        // Arrange
        var email = "test@example.com";
        var type = PublisherType.Individual;

        // Act & Assert
        var action = () => new Publisher(invalidName!, email, type);
        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Publisher_Should_Throw_ArgumentException_For_Invalid_Email(string? invalidEmail) {
        // Arrange
        var name = "Test Publisher";
        var type = PublisherType.Individual;

        // Act & Assert
        var action = () => new Publisher(name, invalidEmail!, type);
        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(PublisherType.Individual)]
    [InlineData(PublisherType.Organization)]
    [InlineData(PublisherType.Enterprise)]
    public void Publisher_Should_Handle_All_Publisher_Types(PublisherType type) {
        // Arrange
        var name = "Test Publisher";
        var email = "test@example.com";

        // Act
        var publisher = new Publisher(name, email, type);

        // Assert
        publisher.Type.Should().Be(type);
        publisher.Name.Should().Be(name);
        publisher.Email.Should().Be(email);
    }

    [Fact]
    public void Publisher_Should_Handle_Verification() {
        // Arrange
        var publisher = new Publisher("Test Publisher", "test@example.com", PublisherType.Individual);
        publisher.Verified.Should().BeFalse();

        // Act
        publisher.Verify();

        // Assert
        publisher.Verified.Should().BeTrue();
        // Verify AuditTrail contains verification entry
        publisher.AuditTrail.Should().HaveCount(2); // Created + Verified
        var verificationEntry = publisher.AuditTrail.Last();
        verificationEntry.Action.Should().Be("Verified");
        verificationEntry.UserId.Should().Be(Guid.Empty); // No user provided
        verificationEntry.DateTime.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Publisher_Should_Handle_Details_Update() {
        // Arrange
        var publisher = new Publisher("Test Publisher", "test@example.com", PublisherType.Organization);
        var newOrganizationName = "New Organization Name";
        var newWebsite = "https://newwebsite.com";

        // Act
        publisher.UpdateDetails(newOrganizationName, newWebsite);

        // Assert
        publisher.OrganizationName.Should().Be(newOrganizationName);
        publisher.Website.Should().Be(newWebsite);
        // Verify AuditTrail contains details update entry
        publisher.AuditTrail.Should().HaveCount(2); // Created + Details Updated
        var updateEntry = publisher.AuditTrail.Last();
        updateEntry.Action.Should().Be("Details Updated");
        updateEntry.UserId.Should().Be(Guid.Empty); // No user provided
        updateEntry.DateTime.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Publisher_Should_Handle_User_Relationship() {
        // Arrange
        var user = new ApplicationUser("publisheruser", "publisher@example.com");
        user.EnablePublisher();

        // Act
        var publisher = new Publisher("Publisher Name", "publisher@example.com", PublisherType.Organization, user: user);

        // Assert
        publisher.User.Should().Be(user);
        publisher.UserId.Should().Be(user.Id);
        user.IsPublisher.Should().BeTrue();
    }

    [Fact]
    public void Publisher_Should_Initialize_Collections() {
        // Arrange & Act
        var publisher = new Publisher("Test Publisher", "test@example.com", PublisherType.Individual);

        // Assert
        publisher.Servers.Should().NotBeNull().And.BeEmpty();
        publisher.Packages.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void Publisher_Should_Handle_Null_Optional_Parameters() {
        // Arrange & Act
        var publisher = new Publisher("Test Publisher", "test@example.com", PublisherType.Individual);

        // Assert
        publisher.OrganizationName.Should().BeNull();
        publisher.Website.Should().BeNull();
        publisher.User.Should().BeNull();
        publisher.UserId.Should().BeNull();
    }
}