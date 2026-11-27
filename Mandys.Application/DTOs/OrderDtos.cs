using Mandys.Domain;

namespace Mandys.Application.DTOs;

public record OrderCardPaymentRequest(
    string CardNumber,
    string CardHolder,
    string ExpiryDate,
    string Cvv
);

public record CreateOrderProductLineRequest(
    int ProductId,
    decimal Quantity
);

public record CreateOrderDishLineRequest(
    int DishId,
    decimal Quantity
);

public record CreateOrderComboLineRequest(
    int ComboId,
    decimal Quantity
);

public record CreateOrderRequest(
    IReadOnlyList<CreateOrderProductLineRequest>? Products = null,
    IReadOnlyList<CreateOrderDishLineRequest>? Dishes = null,
    IReadOnlyList<CreateOrderComboLineRequest>? Combos = null,
    OrderCardPaymentRequest? CardPayment = null,
    int? CustomerId = null,
    decimal? PointsToSpend = null
);

public record OrderProductLineResponse(
    int ProductId,
    string Name,
    decimal Quantity,
    decimal UnitPrice,
    decimal Subtotal
);

public record OrderDishLineResponse(
    int DishId,
    string Name,
    decimal Quantity,
    decimal UnitPrice,
    decimal Subtotal
);

public record OrderComboLineResponse(
    int ComboId,
    string Name,
    decimal Quantity,
    decimal UnitPrice,
    decimal Subtotal
);

public record OrderResponse(
    int Id,
    int BranchId,
    string BranchName,
    int CreatedByUserId,
    string CreatedByRole,
    int? CustomerId,
    string PaymentMethod,
    string? CardLast4,
    string Status,
    DateTime? CompletedAt,
    decimal SubtotalAmount,
    decimal PointsSpent,
    decimal TotalAmount,
    decimal PointsEarned,
    IReadOnlyList<OrderProductLineResponse> Products,
    IReadOnlyList<OrderDishLineResponse> Dishes,
    IReadOnlyList<OrderComboLineResponse> Combos,
    DateTime CreatedAt
);

public record PagedOrdersResponse(
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages,
    IReadOnlyList<OrderResponse> Items
);

public static class OrderDtoMapper
{
    public static OrderResponse ToResponse(this Order order) =>
        new(
            order.Id,
            order.Branch.Id,
            order.Branch.Name,
            order.CreatedByUser.ID,
            order.CreatedByUser.Role,
            order.Customer?.ID,
            order.PaymentMethod,
            order.CardLast4,
            order.Status,
            order.CompletedAt,
            order.SubtotalAmount,
            order.PointsSpent,
            order.TotalAmount,
            order.PointsEarned,
            order.Products
                .Select(p => new OrderProductLineResponse(
                    p.Product.Id,
                    p.Product.Description,
                    p.Quantity,
                    p.UnitPrice,
                    p.Quantity * p.UnitPrice))
                .ToList(),
            order.Dishes
                .Select(d => new OrderDishLineResponse(
                    d.Dish.Id,
                    d.Dish.Name,
                    d.Quantity,
                    d.UnitPrice,
                    d.Quantity * d.UnitPrice))
                .ToList(),
            order.Combos
                .Select(c => new OrderComboLineResponse(
                    c.Combo.Id,
                    c.Combo.Name,
                    c.Quantity,
                    c.UnitPrice,
                    c.Quantity * c.UnitPrice))
                .ToList(),
            order.CreatedAt);
}
