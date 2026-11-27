using Mandys.Domain;
using Mandys.Infrastructure.Persistence.Records;

namespace Mandys.Infrastructure.Persistence.Mappers;

internal static class OrderMapper
{
    internal static Order ToDomain(this OrderRecord record) =>
        new(
            record.Id,
            record.Branch.ToDomain(),
            record.CreatedByUser.ToDomain(),
            record.PaymentMethod,
            record.CardLast4,
            record.OrderProducts.Select(l => l.ToDomain()),
            record.OrderDishes.Select(l => l.ToDomain()),
            record.OrderCombos.Select(l => l.ToDomain()),
            record.CreatedAt,
            record.UpdatedAt,
            record.Customer?.ToDomain(),
            record.PointsEarned,
            record.PointsSpent,
            record.Status,
            record.CompletedAt);

    internal static OrderRecord ToRecord(this Order order) =>
        new()
        {
            BranchId = order.Branch.Id,
            CreatedByUserId = order.CreatedByUser.ID,
            CustomerId = order.Customer?.ID,
            PaymentMethod = order.PaymentMethod,
            CardLast4 = order.CardLast4,
            Status = order.Status,
            CompletedAt = order.CompletedAt,
            SubtotalAmount = order.SubtotalAmount,
            PointsSpent = order.PointsSpent,
            TotalAmount = order.TotalAmount,
            PointsEarned = order.PointsEarned,
            OrderProducts = order.Products.Select(l => l.ToRecord()).ToList(),
            OrderDishes = order.Dishes.Select(l => l.ToRecord()).ToList(),
            OrderCombos = order.Combos.Select(l => l.ToRecord()).ToList(),
        };
}

internal static class OrderProductMapper
{
    internal static OrderProduct ToDomain(this OrderProductRecord record) =>
        new(record.Product.ToDomain(), record.Quantity, record.UnitPrice);

    internal static OrderProductRecord ToRecord(this OrderProduct line) =>
        new()
        {
            ProductId = line.Product.Id,
            Quantity = line.Quantity,
            UnitPrice = line.UnitPrice,
        };
}

internal static class OrderDishMapper
{
    internal static OrderDish ToDomain(this OrderDishRecord record) =>
        new(record.Dish.ToDomain(), record.Quantity, record.UnitPrice);

    internal static OrderDishRecord ToRecord(this OrderDish line) =>
        new()
        {
            DishId = line.Dish.Id,
            Quantity = line.Quantity,
            UnitPrice = line.UnitPrice,
        };
}

internal static class OrderComboMapper
{
    internal static OrderCombo ToDomain(this OrderComboRecord record) =>
        new(record.Combo.ToDomain(), record.Quantity, record.UnitPrice);

    internal static OrderComboRecord ToRecord(this OrderCombo line) =>
        new()
        {
            ComboId = line.Combo.Id,
            Quantity = line.Quantity,
            UnitPrice = line.UnitPrice,
        };
}
