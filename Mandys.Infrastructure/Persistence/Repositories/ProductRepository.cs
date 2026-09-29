using Mandys.Domain;
using Mandys.Infrastructure.Persistence.Mappers;
using Mandys.Infrastructure.Persistence.Records;
using Mandys.Services;
using Mandys.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Mandys.Infrastructure.Persistence.Repositories;

public class ProductRepository(ApplicationDbContext db) : IProductRepository
{
    public async Task<Product?> GetByIdAsync(int id)
    {
        var record = await db.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        return record?.ToDomain();
    }

    public async Task<(int TotalCount, IReadOnlyList<Product> Items)> SearchAsync(
        string? search, int page, int pageSize, string? orderBy, bool? isSupply)
    {
        var query = db.Products.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(p =>
                p.Description.ToLower().Contains(term) ||
                p.MeasureUnit.ToLower().Contains(term));
        }
        
        if (isSupply.HasValue)
        {
            query = query.Where(p =>
                p.IsSupply == isSupply.Value);
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .ApplyOrdering(
                OrderByParser.Parse<ProductSortField>(orderBy)
                    .Select(k => (k.Field.Selector(), k.Descending))
                    .ToList(),
                p => p.Id)
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
        var record = await db.Products.FirstOrDefaultAsync(p => p.Id == product.Id);
        if (record is null)
        {
            throw ServiceException.NotFound($"No se encontró el producto con ID '{product.Id}'.");
        }

        record.Description = product.Description;
        record.IsSupply = product.IsSupply;
        record.CostPrice = product.CostPrice;
        record.SalePrice = product.SalePrice;
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

        var dishCount = await db.DishProducts.AsNoTracking()
            .CountAsync(dp => dp.ProductId == id);
        var comboCount = await db.ComboProducts.AsNoTracking()
            .CountAsync(cp => cp.ProductId == id);

        if (dishCount > 0 || comboCount > 0)
        {
            var totalCount = dishCount + comboCount;
            throw ServiceException.Conflict(
                $"No se puede eliminar el producto '{record.Description}' porque está en uso {totalCount} vez/veces.");
        }

        db.Products.Remove(record);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsByDescriptionAsync(string description, int? excludingId = null)
    {
        var normalized = description.Trim().ToLower();
        return await db.Products
            .AsNoTracking()
            .AnyAsync(p =>
                p.Description.ToLower() == normalized &&
                (excludingId == null || p.Id != excludingId));
    }
}
