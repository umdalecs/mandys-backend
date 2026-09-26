using Mandys.Domain;

namespace Mandys.Services.Interfaces;

/// <summary>
/// Combo persistence in domain terms. Implemented by Infrastructure.
/// </summary>
public interface IComboRepository
{
    Task<Combo?> GetByIdAsync(int id);

    Task<(int TotalCount, IReadOnlyList<Combo> Items)> SearchAsync(
        string? search, int page, int pageSize);

    /// <returns>The saved combo, with its database-generated id.</returns>
    Task<Combo> AddAsync(Combo combo);

    Task UpdateAsync(Combo combo);

    /// <returns>False when no combo with the id exists (soft-delete).</returns>
    Task<bool> RemoveAsync(int id);
}
