using Mandys.Domain;

namespace Mandys.DTOs;

public record DishRecipeLineResponse(
    ProductResponse Product,
    decimal Quantity
);

public record DishResponse(
    int Id,
    string Name,
    decimal Price,
    IReadOnlyList<DishRecipeLineResponse> Recipe
);

public record CreateDishRecipeLineRequest(
    int ProductId,
    decimal Quantity
);

public record CreateDishRequest(
    string Name,
    decimal Price,
    IReadOnlyList<CreateDishRecipeLineRequest>? Recipe = null
);

public record UpdateDishRequest(
    string? Name = null,
    decimal? Price = null,
    IReadOnlyList<CreateDishRecipeLineRequest>? Recipe = null
);

public record PagedDishesResponse(
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages,
    IReadOnlyList<DishResponse> Items
);

public static class DishMapper
{
    public static DishResponse ToResponse(this Dish dish) =>
        new(
            dish.Id,
            dish.Name,
            dish.Price,
            dish.Recipe.Select(l => new DishRecipeLineResponse(l.Product.ToResponse(), l.Quantity)).ToList()
        );
}
