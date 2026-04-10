using GrupoEmiTest.Domain.Enums;

namespace GrupoEmiTest.Application.DTOs.Request;

/// <summary>
/// Represents the data required to register a new <see cref="GrupoEmiTest.Domain.Entities.ApplicationUser"/>.
/// </summary>
/// <param name="Username">The unique username used for login.</param>
/// <param name="Email">The email address of the user.</param>
/// <param name="Password">The plain-text password that will be hashed before storage.</param>
/// <param name="Role">The role assigned to the user. See <see cref="RoleType"/> for valid values.</param>
public record RegisterRequest(
    string Username,
    string Email,
    string Password,
    RoleType Role
    );

