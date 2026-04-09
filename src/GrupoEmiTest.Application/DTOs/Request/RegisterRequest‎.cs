namespace GrupoEmiTest.Application.DTOs.Request;

public record RegisterRequest(
    string Username,
    string Email,
    string Password,
    string Role
    );

