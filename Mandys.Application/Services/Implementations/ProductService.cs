using Mandys.Application.DTOs;
using Mandys.Application.Services.Interfaces;
using Mandys.Domain;

namespace Mandys.Application.Services.Implementations;

public class ProductService(IProductRepository products, IStorageService storageService) : IProductService
{
    public async Task<PagedProductsResponse> GetProductsAsync(int page, int pageSize, string? search, string? orderBy, bool? isSupply)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        var (totalCount, items) = await products.SearchAsync(search, page, pageSize, orderBy, isSupply);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return new PagedProductsResponse(totalCount, page, pageSize, totalPages,
            items.Select(p => p.ToResponse()).ToList());
    }

    public async Task<ProductResponse> GetProductByIdAsync(int id)
    {
        var product = await products.GetByIdAsync(id);
        if (product is null)
        {
            throw ServiceException.NotFound($"No se encontró el producto con ID '{id}'.");
        }

        return product.ToResponse();
    }

    public async Task<ProductResponse> CreateProductAsync(CreateProductRequest request)
    {
        var description = request.Description.Trim();

        if (await products.ExistsByDescriptionAsync(description))
        {
            throw ServiceException.Conflict($"Ya existe un producto con el nombre '{description}'.");
        }

        var created = await products.AddAsync(new Product(
            0,
            description,
            request.IsSupply,
            request.SalePrice,
            MeasureUnits.Normalize(request.MeasureUnit)));

        return created.ToResponse();
    }

    public async Task<ProductResponse> UpdateProductAsync(int id, UpdateProductRequest request)
    {
        var product = await products.GetByIdAsync(id);
        if (product is null)
        {
            throw ServiceException.NotFound($"No se encontró el producto con ID '{id}'.");
        }

        var newDescription = string.IsNullOrWhiteSpace(request.Description)
            ? product.Description
            : request.Description.Trim();

        if (!string.Equals(newDescription, product.Description, StringComparison.OrdinalIgnoreCase) &&
            await products.ExistsByDescriptionAsync(newDescription, excludingId: id))
        {
            throw ServiceException.Conflict($"Ya existe un producto con el nombre '{newDescription}'.");
        }

        var effectiveIsSupply = request.IsSupply ?? product.IsSupply;
        var effectiveMeasureUnit = string.IsNullOrWhiteSpace(request.MeasureUnit)
            ? product.MeasureUnit
            : MeasureUnits.Normalize(request.MeasureUnit);

        if (!MeasureUnits.IsAllowedForProduct(effectiveMeasureUnit, effectiveIsSupply))
        {
            throw ServiceException.BadRequest(
                $"Los productos de venta solo pueden usar la unidad de medida '{MeasureUnits.Units}'.");
        }

        product.UpdateDetails(
            newDescription,
            effectiveIsSupply,
            request.SalePrice ?? product.SalePrice,
            effectiveMeasureUnit);

        await products.UpdateAsync(product);

        return product.ToResponse();
    }

    public async Task<ProductResponse> UpdateProductImageAsync(int id, Stream file, string contentType)
    {
        var product = await products.GetByIdAsync(id);
        if (product is null)
        {
            throw ServiceException.NotFound($"No se encontró el producto con ID '{id}'.");
        }

        var key = $"products/{product.Id}";

        var fileUrl = await storageService.UploadAsync(file, key,contentType);

        product.ImageUrl = fileUrl;

        await products.UpdateAsync(product);

        return product.ToResponse();
    }

    public async Task DeleteProductAsync(int id)
    {
        var removed = await products.RemoveAsync(id);
        if (!removed)
        {
            throw ServiceException.NotFound($"No se encontró el producto con ID '{id}'.");
        }
    }
}
