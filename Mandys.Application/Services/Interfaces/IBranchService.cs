using Mandys.Application.DTOs;

namespace Mandys.Application.Services.Interfaces;

public interface IBranchService
{
    Task<PagedBranchesResponse> GetBranchesAsync(
        int page,
        int pageSize,
        string? search,
        string? orderBy,
        bool? warehouseOnly = null,
        bool includeWarehouseOnlyField = false);

    Task<BranchResponse> GetBranchByIdAsync(int id, bool includeWarehouseOnly = false);

    Task<BranchResponse> CreateBranchAsync(CreateBranchRequest request);

    Task<BranchResponse> UpdateBranchAsync(int id, UpdateBranchRequest request);

    Task DeleteBranchAsync(int id);
}
