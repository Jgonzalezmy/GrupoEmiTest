using GrupoEmiTest.Domain.Entities;

namespace GrupoEmiTest.Application.Interfaces;

/// <summary>
/// Defines the contract for generating JWT access tokens.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Generates a signed JWT token containing the user's identity and role claims.
    /// </summary>
    /// <param name="user">The authenticated user for whom the token is generated.</param>
    /// <returns>A signed JWT string.</returns>
    string GenerateToken(ApplicationUser user);
}