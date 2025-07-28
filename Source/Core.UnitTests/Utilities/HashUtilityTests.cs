namespace MCPHub.Core.Utilities;

public class HashUtilityTests {
    [Fact]
    public void ComputeSha256_WithString_ReturnsValidHash() {
        // Arrange
        const string input = "Hello World";
        const string expectedHash = "a591a6d40bf420404a011733cfb7b190d62c65bf0bcda32b57b277d9ad9f146e";

        // Act
        var result = HashUtility.ComputeSha256(input);

        // Assert
        result.Should().Be(expectedHash);
    }

    [Fact]
    public void ComputeSha256_WithEmptyString_ReturnsValidHash() {
        // Arrange
        const string input = "";
        const string expectedHash = "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855";

        // Act
        var result = HashUtility.ComputeSha256(input);

        // Assert
        result.Should().Be(expectedHash);
    }

    [Fact]
    public void ComputeSha256_WithByteArray_ReturnsValidHash() {
        // Arrange
        var input = Encoding.UTF8.GetBytes("Hello World");
        const string expectedHash = "a591a6d40bf420404a011733cfb7b190d62c65bf0bcda32b57b277d9ad9f146e";

        // Act
        var result = HashUtility.ComputeSha256(input);

        // Assert
        result.Should().Be(expectedHash);
    }

    [Fact]
    public void ComputeSha256_WithEmptyByteArray_ReturnsValidHash() {
        // Arrange
        var input = Array.Empty<byte>();
        const string expectedHash = "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855";

        // Act
        var result = HashUtility.ComputeSha256(input);

        // Assert
        result.Should().Be(expectedHash);
    }

    [Fact]
    public async Task ComputeSha256Async_WithStream_ReturnsValidHash() {
        // Arrange
        var input = new MemoryStream(Encoding.UTF8.GetBytes("Hello World"));
        const string expectedHash = "a591a6d40bf420404a011733cfb7b190d62c65bf0bcda32b57b277d9ad9f146e";

        // Act
        var result = await HashUtility.ComputeSha256Async(input);

        // Assert
        result.Should().Be(expectedHash);
    }

    [Fact]
    public async Task ComputeSha256Async_WithEmptyStream_ReturnsValidHash() {
        // Arrange
        var input = new MemoryStream();
        const string expectedHash = "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855";

        // Act
        var result = await HashUtility.ComputeSha256Async(input);

        // Assert
        result.Should().Be(expectedHash);
    }

    [Theory]
    [InlineData("Hello World", "a591a6d40bf420404a011733cfb7b190d62c65bf0bcda32b57b277d9ad9f146e", true)]
    [InlineData("Hello World", "A591A6D40BF420404A011733CFB7B190D62C65BF0BCDA32B57B277D9AD9F146E", true)]
    [InlineData("Hello World", "wrong-hash", false)]
    [InlineData("", "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855", true)]
    [InlineData("Different Text", "a591a6d40bf420404a011733cfb7b190d62c65bf0bcda32b57b277d9ad9f146e", false)]
    public void VerifySha256_WithString_ReturnsExpectedResult(string input, string expectedHash, bool expected) {
        // Act
        var result = HashUtility.VerifySha256(input, expectedHash);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("Hello World", "a591a6d40bf420404a011733cfb7b190d62c65bf0bcda32b57b277d9ad9f146e", true)]
    [InlineData("Hello World", "A591A6D40BF420404A011733CFB7B190D62C65BF0BCDA32B57B277D9AD9F146E", true)]
    [InlineData("Hello World", "wrong-hash", false)]
    [InlineData("", "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855", true)]
    [InlineData("Different Text", "a591a6d40bf420404a011733cfb7b190d62c65bf0bcda32b57b277d9ad9f146e", false)]
    public void VerifySha256_WithByteArray_ReturnsExpectedResult(string inputText, string expectedHash, bool expected) {
        // Arrange
        var input = Encoding.UTF8.GetBytes(inputText);

        // Act
        var result = HashUtility.VerifySha256(input, expectedHash);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void ComputeSha256_WithSameInput_ReturnsConsistentHash() {
        // Arrange
        const string input = "Consistent Test";

        // Act
        var result1 = HashUtility.ComputeSha256(input);
        var result2 = HashUtility.ComputeSha256(input);

        // Assert
        result1.Should().Be(result2);
    }

    [Fact]
    public void ComputeSha256_WithDifferentInputs_ReturnsDifferentHashes() {
        // Arrange
        const string input1 = "Hello World";
        const string input2 = "Hello World!";

        // Act
        var result1 = HashUtility.ComputeSha256(input1);
        var result2 = HashUtility.ComputeSha256(input2);

        // Assert
        result1.Should().NotBe(result2);
    }

    [Fact]
    public void ComputeSha256_ReturnsLowercaseHexString() {
        // Arrange
        const string input = "Test";

        // Act
        var result = HashUtility.ComputeSha256(input);

        // Assert
        result.Should().MatchRegex("^[a-f0-9]+$");
        result.Should().HaveLength(64); // SHA256 produces 64 hex characters
    }
}