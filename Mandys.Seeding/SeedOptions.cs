namespace Mandys.Seeding;

public sealed record SeedOptions(
    string BaseUrl,
    string Email,
    string Password,
    string UserPassword,
    IReadOnlyList<string> Steps,
    bool ShowHelp = false
)
{
    public static readonly IReadOnlyList<string> AllSteps =
    [
        "branches",
        "users",
        "products",
        "dishes",
        "combos"
    ];

    public static SeedOptions Parse(string[] args)
    {
        var baseUrl = GetEnvOrDefault("SEED_BASE_URL", "http://localhost:8080");
        var email = GetEnvOrDefault("SEED_ADMIN_EMAIL", "admin@mandyspos.com");
        var password = GetEnvOrDefault("SEED_ADMIN_PASSWORD", "mandyspos");
        var userPassword = GetEnvOrDefault("SEED_USER_PASSWORD", "mandyspos");
        var onlySteps = new List<string>();
        var showHelp = false;

        for (var i = 0; i < args.Length; i++)
        {
            var arg = args[i];
            switch (arg)
            {
                case "-h":
                case "--help":
                    showHelp = true;
                    break;

                case "--base-url":
                    baseUrl = RequireNextValue(args, ref i, "--base-url");
                    break;

                case "--email":
                    email = RequireNextValue(args, ref i, "--email");
                    break;

                case "--password":
                    password = RequireNextValue(args, ref i, "--password");
                    break;

                case "--user-password":
                    userPassword = RequireNextValue(args, ref i, "--user-password");
                    break;

                case "--only":
                    while (i + 1 < args.Length && !args[i + 1].StartsWith('-'))
                    {
                        i++;
                        var step = args[i];
                        if (!AllSteps.Contains(step, StringComparer.Ordinal))
                        {
                            throw new ArgumentException(
                                $"Invalid step '{step}' for --only. Allowed values: {string.Join(", ", AllSteps)}");
                        }

                        onlySteps.Add(step);
                    }

                    if (onlySteps.Count == 0)
                    {
                        throw new ArgumentException("--only requires at least one step name.");
                    }

                    break;

                default:
                    throw new ArgumentException($"Unrecognized argument: {arg}");
            }
        }

        IReadOnlyList<string> steps = onlySteps.Count > 0 ? onlySteps : AllSteps;
        return new SeedOptions(baseUrl, email, password, userPassword, steps, showHelp);
    }

    public static void PrintHelp(TextWriter writer)
    {
        writer.WriteLine("Seed catalog data (branches, staff users, products, dishes, combos) via the Mandys HTTP API.");
        writer.WriteLine();
        writer.WriteLine("Options:");
        writer.WriteLine("  --base-url <url>           API base URL (env: SEED_BASE_URL, default: http://localhost:8080)");
        writer.WriteLine("  --email <email>            Admin email (env: SEED_ADMIN_EMAIL, default: admin@mandyspos.com)");
        writer.WriteLine("  --password <password>      Admin password (env: SEED_ADMIN_PASSWORD, default: mandyspos)");
        writer.WriteLine("  --user-password <password> Password given to seeded staff users (env: SEED_USER_PASSWORD, default: mandyspos)");
        writer.WriteLine("  --only <steps...>          Run only these steps (branches, users, products, dishes, combos)");
        writer.WriteLine("  -h, --help                 Show this help message");
    }

    private static string GetEnvOrDefault(string name, string fallback)
    {
        var value = Environment.GetEnvironmentVariable(name);
        return string.IsNullOrWhiteSpace(value) ? fallback : value;
    }

    private static string RequireNextValue(string[] args, ref int index, string optionName)
    {
        if (index + 1 >= args.Length)
        {
            throw new ArgumentException($"Option '{optionName}' requires a value.");
        }

        index++;
        return args[index];
    }
}
