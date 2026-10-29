using Mandys.Seeding;

SeedOptions options;
try
{
    options = SeedOptions.Parse(args);
}
catch (ArgumentException ex)
{
    Console.Error.WriteLine($"ERROR: {ex.Message}");
    SeedOptions.PrintHelp(Console.Error);
    return 1;
}

if (options.ShowHelp)
{
    SeedOptions.PrintHelp(Console.Out);
    return 0;
}

using var api = new ApiClient(options.BaseUrl);
var ctx = new SeedContext();
var steps = options.Steps.ToHashSet(StringComparer.Ordinal);

try
{
    await api.LoginAsync(options.Email, options.Password);
    await Seeders.LoadMissingDependenciesAsync(api, ctx, options.Steps);

    if (steps.Contains("branches"))
    {
        await Seeders.SeedBranchesAsync(api, ctx);
    }

    if (steps.Contains("users"))
    {
        await Seeders.SeedUsersAsync(api, ctx, options.UserPassword);
    }

    if (steps.Contains("products"))
    {
        await Seeders.SeedProductsAsync(api, ctx);
    }

    if (steps.Contains("dishes"))
    {
        await Seeders.SeedDishesAsync(api, ctx);
    }

    if (steps.Contains("combos"))
    {
        await Seeders.SeedCombosAsync(api, ctx);
    }
}
catch (ApiException ex)
{
    Console.Error.WriteLine($"ERROR: {ex.Message}");
    return 1;
}

return 0;
