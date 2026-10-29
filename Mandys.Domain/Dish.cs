namespace Mandys.Domain;

public class Dish
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public decimal Price { get; private set; }
    public IReadOnlyList<DishProduct> Recipe { get; private set; }
    public string? ImageUrl { get; set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public Dish(
        int id,
        string name,
        decimal price,
        IEnumerable<DishProduct>? recipe = null,
        DateTime? createdAt = null,
        DateTime? updatedAt = null,
        string? imageUrl = null)
    {
        Id = id;
        Name = GuardNotEmpty(name, nameof(name));
        Price = GuardPrice(price, nameof(price));
        Recipe = (recipe ?? []).ToList();
        CreatedAt = createdAt ?? DateTime.UtcNow;
        UpdatedAt = updatedAt ?? DateTime.UtcNow;
        ImageUrl = imageUrl;
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
    public Product Product { get; private set; }
    public decimal Quantity { get; private set; }

    public DishProduct(Product product, decimal quantity)
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

        if (!MeasureUnits.IsValidQuantity(product.MeasureUnit, quantity))
        {
            throw new ArgumentException(
                $"Product '{product.Description}' measured in '{product.MeasureUnit}' does not allow decimal quantities.",
                nameof(quantity));
        }

        Product = product;
        Quantity = quantity;
    }
}
