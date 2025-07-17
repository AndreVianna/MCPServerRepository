using System.Security.Cryptography;
using System.Text;

namespace Core.Utilities;

/// <summary>
/// Utility class for hashing operations
/// </summary>
public static class HashUtility
{
    public static string ComputeSha256(string input)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    public static string ComputeSha256(byte[] input)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(input);
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    public static async Task<string> ComputeSha256Async(Stream stream)
    {
        using var sha256 = SHA256.Create();
        var bytes = await sha256.ComputeHashAsync(stream);
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    public static bool VerifySha256(string input, string expectedHash)
    {
        var actualHash = ComputeSha256(input);
        return string.Equals(actualHash, expectedHash, StringComparison.OrdinalIgnoreCase);
    }

    public static bool VerifySha256(byte[] input, string expectedHash)
    {
        var actualHash = ComputeSha256(input);
        return string.Equals(actualHash, expectedHash, StringComparison.OrdinalIgnoreCase);
    }
}