using System.Security.Cryptography;
using System.Text;

namespace titans_admin.Utilities;

/// <summary>
/// Utility class for generating and verifying password hashes
/// </summary>
public static class PasswordHashGenerator
{
    /// <summary>
    /// Generate SHA256 hash for a password
    /// </summary>
    public static string GenerateHash(string password)
    {
        var hashedBytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    /// <summary>
    /// Verify if a password matches a stored hash
    /// </summary>
    public static bool VerifyHash(string password, string storedHash)
    {
        var generatedHash = GenerateHash(password);
        return generatedHash == storedHash;
    }
}
