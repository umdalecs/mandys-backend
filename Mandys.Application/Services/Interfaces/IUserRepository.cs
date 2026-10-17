using Mandys.Domain;

namespace Mandys.Application.Services.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);

    Task<User?> FindByEmailAsync(string email);

    Task<bool> ExistsByEmailAsync(string email, int? excludingId = null);

    Task<(int TotalCount, IReadOnlyList<User> Items)> SearchAsync(
        string? search, string? role, int page, int pageSize, string? orderBy = null);
    
    Task<User> AddAsync(User user);

    Task UpdateAsync(User user);

    Task<bool> RemoveAsync(int id);
}
