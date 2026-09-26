using Mandys.Domain;
using Mandys.Infrastructure.Persistence.Records;

namespace Mandys.Infrastructure.Persistence.Mappers;

/// <summary>
/// Maps between the combos table record and the domain entity.
/// </summary>
internal static class ComboMapper
{
    internal static Combo ToDomain(this ComboRecord record) =>
        new(
            record.Id,
            record.Name,
            record.Price,
            record.ComboDishes.Select(l => l.ToDomain()),
            record.ComboProducts.Select(l => l.ToDomain()),
            record.CreatedAt,
            record.UpdatedAt);

    internal static ComboRecord ToRecord(this Combo combo) =>
        new()
        {
            Name = combo.Name,
            Price = combo.Price,
            ComboDishes = combo.Dishes.Select(l => l.ToRecord()).ToList(),
            ComboProducts = combo.Products.Select(l => l.ToRecord()).ToList(),
        };
}

/// <summary>
/// Maps between the combo dishes table record and the domain combo line.
/// </summary>
internal static class ComboDishMapper
{
    internal static ComboDish ToDomain(this ComboDishRecord record) =>
        new(record.DishId, record.Quantity);

    internal static ComboDishRecord ToRecord(this ComboDish line) =>
        new()
        {
            DishId = line.DishId,
            Quantity = line.Quantity,
        };
}

/// <summary>
/// Maps between the combo products table record and the domain combo line.
/// </summary>
internal static class ComboProductMapper
{
    internal static ComboProduct ToDomain(this ComboProductRecord record) =>
        new(record.ProductId, record.Quantity);

    internal static ComboProductRecord ToRecord(this ComboProduct line) =>
        new()
        {
            ProductId = line.ProductId,
            Quantity = line.Quantity,
        };
}
