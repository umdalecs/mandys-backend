using Mandys.Application.Services;
using Mandys.Application.Services.Interfaces;
using Mandys.Domain;
using Mandys.Infrastructure.Persistence.Mappers;
using Mandys.Infrastructure.Persistence.Records;
using Microsoft.EntityFrameworkCore;

namespace Mandys.Infrastructure.Persistence.Repositories;

public class BranchStorageRepository(ApplicationDbContext db) : IBranchStorageRepository
{
    public async Task<BranchStorage?> GetByBranchAndProductAsync(int branchId, int productId)
    {
        var record = await db.BranchStorages
            .AsNoTracking()
            .Include(bs => bs.Branch)
            .Include(bs => bs.Product)
            .FirstOrDefaultAsync(bs => bs.BranchId == branchId && bs.ProductId == productId);

        return record?.ToDomain();
    }

    public async Task<(int TotalCount, IReadOnlyList<BranchStorage> Items)> SearchByBranchAsync(
        int branchId, string? search, int page, int pageSize, string? orderBy, bool? isSupply)
    {
        IQueryable<BranchStorageRecord> query = db.BranchStorages
            .AsNoTracking()
            .Include(bs => bs.Branch)
            .Include(bs => bs.Product)
            .Where(bs => bs.BranchId == branchId);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(bs => bs.Product.Description.ToLower().Contains(term));
        }

        if (isSupply.HasValue)
        {
            query = query.Where(bs => bs.Product.IsSupply == isSupply.Value);
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .ApplyOrdering(
                OrderByParser.Parse<BranchStorageSortField>(orderBy)
                    .Select(k => (k.Field.Selector(), k.Descending))
                    .ToList(),
                bs => bs.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (totalCount, items.Select(r => r.ToDomain()).ToList());
    }

    public async Task UpdateAsync(BranchStorage storage)
    {
        var record = await db.BranchStorages
            .FirstOrDefaultAsync(bs => bs.Id == storage.Id);

        if (record is null)
        {
            throw ServiceException.NotFound(
                $"No se encontró el registro de almacenamiento para la sucursal '{storage.Branch.Id}' y producto '{storage.Product.Id}'.");
        }

        record.MinimumStock = storage.MinimumStock;
        record.CurrentStock = storage.CurrentStock;

        await db.SaveChangesAsync();
    }

    public async Task InitializeForProductAsync(int productId)
    {
        var branchIds = await db.Branches
            .AsNoTracking()
            .Select(b => b.Id)
            .ToListAsync();

        if (branchIds.Count == 0)
        {
            return;
        }

        var existingBranchIds = await db.BranchStorages
            .AsNoTracking()
            .Where(bs => bs.ProductId == productId)
            .Select(bs => bs.BranchId)
            .ToHashSetAsync();

        var newRecords = branchIds
            .Where(branchId => !existingBranchIds.Contains(branchId))
            .Select(branchId => new BranchStorageRecord
            {
                BranchId = branchId,
                ProductId = productId,
                MinimumStock = 0m,
                CurrentStock = 0m,
            })
            .ToList();

        if (newRecords.Count > 0)
        {
            db.BranchStorages.AddRange(newRecords);
            await db.SaveChangesAsync();
        }
    }

    public async Task InitializeForBranchAsync(int branchId)
    {
        var productIds = await db.Products
            .AsNoTracking()
            .Select(p => p.Id)
            .ToListAsync();

        if (productIds.Count == 0)
        {
            return;
        }

        var existingProductIds = await db.BranchStorages
            .AsNoTracking()
            .Where(bs => bs.BranchId == branchId)
            .Select(bs => bs.ProductId)
            .ToHashSetAsync();

        var newRecords = productIds
            .Where(productId => !existingProductIds.Contains(productId))
            .Select(productId => new BranchStorageRecord
            {
                BranchId = branchId,
                ProductId = productId,
                MinimumStock = 0m,
                CurrentStock = 0m,
            })
            .ToList();

        if (newRecords.Count > 0)
        {
            db.BranchStorages.AddRange(newRecords);
            await db.SaveChangesAsync();
        }
    }

    public Task<bool> HasStockForProductAsync(int productId) =>
        db.BranchStorages
            .AsNoTracking()
            .AnyAsync(bs => bs.ProductId == productId && bs.CurrentStock >= 1m);

    public async Task RemoveByProductAsync(int productId)
    {
        var records = await db.BranchStorages
            .Where(bs => bs.ProductId == productId)
            .ToListAsync();

        if (records.Count == 0)
        {
            return;
        }

        db.BranchStorages.RemoveRange(records);
        await db.SaveChangesAsync();
    }
}
