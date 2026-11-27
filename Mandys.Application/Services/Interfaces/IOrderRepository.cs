using Mandys.Domain;

namespace Mandys.Application.Services.Interfaces;

public interface IOrderRepository
{
    Task<Order> CreateWithStockDeductionAsync(
        Order order,
        IReadOnlyList<BranchStorage> updatedStorages,
        IReadOnlyList<BranchStorageHistoryProduct> historyLines);

    Task<Order?> GetByIdAsync(int id);

    Task<Order?> GetByBranchAndIdAsync(int branchId, int id);

    Task<(int TotalCount, IReadOnlyList<Order> Items)> SearchAsync(
        int? branchId,
        int? createdByUserId,
        string? paymentMethod,
        int page,
        int pageSize,
        string? orderBy);
}
