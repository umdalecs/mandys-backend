using Mandys.Application.DTOs;
using Mandys.Application.Services.Interfaces;
using Mandys.Domain;

namespace Mandys.Application.Services.Implementations;

public class DrawerService(
    IDrawerTransactionRepository drawerTransactions,
    IBranchRepository branches,
    IUserRepository users) : IDrawerService
{
    public async Task<DrawerBalanceResponse> GetBalanceAsync(
        int branchId,
        int userId,
        int requesterId,
        string requesterRole,
        int? requesterBranchId)
    {
        EnsureDrawerAccess(branchId, userId, requesterId, requesterRole, requesterBranchId);

        var branch = await branches.GetByIdAsync(branchId);
        if (branch is null)
        {
            throw ServiceException.NotFound($"No se encontró la sucursal con ID '{branchId}'.");
        }

        if (branch.WarehouseOnly)
        {
            throw ServiceException.BadRequest(
                $"La sucursal '{branch.Name}' es solo de almacén y no tiene cajas registradoras.");
        }

        var user = await users.GetByIdAsync(userId);
        if (user is null)
        {
            throw ServiceException.NotFound($"No se encontró el usuario con ID '{userId}'.");
        }

        var (currentBalance, transactionCount, lastTransactionAt) =
            await drawerTransactions.GetBalanceAsync(branchId, userId);

        return new DrawerBalanceResponse(
            branch.Id,
            branch.Name,
            user.ID,
            DrawerTransactionDtoMapper.FormatFullName(user),
            currentBalance,
            transactionCount,
            lastTransactionAt);
    }

    public async Task<IReadOnlyList<DrawerResponse>> GetBranchDrawersAsync(
        int branchId,
        string requesterRole,
        int? requesterBranchId)
    {
        EnsureBranchAccess(branchId, requesterRole, requesterBranchId);

        var branch = await branches.GetByIdAsync(branchId);
        if (branch is null)
        {
            throw ServiceException.NotFound($"No se encontró la sucursal con ID '{branchId}'.");
        }

        if (branch.WarehouseOnly)
        {
            throw ServiceException.BadRequest(
                $"La sucursal '{branch.Name}' es solo de almacén y no tiene cajas registradoras.");
        }

        var cashiers = await users.GetByBranchAndRoleAsync(branchId, Roles.Cashier);
        return cashiers
            .Select(u => new DrawerResponse(u.ID, DrawerTransactionDtoMapper.FormatFullName(u)))
            .ToList();
    }

    public async Task<IReadOnlyList<DrawerBalanceResponse>> GetBranchBalancesAsync(
        int branchId,
        string requesterRole,
        int? requesterBranchId)
    {
        EnsureBranchAccess(branchId, requesterRole, requesterBranchId);

        var branch = await branches.GetByIdAsync(branchId);
        if (branch is null)
        {
            throw ServiceException.NotFound($"No se encontró la sucursal con ID '{branchId}'.");
        }

        if (branch.WarehouseOnly)
        {
            throw ServiceException.BadRequest(
                $"La sucursal '{branch.Name}' es solo de almacén y no tiene cajas registradoras.");
        }

        var summaries = await drawerTransactions.GetBranchBalancesAsync(branchId);
        return summaries
            .Select(s => new DrawerBalanceResponse(
                s.BranchId,
                s.BranchName,
                s.UserId,
                s.UserFullName,
                s.CurrentBalance,
                s.TransactionCount,
                s.LastTransactionAt))
            .ToList();
    }

    public async Task<PagedDrawerTransactionsResponse> GetTransactionsAsync(
        int? branchId,
        int? userId,
        string? type,
        int requesterId,
        string requesterRole,
        int? requesterBranchId,
        int page,
        int pageSize,
        string? orderBy)
    {
        if (!string.IsNullOrWhiteSpace(type) && !DrawerTransactionTypes.IsValid(type))
        {
            throw ServiceException.BadRequest(
                $"El tipo de transacción de caja '{type}' no es válido. Valores permitidos: {string.Join(", ", DrawerTransactionTypes.All)}.");
        }

        var (scopedBranchId, scopedUserId) = ResolveQueryScope(
            branchId, userId, requesterId, requesterRole, requesterBranchId);

        if (scopedBranchId.HasValue)
        {
            var branch = await branches.GetByIdAsync(scopedBranchId.Value);
            if (branch is null)
            {
                throw ServiceException.NotFound($"No se encontró la sucursal con ID '{scopedBranchId.Value}'.");
            }

            if (branch.WarehouseOnly)
            {
                throw ServiceException.BadRequest(
                    $"La sucursal '{branch.Name}' es solo de almacén y no tiene cajas registradoras.");
            }
        }

        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        var (totalCount, items) = await drawerTransactions.SearchAsync(
            scopedBranchId,
            scopedUserId,
            type,
            page,
            pageSize,
            orderBy);

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return new PagedDrawerTransactionsResponse(
            totalCount,
            page,
            pageSize,
            totalPages,
            items.Select(dt => dt.ToResponse()).ToList());
    }

    private static void EnsureDrawerAccess(
        int branchId,
        int userId,
        int requesterId,
        string requesterRole,
        int? requesterBranchId)
    {
        if (string.Equals(requesterRole, Roles.Administrator, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(requesterRole, Roles.OpChief, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        if (string.Equals(requesterRole, Roles.BranchChief, StringComparison.OrdinalIgnoreCase))
        {
            if (!requesterBranchId.HasValue || requesterBranchId.Value != branchId)
            {
                throw ServiceException.BadRequest("Solo puedes consultar cajas de tu propia sucursal.");
            }

            return;
        }

        if (string.Equals(requesterRole, Roles.Cashier, StringComparison.OrdinalIgnoreCase))
        {
            if (!requesterBranchId.HasValue || requesterBranchId.Value != branchId || requesterId != userId)
            {
                throw ServiceException.BadRequest("Solo puedes consultar tu propia caja.");
            }

            return;
        }

        throw ServiceException.BadRequest("No tienes permisos para consultar cajas.");
    }

    private static void EnsureBranchAccess(
        int branchId,
        string requesterRole,
        int? requesterBranchId)
    {
        if (string.Equals(requesterRole, Roles.Administrator, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(requesterRole, Roles.OpChief, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        if (string.Equals(requesterRole, Roles.BranchChief, StringComparison.OrdinalIgnoreCase))
        {
            if (!requesterBranchId.HasValue || requesterBranchId.Value != branchId)
            {
                throw ServiceException.BadRequest("Solo puedes consultar cajas de tu propia sucursal.");
            }

            return;
        }

        throw ServiceException.BadRequest("No tienes permisos para consultar las cajas de la sucursal.");
    }

    private static (int? BranchId, int? UserId) ResolveQueryScope(
        int? requestedBranchId,
        int? requestedUserId,
        int requesterId,
        string requesterRole,
        int? requesterBranchId)
    {
        if (string.Equals(requesterRole, Roles.Administrator, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(requesterRole, Roles.OpChief, StringComparison.OrdinalIgnoreCase))
        {
            return (requestedBranchId, requestedUserId);
        }

        if (string.Equals(requesterRole, Roles.BranchChief, StringComparison.OrdinalIgnoreCase))
        {
            if (!requesterBranchId.HasValue)
            {
                throw ServiceException.BadRequest("El usuario actual no tiene una sucursal asignada.");
            }

            if (requestedBranchId.HasValue && requestedBranchId.Value != requesterBranchId.Value)
            {
                throw ServiceException.BadRequest("Solo puedes consultar transacciones de caja de tu propia sucursal.");
            }

            return (requesterBranchId.Value, requestedUserId);
        }

        if (string.Equals(requesterRole, Roles.Cashier, StringComparison.OrdinalIgnoreCase))
        {
            if (!requesterBranchId.HasValue)
            {
                throw ServiceException.BadRequest("El usuario actual no tiene una sucursal asignada.");
            }

            if (requestedBranchId.HasValue && requestedBranchId.Value != requesterBranchId.Value)
            {
                throw ServiceException.BadRequest("Solo puedes consultar transacciones de tu propia sucursal.");
            }

            if (requestedUserId.HasValue && requestedUserId.Value != requesterId)
            {
                throw ServiceException.BadRequest("Solo puedes consultar transacciones de tu propia caja.");
            }

            return (requesterBranchId.Value, requesterId);
        }

        throw ServiceException.BadRequest("No tienes permisos para consultar transacciones de caja.");
    }
}
