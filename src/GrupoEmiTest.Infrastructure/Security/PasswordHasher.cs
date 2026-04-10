using System.Security.Cryptography;
using GrupoEmiTest.Application.Interfaces;

namespace GrupoEmiTest.Infrastructure.Security;

/// <summary>
/// Implements password hashing and verification using PBKDF2 with HMAC-SHA256.
/// No external library is required — all cryptographic primitives are provided by the .NET BCL.
/// </summary>
public sealed class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;       // 128-bit salt
    private const int HashSize = 32;       // 256-bit hash
    private const int Iterations = 100_000;

    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;

    /// <inheritdoc/>
    public string Hash(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, HashSize);

        // Store salt + hash together, base64-encoded and separated by a colon.
        return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
    }

    /// <inheritdoc/>
    public bool Verify(string password, string storedHash)
    {
        string[] parts = storedHash.Split(':');
        if (parts.Length != 2)
            return false;

        byte[] salt = Convert.FromBase64String(parts[0]);
        byte[] expectedHash = Convert.FromBase64String(parts[1]);

        byte[] actualHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, HashSize);

        // Use a constant-time comparison to prevent timing attacks.
        return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
    }
}