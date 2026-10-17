using Mandys.Domain;

namespace Mandys.Application.Services.Interfaces;

public interface IBranchRepository
{
    Task<Branch?> GetByIdAsync(int id);

    Task<(int TotalCount, IReadOnlyList<Branch> Items)> SearchAsync(
        string? search, int page, int pageSize, string? orderBy);

    Task<Branch> AddAsync(Branch branch);

    Task UpdateAsync(Branch branch);

    Task<bool> RemoveAsync(int id);

    Task<int> CountUsersAsync(int id);
}
