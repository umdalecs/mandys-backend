using Mandys.Domain;

namespace Mandys.Application.Services.Interfaces;

public record DrawerBalanceSummary(
    int BranchId,
    string BranchName,
    int UserId,
    string UserFullName,
    decimal CurrentBalance,
    int TransactionCount,
    DateTime? LastTransactionAt
);

public interface IDrawerTransactionRepository
{
    Task<(decimal CurrentBalance, int TransactionCount, DateTime? LastTransactionAt)> GetBalanceAsync(
        int branchId,
        int userId);

    Task<IReadOnlyList<DrawerBalanceSummary>> GetBranchBalancesAsync(int branchId);

    Task<(int TotalCount, IReadOnlyList<DrawerTransaction> Items)> SearchAsync(
        int? branchId,
        int? userId,
        string? type,
        int page,
        int pageSize,
        string? orderBy);
}
