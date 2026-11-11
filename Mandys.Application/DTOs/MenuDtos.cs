using Mandys.Domain;

namespace Mandys.Application.DTOs;

// Customer-facing catalog DTOs. Only expose what a customer needs to browse
// and order: no cost prices, recipes, supply flags, stock or audit dates.

public record MenuProductResponse(
    int Id,
    string Name,
    decimal Price,
    string? ImageUrl
);

public record MenuDishResponse(
    int Id,
    string Name,
    decimal Price,
    string? ImageUrl
);

public record MenuComboItemResponse(
    int Id,
    string Name,
    decimal Quantity
);

public record MenuComboResponse(
    int Id,
    string Name,
    decimal Price,
    string? ImageUrl,
    IReadOnlyList<MenuComboItemResponse> Dishes,
    IReadOnlyList<MenuComboItemResponse> Products
);

public record PagedMenuProductsResponse(
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages,
    IReadOnlyList<MenuProductResponse> Items
);

public record PagedMenuDishesResponse(
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages,
    IReadOnlyList<MenuDishResponse> Items
);

public record PagedMenuCombosResponse(
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages,
    IReadOnlyList<MenuComboResponse> Items
);

public static class MenuMapper
{
    public static MenuProductResponse ToMenuResponse(this Product product) =>
        new(
            product.Id,
            product.Description,
            product.SalePrice ?? 0,
            product.ImageUrl
        );

    public static MenuDishResponse ToMenuResponse(this Dish dish) =>
        new(
            dish.Id,
            dish.Name,
            dish.Price,
            dish.ImageUrl
        );

    public static MenuComboResponse ToMenuResponse(this Combo combo) =>
        new(
            combo.Id,
            combo.Name,
            combo.Price,
            combo.ImageUrl,
            combo.Dishes
                .Select(l => new MenuComboItemResponse(l.Dish.Id, l.Dish.Name, l.Quantity))
                .ToList(),
            combo.Products
                .Select(l => new MenuComboItemResponse(l.Product.Id, l.Product.Description, l.Quantity))
                .ToList()
        );
}
