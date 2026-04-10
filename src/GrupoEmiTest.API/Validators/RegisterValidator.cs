using FluentValidation;
using GrupoEmiTest.Application.DTOs.Request;

namespace GrupoEmiTest.API.Validators;

/// <summary>
/// Validates the business rules for the <see cref="RegisterRequest"/> entity
/// before persisting or processing it.
/// </summary>
public class RegisterValidator : AbstractValidator<RegisterRequest>
{
    /// <summary>
    /// Initializes the validation rules for <see cref="RegisterRequest"/>.
    /// </summary>
    public RegisterValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .NotNull()
            .MaximumLength(100);

        RuleFor(x => x.Email)
            .NotEmpty()
            .NotNull()
            .MaximumLength(256)
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .NotNull()
            .MinimumLength(5);

        RuleFor(x => x.Role)
            .NotEmpty()
            .NotNull()
            .IsInEnum();
    }
}
