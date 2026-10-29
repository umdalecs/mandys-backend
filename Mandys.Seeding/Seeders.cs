using Mandys.Application.DTOs;

namespace Mandys.Seeding;

public sealed class SeedContext
{
    public Dictionary<int, int> Branches { get; set; } = [];
    public Dictionary<int, int> Products { get; set; } = [];
    public Dictionary<int, int> Dishes { get; set; } = [];
}

public static class Seeders
{
    public static async Task LoadMissingDependenciesAsync(
        ApiClient api,
        SeedContext ctx,
        IReadOnlyList<string> steps,
        CancellationToken cancellationToken = default)
    {
        var stepSet = new HashSet<string>(steps, StringComparer.Ordinal);

        if (stepSet.Contains("users") && !stepSet.Contains("branches"))
        {
            var existingBranches = await api.ListAllAsync<BranchResponse>("/branches", cancellationToken);
            var liveIdsByName = existingBranches.ToDictionary(b => b.Name, b => b.Id, StringComparer.Ordinal);
            ctx.Branches = MapBranchIds(liveIdsByName);
        }

        if ((stepSet.Contains("dishes") || stepSet.Contains("combos")) && !stepSet.Contains("products"))
        {
            var existingProducts = await api.ListAllAsync<ProductResponse>("/products", cancellationToken);
            var liveIdsByDescription = existingProducts.ToDictionary(p => p.Description, p => p.Id, StringComparer.Ordinal);
            ctx.Products = MapProductIds(liveIdsByDescription);
        }

        if (stepSet.Contains("combos") && !stepSet.Contains("dishes"))
        {
            var existingDishes = await api.ListAllAsync<DishResponse>("/dishes", cancellationToken);
            var liveIdsByName = existingDishes.ToDictionary(d => d.Name, d => d.Id, StringComparer.Ordinal);
            ctx.Dishes = MapDishIds(liveIdsByName);
        }
    }

    public static async Task SeedBranchesAsync(
        ApiClient api,
        SeedContext ctx,
        CancellationToken cancellationToken = default)
    {
        var existingRows = await api.ListAllAsync<BranchResponse>("/branches", cancellationToken);
        var existing = existingRows.ToDictionary(b => b.Name, b => b.Id, StringComparer.Ordinal);
        var created = 0;
        var skipped = 0;

        foreach (var row in SeedData.Branches)
        {
            if (existing.ContainsKey(row.Name))
            {
                skipped++;
                continue;
            }

            var request = new CreateBranchRequest(row.Name, row.Address, row.WarehouseOnly);
            var createdBranch = await api.PostAsync<CreateBranchRequest, BranchResponse>("/branches", request, cancellationToken);
            existing[row.Name] = createdBranch.Id;
            created++;
        }

        ctx.Branches = MapBranchIds(existing);
        Summary("branches", created, skipped);
    }

    public static async Task SeedUsersAsync(
        ApiClient api,
        SeedContext ctx,
        string password,
        CancellationToken cancellationToken = default)
    {
        var existingRows = await api.ListAllAsync<UserResponse>("/users", cancellationToken);
        var existing = existingRows
            .Select(u => u.Email.ToLowerInvariant())
            .ToHashSet(StringComparer.Ordinal);
        var created = 0;
        var skipped = 0;

        foreach (var row in SeedData.Users)
        {
            if (existing.Contains(row.Email.ToLowerInvariant()))
            {
                skipped++;
                continue;
            }

            int? branchId = row.BranchId.HasValue ? ResolveId(ctx.Branches, row.BranchId.Value, "branch") : null;
            var request = new CreateUserRequest(
                row.FirstName,
                row.FatherLastName,
                row.Email,
                password,
                row.MotherLastName,
                row.Role,
                branchId
            );

            await api.PostAsync("/users", request, cancellationToken);
            existing.Add(row.Email.ToLowerInvariant());
            created++;
        }

        Summary("users", created, skipped);
    }

    public static async Task SeedProductsAsync(
        ApiClient api,
        SeedContext ctx,
        CancellationToken cancellationToken = default)
    {
        var existingRows = await api.ListAllAsync<ProductResponse>("/products", cancellationToken);
        var existing = existingRows.ToDictionary(p => p.Description, p => p.Id, StringComparer.Ordinal);
        var created = 0;
        var skipped = 0;

        foreach (var row in SeedData.Products)
        {
            if (existing.ContainsKey(row.Description))
            {
                skipped++;
                continue;
            }

            var request = new CreateProductRequest(
                row.Description,
                row.IsSupply,
                row.MeasureUnit,
                row.SalePrice
            );

            var createdProduct = await api.PostAsync<CreateProductRequest, ProductResponse>("/products", request, cancellationToken);
            existing[row.Description] = createdProduct.Id;
            created++;
        }

        ctx.Products = MapProductIds(existing);
        Summary("products", created, skipped);
    }

    public static async Task SeedDishesAsync(
        ApiClient api,
        SeedContext ctx,
        CancellationToken cancellationToken = default)
    {
        var existingRows = await api.ListAllAsync<DishResponse>("/dishes", cancellationToken);
        var existing = existingRows.ToDictionary(d => d.Name, d => d.Id, StringComparer.Ordinal);
        var created = 0;
        var skipped = 0;

        foreach (var row in SeedData.Dishes)
        {
            if (existing.ContainsKey(row.Name))
            {
                skipped++;
                continue;
            }

            var recipe = row.Recipe
                .Select(l => new CreateDishRecipeLineRequest(
                    ResolveId(ctx.Products, l.ProductId, "product"),
                    l.Quantity))
                .ToList();

            var request = new CreateDishRequest(row.Name, row.Price, recipe);
            var createdDish = await api.PostAsync<CreateDishRequest, DishResponse>("/dishes", request, cancellationToken);
            existing[row.Name] = createdDish.Id;
            created++;
        }

        ctx.Dishes = MapDishIds(existing);
        Summary("dishes", created, skipped);
    }

    public static async Task SeedCombosAsync(
        ApiClient api,
        SeedContext ctx,
        CancellationToken cancellationToken = default)
    {
        var existingRows = await api.ListAllAsync<ComboResponse>("/combos", cancellationToken);
        var existing = existingRows.Select(c => c.Name).ToHashSet(StringComparer.Ordinal);
        var created = 0;
        var skipped = 0;

        foreach (var row in SeedData.Combos)
        {
            if (existing.Contains(row.Name))
            {
                skipped++;
                continue;
            }

            var dishes = row.Dishes
                .Select(l => new CreateComboDishLineRequest(
                    ResolveId(ctx.Dishes, l.DishId, "dish"),
                    l.Quantity))
                .ToList();

            var products = row.Products
                .Select(l => new CreateComboProductLineRequest(
                    ResolveId(ctx.Products, l.ProductId, "product"),
                    l.Quantity))
                .ToList();

            var request = new CreateComboRequest(row.Name, row.Price, dishes, products);
            await api.PostAsync("/combos", request, cancellationToken);
            existing.Add(row.Name);
            created++;
        }

        Summary("combos", created, skipped);
    }

    private static void Summary(string label, int created, int skipped)
    {
        ApiClient.Log($"{label}: {created} created, {skipped} skipped");
    }

    private static Dictionary<int, int> MapBranchIds(IReadOnlyDictionary<string, int> liveIdsByName)
    {
        var map = new Dictionary<int, int>();
        foreach (var row in SeedData.Branches)
        {
            if (!liveIdsByName.TryGetValue(row.Name, out var liveId))
            {
                throw new ApiException($"Missing branch '{row.Name}' in API response.");
            }

            map[row.Id] = liveId;
        }

        return map;
    }

    private static Dictionary<int, int> MapProductIds(IReadOnlyDictionary<string, int> liveIdsByDescription)
    {
        var map = new Dictionary<int, int>();
        foreach (var row in SeedData.Products)
        {
            if (!liveIdsByDescription.TryGetValue(row.Description, out var liveId))
            {
                throw new ApiException($"Missing product '{row.Description}' in API response.");
            }

            map[row.Id] = liveId;
        }

        return map;
    }

    private static Dictionary<int, int> MapDishIds(IReadOnlyDictionary<string, int> liveIdsByName)
    {
        var map = new Dictionary<int, int>();
        foreach (var row in SeedData.Dishes)
        {
            if (!liveIdsByName.TryGetValue(row.Name, out var liveId))
            {
                throw new ApiException($"Missing dish '{row.Name}' in API response.");
            }

            map[row.Id] = liveId;
        }

        return map;
    }

    private static int ResolveId(IReadOnlyDictionary<int, int> map, int seedId, string entityName)
    {
        if (!map.TryGetValue(seedId, out var liveId))
        {
            throw new ApiException($"Could not resolve {entityName} seed id {seedId} to a live API id.");
        }

        return liveId;
    }
}
