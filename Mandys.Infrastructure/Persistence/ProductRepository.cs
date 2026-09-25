using Mandys.Domain;
using Mandys.Services;
using Microsoft.EntityFrameworkCore;

namespace Mandys.Infrastructure.Persistence;

public class ProductRepository(ApplicationDbContext db) : IProductRepository
{
    public async Task<Product?> GetByIdAsync(int id)
    {
        var record = await db.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        return record?.ToDomain();
    }

    public async Task<(int TotalCount, IReadOnlyList<Product> Items)> SearchAsync(
        string? search, int page, int pageSize)
    {
        var query = db.Products.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(p =>
                p.Description.ToLower().Contains(term) ||
                p.MeasureUnit.ToLower().Contains(term));
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (totalCount, items.Select(r => r.ToDomain()).ToList());
    }

    public async Task<Product> AddAsync(Product product)
    {
        var record = product.ToRecord();
        db.Products.Add(record);
        await db.SaveChangesAsync();
        return record.ToDomain();
    }

    public async Task UpdateAsync(Product product)
    {
        // Update the tracked record so the identity key and audit columns
        // are preserved.
        var record = await db.Products.FirstOrDefaultAsync(p => p.Id == product.Id);
        if (record is null)
        {
            throw ServiceException.NotFound($"Product with ID '{product.Id}' not found.");
        }

        record.Description = product.Description;
        record.IsSupply = product.IsSupply;
        record.Price = product.Price;
        record.MeasureUnit = product.MeasureUnit;

        await db.SaveChangesAsync();
    }

    public async Task<bool> RemoveAsync(int id)
    {
        var record = await db.Products.FindAsync(id);
        if (record is null)
        {
            return false;
        }

        // Soft-deleted by the SaveChangesAsync interceptor.
        db.Products.Remove(record);
        await db.SaveChangesAsync();
        return true;
    }
}
