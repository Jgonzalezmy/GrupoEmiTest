namespace GrupoEmiTest.Application.DTOs.Response;

/// <summary>
/// Represents a response record containing the project information
/// assigned to an employee.
/// </summary>
/// <param name="ProjectId">The unique identifier of the project.</param>
/// <param name="Name">The name of the project.</param>
/// <param name="Description">An optional description of the project, or <see langword="null"/> if not provided.</param>
public record EmployeeProjectResponse(
    string Name,
    string? Description
);
