using FluentValidation;
using GrupoEmiTest.Application.DTOs.Request;
using GrupoEmiTest.Domain.Enums;

namespace GrupoEmiTest.API.Validators;

/// <summary>
/// Validates the business rules for the <see cref="EmployeeRequest"/> entity
/// before persisting or processing it.
/// </summary>
public class EmployeeValidator : AbstractValidator<EmployeeRequest>
{
    /// <summary>
    /// Initializes the validation rules for <see cref="EmployeeRequest"/>.
    /// </summary>
    public EmployeeValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .NotNull()
            .MaximumLength(200);

        RuleFor(x => x.CurrentPosition)
            .IsInEnum()
            .WithMessage($"Current position must be one of the following values: {string.Join(", ", Enum.GetNames<PositionType>())}.");

        RuleFor(x => x.Salary)
            .GreaterThan(0);

        RuleFor(x => x.DepartmentId)
            .NotNull()
            .GreaterThan(0);
    }
}
