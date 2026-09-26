using Mandys.DTOs;

namespace Mandys.Services.Interfaces;

public interface IProductService
{
    Task<PagedProductsResponse> GetProductsAsync(int page, int pageSize, string? search);

    Task<ProductResponse> GetProductByIdAsync(int id);

    Task<ProductResponse> CreateProductAsync(CreateProductRequest request);

    Task<ProductResponse> UpdateProductAsync(int id, UpdateProductRequest request);

    Task DeleteProductAsync(int id);
}
