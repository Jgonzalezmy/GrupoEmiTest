namespace GrupoEmiTest.API.Policies;

/// <summary>
/// Defines the authorization policy names used across the application.
/// Each policy maps to a set of allowed roles and controls access to
/// specific HTTP operations.
/// </summary>
public static class PolicyConstants
{
    /// <summary>
    /// Policy that grants read access (GET operations).
    /// Allowed roles: <c>Admin</c>, <c>User</c>.
    /// </summary>
    public const string ReadPolicy = "ReadPolicy";

    /// <summary>
    /// Policy that grants write access (POST operations).
    /// Allowed roles: <c>Admin</c>.
    /// </summary>
    public const string WritePolicy = "WritePolicy";

    /// <summary>
    /// Policy that grants edit access (PUT operations).
    /// Allowed roles: <c>Admin</c>.
    /// </summary>
    public const string EditPolicy = "EditPolicy";

    /// <summary>
    /// Policy that grants delete access (DELETE operations).
    /// Allowed roles: <c>Admin</c>.
    /// </summary>
    public const string DeletePolicy = "DeletePolicy";
}