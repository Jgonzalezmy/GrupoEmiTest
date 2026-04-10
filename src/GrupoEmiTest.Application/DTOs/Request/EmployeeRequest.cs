using GrupoEmiTest.Domain.Entities;
using GrupoEmiTest.Domain.Enums;

namespace GrupoEmiTest.Application.DTOs.Request;

/// <summary>
/// Represents the data required to create or update an employee.
/// </summary>
/// <param name="Name">The full name of the employee.</param>
/// <param name="CurrentPosition">The current job position of the employee.</param>
/// <param name="Salary">The salary assigned to the employee.</param>
/// <param name="DepartmentId">The unique identifier of the department the employee belongs to.</param>
public record EmployeeRequest(
    string Name,
    PositionType CurrentPosition,
    decimal Salary,
    int DepartmentId
    );
