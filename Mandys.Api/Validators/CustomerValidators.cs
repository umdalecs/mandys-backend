using FluentValidation;
using Mandys.Application.DTOs;

namespace Mandys.Api.Validators;

public class RegisterCustomerRequestValidator : AbstractValidator<RegisterCustomerRequest>
{
    public RegisterCustomerRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(50)
            .WithMessage("El nombre es obligatorio y no puede superar los 50 caracteres.");

        RuleFor(x => x.FatherLastName)
            .NotEmpty()
            .MaximumLength(50)
            .WithMessage("El apellido paterno es obligatorio y no puede superar los 50 caracteres.");

        RuleFor(x => x.MotherLastName)
            .MaximumLength(50)
            .When(x => x.MotherLastName is not null)
            .WithMessage("El apellido materno no puede superar los 50 caracteres.");

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

        RuleFor(x => x.FatherLastName)
            .NotEmpty()
            .MaximumLength(50)
            .When(x => x.FatherLastName is not null)
            .WithMessage("El apellido paterno no puede estar vacío y no puede superar los 50 caracteres.");

        RuleFor(x => x.MotherLastName)
            .MaximumLength(50)
            .When(x => x.MotherLastName is not null)
            .WithMessage("El apellido materno no puede superar los 50 caracteres.");

        RuleFor(x => x.Email)
            .EmailAddress()
            .MaximumLength(50)
            .When(x => x.Email is not null)
            .WithMessage("El correo electrónico debe ser válido y de máximo 50 caracteres.");
    }
}
