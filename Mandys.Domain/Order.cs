namespace Mandys.Domain;

public static class PaymentMethods
{
    public const string Cash = "cash";
    public const string Card = "card";

    public static readonly IReadOnlyList<string> All = [Cash, Card];

    public static bool IsValid(string? method) =>
        !string.IsNullOrWhiteSpace(method) &&
        All.Contains(method.Trim(), StringComparer.OrdinalIgnoreCase);

    public static string Normalize(string method) =>
        method.Trim().ToLowerInvariant();
}

public class Order
{
    public int Id { get; private set; }
    public Branch Branch { get; private set; }
    public User CreatedByUser { get; private set; }
    public User? Customer { get; private set; }
    public string PaymentMethod { get; private set; }
    public string? CardLast4 { get; private set; }
    public decimal SubtotalAmount { get; private set; }
    public decimal PointsSpent { get; private set; }
    public decimal TotalAmount { get; private set; }
    public decimal PointsEarned { get; private set; }
    public IReadOnlyList<OrderProduct> Products { get; private set; }
    public IReadOnlyList<OrderDish> Dishes { get; private set; }
    public IReadOnlyList<OrderCombo> Combos { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public Order(
        int id,
        Branch branch,
        User createdByUser,
        string paymentMethod,
        string? cardLast4,
        IEnumerable<OrderProduct>? products = null,
        IEnumerable<OrderDish>? dishes = null,
        IEnumerable<OrderCombo>? combos = null,
        DateTime? createdAt = null,
        DateTime? updatedAt = null,
        User? customer = null,
        decimal pointsEarned = 0m,
        decimal pointsSpent = 0m)
    {
        ArgumentNullException.ThrowIfNull(branch);
        ArgumentNullException.ThrowIfNull(createdByUser);

        if (branch.Id <= 0)
        {
            throw new ArgumentException("Branch id must be positive.", nameof(branch));
        }

        if (branch.WarehouseOnly)
        {
            throw new ArgumentException("Warehouse-only branches cannot accept orders.", nameof(branch));
        }

        if (createdByUser.ID <= 0)
        {
            throw new ArgumentException("User id must be positive.", nameof(createdByUser));
        }

        if (customer is not null)
        {
            if (customer.ID <= 0)
            {
                throw new ArgumentException("Customer id must be positive.", nameof(customer));
            }

            if (!string.Equals(customer.Role, Roles.Customer, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Assigned customer must have the customer role.", nameof(customer));
            }
        }

        if (pointsEarned < 0)
        {
            throw new ArgumentException("PointsEarned cannot be negative.", nameof(pointsEarned));
        }

        if (customer is null && pointsEarned > 0)
        {
            throw new ArgumentException("Cannot award points without an associated customer.", nameof(pointsEarned));
        }

        if (pointsSpent < 0)
        {
            throw new ArgumentException("PointsSpent cannot be negative.", nameof(pointsSpent));
        }

        if (customer is null && pointsSpent > 0)
        {
            throw new ArgumentException("Cannot spend points without an associated customer.", nameof(pointsSpent));
        }

        if (!PaymentMethods.IsValid(paymentMethod))
        {
            throw new ArgumentException($"Invalid payment method '{paymentMethod}'.", nameof(paymentMethod));
        }

        var normalizedMethod = PaymentMethods.Normalize(paymentMethod);
        var normalizedCardLast4 = string.IsNullOrWhiteSpace(cardLast4) ? null : cardLast4.Trim();

        if (normalizedMethod == PaymentMethods.Card)
        {
            if (normalizedCardLast4 is null || normalizedCardLast4.Length != 4 || !normalizedCardLast4.All(char.IsAsciiDigit))
            {
                throw new ArgumentException("CardLast4 must be 4 digits for card payments.", nameof(cardLast4));
            }
        }
        else
        {
            normalizedCardLast4 = null;
        }

        var productList = (products ?? []).ToList();
        var dishList = (dishes ?? []).ToList();
        var comboList = (combos ?? []).ToList();

        if (productList.Count == 0 && dishList.Count == 0 && comboList.Count == 0)
        {
            throw new ArgumentException("Order must contain at least one product, dish, or combo.");
        }

        var subtotalAmount =
            productList.Sum(p => p.Quantity * p.UnitPrice) +
            dishList.Sum(d => d.Quantity * d.UnitPrice) +
            comboList.Sum(c => c.Quantity * c.UnitPrice);

        var normalizedPointsSpent = Math.Round(pointsSpent, 2, MidpointRounding.AwayFromZero);
        if (normalizedPointsSpent > subtotalAmount)
        {
            throw new ArgumentException("PointsSpent cannot exceed the order subtotal.", nameof(pointsSpent));
        }

        Id = id;
        Branch = branch;
        CreatedByUser = createdByUser;
        Customer = customer;
        PaymentMethod = normalizedMethod;
        CardLast4 = normalizedCardLast4;
        Products = productList;
        Dishes = dishList;
        Combos = comboList;
        SubtotalAmount = subtotalAmount;
        PointsSpent = normalizedPointsSpent;
        TotalAmount = subtotalAmount - normalizedPointsSpent;
        PointsEarned = Math.Round(pointsEarned, 2, MidpointRounding.AwayFromZero);
        CreatedAt = createdAt ?? DateTime.UtcNow;
        UpdatedAt = updatedAt ?? DateTime.UtcNow;
    }

    public static decimal CalculatePoints(decimal totalAmount, decimal pointsPercentage)
    {
        if (pointsPercentage < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(pointsPercentage),
                "Points percentage cannot be negative.");
        }

        if (totalAmount <= 0 || pointsPercentage == 0)
        {
            return 0m;
        }

        return Math.Round(totalAmount * (pointsPercentage / 100m), 2, MidpointRounding.AwayFromZero);
    }
}

public class OrderProduct
{
    public Product Product { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }

    public OrderProduct(Product product, decimal quantity, decimal unitPrice)
    {
        ArgumentNullException.ThrowIfNull(product);

        if (product.Id <= 0)
        {
            throw new ArgumentException("Product id must be positive.", nameof(product));
        }

        if (product.IsSupply)
        {
            throw new ArgumentException($"Supply product '{product.Description}' cannot be sold in an order.", nameof(product));
        }

        if (quantity <= 0 || !decimal.IsInteger(quantity))
        {
            throw new ArgumentException("Product quantity in an order must be a positive whole number.", nameof(quantity));
        }

        if (unitPrice < 0)
        {
            throw new ArgumentException("UnitPrice must be non-negative.", nameof(unitPrice));
        }

        Product = product;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
}

public class OrderDish
{
    public Dish Dish { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }

    public OrderDish(Dish dish, decimal quantity, decimal unitPrice)
    {
        ArgumentNullException.ThrowIfNull(dish);

        if (dish.Id <= 0)
        {
            throw new ArgumentException("Dish id must be positive.", nameof(dish));
        }

        if (quantity <= 0 || !decimal.IsInteger(quantity))
        {
            throw new ArgumentException("Dish quantity in an order must be a positive whole number.", nameof(quantity));
        }

        if (unitPrice < 0)
        {
            throw new ArgumentException("UnitPrice must be non-negative.", nameof(unitPrice));
        }

        Dish = dish;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
}

public class OrderCombo
{
    public Combo Combo { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }

    public OrderCombo(Combo combo, decimal quantity, decimal unitPrice)
    {
        ArgumentNullException.ThrowIfNull(combo);

        if (combo.Id <= 0)
        {
            throw new ArgumentException("Combo id must be positive.", nameof(combo));
        }

        if (quantity <= 0 || !decimal.IsInteger(quantity))
        {
            throw new ArgumentException("Combo quantity in an order must be a positive whole number.", nameof(quantity));
        }

        if (unitPrice < 0)
        {
            throw new ArgumentException("UnitPrice must be non-negative.", nameof(unitPrice));
        }

        Combo = combo;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
}
