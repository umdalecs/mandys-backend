using FluentValidation;
using Mandys.Application.DTOs;
using Mandys.Application.Services.Interfaces;
using Mandys.Domain;

namespace Mandys.Api.Validators;

public class DishRecipeLineValidator : AbstractValidator<CreateDishRecipeLineRequest>
{
    public DishRecipeLineValidator(IProductRepository products)
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0)
            .WithMessage("Los IDs de productos en la receta deben ser positivos.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Las cantidades en la receta deben ser positivas.");

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

public class CreateDishRequestValidator : AbstractValidator<CreateDishRequest>
{
    public CreateDishRequestValidator(IProductRepository products)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(50)
            .WithMessage("El nombre es obligatorio y no puede superar los 50 caracteres.");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0)
            .WithMessage("El precio no puede ser negativo.");

        RuleForEach(x => x.Recipe).SetValidator(new DishRecipeLineValidator(products));

        RuleFor(x => x.Recipe)
            .Must(lines => lines!.GroupBy(l => l.ProductId).All(g => g.Count() == 1))
            .When(x => x.Recipe is not null)
            .WithMessage("Los productos de la receta no pueden estar duplicados.");
    }
}

public class UpdateDishRequestValidator : AbstractValidator<UpdateDishRequest>
{
    public UpdateDishRequestValidator(IProductRepository products)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(50)
            .When(x => x.Name is not null)
            .WithMessage("El nombre no puede estar vacío y no puede superar los 50 caracteres.");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0)
            .When(x => x.Price.HasValue)
            .WithMessage("El precio no puede ser negativo.");

        RuleForEach(x => x.Recipe).SetValidator(new DishRecipeLineValidator(products));

        RuleFor(x => x.Recipe)
            .Must(lines => lines!.GroupBy(l => l.ProductId).All(g => g.Count() == 1))
            .When(x => x.Recipe is not null)
            .WithMessage("Los productos de la receta no pueden estar duplicados.");
    }
}
