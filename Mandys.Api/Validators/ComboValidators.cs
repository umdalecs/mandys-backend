using FluentValidation;
using Mandys.DTOs;

namespace Mandys.Validators;

public class ComboDishLineValidator : AbstractValidator<CreateComboDishLineRequest>
{
    public ComboDishLineValidator()
    {
        RuleFor(x => x.DishId)
            .GreaterThan(0)
            .WithMessage("Los IDs de platillos en el combo deben ser positivos.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Las cantidades de platillos en el combo deben ser positivas.");
    }
}

public class ComboProductLineValidator : AbstractValidator<CreateComboProductLineRequest>
{
    public ComboProductLineValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0)
            .WithMessage("Los IDs de productos en el combo deben ser positivos.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Las cantidades de productos en el combo deben ser positivas.");
    }
}

public class CreateComboRequestValidator : AbstractValidator<CreateComboRequest>
{
    public CreateComboRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(50)
            .WithMessage("El nombre es obligatorio y no puede superar los 50 caracteres.");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0)
            .WithMessage("El precio no puede ser negativo.");

        RuleForEach(x => x.Dishes).SetValidator(new ComboDishLineValidator());

        RuleFor(x => x.Dishes)
            .Must(lines => lines!.GroupBy(l => l.DishId).All(g => g.Count() == 1))
            .When(x => x.Dishes is not null)
            .WithMessage("Los platillos del combo no pueden estar duplicados.");

        RuleForEach(x => x.Products).SetValidator(new ComboProductLineValidator());

        RuleFor(x => x.Products)
            .Must(lines => lines!.GroupBy(l => l.ProductId).All(g => g.Count() == 1))
            .When(x => x.Products is not null)
            .WithMessage("Los productos del combo no pueden estar duplicados.");
    }
}

public class UpdateComboRequestValidator : AbstractValidator<UpdateComboRequest>
{
    public UpdateComboRequestValidator()
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

        RuleForEach(x => x.Dishes).SetValidator(new ComboDishLineValidator());

        RuleFor(x => x.Dishes)
            .Must(lines => lines!.GroupBy(l => l.DishId).All(g => g.Count() == 1))
            .When(x => x.Dishes is not null)
            .WithMessage("Los platillos del combo no pueden estar duplicados.");

        RuleForEach(x => x.Products).SetValidator(new ComboProductLineValidator());

        RuleFor(x => x.Products)
            .Must(lines => lines!.GroupBy(l => l.ProductId).All(g => g.Count() == 1))
            .When(x => x.Products is not null)
            .WithMessage("Los productos del combo no pueden estar duplicados.");
    }
}
