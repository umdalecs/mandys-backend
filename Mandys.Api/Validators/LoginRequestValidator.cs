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
            .WithMessage("El correo electrónico no es válido.");

        RuleFor(lr => lr.Password)
            .NotNull()
            .MinimumLength(6)
            .WithMessage("La contraseña es demasiado corta.");
    }
}
