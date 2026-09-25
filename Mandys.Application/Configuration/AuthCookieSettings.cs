namespace Mandys.Configuration;

/// <summary>
/// Cookie policy for the auth cookies, bound from the "Auth" configuration
/// section. Same-site deployments keep the defaults (Lax + Auto). When the
/// frontend runs on a separate server, browsers only send the cookies
/// cross-site with SameSite=None, which in turn requires Secure — so that
/// setup needs CookieSameSite=None, CookieSecure=Always, and HTTPS.
/// </summary>
public class AuthCookieSettings
{
    public const string SectionName = "Auth";

    /// <summary>Lax (default), None, or Strict.</summary>
    public string CookieSameSite { get; set; } = "Lax";

    /// <summary>Auto (default, Secure on HTTPS) or Always.</summary>
    public string CookieSecure { get; set; } = "Auto";
}
