using Mandys.Domain;

namespace Mandys.DTOs;

public record ProductResponse(
    int Id,
    string Description,
    bool IsSupply,
    string Price,
    string MeasureUnit
);

public record CreateProductRequest(
    string Description,
    bool IsSupply,
    string Price,
    string MeasureUnit
);

public record UpdateProductRequest(
    string? Description = null,
    bool? IsSupply = null,
    string? Price = null,
    string? MeasureUnit = null
);

public record PagedProductsResponse(
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages,
    IReadOnlyList<ProductResponse> Items
);

public static class ProductMapper
{
    public static ProductResponse ToResponse(this Product product) =>
        new(
            product.Id,
            product.Description,
            product.IsSupply,
            product.Price,
            product.MeasureUnit
        );
}
