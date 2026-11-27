using System.Text.Json.Serialization;
using Mandys.Domain;

namespace Mandys.Application.DTOs;

public record BranchResponse(
    int Id,
    string Name,
    string Address,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    bool? WarehouseOnly = null
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
    public static BranchResponse ToResponse(this Branch branch, bool includeWarehouseOnly = true) =>
        new(
            branch.Id,
            branch.Name,
            branch.Address,
            includeWarehouseOnly ? branch.WarehouseOnly : null
        );
}
