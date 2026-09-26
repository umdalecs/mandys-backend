using Mandys.Domain;
using Mandys.DTOs;
using Mandys.Services;
using Mandys.Services.Interfaces;

namespace Mandys.Services.Implementations;

public class ProductService(IProductRepository products) : IProductService
{
    public async Task<PagedProductsResponse> GetProductsAsync(int page, int pageSize, string? search)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        var (totalCount, items) = await products.SearchAsync(search, page, pageSize);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return new PagedProductsResponse(totalCount, page, pageSize, totalPages,
            items.Select(p => p.ToResponse()).ToList());
    }

    public async Task<ProductResponse> GetProductByIdAsync(int id)
    {
        var product = await products.GetByIdAsync(id);
        if (product is null)
        {
            throw ServiceException.NotFound($"Product with ID '{id}' not found.");
        }

        return product.ToResponse();
    }

    public async Task<ProductResponse> CreateProductAsync(CreateProductRequest request)
    {
        var created = await products.AddAsync(new Product(
            0,
            request.Description.Trim(),
            request.IsSupply,
            request.Price,
            request.MeasureUnit.Trim()));

        return created.ToResponse();
    }

    public async Task<ProductResponse> UpdateProductAsync(int id, UpdateProductRequest request)
    {
        var product = await products.GetByIdAsync(id);
        if (product is null)
        {
            throw ServiceException.NotFound($"Product with ID '{id}' not found.");
        }

        product.UpdateDetails(
            string.IsNullOrWhiteSpace(request.Description) ? product.Description : request.Description.Trim(),
            request.IsSupply ?? product.IsSupply,
            request.Price ?? product.Price,
            string.IsNullOrWhiteSpace(request.MeasureUnit) ? product.MeasureUnit : request.MeasureUnit.Trim());

        await products.UpdateAsync(product);

        return product.ToResponse();
    }

    public async Task DeleteProductAsync(int id)
    {
        var removed = await products.RemoveAsync(id);
        if (!removed)
        {
            throw ServiceException.NotFound($"Product with ID '{id}' not found.");
        }
    }
}
