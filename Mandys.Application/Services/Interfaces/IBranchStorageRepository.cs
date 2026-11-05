using Mandys.Domain;

namespace Mandys.Application.Services.Interfaces;

public interface IBranchStorageRepository
{
    Task<BranchStorage?> GetByBranchAndProductAsync(int branchId, int productId);
    Task<(int TotalCount, IReadOnlyList<BranchStorage> Items)> SearchByBranchAsync(
        int branchId, string? search, int page, int pageSize, string? orderBy, bool? isSupply);
    Task UpdateAsync(BranchStorage storage);
    Task InitializeForProductAsync(int productId);
    Task InitializeForBranchAsync(int branchId);
    Task<bool> HasStockForProductAsync(int productId);
    Task RemoveByProductAsync(int productId);
}
