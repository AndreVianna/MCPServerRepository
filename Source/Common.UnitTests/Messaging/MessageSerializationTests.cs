using Common.Messaging.Serialization;
using Domain.Events;
using Domain.Commands;

namespace Common.UnitTests.Messaging;

/// <summary>
/// Tests for message serialization and deserialization
/// </summary>
public class MessageSerializationTests
{
    private readonly JsonMessageSerializer _serializer;

    public MessageSerializationTests()
    {
        _serializer = new JsonMessageSerializer();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void SerializeEvent_ShouldReturnValidBytes()
    {
        // Arrange
        var serverRegisteredEvent = new ServerRegisteredEvent(
            Guid.NewGuid().ToString(),
            "test-server",
            "Test server description",
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            "test-user");

        // Act
        var serializedData = _serializer.Serialize(serverRegisteredEvent);

        // Assert
        Assert.NotNull(serializedData);
        Assert.True(serializedData.Length > 0);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void DeserializeEvent_ShouldReturnValidEvent()
    {
        // Arrange
        var originalEvent = new ServerRegisteredEvent(
            Guid.NewGuid().ToString(),
            "test-server",
            "Test server description",
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            "test-user");

        var serializedData = _serializer.Serialize(originalEvent);

        // Act
        var deserializedEvent = _serializer.Deserialize<ServerRegisteredEvent>(serializedData);

        // Assert
        Assert.NotNull(deserializedEvent);
        Assert.Equal(originalEvent.ServerId, deserializedEvent.ServerId);
        Assert.Equal(originalEvent.Name, deserializedEvent.Name);
        Assert.Equal(originalEvent.Description, deserializedEvent.Description);
        Assert.Equal(originalEvent.PublisherId, deserializedEvent.PublisherId);
        Assert.Equal(originalEvent.MessageId, deserializedEvent.MessageId);
        Assert.Equal(originalEvent.CorrelationId, deserializedEvent.CorrelationId);
        Assert.Equal(originalEvent.InitiatedBy, deserializedEvent.InitiatedBy);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void SerializeCommand_ShouldReturnValidBytes()
    {
        // Arrange
        var scanCommand = new ScanServerCommand(
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            "StaticAnalysis",
            Guid.NewGuid().ToString(),
            "test-user");

        // Act
        var serializedData = _serializer.Serialize(scanCommand);

        // Assert
        Assert.NotNull(serializedData);
        Assert.True(serializedData.Length > 0);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void DeserializeCommand_ShouldReturnValidCommand()
    {
        // Arrange
        var originalCommand = new ScanServerCommand(
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            "StaticAnalysis",
            Guid.NewGuid().ToString(),
            "test-user");

        var serializedData = _serializer.Serialize(originalCommand);

        // Act
        var deserializedCommand = _serializer.Deserialize<ScanServerCommand>(serializedData);

        // Assert
        Assert.NotNull(deserializedCommand);
        Assert.Equal(originalCommand.ServerId, deserializedCommand.ServerId);
        Assert.Equal(originalCommand.ServerVersionId, deserializedCommand.ServerVersionId);
        Assert.Equal(originalCommand.ScanType, deserializedCommand.ScanType);
        Assert.Equal(originalCommand.MessageId, deserializedCommand.MessageId);
        Assert.Equal(originalCommand.CorrelationId, deserializedCommand.CorrelationId);
        Assert.Equal(originalCommand.InitiatedBy, deserializedCommand.InitiatedBy);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void DeserializeWithType_ShouldReturnValidObject()
    {
        // Arrange
        var originalEvent = new ServerRegisteredEvent(
            Guid.NewGuid().ToString(),
            "test-server",
            "Test server description",
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            "test-user");

        var serializedData = _serializer.Serialize(originalEvent);

        // Act
        var deserializedObject = _serializer.Deserialize(serializedData, typeof(ServerRegisteredEvent));

        // Assert
        Assert.NotNull(deserializedObject);
        Assert.IsType<ServerRegisteredEvent>(deserializedObject);
        
        var deserializedEvent = (ServerRegisteredEvent)deserializedObject;
        Assert.Equal(originalEvent.ServerId, deserializedEvent.ServerId);
        Assert.Equal(originalEvent.Name, deserializedEvent.Name);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void ContentType_ShouldReturnApplicationJson()
    {
        // Act
        var contentType = _serializer.ContentType;

        // Assert
        Assert.Equal("application/json", contentType);
    }

    [Fact]
    [Trait("Category", "ErrorHandling")]
    public void DeserializeInvalidData_ShouldThrowException()
    {
        // Arrange
        var invalidData = "invalid json data"u8.ToArray();

        // Act & Assert
        Assert.Throws<System.Text.Json.JsonException>(() =>
        {
            _serializer.Deserialize<ServerRegisteredEvent>(invalidData);
        });
    }

    [Fact]
    [Trait("Category", "ErrorHandling")]
    public void DeserializeEmptyData_ShouldThrowException()
    {
        // Arrange
        var emptyData = Array.Empty<byte>();

        // Act & Assert
        Assert.Throws<System.Text.Json.JsonException>(() =>
        {
            _serializer.Deserialize<ServerRegisteredEvent>(emptyData);
        });
    }
}