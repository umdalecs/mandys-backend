using FluentValidation;
using Mandys.Application.DTOs;

namespace Mandys.Api.Validators;

public class OrderProductLineValidator : AbstractValidator<CreateOrderProductLineRequest>
{
    public OrderProductLineValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0)
            .WithMessage("Los IDs de productos en el pedido deben ser mayores a 0.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Las cantidades de productos en el pedido deben ser mayores a 0.")
            .Must(decimal.IsInteger)
            .WithMessage("Las cantidades de productos en el pedido deben ser números enteros.");
    }
}

public class OrderDishLineValidator : AbstractValidator<CreateOrderDishLineRequest>
{
    public OrderDishLineValidator()
    {
        RuleFor(x => x.DishId)
            .GreaterThan(0)
            .WithMessage("Los IDs de platillos en el pedido deben ser mayores a 0.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Las cantidades de platillos en el pedido deben ser mayores a 0.")
            .Must(decimal.IsInteger)
            .WithMessage("Las cantidades de platillos en el pedido deben ser números enteros.");
    }
}

public class OrderComboLineValidator : AbstractValidator<CreateOrderComboLineRequest>
{
    public OrderComboLineValidator()
    {
        RuleFor(x => x.ComboId)
            .GreaterThan(0)
            .WithMessage("Los IDs de combos en el pedido deben ser mayores a 0.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Las cantidades de combos en el pedido deben ser mayores a 0.")
            .Must(decimal.IsInteger)
            .WithMessage("Las cantidades de combos en el pedido deben ser números enteros.");
    }
}

public class OrderCardPaymentValidator : AbstractValidator<OrderCardPaymentRequest>
{
    public OrderCardPaymentValidator()
    {
        RuleFor(x => x.CardNumber)
            .NotEmpty()
            .Must(number =>
            {
                if (string.IsNullOrWhiteSpace(number)) return false;
                var digits = new string(number.Where(char.IsAsciiDigit).ToArray());
                return digits.Length >= 13 && digits.Length <= 19 &&
                       number.All(c => char.IsAsciiDigit(c) || c == ' ' || c == '-');
            })
            .WithMessage("El número de tarjeta debe contener entre 13 y 19 dígitos.");

        RuleFor(x => x.CardHolder)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("El nombre del titular de la tarjeta es obligatorio y no puede superar los 100 caracteres.");

        RuleFor(x => x.ExpiryDate)
            .NotEmpty()
            .Matches(@"^(0[1-9]|1[0-2])\/([0-9]{2})$")
            .WithMessage("La fecha de expiración debe tener el formato MM/AA.");

        RuleFor(x => x.Cvv)
            .NotEmpty()
            .Matches(@"^[0-9]{3,4}$")
            .WithMessage("El CVV debe tener 3 o 4 dígitos.");
    }
}

public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderRequestValidator()
    {
        RuleFor(x => x)
            .Must(x =>
                (x.Products is { Count: > 0 }) ||
                (x.Dishes is { Count: > 0 }) ||
                (x.Combos is { Count: > 0 }))
            .WithMessage("El pedido debe incluir al menos un producto, platillo o combo.");

        RuleForEach(x => x.Products).SetValidator(new OrderProductLineValidator());

        RuleFor(x => x.Products)
            .Must(lines => lines!.GroupBy(l => l.ProductId).All(g => g.Count() == 1))
            .When(x => x.Products is not null)
            .WithMessage("Los productos del pedido no pueden estar duplicados.");

        RuleForEach(x => x.Dishes).SetValidator(new OrderDishLineValidator());

        RuleFor(x => x.Dishes)
            .Must(lines => lines!.GroupBy(l => l.DishId).All(g => g.Count() == 1))
            .When(x => x.Dishes is not null)
            .WithMessage("Los platillos del pedido no pueden estar duplicados.");

        RuleForEach(x => x.Combos).SetValidator(new OrderComboLineValidator());

        RuleFor(x => x.Combos)
            .Must(lines => lines!.GroupBy(l => l.ComboId).All(g => g.Count() == 1))
            .When(x => x.Combos is not null)
            .WithMessage("Los combos del pedido no pueden estar duplicados.");

        RuleFor(x => x.CardPayment!)
            .SetValidator(new OrderCardPaymentValidator())
            .When(x => x.CardPayment is not null);

        RuleFor(x => x.CustomerId)
            .GreaterThan(0)
            .When(x => x.CustomerId.HasValue)
            .WithMessage("El ID del cliente debe ser mayor a 0.");

        RuleFor(x => x.PointsToSpend)
            .GreaterThanOrEqualTo(0)
            .When(x => x.PointsToSpend.HasValue)
            .WithMessage("Los puntos a utilizar no pueden ser negativos.")
            .Must(p => Math.Round(p!.Value, 2) == p.Value)
            .When(x => x.PointsToSpend.HasValue)
            .WithMessage("Los puntos a utilizar no pueden tener más de 2 decimales.");
    }
}
