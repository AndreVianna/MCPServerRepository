namespace MCPHub.Common.Messaging;

/// <summary>
/// Simple test for message serialization without complex dependencies
/// </summary>
public class MessageSerializationSimpleTest {
    [Fact]
    public void SerializeMessage_ShouldWorkWithJsonSerializer() {
        // Arrange
        var message = new TestMessage {
            Id = Guid.NewGuid(),
            Content = "Test message",
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var json = JsonSerializer.Serialize(message);
        var bytes = Encoding.UTF8.GetBytes(json);
        var deserializedJson = Encoding.UTF8.GetString(bytes);
        var deserializedMessage = JsonSerializer.Deserialize<TestMessage>(deserializedJson);

        // Assert
        deserializedMessage.Should().NotBeNull();
        deserializedMessage.Id.Should().Be(message.Id);
        deserializedMessage.Content.Should().Be(message.Content);
        deserializedMessage.CreatedAt.ToString().Should().Be(message.CreatedAt.ToString());
    }

    public class TestMessage {
        public Guid Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}