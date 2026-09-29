using Mandys.Domain;
using Mandys.DTOs;
using Mandys.Services.Interfaces;

namespace Mandys.Services.Implementations;

public class DishService(IDishRepository dishes, IProductRepository products) : IDishService
{
    public async Task<PagedDishesResponse> GetDishesAsync(int page, int pageSize, string? search, string? orderBy)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        var (totalCount, items) = await dishes.SearchAsync(search, page, pageSize, orderBy);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return new PagedDishesResponse(totalCount, page, pageSize, totalPages,
            items.Select(d => d.ToResponse()).ToList());
    }

    public async Task<DishResponse> GetDishByIdAsync(int id)
    {
        var dish = await dishes.GetByIdAsync(id);
        if (dish is null)
        {
            throw ServiceException.NotFound($"No se encontró el platillo con ID '{id}'.");
        }

        return dish.ToResponse();
    }

    public async Task<DishResponse> CreateDishAsync(CreateDishRequest request)
    {
        var name = request.Name.Trim();

        if (await dishes.ExistsByNameAsync(name))
        {
            throw ServiceException.Conflict($"Ya existe un platillo con el nombre '{name}'.");
        }

        var recipe = await ValidateRecipeAsync(request.Recipe);

        var created = await dishes.AddAsync(new Dish(
            0,
            request.Name.Trim(),
            request.Price,
            recipe));

        return created.ToResponse();
    }

    public async Task<DishResponse> UpdateDishAsync(int id, UpdateDishRequest request)
    {
        var dish = await dishes.GetByIdAsync(id);
        if (dish is null)
        {
            throw ServiceException.NotFound($"No se encontró el platillo con ID '{id}'.");
        }

        var newName = string.IsNullOrWhiteSpace(request.Name) ? dish.Name : request.Name.Trim();

        if (!string.Equals(newName, dish.Name, StringComparison.OrdinalIgnoreCase) &&
            await dishes.ExistsByNameAsync(newName, excludingId: id))
        {
            throw ServiceException.Conflict($"Ya existe un platillo con el nombre '{newName}'.");
        }

        var recipe = request.Recipe is null ? null : await ValidateRecipeAsync(request.Recipe);

        dish.UpdateDetails(
            newName,
            request.Price ?? dish.Price,
            recipe);

        await dishes.UpdateAsync(dish);

        return dish.ToResponse();
    }

    public async Task DeleteDishAsync(int id)
    {
        var removed = await dishes.RemoveAsync(id);
        if (!removed)
        {
            throw ServiceException.NotFound($"No se encontró el platillo con ID '{id}'.");
        }
    }

    /// <summary>
    /// Resolves recipe lines against the catalog: every product must exist
    /// and be a kitchen supply (insumo), never a sellable catalog item.
    /// Shape rules (positive ids/quantities, no duplicates) are enforced by
    /// the request validators.
    /// </summary>
    private async Task<IReadOnlyList<DishProduct>> ValidateRecipeAsync(
        IReadOnlyList<CreateDishRecipeLineRequest>? lines)
    {
        if (lines is null || lines.Count == 0)
        {
            return [];
        }

        var recipe = new List<DishProduct>();
        foreach (var line in lines)
        {
            var product = await products.GetByIdAsync(line.ProductId);
            if (product is null)
            {
                throw ServiceException.NotFound($"No se encontró el producto con ID '{line.ProductId}'.");
            }

            if (!product.IsSupply)
            {
                throw ServiceException.BadRequest(
                    $"El producto '{product.Description}' (ID '{product.Id}') no es un insumo y no puede ser ingrediente de una receta.");
            }

            recipe.Add(new DishProduct(product, line.Quantity));
        }

        return recipe;
    }
}
