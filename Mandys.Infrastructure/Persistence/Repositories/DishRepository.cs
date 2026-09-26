using Mandys.Domain;
using Mandys.Infrastructure.Persistence.Mappers;
using Mandys.Infrastructure.Persistence.Records;
using Mandys.Services;
using Mandys.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Mandys.Infrastructure.Persistence.Repositories;

public class DishRepository(ApplicationDbContext db) : IDishRepository
{
    public async Task<Dish?> GetByIdAsync(int id)
    {
        var record = await db.Dishes
            .AsNoTracking()
            .Include(d => d.DishProducts)
            .FirstOrDefaultAsync(d => d.Id == id);
        return record?.ToDomain();
    }

    public async Task<(int TotalCount, IReadOnlyList<Dish> Items)> SearchAsync(
        string? search, int page, int pageSize)
    {
        IQueryable<DishRecord> query = db.Dishes.AsNoTracking().Include(d => d.DishProducts);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(d => d.Name.ToLower().Contains(term));
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(d => d.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (totalCount, items.Select(r => r.ToDomain()).ToList());
    }

    public async Task<Dish> AddAsync(Dish dish)
    {
        var record = dish.ToRecord();
        db.Dishes.Add(record);
        await db.SaveChangesAsync();
        return record.ToDomain();
    }

    public async Task UpdateAsync(Dish dish)
    {
        // Update the tracked record so the identity key and audit columns
        // are preserved. Recipe lines are synced: quantities updated, missing
        // lines added, lines absent from the dish removed.
        var record = await db.Dishes
            .Include(d => d.DishProducts)
            .FirstOrDefaultAsync(d => d.Id == dish.Id);
        if (record is null)
        {
            throw ServiceException.NotFound($"Dish with ID '{dish.Id}' not found.");
        }

        record.Name = dish.Name;
        record.Price = dish.Price;

        foreach (var line in dish.Recipe)
        {
            var existing = record.DishProducts.FirstOrDefault(l => l.ProductId == line.ProductId);
            if (existing is null)
            {
                record.DishProducts.Add(line.ToRecord());
            }
            else
            {
                existing.Quantity = line.Quantity;
            }
        }

        foreach (var stale in record.DishProducts
            .Where(l => dish.Recipe.All(nl => nl.ProductId != l.ProductId))
            .ToList())
        {
            db.Remove(stale);
        }

        await db.SaveChangesAsync();
    }

    public async Task<bool> RemoveAsync(int id)
    {
        var record = await db.Dishes
            .Include(d => d.DishProducts)
            .FirstOrDefaultAsync(d => d.Id == id);
        if (record is null)
        {
            return false;
        }

        // Recipe lines are soft-deleted together with the dish by the
        // SaveChangesAsync interceptor.
        foreach (var line in record.DishProducts.ToList())
        {
            db.DishProducts.Remove(line);
        }

        db.Dishes.Remove(record);
        await db.SaveChangesAsync();
        return true;
    }
}
