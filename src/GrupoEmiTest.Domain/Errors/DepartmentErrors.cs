using GrupoEmiTest.Domain.Common;

namespace GrupoEmiTest.Domain.Errors;

public static class DepartmentErrors
{
    public static Error NotFound(int id) => Error.NotFound("Department.NotFound", $"Department with ID '{id}' was not found.");

    public static readonly Error InvalidName =
        Error.Validation("Department.InvalidName", "Department name cannot be empty.");
}
