namespace Mandys.Application.Configuration;

/// <summary>
/// Loyalty points settings bound from the "Loyalty" configuration section
/// (appsettings.json or environment variables such as
/// <c>Loyalty__PointsPercentage</c>).
/// </summary>
public class LoyaltyOptions
{
    public const string SectionName = "Loyalty";

    /// <summary>
    /// Percentage of the order total awarded as customer points (e.g. 3 = 3%).
    /// Defaults to 3.
    /// </summary>
    public decimal PointsPercentage { get; set; } = 3m;
}
