using FluentValidation;
using Mandys.DTOs;

namespace Mandys.Validators;

public class DishRecipeLineValidator : AbstractValidator<CreateDishRecipeLineRequest>
{
    public DishRecipeLineValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0)
            .WithMessage("Recipe product ids must be positive.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Recipe quantities must be positive.");
    }
}

public class CreateDishRequestValidator : AbstractValidator<CreateDishRequest>
{
    public CreateDishRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Price must be non-negative.");

        RuleForEach(x => x.Recipe).SetValidator(new DishRecipeLineValidator());

        RuleFor(x => x.Recipe)
            .Must(lines => lines!.GroupBy(l => l.ProductId).All(g => g.Count() == 1))
            .When(x => x.Recipe is not null)
            .WithMessage("Recipe products must not be duplicated.");
    }
}

public class UpdateDishRequestValidator : AbstractValidator<UpdateDishRequest>
{
    public UpdateDishRequestValidator()
    {
        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0)
            .When(x => x.Price.HasValue)
            .WithMessage("Price must be non-negative.");

        RuleForEach(x => x.Recipe).SetValidator(new DishRecipeLineValidator());

        RuleFor(x => x.Recipe)
            .Must(lines => lines!.GroupBy(l => l.ProductId).All(g => g.Count() == 1))
            .When(x => x.Recipe is not null)
            .WithMessage("Recipe products must not be duplicated.");
    }
}
