using FluentValidation;
using Mandys.DTOs;

namespace Mandys.Validators;

public class CreateLogRequestValidator : AbstractValidator<CreateLogRequest>
{
    public CreateLogRequestValidator()
    {
        RuleFor(lr => lr.Verb)
            .NotNull()
            .WithMessage("El verbo es obligatorio.");

        RuleFor(lr => lr.Description)
            .NotNull()
            .MinimumLength(25)
            .WithMessage("La descripción debe tener al menos 25 caracteres.");
    }
}
