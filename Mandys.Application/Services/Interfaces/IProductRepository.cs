using Mandys.Domain;

namespace Mandys.Application.Services.Interfaces;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(int id);

    Task<(int TotalCount, IReadOnlyList<Product> Items)> SearchAsync(
        string? search, int page, int pageSize, string? orderBy, bool? isSupply);

    Task<Product> AddAsync(Product product);

    Task UpdateAsync(Product product);

    Task<bool> RemoveAsync(int id);

    Task<bool> ExistsByDescriptionAsync(string description, int? excludingId = null);
}
