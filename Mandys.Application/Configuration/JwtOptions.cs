namespace Mandys.Configuration;

/// <summary>
/// JWT settings consumed by Application and Infrastructure; bound from the
/// "Jwt" configuration section by the Api composition root.
/// </summary>
public class JwtOptions
{
    public const string SectionName = "Jwt";
    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = "Mandys";
    public string Audience { get; set; } = "Mandys";
    public int ExpireMinutes { get; set; } = 10;
    public int RefreshTokenExpireMinutes { get; set; } = 60;
}
