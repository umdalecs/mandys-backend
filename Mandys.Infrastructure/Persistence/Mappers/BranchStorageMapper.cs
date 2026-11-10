using Mandys.Domain;
using Mandys.Infrastructure.Persistence.Records;

namespace Mandys.Infrastructure.Persistence.Mappers;

internal static class BranchStorageMapper
{
    internal static BranchStorage ToDomain(this BranchStorageRecord record) =>
        new(
            record.Id,
            record.Branch.ToDomain(),
            record.Product.ToDomain(),
            record.MinimumStock,
            record.CurrentStock,
            record.CreatedAt,
            record.UpdatedAt);

    internal static BranchStorageRecord ToRecord(this BranchStorage storage) =>
        new()
        {
            BranchId = storage.Branch.Id,
            ProductId = storage.Product.Id,
            MinimumStock = storage.MinimumStock,
            CurrentStock = storage.CurrentStock,
        };
}

internal static class BranchStorageHistoryMapper
{
    internal static BranchStorageHistory ToDomain(this BranchStorageHistoryRecord record) =>
        new(
            record.Id,
            record.Branch.ToDomain(),
            record.Type,
            record.Folio,
            record.SupplierName,
            record.Reason,
            record.Products.Select(l => l.ToDomain()),
            record.CreatedAt,
            record.UpdatedAt);

    internal static BranchStorageHistoryRecord ToRecord(this BranchStorageHistory history) =>
        new()
        {
            BranchId = history.Branch.Id,
            Type = history.Type,
            Folio = history.Folio,
            SupplierName = history.SupplierName,
            Reason = history.Reason,
            Products = history.Products.Select(l => l.ToRecord()).ToList(),
        };
}

internal static class BranchStorageHistoryProductMapper
{
    internal static BranchStorageHistoryProduct ToDomain(this BranchStorageHistoryProductRecord record) =>
        new(record.Product.ToDomain(), record.Quantity, record.CostPrice);

    internal static BranchStorageHistoryProductRecord ToRecord(this BranchStorageHistoryProduct line) =>
        new()
        {
            ProductId = line.Product.Id,
            Quantity = line.Quantity,
            CostPrice = line.CostPrice,
        };
}
