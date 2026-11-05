using Mandys.Application.DTOs;

namespace Mandys.Application.Services.Interfaces;

public interface IBranchStorageService
{
    Task<PagedBranchStorageResponse> GetBranchStorageAsync(
        int branchId, int page, int pageSize, string? search, string? orderBy, bool? isSupply);
    Task<BranchStorageResponse> GetBranchStorageByProductAsync(int branchId, int productId);
    Task<BranchStorageResponse> UpdateMinimumStockAsync(
        int branchId, int productId, UpdateBranchStorageRequest request);
    Task<BranchStorageHistoryResponse> CreateInMovementAsync(
        int branchId, CreateStorageInRequest request);
    Task<BranchStorageHistoryResponse> CreateOutMovementAsync(
        int branchId, CreateStorageOutRequest request);
    Task<PagedBranchStorageHistoryResponse> GetBranchStorageHistoryAsync(
        int branchId, int page, int pageSize, string? search, string? type, string? orderBy);
    Task<BranchStorageHistoryResponse> GetBranchStorageHistoryByIdAsync(
        int branchId, int historyId);
}
