using Mandys.Application.DTOs;

namespace Mandys.Application.Services.Interfaces;

public interface IProductService
{
    Task<PagedProductsResponse> GetProductsAsync(int page, int pageSize, string? search, string? orderBy, bool? isSupply);

    Task<ProductResponse> GetProductByIdAsync(int id);

    Task<ProductResponse> CreateProductAsync(CreateProductRequest request);

    Task<ProductResponse> UpdateProductAsync(int id, UpdateProductRequest request);

    Task<ProductResponse> UpdateProductImageAsync(int id, Stream file, string contentType);

    Task DeleteProductAsync(int id);
}
