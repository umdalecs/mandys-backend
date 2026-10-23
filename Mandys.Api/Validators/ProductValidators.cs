using FluentValidation;
using Mandys.Application.DTOs;
using Mandys.Domain;

namespace Mandys.Api.Validators;

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
            .WithMessage("La unidad de medida es obligatoria.")
            .Must(MeasureUnits.IsValid)
            .WithMessage(x => $"La unidad de medida '{x.MeasureUnit}' no es válida. Unidades permitidas: {string.Join(", ", MeasureUnits.All)}.")
            .Must((request, unit) => !MeasureUnits.IsValid(unit) || MeasureUnits.IsAllowedForProduct(unit, request.IsSupply))
            .WithMessage($"Los productos de venta solo pueden usar la unidad de medida '{MeasureUnits.Units}'.");
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
            .WithMessage("La unidad de medida no puede estar vacía.")
            .Must(MeasureUnits.IsValid)
            .WithMessage(x => $"La unidad de medida '{x.MeasureUnit}' no es válida. Unidades permitidas: {string.Join(", ", MeasureUnits.All)}.")
            .When(x => x.MeasureUnit is not null);
    }
}
