using GrupoEmiTest.Domain.Common;

namespace GrupoEmiTest.Domain.Errors;

public static class EmployeeErrors
{
    public static Error AlreadyExists(string id) => Error.Conflict("Employee.AlreadyExists", $"Employee with ID '{id}' already exists.");
    public static Error NotFound(int id) => Error.NotFound("Employee.NotFound", $"Employee with ID '{id}' was not found.");
    public static Error NotFound() => Error.NotFound("Employee.NotFound", "Employee was not found.");

    public static readonly Error NameEmpty =
        Error.Validation("Employee.NameEmpty", "Employee name cannot be empty.");

    public static readonly Error InvalidSalary =
        Error.Validation("Employee.InvalidSalary", "Salary must be greater than zero.");

    public static readonly Error InvalidDepartmentId =
        Error.Validation("Employee.InvalidDepartmentId", "Department ID must be greater than zero.");
}
