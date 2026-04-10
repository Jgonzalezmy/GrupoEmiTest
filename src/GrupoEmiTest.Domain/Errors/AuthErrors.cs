using GrupoEmiTest.Domain.Common;

namespace GrupoEmiTest.Domain.Errors;

/// <summary>
/// Contains strongly-typed <see cref="Error"/> instances for authentication and registration operations.
/// </summary>
public static class AuthErrors
{
    /// <summary>The provided username is already taken by another user.</summary>
    public static readonly Error UsernameExists =
        Error.Conflict("Auth.UsernameExists", "Username is already taken.");

    /// <summary>The provided email address is already registered.</summary>
    public static readonly Error EmailExists =
        Error.Conflict("Auth.EmailExists", "Email is already registered.");

    /// <summary>The provided username is null or empty.</summary>
    public static readonly Error InvalidUsername =
        Error.Validation("Auth.InvalidUsername", "Username cannot be empty.");

    /// <summary>The provided email is null or empty.</summary>
    public static readonly Error InvalidEmail =
        Error.Validation("Auth.InvalidEmail", "Email cannot be empty.");

    /// <summary>The provided password hash is null or empty.</summary>
    public static readonly Error InvalidPasswordHash =
        Error.Validation("Auth.InvalidPasswordHash", "Password hash cannot be empty.");

    /// <summary>The supplied username or password is incorrect.</summary>
    public static readonly Error InvalidCredentials =
        Error.Unauthorized("Auth.InvalidCredentials", "Invalid username or password.");
}
