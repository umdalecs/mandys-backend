using Mandys.Domain;
using Mandys.Services;
using Microsoft.EntityFrameworkCore;

namespace Mandys.Infrastructure.Persistence;

public class UserRepository(ApplicationDbContext db) : IUserRepository
{
    public async Task<User?> GetByIdAsync(Guid id)
    {
        var record = await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
        return record?.ToDomain();
    }

    public async Task<User?> FindByEmailAsync(string email)
    {
        var normalized = email.Trim().ToLowerInvariant();
        var record = await db.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == normalized);
        return record?.ToDomain();
    }

    public Task<bool> ExistsByEmailAsync(string email, Guid? excludingId = null)
    {
        var normalized = email.Trim().ToLowerInvariant();
        return excludingId.HasValue
            ? db.Users.AnyAsync(u => u.Id != excludingId.Value && u.Email.ToLower() == normalized)
            : db.Users.AnyAsync(u => u.Email.ToLower() == normalized);
    }

    public async Task<(int TotalCount, IReadOnlyList<User> Items)> SearchAsync(
        string? search, string? role, int page, int pageSize)
    {
        var query = db.Users.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(u =>
                u.Email.ToLower().Contains(term) ||
                u.FirstName.ToLower().Contains(term) ||
                u.LastName.ToLower().Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(role))
        {
            var roleFilter = role.Trim().ToLower();
            query = query.Where(u => u.Role.ToLower() == roleFilter);
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(u => u.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (totalCount, items.Select(r => r.ToDomain()).ToList());
    }

    public async Task AddAsync(User user)
    {
        db.Users.Add(user.ToRecord());
        await db.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        db.Users.Update(user.ToRecord());
        await db.SaveChangesAsync();
    }

    public async Task<bool> RemoveAsync(Guid id)
    {
        var record = await db.Users.FindAsync(id);
        if (record is null)
        {
            return false;
        }

        // Soft-deleted by the SaveChangesAsync interceptor.
        db.Users.Remove(record);
        await db.SaveChangesAsync();
        return true;
    }
}
