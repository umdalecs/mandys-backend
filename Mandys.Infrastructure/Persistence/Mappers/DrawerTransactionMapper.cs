using Mandys.Domain;
using Mandys.Infrastructure.Persistence.Records;

namespace Mandys.Infrastructure.Persistence.Mappers;

internal static class DrawerTransactionMapper
{
    internal static DrawerTransaction ToDomain(this DrawerTransactionRecord record) =>
        new(
            record.Id,
            record.Branch.ToDomain(),
            record.User.ToDomain(),
            record.Type,
            record.Amount,
            record.AmountPaid,
            record.ChangeAmount,
            record.OrderId,
            record.Notes,
            record.CreatedAt,
            record.UpdatedAt);

    internal static DrawerTransactionRecord ToRecord(this DrawerTransaction transaction) =>
        new()
        {
            BranchId = transaction.Branch.Id,
            UserId = transaction.User.ID,
            OrderId = transaction.OrderId,
            Type = transaction.Type,
            Amount = transaction.Amount,
            AmountPaid = transaction.AmountPaid,
            ChangeAmount = transaction.ChangeAmount,
            Notes = transaction.Notes,
        };
}
