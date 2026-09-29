using FluentValidation;
using Mandys.DTOs;

namespace Mandys.Validators;

public class CreateLogRequestValidator : AbstractValidator<CreateLogRequest>
{
    public CreateLogRequestValidator()
    {
        RuleFor(lr => lr.Verb)
            .NotNull()
            .MaximumLength(200)
            .WithMessage("El verbo es obligatorio y no puede superar los 200 caracteres.");

        RuleFor(lr => lr.Description)
            .NotNull()
            .MinimumLength(25)
            .MaximumLength(200)
            .WithMessage("La descripción debe tener entre 25 y 200 caracteres.");
    }
}
