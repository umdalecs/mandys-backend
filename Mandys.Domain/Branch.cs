namespace Mandys.Domain;

/// <summary>
/// Domain branch. Guards its own invariants; persistence mapping lives in
/// Infrastructure.
/// </summary>
public class Branch
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public string Address { get; private set; }
    public bool WarehouseOnly { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public Branch(
        int id,
        string name,
        string address,
        bool warehouseOnly,
        DateTime? createdAt = null,
        DateTime? updatedAt = null)
    {
        Id = id;
        Name = GuardNotEmpty(name, nameof(name));
        Address = GuardNotEmpty(address, nameof(address));
        WarehouseOnly = warehouseOnly;
        CreatedAt = createdAt ?? DateTime.UtcNow;
        UpdatedAt = updatedAt ?? DateTime.UtcNow;
    }

    public void UpdateDetails(string name, string address, bool warehouseOnly)
    {
        Name = GuardNotEmpty(name, nameof(name));
        Address = GuardNotEmpty(address, nameof(address));
        WarehouseOnly = warehouseOnly;
    }

    private static string GuardNotEmpty(string value, string name) =>
        string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentException($"{name} is required.", name)
            : value.Trim();
}
