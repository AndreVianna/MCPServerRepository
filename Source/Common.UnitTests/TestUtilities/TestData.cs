namespace Common.UnitTests.TestUtilities;

/// <summary>
/// Provides common test data and utilities for generating test objects.
/// </summary>
public static class TestData {
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
    /// Generates a random GUID as a string.
    /// </summary>
    /// <returns>A random GUID string.</returns>
    public static string RandomGuid() => Guid.NewGuid().ToString();

    /// <summary>
    /// Generates a random integer between min and max values.
    /// </summary>
    /// <param name="min">Minimum value (inclusive).</param>
    /// <param name="max">Maximum value (exclusive).</param>
    /// <returns>A random integer.</returns>
    public static int RandomInt(int min = 0, int max = 1000) {
        var random = new Random();
        return random.Next(min, max);
    }

    /// <summary>
    /// Generates a random DateTime within the last year.
    /// </summary>
    /// <returns>A random DateTime.</returns>
    public static DateTime RandomDateTime() {
        var random = new Random();
        var start = DateTime.Now.AddYears(-1);
        var range = (DateTime.Now - start).Days;
        return start.AddDays(random.Next(range));
    }

    /// <summary>
    /// Generates a random boolean value.
    /// </summary>
    /// <returns>A random boolean.</returns>
    public static bool RandomBool() {
        var random = new Random();
        return random.Next(2) == 1;
    }

    /// <summary>
    /// Generates a list of random strings.
    /// </summary>
    /// <param name="count">Number of strings to generate.</param>
    /// <param name="length">Length of each string.</param>
    /// <returns>A list of random strings.</returns>
    public static List<string> RandomStringList(int count = 5, int length = 10) => Enumerable.Range(0, count)
            .Select(_ => RandomString(length))
            .ToList();

    /// <summary>
    /// Creates a test JSON configuration.
    /// </summary>
    /// <param name="additionalSettings">Additional settings to include.</param>
    /// <returns>A configuration instance.</returns>
    public static IConfiguration CreateTestConfiguration(Dictionary<string, string>? additionalSettings = null) {
        var settings = new Dictionary<string, string> {
            ["Environment"] = "Test",
            ["ConnectionStrings:Default"] = "Data Source=:memory:",
            ["Logging:LogLevel:Default"] = "Information"
        };

        if (additionalSettings != null) {
            foreach (var setting in additionalSettings) {
                settings[setting.Key] = setting.Value;
            }
        }

        return new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();
    }
}