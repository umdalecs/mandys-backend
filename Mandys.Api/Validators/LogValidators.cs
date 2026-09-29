using FluentValidation;
using Mandys.DTOs;

namespace Mandys.Validators;

public class CreateLogRequestValidator : AbstractValidator<CreateLogRequest>
{
    public CreateLogRequestValidator()
    {
        RuleFor(lr => lr.Verb)
            .NotNull()
            .WithMessage("Verb is required");

        RuleFor(lr => lr.Description)
            .NotNull()
            .MinimumLength(25)
            .WithMessage("Description must be at least 25 characters");
    }
}
