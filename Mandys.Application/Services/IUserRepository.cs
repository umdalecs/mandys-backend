using Mandys.Domain;

namespace Mandys.Services;

/// <summary>
/// User persistence in domain terms. Implemented by Infrastructure.
/// </summary>
public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);

    Task<User?> FindByEmailAsync(string email);

    Task<bool> ExistsByEmailAsync(string email, int? excludingId = null);

    Task<(int TotalCount, IReadOnlyList<User> Items)> SearchAsync(
        string? search, string? role, int page, int pageSize);

    /// <returns>The saved user, with its database-generated id.</returns>
    Task<User> AddAsync(User user);

    Task UpdateAsync(User user);

    /// <returns>False when no user with the id exists (soft-delete).</returns>
    Task<bool> RemoveAsync(int id);
}
