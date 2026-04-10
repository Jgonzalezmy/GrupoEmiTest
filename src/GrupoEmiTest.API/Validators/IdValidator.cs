using FluentValidation;
using GrupoEmiTest.Application.DTOs.Request;
using GrupoEmiTest.Domain.Entities;

namespace GrupoEmiTest.API.Validators;

/// <summary>
/// Validates the business rules for the <see cref="IdRequest"/> entity
/// before persisting or processing it.
/// </summary>
public class IdValidator : AbstractValidator<IdRequest>
{
    /// <summary>
    /// Initializes the validation rules for <see cref="IdRequest"/>.
    /// </summary>
    public IdValidator()
    {
        RuleFor(x => x.Id)
           .NotNull()
           .GreaterThan(0)
           .WithMessage("'Id' must be greater than 0.");
    }
}
