using Mandys.Domain;

namespace Mandys.Application.DTOs;

public record ProductResponse(
    int Id,
    string Description,
    bool IsSupply,
    decimal? CostPrice,
    decimal? SalePrice,
    string MeasureUnit,
    string? imageUrl
);

public record CreateProductRequest(
    string Description,
    bool IsSupply,
    string MeasureUnit,
    decimal? SalePrice = null
);

public record UpdateProductRequest(
    string? Description = null,
    bool? IsSupply = null,
    decimal? SalePrice = null,
    string? MeasureUnit = null
);

public record PagedProductsResponse(
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages,
    IReadOnlyList<ProductResponse> Items
);

public record MeasureUnitResponse(
    string Value,
    bool AllowsDecimals,
    bool SupplyOnly
);

public static class ProductMapper
{
    public static ProductResponse ToResponse(this Product product) =>
        new(
            product.Id,
            product.Description,
            product.IsSupply,
            product.CostPrice,
            product.SalePrice,
            product.MeasureUnit,
            product.ImageUrl
        );
}
