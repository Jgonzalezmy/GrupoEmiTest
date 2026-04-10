using GrupoEmiTest.Domain.Common;
using GrupoEmiTest.Domain.Enums;
using GrupoEmiTest.Domain.Errors;

namespace GrupoEmiTest.Domain.Entities;

/// <summary>
/// Represents an application user with authentication credentials and an assigned role.
/// </summary>
public class ApplicationUser
{
    /// <summary>Gets the unique identifier of the user.</summary>
    public int Id { get; private set; }

    /// <summary>Gets the username used for login.</summary>
    public string Username { get; private set; } = string.Empty;

    /// <summary>Gets the email address of the user.</summary>
    public string Email { get; private set; } = string.Empty;

    /// <summary>Gets the hashed password of the user.</summary>
    public string PasswordHash { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the role assigned to the user.
    /// Valid values are <c>"Admin"</c> and <c>"User"</c>.
    /// </summary>
    public RoleType Role { get; private set; } = RoleType.User;

    /// <summary>
    /// Creates a new <see cref="ApplicationUser"/> with the specified credentials and role.
    /// </summary>
    /// <param name="username">The username used for login.</param>
    /// <param name="email">The email address of the user.</param>
    /// <param name="passwordHash">The already-hashed password.</param>
    /// <param name="role">The role assigned to the user. Defaults to <see cref="RoleType.User"/>.</param>
    /// <returns>A new <see cref="ApplicationUser"/> instance.</returns>
    public static Result<ApplicationUser> Create(string username, string email, string passwordHash, RoleType role = RoleType.User)
    {
        if (string.IsNullOrWhiteSpace(username)) return AuthErrors.InvalidUsername;
        if (string.IsNullOrWhiteSpace(email)) return AuthErrors.InvalidEmail;
        if (string.IsNullOrWhiteSpace(passwordHash)) return AuthErrors.InvalidPasswordHash;

        return new ApplicationUser
        {
            Username = username,
            Email = email,
            PasswordHash = passwordHash,
            Role = role
        };
    }
}