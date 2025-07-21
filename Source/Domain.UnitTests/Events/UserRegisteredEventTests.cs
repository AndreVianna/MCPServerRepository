using MCPHub.Domain.Events;
using MCPHub.Domain.TestUtilities;

namespace MCPHub.Domain.UnitTests.Events;

[Collection(DomainTestCategories.Event)]
public class UserRegisteredEventTests {
    [Fact]
    public void UserRegisteredEvent_Should_Be_Created_With_Required_Properties() {
        // Arrange
        var userId = "user-123";
        var userName = "testuser";
        var email = "test@example.com";

        // Act
        var eventObj = new UserRegisteredEvent(userId, userName, email);

        // Assert
        eventObj.UserId.Should().Be(userId);
        eventObj.UserName.Should().Be(userName);
        eventObj.Email.Should().Be(email);
        eventObj.EventType.Should().Be("User");
        eventObj.Version.Should().Be(1);
        eventObj.AggregateId.Should().Be(userId);
        eventObj.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void UserRegisteredEvent_Should_Be_Created_With_Correlation_Info() {
        // Arrange
        var userId = "user-123";
        var userName = "testuser";
        var email = "test@example.com";
        var correlationId = "correlation-123";
        var initiatedBy = "admin";

        // Act
        var eventObj = new UserRegisteredEvent(userId, userName, email, correlationId, initiatedBy);

        // Assert
        eventObj.UserId.Should().Be(userId);
        eventObj.UserName.Should().Be(userName);
        eventObj.Email.Should().Be(email);
        eventObj.EventType.Should().Be("User");
        eventObj.Version.Should().Be(1);
        eventObj.AggregateId.Should().Be(userId);
        eventObj.CorrelationId.Should().Be(correlationId);
        eventObj.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void UserRegisteredEvent_Should_Have_Unique_Id() {
        // Arrange & Act
        var event1 = new UserRegisteredEvent("user1", "user1", "user1@example.com");
        var event2 = new UserRegisteredEvent("user2", "user2", "user2@example.com");

        // Assert
        event1.Id.Should().NotBe(event2.Id);
    }
}