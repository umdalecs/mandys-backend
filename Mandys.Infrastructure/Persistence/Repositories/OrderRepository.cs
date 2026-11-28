using Mandys.Application.Services;
using Mandys.Application.Services.Interfaces;
using Mandys.Domain;
using Mandys.Infrastructure.Persistence.Mappers;
using Mandys.Infrastructure.Persistence.Records;
using Microsoft.EntityFrameworkCore;

namespace Mandys.Infrastructure.Persistence.Repositories;

public class OrderRepository(ApplicationDbContext db) : IOrderRepository
{
    public async Task<Order> CreateWithStockDeductionAsync(
        Order order,
        IReadOnlyList<BranchStorage> updatedStorages,
        IReadOnlyList<BranchStorageHistoryProduct> historyLines)
    {
        await using var transaction = await db.Database.BeginTransactionAsync();

        var storageIds = updatedStorages.Select(s => s.Id).ToList();
        var trackedStorages = await db.BranchStorages
            .Where(bs => storageIds.Contains(bs.Id))
            .ToDictionaryAsync(bs => bs.Id);

        foreach (var storage in updatedStorages)
        {
            if (!trackedStorages.TryGetValue(storage.Id, out var tracked))
            {
                throw ServiceException.NotFound(
                    $"No se encontró el registro de almacenamiento para el producto '{storage.Product.Id}' en la sucursal '{storage.Branch.Id}'.");
            }

            // Ensure no concurrent sale/out-movement dropped stock below our new expected stock.
            if (tracked.CurrentStock < storage.CurrentStock)
            {
                throw ServiceException.Conflict(
                    $"El stock del producto '{storage.Product.Description}' cambió durante la operación.");
            }

            tracked.CurrentStock = storage.CurrentStock;
        }

        var orderRecord = order.ToRecord();
        db.Orders.Add(orderRecord);
        await db.SaveChangesAsync();

        if (historyLines.Count > 0)
        {
            var history = new BranchStorageHistory(
                0,
                order.Branch,
                StorageMovementTypes.Out,
                null,
                null,
                $"Venta - Pedido #{orderRecord.Id}",
                historyLines);

            db.BranchStorageHistories.Add(history.ToRecord());
            await db.SaveChangesAsync();
        }

        if (order.PaymentMethod == PaymentMethods.Cash)
        {
            var drawerTransaction = new DrawerTransaction(
                0,
                order.Branch,
                order.CreatedByUser,
                DrawerTransactionTypes.Sale,
                order.TotalAmount,
                order.AmountPaid,
                order.ChangeAmount,
                orderRecord.Id,
                $"Venta - Pedido #{orderRecord.Id}");

            db.DrawerTransactions.Add(drawerTransaction.ToRecord());
            await db.SaveChangesAsync();
        }

        if (order.Customer is not null && (order.PointsSpent > 0 || order.PointsEarned > 0))
        {
            var trackedCustomer = await db.Users.FirstOrDefaultAsync(u => u.Id == order.Customer.ID);
            if (trackedCustomer is null)
            {
                throw ServiceException.NotFound($"No se encontró el cliente con ID '{order.Customer.ID}'.");
            }

            if (order.PointsSpent > 0 && trackedCustomer.Points < order.PointsSpent)
            {
                throw ServiceException.Conflict(
                    $"El saldo de puntos del cliente cambió durante la operación. Disponible: {trackedCustomer.Points}, requerido: {order.PointsSpent}.");
            }

            trackedCustomer.Points = Math.Round(
                trackedCustomer.Points - order.PointsSpent + order.PointsEarned,
                2,
                MidpointRounding.AwayFromZero);

            await db.SaveChangesAsync();
        }

        await transaction.CommitAsync();

        return new Order(
            orderRecord.Id,
            order.Branch,
            order.CreatedByUser,
            order.PaymentMethod,
            order.CardLast4,
            order.Products,
            order.Dishes,
            order.Combos,
            orderRecord.CreatedAt,
            orderRecord.UpdatedAt,
            order.Customer,
            order.PointsEarned,
            order.PointsSpent,
            order.Status,
            order.CompletedAt,
            order.AmountPaid,
            order.ChangeAmount);
    }

    public async Task<Order?> GetByIdAsync(int id)
    {
        var record = await BuildDetailQuery()
            .FirstOrDefaultAsync(o => o.Id == id);

        return record?.ToDomain();
    }

    public async Task<Order?> GetByBranchAndIdAsync(int branchId, int id)
    {
        var record = await BuildDetailQuery()
            .FirstOrDefaultAsync(o => o.BranchId == branchId && o.Id == id);

        return record?.ToDomain();
    }

    public async Task<(int TotalCount, IReadOnlyList<Order> Items)> SearchAsync(
        int? branchId,
        int? createdByUserId,
        string? paymentMethod,
        string? status,
        int page,
        int pageSize,
        string? orderBy,
        DateTime? createdAtMax = null)
    {
        var query = BuildDetailQuery();

        if (branchId.HasValue)
        {
            query = query.Where(o => o.BranchId == branchId.Value);
        }

        if (createdAtMax.HasValue)
        {
            query = query.Where(o => o.CreatedAt <= createdAtMax.Value);
        }

        if (createdByUserId.HasValue)
        {
            query = query.Where(o =>
                o.CreatedByUserId == createdByUserId.Value ||
                o.CustomerId == createdByUserId.Value);
        }

        if (!string.IsNullOrWhiteSpace(paymentMethod) && PaymentMethods.IsValid(paymentMethod))
        {
            var normalizedMethod = PaymentMethods.Normalize(paymentMethod);
            query = query.Where(o => o.PaymentMethod == normalizedMethod);
        }

        if (!string.IsNullOrWhiteSpace(status) && OrderStatuses.IsValid(status))
        {
            var normalizedStatus = OrderStatuses.Normalize(status);
            query = query.Where(o => o.Status == normalizedStatus);
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .ApplyOrdering(
                OrderByParser.Parse<OrderSortField>(orderBy)
                    .Select(k => (k.Field.Selector(), k.Descending))
                    .ToList(),
                o => o.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (totalCount, items.Select(r => r.ToDomain()).ToList());
    }

    public async Task UpdateAsync(Order order)
    {
        var record = await db.Orders.FirstOrDefaultAsync(o => o.Id == order.Id);
        if (record is null)
        {
            throw ServiceException.NotFound($"No se encontró el pedido con ID '{order.Id}'.");
        }

        record.Status = order.Status;
        record.CompletedAt = order.CompletedAt;

        await db.SaveChangesAsync();
    }

    private IQueryable<OrderRecord> BuildDetailQuery() =>
        db.Orders
            .AsNoTracking()
            .Include(o => o.Branch)
            .Include(o => o.CreatedByUser)
            .Include(o => o.Customer)
            .Include(o => o.DrawerTransaction)
            .Include(o => o.OrderProducts.OrderBy(l => l.Id))
            .ThenInclude(l => l.Product)
            .Include(o => o.OrderDishes.OrderBy(l => l.Id))
            .ThenInclude(l => l.Dish)
            .Include(o => o.OrderCombos.OrderBy(l => l.Id))
            .ThenInclude(l => l.Combo);
}
