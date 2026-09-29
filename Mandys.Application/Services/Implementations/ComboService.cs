using Mandys.Domain;
using Mandys.DTOs;
using Mandys.Services.Interfaces;

namespace Mandys.Services.Implementations;

public class ComboService(
    IComboRepository combos,
    IDishRepository dishes,
    IProductRepository products) : IComboService
{
    public async Task<PagedCombosResponse> GetCombosAsync(int page, int pageSize, string? search, string? orderBy)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        var (totalCount, items) = await combos.SearchAsync(search, page, pageSize, orderBy);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return new PagedCombosResponse(totalCount, page, pageSize, totalPages,
            items.Select(c => c.ToResponse()).ToList());
    }

    public async Task<ComboResponse> GetComboByIdAsync(int id)
    {
        var combo = await combos.GetByIdAsync(id);
        if (combo is null)
        {
            throw ServiceException.NotFound($"No se encontró el combo con ID '{id}'.");
        }

        return combo.ToResponse();
    }

    public async Task<ComboResponse> CreateComboAsync(CreateComboRequest request)
    {
        var dishLines = await ValidateDishLinesAsync(request.Dishes);
        var productLines = await ValidateProductLinesAsync(request.Products);

        var created = await combos.AddAsync(new Combo(
            0,
            request.Name.Trim(),
            request.Price,
            dishLines,
            productLines));

        return created.ToResponse();
    }

    public async Task<ComboResponse> UpdateComboAsync(int id, UpdateComboRequest request)
    {
        var combo = await combos.GetByIdAsync(id);
        if (combo is null)
        {
            throw ServiceException.NotFound($"No se encontró el combo con ID '{id}'.");
        }

        var dishLines = request.Dishes is null ? null : await ValidateDishLinesAsync(request.Dishes);
        var productLines = request.Products is null ? null : await ValidateProductLinesAsync(request.Products);

        combo.UpdateDetails(
            string.IsNullOrWhiteSpace(request.Name) ? combo.Name : request.Name.Trim(),
            request.Price ?? combo.Price,
            dishLines,
            productLines);

        await combos.UpdateAsync(combo);

        return combo.ToResponse();
    }

    public async Task DeleteComboAsync(int id)
    {
        var removed = await combos.RemoveAsync(id);
        if (!removed)
        {
            throw ServiceException.NotFound($"No se encontró el combo con ID '{id}'.");
        }
    }

    /// <summary>
    /// Resolves combo dish lines against the catalog: every dish must exist.
    /// Shape rules (positive ids/quantities, no duplicates) are enforced by
    /// the request validators.
    /// </summary>
    private async Task<IReadOnlyList<ComboDish>> ValidateDishLinesAsync(
        IReadOnlyList<CreateComboDishLineRequest>? lines)
    {
        if (lines is null || lines.Count == 0)
        {
            return [];
        }

        var resolved = new List<ComboDish>();
        foreach (var line in lines)
        {
            var dish = await dishes.GetByIdAsync(line.DishId);
            if (dish is null)
            {
                throw ServiceException.NotFound($"No se encontró el platillo con ID '{line.DishId}'.");
            }

            resolved.Add(new ComboDish(dish, line.Quantity));
        }

        return resolved;
    }

    /// <summary>
    /// Resolves combo product lines against the catalog: every product must
    /// exist and be a sellable catalog item, never a supply (insumo), because
    /// combos are what customers buy.
    /// </summary>
    private async Task<IReadOnlyList<ComboProduct>> ValidateProductLinesAsync(
        IReadOnlyList<CreateComboProductLineRequest>? lines)
    {
        if (lines is null || lines.Count == 0)
        {
            return [];
        }

        var resolved = new List<ComboProduct>();
        foreach (var line in lines)
        {
            var product = await products.GetByIdAsync(line.ProductId);
            if (product is null)
            {
                throw ServiceException.NotFound($"No se encontró el producto con ID '{line.ProductId}'.");
            }

            if (product.IsSupply)
            {
                throw ServiceException.BadRequest(
                    $"El producto '{product.Description}' (ID '{product.Id}') es un insumo y no puede venderse en un combo.");
            }

            resolved.Add(new ComboProduct(product, line.Quantity));
        }

        return resolved;
    }
}
