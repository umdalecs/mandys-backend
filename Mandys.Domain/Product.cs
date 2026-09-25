namespace Mandys.Domain;

/// <summary>
/// Domain product. Guards its own invariants; persistence mapping lives in
/// Infrastructure.
/// </summary>
public class Product
{
    public int Id { get; private set; }
    public string Description { get; private set; }
    public bool IsSupply { get; private set; }
    public decimal Price { get; private set; }
    public string MeasureUnit { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public Product(
        int id,
        string description,
        bool isSupply,
        decimal price,
        string measureUnit,
        DateTime? createdAt = null,
        DateTime? updatedAt = null)
    {
        Id = id;
        Description = GuardNotEmpty(description, nameof(description));
        IsSupply = isSupply;
        Price = GuardPrice(price, nameof(price));
        MeasureUnit = GuardNotEmpty(measureUnit, nameof(measureUnit));
        CreatedAt = createdAt ?? DateTime.UtcNow;
        UpdatedAt = updatedAt ?? DateTime.UtcNow;
    }

    public void UpdateDetails(string description, bool isSupply, decimal price, string measureUnit)
    {
        Description = GuardNotEmpty(description, nameof(description));
        IsSupply = isSupply;
        Price = GuardPrice(price, nameof(price));
        MeasureUnit = GuardNotEmpty(measureUnit, nameof(measureUnit));
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
