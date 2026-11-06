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
    public static async Task SeedBranchesAsync(
        ApiClient api,
        SeedContext ctx,
        CancellationToken cancellationToken = default)
    {
        ApiClient.Log("[branches] Seeding branches...");
        var existingRows = await api.ListAllAsync<BranchResponse>("/branches", cancellationToken);
        var existing = existingRows.ToDictionary(b => b.Name, b => b.Id, StringComparer.Ordinal);
        var createdIds = new List<int>();
        var skippedIds = new List<int>();

        foreach (var row in SeedData.Branches)
        {
            if (existing.TryGetValue(row.Name, out var existingId))
            {
                skippedIds.Add(existingId);
                continue;
            }

            var createdBranch = await api.PostAsync<CreateBranchRequest, BranchResponse>("/branches", row, cancellationToken);
            existing[row.Name] = createdBranch.Id;
            createdIds.Add(createdBranch.Id);
        }

        ctx.Branches = MapBranchIds(existing);
        Summary("branches", createdIds, skippedIds);
    }

    public static async Task SeedUsersAsync(
        ApiClient api,
        SeedContext ctx,
        CancellationToken cancellationToken = default)
    {
        ApiClient.Log("[users] Seeding staff users...");
        var existingRows = await api.ListAllAsync<UserResponse>("/users", cancellationToken);
        var existing = existingRows.ToDictionary(u => u.Email.ToLowerInvariant(), u => u.Id, StringComparer.Ordinal);
        var createdIds = new List<int>();
        var skippedIds = new List<int>();

        foreach (var row in SeedData.Users)
        {
            var emailKey = row.Email.ToLowerInvariant();
            if (existing.TryGetValue(emailKey, out var existingId))
            {
                skippedIds.Add(existingId);
                continue;
            }

            var request = row with
            {
                BranchId = row.BranchId.HasValue ? ResolveId(ctx.Branches, row.BranchId.Value, "branch") : null
            };

            var createdUser = await api.PostAsync<CreateUserRequest, UserResponse>("/users", request, cancellationToken);
            existing[emailKey] = createdUser.Id;
            createdIds.Add(createdUser.Id);
        }

        Summary("users", createdIds, skippedIds);
    }

    public static async Task SeedProductsAsync(
        ApiClient api,
        SeedContext ctx,
        CancellationToken cancellationToken = default)
    {
        ApiClient.Log("[products] Seeding catalog products and supplies...");
        var existingRows = await api.ListAllAsync<ProductResponse>("/products", cancellationToken);
        var existing = existingRows.ToDictionary(p => p.Description, p => p.Id, StringComparer.Ordinal);
        var createdIds = new List<int>();
        var skippedIds = new List<int>();

        foreach (var row in SeedData.Products)
        {
            if (existing.TryGetValue(row.Description, out var existingId))
            {
                skippedIds.Add(existingId);
                continue;
            }

            var createdProduct = await api.PostAsync<CreateProductRequest, ProductResponse>("/products", row, cancellationToken);
            existing[row.Description] = createdProduct.Id;
            createdIds.Add(createdProduct.Id);
        }

        ctx.Products = MapProductIds(existing);
        Summary("products", createdIds, skippedIds);
    }

    public static async Task SeedDishesAsync(
        ApiClient api,
        SeedContext ctx,
        CancellationToken cancellationToken = default)
    {
        ApiClient.Log("[dishes] Seeding dishes and recipes...");
        var existingRows = await api.ListAllAsync<DishResponse>("/dishes", cancellationToken);
        var existing = existingRows.ToDictionary(d => d.Name, d => d.Id, StringComparer.Ordinal);
        var createdIds = new List<int>();
        var skippedIds = new List<int>();

        foreach (var row in SeedData.Dishes)
        {
            if (existing.TryGetValue(row.Name, out var existingId))
            {
                skippedIds.Add(existingId);
                continue;
            }

            var request = row with
            {
                Recipe = row.Recipe?
                    .Select(l => l with { ProductId = ResolveId(ctx.Products, l.ProductId, "product") })
                    .ToList()
            };

            var createdDish = await api.PostAsync<CreateDishRequest, DishResponse>("/dishes", request, cancellationToken);
            existing[row.Name] = createdDish.Id;
            createdIds.Add(createdDish.Id);
        }

        ctx.Dishes = MapDishIds(existing);
        Summary("dishes", createdIds, skippedIds);
    }

    public static async Task SeedCombosAsync(
        ApiClient api,
        SeedContext ctx,
        CancellationToken cancellationToken = default)
    {
        ApiClient.Log("[combos] Seeding combos...");
        var existingRows = await api.ListAllAsync<ComboResponse>("/combos", cancellationToken);
        var existing = existingRows.ToDictionary(c => c.Name, c => c.Id, StringComparer.Ordinal);
        var createdIds = new List<int>();
        var skippedIds = new List<int>();

        foreach (var row in SeedData.Combos)
        {
            if (existing.TryGetValue(row.Name, out var existingId))
            {
                skippedIds.Add(existingId);
                continue;
            }

            var request = row with
            {
                Dishes = row.Dishes?
                    .Select(l => l with { DishId = ResolveId(ctx.Dishes, l.DishId, "dish") })
                    .ToList(),
                Products = row.Products?
                    .Select(l => l with { ProductId = ResolveId(ctx.Products, l.ProductId, "product") })
                    .ToList()
            };

            var createdCombo = await api.PostAsync<CreateComboRequest, ComboResponse>("/combos", request, cancellationToken);
            existing[row.Name] = createdCombo.Id;
            createdIds.Add(createdCombo.Id);
        }

        Summary("combos", createdIds, skippedIds);
    }

    public static async Task SeedStorageMovementsAsync(
        ApiClient api,
        SeedContext ctx,
        CancellationToken cancellationToken = default)
    {
        ApiClient.Log("[storage] Seeding branch storage 'in' and 'out' movements...");
        var createdIds = new List<int>();
        var skippedIds = new List<int>();

        foreach (var (branchSeedId, inRequests) in SeedData.StorageIns)
        {
            var branchId = ResolveId(ctx.Branches, branchSeedId, "branch");
            var existingHistory = await api.ListAllAsync<BranchStorageHistoryResponse>(
                $"/branches/{branchId}/storage/history", cancellationToken);
            var existingByFolio = existingHistory
                .Where(h => !string.IsNullOrWhiteSpace(h.Folio))
                .ToDictionary(h => h.Folio!, h => h.Id, StringComparer.OrdinalIgnoreCase);

            foreach (var row in inRequests)
            {
                if (existingByFolio.TryGetValue(row.Folio, out var existingId))
                {
                    skippedIds.Add(existingId);
                    continue;
                }

                var request = row with
                {
                    Products = row.Products
                        .Select(l => l with { ProductId = ResolveId(ctx.Products, l.ProductId, "product") })
                        .ToList()
                };

                var createdMovement = await api.PostAsync<CreateStorageInRequest, BranchStorageHistoryResponse>(
                    $"/branches/{branchId}/storage/in", request, cancellationToken);
                existingByFolio[row.Folio] = createdMovement.Id;
                createdIds.Add(createdMovement.Id);
            }
        }

        foreach (var (branchSeedId, outRequests) in SeedData.StorageOuts)
        {
            var branchId = ResolveId(ctx.Branches, branchSeedId, "branch");
            var existingHistory = await api.ListAllAsync<BranchStorageHistoryResponse>(
                $"/branches/{branchId}/storage/history", cancellationToken);
            var existingByFolio = existingHistory
                .Where(h => !string.IsNullOrWhiteSpace(h.Folio))
                .ToDictionary(h => h.Folio!, h => h.Id, StringComparer.OrdinalIgnoreCase);

            foreach (var row in outRequests)
            {
                if (row.Folio is not null && existingByFolio.TryGetValue(row.Folio, out var existingId))
                {
                    skippedIds.Add(existingId);
                    continue;
                }

                var request = row with
                {
                    Products = row.Products
                        .Select(l => l with { ProductId = ResolveId(ctx.Products, l.ProductId, "product") })
                        .ToList()
                };

                var createdMovement = await api.PostAsync<CreateStorageOutRequest, BranchStorageHistoryResponse>(
                    $"/branches/{branchId}/storage/out", request, cancellationToken);
                if (row.Folio is not null)
                {
                    existingByFolio[row.Folio] = createdMovement.Id;
                }

                createdIds.Add(createdMovement.Id);
            }
        }

        Summary("storage", createdIds, skippedIds);
    }

    private static void Summary(string label, IReadOnlyList<int> createdIds, IReadOnlyList<int> skippedIds)
    {
        var createdPart = createdIds.Count > 0
            ? $"{createdIds.Count} created (ids: [{string.Join(", ", createdIds)}])"
            : "0 created";
        var skippedPart = skippedIds.Count > 0
            ? $"{skippedIds.Count} skipped (ids: [{string.Join(", ", skippedIds)}])"
            : "0 skipped";

        ApiClient.Log($"[{label}] Done -> {createdPart}, {skippedPart}");
    }

    private static Dictionary<int, int> MapBranchIds(IReadOnlyDictionary<string, int> liveIdsByName)
    {
        var map = new Dictionary<int, int>();
        for (var i = 0; i < SeedData.Branches.Count; i++)
        {
            var row = SeedData.Branches[i];
            if (!liveIdsByName.TryGetValue(row.Name, out var liveId))
            {
                throw new ApiException($"Missing branch '{row.Name}' in API response.");
            }

            map[i + 1] = liveId;
        }

        return map;
    }

    private static Dictionary<int, int> MapProductIds(IReadOnlyDictionary<string, int> liveIdsByDescription)
    {
        var map = new Dictionary<int, int>();
        for (var i = 0; i < SeedData.Products.Count; i++)
        {
            var row = SeedData.Products[i];
            if (!liveIdsByDescription.TryGetValue(row.Description, out var liveId))
            {
                throw new ApiException($"Missing product '{row.Description}' in API response.");
            }

            map[i + 1] = liveId;
        }

        return map;
    }

    private static Dictionary<int, int> MapDishIds(IReadOnlyDictionary<string, int> liveIdsByName)
    {
        var map = new Dictionary<int, int>();
        for (var i = 0; i < SeedData.Dishes.Count; i++)
        {
            var row = SeedData.Dishes[i];
            if (!liveIdsByName.TryGetValue(row.Name, out var liveId))
            {
                throw new ApiException($"Missing dish '{row.Name}' in API response.");
            }

            map[i + 1] = liveId;
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
