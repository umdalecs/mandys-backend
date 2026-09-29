using FluentValidation;
using Mandys.DTOs;

namespace Mandys.Validators;

public class RegisterCustomerRequestValidator : AbstractValidator<RegisterCustomerRequest>
{
    public RegisterCustomerRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(50)
            .WithMessage("El nombre es obligatorio y no puede superar los 50 caracteres.");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(50)
            .WithMessage("El apellido es obligatorio y no puede superar los 50 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(50)
            .WithMessage("Se requiere un correo electrónico válido de máximo 50 caracteres.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6)
            .WithMessage("La contraseña debe tener al menos 6 caracteres.");
    }
}

public class UpdateCustomerRequestValidator : AbstractValidator<UpdateCustomerRequest>
{
    public UpdateCustomerRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(50)
            .When(x => x.FirstName is not null)
            .WithMessage("El nombre no puede estar vacío y no puede superar los 50 caracteres.");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(50)
            .When(x => x.LastName is not null)
            .WithMessage("El apellido no puede estar vacío y no puede superar los 50 caracteres.");

        RuleFor(x => x.Email)
            .EmailAddress()
            .MaximumLength(50)
            .When(x => x.Email is not null)
            .WithMessage("El correo electrónico debe ser válido y de máximo 50 caracteres.");
    }
}
