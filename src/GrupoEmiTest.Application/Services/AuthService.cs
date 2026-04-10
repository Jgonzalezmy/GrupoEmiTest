using GrupoEmiTest.Application.DTOs.Request;
using GrupoEmiTest.Application.DTOs.Response;
using GrupoEmiTest.Application.Interfaces;
using GrupoEmiTest.Application.Settings;
using GrupoEmiTest.Domain.Common;
using GrupoEmiTest.Domain.Entities;
using GrupoEmiTest.Domain.Errors;
using GrupoEmiTest.Domain.Interfaces;
using Microsoft.Extensions.Options;

namespace GrupoEmiTest.Application.Services;

/// <summary>
/// Handles user registration and login, delegating password hashing and
/// JWT generation to the infrastructure layer via injected interfaces.
/// </summary>
public sealed class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly JwtSettings _jwtSettings;

    /// <summary>
    /// Initialises a new instance of <see cref="AuthService"/>.
    /// </summary>
    /// <param name="unitOfWork">The unit of work used to access the user repository.</param>
    /// <param name="passwordHasher">The service used to hash and verify passwords.</param>
    /// <param name="tokenService">The service used to generate JWT tokens.</param>
    /// <param name="jwtSettings">The JWT configuration settings.</param>
    public AuthService(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        IOptions<JwtSettings> jwtSettings)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _jwtSettings = jwtSettings.Value;
    }

    /// <inheritdoc/>
    public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var userRepository = _unitOfWork.Users;

        bool usernameExists = await userRepository.ExistsAsync(u => u.Username == request.Username, cancellationToken);
        if (usernameExists)
            return AuthErrors.UsernameExists;

        bool emailExists = await userRepository.ExistsAsync(u => u.Email == request.Email, cancellationToken);
        if (emailExists)
            return AuthErrors.EmailExists;

        var user = ApplicationUser.Create(
            request.Username,
            request.Email,
            _passwordHasher.Hash(request.Password),
            request.Role);

        await _unitOfWork.AddAndSaveAsync(user, cancellationToken);

        var token = _tokenService.GenerateToken(user);
        return BuildAuthResponse(user, token);
    }

    /// <inheritdoc/>
    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var userRepository = _unitOfWork.Users;

        var user = await userRepository.FindAsync(u => u.Username == request.Username, cancellationToken);
        if (user is null)
            return AuthErrors.InvalidCredentials;

        if (!_passwordHasher.Verify(request.password, user.PasswordHash))
            return AuthErrors.InvalidCredentials;

        var token = _tokenService.GenerateToken(user);
        return BuildAuthResponse(user, token);
    }

    /// <summary>
    /// Builds an <see cref="AuthResponse"/> from a user entity and its generated token.
    /// </summary>
    /// <param name="user">The authenticated or newly registered user.</param>
    /// <param name="token">The signed JWT string.</param>
    /// <returns>A populated <see cref="AuthResponse"/>.</returns>
    private AuthResponse BuildAuthResponse(ApplicationUser user, string token) =>
        new(token, user.Username, user.Role.ToString(), GetTokenExpiration());

    /// <summary>
    /// Calculates the UTC expiration date and time for a JWT token
    /// based on the <see cref="JwtSettings.ExpiryHours"/> value from configuration.
    /// </summary>
    /// <returns>The UTC <see cref="DateTime"/> at which the token will expire.</returns>
    private DateTime GetTokenExpiration() =>
        DateTime.UtcNow.AddHours(_jwtSettings.ExpiryHours);
}