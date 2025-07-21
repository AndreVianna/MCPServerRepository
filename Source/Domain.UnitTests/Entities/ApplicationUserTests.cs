using MCPHub.Domain.Entities;
using MCPHub.Domain.TestUtilities;

namespace MCPHub.Domain.UnitTests.Entities;

[Collection(DomainTestCategories.Entity)]
public class ApplicationUserTests {
    [Fact]
    public void ApplicationUser_Should_Be_Created_With_Required_Properties() {
        // Arrange
        var userName = "testuser";
        var email = "test@example.com";

        // Act
        var user = new ApplicationUser(userName, email);

        // Assert
        user.UserName.Should().Be(userName);
        user.Email.Should().Be(email);
        // Verify AuditTrail contains creation entry
        user.AuditTrail.Should().HaveCount(1);
        var createdEntry = user.AuditTrail.First();
        createdEntry.Action.Should().Be("Created");
        createdEntry.UserId.Should().Be(user.Id);
        createdEntry.DateTime.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
        user.IsEmailVerified.Should().BeFalse();
        user.IsPublisher.Should().BeFalse();
        user.Publishers.Should().BeEmpty();
    }

    [Theory]
    [InlineData(null, "test@example.com")]
    [InlineData("", "test@example.com")]
    [InlineData("   ", "test@example.com")]
    [InlineData("testuser", null)]
    [InlineData("testuser", "")]
    [InlineData("testuser", "   ")]
    public void ApplicationUser_Should_Throw_ArgumentException_When_Required_Properties_Are_Invalid(string? userName, string? email) {
        // Act & Assert
        var act = () => new ApplicationUser(userName!, email!);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void UpdateProfile_Should_Update_Properties_And_AuditTrail() {
        // Arrange
        var user = new ApplicationUser("testuser", "test@example.com");
        var originalAuditCount = user.AuditTrail.Count;
        Thread.Sleep(1); // Ensure time difference

        var displayName = "Test User";
        var gitHubUsername = "testuser";
        var twitterHandle = "testuser";
        var website = "https://example.com";
        var bio = "Test bio";
        var avatarUrl = "https://example.com/avatar.jpg";

        // Act
        user.UpdateProfile(displayName, gitHubUsername, twitterHandle, website, bio, avatarUrl);

        // Assert
        user.DisplayName.Should().Be(displayName);
        user.GitHubUsername.Should().Be(gitHubUsername);
        user.TwitterHandle.Should().Be(twitterHandle);
        user.Website.Should().Be(website);
        user.Bio.Should().Be(bio);
        user.AvatarUrl.Should().Be(avatarUrl);
        // Verify AuditTrail contains profile update entry
        user.AuditTrail.Should().HaveCount(originalAuditCount + 1);
        var profileUpdateEntry = user.AuditTrail.Last();
        profileUpdateEntry.Action.Should().Be("Profile Updated");
        profileUpdateEntry.UserId.Should().Be(user.Id);
        profileUpdateEntry.DateTime.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void UpdateProfile_Should_Handle_Null_Optional_Parameters() {
        // Arrange
        var user = new ApplicationUser("testuser", "test@example.com");

        // Act
        user.UpdateProfile();

        // Assert
        user.DisplayName.Should().BeNull();
        user.GitHubUsername.Should().BeNull();
        user.TwitterHandle.Should().BeNull();
        user.Website.Should().BeNull();
        user.Bio.Should().BeNull();
        user.AvatarUrl.Should().BeNull();
    }

    [Fact]
    public void VerifyEmail_Should_Set_IsEmailVerified_And_Update_AuditTrail() {
        // Arrange
        var user = new ApplicationUser("testuser", "test@example.com");
        var originalAuditCount = user.AuditTrail.Count;
        Thread.Sleep(1); // Ensure time difference

        // Act
        user.VerifyEmail();

        // Assert
        user.IsEmailVerified.Should().BeTrue();
        // Verify AuditTrail contains email verification entry
        user.AuditTrail.Should().HaveCount(originalAuditCount + 1);
        var emailVerifyEntry = user.AuditTrail.Last();
        emailVerifyEntry.Action.Should().Be("Email Verified");
        emailVerifyEntry.UserId.Should().Be(user.Id);
        emailVerifyEntry.DateTime.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void EnablePublisher_Should_Set_IsPublisher_And_Update_AuditTrail() {
        // Arrange
        var user = new ApplicationUser("testuser", "test@example.com");
        var originalAuditCount = user.AuditTrail.Count;
        Thread.Sleep(1); // Ensure time difference

        // Act
        user.EnablePublisher();

        // Assert
        user.IsPublisher.Should().BeTrue();
        // Verify AuditTrail contains publisher enable entry
        user.AuditTrail.Should().HaveCount(originalAuditCount + 1);
        var publisherEnableEntry = user.AuditTrail.Last();
        publisherEnableEntry.Action.Should().Be("Publisher Enabled");
        publisherEnableEntry.UserId.Should().Be(user.Id);
        publisherEnableEntry.DateTime.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void ApplicationUser_Should_Have_Empty_Publishers_Collection_Initially() {
        // Arrange & Act
        var user = new ApplicationUser("testuser", "test@example.com");

        // Assert
        user.Publishers.Should().NotBeNull();
        user.Publishers.Should().BeEmpty();
    }

    [Fact]
    public void ApplicationUser_Should_Set_Initial_AuditTrail_Entry() {
        // Arrange & Act
        var user = new ApplicationUser("testuser", "test@example.com");

        // Assert
        // Verify AuditTrail contains creation entry with consistent timing
        user.AuditTrail.Should().HaveCount(1);
        var createdEntry = user.AuditTrail.First();
        createdEntry.Action.Should().Be("Created");
        createdEntry.UserId.Should().Be(user.Id);
        createdEntry.DateTime.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
    }
}