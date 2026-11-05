using FluentValidation;
using Mandys.Application.DTOs;

namespace Mandys.Api.Validators;

public class UpdateBranchStorageRequestValidator : AbstractValidator<UpdateBranchStorageRequest>
{
    public UpdateBranchStorageRequestValidator()
    {
        RuleFor(x => x.MinimumStock)
            .GreaterThanOrEqualTo(0)
            .WithMessage("El stock mínimo no puede ser negativo.");
    }
}
