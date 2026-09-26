using Mandys.Domain;

namespace Mandys.Infrastructure.Persistence;

/// <summary>
/// Maps between the dishes table record and the domain entity.
/// </summary>
internal static class DishMapper
{
    internal static Dish ToDomain(this DishRecord record) =>
        new(
            record.Id,
            record.Name,
            record.Price,
            record.DishProducts.Select(l => l.ToDomain()),
            record.CreatedAt,
            record.UpdatedAt);

    internal static DishRecord ToRecord(this Dish dish) =>
        new()
        {
            Name = dish.Name,
            Price = dish.Price,
            DishProducts = dish.Recipe.Select(l => l.ToRecord()).ToList(),
        };
}

/// <summary>
/// Maps between the dish products table record and the domain recipe line.
/// </summary>
internal static class DishProductMapper
{
    internal static DishProduct ToDomain(this DishProductRecord record) =>
        new(record.ProductId, record.Quantity);

    internal static DishProductRecord ToRecord(this DishProduct line) =>
        new()
        {
            ProductId = line.ProductId,
            Quantity = line.Quantity,
        };
}
