using Mandys.Domain;
using Mandys.Infrastructure.Persistence.Mappers;
using Mandys.Infrastructure.Persistence.Records;
using Mandys.Services;
using Mandys.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Mandys.Infrastructure.Persistence.Repositories;

public class ComboRepository(ApplicationDbContext db) : IComboRepository
{
    public async Task<Combo?> GetByIdAsync(int id)
    {
        var record = await db.Combos
            .AsNoTracking()
            .Include(c => c.ComboDishes)
            .Include(c => c.ComboProducts)
            .FirstOrDefaultAsync(c => c.Id == id);
        return record?.ToDomain();
    }

    public async Task<(int TotalCount, IReadOnlyList<Combo> Items)> SearchAsync(
        string? search, int page, int pageSize)
    {
        IQueryable<ComboRecord> query = db.Combos
            .AsNoTracking()
            .Include(c => c.ComboDishes)
            .Include(c => c.ComboProducts);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(c => c.Name.ToLower().Contains(term));
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (totalCount, items.Select(r => r.ToDomain()).ToList());
    }

    public async Task<Combo> AddAsync(Combo combo)
    {
        var record = combo.ToRecord();
        db.Combos.Add(record);
        await db.SaveChangesAsync();
        return record.ToDomain();
    }

    public async Task UpdateAsync(Combo combo)
    {
        // Update the tracked record so the identity key and audit columns
        // are preserved. Combo lines are synced per collection: quantities
        // updated, missing lines added, absent lines removed.
        var record = await db.Combos
            .Include(c => c.ComboDishes)
            .Include(c => c.ComboProducts)
            .FirstOrDefaultAsync(c => c.Id == combo.Id);
        if (record is null)
        {
            throw ServiceException.NotFound($"Combo with ID '{combo.Id}' not found.");
        }

        record.Name = combo.Name;
        record.Price = combo.Price;

        foreach (var line in combo.Dishes)
        {
            var existing = record.ComboDishes.FirstOrDefault(l => l.DishId == line.DishId);
            if (existing is null)
            {
                record.ComboDishes.Add(line.ToRecord());
            }
            else
            {
                existing.Quantity = line.Quantity;
            }
        }

        foreach (var stale in record.ComboDishes
            .Where(l => combo.Dishes.All(nl => nl.DishId != l.DishId))
            .ToList())
        {
            db.Remove(stale);
        }

        foreach (var line in combo.Products)
        {
            var existing = record.ComboProducts.FirstOrDefault(l => l.ProductId == line.ProductId);
            if (existing is null)
            {
                record.ComboProducts.Add(line.ToRecord());
            }
            else
            {
                existing.Quantity = line.Quantity;
            }
        }

        foreach (var stale in record.ComboProducts
            .Where(l => combo.Products.All(nl => nl.ProductId != l.ProductId))
            .ToList())
        {
            db.Remove(stale);
        }

        await db.SaveChangesAsync();
    }

    public async Task<bool> RemoveAsync(int id)
    {
        var record = await db.Combos
            .Include(c => c.ComboDishes)
            .Include(c => c.ComboProducts)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (record is null)
        {
            return false;
        }

        // Combo lines are soft-deleted together with the combo by the
        // SaveChangesAsync interceptor.
        foreach (var line in record.ComboDishes.ToList())
        {
            db.ComboDishes.Remove(line);
        }

        foreach (var line in record.ComboProducts.ToList())
        {
            db.ComboProducts.Remove(line);
        }

        db.Combos.Remove(record);
        await db.SaveChangesAsync();
        return true;
    }
}
