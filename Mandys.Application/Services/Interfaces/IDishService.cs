using Mandys.Application.DTOs;

namespace Mandys.Application.Services.Interfaces;

public interface IDishService
{
    Task<PagedDishesResponse> GetDishesAsync(int page, int pageSize, string? search, string? orderBy);

    Task<DishResponse> GetDishByIdAsync(int id);

    Task<DishResponse> CreateDishAsync(CreateDishRequest request);

    Task<DishResponse> UpdateDishAsync(int id, UpdateDishRequest request);

    Task<DishResponse> UpdateDishImageAsync(int id, Stream file, string contentType);

    Task DeleteDishAsync(int id);
}
