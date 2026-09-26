namespace Mandys.Domain;

/// <summary>
/// Sellable menu dish (franchise-wide catalog). Guards its own invariants;
/// persistence mapping lives in Infrastructure.
/// </summary>
public class Dish
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public decimal Price { get; private set; }
    public IReadOnlyList<DishProduct> Recipe { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public Dish(
        int id,
        string name,
        decimal price,
        IEnumerable<DishProduct>? recipe = null,
        DateTime? createdAt = null,
        DateTime? updatedAt = null)
    {
        Id = id;
        Name = GuardNotEmpty(name, nameof(name));
        Price = GuardPrice(price, nameof(price));
        Recipe = (recipe ?? []).ToList();
        CreatedAt = createdAt ?? DateTime.UtcNow;
        UpdatedAt = updatedAt ?? DateTime.UtcNow;
    }

    public void UpdateDetails(string name, decimal price, IEnumerable<DishProduct>? recipe = null)
    {
        Name = GuardNotEmpty(name, nameof(name));
        Price = GuardPrice(price, nameof(price));
        if (recipe is not null)
        {
            Recipe = recipe.ToList();
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
/// One recipe line: the amount of a catalog product used in a dish.
/// </summary>
public class DishProduct
{
    public int ProductId { get; private set; }
    public int Quantity { get; private set; }

    public DishProduct(int productId, int quantity)
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
