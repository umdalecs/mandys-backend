using Mandys.Domain;

namespace Mandys.Application.Services.Interfaces;

public interface IDishRepository
{
    Task<Dish?> GetByIdAsync(int id);

    Task<(int TotalCount, IReadOnlyList<Dish> Items)> SearchAsync(
        string? search, int page, int pageSize, string? orderBy);

    Task<Dish> AddAsync(Dish dish);

    Task UpdateAsync(Dish dish);

    Task<bool> RemoveAsync(int id);

    Task<bool> ExistsByNameAsync(string name, int? excludingId = null);
}
