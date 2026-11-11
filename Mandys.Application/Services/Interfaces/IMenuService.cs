using Mandys.Application.DTOs;

namespace Mandys.Application.Services.Interfaces;

public interface IMenuService
{
    Task<PagedMenuProductsResponse> GetProductsAsync(int page, int pageSize, string? search, string? orderBy);

    Task<PagedMenuDishesResponse> GetDishesAsync(int page, int pageSize, string? search, string? orderBy);

    Task<PagedMenuCombosResponse> GetCombosAsync(int page, int pageSize, string? search, string? orderBy);
}
