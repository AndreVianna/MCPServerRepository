using MCPHub.Domain.Entities;
using MCPHub.Domain.ValueObjects;

namespace MCPHub.Domain.TestUtilities;

/// <summary>
/// Provides test data for Domain unit tests without external dependencies.
/// This class follows the pure xUnit pattern without BaseTest dependencies.
/// </summary>
public static class DomainTestData {
    private static readonly Random Random = new();

    public static string RandomString(int length = 10) {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[Random.Next(s.Length)]).ToArray());
    }

    public static string RandomEmail() => $"{RandomString(8)}@{RandomString(6)}.com";

    public static Guid RandomGuid() => Guid.NewGuid();

    public static List<string> RandomStringList(int count = 3) => Enumerable.Range(0, count)
            .Select(_ => RandomString())
            .ToList();

    public static bool RandomBool() => Random.NextDouble() > 0.5;

    public static int RandomInt(int min = 0, int max = 100) => Random.Next(min, max);

    public static Package CreateValidPackage() => new(
            RandomString(10),
            RandomString(50),
            "1.0.0",
            RandomGuid(),
            $"https://github.com/{RandomString(10)}/{RandomString(10)}",
            "MIT",
            RandomStringList(3));

    public static Server CreateValidServer() => new(
            RandomString(10),
            RandomString(50),
            RandomGuid(),
            $"https://github.com/{RandomString(10)}/{RandomString(10)}",
            "MIT",
            RandomStringList(3));

    public static Publisher CreateValidPublisher() => new(
            RandomString(10),
            RandomEmail(),
            PublisherType.Individual,
            RandomString(20),
            $"https://{RandomString(10)}.com");

    public static SecurityVulnerability CreateValidVulnerability(SecurityScanSeverity severity = SecurityScanSeverity.Low) => new(
            $"VULN-{RandomString(6)}",
            RandomString(20),
            RandomString(100),
            severity,
            $"CVE-2023-{RandomInt(10000, 99999)}",
            $"https://example.com/vuln/{RandomString(10)}");

    public static SecurityScanResult CreateValidScanResult(SecurityScanStatus status = SecurityScanStatus.Passed) {
        var vulnerabilities = status == SecurityScanStatus.Passed
            ? new List<SecurityVulnerability>()
            : new List<SecurityVulnerability> { CreateValidVulnerability() };

        return new SecurityScanResult(
            status,
            vulnerabilities,
            $"Scanner v{RandomInt(1, 5)}.{RandomInt(0, 9)}",
            RandomString(200));
    }
}