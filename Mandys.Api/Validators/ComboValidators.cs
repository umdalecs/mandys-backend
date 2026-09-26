using FluentValidation;
using Mandys.DTOs;

namespace Mandys.Validators;

public class ComboDishLineValidator : AbstractValidator<CreateComboDishLineRequest>
{
    public ComboDishLineValidator()
    {
        RuleFor(x => x.DishId)
            .GreaterThan(0)
            .WithMessage("Combo dish ids must be positive.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Combo dish quantities must be positive.");
    }
}

public class ComboProductLineValidator : AbstractValidator<CreateComboProductLineRequest>
{
    public ComboProductLineValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0)
            .WithMessage("Combo product ids must be positive.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Combo product quantities must be positive.");
    }
}

public class CreateComboRequestValidator : AbstractValidator<CreateComboRequest>
{
    public CreateComboRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Price must be non-negative.");

        RuleForEach(x => x.Dishes).SetValidator(new ComboDishLineValidator());

        RuleFor(x => x.Dishes)
            .Must(lines => lines!.GroupBy(l => l.DishId).All(g => g.Count() == 1))
            .When(x => x.Dishes is not null)
            .WithMessage("Combo dishes must not be duplicated.");

        RuleForEach(x => x.Products).SetValidator(new ComboProductLineValidator());

        RuleFor(x => x.Products)
            .Must(lines => lines!.GroupBy(l => l.ProductId).All(g => g.Count() == 1))
            .When(x => x.Products is not null)
            .WithMessage("Combo products must not be duplicated.");
    }
}

public class UpdateComboRequestValidator : AbstractValidator<UpdateComboRequest>
{
    public UpdateComboRequestValidator()
    {
        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0)
            .When(x => x.Price.HasValue)
            .WithMessage("Price must be non-negative.");

        RuleForEach(x => x.Dishes).SetValidator(new ComboDishLineValidator());

        RuleFor(x => x.Dishes)
            .Must(lines => lines!.GroupBy(l => l.DishId).All(g => g.Count() == 1))
            .When(x => x.Dishes is not null)
            .WithMessage("Combo dishes must not be duplicated.");

        RuleForEach(x => x.Products).SetValidator(new ComboProductLineValidator());

        RuleFor(x => x.Products)
            .Must(lines => lines!.GroupBy(l => l.ProductId).All(g => g.Count() == 1))
            .When(x => x.Products is not null)
            .WithMessage("Combo products must not be duplicated.");
    }
}
