using FluentValidation;
using Mandys.Domain;
using Mandys.DTOs;

namespace Mandys.Validators;

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("El nombre es obligatorio.");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("El apellido es obligatorio.");

        RuleFor(x => x.Email)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.Email))
            .WithMessage("El correo electrónico no es válido.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .When(x => !IsCustomerRole(EffectiveRole(x.Role)))
            .WithMessage(x => $"El rol '{EffectiveRole(x.Role)}' requiere credenciales de acceso: el correo y la contraseña son obligatorios.");

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

    private static bool IsCustomerRole(string role) =>
        string.Equals(role, Roles.Customer, StringComparison.OrdinalIgnoreCase);
}

public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.Email))
            .WithMessage("El correo electrónico no es válido.");

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
