namespace Mandys.Domain;

/// <summary>
/// Domain product. Guards its own invariants; persistence mapping lives in
/// Infrastructure.
/// </summary>
public class Product
{
    public int Id { get; private set; }
    public string Description { get; private set; }
    public string IsSupply { get; private set; }
    public string Price { get; private set; }
    public string MeasureUnit { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public Product(
        int id,
        string description,
        string isSupply,
        string price,
        string measureUnit,
        DateTime? createdAt = null,
        DateTime? updatedAt = null)
    {
        Id = id;
        Description = GuardNotEmpty(description, nameof(description));
        IsSupply = GuardNotEmpty(isSupply, nameof(isSupply));
        Price = GuardNotEmpty(price, nameof(price));
        MeasureUnit = GuardNotEmpty(measureUnit, nameof(measureUnit));
        CreatedAt = createdAt ?? DateTime.UtcNow;
        UpdatedAt = updatedAt ?? DateTime.UtcNow;
    }

    public void UpdateDetails(string description, string isSupply, string price, string measureUnit)
    {
        Description = GuardNotEmpty(description, nameof(description));
        IsSupply = GuardNotEmpty(isSupply, nameof(isSupply));
        Price = GuardNotEmpty(price, nameof(price));
        MeasureUnit = GuardNotEmpty(measureUnit, nameof(measureUnit));
    }

    private static string GuardNotEmpty(string value, string name) =>
        string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentException($"{name} is required.", name)
            : value.Trim();
}
