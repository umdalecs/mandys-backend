using Mandys.Domain;

namespace Mandys.Services;

/// <summary>
/// Product persistence in domain terms. Implemented by Infrastructure.
/// </summary>
public interface IProductRepository
{
    Task<Product?> GetByIdAsync(int id);

    Task<(int TotalCount, IReadOnlyList<Product> Items)> SearchAsync(
        string? search, int page, int pageSize);

    /// <returns>The saved product, with its database-generated id.</returns>
    Task<Product> AddAsync(Product product);

    Task UpdateAsync(Product product);

    /// <returns>False when no product with the id exists (soft-delete).</returns>
    Task<bool> RemoveAsync(int id);
}
