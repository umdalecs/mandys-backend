using FluentValidation;
using Mandys.DTOs;

namespace Mandys.Validators;

public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductRequestValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(50)
            .WithMessage("La descripción es obligatoria y no puede superar los 50 caracteres.");

        RuleFor(x => x.SalePrice)
            .GreaterThanOrEqualTo(0)
            .WithMessage("El precio de venta no puede ser negativo.");

        RuleFor(x => x.MeasureUnit)
            .NotEmpty()
            .MaximumLength(50)
            .WithMessage("La unidad de medida es obligatoria y no puede superar los 50 caracteres.");
    }
}

public class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
{
    public UpdateProductRequestValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(50)
            .When(x => x.Description is not null)
            .WithMessage("La descripción no puede estar vacía y no puede superar los 50 caracteres.");

        RuleFor(x => x.SalePrice)
            .GreaterThanOrEqualTo(0)
            .When(x => x.SalePrice.HasValue)
            .WithMessage("El precio de venta no puede ser negativo.");

        RuleFor(x => x.MeasureUnit)
            .NotEmpty()
            .MaximumLength(50)
            .When(x => x.MeasureUnit is not null)
            .WithMessage("La unidad de medida no puede estar vacía y no puede superar los 50 caracteres.");
    }
}
