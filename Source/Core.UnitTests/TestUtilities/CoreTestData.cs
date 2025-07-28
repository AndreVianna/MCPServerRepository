namespace MCPHub.Core.TestUtilities;

/// <summary>
/// Test data utilities specific to Core project unit tests.
/// </summary>
public static class CoreTestData {
    /// <summary>
    /// Generates a random string of specified length.
    /// </summary>
    /// <param name="length">The length of the string to generate.</param>
    /// <returns>A random string.</returns>
    public static string RandomString(int length = 10) {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    /// <summary>
    /// Generates a random email address.
    /// </summary>
    /// <returns>A random email address.</returns>
    public static string RandomEmail() => $"{RandomString(8)}@{RandomString(5)}.com";

    /// <summary>
    /// Generates a test string with mixed case for testing case conversion.
    /// </summary>
    /// <returns>A mixed case string for testing.</returns>
    public static string MixedCaseString() => "TestStringWithMixedCase";

    /// <summary>
    /// Generates a kebab-case string for testing.
    /// </summary>
    /// <returns>A kebab-case string.</returns>
    public static string KebabCaseString() => "test-string-with-kebab-case";

    /// <summary>
    /// Generates a string with spaces for testing slug generation.
    /// </summary>
    /// <returns>A string with spaces.</returns>
    public static string StringWithSpaces() => "Test String With Spaces";

    /// <summary>
    /// Generates a string with special characters for testing sanitization.
    /// </summary>
    /// <returns>A string with special characters.</returns>
    public static string StringWithSpecialChars() => "Test@String#With$Special%Characters";

    /// <summary>
    /// Generates test data for hash operations.
    /// </summary>
    /// <returns>Test data for hashing.</returns>
    public static string HashTestData() => "This is test data for hashing operations";

    /// <summary>
    /// Generates test byte array for hash operations.
    /// </summary>
    /// <returns>Test byte array.</returns>
    public static byte[] HashTestBytes() => "Test byte array for hashing"u8.ToArray();
}