using Mandys.Application.DTOs;
using Mandys.Application.Services.Interfaces;
using Mandys.Domain;

namespace Mandys.Application.Services.Implementations;

public class MenuService(
    IMenuRepository menu,
    IBranchRepository branches,
    IBranchStorageRepository branchStorages) : IMenuService
{
    public async Task<PagedMenuProductsResponse> GetProductsAsync(
        int branchId, int page, int pageSize, string? search, string? orderBy)
    {
        await EnsureSellingBranchAsync(branchId);
        (page, pageSize) = ClampPaging(page, pageSize);

        var (totalCount, items) = await menu.SearchProductsAsync(search, page, pageSize, orderBy);
        var productIds = items.Select(p => p.Id).Distinct().ToList();
        var stockByProductId = await GetStockMapAsync(branchId, productIds);

        return new PagedMenuProductsResponse(
            totalCount,
            page,
            pageSize,
            TotalPages(totalCount, pageSize),
            items.Select(p => p.ToMenuResponse(stockByProductId.GetValueOrDefault(p.Id, 0m))).ToList());
    }

    public async Task<PagedMenuDishesResponse> GetDishesAsync(
        int branchId, int page, int pageSize, string? search, string? orderBy)
    {
        await EnsureSellingBranchAsync(branchId);
        (page, pageSize) = ClampPaging(page, pageSize);

        var (totalCount, items) = await menu.SearchDishesAsync(search, page, pageSize, orderBy);
        var productIds = items
            .SelectMany(d => d.Recipe)
            .Select(r => r.Product.Id)
            .Distinct()
            .ToList();
        var stockByProductId = await GetStockMapAsync(branchId, productIds);

        return new PagedMenuDishesResponse(
            totalCount,
            page,
            pageSize,
            TotalPages(totalCount, pageSize),
            items.Select(d => d.ToMenuResponse(IsDishAvailable(d, 1m, stockByProductId))).ToList());
    }

    public async Task<PagedMenuCombosResponse> GetCombosAsync(
        int branchId, int page, int pageSize, string? search, string? orderBy)
    {
        await EnsureSellingBranchAsync(branchId);
        (page, pageSize) = ClampPaging(page, pageSize);

        var (totalCount, items) = await menu.SearchCombosAsync(search, page, pageSize, orderBy);
        var productIds = items
            .SelectMany(c => c.Products.Select(p => p.Product.Id)
                .Concat(c.Dishes.SelectMany(d => d.Dish.Recipe.Select(r => r.Product.Id))))
            .Distinct()
            .ToList();
        var stockByProductId = await GetStockMapAsync(branchId, productIds);

        var responses = items.Select(combo =>
        {
            var dishItems = combo.Dishes
                .Select(d => new MenuComboItemResponse(
                    d.Dish.Id,
                    d.Dish.Name,
                    d.Quantity,
                    IsDishAvailable(d.Dish, d.Quantity, stockByProductId)))
                .ToList();

            var productItems = combo.Products
                .Select(p => new MenuComboItemResponse(
                    p.Product.Id,
                    p.Product.Description,
                    p.Quantity,
                    stockByProductId.GetValueOrDefault(p.Product.Id, 0m) >= p.Quantity))
                .ToList();

            var comboAvailable =
                dishItems.All(d => d.IsAvailable) &&
                productItems.All(p => p.IsAvailable) &&
                HasAggregatedStockForCombo(combo, stockByProductId);

            return combo.ToMenuResponse(comboAvailable, dishItems, productItems);
        }).ToList();

        return new PagedMenuCombosResponse(
            totalCount,
            page,
            pageSize,
            TotalPages(totalCount, pageSize),
            responses);
    }

    private async Task EnsureSellingBranchAsync(int branchId)
    {
        var branch = await branches.GetByIdAsync(branchId);
        if (branch is null)
        {
            throw ServiceException.NotFound($"No se encontró la sucursal con ID '{branchId}'.");
        }

        if (branch.WarehouseOnly)
        {
            throw ServiceException.BadRequest(
                $"La sucursal '{branch.Name}' es solo de almacén y no cuenta con menú de venta.");
        }
    }

    private async Task<Dictionary<int, decimal>> GetStockMapAsync(
        int branchId,
        IReadOnlyCollection<int> productIds)
    {
        if (productIds.Count == 0)
        {
            return [];
        }

        var storages = await branchStorages.GetByBranchAndProductsAsync(branchId, productIds);
        return storages.ToDictionary(s => s.Product.Id, s => s.CurrentStock);
    }

    private static bool IsDishAvailable(
        Dish dish,
        decimal dishQuantity,
        IReadOnlyDictionary<int, decimal> stockByProductId) =>
        dish.Recipe.All(r =>
            stockByProductId.GetValueOrDefault(r.Product.Id, 0m) >= r.Quantity * dishQuantity);

    private static bool HasAggregatedStockForCombo(
        Combo combo,
        IReadOnlyDictionary<int, decimal> stockByProductId)
    {
        var requiredByProductId = new Dictionary<int, decimal>();

        foreach (var productLine in combo.Products)
        {
            requiredByProductId[productLine.Product.Id] =
                requiredByProductId.GetValueOrDefault(productLine.Product.Id, 0m) + productLine.Quantity;
        }

        foreach (var dishLine in combo.Dishes)
        {
            foreach (var recipeLine in dishLine.Dish.Recipe)
            {
                requiredByProductId[recipeLine.Product.Id] =
                    requiredByProductId.GetValueOrDefault(recipeLine.Product.Id, 0m) +
                    recipeLine.Quantity * dishLine.Quantity;
            }
        }

        return requiredByProductId.All(kv =>
            stockByProductId.GetValueOrDefault(kv.Key, 0m) >= kv.Value);
    }

    private static (int Page, int PageSize) ClampPaging(int page, int pageSize)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100;
        return (page, pageSize);
    }

    private static int TotalPages(int totalCount, int pageSize) =>
        (int)Math.Ceiling(totalCount / (double)pageSize);
}
