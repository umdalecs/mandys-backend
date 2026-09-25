namespace Mandys.Configuration;

/// <summary>
/// Public origin(s) of the frontend SPA, bound from the "Frontend"
/// configuration section. The backend may run on a separate server, so the
/// list is explicit: CORS and the CSRF origin check both enforce it.
/// Localhost dev origins apply when nothing is configured.
/// </summary>
public class FrontendOptions
{
    public const string SectionName = "Frontend";

    public static readonly string[] DefaultOrigins =
        ["http://localhost:5173", "http://127.0.0.1:5173"];

    public string[] AllowedOrigins { get; set; } = [];

    /// <summary>
    /// Normalized allow-list: each entry may itself hold several origins
    /// separated by ',' or ';' (e.g. Frontend__AllowedOrigins=https://a,https://b).
    /// Falls back to <see cref="DefaultOrigins"/> when nothing is configured.
    /// </summary>
    public string[] GetAllowedOrigins()
    {
        var origins = AllowedOrigins
            .SelectMany(v => v.Split([',', ';'], StringSplitOptions.RemoveEmptyEntries))
            .Select(NormalizeOrigin)
            .Where(o => !string.IsNullOrEmpty(o))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return origins.Length > 0 ? origins : DefaultOrigins;
    }

    public static string NormalizeOrigin(string? origin)
        => origin?.Trim().TrimEnd('/') ?? string.Empty;

    public static bool IsAllowedOrigin(string? origin, string[] allowedOrigins)
    {
        var normalized = NormalizeOrigin(origin);
        return !string.IsNullOrEmpty(normalized)
            && (allowedOrigins.Contains(normalized, StringComparer.OrdinalIgnoreCase)
                || IsLocalOrigin(normalized));
    }

    /// <summary>
    /// Any localhost origin (any port) is trusted for development, matching
    /// the previous behavior. A remote page can never carry such an origin.
    /// </summary>
    public static bool IsLocalOrigin(string? origin)
    {
        if (string.IsNullOrEmpty(origin)) return false;
        if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri)) return false;
        return uri.Host is "localhost" or "127.0.0.1";
    }
}
