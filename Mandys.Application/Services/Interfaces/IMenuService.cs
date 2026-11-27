using Mandys.Application.DTOs;

namespace Mandys.Application.Services.Interfaces;

public interface IMenuService
{
    Task<PagedMenuProductsResponse> GetProductsAsync(int branchId, int page, int pageSize, string? search, string? orderBy);

    Task<PagedMenuDishesResponse> GetDishesAsync(int branchId, int page, int pageSize, string? search, string? orderBy);

    Task<PagedMenuCombosResponse> GetCombosAsync(int branchId, int page, int pageSize, string? search, string? orderBy);
}
