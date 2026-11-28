using Mandys.Domain;

namespace Mandys.Application.Services.Interfaces;

public interface IDrawerReconciliationRepository
{
    Task<DrawerReconciliation?> GetByIdAsync(int id);

    Task<DrawerReconciliation> AddAsync(DrawerReconciliation reconciliation);

    Task<DrawerReconciliation> ReconcileWithAdjustmentAsync(
        DrawerReconciliation reconciliation,
        DrawerTransaction adjustmentTransaction);
}
