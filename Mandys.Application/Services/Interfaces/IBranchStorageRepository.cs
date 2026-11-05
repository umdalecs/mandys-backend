using Mandys.Domain;

namespace Mandys.Application.Services.Interfaces;

public interface IBranchStorageRepository
{
    Task<BranchStorage?> GetByBranchAndProductAsync(int branchId, int productId);
    Task<IReadOnlyList<BranchStorage>> GetByBranchAndProductsAsync(int branchId, IReadOnlyCollection<int> productIds);
    Task<(int TotalCount, IReadOnlyList<BranchStorage> Items)> SearchByBranchAsync(
        int branchId, string? search, int page, int pageSize, string? orderBy, bool? isSupply);
    Task UpdateAsync(BranchStorage storage);
    Task InitializeForProductAsync(int productId);
    Task InitializeForBranchAsync(int branchId);
    Task<bool> HasStockForProductAsync(int productId);
    Task RemoveByProductAsync(int productId);
    Task<bool> ExistsHistoryByFolioAsync(int branchId, string folio);
    Task<BranchStorageHistory> ApplyMovementAsync(
        BranchStorageHistory history, IReadOnlyList<BranchStorage> updatedStorages);
    Task<BranchStorageHistory?> GetHistoryByIdAsync(int branchId, int historyId);
    Task<(int TotalCount, IReadOnlyList<BranchStorageHistory> Items)> SearchHistoryByBranchAsync(
        int branchId, string? search, string? type, int page, int pageSize, string? orderBy);
}
