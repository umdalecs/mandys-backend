using Mandys.Application.DTOs;

namespace Mandys.Application.Services.Interfaces;

public interface IDrawerService
{
    Task<DrawerBalanceResponse> GetBalanceAsync(
        int branchId,
        int userId,
        int requesterId,
        string requesterRole,
        int? requesterBranchId);

    Task<IReadOnlyList<DrawerResponse>> GetBranchDrawersAsync(
        int branchId,
        string requesterRole,
        int? requesterBranchId);

    Task<IReadOnlyList<DrawerBalanceResponse>> GetBranchBalancesAsync(
        int branchId,
        string requesterRole,
        int? requesterBranchId);

    Task<PagedDrawerTransactionsResponse> GetTransactionsAsync(
        int? branchId,
        int? userId,
        string? type,
        int requesterId,
        string requesterRole,
        int? requesterBranchId,
        int page,
        int pageSize,
        string? orderBy);
}
