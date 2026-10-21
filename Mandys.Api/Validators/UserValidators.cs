using FluentValidation;
using Mandys.Application.DTOs;
using Mandys.Domain;

namespace Mandys.Api.Validators;

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
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
            .WithMessage("El correo electrónico es obligatorio y debe ser válido (máx. 50 caracteres).");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("La contraseña es obligatoria.");

        RuleFor(x => x.Role)
            .Must(role => Roles.IsValid(role))
            .When(x => !string.IsNullOrWhiteSpace(x.Role))
            .WithMessage(x => $"El rol '{x.Role}' no es válido. Roles permitidos: {string.Join(", ", Roles.All)}.");

        RuleFor(x => x.BranchId)
            .GreaterThan(0)
            .When(x => x.BranchId.HasValue)
            .WithMessage("El ID de sucursal debe ser un número positivo.");

        RuleFor(x => x.BranchId)
            .NotNull()
            .When(x => Roles.RequiresBranch(EffectiveRole(x.Role)))
            .WithMessage(x => $"El rol '{EffectiveRole(x.Role)}' requiere una sucursal. Solo administradores y clientes pueden omitirla.");
    }

    private static string EffectiveRole(string? role) =>
        string.IsNullOrWhiteSpace(role) ? Roles.Customer : Roles.Normalize(role.Trim());
}

public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
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
            .When(x => !string.IsNullOrWhiteSpace(x.Email))
            .WithMessage("El correo electrónico no es válido o supera los 50 caracteres.");

        RuleFor(x => x.Role)
            .Must(role => Roles.IsValid(role))
            .When(x => !string.IsNullOrWhiteSpace(x.Role))
            .WithMessage(x => $"El rol '{x.Role}' no es válido. Roles permitidos: {string.Join(", ", Roles.All)}.");

        RuleFor(x => x.BranchId)
            .GreaterThan(0)
            .When(x => x.BranchId.HasValue)
            .WithMessage("El ID de sucursal debe ser un número positivo.");
    }
}
