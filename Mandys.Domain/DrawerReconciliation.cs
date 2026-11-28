namespace Mandys.Domain;

public static class DrawerReconciliationStatuses
{
    public const string Matched = "matched";
    public const string Pending = "pending";
    public const string Reconciled = "reconciled";

    public static readonly IReadOnlyList<string> All =
    [
        Matched,
        Pending,
        Reconciled,
    ];

    public static bool IsValid(string? status) =>
        !string.IsNullOrWhiteSpace(status) &&
        All.Contains(status.Trim(), StringComparer.OrdinalIgnoreCase);

    public static string Normalize(string status) =>
        status.Trim().ToLowerInvariant();
}

public class DrawerReconciliation
{
    public int Id { get; private set; }
    public Branch Branch { get; private set; }
    public User User { get; private set; }
    public int PerformedByUserId { get; private set; }
    public decimal ExpectedAmount { get; private set; }
    public decimal CountedAmount { get; private set; }
    public decimal Discrepancy { get; private set; }
    public bool HasDiscrepancy { get; private set; }
    public string Status { get; private set; }
    public string? Reason { get; private set; }
    public int? ResolvedByUserId { get; private set; }
    public DateTime? ResolvedAt { get; private set; }
    public int? AdjustmentTransactionId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public DrawerReconciliation(
        int id,
        Branch branch,
        User user,
        int performedByUserId,
        decimal expectedAmount,
        decimal countedAmount,
        string? status = null,
        string? reason = null,
        int? resolvedByUserId = null,
        DateTime? resolvedAt = null,
        int? adjustmentTransactionId = null,
        DateTime? createdAt = null,
        DateTime? updatedAt = null)
    {
        ArgumentNullException.ThrowIfNull(branch);
        ArgumentNullException.ThrowIfNull(user);

        if (branch.Id <= 0)
        {
            throw new ArgumentException("Branch id must be positive.", nameof(branch));
        }

        if (branch.WarehouseOnly)
        {
            throw new ArgumentException("Warehouse-only branches do not have cash drawers.", nameof(branch));
        }

        if (user.ID <= 0)
        {
            throw new ArgumentException("User id must be positive.", nameof(user));
        }

        if (performedByUserId <= 0)
        {
            throw new ArgumentException("PerformedByUserId must be positive.", nameof(performedByUserId));
        }

        var normalizedExpected = Math.Round(expectedAmount, 2, MidpointRounding.AwayFromZero);
        var normalizedCounted = Math.Round(countedAmount, 2, MidpointRounding.AwayFromZero);

        if (normalizedCounted < 0)
        {
            throw new ArgumentException("CountedAmount cannot be negative.", nameof(countedAmount));
        }

        var discrepancy = Math.Round(normalizedCounted - normalizedExpected, 2, MidpointRounding.AwayFromZero);
        var hasDiscrepancy = discrepancy != 0m;

        string normalizedStatus;
        if (string.IsNullOrWhiteSpace(status))
        {
            normalizedStatus = hasDiscrepancy
                ? DrawerReconciliationStatuses.Pending
                : DrawerReconciliationStatuses.Matched;
        }
        else
        {
            if (!DrawerReconciliationStatuses.IsValid(status))
            {
                throw new ArgumentException($"Invalid drawer reconciliation status '{status}'.", nameof(status));
            }

            normalizedStatus = DrawerReconciliationStatuses.Normalize(status);
        }

        if (resolvedByUserId.HasValue && resolvedByUserId.Value <= 0)
        {
            throw new ArgumentException("ResolvedByUserId must be positive when provided.", nameof(resolvedByUserId));
        }

        if (adjustmentTransactionId.HasValue && adjustmentTransactionId.Value <= 0)
        {
            throw new ArgumentException("AdjustmentTransactionId must be positive when provided.", nameof(adjustmentTransactionId));
        }

        Id = id;
        Branch = branch;
        User = user;
        PerformedByUserId = performedByUserId;
        ExpectedAmount = normalizedExpected;
        CountedAmount = normalizedCounted;
        Discrepancy = discrepancy;
        HasDiscrepancy = hasDiscrepancy;
        Status = normalizedStatus;
        Reason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim();
        ResolvedByUserId = resolvedByUserId;
        ResolvedAt = resolvedAt;
        AdjustmentTransactionId = adjustmentTransactionId;
        CreatedAt = createdAt ?? DateTime.UtcNow;
        UpdatedAt = updatedAt ?? DateTime.UtcNow;
    }

    public void Reconcile(string reason, int resolvedByUserId)
    {
        if (!HasDiscrepancy)
        {
            throw new InvalidOperationException("Cannot reconcile a drawer reconciliation without discrepancies.");
        }

        if (!string.Equals(Status, DrawerReconciliationStatuses.Pending, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Drawer reconciliation has already been reconciled.");
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ArgumentException("Reason is required to reconcile a discrepancy.", nameof(reason));
        }

        if (resolvedByUserId <= 0)
        {
            throw new ArgumentException("ResolvedByUserId must be positive.", nameof(resolvedByUserId));
        }

        Reason = reason.Trim();
        ResolvedByUserId = resolvedByUserId;
        ResolvedAt = DateTime.UtcNow;
        Status = DrawerReconciliationStatuses.Reconciled;
    }
}
