using FluentValidation;
using Mandys.DTOs;

namespace Mandys.Validators;

public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductRequestValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("La descripción es obligatoria.");

        RuleFor(x => x.SalePrice)
            .GreaterThanOrEqualTo(0)
            .WithMessage("El precio de venta no puede ser negativo.");

        RuleFor(x => x.MeasureUnit)
            .NotEmpty()
            .WithMessage("La unidad de medida es obligatoria.");
    }
}

public class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
{
    public UpdateProductRequestValidator()
    {
        RuleFor(x => x.SalePrice)
            .GreaterThanOrEqualTo(0)
            .When(x => x.SalePrice.HasValue)
            .WithMessage("El precio de venta no puede ser negativo.");
    }
}
