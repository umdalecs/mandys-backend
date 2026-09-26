using Mandys.DTOs;

namespace Mandys.Services.Interfaces;

public interface IBranchService
{
    Task<PagedBranchesResponse> GetBranchesAsync(int page, int pageSize, string? search);

    Task<BranchResponse> GetBranchByIdAsync(int id);

    Task<BranchResponse> CreateBranchAsync(CreateBranchRequest request);

    Task<BranchResponse> UpdateBranchAsync(int id, UpdateBranchRequest request);

    Task DeleteBranchAsync(int id);
}
