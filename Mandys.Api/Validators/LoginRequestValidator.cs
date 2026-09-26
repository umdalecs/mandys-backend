using FluentValidation;
using Mandys.DTOs;

namespace Mandys.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(lr => lr.Email)
            .NotNull()
            .EmailAddress()
            .WithMessage("Invalid Email");

        RuleFor(lr => lr.Password)
            .MinimumLength(6)
            .WithMessage("Password try is too short");
    }
}
