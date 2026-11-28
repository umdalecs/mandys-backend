using Mandys.Domain;
using Mandys.Infrastructure.Persistence.Records;

namespace Mandys.Infrastructure.Persistence.Mappers;

internal static class DrawerReconciliationMapper
{
    internal static DrawerReconciliation ToDomain(this DrawerReconciliationRecord record) =>
        new(
            record.Id,
            record.Branch.ToDomain(),
            record.User.ToDomain(),
            record.PerformedByUserId,
            record.ExpectedAmount,
            record.CountedAmount,
            record.Status,
            record.Reason,
            record.ResolvedByUserId,
            record.ResolvedAt,
            record.AdjustmentTransactionId,
            record.CreatedAt,
            record.UpdatedAt);

    internal static DrawerReconciliationRecord ToRecord(this DrawerReconciliation reconciliation) =>
        new()
        {
            BranchId = reconciliation.Branch.Id,
            UserId = reconciliation.User.ID,
            PerformedByUserId = reconciliation.PerformedByUserId,
            ExpectedAmount = reconciliation.ExpectedAmount,
            CountedAmount = reconciliation.CountedAmount,
            Discrepancy = reconciliation.Discrepancy,
            HasDiscrepancy = reconciliation.HasDiscrepancy,
            Status = reconciliation.Status,
            Reason = reconciliation.Reason,
            ResolvedByUserId = reconciliation.ResolvedByUserId,
            ResolvedAt = reconciliation.ResolvedAt,
            AdjustmentTransactionId = reconciliation.AdjustmentTransactionId,
        };
}
