using FluentValidation;
using Mandys.Application.DTOs;
using Mandys.Application.Services.Interfaces;
using Mandys.Domain;

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

public class StorageInProductLineValidator : AbstractValidator<CreateStorageInProductLineRequest>
{
    public StorageInProductLineValidator(IProductRepository products)
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0)
            .WithMessage("Los IDs de productos en el movimiento deben ser positivos.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Las cantidades en el movimiento deben ser positivas.");

        RuleFor(x => x.CostPrice)
            .GreaterThanOrEqualTo(0)
            .WithMessage("El costo de compra no puede ser negativo.");

        RuleFor(x => x)
            .MustAsync(async (line, _) =>
            {
                if (line.ProductId <= 0 || decimal.IsInteger(line.Quantity))
                {
                    return true;
                }

                var product = await products.GetByIdAsync(line.ProductId);
                return product is null || MeasureUnits.IsValidQuantity(product.MeasureUnit, line.Quantity);
            })
            .WithMessage($"Las cantidades para productos con unidad de medida '{MeasureUnits.Units}' deben ser números enteros.");
    }
}

public class StorageOutProductLineValidator : AbstractValidator<CreateStorageOutProductLineRequest>
{
    public StorageOutProductLineValidator(IProductRepository products)
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0)
            .WithMessage("Los IDs de productos en el movimiento deben ser positivos.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Las cantidades en el movimiento deben ser positivas.");

        RuleFor(x => x)
            .MustAsync(async (line, _) =>
            {
                if (line.ProductId <= 0 || decimal.IsInteger(line.Quantity))
                {
                    return true;
                }

                var product = await products.GetByIdAsync(line.ProductId);
                return product is null || MeasureUnits.IsValidQuantity(product.MeasureUnit, line.Quantity);
            })
            .WithMessage($"Las cantidades para productos con unidad de medida '{MeasureUnits.Units}' deben ser números enteros.");
    }
}

public class CreateStorageInRequestValidator : AbstractValidator<CreateStorageInRequest>
{
    public CreateStorageInRequestValidator(IProductRepository products)
    {
        RuleFor(x => x.Folio)
            .NotEmpty()
            .MaximumLength(50)
            .WithMessage("El folio es obligatorio y no puede superar los 50 caracteres.");

        RuleFor(x => x.Products)
            .NotEmpty()
            .WithMessage("El movimiento debe incluir al menos un producto.");

        RuleForEach(x => x.Products).SetValidator(new StorageInProductLineValidator(products));

        RuleFor(x => x.Products)
            .Must(lines => lines!.GroupBy(l => l.ProductId).All(g => g.Count() == 1))
            .When(x => x.Products is not null)
            .WithMessage("Los productos del movimiento no pueden estar duplicados.");
    }
}

public class CreateStorageOutRequestValidator : AbstractValidator<CreateStorageOutRequest>
{
    public CreateStorageOutRequestValidator(IProductRepository products)
    {
        RuleFor(x => x.Folio)
            .NotEmpty()
            .MaximumLength(50)
            .When(x => x.Folio is not null)
            .WithMessage("El folio no puede estar vacío y no puede superar los 50 caracteres.");

        RuleFor(x => x.Products)
            .NotEmpty()
            .WithMessage("El movimiento debe incluir al menos un producto.");

        RuleForEach(x => x.Products).SetValidator(new StorageOutProductLineValidator(products));

        RuleFor(x => x.Products)
            .Must(lines => lines!.GroupBy(l => l.ProductId).All(g => g.Count() == 1))
            .When(x => x.Products is not null)
            .WithMessage("Los productos del movimiento no pueden estar duplicados.");
    }
}
