using Mandys.Domain;
using Mandys.Infrastructure.Persistence.Records;

namespace Mandys.Infrastructure.Persistence.Mappers;

/// <summary>
/// Maps between the branches table record and the domain entity.
/// </summary>
internal static class BranchMapper
{
    internal static Branch ToDomain(this BranchRecord record) =>
        new(
            record.Id,
            record.Name,
            record.Address,
            record.WarehouseOnly,
            record.CreatedAt,
            record.UpdatedAt);

    internal static BranchRecord ToRecord(this Branch branch) =>
        new()
        {
            Name = branch.Name,
            Address = branch.Address,
            WarehouseOnly = branch.WarehouseOnly,
        };
}
