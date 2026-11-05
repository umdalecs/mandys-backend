using Mandys.Application.DTOs;

namespace Mandys.Application.Services.Interfaces;

public interface IBranchStorageService
{
    Task<PagedBranchStorageResponse> GetBranchStorageAsync(
        int branchId, int page, int pageSize, string? search, string? orderBy, bool? isSupply);
    Task<BranchStorageResponse> GetBranchStorageByProductAsync(int branchId, int productId);
    Task<BranchStorageResponse> UpdateMinimumStockAsync(
        int branchId, int productId, UpdateBranchStorageRequest request);
}
