using Mandys.DTOs;

namespace Mandys.Services.Interfaces;

public interface IDishService
{
    Task<PagedDishesResponse> GetDishesAsync(int page, int pageSize, string? search);

    Task<DishResponse> GetDishByIdAsync(int id);

    Task<DishResponse> CreateDishAsync(CreateDishRequest request);

    Task<DishResponse> UpdateDishAsync(int id, UpdateDishRequest request);

    Task DeleteDishAsync(int id);
}
