using Mandys.Domain;
using Mandys.Infrastructure.Persistence.Records;

namespace Mandys.Infrastructure.Persistence.Mappers;

/// <summary>
/// Maps between the product table record and the domain entity.
/// </summary>
internal static class ProductMapper
{
    internal static Product ToDomain(this ProductRecord record) =>
        new(
            record.Id,
            record.Description,
            record.IsSupply,
            record.Price,
            record.MeasureUnit,
            record.CreatedAt,
            record.UpdatedAt);

    internal static ProductRecord ToRecord(this Product product) =>
        new()
        {
            Description = product.Description,
            IsSupply = product.IsSupply,
            Price = product.Price,
            MeasureUnit = product.MeasureUnit,
        };
}
