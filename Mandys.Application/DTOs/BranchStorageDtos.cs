using Mandys.Domain;

namespace Mandys.Application.DTOs;

public record BranchStorageResponse(
    int Id,
    int BranchId,
    ProductResponse Product,
    decimal MinimumStock,
    decimal CurrentStock,
    DateTime LastUpdated
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
    decimal Quantity,
    decimal CostPrice
);

public record BranchStorageHistoryResponse(
    int Id,
    int BranchId,
    string Type,
    string? Folio,
    string? SupplierName,
    string? Reason,
    IReadOnlyList<BranchStorageHistoryProductLineResponse> Products,
    DateTime Date
);

public record CreateStorageInProductLineRequest(
    int ProductId,
    decimal Quantity,
    decimal? CostPrice = null
);

public record CreateStorageOutProductLineRequest(
    int ProductId,
    decimal Quantity
);

public record CreateStorageInRequest(
    string Folio,
    string SupplierName,
    IReadOnlyList<CreateStorageInProductLineRequest> Products
);

public record CreateStorageOutRequest(
    string Reason,
    IReadOnlyList<CreateStorageOutProductLineRequest> Products
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
            storage.Branch.Id,
            storage.Product.ToResponse(),
            storage.MinimumStock,
            storage.CurrentStock,
            storage.UpdatedAt
        );

    public static BranchStorageHistoryResponse ToResponse(this BranchStorageHistory history) =>
        new(
            history.Id,
            history.Branch.Id,
            history.Type,
            history.Folio,
            history.SupplierName,
            history.Reason,
            history.Products
                .Select(l => new BranchStorageHistoryProductLineResponse(
                    l.Product.ToResponse(),
                    l.Quantity,
                    l.CostPrice))
                .ToList(),
            history.CreatedAt
        );
}
