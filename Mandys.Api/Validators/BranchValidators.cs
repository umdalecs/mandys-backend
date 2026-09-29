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
            .WithMessage("El nombre es obligatorio y no puede superar los 50 caracteres.");

        RuleFor(x => x.Address)
            .NotEmpty()
            .MaximumLength(50)
            .WithMessage("La dirección es obligatoria y no puede superar los 50 caracteres.");
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
            .WithMessage("El nombre no puede estar vacío y no puede superar los 50 caracteres.");

        RuleFor(x => x.Address)
            .NotEmpty()
            .MaximumLength(50)
            .When(x => x.Address is not null)
            .WithMessage("La dirección no puede estar vacía y no puede superar los 50 caracteres.");
    }
}
