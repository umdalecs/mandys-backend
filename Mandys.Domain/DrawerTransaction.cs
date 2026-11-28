namespace Mandys.Domain;

public static class DrawerTransactionTypes
{
    public const string Sale = "sale";
    public const string Opening = "opening";
    public const string Deposit = "deposit";
    public const string Withdrawal = "withdrawal";
    public const string Closing = "closing";

    public static readonly IReadOnlyList<string> All =
    [
        Sale,
        Opening,
        Deposit,
        Withdrawal,
        Closing,
    ];

    public static bool IsValid(string? type) =>
        !string.IsNullOrWhiteSpace(type) &&
        All.Contains(type.Trim(), StringComparer.OrdinalIgnoreCase);

    public static string Normalize(string type) =>
        type.Trim().ToLowerInvariant();
}

public class DrawerTransaction
{
    public int Id { get; private set; }
    public Branch Branch { get; private set; }
    public User User { get; private set; }
    public int? OrderId { get; private set; }
    public string Type { get; private set; }
    public decimal Amount { get; private set; }
    public decimal? AmountPaid { get; private set; }
    public decimal? ChangeAmount { get; private set; }
    public string? Notes { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public DrawerTransaction(
        int id,
        Branch branch,
        User user,
        string type,
        decimal amount,
        decimal? amountPaid = null,
        decimal? changeAmount = null,
        int? orderId = null,
        string? notes = null,
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

        if (orderId.HasValue && orderId.Value <= 0)
        {
            throw new ArgumentException("Order id must be positive when provided.", nameof(orderId));
        }

        if (!DrawerTransactionTypes.IsValid(type))
        {
            throw new ArgumentException($"Invalid drawer transaction type '{type}'.", nameof(type));
        }

        var normalizedType = DrawerTransactionTypes.Normalize(type);
        var normalizedAmount = Math.Round(amount, 2, MidpointRounding.AwayFromZero);
        decimal? normalizedAmountPaid = null;
        decimal? normalizedChangeAmount = null;

        switch (normalizedType)
        {
            case DrawerTransactionTypes.Sale:
                if (normalizedAmount < 0)
                {
                    throw new ArgumentException("Sale drawer transaction amount cannot be negative.", nameof(amount));
                }

                if (!amountPaid.HasValue)
                {
                    throw new ArgumentException("AmountPaid is required for sale drawer transactions.", nameof(amountPaid));
                }

                if (!changeAmount.HasValue)
                {
                    throw new ArgumentException("ChangeAmount is required for sale drawer transactions.", nameof(changeAmount));
                }

                normalizedAmountPaid = Math.Round(amountPaid.Value, 2, MidpointRounding.AwayFromZero);
                normalizedChangeAmount = Math.Round(changeAmount.Value, 2, MidpointRounding.AwayFromZero);

                if (normalizedAmountPaid < 0)
                {
                    throw new ArgumentException("AmountPaid cannot be negative.", nameof(amountPaid));
                }

                if (normalizedChangeAmount < 0)
                {
                    throw new ArgumentException("ChangeAmount cannot be negative.", nameof(changeAmount));
                }

                if (Math.Round(normalizedAmountPaid.Value - normalizedChangeAmount.Value, 2, MidpointRounding.AwayFromZero) != normalizedAmount)
                {
                    throw new ArgumentException("Amount must equal AmountPaid minus ChangeAmount for sale drawer transactions.", nameof(amount));
                }

                break;

            case DrawerTransactionTypes.Opening:
                if (normalizedAmount < 0)
                {
                    throw new ArgumentException("Opening drawer transaction amount cannot be negative.", nameof(amount));
                }

                break;

            case DrawerTransactionTypes.Deposit:
                if (normalizedAmount <= 0)
                {
                    throw new ArgumentException("Deposit drawer transaction amount must be positive.", nameof(amount));
                }

                break;

            case DrawerTransactionTypes.Withdrawal:
                if (normalizedAmount >= 0)
                {
                    throw new ArgumentException("Withdrawal drawer transaction amount must be negative.", nameof(amount));
                }

                break;

            case DrawerTransactionTypes.Closing:
                if (normalizedAmount > 0)
                {
                    throw new ArgumentException("Closing drawer transaction amount cannot be positive.", nameof(amount));
                }

                break;
        }

        Id = id;
        Branch = branch;
        User = user;
        OrderId = orderId;
        Type = normalizedType;
        Amount = normalizedAmount;
        AmountPaid = normalizedAmountPaid;
        ChangeAmount = normalizedChangeAmount;
        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        CreatedAt = createdAt ?? DateTime.UtcNow;
        UpdatedAt = updatedAt ?? DateTime.UtcNow;
    }
}
