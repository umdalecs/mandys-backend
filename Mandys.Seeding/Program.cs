using Mandys.Seeding;

const string DefaultBaseUrl = "http://localhost:8080";
const string AdminEmail = "admin@mandyspos.com";
const string AdminPassword = "mandyspos";

var baseUrl = ParseBaseUrl(args);

if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var parsedUri)
    || (parsedUri.Scheme != Uri.UriSchemeHttp && parsedUri.Scheme != Uri.UriSchemeHttps))
{
    Console.Error.WriteLine($"ERROR: Invalid backend URL '{baseUrl}'. Expected an http:// or https:// URL.");
    return 1;
}

using var api = new ApiClient(baseUrl);
var ctx = new SeedContext();

try
{
    ApiClient.Log($"Starting Mandys seeder against {baseUrl}...");
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

static string ParseBaseUrl(string[] args)
{
    for (var i = 0; i < args.Length; i++)
    {
        var arg = args[i];
        if ((arg is "--url" or "-u") && i + 1 < args.Length)
        {
            return args[i + 1];
        }

        if (arg.StartsWith("--url=", StringComparison.OrdinalIgnoreCase))
        {
            return arg["--url=".Length..];
        }
    }

    return args.Length > 0 && !string.IsNullOrWhiteSpace(args[0])
        ? args[0]
        : DefaultBaseUrl;
}
