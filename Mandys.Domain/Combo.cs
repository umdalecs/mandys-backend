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
    public Dish Dish { get; private set; }
    public decimal Quantity { get; private set; }

    public ComboDish(Dish dish, decimal quantity)
    {
        ArgumentNullException.ThrowIfNull(dish);

        if (dish.Id <= 0)
        {
            throw new ArgumentException("Dish id must be positive.", nameof(dish));
        }

        if (quantity <= 0)
        {
            throw new ArgumentException("Quantity must be positive.", nameof(quantity));
        }

        Dish = dish;
        Quantity = quantity;
    }
}

/// <summary>
/// One combo line: how much of a catalog product the combo includes.
/// </summary>
public class ComboProduct
{
    public Product Product { get; private set; }
    public decimal Quantity { get; private set; }

    public ComboProduct(Product product, decimal quantity)
    {
        ArgumentNullException.ThrowIfNull(product);

        if (product.Id <= 0)
        {
            throw new ArgumentException("Product id must be positive.", nameof(product));
        }

        if (quantity <= 0)
        {
            throw new ArgumentException("Quantity must be positive.", nameof(quantity));
        }

        Product = product;
        Quantity = quantity;
    }
}
