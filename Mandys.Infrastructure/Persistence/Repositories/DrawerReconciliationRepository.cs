using Mandys.Application.Services;
using Mandys.Application.Services.Interfaces;
using Mandys.Domain;
using Mandys.Infrastructure.Persistence.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Mandys.Infrastructure.Persistence.Repositories;

public class DrawerReconciliationRepository(ApplicationDbContext db) : IDrawerReconciliationRepository
{
    public async Task<DrawerReconciliation?> GetByIdAsync(int id)
    {
        var record = await db.DrawerReconciliations
            .AsNoTracking()
            .Include(r => r.Branch)
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Id == id);

        return record?.ToDomain();
    }

    public Task<DateTime?> GetLatestReconciliationAtByBranchAsync(int branchId) =>
        db.DrawerReconciliations
            .AsNoTracking()
            .Where(r => r.BranchId == branchId)
            .MaxAsync(r => (DateTime?)r.CreatedAt);

    public async Task<DrawerReconciliation> AddAsync(DrawerReconciliation reconciliation)
    {
        var record = reconciliation.ToRecord();
        db.DrawerReconciliations.Add(record);
        await db.SaveChangesAsync();

        var saved = await db.DrawerReconciliations
            .AsNoTracking()
            .Include(r => r.Branch)
            .Include(r => r.User)
            .FirstAsync(r => r.Id == record.Id);

        return saved.ToDomain();
    }

    public async Task<DrawerReconciliation> ReconcileWithAdjustmentAsync(
        DrawerReconciliation reconciliation,
        DrawerTransaction adjustmentTransaction)
    {
        var record = await db.DrawerReconciliations
            .Include(r => r.Branch)
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Id == reconciliation.Id);

        if (record is null)
        {
            throw ServiceException.NotFound($"No se encontró el cuadre de caja con ID '{reconciliation.Id}'.");
        }

        var adjustmentRecord = adjustmentTransaction.ToRecord();
        db.DrawerTransactions.Add(adjustmentRecord);

        record.Status = reconciliation.Status;
        record.Reason = reconciliation.Reason;
        record.ResolvedByUserId = reconciliation.ResolvedByUserId;
        record.ResolvedAt = reconciliation.ResolvedAt;
        record.AdjustmentTransaction = adjustmentRecord;

        await db.SaveChangesAsync();

        return record.ToDomain();
    }
}
