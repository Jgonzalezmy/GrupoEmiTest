namespace GrupoEmiTest.Application.DTOs.Response;

/// <summary>
/// Represents the response returned after a successful authentication request.
/// </summary>
/// <param name="Token">The JWT bearer token used to authenticate subsequent requests.</param>
/// <param name="Username">The username of the authenticated user.</param>
/// <param name="Role">The role assigned to the authenticated user.</param>
/// <param name="ExpiresAt">The UTC date and time at which the token expires.</param>
public record AuthResponse(
    string Token,
    string Username,
    string Role,
    DateTime ExpiresAt
);


