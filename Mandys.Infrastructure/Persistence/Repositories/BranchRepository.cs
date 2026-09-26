using Mandys.Domain;
using Mandys.Infrastructure.Persistence.Mappers;
using Mandys.Infrastructure.Persistence.Records;
using Mandys.Services;
using Mandys.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Mandys.Infrastructure.Persistence.Repositories;

public class BranchRepository(ApplicationDbContext db) : IBranchRepository
{
    public async Task<Branch?> GetByIdAsync(int id)
    {
        var record = await db.Branches.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);
        return record?.ToDomain();
    }

    public async Task<(int TotalCount, IReadOnlyList<Branch> Items)> SearchAsync(
        string? search, int page, int pageSize)
    {
        var query = db.Branches.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(b =>
                b.Name.ToLower().Contains(term) ||
                b.Address.ToLower().Contains(term));
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(b => b.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (totalCount, items.Select(r => r.ToDomain()).ToList());
    }

    public async Task<Branch> AddAsync(Branch branch)
    {
        var record = branch.ToRecord();
        db.Branches.Add(record);
        await db.SaveChangesAsync();
        return record.ToDomain();
    }

    public async Task UpdateAsync(Branch branch)
    {
        // Update the tracked record so the identity key and audit columns
        // are preserved.
        var record = await db.Branches.FirstOrDefaultAsync(b => b.Id == branch.Id);
        if (record is null)
        {
            throw ServiceException.NotFound($"Branch with ID '{branch.Id}' not found.");
        }

        record.Name = branch.Name;
        record.Address = branch.Address;
        record.WarehouseOnly = branch.WarehouseOnly;

        await db.SaveChangesAsync();
    }

    public async Task<bool> RemoveAsync(int id)
    {
        var record = await db.Branches.FindAsync(id);
        if (record is null)
        {
            return false;
        }

        db.Branches.Remove(record);
        await db.SaveChangesAsync();
        return true;
    }

    public Task<int> CountUsersAsync(int id) =>
        db.Users.CountAsync(u => u.BranchId == id);
}
