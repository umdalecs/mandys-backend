using Mandys.Domain;
using Mandys.Infrastructure.Persistence.Mappers;
using Mandys.Infrastructure.Persistence.Records;
using Mandys.Services;
using Mandys.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Mandys.Infrastructure.Persistence.Repositories;

public class UserRepository(ApplicationDbContext db) : IUserRepository
{
    public async Task<User?> GetByIdAsync(int id)
    {
        var record = await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
        return record?.ToDomain();
    }

    public async Task<User?> FindByEmailAsync(string email)
    {
        var normalized = email.Trim().ToLowerInvariant();
        // Rows with a null email never match, which is what keeps
        // credential-less users out of the login flow.
        var record = await db.Users
            .FirstOrDefaultAsync(u => u.Email!.ToLower() == normalized);
        return record?.ToDomain();
    }

    public Task<bool> ExistsByEmailAsync(string email, int? excludingId = null)
    {
        var normalized = email.Trim().ToLowerInvariant();
        return excludingId.HasValue
            ? db.Users.AnyAsync(u => u.Id != excludingId && u.Email!.ToLower() == normalized)
            : db.Users.AnyAsync(u => u.Email!.ToLower() == normalized);
    }

    public async Task<(int TotalCount, IReadOnlyList<User> Items)> SearchAsync(
        string? search, string? role, int page, int pageSize)
    {
        var query = db.Users.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(u =>
                (u.Email != null && u.Email.ToLower().Contains(term)) ||
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
            .OrderBy(u => u.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (totalCount, items.Select(r => r.ToDomain()).ToList());
    }

    public async Task<User> AddAsync(User user)
    {
        var record = user.ToRecord();
        db.Users.Add(record);
        await db.SaveChangesAsync();
        return record.ToDomain();
    }

    public async Task UpdateAsync(User user)
    {
        // Update the tracked record so the identity key and audit columns
        // are preserved.
        var record = await db.Users.FirstOrDefaultAsync(u => u.Id == user.ID);
        if (record is null)
        {
            throw ServiceException.NotFound($"User with ID '{user.ID}' not found.");
        }

        record.FirstName = user.FirstName;
        record.LastName = user.LastName;
        record.Email = user.Email;
        record.PasswordHash = user.PasswordHash;
        record.Role = user.Role;
        record.BranchId = user.BranchId;
        record.BannedAt = user.BannedAt;

        await db.SaveChangesAsync();
    }

    public async Task<bool> RemoveAsync(int id)
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
