using Mandys.Domain;

namespace Mandys.Application.Services.Interfaces;

public interface IComboRepository
{
    Task<Combo?> GetByIdAsync(int id);

    Task<(int TotalCount, IReadOnlyList<Combo> Items)> SearchAsync(
        string? search, int page, int pageSize, string? orderBy);

    Task<Combo> AddAsync(Combo combo);

    Task UpdateAsync(Combo combo);

    Task<bool> RemoveAsync(int id);

    Task<bool> ExistsByNameAsync(string name, int? excludingId = null);
}
