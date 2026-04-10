using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GrupoEmiTest.Application.Interfaces;
using GrupoEmiTest.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace GrupoEmiTest.Infrastructure.Security;

/// <summary>
/// Generates signed JWT access tokens for authenticated users.
/// Token settings (secret key, issuer, audience, expiry) are read from
/// application configuration under the <c>Jwt</c> section.
/// </summary>
public sealed class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Initialises a new instance of <see cref="TokenService"/>.
    /// </summary>
    /// <param name="configuration">The application configuration.</param>
    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <inheritdoc/>
    public string GenerateToken(ApplicationUser user)
    {
        var jwtSection = _configuration.GetSection("Jwt");
        string secret = jwtSection["Secret"]!;
        string issuer = jwtSection["Issuer"]!;
        string audience = jwtSection["Audience"]!;
        int expiryHours = int.Parse(jwtSection["ExpiryHours"]!);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Name, user.Username),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(expiryHours),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}