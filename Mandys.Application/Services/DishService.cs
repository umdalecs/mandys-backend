using Mandys.Domain;
using Mandys.DTOs;

namespace Mandys.Services;

public class DishService(IDishRepository dishes, IProductRepository products) : IDishService
{
    public async Task<PagedDishesResponse> GetDishesAsync(int page, int pageSize, string? search)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        var (totalCount, items) = await dishes.SearchAsync(search, page, pageSize);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return new PagedDishesResponse(totalCount, page, pageSize, totalPages,
            items.Select(d => d.ToResponse()).ToList());
    }

    public async Task<DishResponse> GetDishByIdAsync(int id)
    {
        var dish = await dishes.GetByIdAsync(id);
        if (dish is null)
        {
            throw ServiceException.NotFound($"Dish with ID '{id}' not found.");
        }

        return dish.ToResponse();
    }

    public async Task<DishResponse> CreateDishAsync(CreateDishRequest request)
    {
        // TODO: Change for fluent validation
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw ServiceException.BadRequest("Name is required.");
        }

        if (request.Price < 0)
        {
            throw ServiceException.BadRequest("Price must be non-negative.");
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
            throw ServiceException.NotFound($"Dish with ID '{id}' not found.");
        }

        // TODO: Use fluent validation here
        if (request.Price.HasValue && request.Price.Value < 0)
        {
            throw ServiceException.BadRequest("Price must be non-negative.");
        }

        var recipe = request.Recipe is null ? null : await ValidateRecipeAsync(request.Recipe);

        dish.UpdateDetails(
            string.IsNullOrWhiteSpace(request.Name) ? dish.Name : request.Name.Trim(),
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
            throw ServiceException.NotFound($"Dish with ID '{id}' not found.");
        }
    }

    /// <summary>
    /// Validates recipe lines: positive ids and quantities, no duplicates,
    /// every product exists and is a kitchen supply (insumo), never a
    /// sellable catalog item.
    /// </summary>
    private async Task<IReadOnlyList<DishProduct>> ValidateRecipeAsync(
        IReadOnlyList<CreateDishRecipeLineRequest>? lines)
    {
        if (lines is null || lines.Count == 0)
        {
            return [];
        }

        if (lines.Any(l => l.ProductId <= 0))
        {
            throw ServiceException.BadRequest("Recipe product ids must be positive.");
        }

        if (lines.Any(l => l.Quantity <= 0))
        {
            throw ServiceException.BadRequest("Recipe quantities must be positive.");
        }

        if (lines.GroupBy(l => l.ProductId).Any(g => g.Count() > 1))
        {
            throw ServiceException.BadRequest("Recipe products must not be duplicated.");
        }

        var recipe = new List<DishProduct>();
        foreach (var line in lines)
        {
            var product = await products.GetByIdAsync(line.ProductId);
            if (product is null)
            {
                throw ServiceException.NotFound($"Product with ID '{line.ProductId}' not found.");
            }

            if (!product.IsSupply)
            {
                throw ServiceException.BadRequest(
                    $"Product '{product.Description}' (ID '{product.Id}') is not a supply and cannot be a recipe ingredient.");
            }

            recipe.Add(new DishProduct(line.ProductId, line.Quantity));
        }

        return recipe;
    }
}
