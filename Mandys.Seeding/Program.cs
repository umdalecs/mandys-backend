using Mandys.Seeding;

const string BaseUrl = "http://localhost:8080";
const string AdminEmail = "admin@mandyspos.com";
const string AdminPassword = "mandyspos";

using var api = new ApiClient(BaseUrl);
var ctx = new SeedContext();

try
{
    ApiClient.Log($"Starting Mandys seeder against {BaseUrl}...");
    await api.LoginAsync(AdminEmail, AdminPassword);
    ApiClient.Log($"[auth] Authenticated as {AdminEmail}");

    await Seeders.SeedBranchesAsync(api, ctx);
    await Seeders.SeedUsersAsync(api, ctx);
    await Seeders.SeedProductsAsync(api, ctx);
    await Seeders.SeedStorageMovementsAsync(api, ctx);
    await Seeders.SeedDishesAsync(api, ctx);
    await Seeders.SeedCombosAsync(api, ctx);

    ApiClient.Log("Seeding completed successfully.");
}
catch (ApiException ex)
{
    Console.Error.WriteLine($"ERROR: {ex.Message}");
    return 1;
}

return 0;
