using Mandys.Domain;

namespace Mandys.Application.DTOs;

public record BranchStorageResponse(
    int Id,
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

public record BranchStorageHistoryProductLineResponse(
    ProductResponse Product,
    decimal Quantity
);

public record BranchStorageHistoryResponse(
    int Id,
    string Type,
    string? Folio,
    IReadOnlyList<BranchStorageHistoryProductLineResponse> Products,
    DateTime CreatedAt
);

public record CreateStorageMovementProductLineRequest(
    int ProductId,
    decimal Quantity
);

public record CreateStorageInRequest(
    string Folio,
    IReadOnlyList<CreateStorageMovementProductLineRequest> Products
);

public record CreateStorageOutRequest(
    IReadOnlyList<CreateStorageMovementProductLineRequest> Products,
    string? Folio = null
);

public record PagedBranchStorageHistoryResponse(
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages,
    IReadOnlyList<BranchStorageHistoryResponse> Items
);

public static class BranchStorageDtoMapper
{
    public static BranchStorageResponse ToResponse(this BranchStorage storage) =>
        new(
            storage.Id,
            storage.Product.ToResponse(),
            storage.MinimumStock,
            storage.CurrentStock
        );

    public static BranchStorageHistoryResponse ToResponse(this BranchStorageHistory history) =>
        new(
            history.Id,
            history.Type,
            history.Folio,
            history.Products
                .Select(l => new BranchStorageHistoryProductLineResponse(l.Product.ToResponse(), l.Quantity))
                .ToList(),
            history.CreatedAt
        );
}
