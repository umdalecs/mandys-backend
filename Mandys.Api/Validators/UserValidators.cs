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
            .WithMessage("First name is required.");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Last name is required.");

        RuleFor(x => x.Email)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.Email))
            .WithMessage("Invalid email.");

        // Email and password are both optional: neither means a counter
        // customer, email alone identifies a customer whose points can be
        // reclaimed, and both together is a login. Only customers may skip
        // them, so every other role has to arrive with credentials.
        RuleFor(x => x.Password)
            .NotEmpty()
            .When(x => !IsCustomerRole(EffectiveRole(x.Role)))
            .WithMessage(x => $"Role '{EffectiveRole(x.Role)}' requires login credentials: email and password are mandatory.");

        RuleFor(x => x.UserName)
            .MaximumLength(50)
            .When(x => !string.IsNullOrWhiteSpace(x.UserName))
            .WithMessage("Username must be at most 50 characters.");

        RuleFor(x => x.Role)
            .Must(role => Roles.IsValid(role))
            .When(x => !string.IsNullOrWhiteSpace(x.Role))
            .WithMessage(x => $"Invalid role '{x.Role}'. Allowed roles: {string.Join(", ", Roles.All)}.");

        RuleFor(x => x.BranchId)
            .GreaterThan(0)
            .When(x => x.BranchId.HasValue)
            .WithMessage("BranchId must be a positive id.");

        RuleFor(x => x.BranchId)
            .NotNull()
            .When(x => Roles.RequiresBranch(EffectiveRole(x.Role)))
            .WithMessage(x => $"Role '{EffectiveRole(x.Role)}' requires a branch. Only administrators and customers may omit it.");
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
            .WithMessage("Invalid email.");

        RuleFor(x => x.Role)
            .Must(role => Roles.IsValid(role))
            .When(x => !string.IsNullOrWhiteSpace(x.Role))
            .WithMessage(x => $"Invalid role '{x.Role}'. Allowed roles: {string.Join(", ", Roles.All)}.");

        RuleFor(x => x.BranchId)
            .GreaterThan(0)
            .When(x => x.BranchId.HasValue)
            .WithMessage("BranchId must be a positive id.");
    }
}
