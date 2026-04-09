namespace GrupoEmiTest.Domain.Entities;

/// <summary>
/// Represents an application user with authentication credentials and an assigned role.
/// </summary>
public class ApplicationUser
{
    /// <summary>Gets or sets the unique identifier of the user.</summary>
    public int Id { get; set; }

    /// <summary>Gets or sets the username used for login.</summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>Gets or sets the email address of the user.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Gets or sets the hashed password of the user.</summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the role assigned to the user.
    /// Valid values are <c>"Admin"</c> and <c>"User"</c>.
    /// </summary>
    public string Role { get; set; } = "User";
}