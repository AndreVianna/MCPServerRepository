namespace MCPHub.Core.Helpers;

public class StringHelperTests {
    [Fact]
    public void Truncate_WithNullOrEmpty_ReturnsOriginalValue() {
        // Arrange & Act & Assert
        string.Empty.Truncate(10).Should().Be(string.Empty);
        ((string)null!).Truncate(10).Should().BeNull();
    }

    [Fact]
    public void Truncate_WithValueShorterThanMaxLength_ReturnsOriginalValue() {
        // Arrange
        const string value = "short";

        // Act
        var result = value.Truncate(10);

        // Assert
        result.Should().Be("short");
    }

    [Fact]
    public void Truncate_WithValueLongerThanMaxLength_ReturnsTruncatedValue() {
        // Arrange
        const string value = "this is a very long string";

        // Act
        var result = value.Truncate(10);

        // Assert
        result.Should().Be("this is a ");
    }

    [Theory]
    [InlineData("", "")]
    [InlineData(null, null)]
    [InlineData("helloWorld", "hello-world")]
    [InlineData("HelloWorld", "hello-world")]
    [InlineData("XMLHttpRequest", "xmlhttp-request")]
    [InlineData("iPhone", "i-phone")]
    [InlineData("already-kebab", "already-kebab")]
    public void ToKebabCase_WithVariousInputs_ReturnsKebabCaseString(string? input, string? expected) {
        // Act
        var result = input!.ToKebabCase();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("", "")]
    [InlineData(null, null)]
    [InlineData("hello-world", "HelloWorld")]
    [InlineData("hello_world", "HelloWorld")]
    [InlineData("hello world", "HelloWorld")]
    [InlineData("hello--world", "HelloWorld")]
    [InlineData("single", "Single")]
    [InlineData("already-PascalCase", "AlreadyPascalcase")]
    public void ToPascalCase_WithVariousInputs_ReturnsPascalCaseString(string? input, string? expected) {
        // Act
        var result = input!.ToPascalCase();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("test@example.com", true)]
    [InlineData("user.name@domain.co.uk", true)]
    [InlineData("user+tag@example.com", true)]
    [InlineData("", false)]
    [InlineData(null, false)]
    [InlineData("   ", false)]
    [InlineData("invalid-email", false)]
    [InlineData("@example.com", false)]
    [InlineData("user@", false)]
    [InlineData("user@.com", false)]
    [InlineData("user name@example.com", false)]
    public void IsValidEmail_WithVariousInputs_ReturnsExpectedResult(string? input, bool expected) {
        // Act
        var result = input!.IsValidEmail();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("", "")]
    [InlineData(null, null)]
    [InlineData("Hello World", "hello-world")]
    [InlineData("Hello  World", "hello-world")]
    [InlineData("Hello-World", "hello-world")]
    [InlineData("Hello_World", "helloworld")]
    [InlineData("Hello@World#Test", "helloworldtest")]
    [InlineData("  Hello World  ", "hello-world")]
    [InlineData("Hello---World", "hello-world")]
    [InlineData("Hello   World   Test", "hello-world-test")]
    [InlineData("123 Test", "123-test")]
    [InlineData("Test 123", "test-123")]
    public void GenerateSlug_WithVariousInputs_ReturnsSlugString(string? input, string? expected) {
        // Act
        var result = input!.GenerateSlug();

        // Assert
        result.Should().Be(expected);
    }
}