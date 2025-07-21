using MCPHub.Domain.Events;
using MCPHub.Domain.TestUtilities;

namespace MCPHub.Domain.UnitTests.Events;

[Collection(DomainTestCategories.Event)]
public class UserLoggedInEventTests {
    [Fact]
    public void UserLoggedInEvent_Should_Be_Created_With_Required_Properties() {
        // Arrange
        var userId = "user-123";
        var userName = "testuser";

        // Act
        var eventObj = new UserLoggedInEvent(userId, userName);

        // Assert
        eventObj.UserId.Should().Be(userId);
        eventObj.UserName.Should().Be(userName);
        eventObj.IpAddress.Should().BeNull();
        eventObj.UserAgent.Should().BeNull();
        eventObj.EventType.Should().Be("User");
        eventObj.Version.Should().Be(1);
        eventObj.AggregateId.Should().Be(userId);
    }

    [Fact]
    public void UserLoggedInEvent_Should_Be_Created_With_Optional_Properties() {
        // Arrange
        var userId = "user-123";
        var userName = "testuser";
        var ipAddress = "192.168.1.1";
        var userAgent = "Mozilla/5.0";

        // Act
        var eventObj = new UserLoggedInEvent(userId, userName, ipAddress, userAgent);

        // Assert
        eventObj.UserId.Should().Be(userId);
        eventObj.UserName.Should().Be(userName);
        eventObj.IpAddress.Should().Be(ipAddress);
        eventObj.UserAgent.Should().Be(userAgent);
    }

    [Fact]
    public void UserLoggedInEvent_Should_Be_Created_With_Correlation_Info() {
        // Arrange
        var userId = "user-123";
        var userName = "testuser";
        var correlationId = "correlation-123";
        var initiatedBy = "system";
        var ipAddress = "192.168.1.1";

        // Act
        var eventObj = new UserLoggedInEvent(userId, userName, correlationId, initiatedBy, ipAddress);

        // Assert
        eventObj.CorrelationId.Should().Be(correlationId);
        eventObj.IpAddress.Should().Be(ipAddress);
    }
}