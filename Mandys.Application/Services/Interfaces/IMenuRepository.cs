using Mandys.Domain;

namespace Mandys.Application.Services.Interfaces;

/// <summary>
/// Read-only access to the customer-facing catalog (sellable products,
/// dishes and combos). Returned entities are lightweight: dish recipes are
/// not loaded, since customers never see them.
/// </summary>
public interface IMenuRepository
{
    Task<(int TotalCount, IReadOnlyList<Product> Items)> SearchProductsAsync(
        string? search, int page, int pageSize, string? orderBy);

    Task<(int TotalCount, IReadOnlyList<Dish> Items)> SearchDishesAsync(
        string? search, int page, int pageSize, string? orderBy);

    Task<(int TotalCount, IReadOnlyList<Combo> Items)> SearchCombosAsync(
        string? search, int page, int pageSize, string? orderBy);
}
