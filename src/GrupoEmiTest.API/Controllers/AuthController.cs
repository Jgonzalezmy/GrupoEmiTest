using GrupoEmiTest.API.Extensions;
using GrupoEmiTest.Application.DTOs.Request;
using GrupoEmiTest.Application.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace GrupoEmiTest.API.Controllers;

/// <summary>
/// Handles user registration and authentication, issuing JWT tokens on success.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    /// <summary>
    /// Initialises a new instance of <see cref="AuthController"/>.
    /// </summary>
    /// <param name="authService">The service that handles authentication logic.</param>
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Registers a new user account and returns a JWT token.
    /// </summary>
    /// <param name="request">The registration data (username, email, password, role).</param>
    /// <returns>
    /// <c>201 Created</c> with an <see cref="Application.DTOs.Response.AuthResponse"/> on success.<br/>
    /// <c>400 Bad Request</c> if the username or email is already taken.
    /// </returns>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _authService.RegisterAsync(request, cancellationToken);

        if (result.IsFailure)
            return result.ToProblemResult(this);

        return CreatedAtAction(nameof(Register), result.Value);
    }

    /// <summary>
    /// Authenticates a user with their username and password and returns a JWT token.
    /// </summary>
    /// <param name="request">The login credentials (username, password).</param>
    /// <returns>
    /// <c>200 OK</c> with an <see cref="Application.DTOs.Response.AuthResponse"/> on success.<br/>
    /// <c>401 Unauthorized</c> if the credentials are invalid.
    /// </returns>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _authService.LoginAsync(request, cancellationToken);

        if (result.IsFailure)
            return result.ToProblemResult(this);

        return Ok(result.Value);
    }
}