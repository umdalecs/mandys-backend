namespace Mandys.Domain;

public static class StorageMovementTypes
{
    public const string In = "in";
    public const string Out = "out";

    public static readonly IReadOnlyList<string> All = [In, Out];

    public static bool IsValid(string? type) =>
        !string.IsNullOrWhiteSpace(type) &&
        All.Contains(type.Trim(), StringComparer.OrdinalIgnoreCase);

    public static string Normalize(string type) =>
        type.Trim().ToLowerInvariant();
}

public class BranchStorageHistory
{
    public int Id { get; private set; }
    public Branch Branch { get; private set; }
    public string Type { get; private set; }
    public string? Folio { get; private set; }
    public IReadOnlyList<BranchStorageHistoryProduct> Products { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public BranchStorageHistory(
        int id,
        Branch branch,
        string type,
        string? folio,
        IEnumerable<BranchStorageHistoryProduct> products,
        DateTime? createdAt = null,
        DateTime? updatedAt = null)
    {
        ArgumentNullException.ThrowIfNull(branch);
        ArgumentNullException.ThrowIfNull(products);

        if (branch.Id <= 0)
        {
            throw new ArgumentException("Branch id must be positive.", nameof(branch));
        }

        if (!StorageMovementTypes.IsValid(type))
        {
            throw new ArgumentException($"Invalid movement type '{type}'.", nameof(type));
        }

        var normalizedType = StorageMovementTypes.Normalize(type);
        var normalizedFolio = string.IsNullOrWhiteSpace(folio) ? null : folio.Trim();

        if (normalizedType == StorageMovementTypes.In && normalizedFolio is null)
        {
            throw new ArgumentException("Folio is required for 'in' movements.", nameof(folio));
        }

        var productList = products.ToList();
        if (productList.Count == 0)
        {
            throw new ArgumentException("At least one product line is required.", nameof(products));
        }

        Id = id;
        Branch = branch;
        Type = normalizedType;
        Folio = normalizedFolio;
        Products = productList;
        CreatedAt = createdAt ?? DateTime.UtcNow;
        UpdatedAt = updatedAt ?? DateTime.UtcNow;
    }
}

public class BranchStorageHistoryProduct
{
    public Product Product { get; private set; }
    public decimal Quantity { get; private set; }

    public BranchStorageHistoryProduct(Product product, decimal quantity)
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
