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
