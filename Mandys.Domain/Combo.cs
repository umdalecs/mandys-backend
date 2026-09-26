namespace Mandys.Domain;

/// <summary>
/// Sellable menu combo (franchise-wide catalog): a bundle of dishes and
/// products sold at a single price. Guards its own invariants; persistence
/// mapping lives in Infrastructure.
/// </summary>
public class Combo
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public decimal Price { get; private set; }
    public IReadOnlyList<ComboDish> Dishes { get; private set; }
    public IReadOnlyList<ComboProduct> Products { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public Combo(
        int id,
        string name,
        decimal price,
        IEnumerable<ComboDish>? dishes = null,
        IEnumerable<ComboProduct>? products = null,
        DateTime? createdAt = null,
        DateTime? updatedAt = null)
    {
        Id = id;
        Name = GuardNotEmpty(name, nameof(name));
        Price = GuardPrice(price, nameof(price));
        Dishes = (dishes ?? []).ToList();
        Products = (products ?? []).ToList();
        CreatedAt = createdAt ?? DateTime.UtcNow;
        UpdatedAt = updatedAt ?? DateTime.UtcNow;
    }

    public void UpdateDetails(
        string name,
        decimal price,
        IEnumerable<ComboDish>? dishes = null,
        IEnumerable<ComboProduct>? products = null)
    {
        Name = GuardNotEmpty(name, nameof(name));
        Price = GuardPrice(price, nameof(price));
        if (dishes is not null)
        {
            Dishes = dishes.ToList();
        }

        if (products is not null)
        {
            Products = products.ToList();
        }
    }

    private static string GuardNotEmpty(string value, string name) =>
        string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentException($"{name} is required.", name)
            : value.Trim();

    private static decimal GuardPrice(decimal value, string name) =>
        value < 0
            ? throw new ArgumentException($"{name} must be non-negative.", name)
            : value;
}

/// <summary>
/// One combo line: how many portions of a dish the combo includes.
/// </summary>
public class ComboDish
{
    public int DishId { get; private set; }
    public int Quantity { get; private set; }

    public ComboDish(int dishId, int quantity)
    {
        if (dishId <= 0)
        {
            throw new ArgumentException("Dish id must be positive.", nameof(dishId));
        }

        if (quantity <= 0)
        {
            throw new ArgumentException("Quantity must be positive.", nameof(quantity));
        }

        DishId = dishId;
        Quantity = quantity;
    }
}

/// <summary>
/// One combo line: how much of a catalog product the combo includes.
/// </summary>
public class ComboProduct
{
    public int ProductId { get; private set; }
    public int Quantity { get; private set; }

    public ComboProduct(int productId, int quantity)
    {
        if (productId <= 0)
        {
            throw new ArgumentException("Product id must be positive.", nameof(productId));
        }

        if (quantity <= 0)
        {
            throw new ArgumentException("Quantity must be positive.", nameof(quantity));
        }

        ProductId = productId;
        Quantity = quantity;
    }
}
