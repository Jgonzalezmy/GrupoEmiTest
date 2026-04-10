using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GrupoEmiTest.Application.Interfaces;
using GrupoEmiTest.Application.Settings;
using GrupoEmiTest.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace GrupoEmiTest.Infrastructure.Security;

/// <summary>
/// Generates signed JWT access tokens for authenticated users.
/// Token settings (secret key, issuer, audience, expiry) are read from
/// <see cref="JwtSettings"/> injected via <see cref="IOptions{TOptions}"/>.
/// </summary>
public sealed class TokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;

    /// <summary>
    /// Initialises a new instance of <see cref="TokenService"/>.
    /// </summary>
    /// <param name="jwtSettings">The JWT configuration settings.</param>
    public TokenService(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }

    /// <inheritdoc/>
    public string GenerateToken(ApplicationUser user)
    {
        string secret = _jwtSettings.Secret;
        string issuer = _jwtSettings.Issuer;
        string audience = _jwtSettings.Audience;
        int expiryHours = _jwtSettings.ExpiryHours;

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