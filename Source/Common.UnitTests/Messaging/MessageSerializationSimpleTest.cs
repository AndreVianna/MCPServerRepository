using System.Text.Json;
using System.Text;

namespace Common.UnitTests.Messaging;

/// <summary>
/// Simple test for message serialization without complex dependencies
/// </summary>
public class MessageSerializationSimpleTest
{
    [Fact]
    public void SerializeMessage_ShouldWorkWithJsonSerializer()
    {
        // Arrange
        var message = new TestMessage 
        { 
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
        Assert.NotNull(deserializedMessage);
        Assert.Equal(message.Id, deserializedMessage.Id);
        Assert.Equal(message.Content, deserializedMessage.Content);
        Assert.Equal(message.CreatedAt.ToString(), deserializedMessage.CreatedAt.ToString());
    }

    public class TestMessage
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}