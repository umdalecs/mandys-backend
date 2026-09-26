using Mandys.Domain;

namespace Mandys.Services.Interfaces;

/// <summary>
/// Dish persistence in domain terms. Implemented by Infrastructure.
/// </summary>
public interface IDishRepository
{
    Task<Dish?> GetByIdAsync(int id);

    Task<(int TotalCount, IReadOnlyList<Dish> Items)> SearchAsync(
        string? search, int page, int pageSize);

    /// <returns>The saved dish, with its database-generated id.</returns>
    Task<Dish> AddAsync(Dish dish);

    Task UpdateAsync(Dish dish);

    /// <returns>False when no dish with the id exists (soft-delete).</returns>
    Task<bool> RemoveAsync(int id);
}
