namespace Mandys.Domain;

public class BranchStorage
{
    public int Id { get; private set; }
    public Branch Branch { get; private set; }
    public Product Product { get; private set; }
    public decimal MinimumStock { get; private set; }
    public decimal CurrentStock { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public BranchStorage(
        int id,
        Branch branch,
        Product product,
        decimal minimumStock,
        decimal currentStock,
        DateTime? createdAt = null,
        DateTime? updatedAt = null)
    {
        ArgumentNullException.ThrowIfNull(branch);
        ArgumentNullException.ThrowIfNull(product);

        if (branch.Id <= 0)
        {
            throw new ArgumentException("Branch id must be positive.", nameof(branch));
        }

        if (product.Id <= 0)
        {
            throw new ArgumentException("Product id must be positive.", nameof(product));
        }

        Id = id;
        Branch = branch;
        Product = product;
        MinimumStock = GuardStock(minimumStock, product, nameof(minimumStock));
        CurrentStock = GuardStock(currentStock, product, nameof(currentStock));
        CreatedAt = createdAt ?? DateTime.UtcNow;
        UpdatedAt = updatedAt ?? DateTime.UtcNow;
    }

    public void UpdateMinimumStock(decimal minimumStock)
    {
        MinimumStock = GuardStock(minimumStock, Product, nameof(minimumStock));
    }

    public void AddStock(decimal quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentException("Quantity must be positive.", nameof(quantity));
        }

        CurrentStock = GuardStock(CurrentStock + quantity, Product, nameof(quantity));
    }

    public void RemoveStock(decimal quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentException("Quantity must be positive.", nameof(quantity));
        }

        if (CurrentStock < quantity)
        {
            throw new InvalidOperationException(
                $"Insufficient stock for product '{Product.Description}'. Current: {CurrentStock}, requested: {quantity}.");
        }

        CurrentStock = GuardStock(CurrentStock - quantity, Product, nameof(quantity));
    }

    private static decimal GuardStock(decimal value, Product product, string name)
    {
        if (value < 0)
        {
            throw new ArgumentException($"{name} must be non-negative.", name);
        }

        if (!MeasureUnits.IsValidQuantity(product.MeasureUnit, value))
        {
            throw new ArgumentException(
                $"Product '{product.Description}' measured in '{product.MeasureUnit}' does not allow decimal quantities.",
                name);
        }

        return value;
    }
}
