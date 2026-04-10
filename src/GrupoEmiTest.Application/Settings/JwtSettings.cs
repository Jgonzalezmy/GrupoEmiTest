namespace GrupoEmiTest.Application.Settings;

/// <summary>
/// Strongly-typed representation of the <c>Jwt</c> section in application configuration.
/// </summary>
public sealed class JwtSettings
{
    /// <summary>Gets or sets the secret key used to sign JWT tokens.</summary>
    public string Secret { get; set; } = string.Empty;

    /// <summary>Gets or sets the token issuer.</summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>Gets or sets the intended token audience.</summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>Gets or sets the number of hours before the token expires. Defaults to <c>8</c>.</summary>
    public int ExpiryHours { get; set; } = 8;
}
