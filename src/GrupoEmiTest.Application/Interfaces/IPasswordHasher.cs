namespace GrupoEmiTest.Application.Interfaces;

/// <summary>
/// Defines operations for hashing and verifying passwords.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Hashes the given plain-text password using a secure algorithm.
    /// </summary>
    /// <param name="password">The plain-text password to hash.</param>
    /// <returns>A hashed representation of the password.</returns>
    string Hash(string password);

    /// <summary>
    /// Verifies whether the given plain-text password matches the stored hash.
    /// </summary>
    /// <param name="password">The plain-text password to verify.</param>
    /// <param name="hash">The previously stored password hash.</param>
    /// <returns><c>true</c> if the password matches the hash; otherwise, <c>false</c>.</returns>
    bool Verify(string password, string hash);
}