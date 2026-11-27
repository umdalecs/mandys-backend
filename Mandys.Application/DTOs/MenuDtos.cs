using Mandys.Domain;

namespace Mandys.Application.DTOs;

// Customer-facing catalog DTOs. Only expose what a customer needs to browse
// and order: no cost prices, recipes, supply flags, or audit dates.

public record MenuProductResponse(
    int Id,
    string Name,
    decimal Price,
    string? ImageUrl,
    decimal AvailableQuantity,
    bool IsAvailable
);

public record MenuDishResponse(
    int Id,
    string Name,
    decimal Price,
    string? ImageUrl,
    bool IsAvailable
);

public record MenuComboItemResponse(
    int Id,
    string Name,
    decimal Quantity,
    bool IsAvailable
);

public record MenuComboResponse(
    int Id,
    string Name,
    decimal Price,
    string? ImageUrl,
    bool IsAvailable,
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
    public static MenuProductResponse ToMenuResponse(
        this Product product,
        decimal availableQuantity) =>
        new(
            product.Id,
            product.Description,
            product.SalePrice ?? 0,
            product.ImageUrl,
            availableQuantity,
            availableQuantity >= 1m
        );

    public static MenuDishResponse ToMenuResponse(
        this Dish dish,
        bool isAvailable) =>
        new(
            dish.Id,
            dish.Name,
            dish.Price,
            dish.ImageUrl,
            isAvailable
        );

    public static MenuComboResponse ToMenuResponse(
        this Combo combo,
        bool isAvailable,
        IReadOnlyList<MenuComboItemResponse> dishes,
        IReadOnlyList<MenuComboItemResponse> products) =>
        new(
            combo.Id,
            combo.Name,
            combo.Price,
            combo.ImageUrl,
            isAvailable,
            dishes,
            products
        );
}
