using Mandys.Domain;

namespace Mandys.Application.DTOs;

public record BranchStorageResponse(
    int Id,
    int BranchId,
    ProductResponse Product,
    decimal MinimumStock,
    decimal CurrentStock
);

public record UpdateBranchStorageRequest(
    decimal MinimumStock
);

public record PagedBranchStorageResponse(
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages,
    IReadOnlyList<BranchStorageResponse> Items
);

public static class BranchStorageDtoMapper
{
    public static BranchStorageResponse ToResponse(this BranchStorage storage) =>
        new(
            storage.Id,
            storage.Branch.Id,
            storage.Product.ToResponse(),
            storage.MinimumStock,
            storage.CurrentStock
        );
}
