using Mandys.Domain;

namespace Mandys.Application.DTOs;

public record DrawerTransactionResponse(
    int Id,
    int BranchId,
    string BranchName,
    int UserId,
    string UserFullName,
    int? OrderId,
    string Type,
    decimal Amount,
    decimal? AmountPaid,
    decimal? ChangeAmount,
    string? Notes,
    DateTime CreatedAt
);

public record PagedDrawerTransactionsResponse(
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages,
    IReadOnlyList<DrawerTransactionResponse> Items
);

public record DrawerBalanceResponse(
    int BranchId,
    string BranchName,
    int UserId,
    string UserFullName,
    decimal CurrentBalance,
    int TransactionCount,
    DateTime? LastTransactionAt
);

public record DrawerResponse(
    int Id,
    string FullName
);

public record CreateDrawerReconciliationRequest(
    decimal CountedAmount
);

public record ReconcileDrawerReasonRequest(
    string Reason
);

public record DrawerReconciliationResponse(
    int Id,
    int BranchId,
    int UserId,
    decimal ExpectedAmount,
    decimal CountedAmount,
    decimal Discrepancy,
    bool HasDiscrepancy,
    string Status,
    string? Reason,
    int? AdjustmentTransactionId,
    DateTime? ResolvedAt,
    DateTime CreatedAt
);

public static class DrawerTransactionDtoMapper
{
    public static DrawerTransactionResponse ToResponse(this DrawerTransaction transaction) =>
        new(
            transaction.Id,
            transaction.Branch.Id,
            transaction.Branch.Name,
            transaction.User.ID,
            FormatFullName(transaction.User),
            transaction.OrderId,
            transaction.Type,
            transaction.Amount,
            transaction.AmountPaid,
            transaction.ChangeAmount,
            transaction.Notes,
            transaction.CreatedAt);

    public static DrawerReconciliationResponse ToResponse(this DrawerReconciliation reconciliation) =>
        new(
            reconciliation.Id,
            reconciliation.Branch.Id,
            reconciliation.User.ID,
            reconciliation.ExpectedAmount,
            reconciliation.CountedAmount,
            reconciliation.Discrepancy,
            reconciliation.HasDiscrepancy,
            reconciliation.Status,
            reconciliation.Reason,
            reconciliation.AdjustmentTransactionId,
            reconciliation.ResolvedAt,
            reconciliation.CreatedAt);

    public static string FormatFullName(User user) =>
        string.IsNullOrWhiteSpace(user.MotherLastName)
            ? $"{user.FirstName} {user.FatherLastName}"
            : $"{user.FirstName} {user.FatherLastName} {user.MotherLastName}";
}
