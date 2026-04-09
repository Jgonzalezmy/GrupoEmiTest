using GrupoEmiTest.Application.DTOs.Request;
using GrupoEmiTest.Application.DTOs.Response;
using GrupoEmiTest.Domain.Common;

namespace GrupoEmiTest.Application.Interfaces;

/// <summary>
/// Defines authentication operations: user registration and login.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Registers a new user account.
    /// </summary>
    /// <param name="request">The registration data including username, email, password, and role.</param>
    /// <returns>
    /// A result containing an <see cref="AuthResponse"/> with a JWT on success,
    /// or a failure if the username or email is already taken.
    /// </returns>
    Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request);

    /// <summary>
    /// Authenticates a user with their username and password.
    /// </summary>
    /// <param name="request">The login credentials.</param>
    /// <returns>
    /// A result containing an <see cref="AuthResponse"/> with a JWT on success,
    /// or a failure if the credentials are invalid.
    /// </returns>
    Task<Result<AuthResponse>> LoginAsync(LoginRequest request);
}