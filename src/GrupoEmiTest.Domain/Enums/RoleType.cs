namespace GrupoEmiTest.Domain.Enums;

/// <summary>
/// Represents the access roles that can be assigned to an <see cref="GrupoEmiTest.Domain.Entities.ApplicationUser"/>.
/// </summary>
public enum RoleType
{
    /// <summary>Administrator with full access.</summary>
    Admin = 1,

    /// <summary>Standard user with limited access.</summary>
    User = 2

}
