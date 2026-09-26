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
            .WithMessage("First name is required and must be at most 50 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(50)
            .WithMessage("Last name is required and must be at most 50 characters.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(50)
            .WithMessage("A valid email is required and must be at most 50 characters.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6)
            .WithMessage("Password must be at least 6 characters.");
    }
}

public class CreateCustomerRequestValidator : AbstractValidator<CreateCustomerRequest>
{
    public CreateCustomerRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(50)
            .WithMessage("First name is required and must be at most 50 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(50)
            .WithMessage("Last name is required and must be at most 50 characters.");

        RuleFor(x => x.Email)
            .EmailAddress()
            .MaximumLength(50)
            .When(x => !string.IsNullOrWhiteSpace(x.Email))
            .WithMessage("Email must be a valid address and at most 50 characters.");
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
            .WithMessage("First name must not be blank and must be at most 50 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(50)
            .When(x => x.LastName is not null)
            .WithMessage("Last name must not be blank and must be at most 50 characters.");

        RuleFor(x => x.Email)
            .EmailAddress()
            .MaximumLength(50)
            .When(x => x.Email is not null)
            .WithMessage("Email must be a valid address and at most 50 characters.");
    }
}
