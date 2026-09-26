using Mandys.Domain;

namespace Mandys.Services.Interfaces;

/// <summary>
/// Branch persistence in domain terms. Implemented by Infrastructure.
/// </summary>
public interface IBranchRepository
{
    Task<Branch?> GetByIdAsync(int id);

    Task<(int TotalCount, IReadOnlyList<Branch> Items)> SearchAsync(
        string? search, int page, int pageSize);

    /// <returns>The saved branch, with its database-generated id.</returns>
    Task<Branch> AddAsync(Branch branch);

    Task UpdateAsync(Branch branch);

    /// <returns>False when no branch with the id exists.</returns>
    Task<bool> RemoveAsync(int id);

    /// <summary>Number of users still assigned to the branch.</summary>
    Task<int> CountUsersAsync(int id);
}
