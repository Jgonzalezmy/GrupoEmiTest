//using GrupoEmiTest.Application.DTOs.Request;
//using GrupoEmiTest.Application.DTOs.Response;
//using GrupoEmiTest.Application.Interfaces;
//using GrupoEmiTest.Domain.Common;
//using GrupoEmiTest.Domain.Entities;
//using GrupoEmiTest.Domain.Interfaces;

//namespace GrupoEmiTest.Application.Services;

///// <summary>
///// Handles user registration and login, delegating password hashing and
///// JWT generation to the infrastructure layer via injected interfaces.
///// </summary>
//public sealed class AuthService : IAuthService
//{
//    private readonly IUnitOfWork _unitOfWork;
//    private readonly IPasswordHasher _passwordHasher;
//    private readonly ITokenService _tokenService;

//    /// <summary>
//    /// Initialises a new instance of <see cref="AuthService"/>.
//    /// </summary>
//    /// <param name="unitOfWork">The unit of work used to access the user repository.</param>
//    /// <param name="passwordHasher">The service used to hash and verify passwords.</param>
//    /// <param name="tokenService">The service used to generate JWT tokens.</param>
//    public AuthService(
//        IUnitOfWork unitOfWork,
//        IPasswordHasher passwordHasher,
//        ITokenService tokenService)
//    {
//        _unitOfWork = unitOfWork;
//        _passwordHasher = passwordHasher;
//        _tokenService = tokenService;
//    }

//    /// <inheritdoc/>
//    public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request)
//    {
//        var userRepository = _unitOfWork.Users;

//        bool usernameExists = await userRepository.ExistsAsync(u => u.Username == request.Username);
//        if (usernameExists)
//            return Result<AuthResponse>.Failure("Username is already taken.");

//        bool emailExists = await userRepository.ExistsAsync(u => u.Email == request.Email);
//        if (emailExists)
//            return Result<AuthResponse>.Failure("Email is already registered.");

//        var user = new ApplicationUser
//        {
//            Username = request.Username,
//            Email = request.Email,
//            PasswordHash = _passwordHasher.Hash(request.Password),
//            Role = request.Role
//        };

//        await userRepository.AddAsync(user);
//        await _unitOfWork.SaveChangesAsync();

//        var token = _tokenService.GenerateToken(user);
//        return Result<AuthResponse>.Success(BuildAuthResponse(user, token));
//    }

//    /// <inheritdoc/>
//    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request)
//    {
//        var userRepository = _unitOfWork.Users;

//        var user = await userRepository.FindAsync(u => u.Username == request.Username);
//        if (user is null)
//            return Result<AuthResponse>.Failure("Invalid username or password.");

//        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
//            return Result<AuthResponse>.Failure("Invalid username or password.");

//        var token = _tokenService.GenerateToken(user);
//        return Result<AuthResponse>.Success(BuildAuthResponse(user, token));
//    }

//    // ── Private helpers ──────────────────────────────────────────────────────

//    /// <summary>
//    /// Builds an <see cref="AuthResponse"/> from a user entity and its generated token.
//    /// </summary>
//    /// <param name="user">The authenticated or newly registered user.</param>
//    /// <param name="token">The signed JWT string.</param>
//    /// <returns>A populated <see cref="AuthResponse"/>.</returns>
//    private static AuthResponse BuildAuthResponse(ApplicationUser user, string token) => new()
//    {
//        Token = token,
//        Username = user.Username,
//        Role = user.Role,
//        ExpiresAt = DateTime.UtcNow.AddHours(8)
//    };
//}