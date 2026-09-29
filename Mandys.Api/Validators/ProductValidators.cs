using FluentValidation;
using Mandys.DTOs;

namespace Mandys.Validators;

public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductRequestValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required.");

        RuleFor(x => x.SalePrice)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Sale price must be non-negative.");

        RuleFor(x => x.MeasureUnit)
            .NotEmpty()
            .WithMessage("Measure unit is required.");
    }
}

public class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
{
    public UpdateProductRequestValidator()
    {
        RuleFor(x => x.SalePrice)
            .GreaterThanOrEqualTo(0)
            .When(x => x.SalePrice.HasValue)
            .WithMessage("Sale price must be non-negative.");
    }
}
