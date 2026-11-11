using Mandys.Application.DTOs;
using Mandys.Application.Services.Interfaces;

namespace Mandys.Application.Services.Implementations;

public class MenuService(IMenuRepository menu) : IMenuService
{
    public async Task<PagedMenuProductsResponse> GetProductsAsync(
        int page, int pageSize, string? search, string? orderBy)
    {
        (page, pageSize) = ClampPaging(page, pageSize);

        var (totalCount, items) = await menu.SearchProductsAsync(search, page, pageSize, orderBy);

        return new PagedMenuProductsResponse(totalCount, page, pageSize, TotalPages(totalCount, pageSize),
            items.Select(p => p.ToMenuResponse()).ToList());
    }

    public async Task<PagedMenuDishesResponse> GetDishesAsync(
        int page, int pageSize, string? search, string? orderBy)
    {
        (page, pageSize) = ClampPaging(page, pageSize);

        var (totalCount, items) = await menu.SearchDishesAsync(search, page, pageSize, orderBy);

        return new PagedMenuDishesResponse(totalCount, page, pageSize, TotalPages(totalCount, pageSize),
            items.Select(d => d.ToMenuResponse()).ToList());
    }

    public async Task<PagedMenuCombosResponse> GetCombosAsync(
        int page, int pageSize, string? search, string? orderBy)
    {
        (page, pageSize) = ClampPaging(page, pageSize);

        var (totalCount, items) = await menu.SearchCombosAsync(search, page, pageSize, orderBy);

        return new PagedMenuCombosResponse(totalCount, page, pageSize, TotalPages(totalCount, pageSize),
            items.Select(c => c.ToMenuResponse()).ToList());
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
