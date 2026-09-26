using Mandys.Domain;

namespace Mandys.DTOs;

public record BranchResponse(
    int Id,
    string Name,
    string Address,
    bool WarehouseOnly
);

public record CreateBranchRequest(
    string Name,
    string Address,
    bool WarehouseOnly = false
);

public record UpdateBranchRequest(
    string? Name = null,
    string? Address = null,
    bool? WarehouseOnly = null
);

public record PagedBranchesResponse(
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages,
    IReadOnlyList<BranchResponse> Items
);

public static class BranchMapper
{
    public static BranchResponse ToResponse(this Branch branch) =>
        new(
            branch.Id,
            branch.Name,
            branch.Address,
            branch.WarehouseOnly
        );
}
