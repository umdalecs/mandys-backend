using Mandys.Domain;
using Mandys.Infrastructure.Persistence.Records;

namespace Mandys.Infrastructure.Persistence.Mappers;

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

internal static class DishProductMapper
{
    internal static DishProduct ToDomain(this DishProductRecord record) =>
        new(record.Product.ToDomain(), record.Quantity);

    internal static DishProductRecord ToRecord(this DishProduct line) =>
        new()
        {
            ProductId = line.Product.Id,
            Quantity = line.Quantity,
        };
}
