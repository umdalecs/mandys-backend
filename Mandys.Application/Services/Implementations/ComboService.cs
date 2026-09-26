using Mandys.Domain;
using Mandys.DTOs;
using Mandys.Services;
using Mandys.Services.Interfaces;

namespace Mandys.Services.Implementations;

public class ComboService(
    IComboRepository combos,
    IDishRepository dishes,
    IProductRepository products) : IComboService
{
    public async Task<PagedCombosResponse> GetCombosAsync(int page, int pageSize, string? search)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        var (totalCount, items) = await combos.SearchAsync(search, page, pageSize);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return new PagedCombosResponse(totalCount, page, pageSize, totalPages,
            items.Select(c => c.ToResponse()).ToList());
    }

    public async Task<ComboResponse> GetComboByIdAsync(int id)
    {
        var combo = await combos.GetByIdAsync(id);
        if (combo is null)
        {
            throw ServiceException.NotFound($"Combo with ID '{id}' not found.");
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
            throw ServiceException.NotFound($"Combo with ID '{id}' not found.");
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
            throw ServiceException.NotFound($"Combo with ID '{id}' not found.");
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
                throw ServiceException.NotFound($"Dish with ID '{line.DishId}' not found.");
            }

            resolved.Add(new ComboDish(line.DishId, line.Quantity));
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
                throw ServiceException.NotFound($"Product with ID '{line.ProductId}' not found.");
            }

            if (product.IsSupply)
            {
                throw ServiceException.BadRequest(
                    $"Product '{product.Description}' (ID '{product.Id}') is a supply and cannot be sold in a combo.");
            }

            resolved.Add(new ComboProduct(line.ProductId, line.Quantity));
        }

        return resolved;
    }
}
