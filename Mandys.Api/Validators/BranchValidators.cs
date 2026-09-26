using FluentValidation;
using Mandys.DTOs;

namespace Mandys.Validators;

public class CreateBranchRequestValidator : AbstractValidator<CreateBranchRequest>
{
    public CreateBranchRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(50)
            .WithMessage("Name is required and must be at most 50 characters.");

        RuleFor(x => x.Address)
            .NotEmpty()
            .MaximumLength(50)
            .WithMessage("Address is required and must be at most 50 characters.");
    }
}

public class UpdateBranchRequestValidator : AbstractValidator<UpdateBranchRequest>
{
    public UpdateBranchRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(50)
            .When(x => x.Name is not null)
            .WithMessage("Name must not be blank and must be at most 50 characters.");

        RuleFor(x => x.Address)
            .NotEmpty()
            .MaximumLength(50)
            .When(x => x.Address is not null)
            .WithMessage("Address must not be blank and must be at most 50 characters.");
    }
}
