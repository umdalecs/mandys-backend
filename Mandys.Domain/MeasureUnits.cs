namespace Mandys.Domain;

public static class MeasureUnits
{
    public const string Kilograms = "Kilogramos";
    public const string Liters = "Litros";
    public const string Units = "Unidades";

    public static readonly IReadOnlyList<string> All = [Kilograms, Liters, Units];

    public static bool IsValid(string? unit) =>
        !string.IsNullOrWhiteSpace(unit) && All.Contains(unit.Trim(), StringComparer.OrdinalIgnoreCase);

    public static string Normalize(string unit)
    {
        var trimmed = unit.Trim();
        if (string.Equals(trimmed, Kilograms, StringComparison.OrdinalIgnoreCase)) return Kilograms;
        if (string.Equals(trimmed, Liters, StringComparison.OrdinalIgnoreCase)) return Liters;
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
