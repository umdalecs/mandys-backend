namespace Mandys.Domain;

public static class Roles
{
    public const string Administrator = "administrador";
    public const string OpChief = "gerenteOperaciones";
    public const string CentralWarehouseChief = "encargadoAlmCentral";
    public const string WarehouseChief = "encargadoAlmacen";
    public const string Cashier = "cajero";
    public const string Customer = "cliente";
    public const string KitchenChief = "jefeCocina";
    public const string BranchChief = "gerenteSucursal";

    public static readonly IReadOnlyList<string> All = [
        Administrator, OpChief, CentralWarehouseChief, WarehouseChief, Cashier, Customer, KitchenChief, BranchChief];

    public static bool IsValid(string? role) =>
        !string.IsNullOrWhiteSpace(role) && All.Contains(role, StringComparer.OrdinalIgnoreCase);

    public static string Normalize(string role)
    {
        if (string.Equals(role, Administrator, StringComparison.OrdinalIgnoreCase)) return Administrator;
        if (string.Equals(role, OpChief, StringComparison.OrdinalIgnoreCase)) return OpChief;
        if (string.Equals(role, CentralWarehouseChief, StringComparison.OrdinalIgnoreCase)) return CentralWarehouseChief;
        if (string.Equals(role, WarehouseChief, StringComparison.OrdinalIgnoreCase)) return WarehouseChief;
        if (string.Equals(role, Cashier, StringComparison.OrdinalIgnoreCase)) return Cashier;
        if (string.Equals(role, Customer, StringComparison.OrdinalIgnoreCase)) return Customer;
        if (string.Equals(role, KitchenChief, StringComparison.OrdinalIgnoreCase)) return KitchenChief;
        if (string.Equals(role, BranchChief, StringComparison.OrdinalIgnoreCase)) return BranchChief;
        return role;
    }

    /// <summary>
    /// Whether the role is tied to a single branch. Administrators and
    /// customers can use any branch at any time, so the branch is optional
    /// for them and required for every other role.
    /// </summary>
    public static bool RequiresBranch(string? role) =>
        !string.Equals(role, Administrator, StringComparison.OrdinalIgnoreCase) &&
        !string.Equals(role, Customer, StringComparison.OrdinalIgnoreCase);
}
