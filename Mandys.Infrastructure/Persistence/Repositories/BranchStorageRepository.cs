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

    public async Task<IReadOnlyList<BranchStorage>> GetByBranchAndProductsAsync(
        int branchId, IReadOnlyCollection<int> productIds)
    {
        var records = await db.BranchStorages
            .AsNoTracking()
            .Include(bs => bs.Branch)
            .Include(bs => bs.Product)
            .Where(bs => bs.BranchId == branchId && productIds.Contains(bs.ProductId))
            .ToListAsync();

        return records.Select(r => r.ToDomain()).ToList();
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

    public async Task<bool> ExistsHistoryByFolioAsync(int branchId, string folio)
    {
        var normalized = folio.Trim().ToLower();
        return await db.BranchStorageHistories
            .AsNoTracking()
            .AnyAsync(bsh =>
                bsh.BranchId == branchId &&
                bsh.Folio != null &&
                bsh.Folio.ToLower() == normalized);
    }

    public async Task<BranchStorageHistory> ApplyMovementAsync(
        BranchStorageHistory history, IReadOnlyList<BranchStorage> updatedStorages)
    {
        await using var transaction = await db.Database.BeginTransactionAsync();

        var storageIds = updatedStorages.Select(s => s.Id).ToList();
        var trackedStorages = await db.BranchStorages
            .Where(bs => storageIds.Contains(bs.Id))
            .ToDictionaryAsync(bs => bs.Id);

        foreach (var storage in updatedStorages)
        {
            if (!trackedStorages.TryGetValue(storage.Id, out var tracked))
            {
                throw ServiceException.NotFound(
                    $"No se encontró el registro de almacenamiento para el producto '{storage.Product.Id}' en la sucursal '{storage.Branch.Id}'.");
            }

            if (history.Type == StorageMovementTypes.Out && tracked.CurrentStock < storage.CurrentStock)
            {
                throw ServiceException.Conflict(
                    $"El stock del producto '{storage.Product.Description}' cambió durante la operación.");
            }

            tracked.MinimumStock = storage.MinimumStock;
            tracked.CurrentStock = storage.CurrentStock;
        }

        var historyRecord = history.ToRecord();
        db.BranchStorageHistories.Add(historyRecord);

        // Entries may update the product's purchase cost (the service mutates
        // the domain Product via UpdateCostPrice). Persist whatever cost the
        // domain product now carries; when the movement didn't change it, the
        // value is identical and EF won't issue an UPDATE.
        if (history.Type == StorageMovementTypes.In)
        {
            var productIds = history.Products
                .Select(p => p.Product.Id)
                .Distinct()
                .ToList();
            var trackedProducts = await db.Products
                .Where(p => productIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id);

            foreach (var line in history.Products)
            {
                if (trackedProducts.TryGetValue(line.Product.Id, out var productRecord))
                {
                    productRecord.CostPrice = line.Product.CostPrice;
                }
            }
        }

        await db.SaveChangesAsync();
        await transaction.CommitAsync();

        return new BranchStorageHistory(
            historyRecord.Id,
            history.Branch,
            history.Type,
            history.Folio,
            history.SupplierName,
            history.Reason,
            history.Products,
            historyRecord.CreatedAt,
            historyRecord.UpdatedAt);
    }

    public async Task<BranchStorageHistory?> GetHistoryByIdAsync(int branchId, int historyId)
    {
        var record = await db.BranchStorageHistories
            .AsNoTracking()
            .Include(bsh => bsh.Branch)
            .Include(bsh => bsh.Products.OrderBy(l => l.Id))
            .ThenInclude(l => l.Product)
            .FirstOrDefaultAsync(bsh => bsh.BranchId == branchId && bsh.Id == historyId);

        return record?.ToDomain();
    }

    public async Task<(int TotalCount, IReadOnlyList<BranchStorageHistory> Items)> SearchHistoryByBranchAsync(
        int branchId, string? search, string? type, int page, int pageSize, string? orderBy)
    {
        IQueryable<BranchStorageHistoryRecord> query = db.BranchStorageHistories
            .AsNoTracking()
            .Include(bsh => bsh.Branch)
            .Include(bsh => bsh.Products.OrderBy(l => l.Id))
            .ThenInclude(l => l.Product)
            .Where(bsh => bsh.BranchId == branchId);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(bsh =>
                (bsh.Folio != null && bsh.Folio.ToLower().Contains(term)) ||
                (bsh.SupplierName != null && bsh.SupplierName.ToLower().Contains(term)) ||
                (bsh.Reason != null && bsh.Reason.ToLower().Contains(term)) ||
                bsh.Products.Any(l => l.Product.Description.ToLower().Contains(term)));
        }

        if (!string.IsNullOrWhiteSpace(type) && StorageMovementTypes.IsValid(type))
        {
            var normalizedType = StorageMovementTypes.Normalize(type);
            query = query.Where(bsh => bsh.Type == normalizedType);
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .ApplyOrdering(
                OrderByParser.Parse<BranchStorageHistorySortField>(orderBy)
                    .Select(k => (k.Field.Selector(), k.Descending))
                    .ToList(),
                bsh => bsh.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (totalCount, items.Select(r => r.ToDomain()).ToList());
    }
}
