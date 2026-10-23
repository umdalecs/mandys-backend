namespace Mandys.Domain;

public static class MeasureUnits
{
    public const string Grams = "Gramos";
    public const string Milliliters = "Mililitros";
    public const string Units = "Unidades";

    public static readonly IReadOnlyList<string> All = [Grams, Milliliters, Units];

    public static bool IsValid(string? unit) =>
        !string.IsNullOrWhiteSpace(unit) && All.Contains(unit.Trim(), StringComparer.OrdinalIgnoreCase);

    public static string Normalize(string unit)
    {
        var trimmed = unit.Trim();
        if (string.Equals(trimmed, Grams, StringComparison.OrdinalIgnoreCase)) return Grams;
        if (string.Equals(trimmed, Milliliters, StringComparison.OrdinalIgnoreCase)) return Milliliters;
        if (string.Equals(trimmed, Units, StringComparison.OrdinalIgnoreCase)) return Units;
        return trimmed;
    }

    /// <summary>
    /// Whether quantities of this unit can have fractional decimals.
    /// Gramos and Mililitros allow decimals; Unidades requires whole numbers.
    /// </summary>
    public static bool AllowsDecimals(string unit) =>
        !string.Equals(unit.Trim(), Units, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Whether this measure unit is valid for a product given its IsSupply flag.
    /// Sellable products (IsSupply = false) can only use Unidades.
    /// </summary>
    public static bool IsAllowedForProduct(string unit, bool isSupply) =>
        isSupply || string.Equals(unit.Trim(), Units, StringComparison.OrdinalIgnoreCase);

    public static bool IsValidQuantity(string unit, decimal quantity) =>
        AllowsDecimals(unit) || decimal.IsInteger(quantity);
}
