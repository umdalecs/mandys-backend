using Mandys.Domain;

namespace Mandys.DTOs;

public record ComboDishLineResponse(
    int DishId,
    decimal Quantity
);

public record ComboProductLineResponse(
    int ProductId,
    decimal Quantity
);

public record ComboResponse(
    int Id,
    string Name,
    decimal Price,
    IReadOnlyList<ComboDishLineResponse> Dishes,
    IReadOnlyList<ComboProductLineResponse> Products
);

public record CreateComboDishLineRequest(
    int DishId,
    decimal Quantity
);

public record CreateComboProductLineRequest(
    int ProductId,
    decimal Quantity
);

public record CreateComboRequest(
    string Name,
    decimal Price,
    IReadOnlyList<CreateComboDishLineRequest>? Dishes = null,
    IReadOnlyList<CreateComboProductLineRequest>? Products = null
);

public record UpdateComboRequest(
    string? Name = null,
    decimal? Price = null,
    IReadOnlyList<CreateComboDishLineRequest>? Dishes = null,
    IReadOnlyList<CreateComboProductLineRequest>? Products = null
);

public record PagedCombosResponse(
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages,
    IReadOnlyList<ComboResponse> Items
);

public static class ComboMapper
{
    public static ComboResponse ToResponse(this Combo combo) =>
        new(
            combo.Id,
            combo.Name,
            combo.Price,
            combo.Dishes.Select(l => new ComboDishLineResponse(l.DishId, l.Quantity)).ToList(),
            combo.Products.Select(l => new ComboProductLineResponse(l.ProductId, l.Quantity)).ToList()
        );
}
