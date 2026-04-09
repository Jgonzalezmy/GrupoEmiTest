using GrupoEmiTest.Domain.Enums;

namespace GrupoEmiTest.Application.DTOs.Response;

/// <summary>
/// Represents the response data transfer object for an employee.
/// </summary>
/// <param name="Id">The unique identifier of the employee.</param>
/// <param name="Name">The full name of the employee.</param>
/// <param name="CurrentPosition">The current position or role held by the employee.</param>
/// <param name="DepartmentName">The name of the department the employee belongs to.</param>
/// <param name="Salary">The base salary of the employee.</param>
/// <param name="YearlyBonus">The yearly bonus amount assigned to the employee.</param>
public record EmployeeResponse(
    int Id,
    string Name,
    PositionType CurrentPosition,
    string DepartmentName,
    decimal Salary,
    decimal YearlyBonus
    );
